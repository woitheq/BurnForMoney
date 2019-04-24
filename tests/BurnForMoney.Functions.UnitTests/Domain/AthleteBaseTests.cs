using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Messages;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.UnitTests.Domain
{
    public abstract  class AthleteBaseTests
    {
        
        protected readonly IAccountsStore _accountsStore = new MemoryAccountsStore();
        protected readonly IRepository<Athlete> _athleteRepo = new Repository<Athlete>(new MemoryEventStore());

        protected async Task<Athlete> GetAthleteAsync(Guid id) =>  await _athleteRepo.GetByIdAsync(id);

        protected async Task<Guid> CreateNewAthleteAsync(string firstName = "test_first_name", string lastName = "test_last_name")
        {
            var newAthleteId = Guid.NewGuid();
            var aadId = Guid.NewGuid();
            await HandleCommand(new CreateAthleteCommand(newAthleteId, aadId, firstName, lastName));

            return newAthleteId;
        }

        protected async Task HandleCommand<T>(T command) where T : Command
        {
            switch(command)
            {
                case CreateAthleteCommand cmd:
                    await new CreateAthleteCommandHandler(_athleteRepo, _accountsStore).HandleAsync(cmd);
                    break;
                case ActivateAthleteCommand cmd:
                    await new ActivateAthleteCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                case DeactivateAthleteCommand cmd:
                    await new DeactivateAthleteCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                case AddActivityCommand cmd:
                    await new AddActivityCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                case UpdateActivityCommand cmd:
                    await new UpdateActivityCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                case DeleteActivityCommand cmd:
                    await new DeleteActivityCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                case AddStravaAccountCommand cmd:
                    await new AddStravaAccountCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                case AssignActiveDirectoryIdToAthleteCommand cmd:
                    await new AssignActiveDirectoryIdToAthleteCommandHandler(_athleteRepo).HandleAsync(cmd);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
