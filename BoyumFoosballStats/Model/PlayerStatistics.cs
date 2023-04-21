using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Model;

public class PlayerStatistics
{
    public Player Player { get; set; }
    public Dictionary<Player, double> WinRateByTeammate { get; set; } = new();
    public Dictionary<Player, double> WinRateByOpponent { get; set; } = new();

    public Dictionary<Player, double> LossRateByTeammate { get; set; } = new();
    public Dictionary<Player, double> LossRateByOpponent { get; set; } = new();

    public Dictionary<string, double> WinRateByPosition{ get; set; } = new();
    public Dictionary<TableSide, double> WinRateBySide{ get; set; } = new();

    public Dictionary<string, double> WeeklyWinRates { get; set; } = new();
    public Dictionary<string, int> WeeklyMatchesPlayed { get; set; } = new();

    public Dictionary<DayOfWeek, double> WinRateByWeekday { get; set; } = new();

    public int GoalsScored { get; set; }
    public int GoalsAgainst { get; set; }
    public int MatchesPlayed { get; set; }

    public double GoalsScoredPerMatch => (double)GoalsScored / MatchesPlayed;
    public double GoalsAgainstPerMatch => (double)GoalsAgainst / MatchesPlayed;
}

