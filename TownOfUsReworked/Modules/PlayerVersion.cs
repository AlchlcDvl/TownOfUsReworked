using System;

namespace TownOfUsReworked.Modules
{
    public class PlayerVersion
    {
        public readonly Version Version;
        public readonly Guid Guid;

        public PlayerVersion(Version Version, Guid Guid)
        {
            this.Version = Version;
            this.Guid = Guid;
        }

        public bool GuidMatches => TownOfUsReworked.Assembly.ManifestModule.ModuleVersionId.Equals(Guid);
    }
}