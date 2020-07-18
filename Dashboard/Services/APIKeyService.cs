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
    public abstract class APIKeyService : Service
    {
        [PersistentConfig]
        public virtual string ApiKey { get; set; } = "";
    }
}
