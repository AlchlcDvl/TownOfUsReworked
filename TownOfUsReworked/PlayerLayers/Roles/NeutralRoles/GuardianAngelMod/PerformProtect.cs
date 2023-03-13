using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformProtect
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.GuardianAngel))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead && !CustomGameOptions.ProtectBeyondTheGrave)
                return true;

            var role = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GAProtect);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}