using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#if DEBUG
using System.Diagnostics;
using System.Linq;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.WindowsServices;
#endif

using captcha;

namespace lingvo.syntax.webService
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Startup
    {
        public const string INDEX_PAGE_PATH = "/index.html";

        public static void ConfigureServices( IServiceCollection services )
        {
            services.AddControllers()
                    .AddCaptchaController()
                    .AddJsonOptions( opts =>
                    {
                        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                        opts.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter() );
                    });

            services.Configure< IISServerOptions >( opts => opts.MaxRequestBodySize = int.MaxValue );
            services.Configure< KestrelServerOptions >( opts => opts.Limits.MaxRequestBodySize = int.MaxValue );
            services.Configure< FormOptions >( opts =>
            {
                opts.ValueLengthLimit            = int.MaxValue;
                opts.MultipartBodyLengthLimit    = int.MaxValue; // if don't set default value is: 128 MB
                opts.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.AddMvc();
        }

        public static void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute( name: "default", pattern: $"{{controller}}/{{action}}/{{id?}}" );
            });

            app.Use( async (ctx, next) =>
            {
                await next( ctx );
                
                if ( (ctx.Response.StatusCode == 404) && (ctx.Request.Path == INDEX_PAGE_PATH) )
                {
                    ctx.Response.Redirect( INDEX_PAGE_PATH );
                }
            });
            //-------------------------------------------------------------//
#if DEBUG
            OpenBrowserIfRunAsConsole( app );
#endif
        }
#if DEBUG
        private static void OpenBrowserIfRunAsConsole( IApplicationBuilder app )
        {
            #region [.open browser if run as console.]
            if ( !WindowsServiceHelpers.IsWindowsService() ) //IsRunAsConsole
            {
                var server    = app.ApplicationServices.GetRequiredService< IServer >();
                var addresses = server.Features?.Get< IServerAddressesFeature >()?.Addresses;
                var address   = addresses?.FirstOrDefault( a => a.StartsWith( "https:" ) ) ?? addresses?.FirstOrDefault();
                
                if ( address == null )
                {
                    var config = app.ApplicationServices.GetService< IConfiguration >();
                    address = config.GetSection( "Kestrel:Endpoints:Https:Url" ).Value ??
                              config.GetSection( "Kestrel:Endpoints:Http:Url"  ).Value;
                }

                if ( address != null )
                {
                    address = address.Replace( "/*:", "/localhost:" );

                    using ( Process.Start( new ProcessStartInfo( address.TrimEnd( '/' ) + INDEX_PAGE_PATH ) { UseShellExecute = true } ) ) { };
                }
            }
            #endregion
        }
#endif
    }
}
