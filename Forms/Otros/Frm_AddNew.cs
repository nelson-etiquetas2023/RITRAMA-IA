using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using Ritrama2025.Services.CommonService;


using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class Frm_AddNew : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TitleForm { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable Dt { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NombreEntidad { get; set; } = null!;

        private readonly Dictionary<string, (string Idcolumn, string NameColumn, Action<string, string> SaveAction)> entidades;

        public Frm_AddNew(ICommonService servicio)
        {
            InitializeComponent();
            entidades = new()
                {
                    { "Transporte", ("transport_id", "transport_name", servicio.SaveTransportEntity) },
                    { "chofer", ("chofer_id", "chofer_name", servicio.SaveChoferEntity) },
                    { "camion", ("placas_id", "camion_name", servicio.SaveCamionEntity) },
                    { "Persona", ("person_id", "person_name", servicio.SavePersonEntity) },
                    { "Proveedor", ("proveedor_id", "proveedor_name", servicio.SaveProvaiderEntity) },
                    { "operadores", ("operador_id", "nombre", servicio.SaveOperatorEntity) },
                    { "clientes", ("customer_id", "customer_name", servicio.SaveCustomerEntity) },
                    { "Vendedores", ("vendor_id", "vendor_name", servicio.SaveVendedorEntity) }
                };
        }

        private void Frm_AddNew_Load(object sender, EventArgs e)
        {
            Titulo.Text = TitleForm;
        }

        private void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (entidades.TryGetValue(NombreEntidad, out (string Idcolumn, string NameColumn, Action<string, string> SaveAction) entidad))
                {
                    Guid ConsecGuid = Guid.NewGuid();
                    string Consecutivo = ConsecGuid.ToString();
                    DataRow dr = Dt.NewRow();
                    dr[entidad.Idcolumn] = Consecutivo.ToString();
                    dr[entidad.NameColumn] = txt_name.Text.ToUpper();
                    Dt.Rows.Add(dr);
                    entidad.SaveAction(Consecutivo.ToString(), txt_name.Text.ToUpper());
                    Close();
                }
            }
            catch (SqlException Ex)
            {
                MessageBox.Show("Error al crear las entidades..." + Ex);
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}


