using System.Drawing;
using System.Windows.Forms;

namespace Ritrama2025.Helpers
{
    /// <summary>
    /// Normaliza las barras de herramientas del sistema para que el icono y el texto
    /// de cada boton siempre entren completos.
    ///
    /// El disenador deja los ToolStripButton con tamano fijo (por ejemplo 92x24) y
    /// <c>ImageScaling = None</c>. Como los bitmaps de Resources vienen en tamanos
    /// dispares (16, 24, 32 y 48 px), el icono mas grande se salia del alto del boton y
    /// el texto quedaba recortado ("Exportar" se leia "Export"). Ademas, cuando la barra
    /// vive dentro de un TabPage con posicion absoluta, se cortaba al cambiar el tamano.
    ///
    /// Se aplica desde el codigo del formulario y no desde el .Designer.cs a proposito:
    /// asi el ajuste sobrevive a una regeneracion del disenador.
    /// </summary>
    public static class ToolStripTheme
    {
        /// <summary>Tamano unico al que se escalan todos los iconos de la barra.</summary>
        public static readonly Size DefaultIconSize = new(24, 24);

        /// <summary>
        /// Deja los botones en auto-ajuste, con icono y padding uniformes, y estira la
        /// barra en horizontal cuando no esta dockeada (caso de las barras dentro de un TabPage).
        /// </summary>
        public static void Ajustar(ToolStrip barra, Size? iconSize = null)
        {
            ArgumentNullException.ThrowIfNull(barra);

            barra.ImageScalingSize = iconSize ?? DefaultIconSize;
            barra.AutoSize = false;

            foreach (ToolStripItem item in barra.Items)
            {
                if (item is not ToolStripButton boton)
                {
                    continue;
                }

                // AutoSize + SizeToFit: el boton mide lo que miden su icono y su texto,
                // nunca menos. ImageScaling.None es lo que recortaba ambos.
                boton.AutoSize = true;
                boton.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                boton.Margin = new Padding(4, 2, 2, 2);
            }

            // La barra no se dockea porque comparte fila con los filtros del formulario;
            // con anclaje horizontal sigue el ancho del TabPage en vez de quedar cortada.
            if (barra.Dock == DockStyle.None)
            {
                barra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }

            AjustarAlto(barra);

            // Recalcular el ancho de los items. Poner AutoSize = true no alcanza: el
            // ToolStrip reparte el espacio una sola vez y despues no vuelve a mirar el
            // tamano preferido de cada item, asi que el boton se queda con el ancho fijo
            // del disenador y el texto sigue recortado.
            barra.PerformLayout();
        }

        /// <summary>
        /// Agranda el alto de la barra si el contenido (icono + margins) no cabe en el
        /// alto fijo que dejo el disenador.
        /// </summary>
        public static void AjustarAlto(ToolStrip barra)
        {
            ArgumentNullException.ThrowIfNull(barra);

            // PreferredSize de la barra ya incluye el alto de sus items visibles y el
            // padding: no hace falta sumar nada.
            int altoNecesario = barra.PreferredSize.Height;
            if (barra.Height < altoNecesario)
            {
                barra.Height = altoNecesario;
            }
        }

        /// <summary>
        /// Sube el ancho minimo del formulario para que la barra completa entre aun con
        /// la ventana reduceda al minimo. Sin esto el ultimo boton se sale de la pantalla.
        /// </summary>
        public static void AjustarAnchoMinimo(Form form, ToolStrip barra, int margen = 8)
        {
            ArgumentNullException.ThrowIfNull(form);
            ArgumentNullException.ThrowIfNull(barra);

            int anchoRequerido = barra.PreferredSize.Width + barra.Padding.Horizontal + margen;
            if (anchoRequerido > form.MinimumSize.Width)
            {
                form.MinimumSize = new Size(anchoRequerido, form.MinimumSize.Height);
            }
        }
    }
}