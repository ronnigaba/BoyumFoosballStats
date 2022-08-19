using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BoyumFoosballStats.Controller
{
    public class AzureBlobStorageHelper
    {
        private readonly string containerName = "foosballmatches";
        private readonly BlobServiceClient _blobServiceClient;
#if !DEBUG
        public static string DefaultMatchesFileName = "matches_debug.json";
        public static string DefaultEloFileName = "elo_debug.json";
#else
        public static string DefaultMatchesFileName = "matches.json";
        public static string DefaultEloFileName = "elo.json";
#endif
        private readonly string blobSasUrl =
            "https://boyumfoosballstorage.blob.core.windows.net/foosballmatches?sp=racwdli&st=2021-11-11T19:06:13Z&se=2099-11-12T03:06:13Z&sv=2020-08-04&sr=c&sig=d%2Fa9iPG41lR54QcBwi1Cy16PVfUac7D2oPTi4ZDQVC0%3D";

        public AzureBlobStorageHelper()
        {
            _blobServiceClient = new BlobServiceClient(new Uri(blobSasUrl));
        }

        public async Task<List<T>> UploadList<T>(List<T> entries, string fileName, bool overwrite = false)
        {
            var entriesToUpload = new List<T>();
            BlobClient blobClient = GetBlobClient(containerName, fileName);
            if (!overwrite)
            {
                entriesToUpload.AddRange(await GetEntries<T>(fileName));
            }

            entriesToUpload.AddRange(entries);
            var localFilePath = $"./{fileName}";
            var json = JsonSerializer.Serialize(entriesToUpload);
            await File.WriteAllTextAsync(localFilePath, json);

            await blobClient.UploadAsync(localFilePath, true);
            return entriesToUpload;
        }

        public async Task<List<T>> GetEntries<T>(string fileName)
        {
            BlobClient blobClient = GetBlobClient(containerName, fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobResult = await blobClient.DownloadContentAsync();
                var entries = JsonSerializer.Deserialize<List<T>>(blobResult.Value.Content);
                if (entries != null && entries.Any())
                {
                    return entries;
                }
            }

            return new List<T>();
        }

        public async Task<List<T>> RemoveEntry<T>(T entry, string fileName)
        {
            var entries = await GetEntries<T>(fileName);
            entries.Remove(entry);
            await UploadList(entries, DefaultMatchesFileName, true);
            return entries;
        }

        public async Task<Stream?> GetFileStream(string fileName)
        {
            BlobClient blobClient = GetBlobClient(containerName, fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobResult = await blobClient.DownloadContentAsync();
                return blobResult.Value.Content.ToStream();
            }

            return null;
        }

        public async Task UploadFileStream(string fileName, Stream stream, bool overwrite = false)
        {
            BlobClient blobClient = GetBlobClient(containerName, fileName);

            if (!overwrite)
            {
                BlobClient backupBlobClient = GetBlobClient(containerName, $"{fileName}_old");
                var oldFile = await GetFileStream(fileName);
                await backupBlobClient.UploadAsync(oldFile);
            }
            await stream.FlushAsync();
            stream.Position = 0;
            await blobClient.UploadAsync(stream, true);
        }

        private BlobClient GetBlobClient(string containerName, string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return containerClient.GetBlobClient(fileName);
        }
    }
}