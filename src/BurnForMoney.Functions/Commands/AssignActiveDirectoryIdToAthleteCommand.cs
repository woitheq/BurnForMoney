using System;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.Commands
{
    public class AssignActiveDirectoryIdToAthleteCommand : Command
    {
        public readonly Guid AthleteId;
        public readonly Guid ActiveDirectoryId;

        public AssignActiveDirectoryIdToAthleteCommand(Guid athleteId, Guid activeDirectoryId)
        {
            AthleteId = athleteId;
            ActiveDirectoryId = activeDirectoryId;
        }
    }
}