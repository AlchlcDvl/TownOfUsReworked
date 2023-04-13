using HarmonyLib;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;
using System.Linq;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PromotedRebelMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.PromotedRebel))
                return true;

            var role = Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer);

            if (role.FormerRole == null || role.FormerRole?.RoleType == RoleEnum.Anarchist)
                return false;

            if (__instance == role.ConcealButton)
            {
                if (role.ConcealTimer() != 0f)
                    return false;

                if (role.HoldsDrive)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Conceal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                    role.Conceal();
                    Utils.Conceal();
                }
                else if (role.ConcealedPlayer == null)
                    role.ConcealMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.Player && x != role.ConcealedPlayer).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Conceal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                    role.Conceal();
                    Utils.Invis(role.ConcealedPlayer, PlayerControl.LocalPlayer.Is(Faction.Syndicate));
                }

                return false;
            }
            else if (__instance == role.FrameButton)
            {
                if (role.FrameTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.HoldsDrive)
                {
                    foreach (var player in Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius))
                        role.Frame(player);

                    role.LastFramed = DateTime.UtcNow;
                }
                else
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestTarget);

                    if (interact[3])
                        role.Frame(role.ClosestTarget);

                    if (interact[0])
                        role.LastFramed = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
                }

                return false;
            }
            else if (__instance == role.PoisonButton)
            {
                if (role.PoisonTimer() != 0f)
                    return false;

                if (!role.HoldsDrive)
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                    if (interact[3])
                    {
                        role.PoisonedPlayer = role.ClosestPlayer;
                        role.PoisonTimeRemaining = CustomGameOptions.PoisonDuration;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.RebelAction);
                        writer.Write((byte)RebelActionsRPC.Poison);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.PoisonedPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.Poison();
                    }
                    else if (interact[0])
                        role.LastPoisoned = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
                    else if (interact[2])
                        role.LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
                }
                else if (role.PoisonedPlayer == null)
                    role.PoisonMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Poison);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.PoisonedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.Poison();
                }

                return false;
            }
            else if (__instance == role.ShapeshiftButton)
            {
                if (role.ShapeshiftTimer() != 0f)
                    return false;

                if (!role.HoldsDrive)
                {
                    var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == role.ShapeshiftPlayer1 || x == role.ShapeshiftPlayer2 || (x == role.Player &&
                        !CustomGameOptions.WarpSelf) || (x.Data.IsDead && Utils.BodyById(x.PlayerId) == null))).ToList();

                    if (role.ShapeshiftPlayer1 == null)
                        role.ShapeshiftMenu1.Open(targets);
                    else if (role.ShapeshiftPlayer2 == null)
                        role.ShapeshiftMenu2.Open(targets);
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.RebelAction);
                        writer.Write((byte)RebelActionsRPC.Shapeshift);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.ShapeshiftTimeRemaining = CustomGameOptions.ShapeshiftDuration;
                        role.Shapeshift();
                    }
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Shapeshift);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ShapeshiftTimeRemaining = CustomGameOptions.ShapeshiftDuration;
                    role.Shapeshift();
                    Utils.Shapeshift();
                }

                return false;
            }
            else if (__instance == role.WarpButton)
            {
                if (role.WarpTimer() != 0f)
                    return false;

                if (role.HoldsDrive)
                {
                    Utils.Warp();
                    role.LastWarped = DateTime.UtcNow;
                }
                else
                {
                    var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == role.WarpPlayer1 || x == role.WarpPlayer2 || (x == role.Player && !CustomGameOptions.WarpSelf)
                        || (x.Data.IsDead && Utils.BodyById(x.PlayerId) == null) || role.UnwarpablePlayers.ContainsKey(x.PlayerId))).ToList();

                    if (role.WarpPlayer1 == null)
                        role.WarpMenu1.Open(targets);
                    else if (role.WarpPlayer2 == null)
                        role.WarpMenu2.Open(targets);
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Warp);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        Coroutines.Start(role.WarpPlayers());
                        role.LastWarped = DateTime.UtcNow;
                    }
                }

                return false;
            }
            else if (__instance == role.ConfuseButton)
            {
                if (role.DrunkTimer() != 0f)
                    return false;

                if (role.HoldsDrive)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Confuse);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ConfuseTimeRemaining = CustomGameOptions.ConfuseDuration;
                    role.Confuse();
                    Reverse.ConfuseAll();
                }
                else if (role.ConfusedPlayer == null)
                    role.ConfuseMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Confuse);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ConfuseTimeRemaining = CustomGameOptions.ConfuseDuration;
                    role.Confuse();
                    Reverse.ConfuseSingle(role.ConfusedPlayer);
                }

                return false;
            }
            else if (__instance == role.CrusadeButton)
            {
                if (role.CrusadeTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestTarget);

                if (interact[3])
                {
                    role.CrusadeTimeRemaining = CustomGameOptions.CrusadeDuration;
                    role.CrusadedPlayer = role.ClosestTarget;
                    role.Crusade();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RebelAction);
                    writer.Write((byte)RebelActionsRPC.Crusade);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.CrusadedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (interact[0])
                    role.LastCrusaded = DateTime.UtcNow;
                else if (interact[1])
                    role.LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.BombButton)
            {
                if (role.BombTimer() != 0f)
                    return false;

                role.Bombs.Add(BombExtensions.CreateBomb(PlayerControl.LocalPlayer.GetTruePosition(), role.HoldsDrive));
                role.LastPlaced = DateTime.UtcNow;

                if (CustomGameOptions.BombCooldownsLinked)
                    role.LastDetonated = DateTime.UtcNow;

                return false;
            }
            else if (__instance == role.DetonateButton)
            {
                if (role.DetonateTimer() != 0f)
                    return false;

                if (role.Bombs.Count == 0)
                    return false;

                role.Bombs.DetonateBombs();
                role.LastDetonated = DateTime.UtcNow;

                if (CustomGameOptions.BombCooldownsLinked)
                    role.LastPlaced = DateTime.UtcNow;

                return false;
            }

            return true;
        }
    }
}