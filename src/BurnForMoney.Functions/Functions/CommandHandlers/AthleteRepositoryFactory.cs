﻿using BurnForMoney.Domain;
using BurnForMoney.Domain.Domain;
using BurnForMoney.Functions.Configuration;

namespace BurnForMoney.Functions.Functions.CommandHandlers
{
    public static class AthleteRepositoryFactory
    {
        public static IRepository<Athlete> Create()
        {
            var configuration = ApplicationConfiguration.GetSettings();

            var repository = new Repository<Athlete>(EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage,
                new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint)));

            return repository;
        }
    }
}