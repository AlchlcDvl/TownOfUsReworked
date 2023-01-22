using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Shapeshifter))
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (__instance == role.ShapeshiftButton)
            {
                if (__instance.isCoolingDown)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.ShapeshiftTimer() != 0)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Shapeshift);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.ConcealDuration;
                Utils.Shapeshift();
                return false;
            }

            return true;
        }
    }
}