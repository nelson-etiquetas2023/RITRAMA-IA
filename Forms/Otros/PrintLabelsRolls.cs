using System.ComponentModel;
using System.Drawing.Printing;
using Ritrama2025.LabelSdk;
using Ritrama2025.Models;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class PrintLabelsRolls : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<RolloCortado> Rollos { get; set; } = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Fechapro { get; set; } = string.Empty;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Orden_Corte { get; set; } = string.Empty;
        public PrintLabelsRolls()
        {
            InitializeComponent();
        }

        private void PrintLabelsRolls_Load(object sender, EventArgs e)
        {
            txt_numero_etiq.Text = Rollos.Count().ToString();
            txt_desde.Text = "1";
            txt_hasta.Text = Rollos.Count().ToString();
            LoadPrinters();
        }

        private void LoadPrinters()
        {
            CboSelectPrinters.Items.Clear();
            foreach (string impresora in PrinterSettings.InstalledPrinters)
            {
                CboSelectPrinters.Items.Add(impresora);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CboSelectPrinters.SelectedItem == null)
            {
                MessageBox.Show("seleccione una impresora primero...");
                return;
            }

            if (!int.TryParse(txt_desde.Text, out int label_init) ||
                !int.TryParse(txt_hasta.Text, out int label_end))
            {
                MessageBox.Show("Los valores Desde y Hasta deben ser numericos...");
                return;
            }
            if (label_init < 1) label_init = 1;
            if (label_end > Rollos.Count) label_end = Rollos.Count;
            if (label_init > label_end)
            {
                MessageBox.Show("El rango de etiquetas no es valido...");
                return;
            }

            for (int i = label_init; i <= label_end; i++)
            {
                string template = @"^XA^POR
                ^FO700,70^A0R,60,60^FDFEDRIGONI^FS
                ^FO750,470^A0R,30,20^FDPRODUCT ID:^FS
                ^FO750,750^A0R,30,20^FDROLL ID:^FS

                ^FO670,450^A0R,70,70^FD{product_id}^FS
                ^FO670,750^A0R,70,70^FD{rollid}^FS

                ^FO680,50^GB1,1200,3,1^FS
                ^FO680,400^GB200,3,1^FS
                ^FO680,700^GB200,3,1^FS

                ^FO635,50^A0R,30,20^FDPRODUCTO^FS
                ^FO635,1000^A0R,30,20^FDFECHA^FS

                ^FO580,50^A0R,50,60^FD{product_name}^FS
                ^FO580,1000^A0R,50,50^FD{fecha}^FS
  
                ^FO550,50^GB1,1200,3,1^FS
                ^FO550,950^GB130,3,1^FS
            
                ^FO510,50^A0R,30,20^FDWIDTH (Inch):^FS
                ^FO510,350^A0R,30,20^FDLENGTH (Pies):^FS
                ^FO510,650^A0R,30,20^FDMSI:^FS
                ^FO510,1000^A0R,30,20^FDSPLICE:^FS

                ^FO440,50^A0R,60,60^FD{width}^FS
                ^FO440,320^A0R,60,60^FD{lenght}^FS
                ^FO440,650^A0R,60,60^FD{msi}^FS
                ^FO440,1000^A0R,60,60^FD{splice}^FS

                ^FO420,50^GB1,1200,3,1^FS
                ^FO420,300^GB130,3,1^FS
                ^FO420,600^GB130,3,1^FS
                ^FO420,950^GB130,3,1^FS

                ^FO380,50^A0R,30,20^FDSTATUS:^FS
                ^FO380,280^A0R,30,20^FDCUSTOMER ID:^FS
                ^FO380,650^A0R,30,20^FDORDEN CORTE:^FS
                ^FO380,1000^A0R,30,20^FDROLL NUMBER:^FS

                ^FO300,50^A0R,60,60^FD{status}^FS
                ^FO300,350^A0R,60,60^FD{customer_id}^FS
                ^FO300,720^A0R,60,60^FD{orden}^FS
                ^FO300,1000^A0R,60,60^FD{roll_number}^FS

                ^FO280,50^GB1,1200,3,1^FS            
                ^FO280,250^GB140,3,1^FS
                ^FO280,600^GB140,3,1^FS
                ^FO280,950^GB140,3,1^FS

                ^FO250,210^A0R,25,25^FDPRODUCT ID:^FS
                ^FO250,900^A0R,25,25^FDUNIQUE CODE:^FS

                ^FO50,600^GB235,3,1^FS
                                    
                ^FO50,120     
                ^BY4,3,80
                ^BCR,150,Y,N,N
                ^FD{product_id}^FS
 

                ^FO50,750
                ^BY4,3,80                
                ^BCR,150,Y,N,N
                ^FD{unique_code}^FS

                ^XZ";

                var rollo = Rollos[i - 1];
                string product_id = rollo.Product_Id.Trim();
                string productName = rollo.Product_Name.Length > 30 ? rollo.Product_Name.Substring(0, 30) : rollo.Product_Name;
                string rollid = rollo.Roll_Id;
                string width = rollo.Width.ToString("F3");
                string length = rollo.Length.ToString("F0");
                string msi = rollo.Msi.ToString("F0");
                string splice = rollo.Splice.ToString();
                string status = rollo.Status.Length > 3 ? rollo.Status.Substring(0, 2) : rollo.Status;
                string code_person = rollo.Code_Person;
                string roll_number = rollo.RollNumber.ToString();
                string unique_code = rollo.UniqueCode.Trim();

                var values = new Dictionary<string, string>
                {
                    { "product_id", "0" + product_id },
                    { "product_name", productName },
                    { "rollid", rollid },
                    { "fecha", Fechapro.ToString() },
                    { "width", width },
                    { "lenght", length  },
                    { "msi", msi },
                    { "splice", splice  },
                    { "status", status  },
                    { "customer_id", code_person },
                    { "orden", Orden_Corte },
                    { "roll_number", roll_number },
                    { "unique_code", unique_code },
                    { "Codigo", product_id }
                };

                string? printerSelection = CboSelectPrinters.SelectedItem?.ToString();

                bool ok = ZebraTemplateEngine.Print(printerSelection!, template, values, StandardLabelSizes.Size_4x6_203dpi);
            }

        }
    }
}


