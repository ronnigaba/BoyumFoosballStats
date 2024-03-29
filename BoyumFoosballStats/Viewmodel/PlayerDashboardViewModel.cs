﻿using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Radzen.Blazor;

namespace BoyumFoosballStats.Viewmodel
{
    public class PlayerDashboardViewModel : IPlayerDashboardViewModel
    {
        public List<Match> Matches { get; set; } = new();
        public AzureBlobStorageHelper blobHelper { get; set; }
        private PlayerStatisticsController _playerStatisticsController { get; set; } = new();
        public IEnumerable<Player> PlayerFilterOption { get; set; }
        private Dictionary<Player, PlayerStatistics> PlayerStatisticsCache { get; set; } = new();
        public PlayerStatistics? CurrentPlayerStats { get; set; }
        public string IdealMatchAdvice { get; set; }
        public Dictionary<string, double> WinRateByTeammate { get; set; } = new();
        public Dictionary<string, double> WinRateByOpponent { get; set; } = new();
        public Dictionary<string, double> WinRateByWeekday { get; set; } = new();
        public Dictionary<string, double> WinRateByPosition{ get; set; }
        public Dictionary<string, double> WinRateBySide{ get; set; }

        public void CalculateStats(object value)
        {
            var player = (Player)value;
            if (PlayerStatisticsCache.TryGetValue(player, out var cachedPlayerStats))
            {
                CurrentPlayerStats = cachedPlayerStats;
            }

            CurrentPlayerStats = _playerStatisticsController.CalculatePlayerStatistics(Matches, player);
            FormatData(CurrentPlayerStats);
        }

        private void FormatData(PlayerStatistics playerStats)
        {
            var players = Enum.GetNames<Player>();
            var weekdays = Enum.GetNames<DayOfWeek>();
            var tableSides = Enum.GetNames<TableSide>();
            WinRateByTeammate = playerStats.WinRateByTeammate.Where(x => !double.IsNaN(x.Value)).OrderBy(x => x.Value)
                .ToDictionary(x => players[(int)x.Key], x => x.Value);
            WinRateByOpponent = playerStats.WinRateByOpponent.Where(x => !double.IsNaN(x.Value)).OrderBy(x => x.Value)
                .ToDictionary(x => players[(int)x.Key], x => x.Value);
            WinRateByWeekday = playerStats.WinRateByWeekday.Where(x => !double.IsNaN(x.Value)).OrderBy(x => x.Key)
                .ToDictionary(x => weekdays[(int)x.Key], x => x.Value);
            WinRateByPosition = playerStats.WinRateByPosition.Where(x => !double.IsNaN(x.Value)).OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
            WinRateBySide = playerStats.WinRateBySide.Where(x => !double.IsNaN(x.Value)).OrderBy(x => x.Key)
                .ToDictionary(x => tableSides[(int)x.Key], x => x.Value);
            var idealTeammate = playerStats.WinRateByTeammate.MaxBy(x => x.Value).Key;
            var idealOpponents = playerStats.WinRateByOpponent.Where(x => x.Key != idealTeammate)
                .OrderByDescending(x => x.Value).Take(2).ToList();

            IdealMatchAdvice =
                $"The ideal match for {playerStats.Player} is played on a {playerStats.WinRateByWeekday.MaxBy(x => x.Value).Key} " +
                $"with teammate {idealTeammate} against {idealOpponents.First().Key} and {idealOpponents.Last().Key}";
        }
    }

    public interface IPlayerDashboardViewModel
    {
        Dictionary<string, double> WinRateByTeammate { get; set; }
        public Dictionary<string, double> WinRateByOpponent { get; set; }
        public Dictionary<string, double> WinRateByWeekday { get; set; }
        public Dictionary<string, double> WinRateByPosition{ get; set; }
        public Dictionary<string, double> WinRateBySide{ get; set; }
        List<Match> Matches { get; set; }
        AzureBlobStorageHelper blobHelper { get; set; }

        IEnumerable<Player> PlayerFilterOption { get; set; }
        PlayerStatistics? CurrentPlayerStats { get; set; }
        string IdealMatchAdvice { get; set; }
        void CalculateStats(object value);
    }
}