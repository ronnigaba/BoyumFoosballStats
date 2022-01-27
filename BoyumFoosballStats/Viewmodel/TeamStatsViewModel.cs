using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{
    public class TeamStatsViewModel : ITeamStatsViewModel
    {
        public List<TeamStatistics> TeamStatistics { get; set; } = new ();
    }
    public interface ITeamStatsViewModel
    {
        List<TeamStatistics> TeamStatistics { get; set; }
    }
}
