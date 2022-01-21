using System.Text.Json.Serialization;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Model
{
    public class Team : TeamBase
    {
        public Team(TableSide side)
        {
            Side = side;
        }

        public TableSide Side { get; set; }
    }

    public class TeamBase
    {
        public Player? Attacker { get; set; }

        public Player? Defender { get; set; }

        [JsonIgnore]
        public List<Player?> Players => new List<Player?>() { Attacker, Defender };

    }
}
