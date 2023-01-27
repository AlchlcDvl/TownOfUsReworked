using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInterrogate
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff, true))
                return false;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.InterrogateTimer() != 0f && __instance == role.InterrogateButton)
                return false;
            
            if (__instance == role.InterrogateButton)
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);

                if (interact[3] == true && interact[0] == true)
                {
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
                else if (interact[1] == true)
                    role.LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}
