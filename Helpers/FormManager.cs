using Microsoft.Extensions.DependencyInjection;
using Ritrama2025.Forms.Otros;
using Sunny.UI;
using System.Drawing;
using System.Windows.Forms;

namespace Ritrama2025.Helpers
{
    public sealed class FormManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Form> _forms = [];
        private readonly Dictionary<Type, TabPage> _tabs = [];
        private bool _hostWired;

        // Contenedor de pestanas central ( junto al sidebar ). Si es null,
        // los formularios se muestran como ventanas independientes (fallback).
        public UITabControl? HostTabControl { get; set; }

        public FormManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public T? ShowForm<T>() where T : Form
        {
            var type = typeof(T);

            // verificar si ya existe y esta abierto
            if (_forms.TryGetValue(type, out var existingForm))
            {
                if (!existingForm.IsDisposed)
                {
                    if (HostTabControl != null && _tabs.TryGetValue(type, out var existingTab))
                    {
                        // Solo seleccionamos la pestana; el TabControl se encarga de
                        // mostrar/ocultar el form embebido. Llamar BringToFront/Activate
                        // sobre un form TopLevel=false puede dejarlo "montado" al volver.
                        HostTabControl.SelectedTab = existingTab;
                        existingTab.Invalidate();
                        existingForm.Refresh();
                    }
                    else if (existingForm.TopLevel)
                    {
                        existingForm.Show();
                        existingForm.BringToFront();
                        existingForm.Activate();
                    }
                }
                else
                {
                    _forms.Remove(type);
                    _tabs.Remove(type);
                }
                return existingForm is T typed ? typed : default;
            }

            // crear una nueva instancia usando DI - explicit dependency via provider
            var form = _serviceProvider.GetRequiredService<T>();

            // Icono por defecto (el del ejecutable) si el form no trae uno,
            // para que la pestana muestre un icono.
            if (form.Icon == null)
            {
                try { form.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            }

            _forms[type] = form;

            // Modo incrustado en pestana central (junto al sidebar).
            if (HostTabControl != null)
            {
                EnsureHostWired();

                // Si el formulario carga datos de forma asincrona, NO lo agregamos a la
                // pestana todavia. Mostramos un FrmLoading mientras llama a
                // InitializeAsync(), y recien cuando termina se crea la pestana y se
                // muestra el form ya pintado. Esto elimina la "pantalla en blanco" y el
                // efecto de zona que crece / minimizado a maximizado durante la carga.
                if (form is IAsyncFormLoad asyncForm)
                {
                    _ = CargarYMostrarAsync(form, asyncForm, type);
                    return form;
                }

                var tab = new TabPage("\u00A0" + (form.Text ?? type.Name));
                tab.Tag = form;
                tab.Padding = new Padding(0);
                tab.Margin = new Padding(0);
                if (form.Icon != null)
                {
                    HostTabControl.ImageList ??= new ImageList();
                    HostTabControl.ImageList.Images.Add(form.Icon.ToBitmap());
                    tab.ImageIndex = HostTabControl.ImageList.Images.Count - 1;
                }
                form.TopLevel = false;
                form.ShowInTaskbar = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.ControlBox = false;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.Text = string.Empty;
                form.Dock = DockStyle.Fill;
                form.AutoScroll = false;
                form.Padding = new Padding(0);
                form.Margin = new Padding(0);
                form.WindowState = FormWindowState.Normal;
                form.Location = Point.Empty;
                form.Size = tab.DisplayRectangle.Size;
                tab.Controls.Add(form);

                HostTabControl.TabPages.Add(tab);
                HostTabControl.SelectedTab = tab;
                HostTabControl.Visible = true;
                HostTabControl.BringToFront();

                _tabs[type] = tab;

                form.FormClosed += (s, e) =>
                {
                    if (_tabs.TryGetValue(type, out var closedTab))
                    {
                        HostTabControl.TabPages.Remove(closedTab);
                        _tabs.Remove(type);
                    }
                    _forms.Remove(type);
                    if (!form.IsDisposed)
                    {
                        form.Dispose();
                    }
                };

                form.Show();
                if (form is IFormTemaClaro temaClaro) temaClaro.ReaplicarTema();
                else TemaOscuroHelper.Aplicar(form);
                return form is T typed2 ? typed2 : default;
            }

            // Fallback: ventana independiente.
            form.WindowState = FormWindowState.Normal;
            form.Show();
            form.BringToFront();

            form.FormClosed += (s, e) =>
            {
                _forms.Remove(type);
            };

            return form is T typed3 ? typed3 : default;
        }

        /// <summary>
        /// Muestra un FrmLoading mientras el form termina su carga (InitializeAsync) y,
        /// al concluir, crea la pestana y muestra el form ya pintado. Se ejecuta en el
        /// hilo de UI gracias a que parte de la llamada de ShowForm (mismo hilo).
        /// </summary>
        private async Task CargarYMostrarAsync(Form form, IAsyncFormLoad asyncForm, Type type)
        {
            var tabHost = HostTabControl!;
            var owner = tabHost.FindForm();
            FrmLoading loading = new("Cargando datos...", titulo: form.Text);
            try
            {
                if (owner != null && !owner.IsDisposed)
                    loading.Show(owner);
                else
                    loading.Show();
                loading.BringToFront();

                // Carga datos y pinta los controles. Al ser el hilo de UI, los
                // DataBindings y grids funcionan sin mostrar el form todavia.
                await asyncForm.InitializeAsync();

                if (form.IsDisposed || tabHost.IsDisposed) return;

                // Crear la pestana y agregar el form ya cargado.
                var tab = CrearTabParaForm(form, type);
                _tabs[type] = tab;
                form.FormClosed += (_, e) =>
                {
                    if (_tabs.TryGetValue(type, out var closedTab))
                    {
                        tabHost.TabPages.Remove(closedTab);
                        _tabs.Remove(type);
                    }
                    _forms.Remove(type);
                    if (!form.IsDisposed) form.Dispose();
                };

                tabHost.TabPages.Add(tab);
                tabHost.SelectedTab = tab;
                tabHost.Visible = true;
                tabHost.BringToFront();

                // Ajustar la geometria del form embebido al area visible de la pestana
                // ANTES de mostrar, para que no parpadee en su tamano de diseno (efecto
                // minimizado->maximizado) al arrancar.
                form.Location = Point.Empty;
                form.Size = tab.DisplayRectangle.Size;

                // Mostrar el form embebido (TopLevel=false) dentro de un layout suspendido
                // para que quede ya dimensionado y centrado; evitamos BringToFront/Activate
                // para no dejarlo "montado" al cambiar y volver.
                form.SuspendLayout();
                form.Show();
                form.Dock = DockStyle.Fill;
                form.ResumeLayout(true);
                form.PerformLayout();

                // Aplica el tema oscuro ANTES del primer Refresh para pisar el
                // re-estilizado naranja del UIStyleManager de SunnyUI antes del
                // primer paint visible. Si se aplica despues del Refresh, el primer
                // frame se pinta del default naranja y luego se repinta (flash).
                // Los forms con tema propio reaplican el suyo (p.ej. el verde de
                // Orden de Corte).
                if (form is IFormTemaClaro temaClaro) temaClaro.ReaplicarTema();
                else TemaOscuroHelper.Aplicar(form);
                form.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el formulario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loading.Close();
            }
        }

        private TabPage CrearTabParaForm(Form form, Type type)
        {
            var tab = new TabPage("\u00A0" + (form.Text ?? type.Name));
            tab.Tag = form;
            tab.Padding = new Padding(0);
            tab.Margin = new Padding(0);
            var tabHost = HostTabControl!;
            if (form.Icon != null)
            {
                tabHost.ImageList ??= new ImageList();
                tabHost.ImageList.Images.Add(form.Icon.ToBitmap());
                tab.ImageIndex = tabHost.ImageList.Images.Count - 1;
            }
            form.TopLevel = false;
            form.ShowInTaskbar = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.ControlBox = false;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Text = string.Empty;
            form.Dock = DockStyle.Fill;
            form.AutoScroll = true;
            form.Padding = new Padding(0);
            form.Margin = new Padding(0);
            form.WindowState = FormWindowState.Normal;
            tab.Controls.Add(form);
            return tab;
        }

        private void EnsureHostWired()
        {
            if (_hostWired || HostTabControl == null) return;

            // Boton "x" en cada pestana para cerrarla.
            HostTabControl.ShowCloseButton = true;
            HostTabControl.ShowActiveCloseButton = false;

            // Cuando SunnyUI quita la pestana (click en la x), cerramos el form
            // embebido que guardamos en TabPage.Tag y limpiamos los diccionarios.
            HostTabControl.ControlRemoved += (_, e) =>
            {
                if (e.Control is TabPage tp && tp.Tag is Form f)
                {
                    _tabs.Remove(f.GetType());
                    _forms.Remove(f.GetType());
                    if (!f.IsDisposed)
                    {
                        f.Dispose();
                    }
                }
            };

            _hostWired = true;
        }

        public void CleanupForm(Form form)
        {
            if (form == null || form.IsDisposed) return;
            Type formType = form.GetType();

            try
            {
                _forms.Remove(formType);
                form.Close();
                form.Dispose();
                CloseAllForms();
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
        }

        public void CloseAllForms()
        {
            foreach (var form in new List<Form>(_forms.Values))
            {
                if (!form.IsDisposed)
                {
                    form.Close();
                }
            }

            _forms.Clear();
        }
    }
}