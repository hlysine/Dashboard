using Dashboard.Config;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Services
{
    public abstract class AuthCodeService : Service
    {
        [PersistentConfig]
        public virtual string ClientId { get; set; } = "";
        [PersistentConfig]
        public virtual string ClientSecret { get; set; } = "";
        // Access token isn't persistent. It should be refreshed upon restart.
        public virtual string AccessToken { get; set; } = "";
        [PersistentConfig(Generated = true)]
        public virtual string RefreshToken { get; set; } = "";

        [PersistentConfig(Generated = true)]
        public virtual List<string> AuthorizedScopes { get; set; } = new();

        public override bool IsAuthorized => !AccessToken.IsNullOrEmpty();

        public override bool CanAuthorize => !ClientId.IsNullOrEmpty() && !ClientSecret.IsNullOrEmpty();

        protected List<string> requiredScopes = new();

        /// <summary>
        /// Set the list of scopes required. To be called before <see cref="Authorize(CancellationToken)"/>.
        /// </summary>
        /// <param name="scopes">The list of scopes</param>
        public virtual void RequireScopes(string[] scopes)
        {
            requiredScopes = requiredScopes.Union(scopes).ToList();
        }


        public abstract Task Authorize(CancellationToken cancel = default);
        public abstract Task Unauthorize(CancellationToken cancel = default);
    }
}
