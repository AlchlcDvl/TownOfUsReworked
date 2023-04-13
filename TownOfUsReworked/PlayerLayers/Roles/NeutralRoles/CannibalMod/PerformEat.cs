using System;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformEat
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cannibal))
                return true;

            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);

            if (__instance == role.EatButton)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                if (role.EatTimer() != 0f)
                    return false;

                var player = Utils.PlayerById(role.CurrentTarget.ParentId);
                Utils.Spread(role.Player, player);
                Role.Cleaned.Add(player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.FadeBody);
                writer.Write(role.CurrentTarget.ParentId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.LastEaten = DateTime.UtcNow;
                role.EatNeed--;
                Coroutines.Start(Utils.FadeBody(role.CurrentTarget));
                return false;
            }

            return true;
        }
    }
}