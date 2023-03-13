using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VeteranMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformAlert
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Veteran))
                return true;

            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

            if (__instance == role.AlertButton)
            {
                if (!Utils.ButtonUsable(role.AlertButton))
                    return false;

                if (!role.ButtonUsable)
                    return false;

                if (role.AlertTimer() != 0f)
                    return false;

                role.TimeRemaining = CustomGameOptions.AlertDuration;
                role.UsesLeft--;
                role.Alert();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Alert);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}