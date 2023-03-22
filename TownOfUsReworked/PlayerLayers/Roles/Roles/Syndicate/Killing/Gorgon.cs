using TownOfUsReworked.Enums;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Gorgon : SyndicateRole
    {
        public AbilityButton GazeButton;
        public List<byte> Gazed = new List<byte>();
        public DateTime LastGazed;
        public PlayerControl ClosestGaze;

        public Gorgon(PlayerControl player) : base(player)
        {
            Name = "Gorgon";
            StartText = "Turn The <color=#8BFDFDFF>Crew</color> Into Sculptures";
            AbilitiesText = "- You can stone gaze players, that slows them down and forces them to stand still till a meeting is called.\n- Stoned players cannot move and will die when " +
                "a meeting is called.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Gorgon : Colors.Syndicate;
            RoleType = RoleEnum.Gorgon;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = SyK;
        }

        public float GazeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastGazed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.GazeCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}