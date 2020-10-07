using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Google_Samples_Test
{
    public class GoogleDriveApiAuthService
    {
        protected static string[] scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        protected readonly UserCredential credential;
        static string ApplicationName = "MyApplicationName";

        public async Task<DriveService> GetDriveServiceAsync()
        {
            string[] scopes = new string[]
            {
                    DriveService.Scope.Drive,
                    DriveService.Scope.DriveFile
            };

            var clientId = "TODO:";
            var clientSecret = "TODO:";

            var store = new FileDataStore("MyAppsToken"); // Find the token path here.
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
                scopes,
                Environment.UserName,
                CancellationToken.None,
                store);

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}
