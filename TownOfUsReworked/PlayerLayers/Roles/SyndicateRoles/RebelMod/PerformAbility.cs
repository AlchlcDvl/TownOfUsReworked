using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.Lobby.CustomOption;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static Sprite PoisonedSprite => TownOfUsReworked.PoisonedSprite;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return false;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);
            
            if (__instance == role.DeclareButton && !role.HasDeclared)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Sidekick);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Sidekick(role, role.ClosestPlayer);
                }
                else if (interact[1] == true)
                    role.LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
                
                return false;
            }

            if (!role.WasSidekick || role.FormerRole == null)
                return false;
            
            var formerRole = role.FormerRole.RoleType;
            
            if (__instance == role.ConcealButton && formerRole == RoleEnum.Concealer)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.ConcealTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Conceal);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                role.Conceal();
                return false;
            }
            else if (__instance == role.FrameButton && formerRole == RoleEnum.Framer)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.FrameTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    role.Framed.Add(role.ClosestPlayer.PlayerId);
                    role.LastFramed = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                    role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.GazeButton && formerRole == RoleEnum.Gorgon)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.GazeTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Gaze);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.Gazed.Add((role.ClosestPlayer, 0, false));
                    role.LastGazed = DateTime.UtcNow;

                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                    } catch {}
                }
                else if (interact[1] == true)
                    role.LastGazed.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastGazed.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }
            else if (__instance == role.PoisonButton && formerRole == RoleEnum.Poisoner)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.PoisonTimer() != 0f)
                    return false;
                
                if (role.PoisonButton.graphic.sprite == PoisonedSprite)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                {
                    role.PoisonedPlayer = role.ClosestPlayer;
                    role.PoisonTimeRemaining = CustomGameOptions.PoisonDuration;

                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer2.Write((byte)ActionsRPC.RebelAction);
                    writer2.Write((byte)RebelActionsRPC.Poison);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(role.PoisonedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.Poison();
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PoisonSound, false, 1f);
                    } catch {}
                }
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }
            else if (__instance == role.ShapeshiftButton && formerRole == RoleEnum.Shapeshifter)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.ShapeshiftTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Shapeshift);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.ShapeshiftTimeRemaining = CustomGameOptions.ShapeshiftDuration;
                role.Shapeshift();
                return false;
            }
            else if (__instance == role.WarpButton && formerRole == RoleEnum.Warper)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.WarpTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Warp);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.Warp();
                role.LastWarped = DateTime.UtcNow;
                return false;
            }
            else if (__instance == role.ConfuseButton && formerRole == RoleEnum.Drunkard)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.DrunkTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Confuse);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.ConfuseTimeRemaining = CustomGameOptions.ConfuseDuration;
                role.Confuse();

                return false;
            }

            return false;
        }

        public static void Sidekick(Rebel reb, PlayerControl target)
        {
            reb.HasDeclared = true;
            var formerRole = Role.GetRole(target);
            var sidekick = new Sidekick(target);
            sidekick.FormerRole = formerRole;
            sidekick.RoleHistory.Add(formerRole);
            sidekick.RoleHistory.AddRange(formerRole.RoleHistory);
            sidekick.Rebel = reb;
        }
    }
}
