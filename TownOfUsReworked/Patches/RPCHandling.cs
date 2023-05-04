using Hazel;
using System;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using Reactor.Utilities;
using AmongUs.GameOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Classes;
using System.Collections.Generic;
using TownOfUsReworked.Extensions;
using Object = UnityEngine.Object;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.PlayerLayers;
using Reactor.Networking.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class RPCHandling
    {
        public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch ((CustomRPC)callId)
            {
                case CustomRPC.SetLayer:
                    var player = Utils.PlayerById(reader.ReadByte());
                    var id = reader.ReadInt32();
                    var rpc = (PlayerLayerEnum)reader.ReadByte();
                    RoleGen.SetLayer(id, player, rpc);
                    break;

                case CustomRPC.NullAbility:
                    _ = new Abilityless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.NullModifier:
                    _ = new Modifierless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.NullObjectifier:
                    _ = new Objectifierless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.NullRole:
                    _ = new Roleless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.SetRevealer:
                    var will4 = reader.ReadByte();
                    SetPostmortals.WillBeRevealer = will4 == 255 ? null : Utils.PlayerById(will4);
                    break;

                case CustomRPC.SetPhantom:
                    var will = reader.ReadByte();
                    SetPostmortals.WillBePhantom = will == 255 ? null : Utils.PlayerById(will);
                    break;

                case CustomRPC.SetBanshee:
                    var will2 = reader.ReadByte();
                    SetPostmortals.WillBeBanshee = will2 == 255 ? null : Utils.PlayerById(will2);
                    break;

                case CustomRPC.SetGhoul:
                    var will3 = reader.ReadByte();
                    SetPostmortals.WillBeGhoul = will3 == 255 ? null : Utils.PlayerById(will3);
                    break;

                case CustomRPC.Whisper:
                    var whisperer = Utils.PlayerById(reader.ReadByte());
                    var whispered = Utils.PlayerById(reader.ReadByte());
                    var message = reader.ReadString();

                    if (whispered == PlayerControl.LocalPlayer)
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} whispers to you:{message}");
                    else if ((PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer) && CustomGameOptions.WhispersNotPrivate) || (PlayerControl.LocalPlayer.Data.IsDead &&
                        CustomGameOptions.DeadSeeEverything))
                    {
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} is whispering to {whispered.name}:{message}");
                    }
                    else if (CustomGameOptions.WhispersAnnouncement)
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} is whispering to {whispered.name}.");

                    break;

                case CustomRPC.Guess:
                    break;

                case CustomRPC.CatchPhantom:
                    Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                    break;

                case CustomRPC.CatchBanshee:
                    Role.GetRole<Banshee>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                    break;

                case CustomRPC.CatchGhoul:
                    Role.GetRole<Ghoul>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                    break;

                case CustomRPC.Start:
                    RoleGen.ResetEverything();
                    break;

                case CustomRPC.AttemptSound:
                    var medicId = reader.ReadByte();
                    var shielded = reader.ReadByte();

                    if (Utils.PlayerById(medicId).Is(RoleEnum.Retributionist))
                        Retributionist.BreakShield(medicId, shielded, CustomGameOptions.ShieldBreaks);
                    else if (Utils.PlayerById(medicId).Is(RoleEnum.Medic))
                        Medic.BreakShield(medicId, shielded, CustomGameOptions.ShieldBreaks);

                    break;

                case CustomRPC.SendChat:
                    string report = reader.ReadString();
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                    break;

                case CustomRPC.CatchRevealer:
                    Role.GetRole<Revealer>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                    break;

                case CustomRPC.AddVoteBank:
                    Ability.GetAbility<Politician>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                    break;

                case CustomRPC.MeetingStart:
                    foreach (var body in Object.FindObjectsOfType<DeadBody>())
                        body.gameObject.Destroy();

                    foreach (var player10 in PlayerControl.AllPlayerControls)
                        player10.MyPhysics.ResetAnimState();

                    break;

                case CustomRPC.DoorSyncToilet:
                    var Id = reader.ReadInt32();
                    var DoorToSync = Object.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == Id);
                    DoorToSync.SetDoorway(true);
                    break;

                case CustomRPC.SyncPlateform:
                    var isLeft = reader.ReadBoolean();
                    CallPlateform.SyncPlateform(isLeft);
                    break;

                case CustomRPC.CheckMurder:
                    var murderKiller = Utils.PlayerById(reader.ReadByte());
                    var murderTarget = Utils.PlayerById(reader.ReadByte());
                    murderKiller.CheckMurder(murderTarget);
                    break;

                case CustomRPC.FixAnimation:
                    var player5 = Utils.PlayerById(reader.ReadByte());
                    player5.MyPhysics.ResetMoveState();
                    player5.Collider.enabled = true;
                    player5.moveable = true;
                    player5.NetTransform.enabled = true;
                    break;

                case CustomRPC.SetColor:
                    Utils.PlayerById(reader.ReadByte()).SetColor(reader.ReadByte());
                    break;

                case CustomRPC.VersionHandshake:
                    var major = reader.ReadByte();
                    var minor = reader.ReadByte();
                    var patch = reader.ReadByte();
                    var timer = reader.ReadSingle();

                    if (!AmongUsClient.Instance.AmHost && timer >= 0f)
                        GameStartManagerPatch.timer = timer;

                    var versionOwnerId = reader.ReadPackedInt32();
                    var revision = 255;
                    Guid guid;

                    if (reader.Length - reader.Position >= 17)
                    {
                        //Enough bytes left to read
                        revision = reader.ReadByte();
                        //GUID
                        var gbytes = reader.ReadBytes(16);
                        guid = new Guid(gbytes);
                    }
                    else
                        guid = new Guid(new byte[16]);

                    Utils.VersionHandshake(major, minor, patch, revision == 255 ? -1 : revision, guid, versionOwnerId);
                    break;

                case CustomRPC.SubmergedFixOxygen:
                    SubmergedCompatibility.RepairOxygen();
                    break;

                case CustomRPC.RemoveMeetings:
                    Utils.PlayerById(reader.ReadByte()).RemainingEmergencies = 0;
                    break;

                case CustomRPC.SetSpawnAirship:
                    while (SpawnInMinigamePatch.SpawnPoints.Count < 3)
                        SpawnInMinigamePatch.SpawnPoints.Add(reader.ReadByte());

                    break;

                case CustomRPC.ChaosDrive:
                    Role.DriveHolder = Utils.PlayerById(reader.ReadByte());
                    Role.SyndicateHasChaosDrive = true;
                    break;

                case CustomRPC.SetPos:
                    var setplayer = Utils.PlayerById(reader.ReadByte());
                    setplayer.transform.position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), setplayer.transform.position.z);
                    break;

                case CustomRPC.SyncCustomSettings:
                    RPC.ReceiveRPC(reader);
                    break;

                case CustomRPC.SetSettings:
                    var map = reader.ReadByte();
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId = map == 255 ? (byte)0 : map;
                    GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                    GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                    GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                    GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                    GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = CustomGameOptions.CrewVision;
                    GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                    GameOptionsManager.Instance.currentNormalGameOptions.VisualTasks = CustomGameOptions.VisualTasks;
                    GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = CustomGameOptions.IntruderCount;
                    GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                    GameOptionsManager.Instance.currentNormalGameOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)CustomGameOptions.TaskBarMode;
                    GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                    GameOptionsManager.Instance.currentNormalGameOptions.VotingTime = CustomGameOptions.VotingTime;
                    GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                    GameOptionsManager.Instance.currentNormalGameOptions.KillDistance = CustomGameOptions.InteractionDistance;
                    GameOptionsManager.Instance.currentNormalGameOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                    GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                    GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;

                    if (CustomGameOptions.AutoAdjustSettings)
                        RandomMap.AdjustSettings(map);

                    break;

                case CustomRPC.Change:
                    var id5 = reader.ReadByte();

                    switch ((TurnRPC)id5)
                    {
                        case TurnRPC.TurnTraitorBetrayer:
                            Objectifier.GetObjectifier<Traitor>(Utils.PlayerById(reader.ReadByte())).TurnBetrayer();
                            break;

                        case TurnRPC.TurnFanaticBetrayer:
                            Objectifier.GetObjectifier<Fanatic>(Utils.PlayerById(reader.ReadByte())).TurnBetrayer();
                            break;

                        case TurnRPC.TurnPestilence:
                            Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                            break;

                        case TurnRPC.TurnVigilante:
                            Role.GetRole<VampireHunter>(Utils.PlayerById(reader.ReadByte())).TurnVigilante();
                            break;

                        case TurnRPC.TurnTroll:
                            Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TurnTroll();
                            break;

                        case TurnRPC.TurnSurv:
                            Role.GetRole<GuardianAngel>(Utils.PlayerById(reader.ReadByte())).TurnSurv();
                            break;

                        case TurnRPC.TurnGodfather:
                            Role.GetRole<Mafioso>(Utils.PlayerById(reader.ReadByte())).TurnGodfather();
                            break;

                        case TurnRPC.TurnJest:
                            Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TurnJest();
                            break;

                        case TurnRPC.TurnRebel:
                            Role.GetRole<Sidekick>(Utils.PlayerById(reader.ReadByte())).TurnRebel();
                            break;

                        case TurnRPC.TurnSheriff:
                            Role.GetRole<Seer>(Utils.PlayerById(reader.ReadByte())).TurnSheriff();
                            break;

                        case TurnRPC.TurnAct:
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TurnAct();
                            break;

                        case TurnRPC.TurnSeer:
                            Role.GetRole<Mystic>(Utils.PlayerById(reader.ReadByte())).TurnSeer();
                            break;

                        case TurnRPC.TurnTraitor:
                            CompleteTasksPatch.TurnTraitor(Utils.PlayerById(reader.ReadByte()));
                            break;

                        case TurnRPC.TurnFanatic:
                            var attacker = Utils.PlayerById(reader.ReadByte());
                            var fanatic = Utils.PlayerById(reader.ReadByte());
                            var attackerRole = Role.GetRole(attacker);
                            Fanatic.TurnFanatic(fanatic, attackerRole.Faction);
                            break;
                    }

                    break;

                case CustomRPC.Target:
                    var id2 = reader.ReadByte();

                    switch ((TargetRPC)id2)
                    {
                        case TargetRPC.SetExeTarget:
                            var exe = Utils.PlayerById(reader.ReadByte());
                            var exeTarget = Utils.PlayerById(reader.ReadByte());
                            var exeRole = Role.GetRole<Executioner>(exe);
                            exeRole.TargetPlayer = exeTarget;
                            break;

                        case TargetRPC.SetGuessTarget:
                            var guess = Utils.PlayerById(reader.ReadByte());
                            var guessTarget = Utils.PlayerById(reader.ReadByte());
                            var guessRole = Role.GetRole<Guesser>(guess);
                            guessRole.TargetPlayer = guessTarget;
                            break;

                        case TargetRPC.SetGATarget:
                            var ga = Utils.PlayerById(reader.ReadByte());
                            var gaTarget = Utils.PlayerById(reader.ReadByte());
                            var gaRole = Role.GetRole<GuardianAngel>(ga);
                            gaRole.TargetPlayer = gaTarget;
                            break;

                        case TargetRPC.SetBHTarget:
                            var bh = Utils.PlayerById(reader.ReadByte());
                            var bhTarget = Utils.PlayerById(reader.ReadByte());
                            var bhRole = Role.GetRole<BountyHunter>(bh);
                            bhRole.TargetPlayer = bhTarget;
                            break;

                        case TargetRPC.SetActPretendList:
                            var act = Utils.PlayerById(reader.ReadByte());
                            var targetRoles = reader.ReadByte();
                            var actRole = Role.GetRole<Actor>(act);
                            actRole.PretendRoles = (InspectorResults)targetRoles;
                            break;

                        case TargetRPC.SetGoodRecruit:
                            var jackal = Utils.PlayerById(reader.ReadByte());
                            var goodRecruit = Utils.PlayerById(reader.ReadByte());
                            var jackalRole = Role.GetRole<Jackal>(jackal);
                            jackalRole.GoodRecruit = goodRecruit;
                            jackalRole.Recruited.Add(goodRecruit.PlayerId);
                            Role.GetRole(goodRecruit).SubFaction = SubFaction.Cabal;
                            Role.GetRole(goodRecruit).SubFactionColor = Colors.Cabal;
                            Role.GetRole(goodRecruit).IsRecruit = true;
                            break;

                        case TargetRPC.SetEvilRecruit:
                            var jackal2 = Utils.PlayerById(reader.ReadByte());
                            var evilRecruit = Utils.PlayerById(reader.ReadByte());
                            var jackalRole2 = Role.GetRole<Jackal>(jackal2);
                            jackalRole2.EvilRecruit = evilRecruit;
                            jackalRole2.Recruited.Add(evilRecruit.PlayerId);
                            Role.GetRole(evilRecruit).SubFactionColor = Colors.Cabal;
                            Role.GetRole(evilRecruit).SubFaction = SubFaction.Cabal;
                            Role.GetRole(evilRecruit).IsRecruit = true;
                            break;

                        case TargetRPC.SetAlliedFaction:
                            var player6 = Utils.PlayerById(reader.ReadByte());
                            var alliedRole = Role.GetRole(player6);
                            var ally = Objectifier.GetObjectifier<Allied>(player6);
                            var faction = (Faction)reader.ReadByte();
                            alliedRole.Faction = faction;

                            if (faction == Faction.Crew)
                            {
                                alliedRole.FactionColor = Colors.Crew;
                                alliedRole.IsCrewAlly = true;
                                ally.Color = Colors.Crew;
                            }
                            else if (faction == Faction.Intruder)
                            {
                                alliedRole.FactionColor = Colors.Intruder;
                                alliedRole.IsIntAlly = true;
                                ally.Color = Colors.Intruder;
                            }
                            else if (faction == Faction.Syndicate)
                            {
                                alliedRole.FactionColor = Colors.Syndicate;
                                alliedRole.IsSynAlly = true;
                                ally.Color = Colors.Syndicate;
                            }

                            ally.Side = alliedRole.Faction;
                            break;

                        case TargetRPC.SetCouple:
                            var lover1 = Utils.PlayerById(reader.ReadByte());
                            var lover2 = Utils.PlayerById(reader.ReadByte());
                            Objectifier.GetObjectifier<Lovers>(lover1).OtherLover = lover2;
                            Objectifier.GetObjectifier<Lovers>(lover2).OtherLover = lover1;
                            break;

                        case TargetRPC.SetDuo:
                            var rival1 = Utils.PlayerById(reader.ReadByte());
                            var rival2 = Utils.PlayerById(reader.ReadByte());
                            Objectifier.GetObjectifier<Rivals>(rival1).OtherRival = rival2;
                            Objectifier.GetObjectifier<Rivals>(rival2).OtherRival = rival1;
                            break;
                    }

                    break;

                case CustomRPC.Action:
                    var id6 = reader.ReadByte();

                    switch ((ActionsRPC)id6)
                    {
                        case ActionsRPC.RetributionistAction:
                            var retId = reader.ReadByte();

                            switch ((RetributionistActionsRPC)retId)
                            {
                                case RetributionistActionsRPC.RetributionistRevive:
                                    var retRole2 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    retRole2.Revived = Utils.PlayerById(reader.ReadByte());
                                    retRole2.RevivedRole = retRole2.Revived == null ? null : (retRole2.Revived.Is(RoleEnum.Revealer) ? Role.GetRole<Revealer>(retRole2.Revived).FormerRole :
                                        Role.GetRole(retRole2.Revived));
                                    break;

                                case RetributionistActionsRPC.Transport:
                                    var ret3 = Utils.PlayerById(reader.ReadByte());
                                    var transed1_1 = Utils.PlayerById(reader.ReadByte());
                                    var transed2_1 = Utils.PlayerById(reader.ReadByte());
                                    var retRole3 = Role.GetRole<Retributionist>(ret3);
                                    retRole3.TransportPlayer1 = transed1_1;
                                    retRole3.TransportPlayer2 = transed2_1;
                                    Coroutines.Start(retRole3.TransportPlayers());
                                    break;

                                case RetributionistActionsRPC.Protect:
                                    var ret5 = Utils.PlayerById(reader.ReadByte());
                                    var shielded2 = Utils.PlayerById(reader.ReadByte());
                                    var retRole5 = Role.GetRole<Retributionist>(ret5);
                                    retRole5.ShieldedPlayer = shielded2;
                                    break;

                                case RetributionistActionsRPC.Alert:
                                    var ret7 = Utils.PlayerById(reader.ReadByte());
                                    var retRole7 = Role.GetRole<Retributionist>(ret7);
                                    retRole7.TimeRemaining = CustomGameOptions.AlertDuration;
                                    retRole7.Alert();
                                    break;

                                case RetributionistActionsRPC.AltruistRevive:
                                    var ret8 = Utils.PlayerById(reader.ReadByte());
                                    var retRole8 = Role.GetRole<Retributionist>(ret8);
                                    retRole8.RevivingBody = Utils.BodyById(reader.ReadByte());
                                    retRole8.TimeRemaining = CustomGameOptions.AltReviveDuration;
                                    retRole8.Revive();
                                    break;

                                case RetributionistActionsRPC.Swoop:
                                    var ret9 = Utils.PlayerById(reader.ReadByte());
                                    var retRole9 = Role.GetRole<Retributionist>(ret9);
                                    retRole9.TimeRemaining = CustomGameOptions.SwoopDuration;
                                    retRole9.Invis();
                                    Utils.Invis(ret9);
                                    break;

                                case RetributionistActionsRPC.Mediate:
                                    var mediatedPlayer2 = Utils.PlayerById(reader.ReadByte());
                                    var retRole10 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));

                                    if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer2.PlayerId)
                                        break;

                                    retRole10.AddMediatePlayer(mediatedPlayer2.PlayerId);
                                    break;

                                case RetributionistActionsRPC.EscRoleblock:
                                    var ret1 = Utils.PlayerById(reader.ReadByte());
                                    var blocked4 = Utils.PlayerById(reader.ReadByte());
                                    var retRole1 = Role.GetRole<Retributionist>(ret1);

                                    foreach (var layer in PlayerLayer.GetLayers(blocked4))
                                        layer.IsBlocked = !Role.GetRole(blocked4).RoleBlockImmune;

                                    retRole1.BlockTarget = blocked4;
                                    retRole1.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                                    retRole1.Block();
                                    break;
                            }

                            break;

                        case ActionsRPC.GodfatherAction:
                            var gfId = reader.ReadByte();

                            switch ((GodfatherActionsRPC)gfId)
                            {
                                case GodfatherActionsRPC.Morph:
                                    var gf3 = Utils.PlayerById(reader.ReadByte());
                                    var morphTarget2 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole3 = Role.GetRole<PromotedGodfather>(gf3);
                                    gfRole3.TimeRemaining = CustomGameOptions.MorphlingDuration;
                                    gfRole3.MorphedPlayer = morphTarget2;
                                    gfRole3.Morph();
                                    break;

                                case GodfatherActionsRPC.Disguise:
                                    var gf4 = Utils.PlayerById(reader.ReadByte());
                                    var disguiseTarget2 = Utils.PlayerById(reader.ReadByte());
                                    var disguiserForm2 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole4 = Role.GetRole<PromotedGodfather>(gf4);
                                    gfRole4.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                    gfRole4.TimeRemaining = CustomGameOptions.DisguiseDuration;
                                    gfRole4.DisguisedPlayer = disguiseTarget2;
                                    gfRole4.DisguisePlayer = disguiserForm2;
                                    gfRole4.DisgDelay();
                                    break;

                                case GodfatherActionsRPC.ConsRoleblock:
                                    var gf5 = Utils.PlayerById(reader.ReadByte());
                                    var blocked5 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole5 = Role.GetRole<PromotedGodfather>(gf5);

                                    foreach (var layer in PlayerLayer.GetLayers(blocked5))
                                        layer.IsBlocked = !Role.GetRole(blocked5).RoleBlockImmune;

                                    gfRole5.BlockTarget = blocked5;
                                    gfRole5.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                                    gfRole5.Block();
                                    break;

                                case GodfatherActionsRPC.Blackmail:
                                    Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte())).BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                                    break;

                                case GodfatherActionsRPC.Invis:
                                    var gf8 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole8 = Role.GetRole<PromotedGodfather>(gf8);
                                    gfRole8.TimeRemaining = CustomGameOptions.InvisDuration;
                                    gfRole8.Invis();
                                    Utils.Invis(gf8, PlayerControl.LocalPlayer.Is(Faction.Intruder));
                                    break;

                                case GodfatherActionsRPC.Drag:
                                    Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte())).CurrentlyDragging = Utils.BodyById(reader.ReadByte());
                                    break;

                                case GodfatherActionsRPC.Drop:
                                    var gf10 = Utils.PlayerById(reader.ReadByte());
                                    var v2_1 = reader.ReadVector2();
                                    var v2z_1 = reader.ReadSingle();
                                    var gfRole10 = Role.GetRole<PromotedGodfather>(gf10);
                                    gfRole10.CurrentlyDragging.transform.position = new(v2_1.x, v2_1.y, v2z_1);
                                    gfRole10.CurrentlyDragging = null;
                                    break;

                                case GodfatherActionsRPC.Camouflage:
                                    var gf11 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole11 = Role.GetRole<PromotedGodfather>(gf11);
                                    Utils.Camouflage();
                                    gfRole11.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                                    gfRole11.Camouflage();
                                    break;

                                case GodfatherActionsRPC.FlashGrenade:
                                    var gf12 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole12 = Role.GetRole<PromotedGodfather>(gf12);
                                    gfRole12.TimeRemaining = CustomGameOptions.GrenadeDuration;
                                    gfRole12.Flash();
                                    break;

                                case GodfatherActionsRPC.SetBomb:
                                    var gf13 = Utils.PlayerById(reader.ReadByte());
                                    var victim3 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole13 = Role.GetRole<PromotedGodfather>(gf13);
                                    gfRole13.TimeRemaining = CustomGameOptions.EnforceDuration;
                                    gfRole13.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                                    gfRole13.BombedPlayer = victim3;
                                    gfRole13.BombDelay();
                                    break;

                                case GodfatherActionsRPC.Ambush:
                                    var gf14 = Utils.PlayerById(reader.ReadByte());
                                    var ambushed2 = Utils.PlayerById(reader.ReadByte());
                                    var gfRole14 = Role.GetRole<PromotedGodfather>(gf14);
                                    gfRole14.TimeRemaining = CustomGameOptions.AmbushDuration;
                                    gfRole14.AmbushedPlayer = ambushed2;
                                    gfRole14.Ambush();
                                    break;
                            }

                            break;

                        case ActionsRPC.RebelAction:
                            var rebId = reader.ReadByte();

                            switch ((RebelActionsRPC)rebId)
                            {
                                case RebelActionsRPC.Poison:
                                    var reb1 = Utils.PlayerById(reader.ReadByte());
                                    var poisoned2 = Utils.PlayerById(reader.ReadByte());
                                    var rebRole1 = Role.GetRole<PromotedRebel>(reb1);
                                    rebRole1.PoisonedPlayer = poisoned2;
                                    rebRole1.TimeRemaining = CustomGameOptions.PoisonDuration;
                                    rebRole1.Poison();
                                    break;

                                case RebelActionsRPC.Conceal:
                                    var reb2 = Utils.PlayerById(reader.ReadByte());
                                    var rebRole2 = Role.GetRole<PromotedRebel>(reb2);
                                    rebRole2.TimeRemaining = CustomGameOptions.ConcealDuration;
                                    rebRole2.Conceal();

                                    if (Role.SyndicateHasChaosDrive)
                                        Utils.Conceal();
                                    else
                                    {
                                        rebRole2.ConcealedPlayer = Utils.PlayerById(reader.ReadByte());
                                        Utils.Invis(rebRole2.ConcealedPlayer, PlayerControl.LocalPlayer.Is(Faction.Syndicate));
                                    }

                                    break;

                                case RebelActionsRPC.Shapeshift:
                                    var reb3 = Utils.PlayerById(reader.ReadByte());
                                    var rebRole3 = Role.GetRole<PromotedRebel>(reb3);
                                    rebRole3.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                    rebRole3.Shapeshift();

                                    if (Role.SyndicateHasChaosDrive)
                                        Utils.Shapeshift();
                                    else
                                    {
                                        rebRole3.ShapeshiftPlayer1 = Utils.PlayerById(reader.ReadByte());
                                        rebRole3.ShapeshiftPlayer2 = Utils.PlayerById(reader.ReadByte());
                                        Utils.Morph(rebRole3.ShapeshiftPlayer1, rebRole3.ShapeshiftPlayer2);
                                        Utils.Morph(rebRole3.ShapeshiftPlayer2, rebRole3.ShapeshiftPlayer1);
                                    }

                                    break;

                                case RebelActionsRPC.Warp:
                                    var reb4 = Utils.PlayerById(reader.ReadByte());
                                    var warped1_1 = Utils.PlayerById(reader.ReadByte());
                                    var warped2_1 = Utils.PlayerById(reader.ReadByte());
                                    var rebRole4 = Role.GetRole<Warper>(reb4);
                                    rebRole4.WarpPlayer1 = warped1_1;
                                    rebRole4.WarpPlayer2 = warped2_1;
                                    Coroutines.Start(rebRole4.WarpPlayers());
                                    break;

                                case RebelActionsRPC.Frame:
                                    var reb6 = Utils.PlayerById(reader.ReadByte());
                                    var rebRole6 = Role.GetRole<PromotedRebel>(reb6);
                                    rebRole6.Framed.Add(reader.ReadByte());
                                    break;

                                case RebelActionsRPC.Crusade:
                                    var reb7 = Utils.PlayerById(reader.ReadByte());
                                    var crused2 = Utils.PlayerById(reader.ReadByte());
                                    var rebRole7 = Role.GetRole<PromotedRebel>(reb7);
                                    rebRole7.TimeRemaining = CustomGameOptions.CrusadeDuration;
                                    rebRole7.CrusadedPlayer = crused2;
                                    rebRole7.Crusade();
                                    break;
                            }

                            break;

                        case ActionsRPC.FreezeDouse:
                            var cryomaniac = Utils.PlayerById(reader.ReadByte());
                            var freezedouseTarget = Utils.PlayerById(reader.ReadByte());
                            var cryomaniacRole = Role.GetRole<Cryomaniac>(cryomaniac);
                            cryomaniacRole.DousedPlayers.Add(freezedouseTarget.PlayerId);
                            break;

                        case ActionsRPC.RevealerFinished:
                            var revealer2 = Utils.PlayerById(reader.ReadByte());
                            var revealerRole2 = Role.GetRole<Revealer>(revealer2);
                            revealerRole2.CompletedTasks = true;
                            break;

                        case ActionsRPC.FadeBody:
                            Coroutines.Start(Utils.FadeBody(Utils.BodyById(reader.ReadByte())));
                            break;

                        case ActionsRPC.FixLights:
                            var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                            lights.ActualSwitches = lights.ExpectedSwitches;
                            break;

                        case ActionsRPC.SetExtraVotes:
                            var politian = Utils.PlayerById(reader.ReadByte());
                            var polRole = Ability.GetAbility<Politician>(politian);
                            polRole.ExtraVotes = reader.ReadBytesAndSize().ToList();
                            polRole.VoteBank -= polRole.ExtraVotes.Count;
                            break;

                        case ActionsRPC.SetSwaps:
                            var swapper = Ability.GetAbility<Swapper>(Utils.PlayerById(reader.ReadByte()));
                            swapper.Swap1 = Utils.VoteAreaById(reader.ReadByte());
                            swapper.Swap2 = Utils.VoteAreaById(reader.ReadByte());
                            break;

                        case ActionsRPC.Remember:
                            var amnesiac = Utils.PlayerById(reader.ReadByte());
                            var other = Utils.PlayerById(reader.ReadByte());
                            Amnesiac.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                            break;

                        case ActionsRPC.Steal:
                            var thief = Utils.PlayerById(reader.ReadByte());
                            var other4 = Utils.PlayerById(reader.ReadByte());
                            Thief.Steal(Role.GetRole<Thief>(thief), other4);
                            break;

                        case ActionsRPC.Shift:
                            var shifter = Utils.PlayerById(reader.ReadByte());
                            var other2 = Utils.PlayerById(reader.ReadByte());
                            Shifter.Shift(Role.GetRole<Shifter>(shifter), other2);
                            break;

                        case ActionsRPC.Convert:
                            RoleGen.Convert(reader.ReadByte(), reader.ReadByte(), (SubFaction)reader.ReadByte(), reader.ReadBoolean());
                            break;

                        case ActionsRPC.Teleport:
                            var teleporter = Utils.PlayerById(reader.ReadByte());
                            Utils.Teleport(teleporter, reader.ReadVector2());
                            break;

                        case ActionsRPC.Sidekick:
                            var reb = Utils.PlayerById(reader.ReadByte());
                            var side = Utils.PlayerById(reader.ReadByte());
                            Rebel.Sidekick(Role.GetRole<Rebel>(reb), side);
                            break;

                        case ActionsRPC.Frame:
                            var framer = Utils.PlayerById(reader.ReadByte());
                            var framerRole = Role.GetRole<Framer>(framer);
                            framerRole.Framed.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Declare:
                            var gf = Utils.PlayerById(reader.ReadByte());
                            var maf = Utils.PlayerById(reader.ReadByte());
                            Godfather.Declare(Role.GetRole<Godfather>(gf), maf);
                            break;

                        case ActionsRPC.Protect:
                            var medic = Utils.PlayerById(reader.ReadByte());
                            Role.GetRole<Medic>(medic).ShieldedPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case ActionsRPC.BypassKill:
                            Utils.MurderPlayer(Utils.PlayerById(reader.ReadByte()), Utils.PlayerById(reader.ReadByte()), (DeathReasonEnum)reader.ReadByte(), reader.ReadBoolean());
                            break;

                        case ActionsRPC.AssassinKill:
                            var toDie = Utils.PlayerById(reader.ReadByte());
                            var guessString = reader.ReadString();
                            Ability.GetAbility<Assassin>(Utils.PlayerById(reader.ReadByte())).MurderPlayer(toDie, guessString);
                            break;

                        case ActionsRPC.GuesserKill:
                            var toDie2 = Utils.PlayerById(reader.ReadByte());
                            var guessString2 = reader.ReadString();
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).MurderPlayer(toDie2, guessString2);
                            break;

                        case ActionsRPC.ForceKill:
                            var victim = Utils.PlayerById(reader.ReadByte());
                            victim.GetEnforcer().BombSuccessful = reader.ReadBoolean();
                            Role.GetRole(victim).Bombed = false;
                            break;

                        case ActionsRPC.SetBomb:
                            var enf = Utils.PlayerById(reader.ReadByte());
                            var victim2 = Utils.PlayerById(reader.ReadByte());
                            var enfRole = Role.GetRole<Enforcer>(enf);
                            enfRole.TimeRemaining = CustomGameOptions.EnforceDuration;
                            enfRole.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                            enfRole.BombedPlayer = victim2;
                            enfRole.Delay();
                            break;

                        case ActionsRPC.Morph:
                            var morphling = Utils.PlayerById(reader.ReadByte());
                            var morphTarget = Utils.PlayerById(reader.ReadByte());
                            var morphRole = Role.GetRole<Morphling>(morphling);
                            morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                            morphRole.MorphedPlayer = morphTarget;
                            morphRole.Morph();
                            break;

                        case ActionsRPC.Scream:
                            var banshee2 = Utils.PlayerById(reader.ReadByte());
                            var bansheeRole2 = Role.GetRole<Banshee>(banshee2);
                            bansheeRole2.TimeRemaining = CustomGameOptions.ScreamDuration;

                            foreach (var player8 in PlayerControl.AllPlayerControls)
                            {
                                if (!player8.Data.IsDead && !player8.Data.Disconnected && !player8.Is(Faction.Syndicate))
                                    bansheeRole2.Blocked.Add(player8.PlayerId);
                            }

                            bansheeRole2.Scream();
                            break;

                        case ActionsRPC.Mark:
                            var ghoul2 = Utils.PlayerById(reader.ReadByte());
                            var marked = Utils.PlayerById(reader.ReadByte());
                            var ghoulRole2 = Role.GetRole<Ghoul>(ghoul2);
                            ghoulRole2.MarkedPlayer = marked;
                            break;

                        case ActionsRPC.Disguise:
                            var disguiser = Utils.PlayerById(reader.ReadByte());
                            var disguiserForm = Utils.PlayerById(reader.ReadByte());
                            var disguiseTarget = Utils.PlayerById(reader.ReadByte());
                            var disguiseRole = Role.GetRole<Disguiser>(disguiser);
                            disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                            disguiseRole.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                            disguiseRole.DisguisePlayer = disguiserForm;
                            disguiseRole.DisguisedPlayer = disguiseTarget;
                            disguiseRole.Delay();
                            break;

                        case ActionsRPC.Poison:
                            var poisoner = Utils.PlayerById(reader.ReadByte());
                            var poisoned = Utils.PlayerById(reader.ReadByte());
                            var poisonerRole = Role.GetRole<Poisoner>(poisoner);
                            poisonerRole.PoisonedPlayer = poisoned;
                            break;

                        case ActionsRPC.Blackmail:
                            var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                            blackmailer.BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case ActionsRPC.Mine:
                            var ventId = reader.ReadInt32();
                            var miner = Utils.PlayerById(reader.ReadByte());
                            var minerRole = Role.GetRole(miner);
                            var pos = reader.ReadVector2();
                            var zAxis = reader.ReadSingle();
                            Utils.SpawnVent(ventId, minerRole, pos, zAxis);
                            break;

                        case ActionsRPC.Invis:
                            var wraith = Utils.PlayerById(reader.ReadByte());
                            var wraithRole = Role.GetRole<Wraith>(wraith);
                            wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                            wraithRole.Invis();
                            Utils.Invis(wraith, PlayerControl.LocalPlayer.Is(Faction.Intruder));
                            break;

                        case ActionsRPC.Alert:
                            var veteran = Utils.PlayerById(reader.ReadByte());
                            var veteranRole = Role.GetRole<Veteran>(veteran);
                            veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                            veteranRole.Alert();
                            break;

                        case ActionsRPC.Vest:
                            var surv = Utils.PlayerById(reader.ReadByte());
                            var survRole = Role.GetRole<Survivor>(surv);
                            survRole.TimeRemaining = CustomGameOptions.VestDuration;
                            survRole.Vest();
                            break;

                        case ActionsRPC.Ambush:
                            var amb = Utils.PlayerById(reader.ReadByte());
                            var ambushed = Utils.PlayerById(reader.ReadByte());
                            var ambRole = Role.GetRole<Ambusher>(amb);
                            ambRole.TimeRemaining = CustomGameOptions.AmbushDuration;
                            ambRole.AmbushedPlayer = ambushed;
                            ambRole.Ambush();
                            break;

                        case ActionsRPC.Crusade:
                            var crus = Utils.PlayerById(reader.ReadByte());
                            var crused = Utils.PlayerById(reader.ReadByte());
                            var crusRole = Role.GetRole<Crusader>(crus);
                            crusRole.TimeRemaining = CustomGameOptions.CrusadeDuration;
                            crusRole.CrusadedPlayer = crused;
                            crusRole.Crusade();
                            break;

                        case ActionsRPC.GAProtect:
                            var ga2 = Utils.PlayerById(reader.ReadByte());
                            var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                            ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                            ga2Role.Protect();
                            break;

                        case ActionsRPC.Transport:
                            var trans = Utils.PlayerById(reader.ReadByte());
                            var transed1 = Utils.PlayerById(reader.ReadByte());
                            var transed2 = Utils.PlayerById(reader.ReadByte());
                            var transRole = Role.GetRole<Transporter>(trans);
                            transRole.TransportPlayer1 = transed1;
                            transRole.TransportPlayer2 = transed2;
                            Coroutines.Start(transRole.TransportPlayers());
                            break;

                        case ActionsRPC.SetUninteractable:
                            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                                Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                                Role.GetRole<Warper>(PlayerControl.LocalPlayer).UnwarpablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                                Role.GetRole<Retributionist>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                            break;

                        case ActionsRPC.Warp:
                            var warp = Utils.PlayerById(reader.ReadByte());
                            var warped1 = Utils.PlayerById(reader.ReadByte());
                            var warped2 = Utils.PlayerById(reader.ReadByte());
                            var warpRole = Role.GetRole<Warper>(warp);
                            warpRole.WarpPlayer1 = warped1;
                            warpRole.WarpPlayer2 = warped2;
                            Coroutines.Start(warpRole.WarpPlayers());
                            break;

                        case ActionsRPC.Mediate:
                            var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                            var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));

                            if (PlayerControl.LocalPlayer != mediatedPlayer)
                                break;

                            medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                            break;

                        case ActionsRPC.FlashGrenade:
                            var grenadier = Utils.PlayerById(reader.ReadByte());
                            var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                            grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                            grenadierRole.Flash();
                            break;

                        case ActionsRPC.Douse:
                            var arsonist = Utils.PlayerById(reader.ReadByte());
                            var douseTarget = Utils.PlayerById(reader.ReadByte());
                            var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                            arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                            break;

                        case ActionsRPC.Infect:
                            var pb = Utils.PlayerById(reader.ReadByte());
                            var infected = reader.ReadByte();
                            Role.GetRole<Plaguebearer>(pb).InfectedPlayers.Add(infected);
                            break;

                        case ActionsRPC.AltruistRevive:
                            var altruistPlayer = Utils.PlayerById(reader.ReadByte());
                            var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                            altruistRole.RevivingBody = Utils.BodyById(reader.ReadByte());
                            altruistRole.TimeRemaining = CustomGameOptions.AltReviveDuration;
                            altruistRole.Revive();
                            break;

                        case ActionsRPC.NecromancerResurrect:
                            var necroPlayer = Utils.PlayerById(reader.ReadByte());
                            var necroRole = Role.GetRole<Necromancer>(necroPlayer);
                            necroRole.ResurrectingBody = Utils.BodyById(reader.ReadByte());
                            necroRole.TimeRemaining = CustomGameOptions.NecroResurrectDuration;
                            necroRole.Resurrect();
                            break;

                        case ActionsRPC.WarpAll:
                            var teleports = reader.ReadByte();
                            var coordinates = new Dictionary<byte, Vector2>();

                            for (var i = 0; i < teleports; i++)
                            {
                                var playerId = reader.ReadByte();
                                var location = reader.ReadVector2();
                                coordinates.Add(playerId, location);
                            }

                            Utils.WarpPlayersToCoordinates(coordinates);
                            break;

                        case ActionsRPC.Swoop:
                            var chameleon = Utils.PlayerById(reader.ReadByte());
                            var chameleonRole = Role.GetRole<Chameleon>(chameleon);
                            chameleonRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                            chameleonRole.Invis();
                            break;

                        case ActionsRPC.BarryButton:
                            var buttonBarry = Utils.PlayerById(reader.ReadByte());

                            if (AmongUsClient.Instance.AmHost)
                            {
                                MeetingRoomManager.Instance.reporter = buttonBarry;
                                MeetingRoomManager.Instance.target = null;
                                AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                                HudManager.Instance.OpenMeetingRoom(buttonBarry);
                                buttonBarry.RpcStartMeeting(null);
                            }

                            break;

                        case ActionsRPC.BaitReport:
                            var baitKiller = Utils.PlayerById(reader.ReadByte());
                            var bait = Utils.PlayerById(reader.ReadByte());
                            baitKiller.ReportDeadBody(bait.Data);
                            break;

                        case ActionsRPC.Drag:
                            var dienerPlayer = Utils.PlayerById(reader.ReadByte());
                            var dienerRole = Role.GetRole<Janitor>(dienerPlayer);
                            dienerRole.CurrentlyDragging = Utils.BodyById(reader.ReadByte());
                            break;

                        case ActionsRPC.Drop:
                            var dienerPlayer2 = Utils.PlayerById(reader.ReadByte());
                            var v2 = reader.ReadVector2();
                            var v2z = reader.ReadSingle();
                            var dienerRole2 = Role.GetRole<Janitor>(dienerPlayer2);
                            var body3 = dienerRole2.CurrentlyDragging;
                            dienerRole2.CurrentlyDragging = null;
                            body3.transform.position = new Vector3(v2.x, v2.y, v2z);
                            break;

                        case ActionsRPC.Camouflage:
                            var camouflager = Utils.PlayerById(reader.ReadByte());
                            var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                            Utils.Camouflage();
                            camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                            camouflagerRole.Camouflage();
                            break;

                        case ActionsRPC.EscRoleblock:
                            var escort = Utils.PlayerById(reader.ReadByte());
                            var blocked2 = Utils.PlayerById(reader.ReadByte());
                            var escortRole = Role.GetRole<Escort>(escort);
                            escortRole.BlockTarget = blocked2;
                            escortRole.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                            escortRole.Block();
                            break;

                        case ActionsRPC.GlitchRoleblock:
                            var glitch = Utils.PlayerById(reader.ReadByte());
                            var blocked3 = Utils.PlayerById(reader.ReadByte());
                            var glitchRole = Role.GetRole<Glitch>(glitch);
                            glitchRole.HackTarget = blocked3;
                            glitchRole.TimeRemaining = CustomGameOptions.HackDuration;
                            glitchRole.Hack();
                            break;

                        case ActionsRPC.ConsRoleblock:
                            var consort = Utils.PlayerById(reader.ReadByte());
                            var blocked = Utils.PlayerById(reader.ReadByte());
                            var consortRole = Role.GetRole<Consort>(consort);
                            consortRole.BlockTarget = blocked;
                            consortRole.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                            consortRole.Block();
                            break;

                        case ActionsRPC.Mimic:
                            var glitch4 = Utils.PlayerById(reader.ReadByte());
                            var mimicTarget = Utils.PlayerById(reader.ReadByte());
                            var glitchRole4 = Role.GetRole<Glitch>(glitch4);
                            glitchRole4.MimicTarget = mimicTarget;
                            glitchRole4.TimeRemaining2 = CustomGameOptions.MimicDuration;
                            glitchRole4.Mimic();
                            break;

                        case ActionsRPC.Conceal:
                            var concealer = Utils.PlayerById(reader.ReadByte());
                            var concealerRole = Role.GetRole<Concealer>(concealer);

                            if (Role.SyndicateHasChaosDrive)
                                Utils.Conceal();
                            else
                            {
                                concealerRole.ConcealedPlayer = Utils.PlayerById(reader.ReadByte());
                                Utils.Invis(concealerRole.ConcealedPlayer, PlayerControl.LocalPlayer.Is(Faction.Syndicate));
                            }

                            concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                            concealerRole.Conceal();
                            break;

                        case ActionsRPC.Shapeshift:
                            var ss = Utils.PlayerById(reader.ReadByte());
                            var ssRole = Role.GetRole<Shapeshifter>(ss);

                            if (Role.SyndicateHasChaosDrive)
                                Utils.Shapeshift();
                            else
                            {
                                ssRole.ShapeshiftPlayer1 = Utils.PlayerById(reader.ReadByte());
                                ssRole.ShapeshiftPlayer2 = Utils.PlayerById(reader.ReadByte());
                                Utils.Morph(ssRole.ShapeshiftPlayer1, ssRole.ShapeshiftPlayer2);
                                Utils.Morph(ssRole.ShapeshiftPlayer2, ssRole.ShapeshiftPlayer1);
                            }

                            ssRole.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                            ssRole.Shapeshift();
                            break;

                        case ActionsRPC.Burn:
                            var arso = Utils.PlayerById(reader.ReadByte());
                            var arsoRole = Role.GetRole<Arsonist>(arso);

                            foreach (var body in Object.FindObjectsOfType<DeadBody>())
                            {
                                if (arsoRole.DousedPlayers.Contains(body.ParentId))
                                {
                                    Coroutines.Start(Utils.FadeBody(body));
                                    _ = new Ash(body.TruePosition);
                                }
                            }

                            break;

                        case ActionsRPC.MayorReveal:
                            var mayor = Utils.PlayerById(reader.ReadByte());
                            var mayorRole = Role.GetRole<Mayor>(mayor);
                            mayorRole.Revealed = true;
                            Utils.Flash(Colors.Mayor);

                            foreach (var medic1 in Role.GetRoles<Medic>(RoleEnum.Medic))
                            {
                                if (medic1.ShieldedPlayer == mayor)
                                    Medic.BreakShield(medic1.PlayerId, mayor.PlayerId, true);
                            }

                            foreach (var ret1 in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
                            {
                                if (ret1.ShieldedPlayer == mayor)
                                    Retributionist.BreakShield(ret1.PlayerId, mayor.PlayerId, true);
                            }

                            break;

                        case ActionsRPC.DictatorReveal:
                            var dictator = Utils.PlayerById(reader.ReadByte());
                            var dictatorRole = Role.GetRole<Dictator>(dictator);
                            dictatorRole.Revealed = true;
                            Utils.Flash(Colors.Mayor);

                            foreach (var medic1 in Role.GetRoles<Medic>(RoleEnum.Medic))
                            {
                                if (medic1.ShieldedPlayer == dictator)
                                    Medic.BreakShield(medic1.PlayerId, dictator.PlayerId, true);
                            }

                            foreach (var ret1 in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
                            {
                                if (ret1.ShieldedPlayer == dictator)
                                    Retributionist.BreakShield(ret1.PlayerId, dictator.PlayerId, true);
                            }

                            break;

                        case ActionsRPC.SetExiles:
                            var dict = Utils.PlayerById(reader.ReadByte());
                            var dictRole = Role.GetRole<Dictator>(dict);
                            dictRole.ToBeEjected.Add(reader.ReadByte());
                            dictRole.ToBeEjected.Add(reader.ReadByte());
                            dictRole.ToBeEjected.Add(reader.ReadByte());
                            dictRole.ToDie = reader.ReadBoolean();
                            break;

                        case ActionsRPC.Spell:
                            Role.GetRole<Spellslinger>(Utils.PlayerById(reader.ReadByte())).Spelled.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Knight:
                            Role.GetRole<Monarch>(Utils.PlayerById(reader.ReadByte())).ToBeKnighted.Add(reader.ReadByte());
                            break;
                    }

                    break;

                case CustomRPC.WinLose:
                    var id7 = reader.ReadByte();

                    switch ((WinLoseRPC)id7)
                    {
                        case WinLoseRPC.CrewWin:
                            Role.CrewWin = true;
                            break;

                        case WinLoseRPC.IntruderWin:
                            Role.IntruderWin = true;
                            break;

                        case WinLoseRPC.SyndicateWin:
                            Role.SyndicateWin = true;
                            break;

                        case WinLoseRPC.UndeadWin:
                            Role.UndeadWin = true;
                            break;

                        case WinLoseRPC.ReanimatedWin:
                            Role.ReanimatedWin = true;
                            break;

                        case WinLoseRPC.SectWin:
                            Role.SectWin = true;
                            break;

                        case WinLoseRPC.CabalWin:
                            Role.CabalWin = true;
                            break;

                        case WinLoseRPC.NobodyWins:
                            PlayerLayer.NobodyWins = true;
                            break;

                        case WinLoseRPC.AllNeutralsWin:
                            Role.AllNeutralsWin = true;
                            break;

                        case WinLoseRPC.AllNKsWin:
                            Role.NKWins = true;
                            break;

                        case WinLoseRPC.SameNKWins:
                        case WinLoseRPC.SoloNKWins:
                            var nkRole = Role.GetRole(Utils.PlayerById(reader.ReadByte()));

                            switch (nkRole.RoleType)
                            {
                                case RoleEnum.Glitch:
                                    Role.GlitchWins = true;
                                    break;

                                case RoleEnum.Arsonist:
                                    Role.ArsonistWins = true;
                                    break;

                                case RoleEnum.Cryomaniac:
                                    Role.CryomaniacWins = true;
                                    break;

                                case RoleEnum.Juggernaut:
                                    Role.JuggernautWins = true;
                                    break;

                                case RoleEnum.Murderer:
                                    Role.MurdererWins = true;
                                    break;

                                case RoleEnum.Werewolf:
                                    Role.WerewolfWins = true;
                                    break;

                                case RoleEnum.SerialKiller:
                                    Role.SerialKillerWins = true;
                                    break;
                            }

                            if ((WinLoseRPC)id7 == WinLoseRPC.SameNKWins)
                            {
                                foreach (var role in Role.GetRoles(nkRole.RoleType))
                                {
                                    if (!role.Disconnected && role.NotDefective)
                                        role.Winner = true;
                                }
                            }

                            nkRole.Winner = true;
                            break;

                        case WinLoseRPC.InfectorsWin:
                            Role.InfectorsWin = true;
                            break;

                        case WinLoseRPC.JesterWin:
                            Role.GetRole<Jester>(Utils.PlayerById(reader.ReadByte())).VotedOut = true;
                            break;

                        case WinLoseRPC.CannibalWin:
                            Role.GetRole<Cannibal>(Utils.PlayerById(reader.ReadByte())).CannibalWin = true;
                            break;

                        case WinLoseRPC.ExecutionerWin:
                            Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TargetVotedOut = true;
                            break;

                        case WinLoseRPC.BountyHunterWin:
                            Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TargetKilled = true;
                            break;

                        case WinLoseRPC.TrollWin:
                            Role.GetRole<Troll>(Utils.PlayerById(reader.ReadByte())).Killed = true;
                            break;

                        case WinLoseRPC.ActorWin:
                            Role.GetRole<Actor>(Utils.PlayerById(reader.ReadByte())).Guessed = true;
                            break;

                        case WinLoseRPC.GuesserWin:
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TargetGuessed = true;
                            break;

                        case WinLoseRPC.CorruptedWin:
                            Objectifier.CorruptedWins = true;

                            if (CustomGameOptions.AllCorruptedWin)
                            {
                                foreach (var corr in Objectifier.GetObjectifiers<Corrupted>(ObjectifierEnum.Corrupted))
                                    corr.Winner = true;
                            }

                            Objectifier.GetObjectifier(Utils.PlayerById(reader.ReadByte())).Winner = true;
                            break;

                        case WinLoseRPC.LoveWin:
                            Objectifier.LoveWins = true;
                            var lover = Objectifier.GetObjectifier<Lovers>(Utils.PlayerById(reader.ReadByte()));
                            lover.Winner = true;
                            Objectifier.GetObjectifier(lover.OtherLover).Winner = true;
                            break;

                        case WinLoseRPC.OverlordWin:
                            Objectifier.OverlordWins = true;

                            foreach (var ov in Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord))
                                ov.Winner = true;

                            break;

                        case WinLoseRPC.MafiaWins:
                            Objectifier.MafiaWins = true;
                            break;

                        case WinLoseRPC.TaskmasterWin:
                            Objectifier.TaskmasterWins = true;
                            var tm = Objectifier.GetObjectifier<Taskmaster>(Utils.PlayerById(reader.ReadByte()));
                            tm.Winner = true;
                            break;

                        case WinLoseRPC.RivalWin:
                            Objectifier.RivalWins = true;
                            var rival = Objectifier.GetObjectifier<Rivals>(Utils.PlayerById(reader.ReadByte()));
                            rival.Winner = true;
                            break;

                        case WinLoseRPC.PhantomWin:
                            Role.PhantomWins = true;
                            var phantom3 = Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte()));
                            phantom3.CompletedTasks = true;
                            phantom3.Winner = true;
                            break;
                    }

                    Utils.EndGame();
                    break;
            }
        }
    }
}