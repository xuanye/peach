using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Hey.Infrastructure;

namespace Hey.Hosting
{
    public class HeyHostedService : IHostedService 
    {
        private readonly IServerBootstrap _server;
        public HeyHostedService(IServerBootstrap server)
        {
            Preconditions.CheckNotNull(server, nameof(server));
            this._server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this._server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this._server.StopAsync(cancellationToken);
        }
       
    }
}