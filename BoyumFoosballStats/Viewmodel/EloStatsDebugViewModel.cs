using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{
    public class EloStatsDebugViewModel : IEloStatsDebugViewModel
    {
        public List<TeamEloRating> EloRatings { get; set; } = new ();
    }
    public interface IEloStatsDebugViewModel
    {
        List<TeamEloRating> EloRatings { get; set; }
    }
}
