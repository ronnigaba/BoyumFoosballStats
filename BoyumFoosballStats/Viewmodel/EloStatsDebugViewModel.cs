using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{
    public class TeamStatsDebugViewModel : ITeamStatsDebugViewModel
    {
        public List<TeamStatistics> TeamStatistics { get; set; } = new ();
    }
    public interface ITeamStatsDebugViewModel
    {
        List<TeamStatistics> TeamStatistics { get; set; }
    }
}
