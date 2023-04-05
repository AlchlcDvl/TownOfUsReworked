using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using Hazel;
using System;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;
using Reactor.Networking.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return true;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (__instance == role.DeclareButton && !role.HasDeclared)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestTarget);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Declare);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Declare(role, role.ClosestTarget);
                }
                else if (interact[0])
                    role.LastDeclared = DateTime.UtcNow;
                else if (interact[1])
                    role.LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            if (!role.WasMafioso || role.FormerRole == null || role.FormerRole.RoleType == RoleEnum.Impostor)
                return false;

            var formerRole = role.FormerRole.RoleType;

            if (__instance == role.BlackmailButton && formerRole == RoleEnum.Blackmailer)
            {
                if (role.BlackmailTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestBlackmail))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestBlackmail);

                if (interact[3])
                {
                    role.BlackmailedPlayer = role.ClosestBlackmail;
                    role.BlackmailButton.SetCoolDown(1f, 1f);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Blackmail);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (interact[0])
                    role.LastBlackmailed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.CamouflageButton && formerRole == RoleEnum.Camouflager)
            {
                if (role.CamouflageTimer() != 0f)
                    return false;

                if (CamouflageUnCamouflage.IsCamoed)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.Camouflage);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.CamoTimeRemaining = CustomGameOptions.CamouflagerDuration;
                role.Camouflage();
                return false;
            }
            else if (__instance == role.InvestigateButton && formerRole == RoleEnum.Consigliere)
            {
                if (role.ConsigliereTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestTarget);

                if (interact[3])
                    role.Investigated.Add(role.ClosestTarget.PlayerId);

                if (interact[0])
                    role.LastInvestigated = DateTime.UtcNow;
                else if (interact[1])
                    role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (formerRole == RoleEnum.Disguiser)
            {
                if (__instance == role.DisguiseButton)
                {
                    if (role.DisguiseTimer() != 0f)
                        return false;

                    if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                        return false;

                    if (role.ClosestTarget == role.MeasuredPlayer)
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestTarget);

                    if (interact[3])
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.GodfatherAction);
                        writer.Write((byte)GodfatherActionsRPC.Disguise);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.MeasuredPlayer.PlayerId);
                        writer.Write(role.ClosestTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.DisguiserTimeRemaining = CustomGameOptions.DisguiseDuration;
                        role.Delay();
                    }
                    else if (interact[0])
                        role.LastDisguised = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);

                    return false;
                }
                else if (__instance == role.MeasureButton)
                {
                    if (role.MeasureTimer() != 0f)
                        return false;

                    if (Utils.IsTooFar(role.Player, role.MeasureTarget))
                        return false;

                    if (role.ClosestTarget == role.MeasuredPlayer)
                        return false;

                    var interact = Utils.Interact(role.Player, role.MeasureTarget);

                    if (interact[3])
                        role.MeasuredPlayer = role.MeasureTarget;

                    if (interact[0])
                        role.LastMeasured = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastMeasured.AddSeconds(CustomGameOptions.ProtectKCReset);

                    return false;
                }

                return false;
            }
            else if (__instance == role.FlashButton && formerRole == RoleEnum.Grenadier)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var sabActive = (bool)system?.specials.Any(s => s.IsActive);

                if (sabActive)
                    return false;

                if (role.FlashTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.FlashGrenade);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.FlashTimeRemaining = CustomGameOptions.GrenadeDuration;
                role.Flash();
                return false;
            }
            else if (formerRole == RoleEnum.Janitor)
            {
                if (__instance == role.CleanButton)
                {
                    if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                        return false;

                    var playerId = role.CurrentTarget.ParentId;
                    var player = Utils.PlayerById(playerId);
                    Utils.Spread(role.Player, player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.FadeBody);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Coroutines.Start(Utils.FadeBody(role.CurrentTarget));
                    role.LastCleaned = DateTime.UtcNow;

                    if (CustomGameOptions.JaniCooldownsLinked)
                        role.LastKilled = DateTime.UtcNow;

                    return false;
                }
                else if (__instance == role.DragButton)
                {
                    if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                        return false;

                    var playerId = role.CurrentTarget.ParentId;
                    var player = Utils.PlayerById(playerId);
                    Utils.Spread(role.Player, player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Drag);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.CurrentlyDragging = role.CurrentTarget;
                    return false;
                }
                else if (__instance == role.DropButton)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Drop);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    Vector3 position = PlayerControl.LocalPlayer.GetTruePosition();

                    if (SubmergedCompatibility.IsSubmerged())
                    {
                        if (position.y > -7f)
                            position.z = 0.0208f;
                        else
                            position.z = -0.0273f;
                    }

                    writer.Write(position);
                    writer.Write(position.z);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    var body = role.CurrentlyDragging;

                    foreach (var component in body?.bodyRenderers)
                        component.material.SetFloat("_Outline", 0f);

                    role.CurrentlyDragging = null;
                    role.LastDragged = DateTime.UtcNow;
                    body.transform.position = position;
                    return false;
                }
            }
            else if (__instance == role.MineButton && formerRole == RoleEnum.Miner)
            {
                if (!role.CanPlace)
                    return false;

                if (role.MineTimer() != 0f)
                    return false;

                if (SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Mine);
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = Utils.GetAvailableId();
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(position);
                writer.Write(position.z + 0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.SpawnVent(id, role, position, position.z + 0.01f);
                role.LastMined = DateTime.UtcNow;
                return false;
            }
            else if (__instance == role.MorphButton && formerRole == RoleEnum.Morphling)
            {
                if (__instance == role.MorphButton)
                {
                    if (role.MorphTimer() != 0f)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Morph);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.SampledPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.MorphTimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    role.Morph();
                    return false;
                }
                else if (__instance == role.SampleButton)
                {
                    if (role.SampleTimer() != 0f)
                        return false;

                    if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                        return false;

                    if (role.SampledPlayer == role.ClosestTarget)
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                    if (interact[3])
                        role.SampledPlayer = role.ClosestTarget;

                    if (interact[0])
                    {
                        role.LastSampled = DateTime.UtcNow;

                        if (CustomGameOptions.MorphCooldownsLinked)
                            role.LastMorphed = DateTime.UtcNow;
                    }
                    else if (interact[1])
                    {
                        role.LastSampled.AddSeconds(CustomGameOptions.ProtectKCReset);

                        if (CustomGameOptions.MorphCooldownsLinked)
                            role.LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
                    }
                }
            }
            else if (formerRole == RoleEnum.Teleporter)
            {
                if (__instance == role.TeleportButton)
                {
                    if (role.Player.inVent)
                        return false;

                    if (!role.CanMark)
                        return false;

                    if (role.MarkTimer() != 0f)
                        return false;

                    if (role.TeleportPoint == PlayerControl.LocalPlayer.transform.position)
                        return false;

                    role.TeleportPoint = PlayerControl.LocalPlayer.transform.position;
                    role.LastMarked = DateTime.UtcNow;

                    if (CustomGameOptions.TeleCooldownsLinked)
                        role.LastTeleport = DateTime.UtcNow;

                    return false;
                }
                else if (__instance == role.TeleportButton)
                {
                    if (role.TeleportTimer() != 0f)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Teleport);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.TeleportPoint);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.LastTeleport = DateTime.UtcNow;
                    Teleporter.Teleport(role.Player);

                    if (CustomGameOptions.TeleCooldownsLinked)
                        role.LastMarked = DateTime.UtcNow;

                    return false;
                }
            }
            else if (__instance == role.FreezeButton && formerRole == RoleEnum.TimeMaster)
            {
                if (role.FreezeTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.TimeFreeze);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.FreezeTimeRemaining = CustomGameOptions.FreezeDuration;
                role.TimeFreeze();
                return false;
            }
            else if (__instance == role.InvisButton && formerRole == RoleEnum.Wraith)
            {
                if (role.InvisTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.Invis);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.InvisTimeRemaining = CustomGameOptions.InvisDuration;
                role.Invis();
                Utils.Invis(role.Player, PlayerControl.LocalPlayer == role.Player);
                return false;
            }

            return true;
        }

        public static void Declare(Godfather gf, PlayerControl target)
        {
            gf.HasDeclared = true;
            var formerRole = Role.GetRole(target);

            var mafioso = new Mafioso(target)
            {
                FormerRole = formerRole,
                Godfather = gf
            };

            mafioso.RoleUpdate(formerRole);

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mafioso, "You have been promoted to <color=#6400FFFF>Mafioso</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone changed their identity!");
        }
    }
}