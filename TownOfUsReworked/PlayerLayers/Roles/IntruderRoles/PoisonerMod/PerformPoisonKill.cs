using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.PoisonerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformPoisonKill
    {
        public static Sprite PoisonSprite => TownOfUsReworked.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfUsReworked.PoisonedSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;

            if (target == null)
                return false;

            if (!__instance.isActiveAndEnabled)
                return false;

            if (role.PoisonTimer() > 0)
                return false;

            if (role.Enabled == true)
                return false;

            if (role.Player.inVent)
            {
                role.PoisonButton.SetCoolDown(0.01f, 1f);
                return false;
            }

            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastPoisoned = DateTime.UtcNow;

                    role.PoisonButton.SetCoolDown(0.01f, 1f);

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }

                if (role.Player.IsProtected())
                {
                    role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
                    role.PoisonButton.SetCoolDown(0.01f, 1f);
                    return false;
                }

                Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }

            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks) role.LastPoisoned = DateTime.UtcNow;
                    role.PoisonButton.SetCoolDown(0.01f, 1f);

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (!PlayerControl.LocalPlayer.IsProtected())
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastPoisoned = DateTime.UtcNow;

                    role.PoisonButton.SetCoolDown(0.01f, 1f);

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else
                {
                    if (!PlayerControl.LocalPlayer.IsProtected())
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                    else
                    {
                        role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset + 0.01f);
                        role.PoisonButton.SetCoolDown(0.01f, 1f);
                    }
                }

                return false;
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer.Write(medic);
                writer.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastPoisoned = DateTime.UtcNow;

                role.PoisonButton.SetCoolDown(0.01f, 1f);

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }
            else if (role.ClosestPlayer.IsVesting())
            {
                role.LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset + 0.01f);
                role.PoisonButton.SetCoolDown(0.01f, 1f);
                return false;
            }
            else if (role.ClosestPlayer.IsProtected())
            {
                role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset + 0.01f);
                role.PoisonButton.SetCoolDown(0.01f, 1f);
                return false;
            }

            role.PoisonedPlayer = target;
            role.PoisonButton.SetTarget(null);
            DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            role.TimeRemaining = CustomGameOptions.PoisonDuration;
            role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDuration);

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer2.Write((byte)ActionsRPC.Poison);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(role.PoisonedPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            
            try
            {
                //SoundManager.Instance.PlaySound(TownOfUsReworked.PoisonSound, false, 1f);
            } catch {}
            
            return false;
        }
    }
}