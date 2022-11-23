using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Ai.Controller;
using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats_Ai;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BoyumFoosballStats.Functions
{
    public static class AiTrainingHttpTrigger
    {
        //ToDo - Merge with logic in the timer trigger, or have the timer call the HTTP trigger instead
        [FunctionName("AiTraining")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
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
                var result = test.Train(predictionMatches, nameof(MatchOutcomeModel.ModelInput.Winner), 250, stream);
                //Backup the old model
                var oldModel = await blobHelper.GetFileStream("MatchOutcomeModel.zip");
                if (oldModel != null)
                {
                    await blobHelper.UploadFileStream("MatchOutcomeModel_old.zip", oldModel, true);
                }
                //Upload the new one
                await blobHelper.UploadFileStream("MatchOutcomeModel.zip", stream, true);
            }

            return new OkObjectResult("Model trained and uploaded successfully");
        }
    }
}
