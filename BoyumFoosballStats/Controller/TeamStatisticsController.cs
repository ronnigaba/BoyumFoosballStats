using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Controller
{
    public class TeamStatisticsController
    {
        private readonly EloController _eloController = new();

        public List<TeamStatistics> CalculateTeamStats(List<Match> matches, int kFactor = 15)
        {
            var teamStats = new List<TeamStatistics>();

            foreach (var match in matches)
            {
                var winningIdentifier = match.WinningTeam.TeamIdentifier;
                var winningTeamStats = teamStats.SingleOrDefault(x => x.TeamIdentifier == winningIdentifier);
                var losingIdentifier = match.LosingTeam.TeamIdentifier;
                var losingTeamStats = teamStats.SingleOrDefault(x => x.TeamIdentifier == losingIdentifier);
                var winningElo = winningTeamStats?.EloRating ?? 0;
                var losingElo = losingTeamStats?.EloRating ?? 0;

                var newElo = _eloController.CalculateElo(winningElo, losingElo, (decimal)EloController.WIN, (decimal)EloController.LOSE, kFactor);
                if (winningTeamStats == null)
                {
                    teamStats.Add(new TeamStatistics { TeamIdentifier = winningIdentifier });
                    winningTeamStats = teamStats.Single(x => x.TeamIdentifier == winningIdentifier);
                }
                winningTeamStats.EloRating = newElo[0];
                winningTeamStats.GoalsScored += match.WinningScore;
                winningTeamStats.GoalsAgainst += match.LosingScore;
                winningTeamStats.MatchesPlayed++;
                winningTeamStats.Wins++;
                winningTeamStats.ActiveWinningStreak++;
                winningTeamStats.ActiveLosingStreak = 0;
                winningTeamStats.HighestWinningStreak = winningTeamStats.ActiveWinningStreak > winningTeamStats.HighestWinningStreak
                    ? winningTeamStats.ActiveWinningStreak
                    : winningTeamStats.HighestWinningStreak;


                if (losingTeamStats == null)
                {
                    teamStats.Add(new TeamStatistics { TeamIdentifier = losingIdentifier });
                    losingTeamStats = teamStats.Single(x => x.TeamIdentifier == losingIdentifier);
                }
                losingTeamStats.EloRating = newElo[1];
                losingTeamStats.GoalsScored += match.LosingScore;
                losingTeamStats.GoalsAgainst += match.WinningScore;
                losingTeamStats.MatchesPlayed++;
                losingTeamStats.Losses++;
                losingTeamStats.ActiveLosingStreak++;
                losingTeamStats.ActiveWinningStreak = 0;
                losingTeamStats.HighestLosingStreak = losingTeamStats.ActiveLosingStreak > losingTeamStats.HighestLosingStreak
                    ? losingTeamStats.ActiveLosingStreak
                    : losingTeamStats.HighestLosingStreak;
            }

            return teamStats;
        }
    }
}
