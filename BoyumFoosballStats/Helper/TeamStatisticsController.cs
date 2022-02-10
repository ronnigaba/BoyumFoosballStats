using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Helper
{
    public class TeamStatisticsController
    {
        public List<TeamStatistics> CalculateTeamStats(List<Match> matches, int kFactor = 15)
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

                var newElo = EloHelper.CalculateElo(winningElo, losingElo, (decimal)EloHelper.WIN, (decimal)EloHelper.LOSE, kFactor);
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
                winningTeamStats.CurrentStreak++;
                winningTeamStats.HIghestStreak = winningTeamStats.CurrentStreak > winningTeamStats.HIghestStreak
                    ? winningTeamStats.CurrentStreak
                    : winningTeamStats.HIghestStreak;


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
                losingTeamStats.CurrentStreak = 0;
            }

            return eloRatings;
        }

        //Experimental / WIP :p
        public void CalculatePlayerElo(List<Match> matches)
        {
            var teamStats = CalculateTeamStats(matches);
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

        public double CalculateEloAccuracy(List<Match> matches, List<TeamStatistics>? teamStats = null, int kFactor = 15)
        {
            int unexpectedWins = 0;
            int expectedWins = 0;
            if (teamStats == null)
            {
                teamStats = CalculateTeamStats(matches, kFactor);
            }
            foreach (var match in matches)
            {
                var losingRating = teamStats.SingleOrDefault(x => x.TeamIdentifier == match.LosingTeam.TeamIdentifier);
                var losingElo = losingRating?.EloRating ?? 0;
                var winningRating = teamStats.SingleOrDefault(x => x.TeamIdentifier == match.WinningTeam.TeamIdentifier);
                var winningElo = winningRating?.EloRating ?? 0;
                var prediction = EloHelper.PredictResult(winningElo, losingElo);
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
