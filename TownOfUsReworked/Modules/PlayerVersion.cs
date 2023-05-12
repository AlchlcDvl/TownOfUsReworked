namespace TownOfUsReworked.Modules
{
    public class PlayerVersion
    {
        public readonly Version Version;
        public readonly Guid Guid;

        public PlayerVersion(Version version, Guid guid)
        {
            Version = version;
            Guid = guid;
        }

        public bool GuidMatches => TownOfUsReworked.Assembly.ManifestModule.ModuleVersionId.Equals(Guid);
    }
}