﻿namespace BurnForMoney.Functions.Helpers
{
    public static class FunctionsNames
    {
        public const string AuthorizeStravaUser = "AuthorizeStravaUser";

        public const string CollectStravaActivitiesInEvery20Minutes = "CollectStravaActivitiesInEvery20Minutes";
        public const string O_CollectStravaActivities = "O_CollectStravaActivities";
        public const string A_GetAccessTokens = "A_GetAccessTokens";
        public const string A_SaveSingleUserActivities = "A_SaveSingleUserActivities";
        public const string A_GetLastActivitiesUpdateDate = "A_GetLastActivitiesUpdateDate";
        public const string A_SetLastActivitiesUpdateDate = "A_SetLastActivitiesUpdateDate";
        public const string A_EncryptAccessToken = "A_EncryptAccessToken";
        public const string A_DecryptAccessToken = "A_DecryptAccessToken";


        public const string Support_EncryptString = "Support_EncryptString";
        public const string Support_DecryptString = "Support_DecryptString";
        public const string Support_InitializeDatabase = "Support_InitializeDatabase";
        public const string Support_Strava_CollectActivities = "Support_Strava_CollectActivities";
        public const string Support_Strava_DeactivateAthlete = "Support_Strava_DeactivateAthlete";
        public const string Support_Strava_ActivateAthlete = "Support_Strava_ActivateAthlete";
        
        public const string AuthorizeNewAthleteStarter = "AuthorizeNewAthleteStarter";
        public const string O_AuthorizeNewAthlete = "O_AuthorizeNewAthlete";
        public const string A_GenerateAccessToken = "A_GenerateAccessToken";
        public const string A_AddAthleteToDatabase = "A_AddAthleteToDatabase";

        public const string CalculateMonthlyAthleteResultsOnFirstDayOfTheMonth = "CalculateMonthlyAthleteResultsOnFirstDayOfTheMonth";
        public const string O_CalculateMonthlyAthleteResults = "O_CalculateMonthlyAthleteResults";
        public const string A_GetLastMonthActivities = "A_GetLastMonthActivities";
        public const string A_StoreAggregatedAthleteResults = "A_StoreAggregatedAthleteResults";
    }
}