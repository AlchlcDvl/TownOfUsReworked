namespace TownOfUsReworked.Cosmetics
{
    public class HatMetadataElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public bool Adaptive { get; set; }
        public bool NoBounce { get; set; }
    }

    public class NameplateMetadataElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
    }

    public class VisorMetadataElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public bool Adaptive { get; set; }
    }

    public class HatMetadataJson
    {
        public HatMetadataElement[] Credits { get; set; }
    }

    public class NameplateMetadataJson
    {
        public NameplateMetadataElement[] Credits { get; set; }
    }

    public class VisorMetadataJson
    {
        public VisorMetadataElement[] Credits { get; set; }
    }
}