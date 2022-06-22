using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Controller;

public class PlayerStatisticsController
{
    //Experimental / WIP :p
    public void CalculatePlayerRatingUsingAlgorhitm(List<Match> matches, List<TeamStatistics>? teamStats = null)
    {
        if (teamStats == null)
        {
            teamStats = new TeamStatisticsController().CalculateTeamStats(matches);
        }

        Dictionary<string, double> defenderRatings = new();
        Dictionary<string, double> attackerRatings = new();
        foreach (var player in Enum.GetNames<Player>())
        {
            var defRating = teamStats.Where(x => x.Defender.Contains(player)).Sum(x =>
                x.WinRate / (x.GoalsAgainstPerMatch > 0 ? x.GoalsAgainstPerMatch : 1) * ((double)x.MatchesPlayed /
                    matches.Count(y => y.WasPlayerDefenderInMatch(Enum.Parse<Player>(player)))));
            var atkRating = teamStats.Where(x => x.Attacker.Contains(player)).Sum(x =>
                (double)(x.WinRate * (x.GoalsScoredPerMatch > 0 ? x.GoalsScoredPerMatch : 1)) *
                ((double)x.MatchesPlayed / matches.Count(y => y.WasPlayerAttackerInMatch(Enum.Parse<Player>(player)))));
            defenderRatings.Add(player, defRating);
            attackerRatings.Add(player, atkRating);
        }

        var defRatingsList = defenderRatings.OrderByDescending(x => x.Value).ToList();
        var atkRatingsList = attackerRatings.OrderByDescending(x => x.Value).ToList();
    }

    //Experimental / WIP :p
    public void CalculatePlayerRatingUsingElo(List<Match> matches, List<TeamStatistics>? teamStats = null)
    {
        if (teamStats == null)
        {
            teamStats = new TeamStatisticsController().CalculateTeamStats(matches);
        }

        Dictionary<string, decimal> defenderRatings = new Dictionary<string, decimal>();
        Dictionary<string, decimal> attackerRatings = new Dictionary<string, decimal>();
        foreach (var player in Enum.GetNames<Player>())
        {
            var defRating = teamStats.Where(x => x.Defender.Contains(player)).Sum(x => x.EloRating);
            var atkRating = teamStats.Where(x => x.Attacker.Contains(player)).Sum(x => x.EloRating);
            defenderRatings.Add(player, defRating);
            attackerRatings.Add(player, atkRating);
        }

        var defRatingsList = defenderRatings.OrderByDescending(x => x.Value).ToList();
        var atkRatingsList = attackerRatings.OrderByDescending(x => x.Value).ToList();
    }
}