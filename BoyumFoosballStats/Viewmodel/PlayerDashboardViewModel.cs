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
        public Dictionary<string, double>? WeeklyWinRates { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, int>? WeeklyMatchesPlayed { get; set; } = new Dictionary<string, int>();

        private IOrderedEnumerable<IGrouping<string, Match>> MatchesByDate { get; set; }
        public void CalculateWeeklyStats(object value)
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
                var week = $"Week {@group.Key}";
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
    }

    public interface IPlayerDashboardViewModel
    {
        List<Match> Matches { get; set; }
        AzureBlobStorageHelper blobHelper { get; set; }
        MatchAnalysisHelper analysisHelper { get; set; }

        IEnumerable<Player> PlayerFilterOption { get; set; }
        public Dictionary<string, double> WeeklyWinRates { get; set; }
        public Dictionary<string, int> WeeklyMatchesPlayed { get; set; }

        void CalculateWeeklyStats(object value);
    }
}
