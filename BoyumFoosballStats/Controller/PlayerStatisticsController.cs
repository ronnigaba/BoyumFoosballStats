using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Controller;

public class PlayerStatisticsController
{
    private MatchAnalysisHelper _matchAnalysisHelper = new();

    public PlayerStatistics CalculatePlayerStatistics(List<Match> matches, Player player)
    {
        var playerStats = new PlayerStatistics();
        playerStats.Player = player;
        foreach (var secondPlayer in Enum.GetValues<Player>().Where(x => x != player))
        {
            var winRateWithPlayer = _matchAnalysisHelper.CalculateWinRateTeam(matches, player, secondPlayer);
            playerStats.WinRateByTeammate.Add(secondPlayer, winRateWithPlayer);

            var winRateAgainstPlayer =
                _matchAnalysisHelper.CalculateWinRate(
                    matches.Where(x => x.PlayedOnOpposingTeams(player, secondPlayer)).ToList(),
                    player);
            playerStats.WinRateByOpponent.Add(secondPlayer, winRateAgainstPlayer);

            var lossRateWithPlayer = _matchAnalysisHelper.CalculateLossRateTeam(matches, player, secondPlayer);
            playerStats.LossRateByTeammate.Add(secondPlayer, lossRateWithPlayer);

            var lossRateAgainstPlayer =
                _matchAnalysisHelper.CalculateLossRate(
                    matches.Where(x => x.PlayedOnOpposingTeams(player, secondPlayer)).ToList(),
                    player);
            playerStats.LossRateByOpponent.Add(secondPlayer, lossRateAgainstPlayer);
        }

        var matchesByWeekdays = matches.GroupBy(x => x.MatchDate.DayOfWeek);
        foreach (var matchesByWeekday in matchesByWeekdays)
        {
            var winRate = _matchAnalysisHelper.CalculateWinRate(matchesByWeekday.ToList(), player);
            playerStats.WinRateByWeekday.Add(matchesByWeekday.Key, winRate);
        }

        var weeklyStats = CalculateWeeklyStats(matches, player);
        playerStats.WeeklyWinRates = weeklyStats.WeeklyWinRates;
        playerStats.WeeklyMatchesPlayed = weeklyStats.WeeklyMatchesPlayed;
        
        return playerStats;
    }

    private (Dictionary<String, double> WeeklyWinRates, Dictionary<string, int> WeeklyMatchesPlayed)
        CalculateWeeklyStats(List<Match> matches, Player player)
    {
        var matchesByWeek = _matchAnalysisHelper.SortMatchesByWeek(matches).TakeLast(5).OrderBy(x => x.Key);
        var weeklyWinRates = new Dictionary<string, double>();
        var weeklyMatchesPlayed = new Dictionary<string, int>();
        foreach (var group in matchesByWeek)
        {
            var week = $"{group.Key}";
            var weeklyMatches = group.ToList();
            var winRate = _matchAnalysisHelper.CalculateWinRate(weeklyMatches, player);
            if (!double.IsNaN(winRate))
            {
                weeklyWinRates.Add(week, winRate);
            }

            weeklyMatchesPlayed.Add(week, _matchAnalysisHelper.CalculateMatchesPlayed(weeklyMatches, player));
        }

        return (WeeklyWinRates: weeklyWinRates, WeeklyMatchesPlayed: weeklyMatchesPlayed);
    }

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