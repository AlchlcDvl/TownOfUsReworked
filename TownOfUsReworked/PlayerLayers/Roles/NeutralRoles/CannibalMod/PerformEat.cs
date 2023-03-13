using System;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformEat
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cannibal))
                return true;

            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

            if (__instance == role.EatButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;
                
                if (role.EatTimer() != 0f)
                    return false;

                Utils.Spread(role.Player, Utils.PlayerById(role.CurrentTarget.ParentId));
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.CannibalEat);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.CurrentTarget.ParentId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.LastEaten = DateTime.UtcNow;
                Coroutines.Start(Coroutine.EatCoroutine(role.CurrentTarget, role));
                return false;
            }

            return true;
        }
    }
}