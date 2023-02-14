using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System;
using TownOfUsReworked.Lobby.CustomOption;
using System.Linq;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod;
using TownOfUsReworked.Lobby.Extras.RainbowMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using Random = UnityEngine.Random;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbilities
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return false;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole == null)
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;
            
            var revivedRole = role.RevivedRole?.RoleType;

            if (__instance == role.ReviveButton && revivedRole == RoleEnum.Altruist)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.AltruistRevive);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);        
                Coroutines.Start(Coroutine.RetributionistRevive(role.CurrentTarget, role));
                return false;
            }
            else if (__instance == role.ShieldButton && revivedRole == RoleEnum.Medic)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.RetributionistAction);
                    writer.Write((byte)RetributionistActionsRPC.Protect);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ShieldedPlayer = role.ClosestPlayer;
                    role.UsedAbility = true;
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.ProtectSound, false, 1f);
                    } catch {}
                }

                return false;
            }
            else if (__instance == role.InspectButton && revivedRole == RoleEnum.Inspector)
            {
                if (role.InspectTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.InspectedPlayers.Contains(role.ClosestPlayer.PlayerId))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    role.InspectedPlayers.Add(role.ClosestPlayer.PlayerId);
                    var results = new List<Role>();
                    var targetRole = Role.GetRole(role.ClosestPlayer);
                    results.Add(targetRole);

                    var i = 0;

                    while (i < 4)
                    {
                        var random = Random.RandomRangeInt(0, Role.AllRoles.Count());
                        var role2 = Role.AllRoles.ToList()[random];

                        if (role2 != targetRole)
                        {
                            results.Add(role2);
                            i++;
                        }

                        results.Shuffle();
                    }

                    role.InspectResults.Add(role.ClosestPlayer, results);
            
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                    } catch {}
                }
                
                if (interact[0] == true)
                    role.LastInspected = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.FixButton && revivedRole == RoleEnum.Engineer)
            {
                if (!role.FixButtonUsable)
                    return false;

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);

                if (!sabActive || dummyActive)
                    return false;

                role.FixUsesLeft--;

                var camouflager = Role.GetRoleValue(RoleEnum.Camouflager);
                var camo = (Camouflager)camouflager;
                var concealer = Role.GetRoleValue(RoleEnum.Concealer);
                var conc = (Concealer)concealer;
                var shapeshifter = Role.GetRoleValue(RoleEnum.Shapeshifter);
                var ss = (Shapeshifter)shapeshifter;

                switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                {
                    case 0:
                    case 1:
                        var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                        if (comms2.IsActive)
                            return FixFunctions.FixMiraComms();

                        var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor2.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen2.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights2.IsActive)
                            return FixFunctions.FixLights(lights2);

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

                        break;

                    case 2:
                        var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms3.IsActive)
                            return FixFunctions.FixComms();

                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                        if (seismic.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Laboratory);

                        var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights3.IsActive)
                            return FixFunctions.FixLights(lights3);

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

                        break;

                    case 3:
                        var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms1.IsActive)
                            return FixFunctions.FixComms();

                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor1.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen1.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights1.IsActive)
                            return FixFunctions.FixLights(lights1);

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

                        break;

                    case 4:
                        var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms4.IsActive)
                            return FixFunctions.FixComms();

                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                        if (reactor.IsActive)
                            return FixFunctions.FixAirshipReactor();

                        var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights4.IsActive)
                            return FixFunctions.FixLights(lights4);

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

                        break;

                    case 5:
                        var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor5.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights5.IsActive)
                            return FixFunctions.FixLights(lights5);

                        var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms5.IsActive)
                            return FixFunctions.FixComms();

                        foreach (PlayerTask i in PlayerControl.LocalPlayer.myTasks)
                        {
                            if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                                return FixFunctions.FixSubOxygen();
                        }

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

                        break;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.EngineerFix);
                writer.Write(PlayerControl.LocalPlayer.NetId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.FixSound, false, 1f);
                } catch {}

                return false;
            }
            else if (__instance == role.ExamineButton && revivedRole == RoleEnum.Detective)
            {
                if (role.ExamineTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var hasKilled = false;

                    foreach (var player in Murder.KilledPlayers)
                    {
                        if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                            hasKilled = true;
                    }

                    if (hasKilled || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                
                if (interact[0] == true)
                    role.LastExamined = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.SwoopButton && revivedRole == RoleEnum.Chameleon)
            {
                if (role.SwoopTimer() != 0f)
                    return false;

                role.SwoopTimeRemaining = CustomGameOptions.SwoopDuration;
                role.RegenTask();
                role.Invis();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.Swoop);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.MediateButton && revivedRole == RoleEnum.Medium)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.MediateTimer() != 0f)
                    return false;

                role.LastMediated = DateTime.UtcNow;
                List<DeadPlayer> PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
                {
                    foreach (var dead in PlayersDead)
                    {
                        if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.Keys.Contains(x.ParentId)))
                        {
                            role.AddMediatePlayer(dead.PlayerId);
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                            writer.Write((byte)ActionsRPC.RetributionistAction);
                            writer.Write((byte)RetributionistActionsRPC.Mediate);
                            writer.Write(dead.PlayerId);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                                return false;
                        }
                    }
                }
                else
                {
                    PlayersDead.Shuffle();
                    var dead = PlayersDead[Random.RandomRangeInt(0, PlayersDead.Count)];

                    if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.Keys.Contains(x.ParentId)))
                    {
                        role.AddMediatePlayer(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer.Write((byte)ActionsRPC.RetributionistAction);
                        writer.Write((byte)RetributionistActionsRPC.Mediate);
                        writer.Write(dead.PlayerId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                return false;
            }
            else if (__instance == role.InterrogateButton && revivedRole == RoleEnum.Sheriff)
            {
                if (role.InterrogateTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
            
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                    role.Interrogated.Add(role.ClosestPlayer.PlayerId);
                
                if (interact[0] == true)
                    role.LastInterrogated = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.BugButton && revivedRole == RoleEnum.Operative)
            {
                if (role.BugTimer() != 0f)
                    return false;

                role.BugUsesLeft--;
                role.LastBugged = System.DateTime.UtcNow;
                role.Bugs.Add(BugExtentions2.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));
                return false;
            }
            else if (__instance == role.RewindButton && revivedRole == RoleEnum.TimeLord)
            {
                if (!role.RewindButtonUsable)
                    return false;

                if (role.TimeLordRewindTimer() != 0f && !RecordRewind.rewinding)
                    return false;

                role.RewindUsesLeft--;
                StartStop.StartRewind(role);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.Rewind);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
        
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.RewindSound, false, 1f);
                } catch {}

                return false;
            }
            else if (__instance == role.TrackButton && revivedRole == RoleEnum.Tracker)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.TrackerTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var target = role.ClosestPlayer;
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;

                    if (!CamouflageUnCamouflage.IsCamoed)
                    {
                        if (RainbowUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                            renderer.color = RainbowUtils.Rainbow;
                        else
                            renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];
                    }
                    else
                        renderer.color = Color.gray;

                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrow.target = target.transform.position;
                    role.TrackerArrows.Add(target.PlayerId, arrow);
                    role.TrackUsesLeft--;
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.TrackSound, false, 1f);
                    } catch {}
                }
                
                if (interact[0] == true)
                    role.LastTracked = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.StakeButton && revivedRole == RoleEnum.VampireHunter)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.StakeTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), role.ClosestPlayer.Is(SubFaction.Undead));

                if (interact[3] == true && interact[0] == true)
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.AlertButton && revivedRole == RoleEnum.Veteran)
            {
                if (!role.AlertButtonUsable)
                    return false;

                if (role.AlertTimer() != 0f)
                    return false;

                role.AlertTimeRemaining = CustomGameOptions.AlertDuration;
                role.AlertUsesLeft--;
                role.Alert();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.Alert);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.AlertSound, false, 1f);
                } catch {}
                
                return false;
            }

            return false;
        }
    }
}