// Taken from https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/master/SpotifyAPI.Web.Auth/EmbedIOAuthServer.cs

using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dashboard.Utilities.Auth.Models;
using EmbedIO;
using EmbedIO.Actions;

namespace Dashboard.Utilities.Auth;

public class EmbedIOAuthServer : IDisposable
{
    public event Func<object, AuthorizationCodeResponse, Task>? AuthorizationCodeReceived;
    public event Func<object, ImplicitGrantResponse, Task>? ImplicitGrantReceived;

    private const string assets_resource_path = "Dashboard.Assets.auth_assets";
    private const string default_resource_path = "Dashboard.Assets.default_site";

    private CancellationTokenSource? cancelTokenSource;
    private readonly WebServer webServer;

    public EmbedIOAuthServer(Uri baseUri, int port)
        : this(baseUri, port, Assembly.GetExecutingAssembly(), default_resource_path)
    {
    }

    public EmbedIOAuthServer(Uri baseUri, int port, Assembly resourceAssembly, string resourcePath)
    {
        BaseUri = baseUri;
        Port = port;

        webServer = new WebServer(port)
                    .WithModule(
                        new ActionModule(
                            "/", HttpVerbs.Post, (ctx) =>
                            {
                                NameValueCollection query = ctx.Request.QueryString;
                                if (query["error"] != null)
                                {
                                    throw new AuthException(query["error"], query["state"]);
                                }

                                string requestType = query.Get("request_type");
                                switch (requestType)
                                {
                                    case "token":
                                        ImplicitGrantReceived?.Invoke(
                                            this, new ImplicitGrantResponse(
                                                query["access_token"], query["token_type"], int.Parse(query["expires_in"])
                                            )
                                            {
                                                State = query["state"],
                                            }
                                        );

                                        break;
                                    case "code":
                                        AuthorizationCodeReceived?.Invoke(
                                            this, new AuthorizationCodeResponse(query["code"])
                                            {
                                                State = query["state"],
                                            }
                                        );

                                        break;
                                }

                                return ctx.SendStringAsync("OK", "text/plain", Encoding.UTF8);
                            }
                        )
                    )
                    .WithEmbeddedResources("/auth_assets", Assembly.GetExecutingAssembly(), assets_resource_path)
                    .WithEmbeddedResources(baseUri.AbsolutePath, resourceAssembly, resourcePath);
    }

    public Uri BaseUri { get; }
    public int Port { get; }

    public Task Start()
    {
        cancelTokenSource = new CancellationTokenSource();
        webServer.Start(cancelTokenSource.Token);

        return Task.CompletedTask;
    }

    public Task Stop()
    {
        cancelTokenSource?.Cancel();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            webServer?.Dispose();
        }
    }
}
