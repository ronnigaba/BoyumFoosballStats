using System.Collections.Generic;
using System.Linq;
using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Moserware.Skills;
using Xunit;
using Player = Moserware.Skills.Player;
using Team = Moserware.Skills.Team;

namespace BoyumFoosballStats.Test;

public class TrueSkillTesting
{
    [Fact]
    public async void Test_TrueSkill()
    {
        var ratings = new Dictionary<Model.Enums.Player?, Rating>();
        var gameInfo = GameInfo.DefaultGameInfo;
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);

        foreach (var match in matches)
        {
            var blackAttacker = new Player(match.Black.Attacker);
            var blackDefender = new Player(match.Black.Defender);
            var grayAttacker = new Player(match.Gray.Attacker);
            var grayDefender = new Player(match.Gray.Defender);

            var blackAttackerRating = ratings.TryGetValue(match.Black.Defender, out var rating) ? rating : gameInfo.DefaultRating;
            var blackDefenderRating = ratings.TryGetValue(match.Black.Defender, out var rating1) ? rating1 : gameInfo.DefaultRating;
            var grayAttackerRating = ratings.TryGetValue(match.Gray.Attacker, out var rating2) ? rating2 : gameInfo.DefaultRating;
            var grayDefenderRating = ratings.TryGetValue(match.Gray.Defender, out var rating3) ? rating3 : gameInfo.DefaultRating;
            
            var blackTeam = new Team().AddPlayer(blackAttacker, blackAttackerRating)
                .AddPlayer(blackDefender, blackDefenderRating);
            
            var grayTeam = new Team().AddPlayer(grayAttacker, grayAttackerRating)
                .AddPlayer(grayDefender, grayDefenderRating);

            var teams = Teams.Concat(blackTeam, grayTeam);
            var blackRank = match.WinningTeam.Side == TableSide.Black ? 1 : 2;
            var grayRank = match.WinningTeam.Side == TableSide.Gray ? 1 : 2;
            var newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, blackRank, grayRank);
            
            ratings[match.Black.Attacker] = newRatings[blackAttacker];
            ratings[match.Black.Defender] = newRatings[blackDefender];
            ratings[match.Gray.Attacker] = newRatings[grayAttacker];
            ratings[match.Gray.Defender] = newRatings[grayDefender];
        }

        var tiers = new Dictionary<Model.Enums.Player?, string>();
        var test = ratings.OrderBy(x=>x.Value.Mean);
        foreach (var rating in test)
        {
            var meanRating = rating.Value.Mean * 100;
            if (meanRating > 3000)
            {
                tiers.Add(rating.Key, "D1");
            }
            else if (meanRating > 2900)
            {
                tiers.Add(rating.Key, "D2");
            }
            else if (meanRating > 2800)
            {
                tiers.Add(rating.Key, "D3");
            }
            else if (meanRating > 2700)
            {
                tiers.Add(rating.Key, "D4");
            }
            else if (meanRating > 2600)
            {
                tiers.Add(rating.Key, "D5");
            }
            else if (meanRating > 2500)
            {
                tiers.Add(rating.Key, "G");
            }
            else if (meanRating > 2000)
            {
                tiers.Add(rating.Key, "S");
            }
            else if (meanRating > 1500)
            {
                tiers.Add(rating.Key, "B");
            }
            else
            {
                tiers.Add(rating.Key, "I");
            }
        }

        var test2 = 0;
    }
}