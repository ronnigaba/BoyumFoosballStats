using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Radzen.Blazor;

namespace BoyumFoosballStats.Viewmodel
{
    public class PlayerDashboardViewModel : IPlayerDashboardViewModel
    {
        public List<Match> Matches { get; set; } = new List<Match>();
        public AzureBlobStorageHelper blobHelper { get; set; }
        public MatchAnalysisHelper analysisHelper { get; set; }
        public IEnumerable<Player> PlayerFilterOption { get; set; }
        public Dictionary<string, double>? WeeklyWinRates { get; set; } = new();
        public Dictionary<string, int>? WeeklyMatchesPlayed { get; set; } = new();
        public Dictionary<string, double>? WinRatesByTeammate { get; set; } = new();
        private IOrderedEnumerable<IGrouping<string, Match>> MatchesByDate { get; set; }

        public void CalculateStats(object value)
        {
            CalculateWinRatesByTeammate(value);
            CalculateWeeklyStats(value);
        }

        private void CalculateWeeklyStats(object value)
        {
            if (MatchesByDate == null || !MatchesByDate.Any())
            {
                MatchesByDate = analysisHelper.SortMatchesByWeek(Matches).TakeLast(5).OrderBy(x => x.Key);
            }

            var player = (Player)value;
            var weeklyWinRates = new Dictionary<string, double>();
            var weeklyMatchesPlayed = new Dictionary<string, int>();
            foreach (var group in MatchesByDate)
            {
                var week = $"{@group.Key}";
                var matches = group.ToList();
                var winRate = analysisHelper.CalculateWinRate(matches, player);
                if (!double.IsNaN(winRate))
                {
                    weeklyWinRates.Add(week, winRate);
                }

                weeklyMatchesPlayed.Add(week, analysisHelper.CalculateMatchesPlayed(matches, player));
            }

            WeeklyWinRates = weeklyWinRates.Any() ? weeklyWinRates : null;
            WeeklyMatchesPlayed = weeklyMatchesPlayed.Any() ? weeklyMatchesPlayed : null;
        }

        private void CalculateWinRatesByTeammate(object value)
        {
            var currentPlayer = (Player)value;
            var winRatesByTeammate = new Dictionary<string, double>();
            foreach (var teammate in (Player[])Enum.GetValues(typeof(Player)))
            {
                if (currentPlayer == teammate)
                {
                    continue;
                }

                var winRate = analysisHelper.CalculateWinRateTeam(Matches, currentPlayer, teammate);
                if (double.IsNaN(winRate))
                {
                    continue;
                }

                winRatesByTeammate.Add(Enum.GetName(teammate)!, winRate);
            }
            WinRatesByTeammate = winRatesByTeammate.Any() ? winRatesByTeammate : null;
        }
    }

    public interface IPlayerDashboardViewModel
    {
        List<Match> Matches { get; set; }
        AzureBlobStorageHelper blobHelper { get; set; }
        MatchAnalysisHelper analysisHelper { get; set; }

        IEnumerable<Player> PlayerFilterOption { get; set; }
        public Dictionary<string, double> WeeklyWinRates { get; set; }
        public Dictionary<string, int> WeeklyMatchesPlayed { get; set; }
        public Dictionary<string, double> WinRatesByTeammate { get; set; }

        void CalculateStats(object value);
    }
}