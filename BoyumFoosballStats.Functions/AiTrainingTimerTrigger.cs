using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Ai.Controller;
using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats_Ai;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BoyumFoosballStats.Functions
{
    public class AiTrainingTimerTrigger
    {
        [FunctionName("AiTrainingTimerTrigger")]
        public async Task Run([TimerTrigger("0 0 12 * * SAT")]TimerInfo myTimer, ILogger log)
        {
            var blobHelper = new AzureBlobStorageHelper();
            var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
            var predictionMatches = matches.Select(x => new MatchOutcomeModel.ModelInput
            {
                GrayDefender = (float)x.Gray.Defender,
                GrayAttacker = (float)x.Gray.Attacker,
                BlackDefender = (float)x.Black.Defender,
                BlackAttacker = (float)x.Black.Attacker,
                Winner = (int)x.WinningSide
            });
            var test = new AiModelController<MatchOutcomeModel.ModelInput, MatchOutcomeModel.ModelOutput>();
            await using (Stream stream = new MemoryStream())
            {
                var result = test.Train(predictionMatches, nameof(MatchOutcomeModel.ModelInput.Winner), 300, stream);
                //Backup the old model
                var oldModel = await blobHelper.GetFileStream("MatchOutcomeModel.zip");
                if (oldModel != null)
                {
                    await blobHelper.UploadFileStream("MatchOutcomeModel_old.zip", oldModel, true);
                }
                //Upload the new one
                await blobHelper.UploadFileStream("MatchOutcomeModel.zip", stream, true);
            }
        }
    }
}
