using Dashboard.Config;
using Dashboard.Utilities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dashboard.Services
{
    public class GoogleService : AuthCodeService
    {
        // Properties that are not used since the Google client libraries handle the credentials themselves
        [Obsolete("This property is not used", true)]
        public new string AccessToken { get; set; }
        [Obsolete("This property is not used", true)]
        public new string RefreshToken { get; set; }

        [PersistentConfig(Generated = true)]
        public List<CredentialKeyValuePair<string, object>> Credentials { get; set; } = new List<CredentialKeyValuePair<string, object>>();

        public override bool IsAuthorized => credential != null;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private UserCredential credential;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            // Use a semaphore to avoid opening multiple consent screens when multiple Google services are authenticating
            await semaphore.WaitAsync();
            try
            {
                if (!requiredScopes.IsSubsetOf(AuthorizedScopes))
                {
                    await Unauthorize();
                }

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = ClientId,
                        ClientSecret = ClientSecret
                    },
                    requiredScopes,
                    Id,
                    cancel,
                    new ConfigDataStore(this));
                AuthorizedScopes = requiredScopes;
                RaiseConfigUpdated(EventArgs.Empty);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            Credentials.Clear();
            AuthorizedScopes.Clear();
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public UserCredential GetCredential()
        {
            return credential;
        }
    }

    [XmlInclude(typeof(TokenResponse))]
    public class CredentialKeyValuePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public CredentialKeyValuePair(TKey _key, TValue _value) => (Key, Value) = (_key, _value);
        public CredentialKeyValuePair()
        {

        }
    }

    public class ConfigDataStore : IDataStore
    {
        private GoogleService service;

        public ConfigDataStore(GoogleService _service) => service = _service;

        public async Task ClearAsync()
        {
            service.Credentials.Clear();
        }

        public async Task DeleteAsync<T>(string key)
        {
            service.Credentials.RemoveAll(x => x.Key == key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return (T)service.Credentials.FirstOrDefault(x => x.Key == key)?.Value;
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (service.Credentials.Any(x => x.Key == key))
            {
                service.Credentials.RemoveAll(x => x.Key == key);
            }
            service.Credentials.Add(new CredentialKeyValuePair<string, object>(key, value));
        }
    }
}
