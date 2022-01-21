using BoyumFoosballStats.Model;
using Kezyma.EloRating;

namespace BoyumFoosballStats.Helper
{
    public class EloRatingHelper
    {
        public List<TeamEloRating> CalculateTeamEloRatings(List<Match> matches)
        {
            var eloRatings = new List<TeamEloRating>();

            foreach (var match in matches)
            {
                var losingIdentifier = match.LosingTeam.TeamIdentifier;
                var losingRating = eloRatings.SingleOrDefault(x => x.TeamIdentifier == losingIdentifier);
                var losingElo = losingRating?.EloRating ?? 0;
                var winningIdentifier = match.WinningTeam.TeamIdentifier;
                var winningRating = eloRatings.SingleOrDefault(x => x.TeamIdentifier == winningIdentifier);
                var winningElo = winningRating?.EloRating ?? 0;

                var newElo = EloCalculator.CalculateElo(winningElo, losingElo, (decimal)EloCalculator.WIN, (decimal)EloCalculator.LOSE);
                if (winningRating == null)
                {
                    eloRatings.Add(new TeamEloRating { TeamIdentifier = winningIdentifier, EloRating = newElo[0] });
                }
                else
                {
                    winningRating.EloRating = newElo[0];
                }
                if (losingRating == null)
                {
                    eloRatings.Add(new TeamEloRating { TeamIdentifier = losingIdentifier, EloRating = newElo[1] });
                }
                else
                {
                    losingRating.EloRating = newElo[1];
                }
            }

            return eloRatings;
        }

        public double CalculateEloAccuracy(List<Match> matches, List<TeamEloRating>? eloRatings = null)
        {
            int unexpectedWins = 0;
            int expectedWins = 0;
            if (eloRatings == null)
            {
                eloRatings = CalculateTeamEloRatings(matches);
            }
            foreach (var match in matches)
            {
                var losingRating = eloRatings.SingleOrDefault(x => x.TeamIdentifier == match.LosingTeam.TeamIdentifier);
                var losingElo = losingRating?.EloRating ?? 0;
                var winningRating = eloRatings.SingleOrDefault(x => x.TeamIdentifier == match.WinningTeam.TeamIdentifier);
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
