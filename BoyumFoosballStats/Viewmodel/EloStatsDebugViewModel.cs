namespace BoyumFoosballStats.Viewmodel
{
    public class EloStatsDebugViewModel : IEloStatsDebugViewModel
    {
        public Dictionary<string, decimal> EloRatings { get; set; } = new Dictionary<string, decimal>();
    }
    public interface IEloStatsDebugViewModel
    {
        Dictionary<string, decimal> EloRatings { get; set; }
    }
}
