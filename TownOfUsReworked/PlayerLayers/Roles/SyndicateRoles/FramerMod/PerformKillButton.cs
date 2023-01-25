using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Framer))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Utils.GetDistBetweenPlayers(role.Player, role.ClosestPlayer) > maxDistance)
                return false;

            if (__instance == role.FrameButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null || __instance.isCoolingDown || !__instance.isActiveAndEnabled || role.FrameTimer() != 0)
                    return false;

                if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
                }

                if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
                {
                    if (role.Player.IsShielded())
                    {
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer2.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                        writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                        role.Framed.Add(target.PlayerId);
                        role.LastFramed = DateTime.UtcNow;
                    }
                    else if (!role.Player.IsProtected())
                        Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);

                    return false;
                }
                
                role.Framed.Add(target.PlayerId);
                role.LastFramed = DateTime.UtcNow;
                return false;
            }
            
            return true;
        }
    }
}