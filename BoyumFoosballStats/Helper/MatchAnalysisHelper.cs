using System.Runtime.Serialization;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Helper
{
    public class MatchAnalysisHelper
    {
        public bool DidPlayerParticipateInAnyGames(List<Match> matches, Player player)
        {
            return CalculateMatchesPlayed(matches, player) > 0;
        }

        public double CalculateWinRate(List<Match> matches, Player player)
        {
            var wins = matches.Count(x => x.WinningTeam.Players.Contains(player));
            return Math.Round(wins / (float)CalculateMatchesPlayed(matches, player) * 100);
        }

        public int CalculateMatchesPlayed(List<Match> matches, Player player)
        {
            return matches.Count(x => x.Black.Players.Contains(player) || x.Gray.Players.Contains(player));
        }

        public double CalculateWinRateTeam(List<Match> matches, Player attacker, Player defender, bool ignorePosition = true)
        {
            var wins = matches.Count(x => x.WinningTeam.Players.Contains(attacker) && x.WinningTeam.Players.Contains(defender));
            return Math.Round(wins / (float)CalculateMatchesPlayedTeam(matches, attacker, defender, ignorePosition) * 100);
        }

        public int CalculateMatchesPlayedTeam(List<Match> matches, Player attacker, Player defender, bool ignorePosition = true)
        {
            return matches.Count(x => x.Black.Players.Contains(attacker) && x.Black.Players.Contains(defender) 
                                      || x.Gray.Players.Contains(attacker) && x.Gray.Players.Contains(defender));
        }

        public IOrderedEnumerable<KeyValuePair<string, double>> CalculateWinRatesForAllPlayers(List<Match> matches)
        {
            var result = new Dictionary<string, double>();

            foreach (var player in (Player[])Enum.GetValues(typeof(Player)))
            {
                var playerName = Enum.GetName(player);
                if (playerName != null && DidPlayerParticipateInAnyGames(matches, player))
                {
                    result.Add(playerName, CalculateWinRate(matches, player));
                }
            }
            return result.OrderBy(x => x.Value);
        }



        public IOrderedEnumerable<KeyValuePair<string, int>> CalculateMatchesPlayedForAllPlayers(List<Match> matches)
        {
            var result = new Dictionary<string, int>();

            foreach (var player in (Player[])Enum.GetValues(typeof(Player)))
            {
                var playerName = Enum.GetName(player);
                var matchesPlayed = CalculateMatchesPlayed(matches, player);
                if (playerName != null && matchesPlayed > 0)
                {
                    result.Add(playerName, matchesPlayed);
                }
            }
            return result.OrderBy(x => x.Value);
        }

        public Dictionary<string, double> CalculateWinRatesForTableSides(List<Match> matches)
        {
            var result = new Dictionary<string, double>();

            foreach (var tableSide in (TableSide[])Enum.GetValues(typeof(TableSide)))
            {
                var sideName = Enum.GetName(tableSide);
                if (sideName != null)
                {
                    var wins = matches.Count(x => x.WinningTeam.Side == tableSide);
                    result.Add(sideName, Math.Round(wins / (float)matches.Count * 100));
                }
            }

            return result;
        }
    }
}
