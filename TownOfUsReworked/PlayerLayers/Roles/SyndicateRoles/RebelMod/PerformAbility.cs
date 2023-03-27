using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.CustomOptions;
using System;
using Reactor.Utilities;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return true;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (__instance == role.DeclareButton && !role.HasDeclared)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Sidekick);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Sidekick(role, role.ClosestPlayer);
                }
                else if (interact[0])
                    role.LastDeclared = DateTime.UtcNow;
                else if (interact[1])
                    role.LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            if (!role.WasSidekick || role.FormerRole == null)
                return false;

            var formerRole = role.FormerRole.RoleType;

            if (__instance == role.ConcealButton && formerRole == RoleEnum.Concealer)
            {
                if (role.ConcealTimer() != 0f)
                    return false;

                if (role.Concealed)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Conceal);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                role.Conceal();
                Utils.Conceal();
                return false;
            }
            else if (__instance == role.FrameButton && formerRole == RoleEnum.Framer)
            {
                if (role.FrameTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (!Role.SyndicateHasChaosDrive)
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestFrame))
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestFrame);

                    if (interact[3])
                        role.Frame(role.ClosestFrame);

                    if (interact[0])
                        role.LastFramed = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else
                {
                    foreach (var player in Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius))
                        role.Frame(player);

                    role.LastFramed = DateTime.UtcNow;
                }

                return false;
            }
            else if (__instance == role.PoisonButton && formerRole == RoleEnum.Poisoner)
            {
                if (role.PoisonTimer() != 0f)
                    return false;

                if (role.Poisoned)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3])
                {
                    role.PoisonedPlayer = role.ClosestPlayer;
                    role.PoisonTimeRemaining = CustomGameOptions.PoisonDuration;
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer2.Write((byte)ActionsRPC.RebelAction);
                    writer2.Write((byte)RebelActionsRPC.Poison);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(role.PoisonedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.Poison();
                }

                if (interact[0] && Role.SyndicateHasChaosDrive)
                    role.LastPoisoned = DateTime.UtcNow;
                else if (interact[1])
                    role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }
            else if (__instance == role.ShapeshiftButton && formerRole == RoleEnum.Shapeshifter)
            {
                if (role.ShapeshiftTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
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
                if (role.WarpTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Warp);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Rebel.Warp();
                role.LastWarped = DateTime.UtcNow;
                return false;
            }
            else if (__instance == role.ConfuseButton && formerRole == RoleEnum.Drunkard)
            {
                if (role.DrunkTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Confuse);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.ConfuseTimeRemaining = CustomGameOptions.ConfuseDuration;
                role.Confuse();
                return false;
            }

            return true;
        }

        public static void Sidekick(Rebel reb, PlayerControl target)
        {
            reb.HasDeclared = true;
            var formerRole = Role.GetRole(target);

            var sidekick = new Sidekick(target)
            {
                FormerRole = formerRole,
                Rebel = reb
            };

            sidekick.RoleUpdate(formerRole);

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Rebel, "You have been promoted to <color=#979C9FFF>Sidekick</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}