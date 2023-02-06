using System;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformEat
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cannibal))
                return false;

            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);

            if (__instance == role.EatButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;
                
                if (role.EatTimer() > 0f)
                    return false;

                Utils.Spread(role.Player, Utils.PlayerById(role.CurrentTarget.ParentId));
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.CannibalEat);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.CurrentTarget.ParentId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                //SoundManager.Instance.PlaySound(TownOfUsReworked.EatSound, false, 0.4f);
                role.LastEaten = DateTime.UtcNow;
                Coroutines.Start(Coroutine.EatCoroutine(role.CurrentTarget, role));
                return false;
            }

            return false;
        }
    }
}