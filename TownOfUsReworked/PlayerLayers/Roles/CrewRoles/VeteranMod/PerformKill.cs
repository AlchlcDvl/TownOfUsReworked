using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VeteranMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Alert
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Veteran);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (!role.ButtonUsable)
                return false;

            var alertButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            if (__instance == alertButton)
            {
                if (__instance.isCoolingDown)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.AlertTimer() != 0)
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
                    SoundManager.Instance.PlaySound(TownOfUsReworked.AlertSound, false, 1f);
                } catch {}
                
                return false;
            }

            return true;
        }
    }
}