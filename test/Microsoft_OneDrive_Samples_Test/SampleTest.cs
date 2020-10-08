using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;


namespace Microsoft_OneDrive_Samples_Test
{
    public class SampleTest
    {
        public const string ClientId = "";  // Put clientId there
        public const string TenantId = "";  // Put tenantId there
        public const string Secret = "";  // Put secret there
        public static string[] Scopes = new[] { "User.Read", "Files.ReadWrite" };

        [Fact]
        public async void GetAccountInfo()
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                 .Create(ClientId)
                 .WithTenantId(TenantId)
                 .WithClientSecret(Secret)
                 .Build();

            var authenticationProvider = new ClientCredentialProvider(confidentialClientApplication);
            var graphClient = new GraphServiceClient(authenticationProvider);

            var me = await graphClient.Me.Drive.Request().GetAsync();

            Assert.NotNull(me);
        }

        [Fact]
        public async void DownloadFile()
        {
            var provider = new GraphAuthProvider();
            var graphClient = await provider.AuthenticateViaAppIdAndSecret(TenantId, ClientId, Secret);

            var file = $"Dev.txt";

            var fileStream = await graphClient.Me.Drive.Items[file]
                                      .Content
                                      .Request()
                                      .GetAsync();

            var path = @"C:\temp\";
            var fullPath = Path.Combine(path, file);

            using (var fs = System.IO.File.Create(fullPath))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.CopyTo(fs);
            }
        }

        public class GraphAuthProvider
        {
            public async Task<GraphServiceClient> AuthenticateViaAppIdAndSecret(
                string tenantId,
                string clientId,
                string clientSecret)
            {
                var scopes = new string[] { "https://graph.microsoft.com/.default" };

                // Configure the MSAL client as a confidential client
                var confidentialClient = ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithAuthority($"https://login.microsoftonline.com/{tenantId}/v2.0")
                    .WithClientSecret(clientSecret)
                    .Build();

                // Build the Microsoft Graph client. As the authentication provider, set an async lambda
                // which uses the MSAL client to obtain an app-only access token to Microsoft Graph,
                // and inserts this access token in the Authorization header of each API request. 
                GraphServiceClient graphServiceClient =
                    new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
                    {

                // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
                var authResult = await confidentialClient
                            .AcquireTokenForClient(scopes)
                            .ExecuteAsync();

                // Add the access token in the Authorization header of the API request.
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                    })
                );

                return graphServiceClient;
            }
        }

    }
}
