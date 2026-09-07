using System.Globalization;
using Sunny.UI;

namespace Ritrama2025
{
    /// <summary>
    /// Recursos de SunnyUI en español. La librería solo trae zh-CN (chino) y en-US (inglés),
    /// así que registramos esta clase para que los controles (datepicker, grillas, diálogos,
    /// paginación, etc.) se muestren en español en lugar de chino.
    /// </summary>
    public class es_ES_Resources : UIBuiltInResources
    {
        public override CultureInfo CultureInfo => new CultureInfo("es-ES");

        public override string InfoTitle { get; set; } = "Información";
        public override string SuccessTitle { get; set; } = "Correcto";
        public override string WarningTitle { get; set; } = "Advertencia";
        public override string ErrorTitle { get; set; } = "Error";
        public override string AskTitle { get; set; } = "Confirmar";
        public override string InputTitle { get; set; } = "Entrada";
        public override string SelectTitle { get; set; } = "Seleccionar";
        public override string CloseAll { get; set; } = "Cerrar todo";
        public override string OK { get; set; } = "Aceptar";
        public override string Cancel { get; set; } = "Cancelar";
        public override string Yes { get; set; } = "Sí";
        public override string No { get; set; } = "No";
        public override string GridNoData { get; set; } = "[ Sin datos ]";
        public override string GridDataLoading { get; set; } = "Cargando datos, por favor espere...";
        public override string GridDataSourceException { get; set; } = "El origen de datos debe ser DataTable o List";
        public override string SystemProcessing { get; set; } = "El sistema está procesando, por favor espere...";
        public override string Monday { get; set; } = "LUN";
        public override string Tuesday { get; set; } = "MAR";
        public override string Wednesday { get; set; } = "MIÉ";
        public override string Thursday { get; set; } = "JUE";
        public override string Friday { get; set; } = "VIE";
        public override string Saturday { get; set; } = "SÁB";
        public override string Sunday { get; set; } = "DOM";
        public override string Prev { get; set; } = "Ant";
        public override string Next { get; set; } = "Sig";
        public override string SelectPageLeft { get; set; } = "Página";
        public override string SelectPageRight { get; set; } = "";
        public override string January { get; set; } = "Ene.";
        public override string February { get; set; } = "Feb.";
        public override string March { get; set; } = "Mar.";
        public override string April { get; set; } = "Abr.";
        public override string May { get; set; } = "May";
        public override string June { get; set; } = "Jun.";
        public override string July { get; set; } = "Jul.";
        public override string August { get; set; } = "Ago.";
        public override string September { get; set; } = "Sep.";
        public override string October { get; set; } = "Oct.";
        public override string November { get; set; } = "Nov.";
        public override string December { get; set; } = "Dic.";
        public override string Today { get; set; } = "Hoy";
        public override string Search { get; set; } = "Buscar";
        public override string Clear { get; set; } = "Limpiar";
        public override string Open { get; set; } = "Abrir";
        public override string Save { get; set; } = "Guardar";
        public override string All { get; set; } = "Todo";
        public override string EditorCantEmpty { get; set; } = "El contenido del editor no puede estar vacío.";
    }
}
