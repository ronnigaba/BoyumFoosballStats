﻿using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Tensorflow;

namespace BoyumFoosballStats.Controller
{
    public class MatchAnalysisHelper
    {
        public bool DidPlayerParticipateInAnyGames(List<Match> matches, Player player, PlayerPosition? position = null)
        {
            return CalculateMatchesPlayed(matches, player, position) > 0;
        }

        public double CalculateWinRate(List<Match> matches, Player player, PlayerPosition? position = null, TableSide? side = null)
        {
            var sides = new List<TableSide>();
            if (side == null)
            {
                sides.Add(TableSide.Black);
                sides.Add(TableSide.Gray);
            }
            else
            {
                sides.Add(side.Value);
            }


            int wins;
            switch (position)
            {
                case PlayerPosition.Attacker:
                    wins = matches.Count(x => x.WinningTeam.Attacker == player && sides.Contains(x.WinningTeam.Side));
                    break;
                case PlayerPosition.Defender:
                    wins = matches.Count(x => x.WinningTeam.Defender == player  && sides.Contains(x.WinningTeam.Side));
                    break;
                default:
                    wins = matches.Count(x => x.WinningTeam.Players.Contains(player)  && sides.Contains(x.WinningTeam.Side));
                    break;
            }
            return Math.Round(wins / (float)CalculateMatchesPlayed(matches, player, position, side) * 100);
        }
        
        public double CalculateLossRate(List<Match> matches, Player player, PlayerPosition? position = null)
        {
            int losses;
            switch (position)
            {
                case PlayerPosition.Attacker:
                    losses = matches.Count(x => x.LosingTeam.Attacker == player);
                    break;
                case PlayerPosition.Defender:
                    losses = matches.Count(x => x.LosingTeam.Defender == player);
                    break;
                default:
                    losses = matches.Count(x => x.LosingTeam.Players.Contains(player));
                    break;
            }
            return Math.Round(losses / (float)CalculateMatchesPlayed(matches, player, position) * 100);
        }

        public int CalculateMatchesPlayed(List<Match> matches, Player player, PlayerPosition? position = null, TableSide? side = null)
        {
            switch (side)
            {
                case TableSide.Black:
                    matches = matches.Where(x => x.Black.Players.Contains(player)).ToList();
                    break;
                case TableSide.Gray:
                    matches = matches.Where(x => x.Gray.Players.Contains(player)).ToList();
                    break;
            }
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

        public int CalculateMatchesPlayed(List<Match> matches, Player attacker, Player defender, bool ignorePosition = true)
        {
            if (!ignorePosition)
            {
                return matches.Count(x => x.Black.Attacker == attacker && x.Black.Defender == defender
                                          || x.Gray.Attacker == attacker && x.Gray.Defender == defender);
            }
            return matches.Count(x => x.Black.Players.Contains(attacker) && x.Black.Players.Contains(defender)
                                      || x.Gray.Players.Contains(attacker) && x.Gray.Players.Contains(defender));
        }

        public double CalculateWinRateTeam(List<Match> matches, Player attacker, Player defender, bool ignorePosition = true)
        {
            var wins = matches.Count(x => x.WinningTeam.Players.Contains(attacker) && x.WinningTeam.Players.Contains(defender));
            return Math.Round(wins / (float)CalculateMatchesPlayed(matches, attacker, defender, ignorePosition) * 100);
        }
        public double CalculateLossRateTeam(List<Match> matches, Player attacker, Player defender, bool ignorePosition = true)
        {
            var losses = matches.Count(x => x.LosingTeam.Players.Contains(attacker) && x.LosingTeam.Players.Contains(defender));
            return Math.Round(losses / (float)CalculateMatchesPlayed(matches, attacker, defender, ignorePosition) * 100);
        }

        public IOrderedEnumerable<KeyValuePair<string, double>> CalculateWinRatesForAllTeams(List<Match> matches, bool ignorePositions = true)
        {
            var result = new Dictionary<string, double>();
            var values = Enum.GetValues(typeof(Player)).Cast<Player>();
            var combinations = CollectionCombinationHelper.GetUniqueCombinations<Player>(values, 2);
            var teamCombinations = combinations.ToList();
            foreach (var team in teamCombinations)
            {
                var p = team.ToList();
                var attacker = p.First();
                var defender = p.Last();
                var count = CalculateMatchesPlayed(matches, attacker, defender, ignorePositions);
                if (count > 0)
                {
                    result.Add($"{attacker}/{defender}", CalculateWinRateTeam(matches, attacker, defender, ignorePositions));
                }
            }
            return result.OrderBy(x => x.Value);
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

        public IEnumerable<IGrouping<string, Match>> SortMatchesByWeek(List<Match> matches)
        {
            return matches.GroupBy(x => DateHelper.GetCurrentWeekByDate(x.MatchDate));
        }

        public IEnumerable<IGrouping<string, Match>> SortMatchesBySeason(List<Match> matches)
        {
            return matches.GroupBy(x => x.MatchDate.ToString("yyyy/MM"));
        }
    }
}
