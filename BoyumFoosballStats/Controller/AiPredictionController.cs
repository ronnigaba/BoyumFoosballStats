using BoyumFoosballStats_Ai;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Controller;

public static class AiPredictionController
{
    public static async Task<AiMatchPrediction?> Predict(Match match)
    {
        if (!match.IsValid())
        {
            return null;
        }
        var sampleData = new MatchOutcomeModel.ModelInput
        {
            GrayDefender = (float)match.Gray.Defender,
            GrayAttacker = (float)match.Gray.Attacker,
            BlackDefender = (float)match.Black.Defender,
            BlackAttacker = (float)match.Black.Attacker,
        };
        var outcomeModel = new MatchOutcomeModel();
        var result = await outcomeModel.Predict(sampleData);
        var blackChance = 100 - result.Score * 100;
        var grayChance = 0 + result.Score * 100;

        return new AiMatchPrediction
        {
            WinChanceBlack = blackChance,
            WinChanceGray = grayChance,
            WinningTeam = blackChance > grayChance ? match.Black : match.Gray,
            WinningSide = blackChance > grayChance ? TableSide.Black : TableSide.Gray
        };
    }
}