using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using BoyumFoosballStats_Ai;
using Radzen;

namespace BoyumFoosballStats.Viewmodel
{

    public class ScoreInputViewModel : IScoreInputViewModel
    {
        public AzureBlobStorageHelper? BlobStorageHelper { get; set; }
        public TeamStatisticsController? TeamStatisticsController { get; set; }
        public List<TeamStatistics>? EloRatings { get; set; }
        public Match Match { get; set; } = new Match();
        public bool SavingData { get; set; }
        public decimal[]? MatchPredictionElo { get; set; }
        public float[] MatchPredictionAi { get; set; }
        private EloController _eloController = new();
        private MatchOutcomeModel matchOutcomeModel = new();
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
            MatchPredictionElo = null;
            if (Match.IsValid())
            {
                var grayElo = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Gray.TeamIdentifier)?.EloRating ?? 0;
                var blackElo = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Black.TeamIdentifier)?.EloRating ?? 0;

                //ToDo - Inform about swapping in UI
                //var grayEloSwapped = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Gray.TeamIdentifierSwapped)?.EloRating ?? 0;
                //var blackEloSwapped = EloRatings?.SingleOrDefault(x => x.TeamIdentifier == Match.Black.TeamIdentifierSwapped)?.EloRating ?? 0;
                var sampleData = new MatchOutcomeModel.ModelInput
                {
                    GrayDefender = (float)Match.Gray.Defender,
                    GrayAttacker = (float)Match.Gray.Attacker,
                    BlackDefender = (float)Match.Black.Defender,
                    BlackAttacker = (float)Match.Black.Attacker,
                };

                //Load model and predict output
                var result = await matchOutcomeModel.Predict(sampleData);
                var blackChance = 100 - result.Score * 100;
                var grayChance = 0 + result.Score * 100;
                var prediction = _eloController.PredictResult(grayElo, blackElo);
                MatchPredictionAi = new[] { grayChance, blackChance };
                MatchPredictionElo = prediction;
            }
        }


        public string? GetMatchPredictionText(TableSide side)
        {
            if (MatchPredictionElo == null || MatchPredictionElo.Length < 2)
            {
                return null;
            }
            var arrayIndex = side == TableSide.Black ? 1 : 0;
            return $"Elo: {(MatchPredictionElo[arrayIndex] * 100):0.00}% | Ai: {(MatchPredictionAi[arrayIndex]):0.00}%";
        }
        public BadgeStyle GetPredictionBadgeStyle(TableSide side)
        {
            if (MatchPredictionElo == null || MatchPredictionElo.Length < 2 || MatchPredictionAi == null || MatchPredictionAi.Length < 2)
            {
                return BadgeStyle.Info;
            }
            var arrayIndex = side == TableSide.Black ? 1 : 0;
            return (MatchPredictionElo[arrayIndex] * 100) >= 50 ? BadgeStyle.Success : BadgeStyle.Danger;
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
        decimal[]? MatchPredictionElo { get; set; }
        Task SaveScores();
        void PredictMatch();

        Task Reset();
        string GetMatchPredictionText(TableSide side);
        BadgeStyle GetPredictionBadgeStyle(TableSide side);
    }
}
