using Dashboard.Config;
using Dashboard.Utilities;

namespace Dashboard.Services;

public abstract class APIKeyService : Service
{
    [PersistentConfig]
    public virtual string ApiKey { get; set; } = "";

    public override bool CanAuthorize => !ApiKey.IsNullOrEmpty();
}