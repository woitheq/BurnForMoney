﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Helpers;
using BurnForMoney.Functions.Strava.Api;
using BurnForMoney.Functions.Strava.Api.Model;
using BurnForMoney.Functions.Strava.Services;
using Dapper;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Strava
{
    public static class CollectActivitiesActivities
    {
        private static readonly IPointsCalculatingStrategy PointsCalculator = new DefaultPointsCalculatingStrategy();

        [FunctionName(FunctionsNames.A_GetAccessTokens)]
        public static async Task<string[]> GetAccessTokensAsync([ActivityTrigger]DurableActivityContext activityContext, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_GetAccessTokens} function processed a request. Instance id: `{activityContext.InstanceId}`");

            var accessTokens = await GetAllActiveAccessTokensAsync(log, executionContext);
            log.LogInformation($"Received information about {accessTokens.Count} active access tokens.");

            return accessTokens.ToArray();
        }

        public static async Task<List<string>> GetAllActiveAccessTokensAsync(ILogger log, ExecutionContext executionContext)
        {
            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            var keyVaultClient = KeyVaultClientFactory.Create();
            var secret = await keyVaultClient.GetSecretAsync(
                configuration.ConnectionStrings.KeyVaultConnectionString,
                KeyVaultSecretNames.StravaTokensEncryptionKey);
            var accessTokenEncryptionKey = secret.Value;

            var encryptionService = new AccessTokensEncryptionService(log, accessTokenEncryptionKey);

            IEnumerable<string> tokens;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                tokens = await conn.QueryAsync<string>("SELECT AccessToken FROM dbo.[Strava.Athletes] where Active = 1").ConfigureAwait(false);
            }

            return tokens.Select(encryptionService.DecryptAccessToken).ToList();
        }

        [FunctionName(FunctionsNames.A_SaveSingleUserActivities)]
        public static async Task SaveSingleUserActivitiesAsync([ActivityTrigger]DurableActivityContext context, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"{FunctionsNames.A_SaveSingleUserActivities} function processed a request.");

            var (accessToken, lastUpdate) = context.GetInput<ValueTuple<string, DateTime?>>();

            var configuration = ApplicationConfiguration.GetSettings(executionContext);
            var stravaService = new StravaService();
            var activities = stravaService.GetActivitiesFrom(accessToken, lastUpdate ?? DateTime.UtcNow.AddMonths(-3));

            foreach (var activity in activities)
            {
                await SaveActivityAsync(activity, configuration.ConnectionStrings.SqlDbConnectionString, log);
            }
        }

        private static async Task SaveActivityAsync(StravaActivity activity, string connectionString, ILogger log)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var activityCategory = StravaActivityMapper.MapToActivityCategory(activity.Type);
                var points = PointsCalculator.Calculate(activityCategory, activity.Distance, activity.MovingTime);

                var affectedRows = await conn.ExecuteAsync("Strava_Activity_Insert",
                        new
                        {
                            AthleteId = activity.Athlete.Id,
                            ActivityId = activity.Id,
                            ActivityTime = activity.StartDate,
                            ActivityType = activity.Type.ToString(),
                            Distance = activity.Distance,
                            MovingTime = activity.MovingTime,
                            Category = activityCategory.ToString(),
                            Points = points
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                if (affectedRows > 0)
                {
                    log.LogInformation($"Activity with id: {activity.Id} has been added.");
                }
            }
        }
    }
}
