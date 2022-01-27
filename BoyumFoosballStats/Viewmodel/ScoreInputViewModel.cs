using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Viewmodel
{

    public class ScoreInputViewModel : IScoreInputViewModel
    {
        public AzureBlobStorageHelper? BlobStorageHelper { get; set; }
        public TeamStatisticsController? TeamStatisticsController { get; set; }
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
                if (BlobStorageHelper != null && TeamStatisticsController != null)
                {
                    matches = await BlobStorageHelper.UploadList(matches, AzureBlobStorageHelper.DefaultMatchesFileName);
                    var teamStats = TeamStatisticsController.CalculateTeamStats(matches);
                    EloRatings = await BlobStorageHelper.UploadList(teamStats, AzureBlobStorageHelper.DefaultEloFileName, true);
                    await Reset();
                    PredictMatch();
                }
            }
            SavingData = false;
        }

        public async void PredictMatch()
        {
            await Task.Delay(0);
            if (Match.IsValid())
            {
                var grayElo = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Gray.TeamIdentifier)?.EloRating ?? 0;
                var blackElo = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Black.TeamIdentifier)?.EloRating ?? 0;
                var prediction = EloHelper.PredictResult(grayElo, blackElo);
                MatchPrediction = prediction;
            }
        }

        public string GetMatchPredictionText()
        {
            if (MatchPrediction == null || MatchPrediction.Length < 2)
            {
                return "";
            }
            return $"Black: {(MatchPrediction[1] * 100):0.##}% - Gray: {(MatchPrediction[0] * 100):0.##}%";
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
        TeamStatisticsController TeamStatisticsController { get; set; }
        List<TeamStatistics>? EloRatings { get; set; }
        Match Match { get; set; }
        bool SavingData { get; set; }
        decimal[]? MatchPrediction { get; set; }
        Task SaveScores();
        void PredictMatch();

        Task Reset();
        string GetMatchPredictionText();
    }
}
