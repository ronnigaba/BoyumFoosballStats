using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Viewmodel
{
    public class BaseDashboardViewModel : IBaseDashboardViewModel
    {
        public List<Match> Matches { get; set; } = new List<Match>();
        public AzureBlobStorageHelper blobHelper { get; set; }
        public MatchAnalysisHelper analysisHelper { get; set; }

        public IOrderedEnumerable<KeyValuePair<string, double>> OverallWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>> AttackerWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>> DefenderWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>> TeamWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>> OverallMatchesPlayed { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>> AttackerMatchesPlayed { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>> DefenderMatchesPlayed { get; set; }
        public Dictionary<string, double> TableSideWinRates { get; set; } = new ();
        public IOrderedEnumerable<KeyValuePair<string, double>>? DrillDownAttackerWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>>? DrillDownDefenderWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>>? DrillDownAttackerMatchesPlayed { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>>? DrillDownDefenderMatchesPlayed { get; set; }

        public List<PlayerPosition?> WinRateFilterOptions { get; set; } = new();

        public void CalculateWinRates(object value)
        {
            var positionFilter = value as PlayerPosition?;
            OverallWinrates = analysisHelper.CalculateWinRatesForAllPlayers(Matches, positionFilter);
            OverallMatchesPlayed = analysisHelper.CalculateMatchesPlayedForAllPlayers(Matches, positionFilter);
        }

        public void CalculateStats()
        {
            OverallWinrates = analysisHelper.CalculateWinRatesForAllPlayers(Matches);
            AttackerWinrates = analysisHelper.CalculateWinRatesForAllPlayers(Matches, PlayerPosition.Attacker).OrderBy(x => x.Key);
            DefenderWinrates = analysisHelper.CalculateWinRatesForAllPlayers(Matches, PlayerPosition.Defender).OrderBy(x => x.Key);
            TeamWinrates = analysisHelper.CalculateWinRatesForAllTeams(Matches);
            TableSideWinRates = analysisHelper.CalculateWinRatesForTableSides(Matches);
            OverallMatchesPlayed = analysisHelper.CalculateMatchesPlayedForAllPlayers(Matches);
            AttackerMatchesPlayed = analysisHelper.CalculateMatchesPlayedForAllPlayers(Matches, PlayerPosition.Attacker).OrderBy(x => x.Key);
            DefenderMatchesPlayed = analysisHelper.CalculateMatchesPlayedForAllPlayers(Matches, PlayerPosition.Defender).OrderBy(x => x.Key);
        }
    }

    public interface IBaseDashboardViewModel
    {
        List<Match> Matches { get; set; }
        AzureBlobStorageHelper blobHelper { get; set; }
        MatchAnalysisHelper analysisHelper { get; set; }

        List<PlayerPosition?> WinRateFilterOptions { get; set; }
        IOrderedEnumerable<KeyValuePair<string, double>> OverallWinrates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, double>> AttackerWinrates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, double>> DefenderWinrates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, double>> TeamWinrates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, int>> OverallMatchesPlayed { get; set; }
        IOrderedEnumerable<KeyValuePair<string, int>> AttackerMatchesPlayed { get; set; }
        IOrderedEnumerable<KeyValuePair<string, int>> DefenderMatchesPlayed { get; set; }
        Dictionary<string, double> TableSideWinRates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, double>>? DrillDownAttackerWinrates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, double>>? DrillDownDefenderWinrates { get; set; }
        IOrderedEnumerable<KeyValuePair<string, int>>? DrillDownAttackerMatchesPlayed { get; set; }
        IOrderedEnumerable<KeyValuePair<string, int>>? DrillDownDefenderMatchesPlayed { get; set; }

        void CalculateWinRates(object value);
        void CalculateStats();
    }
}
