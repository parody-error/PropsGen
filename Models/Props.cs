namespace PropsGen.Models
{
    internal class ReservoirProps
    {
        public static readonly int FIELD_COUNT = 8;

        public double temperature { get; set; }
        public double pressure { get; set; }
        public double netPay { get; set; }
        public double porosity { get; set; }
        public double gasSaturation { get; set; }
        public double oilSaturation { get; set; }
        public double waterSaturation { get; set; }
        public double initialFormationCompressibility { get; set; }
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

internal class OilProps
{
    public static readonly int FIELD_COUNT = 5;

    public int pvtCorrelation { get; set; }
    public int viscosityCorrelation { get; set; }
    public double apiGravity { get; set; }
    public double initialSaturationPressure { get; set; }
    public double initialSolutionGasOilRatio { get; set; }
}

internal class WaterProps
{
    public static readonly int FIELD_COUNT = 4;

    public int generalCorrelation { get; set; }
    public double specificGravity { get; set; }
    public double salinity { get; set; }
    public bool isSaturated { get; set; }
}

internal class Parameters
{
    public double temperature { get; set; }
    public double pressure { get; set; }
}

internal class Props
{
    public ReservoirProps basicReservoir { get; set; } = new ReservoirProps();
    public GasProps gas { get; set; } = new GasProps();
    public OilProps oil { get; set; } = new OilProps();
    public WaterProps water { get; set; } = new WaterProps();

    public Parameters parameters { get; set; } = new Parameters();
}
}
