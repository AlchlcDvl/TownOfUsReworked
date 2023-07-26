namespace TownOfUsReworked.Modules
{
    public class PlayerVersion
    {
        public readonly Version Version;
        public readonly Guid Guid;
        public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);

        public PlayerVersion(Version version, Guid guid)
        {
            Version = version;
            Guid = guid;
        }
    }
}