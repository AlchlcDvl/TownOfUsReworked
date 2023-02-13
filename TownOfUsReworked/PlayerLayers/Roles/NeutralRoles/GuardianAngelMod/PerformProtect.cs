using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformProtect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead && !CustomGameOptions.ProtectBeyondTheGrave)
                return false;

            var role = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (__instance == role.ProtectButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (!role.ButtonUsable)
                    return false;

                if (role.ProtectTimer() != 0f)
                    return false;

                role.TimeRemaining = CustomGameOptions.ProtectDuration;
                role.UsesLeft--;
                role.Protect();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.GAProtect);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}