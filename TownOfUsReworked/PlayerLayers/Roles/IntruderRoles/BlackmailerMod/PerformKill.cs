using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Blackmailer))
                return false;

            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            if (__instance == role.BlackmailButton)
            {
                if (role.BlackmailTimer() != 0f)
                    return false;

                role.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);

                if (role.Blackmailed != null && role.Blackmailed.Data.IsImpostor())
                {
                    if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
                        role.Blackmailed.nameText().color = Colors.Blackmailer;
                    else
                        role.Blackmailed.nameText().color = Color.clear;
                }

                role.Blackmailed = role.ClosestPlayer;
                role.BlackmailButton.SetCoolDown(1f, 1f);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Blackmail);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer1.Write(medic);
                    writer1.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);
                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastKilled = DateTime.UtcNow;
                }
                else if (role.ClosestPlayer.IsVesting())
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
                else if (role.ClosestPlayer.IsProtected())
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (PlayerControl.LocalPlayer.IsOtherRival(role.ClosestPlayer))
                    role.LastKilled = DateTime.UtcNow;
                else
                {
                    role.LastKilled = DateTime.UtcNow;
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                }

                return false;
            }
            
            return false;
        }
    }
}