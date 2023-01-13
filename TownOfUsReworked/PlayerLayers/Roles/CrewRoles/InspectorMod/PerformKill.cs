using Hazel;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                return true;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null)
                return false;

            var flag2 = role.ExamineTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() || role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer2.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastExamined = DateTime.UtcNow;

                    StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    return false;
                }
                else if (!role.Player.IsProtected())
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    return false;
                }

                role.LastExamined = DateTime.UtcNow;
                return false;
            }

            role.Examined.Add(role.ClosestPlayer);
            role.LastExamined = DateTime.UtcNow;
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
            } catch {}
            
            return false;
        }
    }
}
