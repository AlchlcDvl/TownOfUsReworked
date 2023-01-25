using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff))
                return false;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff, role.ClosestPlayer, __instance) || __instance != role.InterrogateButton)
                return false;

            if (role.InterrogateTimer() != 0f && __instance == role.InterrogateButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer);
                return false;
            }
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.Interrogate);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(role.ClosestPlayer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            role.Interrogated.Add(role.ClosestPlayer.PlayerId);
            role.LastInterrogated = DateTime.UtcNow;
            //SoundManager.Instance.PlaySound(TownOfUsReworked.InterrogateSound, false, 0.4f);
            return false;
        }
    }
}
