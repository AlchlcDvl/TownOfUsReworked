using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using System;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jackal : NeutralRole
    {
        public PlayerControl EvilRecruit;
        public PlayerControl GoodRecruit;
        public PlayerControl BackupRecruit;
        public PlayerControl ClosestPlayer;
        public AbilityButton RecruitButton;
        public bool HasRecruited;
        public bool RecruitsDead => (EvilRecruit == null || GoodRecruit == null || ((EvilRecruit?.Data.IsDead == true || EvilRecruit.Data.Disconnected) &&
            (GoodRecruit?.Data.Disconnected == true || GoodRecruit.Data.IsDead))) && BackupRecruit == null;
        public DateTime LastRecruited;
        public List<byte> Recruited = new();

        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            RoleType = RoleEnum.Jackal;
            StartText = "Gain A Majority";
            AbilitiesText = "- You can recruit one player into joining your organisation.\n- You start off with 2 recruits. 1 of them is always <color=#8BFDFDFF>Crew</color>" +
                "\nand the other is either a <color=#008000FF>Syndicate</color>, <color=#FF0000FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
            SubFaction = SubFaction.Cabal;
            SubFactionColor = Colors.Cabal;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;

            Recruited = new List<byte>
            {
                Player.PlayerId
            };
        }

        public float RecruitTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastRecruited;
            var num = CustomGameOptions.RecruitCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}