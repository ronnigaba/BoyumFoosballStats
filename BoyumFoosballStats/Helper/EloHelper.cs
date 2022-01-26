﻿namespace BoyumFoosballStats.Helper;

public class EloHelper
{
    private const int KFACTOR = 15;
    public const double WIN = 1.0;
    public const double LOSE = 0.0;
    public const double DRAW = 0.5;

    public static Decimal[] CalculateElo(
        Decimal currentRatingA,
        Decimal currentRatingB,
        Decimal scoreA,
        Decimal scoreB,
        int kFactorA = 15,
        int kFactorB = 15)
    {
        Decimal[] numArray = EloHelper.PredictResult(currentRatingA, currentRatingB);
        return new Decimal[2]
        {
            currentRatingA + (Decimal) kFactorA * (scoreA - numArray[0]),
            currentRatingB + (Decimal) kFactorB * (scoreB - numArray[1])
        };
    }

    public static Decimal[] PredictResult(Decimal currentRatingA, Decimal currentRatingB) => new Decimal[2]
    {
        1M / (1M + (Decimal) Math.Pow(10.0, (double) (currentRatingB - currentRatingA) / 400.0)) * 100,
        1M / (1M + (Decimal) Math.Pow(10.0, (double) (currentRatingA - currentRatingB) / 400.0)) * 100
    };
}