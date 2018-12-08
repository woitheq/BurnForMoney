﻿using System.Threading.Tasks;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Infrastructure.Events;
using Dapper;

namespace BurnForMoney.Functions.ReadModel
{
    public class AthleteView : IHandles<AthleteCreated>
    {
        private readonly string _sqlConnectionString;

        public AthleteView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task HandleAsync(AthleteCreated message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var affectedRows = await conn.ExecuteAsync(
                    @"INSERT INTO dbo.Athletes (Id, ExternalId, FirstName, LastName, ProfilePictureUrl, Active, System)
VALUES (@Id, @ExternalId, @FirstName, @LastName, @ProfilePictureUrl, @Active, @System)", new
                    {
                        message.Id,
                        message.ExternalId,
                        message.FirstName,
                        message.LastName,
                        message.ProfilePictureUrl,
                        Active = true,
                        System = message.System.ToString()
                    });
                if (affectedRows != 1)
                {
                    throw new FailedToAddAthleteException(message.Id.ToString());
                }
            }
        }
    }
}