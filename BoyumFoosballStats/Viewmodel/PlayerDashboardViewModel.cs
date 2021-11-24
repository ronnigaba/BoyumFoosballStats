using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Viewmodel
{
    public class PlayerDashboardViewModel : IPlayerDashboardViewModel
    {
        public List<Match> Matches { get; set; } = new List<Match>();
        public AzureBlobStorageHelper blobHelper { get; set; }
        public MatchAnalysisHelper analysisHelper { get; set; }
        public IEnumerable<Player> PlayerFilterOption { get; set; }
        public Dictionary<string, double> WeeklyWinRates { get; set; }
        public Dictionary<string, int> WeeklyMatchesPlayed { get; set; }
        public void CalculateWeeklyStats(object value)
        {
            var matchesByDate = analysisHelper.SortMatchesByWeek(Matches).OrderBy(x => x.Key);
            var player = (Player)value;
            WeeklyWinRates = new Dictionary<string, double>();
            WeeklyMatchesPlayed = new Dictionary<string, int>();
            foreach (var group in matchesByDate.TakeLast(5))
            {
                var week = $"Week {@group.Key}";
                var matches = group.ToList();
                WeeklyWinRates.Add(week, analysisHelper.CalculateWinRate(matches, player));
                WeeklyMatchesPlayed.Add(week, analysisHelper.CalculateMatchesPlayed(matches, player));
            }
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
