namespace PropsGen.Models
{
    internal class BasicReservoir
    {
        public static readonly int FIELD_COUNT = 0;
    }

    internal class GasProps
    {
        public static readonly int FIELD_COUNT = 12;

        public int pvtCorrelation { get; set; }
        public int viscosityCorrelation { get; set; }
        public int gasType { get; set; }
        public int rvCorrelation { get; set; }
        public double separatorSpecificGravity { get; set; }
        public double CO2 { get; set; }
        public double N2 { get; set; }
        public double H2S { get; set; }
        public double separatorPressure { get; set; }
        public double separatorTemperature { get; set; }
        public double condensateGasRatio { get; set; }
        public double rvOverRvSat { get; set; }
    }

    internal class Parameters
    {
        public double temperature { get; set; }
        public double pressure { get; set; }
    }

    internal class Props
    {
        public GasProps gas { get; set; } = new GasProps();

        public Parameters parameters { get; set; } = new Parameters();
    }
}
