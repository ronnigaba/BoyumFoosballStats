﻿using System.Runtime.Serialization;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Helper
{
    public class MatchAnalysisHelper
    {
        public bool DidPlayerParticipateInAnyGames(List<Match> matches, Player player, PlayerPosition? position = null)
        {
            return CalculateMatchesPlayed(matches, player, position) > 0;
        }

        public double CalculateWinRate(List<Match> matches, Player player, PlayerPosition? position = null)
        {
            int wins;
            switch (position)
            {
                case PlayerPosition.Attacker:
                    wins = matches.Count(x => x.WinningTeam.Attacker == player);
                    break;
                case PlayerPosition.Defender:
                    wins = matches.Count(x => x.WinningTeam.Defender == player);
                    break;
                default:
                    wins = matches.Count(x => x.WinningTeam.Players.Contains(player));
                    break;
            }
            return Math.Round(wins / (float)CalculateMatchesPlayed(matches, player, position) * 100);
        }

        public int CalculateMatchesPlayed(List<Match> matches, Player player, PlayerPosition? position = null)
        {
            switch (position)
            {
                case PlayerPosition.Attacker:
                    return matches.Count(x => x.Black.Attacker == player || x.Gray.Attacker == player);
                case PlayerPosition.Defender:
                    return matches.Count(x => x.Black.Defender == player || x.Gray.Defender == player);
                default:
                    return matches.Count(x => x.Black.Players.Contains(player) || x.Gray.Players.Contains(player));
            }
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

        public IOrderedEnumerable<KeyValuePair<string, double>> CalculateWinRatesForAllPlayers(List<Match> matches, PlayerPosition? position = null)
        {
            var result = new Dictionary<string, double>();

            foreach (var player in (Player[])Enum.GetValues(typeof(Player)))
            {
                var playerName = Enum.GetName(player);
                if (playerName != null && DidPlayerParticipateInAnyGames(matches, player, position))
                {
                    result.Add(playerName, CalculateWinRate(matches, player, position));
                }
            }
            return result.OrderBy(x => x.Value);
        }

        public IOrderedEnumerable<KeyValuePair<string, int>> CalculateMatchesPlayedForAllPlayers(List<Match> matches, PlayerPosition? position = null)
        {
            var result = new Dictionary<string, int>();

            foreach (var player in (Player[])Enum.GetValues(typeof(Player)))
            {
                var playerName = Enum.GetName(player);
                var matchesPlayed = CalculateMatchesPlayed(matches, player, position);
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
