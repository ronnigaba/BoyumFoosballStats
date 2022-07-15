using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BoyumFoosballStats.Ai;
using BoyumFoosballStats.Ai.Controller;
using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using BoyumFoosballStats_Ai;
using CsvHelper;
using Xunit;

namespace BoyumFoosballStats.Test;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        var sut = new PlayerStatisticsController();
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        sut.CalculatePlayerRatingUsingAlgorhitm(matches);
    }

    [Fact]
    public async void TrainCustomModel()
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
            await blobHelper.UploadFileStream("MatchOutcomeModel.zip", stream, true);
        }
    }

    [Fact]
    public async void GenerateTrainingData_Csv()
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
        var path = "..\\..\\..\\..\\MatchPredictionData.csv";
        File.Delete(path);
        await using (var writer = new StreamWriter(path))
        {
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(predictionMatches);
                await csv.FlushAsync();
            }
        }
    }

    private Player GetRandomPlayer(Random randomizer)
    {
        return Enum.GetValues<Player>()[randomizer.NextInt64(0, 8)];
    }
    
    [Fact]
    public async void MatchPredictionTest()
    {
        var results = new List<bool>();
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        var teamStats = new TeamStatisticsController().CalculateTeamStats(matches);
        var outcomeModel = new MatchOutcomeModel();
        var eloController = new EloController();
        for (int i = 0; i < 100000; i++)
        {

            var match = new Match();
            var random = new Random(DateTime.UtcNow.Millisecond);

            while (!match.IsValid())
            {
                match.Gray.Defender = GetRandomPlayer(random);
                match.Gray.Attacker = GetRandomPlayer(random);
                match.Black.Defender = GetRandomPlayer(random);
                match.Black.Attacker = GetRandomPlayer(random);
            }
            //Load sample data
            var sampleData = new MatchOutcomeModel.ModelInput
            {
                GrayDefender = (float)match.Gray.Defender,
                GrayAttacker = (float)match.Gray.Attacker,
                BlackDefender = (float)match.Black.Defender,
                BlackAttacker = (float)match.Black.Attacker,
            };

            var grayElo = teamStats?.SingleOrDefault(x => x.TeamIdentifier == match.Gray.TeamIdentifier)?.EloRating ?? 0;
            var blackElo = teamStats?.SingleOrDefault(x => x.TeamIdentifier == match.Black.TeamIdentifier)?.EloRating ?? 0;

            //Load model and predict output
            var result = await outcomeModel.Predict(sampleData);
            var aiPredictedWinner = result.Score > 0.5 ? TableSide.Gray : TableSide.Black;
            var blackChange = 100 - result.Score * 100;
            var grayChance = 0 + result.Score * 100;
            
            var prediction = eloController.PredictResult(grayElo, blackElo);
            var eloPredictedWinner = prediction[0] > prediction[1] ? TableSide.Gray : TableSide.Black;
            results.Add(eloPredictedWinner == aiPredictedWinner);
        }

        var accuraccy = ((double)results.Count(x => x) / results.Count) * 100;
    }

    [Fact]
    public async void TestAccuraccyAgainstExistingMatches_Elo()
    {
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        var teamStats = new TeamStatisticsController().CalculateTeamStats(matches);
        var eloController = new EloController();
        int unexpectedWins = 0;
        int expectedWins = 0;
        foreach (var match in matches)
        {
            var losingRating = teamStats.SingleOrDefault(x => x.TeamIdentifier == match.LosingTeam.TeamIdentifier);
            var losingElo = losingRating?.EloRating ?? 0;
            var winningRating = teamStats.SingleOrDefault(x => x.TeamIdentifier == match.WinningTeam.TeamIdentifier);
            var winningElo = winningRating?.EloRating ?? 0;
            var prediction = eloController.PredictResult(winningElo, losingElo);
            if (prediction[0] > prediction[1])
            {
                expectedWins++;
            }
            else
            {
                unexpectedWins++;
            }
        }

        var accuracy = (double)expectedWins / matches.Count * 100;
    }
    [Fact]
    public async void TestAccuraccyAgainstExistingMatches_Ai()
    {
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        var outcomeModel = new MatchOutcomeModel();
        int unexpectedWins = 0;
        int expectedWins = 0;
        foreach (var match in matches)
        {
            //Load sample data
            var sampleData = new MatchOutcomeModel.ModelInput
            {
                GrayDefender = (float)match.Gray.Defender,
                GrayAttacker = (float)match.Gray.Attacker,
                BlackDefender = (float)match.Black.Defender,
                BlackAttacker = (float)match.Black.Attacker,
            };
            var result = await outcomeModel.Predict(sampleData);
            var aiPredictedWinner = result.Score > 0.5 ? TableSide.Gray : TableSide.Black;
            if (aiPredictedWinner == match.WinningSide)
            {
                expectedWins++;
            }
            else
            {
                unexpectedWins++;
            }
        }

        var accuracy = (double)expectedWins / matches.Count * 100;
    }
}