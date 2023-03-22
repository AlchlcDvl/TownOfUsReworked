using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformClean
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Janitor))
                return true;

            var role = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);

            if (__instance == role.CleanButton)
            {
                if (role.CleanTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.JanitorClean);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(Coroutine.CleanCoroutine(role.CurrentTarget, role));
                role.LastCleaned = DateTime.UtcNow;

                if (CustomGameOptions.JaniCooldownsLinked)
                    role.LastKilled = DateTime.UtcNow;

                return false;
            }

            return true;
        }
    }
}