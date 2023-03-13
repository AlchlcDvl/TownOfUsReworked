using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jackal : NeutralRole
    {
        public PlayerControl EvilRecruit = null;
        public PlayerControl GoodRecruit = null;
        public PlayerControl BackupRecruit = null;
        public PlayerControl ClosestPlayer;
        public AbilityButton RecruitButton;
        public bool HasRecruited = false;
        public bool RecruitsDead => (EvilRecruit == null || GoodRecruit == null || ((EvilRecruit != null && EvilRecruit.Data.IsDead || EvilRecruit.Data.Disconnected) &&
            (GoodRecruit != null && GoodRecruit.Data.Disconnected || GoodRecruit.Data.IsDead))) && BackupRecruit == null;
        public DateTime LastRecruited;
        public List<byte> Recruited;

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
            RoleDescription = "You are a Jackal! You are a greedy double agent sent from a rival company! Use your recruits to your advantage and take over the mission!";
            Recruited = new List<byte>();
            Recruited.Add(Player.PlayerId);
        }
        
        public float RecruitTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRecruited;
            var num = CustomGameOptions.RecruitCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}