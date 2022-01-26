using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{

    public class ScoreInputViewModel : IScoreInputViewModel
    {
        public AzureBlobStorageHelper? BlobStorageHelper { get; set; }
        public List<TeamStatistics>? EloRatings { get; set; }
        public Match Match { get; set; } = new Match();
        public bool SavingData { get; set; }
        public decimal[]? MatchPrediction { get; set; }

        public async Task SaveScores()
        {
            SavingData = true;
            if (Match.IsValid())
            {
                Match.MatchDate = DateTime.Now;
                var matches = new List<Match> { Match };
                if (BlobStorageHelper != null)
                {
                    await BlobStorageHelper.UploadList(matches, AzureBlobStorageHelper.DefaultMatchesFileName);
                    await Reset();
                }
            }
            SavingData = false;
        }

        public async Task PredictMatch()
        {
            await Task.Delay(0);
            var grayElo = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Gray.TeamIdentifier)?.EloRating ?? 0;
            var blackElo = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Black.TeamIdentifier)?.EloRating ?? 0;
            var prediction = EloHelper.PredictResult(grayElo, blackElo);
            MatchPrediction = prediction;
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
        AzureBlobStorageHelper BlobStorageHelper { get; set; }
        List<TeamStatistics>? EloRatings { get; set; }
        Match Match { get; set; }
        bool SavingData { get; set; }
        decimal[]? MatchPrediction { get; set; }
        Task SaveScores();
        Task PredictMatch();

        Task Reset();
    }
}
