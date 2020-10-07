using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Google_Samples_Test
{
    public class GoogleApiDriveSample
    {
        public const string TestFileName = "A4661F2C-F628-4072-9B18-16DC8E9EEFC0";

        [Fact]
        public async void AuthenticateTest()
        {
            var authService = new GoogleDriveApiAuthService();

            var service = await authService.GetDriveServiceAsync();
        }

        [Fact]
        public async void ListFiles()
        {
            var authService = new GoogleDriveApiAuthService();
            var service = await authService.GetDriveServiceAsync();

            var id = "root";
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name, parents, createdTime, modifiedTime, mimeType)";
            listRequest.Q = $"'{id}' in parents";

            var result = await listRequest.ExecuteAsync();
            var files = result.Files;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }

        [Fact]
        public async void UploadFile()
        {
            var file = $"{TestFileName}.txt";
            var content = DateTime.UtcNow.ToString();
            var contentType = "text/plain";

            var authService = new GoogleDriveApiAuthService();
            var service = await authService.GetDriveServiceAsync();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file,
                MimeType = contentType,
            };

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                FilesResource.CreateMediaUpload request;
                request = service.Files.Create(fileMetadata, stream, contentType);
                request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType, thumbnailLink";
                await request.UploadAsync();
            }
        }

        [Fact]
        public async void DownloadFile()
        {
            var authService = new GoogleDriveApiAuthService();
            var service = await authService.GetDriveServiceAsync();

            // Find file id by file name
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q = $"name='{TestFileName}.txt'";

            var result = await listRequest.ExecuteAsync();
            var files = result.Files;

            var fileId = files.First().Id;

            using (var outputstream = new MemoryStream())
            {
                var request = service.Files.Get(fileId);
                await request.DownloadAsync(outputstream);

                outputstream.Position = 0;
                var str = Encoding.UTF8.GetString(outputstream.ToArray());
                Console.WriteLine(str);
            }
        }

        [Fact]
        public async void DeletAllFiles()
        {
            var authService = new GoogleDriveApiAuthService();
            var service = await authService.GetDriveServiceAsync();

            var id = "root";
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name, parents, createdTime, modifiedTime, mimeType)";
            listRequest.Q = $"'{id}' in parents";

            var result = await listRequest.ExecuteAsync();
            var files = result.Files;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                  await  service.Files.Delete(file.Id).ExecuteAsync();
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
    }
}
