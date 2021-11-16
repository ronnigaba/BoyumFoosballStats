using System.Text.Json.Serialization;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Model
{
    public class Team
    {
        public Team(TableSide side)
        {
            Side = side;
            Attacker = null;
            Defender = null;
        }

        public Player? Attacker { get; set; }

        public Player? Defender { get; set; }
        public TableSide Side { get; set; }

        [JsonIgnore]
        public List<Player?> Players => new List<Player?>() { Attacker, Defender };
    }
}
