using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
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

            var me = await graphClient.Me.Request().WithForceRefresh(true).GetAsync();

            Assert.NotNull(me);
        }
    }
}
