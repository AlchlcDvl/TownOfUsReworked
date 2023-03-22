using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class VampireHunter : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastStaked;
        public bool VampsDead => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead)) == 0;
        public AbilityButton StakeButton;

        public VampireHunter(PlayerControl player) : base(player)
        {
            Name = "Vampire Hunter";
            StartText = "Stake The <color=#7B8968FF>Undead</color>";
            AbilitiesText = "- You can stake players to see if they have been turned.\n- When you stake a turned person, or an <color=#7B8968FF>Undead</color>" +
                " tries to interact with you, you will kill them.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
            RoleType = RoleEnum.VampireHunter;
            RoleAlignment = RoleAlignment.CrewAudit;
            AlignmentName = CA;
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStaked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnVigilante()
        {
            var vh = Role.GetRole<VampireHunter>(Player);
            var role = new Vigilante(Player);
            role.RoleUpdate(vh);
            Player.RegenTask();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}