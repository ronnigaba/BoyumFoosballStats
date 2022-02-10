using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{
    public class SeasonDashboardViewModel : BaseDashboardViewModel, ISeasonDashboardViewModel
    {
        public List<Match>? AllMatches { get; set; }
        public IEnumerable<IGrouping<string, Match>>? MatchesBySeason { get; set; }
        public List<string> SeasonFilterOptions { get; set; } = new List<string>();
        public string? SeasonFilterValue { get; set; }
        public List<TeamStatistics> TeamStatistics { get; set; } = new List<TeamStatistics>();

        private TeamStatisticsController _teamStatisticsController = new TeamStatisticsController();

        public void CalculateSeasonStats(object value)
        {
            if (string.IsNullOrWhiteSpace(SeasonFilterValue))
            {
                Matches = AllMatches ?? new List<Match>();
            }
            else if (MatchesBySeason != null)
            {
                Matches = MatchesBySeason.SingleOrDefault(x => x.Key == SeasonFilterValue)?.ToList() ?? new List<Match>();
            }
            CalculateStats();
            CalculateTeamStatistics();
        }

        public void CalculateTeamStatistics()
        {
            TeamStatistics = _teamStatisticsController.CalculateTeamStats(Matches);
        }
    }

    public interface ISeasonDashboardViewModel : IBaseDashboardViewModel
    {
        List<Match>? AllMatches { get; set; }
        IEnumerable<IGrouping<string, Match>> MatchesBySeason { get; set; }
        List<string> SeasonFilterOptions { get; set; }
        List<TeamStatistics> TeamStatistics { get; set; }
        string SeasonFilterValue { get; set; }
        void CalculateSeasonStats(object value);
        void CalculateTeamStatistics();
    }
}
