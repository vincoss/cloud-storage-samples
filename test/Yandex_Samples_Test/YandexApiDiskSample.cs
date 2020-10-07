using System;
using System.Linq;
using System.Threading;
using Xunit;
using YandexDisk.Client;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace Yandex_Samples_Test
{
    public class YandexApiDiskSample
    {
        [Fact]
        public async void ListFiles()
        {
            string oauthToken = "TODO"; // Token in here

            // Create a client instance
            IDiskApi diskApi = new DiskHttpApi(oauthToken);

            //Getting information about folder /foo and all files in it
            Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = "", //Folder on Yandex Disk
            }, CancellationToken.None);

            // Getting all files from response
            var files = fooResourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File);

            //Path to local folder for downloading files
            string localFolder = @"C:\foo";

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Type);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
    }
}
