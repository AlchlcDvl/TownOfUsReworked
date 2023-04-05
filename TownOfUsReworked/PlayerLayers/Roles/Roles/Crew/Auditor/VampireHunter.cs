using System;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class VampireHunter : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastStaked;
        public static bool VampsDead => !PlayerControl.AllPlayerControls.Any(x => x?.Data.IsDead == false && !x.Data.Disconnected && x.Is(SubFaction.Undead));
        public AbilityButton StakeButton;

        public VampireHunter(PlayerControl player) : base(player)
        {
            Name = "Vampire Hunter";
            StartText = "Stake The <color=#7B8968FF>Undead</color>";
            AbilitiesText = "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <color=#7B8968FF>Undead</color> tries to interact with " +
                "you, you will kill them\n- When all <color=#7B8968FF>Undead</color> players die, you will become a <color=#FFFF00FF>Vigilante</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
            RoleType = RoleEnum.VampireHunter;
            RoleAlignment = RoleAlignment.CrewAudit;
            AlignmentName = CA;
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStaked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnVigilante()
        {
            StakeButton.gameObject.SetActive(false);
            var role = new Vigilante(Player);
            role.RoleUpdate(this);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}