using System.Reflection;

namespace PomoTimer
{
    public class Settings
    {
        public string BgColor { get; set; } = App.ASPHALT;
        public int LocationLeft { get; set; } = 82;
        public int LocationTop { get; set; } = 32;
        public float OpacityValue { get; set; } = 0.5f;
        public string Version { get; set; } = "Update - V1.1";
        public int PomoTimeMinutes { get; set; } = 25;
        public int RelaxTimeMinutes { get; set; } = 5;
    }
}
