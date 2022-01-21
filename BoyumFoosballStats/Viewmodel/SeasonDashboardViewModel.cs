using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{
    public class SeasonDashboardViewModel : BaseDashboardViewModel, ISeasonDashboardViewModel
    {
        public IEnumerable<IGrouping<string, Match>>? MatchesBySeason { get; set; }
        public List<string> SeasonFilterOptions { get; set; } = new List<string>();

        public void CalculateSeasonStats(object value)
        {
            var season = value as string;
            if (MatchesBySeason != null)
            {
                Matches = MatchesBySeason.SingleOrDefault(x => x.Key == season)?.ToList() ?? new List<Match>();
                CalculateStats();
            }
        }
    }

    public interface ISeasonDashboardViewModel : IBaseDashboardViewModel
    {
        IEnumerable<IGrouping<string, Match>> MatchesBySeason { get; set; }
        List<string> SeasonFilterOptions { get; set; }
        void CalculateSeasonStats(object value);
    }
}
