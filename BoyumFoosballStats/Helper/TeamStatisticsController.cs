using BoyumFoosballStats.Model;
using Kezyma.EloRating;

namespace BoyumFoosballStats.Helper
{
    public class TeamStatisticsController
    {
        public List<TeamStatistics> CalculateTeamStats(List<Match> matches)
        {
            var eloRatings = new List<TeamStatistics>();

            foreach (var match in matches)
            {
                var losingIdentifier = match.LosingTeam.TeamIdentifier;
                var losingTeamStats = eloRatings.SingleOrDefault(x => x.TeamIdentifier == losingIdentifier);
                var losingElo = losingTeamStats?.EloRating ?? 0;
                var winningIdentifier = match.WinningTeam.TeamIdentifier;
                var winningTeamStats = eloRatings.SingleOrDefault(x => x.TeamIdentifier == winningIdentifier);
                var winningElo = winningTeamStats?.EloRating ?? 0;

                var newElo = EloCalculator.CalculateElo(winningElo, losingElo, (decimal)EloCalculator.WIN, (decimal)EloCalculator.LOSE);
                if (winningTeamStats == null)
                {
                    eloRatings.Add(new TeamStatistics { TeamIdentifier = winningIdentifier });
                    winningTeamStats = eloRatings.Single(x => x.TeamIdentifier == winningIdentifier);
                }
                winningTeamStats.EloRating = newElo[0];
                winningTeamStats.GoalsScored += match.WinningScore;
                winningTeamStats.GoalsAgainst += match.LosingScore;
                winningTeamStats.MatchesPlayed++;
                winningTeamStats.Wins++;
                if (losingTeamStats == null)
                {
                    eloRatings.Add(new TeamStatistics { TeamIdentifier = losingIdentifier });
                    losingTeamStats = eloRatings.Single(x => x.TeamIdentifier == losingIdentifier);
                }
                losingTeamStats.EloRating = newElo[1];
                losingTeamStats.GoalsScored += match.LosingScore;
                losingTeamStats.GoalsAgainst += match.WinningScore;
                losingTeamStats.MatchesPlayed++;
                losingTeamStats.Losses++;
            }

            return eloRatings;
        }

        public double CalculateEloAccuracy(List<Match> matches, List<TeamStatistics>? teamStats = null)
        {
            int unexpectedWins = 0;
            int expectedWins = 0;
            if (teamStats == null)
            {
                teamStats = CalculateTeamStats(matches);
            }
            foreach (var match in matches)
            {
                var losingRating = teamStats.SingleOrDefault(x => x.TeamIdentifier == match.LosingTeam.TeamIdentifier);
                var losingElo = losingRating?.EloRating ?? 0;
                var winningRating = teamStats.SingleOrDefault(x => x.TeamIdentifier == match.WinningTeam.TeamIdentifier);
                var winningElo = winningRating?.EloRating ?? 0;
                var prediction = EloCalculator.PredictResult(winningElo, losingElo);
                if (prediction[0] > prediction[1])
                {
                    expectedWins++;
                }
                else
                {
                    unexpectedWins++;
                }
            }

            var accuracy = (double)expectedWins / matches.Count * 100;
            return accuracy;
        }
    }
}
