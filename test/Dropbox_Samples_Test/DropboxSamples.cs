using Dropbox.Api;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.IO;
using Dropbox.Api.Files;

namespace Dropbox_Samples_Test
{
    public class DropboxSamples
    {
        public const string Token = "todo";  // Put token there

        [Fact]
        public async void GetAccountInfo()
        {
            using (var dbx = new DropboxClient(Token))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);
            }
        }

        [Fact]
        public async void GetFoldersAndFiles()
        {
            using (var client = new DropboxClient(Token))
            {
                var list = await client.Files.ListFolderAsync(string.Empty);

                // show folders then files
                foreach (var item in list.Entries.Where(i => i.IsFolder))
                {
                    Console.WriteLine("D  {0}/", item.Name);
                }

                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
                }
            }
        }

        [Fact]
        public async void UploadFile()
        {
            var folder = "";
            var file = $"{nameof(UploadFile)}.txt";
            var content = DateTime.UtcNow.ToString();

            using (var client = new DropboxClient(Token))
            {
                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    var updated = await client.Files.UploadAsync(
                        folder + "/" + file,
                        WriteMode.Overwrite.Instance,
                        body: mem);

                    Console.WriteLine("Saved {0}/{1} rev {2}", folder, file, updated.Rev);
                }
            }
        }

        [Fact]
        public async void DownloadFile()
        {
            var folder = "";
            var file = $"{nameof(UploadFile)}.txt";

            using (var client = new DropboxClient(Token))
            {
                using (var response = await client.Files.DownloadAsync(folder + "/" + file))
                {
                    Console.WriteLine(await response.GetContentAsStringAsync());
                }
            }
        }

    }
}