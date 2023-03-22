using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Blackmailer : IntruderRole
    {
        public AbilityButton BlackmailButton;
        public PlayerControl BlackmailedPlayer;
        public PlayerControl ClosestBlackmail;
        public DateTime LastBlackmailed;
        public bool Blackmailed => BlackmailedPlayer != null;

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            StartText = "You Know Their Secrets";
            AbilitiesText = "- You can blackmail players to ensure they cannot speak in the next meeting.\n- You can blackmail fellow <color=#FF0000FF>Intruders</color>.\n- " +
                "Everyone will be alerted at the start of the meeting that someone has been blackmailed." + (CustomGameOptions.WhispersNotPrivate ? "\n- You can read whispers during " +
                "meetings." : "");
            Color = CustomGameOptions.CustomIntColors ? Colors.Blackmailer : Colors.Intruder;
            RoleType = RoleEnum.Blackmailer;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            InspectorResults = InspectorResults.HasInformation;
            BlackmailedPlayer = null;
        }
        
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BlackmailCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}