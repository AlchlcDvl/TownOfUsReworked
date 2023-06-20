namespace TownOfUsReworked.Modules
{
    public class HatExtension
    {
        public string Artist { get; set; }
        public string Condition { get; set; }
        public Sprite FlipImage { get; set; }
        public Sprite BackFlipImage { get; set; }
    }

    public class CustomHat
    {
        public string Artist { get; set; }
        public string Condition { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public string FlipID { get; set; }
        public string BackflipID { get; set; }
        public string BackID { get; set; }
        public string ClimbID { get; set; }
        public string FloorID { get; set; }
        public bool NoBouce { get; set; }
        public bool Adaptive { get; set; }
        public bool Behind { get; set; }
    }
}