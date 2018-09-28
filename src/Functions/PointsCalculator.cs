﻿using System;
using BurnForMoney.Functions.Model;

namespace BurnForMoney.Functions
{
    public class PointsCalculator
    {
        public double Calculate(ActivityCategory category, double distanceInMeters, int timeInMinutes)
        {
            double points;

            switch (category)
            {
                case ActivityCategory.Run:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters, 2); // 1km = 2 points
                    break;
                case ActivityCategory.Ride:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters); // 1km = 1 point
                    break;
                case ActivityCategory.Walk:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters);
                    break;
                case ActivityCategory.WinterSports:
                    points = GetPointsForTimeBasedCategory(timeInMinutes); // 10min = 1 point
                    break;
                case ActivityCategory.WaterSports:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.TeamSports:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.Gym:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                case ActivityCategory.Hike:
                    points = GetPointsForDistanceBasedCategory(distanceInMeters);
                    break;
                case ActivityCategory.Fitness:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
                default:
                    points = GetPointsForTimeBasedCategory(timeInMinutes);
                    break;
            }

            return Math.Round(points, 2);
        }

        private static double GetPointsForDistanceBasedCategory(double distanceInMeters, double factor = 1)
        {
            return distanceInMeters * factor / 1000; 
        }

        private static double GetPointsForTimeBasedCategory(int timeInMinutes, double factor = 1)
        {
            return timeInMinutes * factor / 10;
        }
    }
}