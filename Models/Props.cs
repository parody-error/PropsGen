namespace PropsGen.Models
{
    internal class GasProps
    {
        public double EUR { get; set; }
        public double S_G { get; set; }
        public double H_2_S { get; set; }
        public double C_O_2 { get; set; }
    }

    internal class Props
    {
        public GasProps GasProps { get; set; } = new GasProps();
    }
}
