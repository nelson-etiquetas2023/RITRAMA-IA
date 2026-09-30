using System.Drawing;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.DespachoService.DespachoService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.InventarioService;
using Ritrama2025.Services.MateriaPrima;
using Ritrama2025.Services.PedidoService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Ritrama2025
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // SunnyUI usa sus propios recursos (registramos español en Main.cs).
            // UICulture=es-ES para cualquier texto de framework; Culture=es-ES mantiene
            // el formato de números/fechas de la app en español.
            CultureInfo culturaUI = new CultureInfo("es-ES");
            CultureInfo culturaApp = new CultureInfo("es-ES");
            CultureInfo.DefaultThreadCurrentUICulture = culturaUI;
            CultureInfo.DefaultThreadCurrentCulture = culturaApp;
            Thread.CurrentThread.CurrentUICulture = culturaUI;
            Thread.CurrentThread.CurrentCulture = culturaApp;

            // dotnet-winforms-basics: High-DPI PerMonitorV2 antes de Initialize (recomendado para multi-monitor)
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();

            // Tema global SunnyUI (cambiar UIStyle para otro look: Blue, Dark, DarkBlue, Colorful, etc.)
            // Debe configurarse ANTES de crear el primer IWin32Window (el form Main se resuelve por DI abajo).
            UIStyles.SetStyle(UIStyle.Blue);
            Application.SetDefaultFont(new Font("JetBrains Mono", 10.5F, FontStyle.Regular, GraphicsUnit.Point));

            // Entorno: Host usa "Production" por defecto. En compilación Debug arrancamos
            // como "Development" (salvo que se defina DOTNET_ENVIRONMENT / ASPNETCORE_ENVIRONMENT),
            // para no cargar appsettings.Production.json al programar. Release sigue en Production.
            string? environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                                 ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
#if DEBUG
            environmentName ??= "Development";
#endif
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                EnvironmentName = environmentName
            });

            // Configuración: appsettings.json + environment + User Secrets (Development) + env vars
            builder.Configuration
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Main>(optional: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Configuration;
            builder.Services.AddSingleton<IConfiguration>(configuration);

            // Servicios estándar de la App
            builder.Services.AddTransient<IServiceCommonData, ServiceDataCommon>();
            builder.Services.AddTransient<IServiceMateriaPrima, ServiceMateriaPrima>();
            builder.Services.AddTransient<IProduccionService, ProduccionService>();
            builder.Services.AddTransient<IOrdenCorteService, OrdenCorteService>();
            builder.Services.AddTransient<IConsecutivosService, ConsecutivosService>();
            builder.Services.AddTransient<IConsumoMasterService, ConsumoMasterService>();
            builder.Services.AddTransient<IDespachoService, DespachoService>();
            builder.Services.AddTransient<IReportsService, ReportsService>();
            builder.Services.AddTransient<ICommonService, CommonService>();
            builder.Services.AddTransient<IExportDataService, ExportDataService>();
            builder.Services.AddTransient<IProductsService, ProductsService>();
            builder.Services.AddTransient<IPedidoService, PedidoService>();
            builder.Services.AddTransient<IInventarioService, InventarioService>();
            builder.Services.AddTransient<IReconciliacionService, ReconciliacionService>();
            builder.Services.AddTransient<IOperacionLogService, OperacionLogService>();
            builder.Services.AddTransient<ILogViewerService, LogViewerService>();
            builder.Services.AddSingleton<IAuditoriaStore, AuditoriaStore>();

            // Servicio de Seguridad
            builder.Services.AddTransient<ISeguridadService, SeguridadService>();

            builder.Services.AddSingleton<FormManager>();
            // Forms registrados para DI
            builder.Services.AddTransient<Main>();
            builder.Services.AddTransient<FrmMateriaPrima>();
            builder.Services.AddTransient<FrmDespacho>();
            builder.Services.AddTransient<FrmCodeBarLabel>();
            builder.Services.AddTransient<FrmOrdenCorte>();
            builder.Services.AddTransient<FrmProductos>();
            builder.Services.AddTransient<Frm_Inventarios>();
            builder.Services.AddTransient<FrmPedidos>();
            builder.Services.AddTransient<FrmClientes>();
            builder.Services.AddTransient<FrmProveedores>();
            builder.Services.AddTransient<FrmLogViewer>();
            builder.Services.AddTransient<FrmLogin>();
            builder.Services.AddTransient<FrmUsuarios>();
            builder.Services.AddTransient<FrmRoles>();
            builder.Services.AddTransient<FrmAuditoriaInconsistencias>();
            builder.Services.AddTransient<FrmCambiarContrasena>();

            using IHost host = builder.Build();

            using IServiceScope scope = host.Services.CreateScope();
            IServiceProvider serviceProvider = scope.ServiceProvider;

            // Mostrar login antes de abrir Main
            ISeguridadService seguridadService = serviceProvider.GetRequiredService<ISeguridadService>();
            using (FrmLogin loginForm = serviceProvider.GetRequiredService<FrmLogin>())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                SesionActual.Usuario = loginForm.UsuarioAutenticado;
                SesionActual.Permisos = loginForm.Permisos;

                // Si es primer login, mostrar cambio de contraseña
                if (SesionActual.Usuario!.PrimerLogin)
                {
                    using FrmCambiarContrasena frmCambiar = serviceProvider.GetRequiredService<FrmCambiarContrasena>();
                    if (frmCambiar.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            Main main = serviceProvider.GetRequiredService<Main>();
            Application.Run(main);
        }
    }
}
