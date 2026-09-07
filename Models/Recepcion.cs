namespace Ritrama2025.Models
{
    public class Recepcion
    {
        public int Numero { get; set; }

        public string Supply_Id { get; set; } = null!;

        public DateTime Fecha_reg { get; set; }

        public string Hora_reg { get; set; } = null!;

        public string SupplyName { get; set; } = null!;

        public string Orden { get; set; } = null!;

        public double Width { get; set; }

        public double Lenght { get; set; }

        public double Width_metros { get; set; }

        public double Lenght_metros { get; set; }

        public int Splice { get; set; }

        public decimal Core { get; set; }

        public string Roll_ID { get; set; } = null!;

        public string Ubicacion { get; set; } = null!;

        public string Part_Number { get; set; } = null!;

        public string Unidad { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public bool Anulado { get; set; }

        public DateTime? Fecha_produccion { get; set; }

        public DateTime Fecha_recepcion { get; set; }

        public bool Master { get; set; }

        public bool Resma { get; set; }

        public bool Graphics { get; set; }

        public bool Rollo { get; set; }

        public string Embarque { get; set; } = null!;

        public string Palet_number { get; set; } = null!;

        public decimal Palet_cant { get; set; }

        public int Palet_paginas { get; set; }

        public string Tipo { get; set; } = null!;

        public int Num_sincro { get; set; }

        public bool Registro_movil { get; set; }

        public bool Disponible { get; set; }

        public double Cant_despacho { get; set; }

        public double Kilos { get; set; }

        public string Status { get; set; } = null!;

        public double Metros { get; set; }

        public double Width_c { get; set; }

        public double Lenght_c { get; set; }

        public double Width_mm { get; set; }

        public double Length_mm { get; set; }

        public string No_paleta { get; set; } = null!;
    }
}
