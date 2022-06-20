using Newtonsoft.Json;

namespace BoyumFoosballStats.Model
{
    public class TeamStatistics
    {
        public string TeamIdentifier { get; set; }
        public decimal EloRating { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsAgainst { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses{ get; set; }
        public int CurrentWinningStreak{ get; set; }
        public int CurrentLosingStreak{ get; set; }
        public int HIghestWinningStreak { get; set; }
        public int HIghestLosingStreak { get; set; }
        [JsonIgnore] public int GoalDifference => GoalsScored - GoalsAgainst;
        [JsonIgnore] public double WinRate => (double)Wins / MatchesPlayed * 100;
        [JsonIgnore] public string Attacker => TeamIdentifier.Split(' ').First();
        [JsonIgnore] public string Defender => TeamIdentifier.Split(' ').Last();
        [JsonIgnore] public string WinRateFormatted => WinRate.ToString("0.00");
        [JsonIgnore] public string EloRatingFormatted => EloRating.ToString("0.00");

    }
}
