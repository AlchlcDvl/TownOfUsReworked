using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformRevive
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Altruist, true))
                return false;

            var role = Role.GetRole<Altruist>(PlayerControl.LocalPlayer);

            if (__instance != role.ReviveButton)
                return false;

            if (__instance == role.ReviveButton)
            {
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.AltruistRevive);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);        
                Coroutines.Start(Coroutine.AltruistRevive(role.CurrentTarget, role));
                return false;
            }

            return false;
        }
    }
}