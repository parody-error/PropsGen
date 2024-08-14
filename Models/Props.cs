using System.Security.RightsManagement;

namespace PropsGen.Models
{
    internal class BasicReservoir
    {
        public static readonly int FIELD_COUNT = 8;
    }

    internal class GasProps
    {
        public static readonly int FIELD_COUNT = 4;

        public double EUR { get; set; }
        public double S_G { get; set; }
        public double H_2_S { get; set; }
        public double C_O_2 { get; set; }
    }

    internal class Parameters
    {
        public double temperature { get; set; }
        public double pressure { get; set; }
    }

    internal class Props
    {
        // Total number of fields to be read from the database.
        public static readonly int FIELD_COUNT = GasProps.FIELD_COUNT;

        public GasProps gas { get; set; } = new GasProps();
        public Parameters parameters { get; set; } = new Parameters();
    }
}
