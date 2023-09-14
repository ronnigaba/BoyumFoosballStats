using System;
using System.Collections.Generic;
using System.Linq;
using BoyumFoosballStats_Ai;
using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Xunit;

namespace BoyumFoosballStats.Test;

public class CollectionCombinationTesting
{
    [Fact]
    public async void TestCombinations()
    {
        var eloController = new EloController();
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        var teamStats = new TeamStatisticsController().CalculateTeamStats(matches);
        var players = new List<Player>() { Player.Ronni, Player.Jeppe, Player.Alex, Player.Peter };
        var combinations = CollectionCombinationHelper.GetAllCombinations(players, 2).ToList();
        var validMatchFairness = new Dictionary<Match, decimal>();
        var validMatchFairnessAi = new Dictionary<Match, float>();

        foreach (var comb1 in combinations)
        {
            foreach (var comb2 in combinations)
            {
                var match = new Match
                {
                    Black = new Team(TableSide.Black) { Defender = comb1.First(), Attacker = comb1.Last() },
                    Gray = new Team(TableSide.Black) { Defender = comb2.First(), Attacker = comb2.Last() }
                };
                var grayElo =
                    teamStats?.SingleOrDefault(x => x.TeamIdentifier == match.Gray.TeamIdentifier)?.EloRating ?? 0;
                var blackElo = teamStats?.SingleOrDefault(x => x.TeamIdentifier == match.Black.TeamIdentifier)
                    ?.EloRating ?? 0;
                if (match.IsValid())
                {
                    var prediction = eloController.PredictResult(grayElo, blackElo);
                    validMatchFairness.Add(match, Math.Abs(prediction[0] - prediction[1]));

                    var result = await AiPredictionController.Predict(match);
                    validMatchFairnessAi.Add(match, result?.WinChanceBlack ?? 0);
                }
            }
        }

        var fairestMatch = validMatchFairness.FirstOrDefault(x => x.Value == validMatchFairness.Min(v => v.Value));
        var fairestMatchX = validMatchFairnessAi.MinBy(kv => Math.Abs(kv.Value - 50));
        var test = 0;
    }
}