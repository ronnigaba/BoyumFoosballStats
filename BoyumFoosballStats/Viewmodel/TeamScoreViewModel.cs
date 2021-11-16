using BoyumFoosballStats.Model;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Viewmodel
{
    public class TeamScoreViewModel : ITeamScoreViewModel
    {
        [Parameter]
        public Team? Team { get; set; }

        [Parameter]
        public int Score { get; set; }
    }

    public interface ITeamScoreViewModel
    {
        Team Team { get; set; }
        int Score { get; set; }
    }
}
