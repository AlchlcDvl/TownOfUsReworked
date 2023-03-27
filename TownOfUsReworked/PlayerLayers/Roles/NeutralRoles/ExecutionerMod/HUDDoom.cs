using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDoom
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Executioner))
                return;

            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (!role.TargetPlayer.Data.IsDead && !role.TargetPlayer.Data.Disconnected)
                return;

            if (role.Failed && !role.Player.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnJest);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TurnJest();
            }
            else if (role.TargetVotedOut)
            {
                if (role.DoomButton == null)
                    role.DoomButton = CustomButtons.InstantiateButton();

                var toBeDoomed = PlayerControl.AllPlayerControls.ToArray().Where(x => role.ToDoom.Contains(x.PlayerId)).ToList();
                role.DoomButton.UpdateButton(role, "DOOM", role.DoomTimer(), CustomGameOptions.DoomCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", toBeDoomed,
                    role.TargetVotedOut && role.CanDoom, role.CanDoom, false, 0, 1, true, role.MaxUses);
            }
        }
    }
}