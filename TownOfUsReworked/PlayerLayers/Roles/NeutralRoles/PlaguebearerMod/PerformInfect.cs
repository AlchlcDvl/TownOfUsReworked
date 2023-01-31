using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInfect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Plaguebearer, true))
                return false;

            var role = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.InfectTimer() != 0f && __instance == role.InfectButton)
                return false;

            if (__instance == role.InfectButton)
            {
                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[0] == true)
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer2.Write((byte)ActionsRPC.Infect);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.InfectedPlayers.Add(role.ClosestPlayer.PlayerId);
                    role.LastInfected = DateTime.UtcNow;
            
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                    } catch {}
                }
                
                if (interact[3] == true)
                    role.LastInfected = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}