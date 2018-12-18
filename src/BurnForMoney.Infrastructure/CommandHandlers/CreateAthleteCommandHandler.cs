﻿using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Domain;

namespace BurnForMoney.Infrastructure.CommandHandlers
{
    public class CreateAthleteCommandHandler : ICommandHandler<CreateAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;

        public CreateAthleteCommandHandler(IRepository<Athlete> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(CreateAthleteCommand message)
        {
            var athlete = new Athlete(message.Id, message.ExternalId, message.FirstName, message.LastName,
                message.ProfilePictureUrl, message.System);
            await _repository.SaveAsync(athlete, 0);
        }
    }
}