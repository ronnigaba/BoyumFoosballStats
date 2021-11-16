using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{

    public class ScoreInputViewModel : IScoreInputViewModel
    {
        public Match Match { get; set; } = new Match();

        public async Task SaveScores()
        {
            if (Match.IsValid())
            {
                Match.MatchDate = DateTime.Now;
                var matches = new List<Match> { Match };
                var azureBlobHelper = new AzureBlobStorageHelper();
                await azureBlobHelper.UploadMatches(matches, AzureBlobStorageHelper.DefaultFileName);
                await Reset();
            }
        }

        public async Task Reset()
        {
            //We don't want to reset players
            await Task.Delay(0);
            Match.ScoreBlack = 0;
            Match.ScoreGray = 0;
            Match.Id = Guid.NewGuid().ToString();
        }
    }
    public interface IScoreInputViewModel
    {
        Match Match { get; set; }
        Task SaveScores();

        Task Reset();
    }
}
