﻿using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Commands;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;

namespace BurnForMoney.Functions.CommandHandlers
{
    public class AddActivityCommandHandler : ICommandHandler<AddActivityCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public AddActivityCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(AddActivityCommand message)
        {
            var athlete = await _repository.GetByIdAsync(message.AthleteId);

            athlete.AddActivity(message.Id, message.ExternalId, message.ActivityType, message.DistanceInMeters,
                message.MovingTimeInMinutes, message.StartDate, message.Source);
            await _repository.SaveAsync(athlete, athlete.OriginalVersion);
        }
    }
}