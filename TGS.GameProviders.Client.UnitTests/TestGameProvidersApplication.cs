using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TGS.GameProviders.Client.IntegrationTests
{
    internal class TestGameProvidersApplication : WebApplicationFactory<Startup>
    {
        private readonly Action<IWebHostBuilder>? _webHostConfigure;

        public TestGameProvidersApplication(Action<IWebHostBuilder>? webHostConfigure = null)
        {
            _webHostConfigure = webHostConfigure;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => _webHostConfigure?.Invoke(builder);
    }
}