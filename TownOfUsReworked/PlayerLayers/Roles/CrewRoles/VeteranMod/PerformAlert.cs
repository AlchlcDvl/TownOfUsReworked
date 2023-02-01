using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VeteranMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAlert
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Veteran))
                return false;

            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (__instance == role.AlertButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (!role.ButtonUsable)
                    return false;

                if (role.AlertTimer() != 0f)
                    return false;

                role.TimeRemaining = CustomGameOptions.AlertDuration;
                role.UsesLeft--;
                role.Alert();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Alert);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.AlertSound, false, 1f);
                } catch {}
                
                return false;
            }

            return true;
        }
    }
}