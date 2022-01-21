using System.Text.Json;
using Azure.Storage.Blobs;
using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.Helper
{
    public class AzureBlobStorageHelper
    {
        private readonly string containerName = "foosballmatches";
        private readonly BlobServiceClient _blobServiceClient;
#if DEBUG
        public static string DefaultFileName = "matches_debug.json";
#else
        public static string DefaultFileName = "matches.json";
#endif

        private readonly string blobSasUrl =
            "https://boyumfoosballstorage.blob.core.windows.net/foosballmatches?sp=racwdli&st=2021-11-11T19:06:13Z&se=2099-11-12T03:06:13Z&sv=2020-08-04&sr=c&sig=d%2Fa9iPG41lR54QcBwi1Cy16PVfUac7D2oPTi4ZDQVC0%3D";
        public AzureBlobStorageHelper()
        {
            _blobServiceClient = new BlobServiceClient(new Uri(blobSasUrl));
        }

        public async Task UploadMatches(List<Match> matches, string fileName, bool overwrite = false)
        {
            var matchesToUpload = new List<Match>();
            BlobClient blobClient = GetBlobClient(containerName, fileName);
            if (!overwrite)
            {
                matchesToUpload.AddRange(await GetMatches(fileName));
            }
            matchesToUpload.AddRange(matches);
            var localFilePath = $"./{fileName}";
            var matchesJson = JsonSerializer.Serialize(matchesToUpload);
            await File.WriteAllTextAsync(localFilePath, matchesJson);

            await blobClient.UploadAsync(localFilePath, true);
        }

        public async Task<List<Match>> GetMatches(string fileName)
        {
            BlobClient blobClient = GetBlobClient(containerName, fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobResult = await blobClient.DownloadContentAsync();
                var matches = JsonSerializer.Deserialize<List<Match>>(blobResult.Value.Content);
                if (matches != null && matches.Any())
                {
                    return matches;
                }
            }
            return new List<Match>();

        }

        public async Task<List<Match>> RemoveEntry(string id)
        {
            var matches = await GetMatches(DefaultFileName);
            matches.RemoveAll(x => x.Id == id);
            await UploadMatches(matches, DefaultFileName, true);
            return matches;
        }

        private BlobClient GetBlobClient(string containerName, string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return containerClient.GetBlobClient(fileName);
        }
    }
}
