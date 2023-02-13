using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using System;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Utilities;
using Reactor.Networking.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUsReworked.DisguiseSprite;
        public static Sprite SampleSprite => TownOfUsReworked.SampleSprite;
        public static Sprite MorphSprite => TownOfUsReworked.MorphSprite;
        public static Sprite MarkSprite => TownOfUsReworked.MarkSprite;
        public static Sprite TeleportSprite => TownOfUsReworked.TeleportSprite;
        public static Sprite Drag => TownOfUsReworked.DragSprite;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return false;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;

                    if (CustomGameOptions.JaniCooldownsLinked && role.FormerRole?.RoleType == RoleEnum.Janitor)
                        role.LastCleaned = DateTime.UtcNow;
                }
                else if (interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;

                    if (CustomGameOptions.JaniCooldownsLinked && role.FormerRole?.RoleType == RoleEnum.Janitor)
                        role.LastCleaned = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                {
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                    if (CustomGameOptions.JaniCooldownsLinked && role.FormerRole?.RoleType == RoleEnum.Janitor)
                        role.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else if (interact[2] == true)
                {
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                    if (CustomGameOptions.JaniCooldownsLinked && role.FormerRole?.RoleType == RoleEnum.Janitor)
                        role.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
                }

                return false;
            }
            else if (__instance == role.DeclareButton && !role.HasDeclared)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Declare);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Declare(role, role.ClosestPlayer);
                }
                else if (interact[1] == true)
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

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
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
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Blackmail);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (interact[1] == true)
                    role.LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.CamouflageButton && formerRole == RoleEnum.Camouflager)
            {
                if (role.CamouflageTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
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

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                    role.Investigated.Add(role.ClosestPlayer.PlayerId);
                
                if (interact[0] == true)
                    role.LastInvestigated = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.DisguiseButton && formerRole == RoleEnum.Disguiser)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;
                
                if (role.DisguiseTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));
                
                if (interact[3] == true)
                {
                    if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                    {
                        role.MeasuredPlayer = role.ClosestPlayer;
                        role.DisguiseButton.graphic.sprite = DisguiseSprite;

                        if (role.DisguiseTimer() < 5f)
                            role.LastDisguised = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.DisguiseCooldown);
                            
                        try
                        {
                            //SoundManager.Instance.PlaySound(TownOfUsReworked.SampleSound, false, 1f);
                        } catch {}

                        return false;
                    }
                    else if (role.DisguiseButton.graphic.sprite == DisguiseSprite)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer.Write((byte)ActionsRPC.GodfatherAction);
                        writer.Write((byte)GodfatherActionsRPC.Disguise);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.MeasuredPlayer.PlayerId);
                        writer.Write(role.ClosestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.DisguiseTimeRemaining = CustomGameOptions.DisguiseDuration;
                        role.Disguise();
                        
                        try
                        {
                            //SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                        } catch {}

                        return false;
                    }
                }
                else if (interact[1] == true)
                    role.LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.FlashButton && formerRole == RoleEnum.Grenadier)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);

                if (sabActive)
                    return false;

                if (role.FlashTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.FlashGrenade);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.FlashTimeRemaining = CustomGameOptions.GrenadeDuration;
                
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.FlashSound, false, 1f);
                } catch {}

                role.Flash();
                return false;
            }
            else if (__instance == role.CleanButton && formerRole == RoleEnum.Janitor)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                if (!Utils.ButtonUsable(__instance))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.JanitorClean);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(Coroutine.CleanCoroutine(role.CurrentTarget, role));
                role.LastCleaned = DateTime.UtcNow;

                if (CustomGameOptions.JaniCooldownsLinked)
                    role.LastKilled = DateTime.UtcNow;
                
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.CleanSound, false, 1f);
                } catch {}

                return false;
            }
            else if (__instance == role.MineButton && formerRole == RoleEnum.Miner)
            {
                if (!role.CanPlace)
                    return false;

                if (role.MineTimer() != 0f)
                    return false;

                if (SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1)
                    return false;
                
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.Mine);
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = GetAvailableId();
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(position);
                writer.Write(position.z + 0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                SpawnVent(id, role, position, position.z + 0.01f);

                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.MineSound, false, 1f);
                } catch {}

                return false;
            }
            else if (__instance == role.MorphButton && formerRole == RoleEnum.Morphling)
            {
                if (role.MorphButton.graphic.sprite == SampleSprite)
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                        return false;
                
                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                    if (interact[3] == true)
                    {
                        role.SampledPlayer = role.ClosestPlayer;
                        role.MorphButton.graphic.sprite = MorphSprite;
                        role.MorphButton.SetTarget(null);

                        if (role.MorphTimer() < 5f)
                            role.LastMorphed = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MorphlingCd);
                    }
                    else if (interact[1] == true)
                        role.LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
                        
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.SampleSound, false, 1f);
                    } catch {}
                }
                else
                {
                    if (role.MorphTimer() != 0f)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Morph);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.SampledPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.MorphTimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    role.Morph();
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                    } catch {}
                }

                return false;
            }
            else if (__instance == role.TeleportButton && formerRole == RoleEnum.Teleporter)
            {
                if (role.Player.inVent)
                    return false;

                if (role.TeleportButton.graphic.sprite == MarkSprite)
                {
                    role.TeleportPoint = PlayerControl.LocalPlayer.transform.position;
                    role.TeleportButton.graphic.sprite = TeleportSprite;
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

                    if (role.TeleportTimer() < 5f)
                        role.LastTeleport = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.TeleportCd);
                }
                else
                {
                    if (role.TeleportTimer() != 0f)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Teleport);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.TeleportPoint);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.LastTeleport = DateTime.UtcNow;
                    Teleporter.Teleport(role.Player);
                }

                return false;
            }
            else if (__instance == role.FreezeButton && formerRole == RoleEnum.TimeMaster)
            {
                if (role.FreezeTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.TimeFreeze);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.FreezeTimeRemaining = CustomGameOptions.FreezeDuration;
                role.TimeFreeze();
                return false;
            }
            else if (__instance == role.DragDropButton && formerRole == RoleEnum.Undertaker)
            {
                if (role.DragDropButton.graphic.sprite == Drag)
                {
                    if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                        return false;

                    var playerId = role.CurrentTarget.ParentId;
                    var player = Utils.PlayerById(playerId);
                    Utils.Spread(role.Player, player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Drag);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.CurrentlyDragging = role.CurrentTarget;
                    KillButtonTarget.SetTarget(__instance, null, role);
                    role.DragDropButton.graphic.sprite = TownOfUsReworked.DropSprite;
                    return false;
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.GodfatherAction);
                    writer.Write((byte)GodfatherActionsRPC.Drop);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    Vector3 position = PlayerControl.LocalPlayer.GetTruePosition();

                    if (SubmergedCompatibility.isSubmerged())
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
                    body.bodyRenderer.material.SetFloat("_Outline", 0f);
                    role.CurrentlyDragging = null;
                    __instance.graphic.sprite = TownOfUsReworked.DragSprite;
                    role.LastDragged = DateTime.UtcNow;
                    body.transform.position = position;
                    return false;
                }
            }
            else if (__instance == role.InvisButton && formerRole == RoleEnum.Wraith)
            {
                if (role.InvisTimer() != 0f)
                    return false;
                
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.GodfatherAction);
                writer.Write((byte)GodfatherActionsRPC.Invis);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.InvisTimeRemaining = CustomGameOptions.InvisDuration;
                role.Invis();
                //SoundManager.Instance.PlaySound(TownOfUsReworked.InvisSound, false, 0.4f);
                return false;
            }

            return false;
        }

        public static void Declare(Godfather gf, PlayerControl target)
        {
            gf.HasDeclared = true;
            var formerRole = Role.GetRole(target);
            var mafioso = new Mafioso(target);
            mafioso.FormerRole = formerRole;
            mafioso.RoleHistory.Add(formerRole);
            mafioso.RoleHistory.AddRange(formerRole.RoleHistory);
            mafioso.Godfather = gf;
        }

        public static void SpawnVent(int ventId, Godfather role, Vector2 position, float zAxis)
        {
            var ventPrefab = UnityEngine.Object.FindObjectOfType<Vent>();
            var vent = UnityEngine.Object.Instantiate(ventPrefab, ventPrefab.transform.parent);
            
            vent.Id = ventId;
            vent.transform.position = new Vector3(position.x, position.y, zAxis);

            if (role.Vents.Count > 0)
            {
                var leftVent = role.Vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else
                vent.Left = null;

            vent.Right = null;
            vent.Center = null;

            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();

            role.Vents.Add(vent);
            role.LastMined = DateTime.UtcNow;

            if (SubmergedCompatibility.isSubmerged())
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover); // just in case elevator vent is not blocked

                if (vent.gameObject.transform.position.y > -7)
                    vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.03f);
                else
                {
                    vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.0009f);
                    vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x, vent.gameObject.transform.localPosition.y, -0.003f);
                }
            }
        }

        public static int GetAvailableId()
        {
            var id = 0;

            while (true)
            {
                if (ShipStatus.Instance.AllVents.All(v => v.Id != id))
                    return id;
                
                id++;
            }
        }
    }
}
