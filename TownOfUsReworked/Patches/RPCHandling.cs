using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using Reactor.Networking.Extensions;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod;
using Object = UnityEngine.Object;
using Revive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod.Coroutine;
using Resurrect = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod.Coroutine;
using RetRevive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.Coroutine;
using PerformRemember = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod.PerformRemember;
using RetStopKill = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.StopKill;
using MedStopKill = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod.StopKill;
using PerformSteal = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod.PerformSteal;
using PerformDeclare = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod.PerformAbility;
using PerformSidekick = TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod.PerformAbility;
using PerformShift = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod.PerformShift;
using PerformConvert = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod.PerformConvert;
using Mine = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MinerMod.PerformMine;
using Recruit = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod.PerformRecruit;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class RPCHandling
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRPC
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte readByte, readByte1;

                switch ((CustomRPC)callId)
                {
                    case CustomRPC.SetRole:
                        var player = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadByte())
                        {
                            case 0:
                                _ = new Mayor(player);
                                break;
                            case 1:
                                _ = new Sheriff(player);
                                break;
                            case 2:
                                _ = new Inspector(player);
                                break;
                            case 3:
                                _ = new Vigilante(player);
                                break;
                            case 4:
                                _ = new Engineer(player);
                                break;
                            case 5:
                                _ = new Swapper(player);
                                break;
                            case 6:
                                _ = new Drunkard(player);
                                break;
                            case 7:
                                _ = new TimeLord(player);
                                break;
                            case 8:
                                _ = new Medic(player);
                                break;
                            case 9:
                                _ = new Agent(player);
                                break;
                            case 10:
                                _ = new Altruist(player);
                                break;
                            case 11:
                                _ = new Veteran(player);
                                break;
                            case 12:
                                _ = new Tracker(player);
                                break;
                            case 13:
                                _ = new Transporter(player);
                                break;
                            case 14:
                                _ = new Medium(player);
                                break;
                            case 15:
                                _ = new Coroner(player);
                                break;
                            case 16:
                                _ = new Operative(player);
                                break;
                            case 17:
                                _ = new Detective(player);
                                break;
                            case 18:
                                _ = new Escort(player);
                                break;
                            case 19:
                                _ = new Shifter(player);
                                break;
                            case 20:
                                _ = new Crewmate(player);
                                break;
                            case 21:
                                _ = new VampireHunter(player);
                                break;
                            case 22:
                                _ = new Jester(player);
                                break;
                            case 23:
                                _ = new Amnesiac(player);
                                break;
                            case 24:
                                _ = new Executioner(player);
                                break;
                            case 25:
                                _ = new Survivor(player);
                                break;
                            case 26:
                                _ = new GuardianAngel(player);
                                break;
                            case 27:
                                _ = new Glitch(player);
                                break;
                            case 28:
                                _ = new Murderer(player);
                                break;
                            case 29:
                                _ = new Cryomaniac(player);
                                break;
                            case 30:
                                _ = new Werewolf(player);
                                break;
                            case 31:
                                _ = new Arsonist(player);
                                break;
                            case 32:
                                _ = new Jackal(player);
                                break;
                            case 33:
                                _ = new Plaguebearer(player);
                                break;
                            case 34:
                                _ = new Pestilence(player);
                                break;
                            case 35:
                                _ = new SerialKiller(player);
                                break;
                            case 36:
                                _ = new Juggernaut(player);
                                break;
                            case 37:
                                _ = new Cannibal(player);
                                break;
                            case 38:
                                _ = new Thief(player);
                                break;
                            case 39:
                                _ = new Dracula(player);
                                break;
                            case 40:
                                _ = new Troll(player);
                                break;
                            case 41:
                                _ = new Undertaker(player);
                                break;
                            case 42:
                                _ = new Morphling(player);
                                break;
                            case 43:
                                _ = new Blackmailer(player);
                                break;
                            case 44:
                                _ = new Miner(player);
                                break;
                            case 45:
                                _ = new Teleporter(player);
                                break;
                            case 46:
                                _ = new Wraith(player);
                                break;
                            case 47:
                                _ = new Consort(player);
                                break;
                            case 48:
                                _ = new Janitor(player);
                                break;
                            case 49:
                                _ = new Camouflager(player);
                                break;
                            case 50:
                                _ = new Grenadier(player);
                                break;
                            case 51:
                                _ = new Poisoner(player);
                                break;
                            case 52:
                                _ = new Impostor(player);
                                break;
                            case 53:
                                _ = new Consigliere(player);
                                break;
                            case 54:
                                _ = new Disguiser(player);
                                break;
                            case 55:
                                _ = new TimeMaster(player);
                                break;
                            case 56:
                                _ = new Godfather(player);
                                break;
                            case 57:
                                _ = new Anarchist(player);
                                break;
                            case 58:
                                _ = new Shapeshifter(player);
                                break;
                            case 59:
                                _ = new Gorgon(player);
                                break;
                            case 60:
                                _ = new Framer(player);
                                break;
                            case 61:
                                _ = new Rebel(player);
                                break;
                            case 62:
                                _ = new Concealer(player);
                                break;
                            case 63:
                                _ = new Warper(player);
                                break;
                            case 64:
                                _ = new Bomber(player);
                                break;
                            case 65:
                                _ = new Chameleon(player);
                                break;
                            case 66:
                                _ = new Guesser(player);
                                break;
                            case 67:
                                _ = new Whisperer(player);
                                break;
                            case 68:
                                _ = new Retributionist(player);
                                break;
                            case 69:
                                _ = new Actor(player);
                                break;
                            case 70:
                                _ = new BountyHunter(player);
                                break;
                            case 71:
                                _ = new Mystic(player);
                                break;
                            case 72:
                                _ = new Seer(player);
                                break;
                            case 73:
                                _ = new Necromancer(player);
                                break;
                            case 74:
                                _ = new Beamer(player);
                                break;
                            case 75:
                                _ = new Ambusher(player);
                                break;
                            case 76:
                                _ = new Crusader(player);
                                break;
                        }

                        break;

                    case CustomRPC.SetModifier:
                        var player2 = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadInt32())
                        {
                            case 0:
                                _ = new Diseased(player2);
                                break;
                            case 1:
                                _ = new Bait(player2);
                                break;
                            case 2:
                                _ = new Dwarf(player2);
                                break;
                            case 3:
                                _ = new VIP(player2);
                                break;
                            case 4:
                                _ = new Shy(player2);
                                break;
                            case 5:
                                _ = new Giant(player2);
                                break;
                            case 6:
                                _ = new Drunk(player2);
                                break;
                            case 7:
                                _ = new Flincher(player2);
                                break;
                            case 8:
                                _ = new Coward(player2);
                                break;
                            case 9:
                                _ = new Volatile(player2);
                                break;
                            case 10:
                                _ = new Professional(player2);
                                break;
                            case 11:
                                _ = new Indomitable(player2);
                                break;
                        }

                        break;

                    case CustomRPC.SetAbility:
                        var player3 = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadInt32())
                        {
                            case 0:
                            case 11:
                            case 12:
                            case 13:
                                _ = new Assassin(player3);
                                break;
                            case 1:
                                _ = new Snitch(player3);
                                break;
                            case 2:
                                _ = new Insider(player3);
                                break;
                            case 4:
                                _ = new Multitasker(player3);
                                break;
                            case 5:
                                _ = new Radar(player3);
                                break;
                            case 6:
                                _ = new Tiebreaker(player3);
                                break;
                            case 7:
                                _ = new Torch(player3);
                                break;
                            case 8:
                                _ = new Underdog(player3);
                                break;
                            case 9:
                                _ = new Tunneler(player3);
                                break;
                            case 10:
                                _ = new Ruthless(player3);
                                break;
                            case 14:
                                _ = new Ninja(player3);
                                break;
                            case 15:
                                _ = new ButtonBarry(player3);
                                break;
                        }

                        break;

                    case CustomRPC.SetObjectifier:
                        var player4 = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadInt32())
                        {
                            case 2:
                                _ = new Fanatic(player4);
                                break;
                            case 3:
                                _ = new Corrupted(player4);
                                break;
                            case 4:
                                _ = new Overlord(player4);
                                break;
                            case 5:
                                _ = new Allied(player4);
                                break;
                            case 6:
                                _ = new Traitor(player4);
                                break;
                            case 7:
                                _ = new Taskmaster(player4);
                                break;
                        }

                        break;

                    case CustomRPC.SetCouple:
                        var lover1 = Utils.PlayerById(reader.ReadByte());
                        var lover2 = Utils.PlayerById(reader.ReadByte());
                        var objectifierLover1 = new Lovers(lover1);
                        var objectifierLover2 = new Lovers(lover2);
                        objectifierLover1.OtherLover = lover2;
                        objectifierLover2.OtherLover = lover1;
                        break;

                    case CustomRPC.SetDuo:
                        var rival1 = Utils.PlayerById(reader.ReadByte());
                        var rival2 = Utils.PlayerById(reader.ReadByte());
                        var objectifierRival1 = new Rivals(rival1);
                        var objectifierRival2 = new Rivals(rival2);
                        objectifierRival1.OtherRival = rival2;
                        objectifierRival2.OtherRival = rival1;
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
                                SetTraitor.TurnTraitor(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case TurnRPC.TurnFanatic:
                                var attacker = Utils.PlayerById(reader.ReadByte());
                                var fanatic = Utils.PlayerById(reader.ReadByte());
                                var attackerRole = Role.GetRole(attacker);
                                Fanatic.TurnFanatic(fanatic, attackerRole.Faction);
                                break;
                        }

                        break;

                    case CustomRPC.SetRevealer:
                        readByte = reader.ReadByte();
                        SetRevealer.WillBeRevealer = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.SetBanshee:
                        readByte = reader.ReadByte();
                        SetBanshee.WillBeBanshee = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.SetGhoul:
                        readByte = reader.ReadByte();
                        SetGhoul.WillBeGhoul = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.RevealerDied:
                        var revealer = Utils.PlayerById(reader.ReadByte());
                        var former = Role.GetRole(revealer);
                        var revealerRole = new Revealer(revealer);
                        revealer.RegenTask();
                        revealerRole.FormerRole = former;
                        revealerRole.RoleUpdate(former);
                        revealer.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetRevealer.RemoveTasks(revealer);
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        break;

                    case CustomRPC.PhantomDied:
                        var phantom = Utils.PlayerById(reader.ReadByte());
                        var phantomFormer = Role.GetRole(phantom);
                        var phantomRole = new Phantom(phantom);
                        phantom.RegenTask();
                        phantomRole.RoleUpdate(phantomFormer);
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        break;

                    case CustomRPC.BansheeDied:
                        var banshee = Utils.PlayerById(reader.ReadByte());
                        var bansheeFormer = Role.GetRole(banshee);
                        var bansheeRole = new Banshee(banshee);
                        banshee.RegenTask();
                        bansheeRole.RoleUpdate(bansheeFormer);
                        banshee.gameObject.layer = LayerMask.NameToLayer("Players");
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        break;

                    case CustomRPC.GhoulDied:
                        var ghoul = Utils.PlayerById(reader.ReadByte());
                        var ghoulFormer = Role.GetRole(ghoul);
                        var ghoulRole = new Ghoul(ghoul);
                        ghoul.RegenTask();
                        ghoulRole.RoleUpdate(ghoulFormer);
                        ghoul.gameObject.layer = LayerMask.NameToLayer("Players");
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
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
                        readByte = reader.ReadByte();

                        if (Utils.PlayerById(medicId).Is(RoleEnum.Retributionist))
                            RetStopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        else if (Utils.PlayerById(medicId).Is(RoleEnum.Medium))
                            MedStopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);

                        break;

                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.TargetPlayer = exeTarget;
                        break;

                    case CustomRPC.SetGuessTarget:
                        var guess = Utils.PlayerById(reader.ReadByte());
                        var guessTarget = Utils.PlayerById(reader.ReadByte());
                        var guessRole = Role.GetRole<Guesser>(guess);
                        guessRole.TargetPlayer = guessTarget;
                        break;

                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.TargetPlayer = gaTarget;
                        break;

                    case CustomRPC.SetBHTarget:
                        var bh = Utils.PlayerById(reader.ReadByte());
                        var bhTarget = Utils.PlayerById(reader.ReadByte());
                        var bhRole = Role.GetRole<BountyHunter>(bh);
                        bhRole.TargetPlayer = bhTarget;
                        break;

                    case CustomRPC.SetActPretendList:
                        var act = Utils.PlayerById(reader.ReadByte());
                        var targetRoles = reader.ReadByte();
                        var actRole = Role.GetRole<Actor>(act);
                        actRole.PretendRoles = (InspectorResults)targetRoles;
                        break;

                    case CustomRPC.SetGoodRecruit:
                        var jackal = Utils.PlayerById(reader.ReadByte());
                        var goodRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole = Role.GetRole<Jackal>(jackal);
                        jackalRole.GoodRecruit = goodRecruit;
                        jackalRole.Recruited.Add(goodRecruit.PlayerId);
                        Role.GetRole(goodRecruit).SubFaction = SubFaction.Cabal;
                        Role.GetRole(goodRecruit).IsRecruit = true;
                        break;

                    case CustomRPC.SetBackupRecruit:
                        var jackal3 = Utils.PlayerById(reader.ReadByte());
                        var backRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole3 = Role.GetRole<Jackal>(jackal3);
                        Recruit.Recruit(jackalRole3, backRecruit);
                        break;

                    case CustomRPC.SetEvilRecruit:
                        var jackal2 = Utils.PlayerById(reader.ReadByte());
                        var evilRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole2 = Role.GetRole<Jackal>(jackal2);
                        jackalRole2.EvilRecruit = evilRecruit;
                        jackalRole2.Recruited.Add(evilRecruit.PlayerId);
                        Role.GetRole(evilRecruit).SubFaction = SubFaction.Cabal;
                        Role.GetRole(evilRecruit).IsRecruit = true;
                        break;

                    case CustomRPC.SendChat:
                        string report = reader.ReadString();
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        break;

                    case CustomRPC.CatchRevealer:
                        Role.GetRole<Revealer>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;

                    case CustomRPC.RemoveAllBodies:
                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                            body.gameObject.Destroy();

                        break;

                    case CustomRPC.SetSpawnAirship:
                        SpawnInMinigamePatch.SpawnPoints = reader.ReadBytesAndSize().ToList();
                        break;

                    case CustomRPC.DoorSyncToilet:
                        int Id = reader.ReadInt32();
                        PlainDoor DoorToSync = Object.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == Id);
                        DoorToSync.SetDoorway(true);
                        break;

                    case CustomRPC.SyncPlateform:
                        bool isLeft = reader.ReadBoolean();
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

                    case CustomRPC.VersionHandshake:
                        var major = reader.ReadByte();
                        var minor = reader.ReadByte();
                        var patch = reader.ReadByte();
                        var timer = reader.ReadSingle();

                        if (!AmongUsClient.Instance.AmHost && timer >= 0f)
                            GameStartManagerPatch.timer = timer;

                        var versionOwnerId = reader.ReadPackedInt32();
                        var revision = byte.MaxValue;
                        Guid guid;

                        if (reader.Length - reader.Position >= 17)
                        {
                            // Enough bytes left to read
                            revision = reader.ReadByte();
                            // GUID
                            var gbytes = reader.ReadBytes(16);
                            guid = new Guid(gbytes);
                        }
                        else
                            guid = new Guid(new byte[16]);

                        Utils.VersionHandshake(major, minor, patch, revision == byte.MaxValue ? -1 : revision, guid, versionOwnerId);
                        break;

                    case CustomRPC.SetAlliedFaction:
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

                    case CustomRPC.SubmergedFixOxygen:
                        SubmergedCompatibility.RepairOxygen();
                        break;

                    case CustomRPC.ChaosDrive:
                        Role.SyndicateHasChaosDrive = reader.ReadBoolean();
                        break;

                    case CustomRPC.SetPos:
                        var setplayer = Utils.PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), setplayer.transform.position.z);
                        break;

                    case CustomRPC.SyncCustomSettings:
                        CustomOptions.RPC.ReceiveRPC(reader);
                        break;

                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
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
                            RandomMap.AdjustSettings(readByte);

                        break;

                    case CustomRPC.Action:
                        var id6 = reader.ReadByte();

                        switch ((ActionsRPC)id6)
                        {
                            case ActionsRPC.FreezeDouse:
                                var cryomaniac = Utils.PlayerById(reader.ReadByte());
                                var freezedouseTarget = Utils.PlayerById(reader.ReadByte());
                                var cryomaniacRole = Role.GetRole<Cryomaniac>(cryomaniac);
                                cryomaniacRole.DousedPlayers.Add(freezedouseTarget.PlayerId);
                                cryomaniacRole.LastDoused = DateTime.UtcNow;
                                break;

                            case ActionsRPC.RevealerFinished:
                                var revealer2 = Utils.PlayerById(reader.ReadByte());
                                var revealerRole2 = Role.GetRole<Revealer>(revealer2);
                                revealerRole2.CompletedTasks = true;
                                break;

                            case ActionsRPC.AllFreeze:
                                var theCryomaniac = Utils.PlayerById(reader.ReadByte());
                                var theCryomaniacRole = Role.GetRole<Cryomaniac>(theCryomaniac);
                                theCryomaniacRole.FreezeUsed = true;
                                break;

                            case ActionsRPC.FadeBody:
                                var id = reader.ReadByte();

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == id)
                                        Coroutines.Start(Utils.FadeBody(body));
                                }

                                break;

                            case ActionsRPC.EngineerFix:
                                var engineer = Utils.PlayerById(reader.ReadByte());
                                Role.GetRole<Engineer>(engineer).UsesLeft--;
                                break;

                            case ActionsRPC.FixLights:
                                var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                                lights.ActualSwitches = lights.ExpectedSwitches;
                                break;

                            case ActionsRPC.SetExtraVotes:
                                var mayor = Utils.PlayerById(reader.ReadByte());
                                var mayorRole = Role.GetRole<Mayor>(mayor);
                                mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();

                                if (!mayor.Is(RoleEnum.Mayor))
                                    mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;

                                break;

                            case ActionsRPC.SetSwaps:
                                var readSByte = reader.ReadSByte();
                                SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                                var readSByte2 = reader.ReadSByte();
                                SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                                Utils.LogSomething("Bytes received - " + readSByte + " - " + readSByte2);
                                break;

                            case ActionsRPC.Remember:
                                var amnesiac = Utils.PlayerById(reader.ReadByte());
                                var other = Utils.PlayerById(reader.ReadByte());
                                PerformRemember.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                                break;

                            case ActionsRPC.Steal:
                                var thief = Utils.PlayerById(reader.ReadByte());
                                var other4 = Utils.PlayerById(reader.ReadByte());
                                PerformSteal.Steal(Role.GetRole<Thief>(thief), other4);
                                break;

                            case ActionsRPC.Whisper:
                                var whisp = Utils.PlayerById(reader.ReadByte());
                                var whispRole = Role.GetRole<Whisperer>(whisp);
                                whispRole.Whisper();
                                break;

                            case ActionsRPC.RetributionistAction:
                                var retId = reader.ReadByte();

                                switch ((RetributionistActionsRPC)retId)
                                {
                                    case RetributionistActionsRPC.RetributionistReviveSet:
                                        var ret = Utils.PlayerById(reader.ReadByte());
                                        var id8 = reader.ReadByte();

                                        if (id8 == sbyte.MaxValue)
                                            break;

                                        var revived = Utils.PlayerById(reader.ReadByte());
                                        var retRole = Role.GetRole<Retributionist>(ret);

                                        if (revived != null)
                                            retRole.Revived = revived;

                                        break;

                                    case RetributionistActionsRPC.RetributionistRevive:
                                        var ret2 = Utils.PlayerById(reader.ReadByte());
                                        var revived2 = Utils.PlayerById(reader.ReadByte());
                                        var retRole2 = Role.GetRole<Retributionist>(ret2);

                                        if (retRole2.RevivedRole.RoleType != RoleEnum.Altruist)
                                            break;

                                        StartRevive.Revive(retRole2, revived2);
                                        break;

                                    case RetributionistActionsRPC.EngineerFix:
                                        var ret3 = Utils.PlayerById(reader.ReadByte());
                                        var retRole3 = Role.GetRole<Retributionist>(ret3);

                                        if (retRole3.RevivedRole.RoleType != RoleEnum.Engineer)
                                            break;

                                        retRole3.FixUsesLeft--;
                                        break;

                                    case RetributionistActionsRPC.Protect:
                                        var ret5 = Utils.PlayerById(reader.ReadByte());
                                        var shielded = Utils.PlayerById(reader.ReadByte());
                                        var retRole5 = Role.GetRole<Retributionist>(ret5);

                                        if (retRole5.RevivedRole.RoleType != RoleEnum.Medic)
                                            break;

                                        retRole5.ShieldedPlayer = shielded;
                                        retRole5.UsedAbility = true;
                                        break;

                                    case RetributionistActionsRPC.Interrogate:
                                        var ret6 = Utils.PlayerById(reader.ReadByte());
                                        var interrogated = Utils.PlayerById(reader.ReadByte());
                                        var retRole6 = Role.GetRole<Retributionist>(ret6);

                                        if (retRole6.RevivedRole.RoleType != RoleEnum.Sheriff)
                                            break;

                                        retRole6.Interrogated.Add(interrogated.PlayerId);
                                        retRole6.LastInterrogated = DateTime.UtcNow;
                                        break;

                                    case RetributionistActionsRPC.Alert:
                                        var ret7 = Utils.PlayerById(reader.ReadByte());
                                        var retRole7 = Role.GetRole<Retributionist>(ret7);

                                        if (retRole7.RevivedRole.RoleType != RoleEnum.Veteran)
                                            break;

                                        retRole7.AlertTimeRemaining = CustomGameOptions.AlertDuration;
                                        retRole7.Alert();
                                        break;

                                    case RetributionistActionsRPC.AltruistRevive:
                                        var ret8 = Utils.PlayerById(reader.ReadByte());
                                        var retRole8 = Role.GetRole<Retributionist>(ret8);

                                        if (retRole8.RevivedRole.RoleType != RoleEnum.Altruist)
                                            break;

                                        readByte = reader.ReadByte();

                                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                        {
                                            if (body.ParentId == readByte)
                                            {
                                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                                    Utils.Flash(retRole8.Color, "You are being revived!");

                                                Coroutines.Start(RetRevive.RetributionistRevive(body, retRole8));
                                            }
                                        }

                                        break;

                                    case RetributionistActionsRPC.Swoop:
                                        var ret9 = Utils.PlayerById(reader.ReadByte());
                                        var retRole9 = Role.GetRole<Retributionist>(ret9);

                                        if (retRole9.RevivedRole.RoleType != RoleEnum.Chameleon)
                                            break;

                                        retRole9.SwoopTimeRemaining = CustomGameOptions.SwoopDuration;
                                        retRole9.Invis();
                                        break;

                                    case RetributionistActionsRPC.Mediate:
                                        var mediatedPlayer2 = Utils.PlayerById(reader.ReadByte());
                                        var retRole10 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));

                                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer2.PlayerId)
                                            break;

                                        retRole10.AddMediatePlayer(mediatedPlayer2.PlayerId);
                                        break;
                                }

                                break;

                            case ActionsRPC.GodfatherAction:
                                var gfId = reader.ReadByte();

                                switch ((GodfatherActionsRPC)gfId)
                                {
                                    case GodfatherActionsRPC.Declare:
                                        var gf = Utils.PlayerById(reader.ReadByte());
                                        var maf = Utils.PlayerById(reader.ReadByte());
                                        PerformDeclare.Declare(Role.GetRole<Godfather>(gf), maf);
                                        break;

                                    case GodfatherActionsRPC.Teleport:
                                        var gf2 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole2 = Role.GetRole<Godfather>(gf2);
                                        gfRole2.TeleportPoint = reader.ReadVector2();
                                        Godfather.Teleport(gf2);
                                        break;

                                    case GodfatherActionsRPC.Morph:
                                        var gf3 = Utils.PlayerById(reader.ReadByte());
                                        var morphTarget2 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole3 = Role.GetRole<Godfather>(gf3);
                                        gfRole3.MorphTimeRemaining = CustomGameOptions.MorphlingDuration;
                                        gfRole3.MorphedPlayer = morphTarget2;
                                        gfRole3.Morph();
                                        break;

                                    case GodfatherActionsRPC.Disguise:
                                        var gf4 = Utils.PlayerById(reader.ReadByte());
                                        var disguiseTarget2 = Utils.PlayerById(reader.ReadByte());
                                        var disguiserForm2 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole4 = Role.GetRole<Godfather>(gf4);
                                        gfRole4.DisguiserTimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                        gfRole4.DisguiserTimeRemaining = CustomGameOptions.DisguiseDuration;
                                        gfRole4.MeasuredPlayer = disguiseTarget2;
                                        gfRole4.ClosestPlayer = disguiserForm2;
                                        gfRole4.Delay();
                                        break;

                                    case GodfatherActionsRPC.Blackmail:
                                        var gfRole5 = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                                        gfRole5.BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                                        break;

                                    case GodfatherActionsRPC.Mine:
                                        var ventId2 = reader.ReadInt32();
                                        var gf6 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole6 = Role.GetRole<Godfather>(gf6);
                                        var pos2 = reader.ReadVector2();
                                        var zAxis2 = reader.ReadSingle();
                                        PerformDeclare.SpawnVent(ventId2, gfRole6, pos2, zAxis2);
                                        break;

                                    case GodfatherActionsRPC.TimeFreeze:
                                        var gf7 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole7 = Role.GetRole<Godfather>(gf7);
                                        Freeze.FreezeFunctions.FreezeAll();
                                        gfRole7.FreezeTimeRemaining = CustomGameOptions.FreezeDuration;
                                        gfRole7.TimeFreeze();
                                        break;

                                    case GodfatherActionsRPC.Invis:
                                        var gf8 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole8 = Role.GetRole<Godfather>(gf8);
                                        gfRole8.InvisTimeRemaining = CustomGameOptions.InvisDuration;
                                        gfRole8.Invis();
                                        break;

                                    case GodfatherActionsRPC.Drag:
                                        var gf9 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole9 = Role.GetRole<Godfather>(gf9);

                                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                        {
                                            if (body.ParentId == reader.ReadByte())
                                                gfRole9.CurrentlyDragging = body;
                                        }

                                        break;

                                    case GodfatherActionsRPC.Drop:
                                        readByte1 = reader.ReadByte();
                                        var v2_1 = reader.ReadVector2();
                                        var v2z_1 = reader.ReadSingle();
                                        var gf10 = Utils.PlayerById(readByte1);
                                        var gfRole10 = Role.GetRole<Godfather>(gf10);
                                        var body2_1 = gfRole10.CurrentlyDragging;
                                        gfRole10.CurrentlyDragging = null;
                                        body2_1.transform.position = new Vector3(v2_1.x, v2_1.y, v2z_1);
                                        break;

                                    case GodfatherActionsRPC.Camouflage:
                                        var gf11 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole11 = Role.GetRole<Godfather>(gf11);
                                        Utils.Camouflage();
                                        gfRole11.CamoTimeRemaining = CustomGameOptions.CamouflagerDuration;
                                        gfRole11.Camouflage();
                                        break;

                                    case GodfatherActionsRPC.FlashGrenade:
                                        var gf12 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole12 = Role.GetRole<Godfather>(gf12);
                                        gfRole12.FlashTimeRemaining = CustomGameOptions.GrenadeDuration;
                                        gfRole12.Flash();
                                        break;
                                }

                                break;

                            case ActionsRPC.RebelAction:
                                var rebId = reader.ReadByte();

                                switch ((RebelActionsRPC)rebId)
                                {
                                    case RebelActionsRPC.Sidekick:
                                        var reb = Utils.PlayerById(reader.ReadByte());
                                        var side = Utils.PlayerById(reader.ReadByte());
                                        PerformSidekick.Sidekick(Role.GetRole<Rebel>(reb), side);
                                        break;

                                    case RebelActionsRPC.Poison:
                                        var reb1 = Utils.PlayerById(reader.ReadByte());
                                        var poisoned2 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole1 = Role.GetRole<Rebel>(reb1);
                                        rebRole1.PoisonedPlayer = poisoned2;
                                        break;

                                    case RebelActionsRPC.Warp:
                                        var teleports2 = reader.ReadByte();
                                        var coordinates2 = new Dictionary<byte, Vector2>();

                                        for (var i = 0; i < teleports2; i++)
                                        {
                                            var playerId = reader.ReadByte();
                                            var location = reader.ReadVector2();
                                            coordinates2.Add(playerId, location);
                                        }

                                        Rebel.WarpPlayersToCoordinates(coordinates2);
                                        break;

                                    case RebelActionsRPC.Conceal:
                                        var reb2 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole2 = Role.GetRole<Rebel>(reb2);
                                        rebRole2.ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                                        rebRole2.Conceal();
                                        Utils.Conceal();
                                        break;

                                    case RebelActionsRPC.Shapeshift:
                                        var reb3 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole3 = Role.GetRole<Rebel>(reb3);
                                        rebRole3.ShapeshiftTimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                        rebRole3.Shapeshift();
                                        break;

                                    case RebelActionsRPC.Confuse:
                                        var reb5 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole5 = Role.GetRole<Rebel>(reb5);
                                        rebRole5.ConfuseTimeRemaining = CustomGameOptions.ConfuseDuration;
                                        rebRole5.Confuse();
                                        Reverse.ConfuseFunctions.ConfuseAll();
                                        break;
                                }

                                break;

                            case ActionsRPC.Shift:
                                var shifter = Utils.PlayerById(reader.ReadByte());
                                var other2 = Utils.PlayerById(reader.ReadByte());
                                PerformShift.Shift(Role.GetRole<Shifter>(shifter), other2);
                                break;

                            case ActionsRPC.Convert:
                                var drac = Utils.PlayerById(reader.ReadByte());
                                var other3 = Utils.PlayerById(reader.ReadByte());
                                PerformConvert.Convert(Role.GetRole<Dracula>(drac), other3);
                                break;

                            case ActionsRPC.Teleport:
                                var teleporter = Utils.PlayerById(reader.ReadByte());
                                var teleporterRole = Role.GetRole<Teleporter>(teleporter);
                                teleporterRole.TeleportPoint = reader.ReadVector2();
                                Teleporter.Teleport(teleporter);
                                break;

                            case ActionsRPC.Rewind:
                                var TimeLordPlayer = Utils.PlayerById(reader.ReadByte());
                                var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                                StartStop.StartRewind(TimeLordRole);
                                break;

                            case ActionsRPC.Protect:
                                var medic = Utils.PlayerById(reader.ReadByte());
                                var shield = Utils.PlayerById(reader.ReadByte());
                                Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                                break;

                            case ActionsRPC.RewindRevive:
                                RecordRewind.ReviveBody(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case ActionsRPC.BypassKill:
                                var killer = Utils.PlayerById(reader.ReadByte());
                                var target = Utils.PlayerById(reader.ReadByte());
                                var lunge = reader.ReadBoolean();
                                Utils.MurderPlayer(killer, target, lunge);
                                break;

                            case ActionsRPC.AssassinKill:
                                var toDie = Utils.PlayerById(reader.ReadByte());
                                var guessString = reader.ReadString();
                                var assassin = Ability.GetAbilityValue<Assassin>(AbilityEnum.Assassin);
                                AssassinKill.MurderPlayer(assassin, toDie, guessString);
                                break;

                            case ActionsRPC.GuesserKill:
                                var toDie2 = Utils.PlayerById(reader.ReadByte());
                                var guessString2 = reader.ReadString();
                                var assassin2 = Role.GetRoleValue<Guesser>(RoleEnum.Guesser);
                                GuesserKill.MurderPlayer(assassin2, toDie2, guessString2);
                                break;

                            case ActionsRPC.Interrogate:
                                var sheriff = Utils.PlayerById(reader.ReadByte());
                                var otherPlayer = Utils.PlayerById(reader.ReadByte());
                                var sheriffRole = Role.GetRole<Sheriff>(sheriff);
                                sheriffRole.Interrogated.Add(otherPlayer.PlayerId);
                                sheriffRole.LastInterrogated = DateTime.UtcNow;
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
                                bansheeRole2.Scream();

                                foreach (var player8 in PlayerControl.AllPlayerControls)
                                {
                                    if (!player8.Data.IsDead && !player8.Data.Disconnected && !player8.Is(Faction.Syndicate))
                                    {
                                        var targetRole4 = Role.GetRole(player8);
                                        targetRole4.IsBlocked = !targetRole4.RoleBlockImmune;
                                        bansheeRole2.Blocked.Add(player8.PlayerId);
                                    }
                                }

                                break;

                            case ActionsRPC.Mark:
                                var ghoul2 = Utils.PlayerById(reader.ReadByte());
                                var marked = Utils.PlayerById(reader.ReadByte());
                                var ghoulRole2 = Role.GetRole<Ghoul>(ghoul2);
                                ghoulRole2.MarkedPlayer = marked;
                                break;

                            case ActionsRPC.Disguise:
                                var disguiser = Utils.PlayerById(reader.ReadByte());
                                var disguiseTarget = Utils.PlayerById(reader.ReadByte());
                                var disguiserForm = Utils.PlayerById(reader.ReadByte());
                                var disguiseRole = Role.GetRole<Disguiser>(disguiser);
                                disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                                disguiseRole.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                disguiseRole.MeasuredPlayer = disguiseTarget;
                                disguiseRole.ClosestPlayer = disguiserForm;
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
                                var minerRole = Role.GetRole<Miner>(miner);
                                var pos = reader.ReadVector2();
                                var zAxis = reader.ReadSingle();
                                Mine.SpawnVent(ventId, minerRole, pos, zAxis);
                                break;

                            case ActionsRPC.TimeFreeze:
                                var tm = Utils.PlayerById(reader.ReadByte());
                                var tmRole = Role.GetRole<TimeMaster>(tm);
                                Freeze.FreezeFunctions.FreezeAll();
                                tmRole.TimeRemaining = CustomGameOptions.FreezeDuration;
                                tmRole.TimeFreeze();
                                break;

                            case ActionsRPC.Confuse:
                                var drunk = Utils.PlayerById(reader.ReadByte());
                                var drunkRole = Role.GetRole<Drunkard>(drunk);
                                drunkRole.TimeRemaining = CustomGameOptions.ConfuseDuration;
                                drunkRole.Confuse();
                                Reverse.ConfuseFunctions.ConfuseAll();
                                break;

                            case ActionsRPC.Invis:
                                var wraith = Utils.PlayerById(reader.ReadByte());
                                var wraithRole = Role.GetRole<Wraith>(wraith);
                                wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                                wraithRole.Invis();
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
                                ambRole.TimeRemaining = CustomGameOptions.VestDuration;
                                ambRole.AmbushedPlayer = ambushed;
                                ambRole.Ambush();
                                break;

                            case ActionsRPC.GAProtect:
                                var ga2 = Utils.PlayerById(reader.ReadByte());
                                var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                                ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                                ga2Role.Protect();
                                break;

                            case ActionsRPC.Transport:
                                Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                                break;

                            case ActionsRPC.Beam:
                                Coroutines.Start(Beamer.BeamPlayers(reader.ReadByte(), reader.ReadByte()));
                                break;

                            case ActionsRPC.SetUntransportable:
                                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                                    Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                                break;

                            case ActionsRPC.SetUnbeamable:
                                if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer))
                                    Role.GetRole<Beamer>(PlayerControl.LocalPlayer).UnbeamablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                                break;

                            case ActionsRPC.Mediate:
                                var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                                var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));

                                if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId)
                                    break;

                                medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                                break;

                            case ActionsRPC.FlashGrenade:
                                var grenadier = Utils.PlayerById(reader.ReadByte());
                                var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                                grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                                grenadierRole.Flash();
                                break;

                            case ActionsRPC.Maul:
                                var ww = Utils.PlayerById(reader.ReadByte());
                                var wwRole = Role.GetRole<Werewolf>(ww);
                                wwRole.Maul(wwRole.Player);
                                break;

                            case ActionsRPC.Douse:
                                var arsonist = Utils.PlayerById(reader.ReadByte());
                                var douseTarget = Utils.PlayerById(reader.ReadByte());
                                var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                                arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                                arsonistRole.LastDoused = DateTime.UtcNow;
                                arsonistRole.LastIgnited = DateTime.UtcNow;
                                break;

                            case ActionsRPC.Ignite:
                                var theArsonist = Utils.PlayerById(reader.ReadByte());
                                var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                                theArsonistRole.Ignite();
                                theArsonistRole.LastIgnited = DateTime.UtcNow;
                                break;

                            case ActionsRPC.Infect:
                                var pb = Utils.PlayerById(reader.ReadByte());
                                var infected = reader.ReadByte();
                                Role.GetRole<Plaguebearer>(pb).InfectedPlayers.Add(infected);
                                break;

                            case ActionsRPC.AltruistRevive:
                                readByte1 = reader.ReadByte();
                                var altruistPlayer = Utils.PlayerById(readByte1);
                                var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                                readByte = reader.ReadByte();

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == readByte)
                                    {
                                        if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                            Utils.Flash(altruistRole.Color, "You are being revived!");

                                        Coroutines.Start(Revive.AltruistRevive(body, altruistRole));
                                    }
                                }

                                break;

                            case ActionsRPC.NecromancerResurrect:
                                readByte1 = reader.ReadByte();
                                var necroPlayer = Utils.PlayerById(readByte1);
                                var necroRole = Role.GetRole<Necromancer>(necroPlayer);
                                readByte = reader.ReadByte();

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == readByte)
                                    {
                                        if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                            Utils.Flash(necroRole.Color, "You are being resurrected!");

                                        Coroutines.Start(Resurrect.NecromancerResurrect(body, necroRole));
                                    }
                                }

                                break;

                            case ActionsRPC.Warp:
                                var teleports = reader.ReadByte();
                                var coordinates = new Dictionary<byte, Vector2>();

                                for (var i = 0; i < teleports; i++)
                                {
                                    var playerId = reader.ReadByte();
                                    var location = reader.ReadVector2();
                                    coordinates.Add(playerId, location);
                                }

                                Warper.WarpPlayersToCoordinates(coordinates);
                                break;

                            case ActionsRPC.Detonate:
                                var bomber = Utils.PlayerById(reader.ReadByte());
                                var bomberRole = Role.GetRole<Bomber>(bomber);
                                bomberRole.Bombs.DetonateBombs(bomber.name);
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

                                    if (GameManager.Instance.CheckTaskCompletion())
                                        return;

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
                                var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                                var dienerBodies = Object.FindObjectsOfType<DeadBody>();

                                foreach (var body in dienerBodies)
                                {
                                    if (body.ParentId == reader.ReadByte())
                                        dienerRole.CurrentlyDragging = body;
                                }

                                break;

                            case ActionsRPC.Drop:
                                readByte1 = reader.ReadByte();
                                var v2 = reader.ReadVector2();
                                var v2z = reader.ReadSingle();
                                var dienerPlayer2 = Utils.PlayerById(readByte1);
                                var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                                var body2 = dienerRole2.CurrentlyDragging;
                                dienerRole2.CurrentlyDragging = null;
                                body2.transform.position = new Vector3(v2.x, v2.y, v2z);
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
                                var targetRole = Role.GetRole(blocked2);
                                targetRole.IsBlocked = !targetRole.RoleBlockImmune;
                                escortRole.BlockTarget = blocked2;
                                escortRole.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                                escortRole.Block();
                                break;

                            case ActionsRPC.GlitchRoleblock:
                                var glitch = Utils.PlayerById(reader.ReadByte());
                                var blocked3 = Utils.PlayerById(reader.ReadByte());
                                var glitchRole = Role.GetRole<Glitch>(glitch);
                                var targetRole3 = Role.GetRole(blocked3);
                                targetRole3.IsBlocked = !targetRole3.RoleBlockImmune;
                                glitchRole.HackTarget = blocked3;
                                glitchRole.TimeRemaining = CustomGameOptions.HackDuration;
                                glitchRole.Hack();
                                break;

                            case ActionsRPC.ConsRoleblock:
                                var consort = Utils.PlayerById(reader.ReadByte());
                                var blocked = Utils.PlayerById(reader.ReadByte());
                                var consortRole = Role.GetRole<Consort>(consort);
                                var targetRole2 = Role.GetRole(blocked);
                                targetRole2.IsBlocked = !targetRole2.RoleBlockImmune;
                                consortRole.BlockTarget = blocked;
                                consortRole.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                                consortRole.Block();
                                break;

                            case ActionsRPC.SetMimic:
                                var glitch3 = Utils.PlayerById(reader.ReadByte());
                                var mimicTarget = Utils.PlayerById(reader.ReadByte());
                                var glitchRole3 = Role.GetRole<Glitch>(glitch3);
                                glitchRole3.TimeRemaining2 = CustomGameOptions.MimicDuration;
                                glitchRole3.MimicTarget = mimicTarget;
                                glitchRole3.Mimic();
                                break;

                            case ActionsRPC.Conceal:
                                var concealer = Utils.PlayerById(reader.ReadByte());
                                var concealerRole = Role.GetRole<Concealer>(concealer);
                                concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                                concealerRole.Conceal();
                                Utils.Conceal();
                                break;

                            case ActionsRPC.Shapeshift:
                                var ss = Utils.PlayerById(reader.ReadByte());
                                var ssRole = Role.GetRole<Shapeshifter>(ss);
                                ssRole.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                ssRole.Shapeshift();
                                break;

                            case ActionsRPC.Gaze:
                                var gorg = Utils.PlayerById(reader.ReadByte());
                                var stoned = reader.ReadByte();
                                var gorgon = Role.GetRole<Gorgon>(gorg);
                                gorgon.Gazed.Add(stoned);
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
                                Role.NobodyWins = true;
                                Objectifier.NobodyWins = true;
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
                                        if (!role.Player.Data.Disconnected && role.NotDefective)
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

                                if (reader.ReadBoolean())
                                {
                                    foreach (Corrupted corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted).Cast<Corrupted>())
                                        corr.Winner = true;
                                }
                                else
                                {
                                    Objectifier.GetObjectifier(Utils.PlayerById(reader.ReadByte())).Winner = true;
                                }

                                break;

                            case WinLoseRPC.LoveWin:
                                Objectifier.LoveWins = true;
                                var lover = Objectifier.GetObjectifier<Lovers>(Utils.PlayerById(reader.ReadByte()));
                                lover.Winner = true;
                                Objectifier.GetObjectifier(lover.OtherLover).Winner = true;
                                break;

                            case WinLoseRPC.OverlordWin:
                                Objectifier.OverlordWins = true;

                                foreach (Overlord ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord).Cast<Overlord>())
                                    ov.Winner = true;

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

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RPCSetRole
        {
            public static void Postfix()
            {
                Utils.LogSomething("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                RoleGen.ResetEverything();

                Utils.LogSomething("Cleared Variables");

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Start, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);

                RoleGen.BeginRoleGen(infected.ToList());

                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MaxReportDistance = CustomGameOptions.ReportDistance;
            }
        }
    }
}