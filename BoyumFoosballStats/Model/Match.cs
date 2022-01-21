using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Model
{
    public class Match
    {
        public Match()
        {
            Black = new Team(TableSide.Black);
            Gray = new Team(TableSide.Gray);
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public Team Black { get; set; }
        public Team Gray { get; set; }

        [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
        public int ScoreBlack { get; set; }

        [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
        public int ScoreGray { get; set; }

        public DateTime MatchDate { get; set; }

        [JsonIgnore]
        public Team WinningTeam => ScoreBlack > ScoreGray ? Black : Gray;

        [JsonIgnore]
        public Team LosingTeam => ScoreBlack > ScoreGray ? Gray : Black;

        [JsonIgnore]
        public int WinningScore => ScoreBlack > ScoreGray ? ScoreBlack : ScoreGray;
        [JsonIgnore]
        public int LosingScore => ScoreBlack > ScoreGray ? ScoreGray : ScoreBlack;
        
        public bool IsValid()
        {
            var players = new List<Player?>() { Black.Attacker, Black.Defender, Gray.Attacker, Gray.Defender };
            if (players.Any(x => x == null) || players.GroupBy(x => x).Any(y => y.Count() > 1))
            {
                return false;
            }
            return true;
        }
    }
}
