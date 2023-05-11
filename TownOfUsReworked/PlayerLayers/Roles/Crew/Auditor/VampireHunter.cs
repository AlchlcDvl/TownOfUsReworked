using System;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class VampireHunter : CrewRole
    {
        public DateTime LastStaked;
        public static bool VampsDead => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead));
        public CustomButton StakeButton;

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
            Type = LayerEnum.VampireHunter;
            StakeButton = new(this, "Stake", AbilityTypes.Direct, "ActionSecondary", Stake);
        }

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStaked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnVigilante()
        {
            var newRole = new Vigilante(Player);
            newRole.RoleUpdate(this);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StakeButton.Update("STAKE", StakeTimer(), CustomGameOptions.StakeCooldown);

            if (VampsDead && !IsDead)
            {
                TurnVigilante();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnVigilante);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public void Stake()
        {
            if (Utils.IsTooFar(Player, StakeButton.TargetPlayer) || StakeTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());

            if (interact[3] || interact[0])
                LastStaked = DateTime.UtcNow;
            else if (interact[1])
                LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastStaked.AddSeconds(CustomGameOptions.VestKCReset);
        }
    }
}