﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Strava.Commands;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete.Dto;
using BurnForMoney.Functions.Strava.Security;
using BurnForMoney.Identity;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();
        
        [FunctionName(FunctionsNames.A_GenerateAthleteId)]
        public static async Task<Guid> A_GenerateAthleteId([ActivityTrigger] object input)
        {
            var id = await Task.Run(() => AthleteIdentity.Next());
            return id;
        }

        [FunctionName(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary)]
        public static async Task<AthleteDto> A_ExchangeTokenAndGetAthleteSummaryAsync([ActivityTrigger]ExchangeTokenAndGetAthleteSummaryInput input, ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");
            var response = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, input.AuthorizationCode);

            var athleteReadRepository = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athleteExists = await athleteReadRepository.AthleteWithStravaIdExistsAsync(response.Athlete.Id.ToString());
            if (athleteExists)
            {
                throw new AthleteAlreadyExistsException(response.Athlete.Id.ToString());
            }

            try
            {
                await AccessTokensStore.AddAsync(input.AthleteId, response.AccessToken, response.RefreshToken, response.ExpiresAt,
                    configuration.Strava.AccessTokensKeyVaultUrl);

                log.LogInformation(nameof(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary), $"Updated tokens for athlete with id: {configuration.Strava.ClientId}.");
            }
            catch (Exception ex)
            {
                throw new FailedToAddAccessTokenException(response.Athlete.Id.ToString(), ex);
            }

            return new AthleteDto
            {
                Id = input.AthleteId,
                ExternalId = response.Athlete.Id.ToString(),
                FirstName = response.Athlete.Firstname,
                LastName = response.Athlete.Lastname,
                ProfilePictureUrl = response.Athlete.Profile
            };
        }

        [FunctionName(FunctionsNames.A_SendAthleteApprovalRequest)]
        public static async Task A_SendAthleteApprovalRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            [Queue(AppQueueNames.NotificationsToSend, Connection = "AppQueuesStorage")] CloudQueue notificationsQueue,
            [Table("AthleteApprovals", "AzureWebJobsStorage")] IAsyncCollector<AthleteApproval> athleteApprovalCollector,
            [Configuration] ConfigurationRoot configuration)
        {
            var (firstName, lastName) = activityContext.GetInput<(string, string)>();

            var approvalCode = Guid.NewGuid().ToString("N");
            var athleteApproval = new AthleteApproval
            {
                PartitionKey = "AthleteApproval",
                RowKey = approvalCode,
                OrchestrationId = activityContext.InstanceId
            };

            var approvalFunctionAddress = $"{configuration.HostName}/api/SubmitAthleteApproval/{approvalCode}";
            var notification = new Notification
            {
                Recipients = new List<string> { configuration.Email.AthletesApprovalEmail },
                Subject = "Athlete is awaiting approval",
                HtmlContent = $@"
<p>Hi there,</p>
<p>Please review a new authorization request. Athlete: {firstName} {lastName}.</p>" +
                          $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Approved.ToString()}\">Approve</a><br>" +
                          $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Rejected.ToString()}\">Reject</a>"
            };

            log.LogInformation(FunctionsNames.A_SendAthleteApprovalRequest, $"Sending approval request for athlete {firstName} {lastName} to: {configuration.Email.AthletesApprovalEmail}.");
            await athleteApprovalCollector.AddAsync(athleteApproval);
            await notificationsQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(notification)));
        }

        [FunctionName(FunctionsNames.A_ProcessNewAthleteRequest)]
        public static async Task A_ProcessNewAthleteRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [Queue(AppQueueNames.AddAthleteRequests, Connection = "AppQueuesStorage")] CloudQueue addAthleteRequestsQueue)
        {
            var athlete = activityContext.GetInput<AthleteDto>();

            var command = new CreateAthleteCommand(athlete.Id, athlete.ExternalId, athlete.FirstName, athlete.LastName,
                athlete.ProfilePictureUrl, Source.Strava);
            var json = JsonConvert.SerializeObject(command);
            await addAthleteRequestsQueue.AddMessageAsync(new CloudQueueMessage(json));
        }

        [FunctionName(FunctionsNames.A_AuthorizeNewAthleteCompensation)]
        public static async Task A_AuthorizeNewAthleteCompensation([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [Queue(StravaQueueNames.AuthorizationCodesPoison)] CloudQueue authorizationCodePoisonQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            var input = activityContext.GetInput<AuthorizeNewAthleteCompensation>();
            var json = JsonConvert.SerializeObject(input);
            await authorizationCodePoisonQueue.AddMessageAsync(new CloudQueueMessage(json));

            try
            {
                await AccessTokensStore.DeleteAsync(input.AthleteId, configuration.Strava.AccessTokensKeyVaultUrl);
            }
            catch (KeyVaultErrorException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // ignored, it may not be added
            }
        }
    }

    internal enum AthleteApprovalResult
    {
        Approved,
        Rejected
    }

    public class AthleteApproval
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
    }

    public class ExchangeTokenAndGetAthleteSummaryInput
    {
        public Guid AthleteId { get; set; }
        public string AuthorizationCode { get; set; }

        public ExchangeTokenAndGetAthleteSummaryInput(Guid athleteId, string authorizationCode)
        {
            AthleteId = athleteId;
            AuthorizationCode = authorizationCode;
        }
    }

    public class AthleteEntity : TableEntity
    {
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool Active { get; set; } = true;
    }
}
