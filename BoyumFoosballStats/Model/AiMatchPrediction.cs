using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Model;

public class AiMatchPrediction
{
    public TableSide WinningSide { get; set; }
    public Team WinningTeam { get; set; }
    public float WinChanceBlack { get; set; }
    public float WinChanceGray { get; set; }
}