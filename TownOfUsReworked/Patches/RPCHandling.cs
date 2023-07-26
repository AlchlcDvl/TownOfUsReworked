namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class RPCHandling
    {
        public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (callId != 255)
                return;

            var rpc = (CustomRPC)reader.ReadByte();

            switch (rpc)
            {
                case CustomRPC.Test:
                    LogSomething("Recieved RPC!");
                    break;

                case CustomRPC.Misc:
                    var misc = (MiscRPC)reader.ReadByte();

                    switch (misc)
                    {
                        case MiscRPC.SetLayer:
                            RoleGen.SetLayer(reader.ReadInt32(), reader.ReadPlayer(), (PlayerLayerEnum)reader.ReadByte());
                            break;

                        case MiscRPC.SetRevealer:
                            SetPostmortals.WillBeRevealers.Add(reader.ReadPlayer());
                            break;

                        case MiscRPC.SetPhantom:
                            SetPostmortals.WillBePhantoms.Add(reader.ReadPlayer());
                            break;

                        case MiscRPC.SetBanshee:
                            SetPostmortals.WillBeBanshees.Add(reader.ReadPlayer());
                            break;

                        case MiscRPC.SetGhoul:
                            SetPostmortals.WillBeGhouls.Add(reader.ReadPlayer());
                            break;

                        case MiscRPC.Whisper:
                            if (!HUD.Chat)
                                break;

                            var whisperer = reader.ReadPlayer();
                            var whispered = reader.ReadPlayer();
                            var message = reader.ReadString();

                            if (whispered == CustomPlayer.Local)
                                HUD.Chat.AddChat(CustomPlayer.Local, $"{whisperer.name} whispers to you:{message}");
                            else if ((CustomPlayer.Local.Is(RoleEnum.Blackmailer) && CustomGameOptions.WhispersNotPrivate) || DeadSeeEverything ||
                                (CustomPlayer.Local.Is(RoleEnum.Silencer) && CustomGameOptions.WhispersNotPrivateSilencer))
                            {
                                HUD.Chat.AddChat(CustomPlayer.Local, $"{whisperer.name} is whispering to {whispered.name}: {message}");
                            }
                            else if (CustomGameOptions.WhispersAnnouncement)
                                HUD.Chat.AddChat(CustomPlayer.Local, $"{whisperer.name} is whispering to {whispered.name}.");

                            if (!ChatCommand.BubbleModifications.ContainsKey(HUD.Chat.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                                ChatCommand.BubbleModifications.Add(HUD.Chat.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Whispers", UColor.blue));

                            break;

                        case MiscRPC.CatchPhantom:
                            var phan = reader.ReadPlayer();
                            Role.GetRole<Phantom>(phan).Caught = true;
                            phan.Exiled();
                            break;

                        case MiscRPC.CatchBanshee:
                            var ban = reader.ReadPlayer();
                            Role.GetRole<Banshee>(ban).Caught = true;
                            ban.Exiled();
                            break;

                        case MiscRPC.CatchGhoul:
                            var gho = reader.ReadPlayer();
                            Role.GetRole<Ghoul>(gho).Caught = true;
                            gho.Exiled();
                            break;

                        case MiscRPC.Start:
                            RoleGen.ResetEverything();
                            break;

                        case MiscRPC.AttemptSound:
                            Role.BreakShield(reader.ReadPlayer(), CustomGameOptions.ShieldBreaks);
                            break;

                        case MiscRPC.CatchRevealer:
                            var rev = reader.ReadPlayer();
                            var revRole = Role.GetRole<Revealer>(rev);
                            revRole.Caught = true;
                            revRole.CompletedTasks = false;
                            rev.Exiled();
                            break;

                        case MiscRPC.AddVoteBank:
                            ((Politician)reader.ReadLayer()).VoteBank += reader.ReadInt32();
                            break;

                        case MiscRPC.MeetingStart:
                            foreach (var body in AllBodies)
                                body.gameObject.Destroy();

                            foreach (var player10 in CustomPlayer.AllPlayers)
                                player10.MyPhysics.ResetAnimState();

                            break;

                        case MiscRPC.DoorSyncToilet:
                            UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == reader.ReadInt32())?.SetDoorway(true);
                            break;

                        case MiscRPC.SyncPlateform:
                            CallPlateform.SyncPlateform(reader.ReadBoolean());
                            break;

                        case MiscRPC.CheckMurder:
                            reader.ReadPlayer().CheckMurder(reader.ReadPlayer());
                            break;

                        case MiscRPC.SetColor:
                            reader.ReadPlayer().SetColor(reader.ReadByte());
                            break;

                        case MiscRPC.VersionHandshake:
                            VersionHandshake(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), new(reader.ReadBytesAndSize()), reader.ReadPackedInt32());
                            break;

                        case MiscRPC.SubmergedFixOxygen:
                            RepairOxygen();
                            break;

                        case MiscRPC.FixLights:
                            var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                            lights.ActualSwitches = lights.ExpectedSwitches;
                            break;

                        case MiscRPC.RemoveMeetings:
                            reader.ReadPlayer().RemainingEmergencies = 0;
                            break;

                        case MiscRPC.SetReports:
                            reader.ReadPlayer().MaxReportDistance = CustomGameOptions.ReportDistance;
                            break;

                        case MiscRPC.SetSpawnAirship:
                            SpawnInMinigamePatch.SpawnPoints.Clear();
                            SpawnInMinigamePatch.SpawnPoints.AddRange(reader.ReadBytesAndSize().ToList());
                            break;

                        case MiscRPC.ChaosDrive:
                            Role.DriveHolder = reader.ReadPlayer();
                            Role.SyndicateHasChaosDrive = true;
                            break;

                        case MiscRPC.SetPos:
                            var setplayer = reader.ReadPlayer();

                            if (IsSubmerged)
                                ChangeFloor(setplayer.transform.position.y > -7f);

                            break;

                        case MiscRPC.SyncCustomSettings:
                            ReceiveOptionRPC(reader);
                            CustomOption.SaveSettings("LastUsedSettings");
                            break;

                        case MiscRPC.Notify:
                            ChatCommands.Notify(reader.ReadByte());
                            break;

                        case MiscRPC.SetSettings:
                            var map = reader.ReadByte();
                            TownOfUsReworked.NormalOptions.MapId = map == 255 ? (byte)0 : map;
                            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                            TownOfUsReworked.NormalOptions.CrewLightMod = CustomGameOptions.CrewVision;
                            TownOfUsReworked.NormalOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                            TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                            TownOfUsReworked.NormalOptions.VisualTasks = CustomGameOptions.VisualTasks;
                            TownOfUsReworked.NormalOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                            TownOfUsReworked.NormalOptions.NumImpostors = CustomGameOptions.IntruderCount;
                            TownOfUsReworked.NormalOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)reader.ReadByte();
                            TownOfUsReworked.NormalOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                            TownOfUsReworked.NormalOptions.VotingTime = CustomGameOptions.VotingTime;
                            TownOfUsReworked.NormalOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                            TownOfUsReworked.NormalOptions.KillDistance = CustomGameOptions.InteractionDistance;
                            TownOfUsReworked.NormalOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                            TownOfUsReworked.NormalOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                            TownOfUsReworked.NormalOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                            TownOfUsReworked.NormalOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                            TownOfUsReworked.NormalOptions.MaxPlayers = CustomGameOptions.LobbySize;
                            GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.NormalOptions;

                            foreach (var player in CustomPlayer.AllPlayers)
                                player.MaxReportDistance = CustomGameOptions.ReportDistance;

                            break;

                        case MiscRPC.SetFirstKilled:
                            CachedFirstDead = FirstDead = reader.ReadPlayer();
                            break;

                        default:
                            LogSomething($"Recieved unknown RPC - {nameof(misc)}");
                            break;
                    }

                    break;

                case CustomRPC.Change:
                    var turn = (TurnRPC)reader.ReadByte();

                    switch (turn)
                    {
                        case TurnRPC.TurnFanatic:
                            ((Fanatic)reader.ReadLayer()).TurnFanatic((Faction)reader.ReadByte());
                            break;

                        case TurnRPC.TurnAct:
                            ((Guesser)reader.ReadLayer()).TurnAct((Role)reader.ReadLayer());
                            break;

                        case TurnRPC.TurnVigilante:
                            ((VampireHunter)reader.ReadLayer()).TurnVigilante();
                            break;

                        case TurnRPC.TurnPestilence:
                            ((Plaguebearer)reader.ReadLayer()).TurnPestilence();
                            break;

                        case TurnRPC.TurnTroll:
                            ((BountyHunter)reader.ReadLayer()).TurnTroll();
                            break;

                        case TurnRPC.TurnSurv:
                            ((GuardianAngel)reader.ReadLayer()).TurnSurv();
                            break;

                        case TurnRPC.TurnGodfather:
                            ((Mafioso)reader.ReadLayer()).TurnGodfather();
                            break;

                        case TurnRPC.TurnJest:
                            ((Executioner)reader.ReadLayer()).TurnJest();
                            break;

                        case TurnRPC.TurnTraitorBetrayer:
                            ((Traitor)reader.ReadLayer()).TurnBetrayer();
                            break;

                        case TurnRPC.TurnFanaticBetrayer:
                            ((Fanatic)reader.ReadLayer()).TurnBetrayer();
                            break;

                        case TurnRPC.TurnTraitor:
                            ((Traitor)reader.ReadLayer()).TurnTraitor();
                            break;

                        case TurnRPC.TurnSides:
                            ((Defector)reader.ReadLayer()).TurnSides();
                            break;

                        case TurnRPC.TurnRebel:
                            ((Sidekick)reader.ReadLayer()).TurnRebel();
                            break;

                        case TurnRPC.TurnThief:
                            ((Amnesiac)reader.ReadLayer()).TurnThief();
                            break;

                        case TurnRPC.TurnSheriff:
                            ((Seer)reader.ReadLayer()).TurnSheriff();
                            break;

                        case TurnRPC.TurnSeer:
                            ((Mystic)reader.ReadLayer()).TurnSeer();
                            break;

                        case TurnRPC.TurnRole:
                            ((Actor)reader.ReadLayer()).TurnRole();
                            break;

                        default:
                            LogSomething($"Recieved unknown RPC - {nameof(turn)}");
                            break;
                    }

                    break;

                case CustomRPC.Target:
                    var target = (TargetRPC)reader.ReadByte();

                    switch (target)
                    {
                        case TargetRPC.SetExeTarget:
                            ((Executioner)reader.ReadLayer()).TargetPlayer = reader.ReadPlayer();
                            break;

                        case TargetRPC.SetGuessTarget:
                            ((Guesser)reader.ReadLayer()).TargetPlayer = reader.ReadPlayer();
                            break;

                        case TargetRPC.SetGATarget:
                            ((GuardianAngel)reader.ReadLayer()).TargetPlayer = reader.ReadPlayer();
                            break;

                        case TargetRPC.SetBHTarget:
                            ((BountyHunter)reader.ReadLayer()).TargetPlayer = reader.ReadPlayer();
                            break;

                        case TargetRPC.SetActPretendList:
                            ((Actor)reader.ReadLayer()).TargetRole = (Role)reader.ReadLayer();
                            break;

                        case TargetRPC.SetAlliedFaction:
                            var player6 = reader.ReadPlayer();
                            var alliedRole = Role.GetRole(player6);
                            var ally = Objectifier.GetObjectifier<Allied>(player6);
                            var faction = (Faction)reader.ReadByte();
                            alliedRole.Faction = ally.Side = faction;
                            alliedRole.RoleAlignment = alliedRole.RoleAlignment.GetNewAlignment(faction);

                            if (faction == Faction.Crew)
                            {
                                alliedRole.FactionColor = Colors.Crew;
                                alliedRole.IsCrewAlly = true;
                            }
                            else if (faction == Faction.Intruder)
                            {
                                alliedRole.FactionColor = Colors.Intruder;
                                alliedRole.IsIntAlly = true;
                            }
                            else if (faction == Faction.Syndicate)
                            {
                                alliedRole.FactionColor = Colors.Syndicate;
                                alliedRole.IsSynAlly = true;
                            }

                            break;

                        case TargetRPC.SetCouple:
                            var lover1 = reader.ReadPlayer();
                            var lover2 = reader.ReadPlayer();
                            Objectifier.GetObjectifier<Lovers>(lover1).OtherLover = lover2;
                            Objectifier.GetObjectifier<Lovers>(lover2).OtherLover = lover1;
                            break;

                        case TargetRPC.SetDuo:
                            var rival1 = reader.ReadPlayer();
                            var rival2 = reader.ReadPlayer();
                            Objectifier.GetObjectifier<Rivals>(rival1).OtherRival = rival2;
                            Objectifier.GetObjectifier<Rivals>(rival2).OtherRival = rival1;
                            break;

                        case TargetRPC.SetLinked:
                            var link1 = reader.ReadPlayer();
                            var link2 = reader.ReadPlayer();
                            Objectifier.GetObjectifier<Linked>(link1).OtherLink = link2;
                            Objectifier.GetObjectifier<Linked>(link2).OtherLink = link1;
                            break;

                        default:
                            LogSomething($"Recieved unknown RPC - {nameof(target)}");
                            break;
                    }

                    break;

                case CustomRPC.Action:
                    var action = (ActionsRPC)reader.ReadByte();

                    switch (action)
                    {
                        case ActionsRPC.RetributionistAction:
                            var retAction = (RetributionistActionsRPC)reader.ReadByte();

                            switch (retAction)
                            {
                                case RetributionistActionsRPC.RetributionistRevive:
                                    var retRole2 = (Retributionist)reader.ReadLayer();
                                    retRole2.Revived = reader.ReadPlayer();
                                    retRole2.RevivedRole = retRole2.Revived == null ? null : (retRole2.Revived.Is(RoleEnum.Revealer) ? Role.GetRole<Revealer>(retRole2.Revived).FormerRole :
                                        Role.GetRole(retRole2.Revived));
                                    break;

                                case RetributionistActionsRPC.Transport:
                                    var retRole3 = (Retributionist)reader.ReadLayer();
                                    retRole3.TransportPlayer1 = reader.ReadPlayer();
                                    retRole3.TransportPlayer2 = reader.ReadPlayer();
                                    Coroutines.Start(retRole3.TransportPlayers());
                                    break;

                                case RetributionistActionsRPC.Protect:
                                    ((Retributionist)reader.ReadLayer()).ShieldedPlayer = reader.ReadPlayer();
                                    break;

                                case RetributionistActionsRPC.Alert:
                                    var retRole7 = (Retributionist)reader.ReadLayer();
                                    retRole7.TimeRemaining = CustomGameOptions.AlertDuration;
                                    retRole7.Alert();
                                    break;

                                case RetributionistActionsRPC.AltruistRevive:
                                    var retRole8 = (Retributionist)reader.ReadLayer();
                                    retRole8.RevivingBody = reader.ReadBody();
                                    retRole8.TimeRemaining = CustomGameOptions.AltReviveDuration;
                                    retRole8.Revive();
                                    break;

                                case RetributionistActionsRPC.Swoop:
                                    var retRole9 = (Retributionist)reader.ReadLayer();
                                    retRole9.TimeRemaining = CustomGameOptions.SwoopDuration;
                                    retRole9.Invis();
                                    break;

                                case RetributionistActionsRPC.Mediate:
                                    var retId2 = reader.ReadByte();
                                    var ret = Role.GetRole<Medium>(PlayerById(retId2));
                                    var playerid2 = reader.ReadByte();
                                    ret.MediatedPlayers.Add(playerid2);

                                    if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.IsDead && CustomGameOptions.ShowMediumToDead == ShowMediumToDead.AllDead))
                                        Role.LocalRole.DeadArrows.Add(retId2, new(CustomPlayer.Local, Colors.Retributionist));

                                    break;

                                case RetributionistActionsRPC.EscRoleblock:
                                    var retRole1 = (Retributionist)reader.ReadLayer();
                                    var blocked4 = reader.ReadPlayer();
                                    retRole1.BlockTarget = reader.ReadPlayer();
                                    retRole1.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                                    retRole1.Block();
                                    break;

                                default:
                                    LogSomething($"Recieved unknown RPC - {nameof(retAction)}");
                                    break;
                            }

                            break;

                        case ActionsRPC.GodfatherAction:
                            var gfAction = (GodfatherActionsRPC)reader.ReadByte();

                            switch (gfAction)
                            {
                                case GodfatherActionsRPC.Morph:
                                    var gfRole3 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole3.TimeRemaining = CustomGameOptions.MorphlingDuration;
                                    gfRole3.MorphedPlayer = reader.ReadPlayer();
                                    gfRole3.Morph();
                                    break;

                                case GodfatherActionsRPC.Disguise:
                                    var gfRole4 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole4.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                    gfRole4.TimeRemaining = CustomGameOptions.DisguiseDuration;
                                    gfRole4.DisguisedPlayer = reader.ReadPlayer();
                                    gfRole4.CopiedPlayer = reader.ReadPlayer();
                                    gfRole4.DisgDelay();
                                    break;

                                case GodfatherActionsRPC.ConsRoleblock:
                                    var gfRole5 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole5.BlockTarget = reader.ReadPlayer();
                                    gfRole5.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                                    gfRole5.Block();
                                    break;

                                case GodfatherActionsRPC.Blackmail:
                                    ((PromotedGodfather)reader.ReadLayer()).BlackmailedPlayer = reader.ReadPlayer();
                                    break;

                                case GodfatherActionsRPC.Invis:
                                    var gfRole8 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole8.TimeRemaining = CustomGameOptions.InvisDuration;
                                    gfRole8.Invis();
                                    break;

                                case GodfatherActionsRPC.Drag:
                                    var gf = reader.ReadPlayer();
                                    var gfRole7 = Role.GetRole<PromotedGodfather>(gf);
                                    var dragged2 = reader.ReadBody();
                                    gfRole7.CurrentlyDragging = dragged2;
                                    var drag3 = dragged2.gameObject.AddComponent<DragBehaviour>();
                                    drag3.Source = gf;
                                    drag3.Dragged = dragged2;
                                    break;

                                case GodfatherActionsRPC.Camouflage:
                                    var gfRole11 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole11.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                                    gfRole11.Camouflage();
                                    break;

                                case GodfatherActionsRPC.FlashGrenade:
                                    var gfRole12 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole12.TimeRemaining = CustomGameOptions.GrenadeDuration;
                                    gfRole12.Flash();
                                    break;

                                case GodfatherActionsRPC.SetBomb:
                                    var gfRole13 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole13.TimeRemaining = CustomGameOptions.EnforceDuration;
                                    gfRole13.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                                    gfRole13.BombedPlayer = reader.ReadPlayer();
                                    gfRole13.BombDelay();
                                    break;

                                case GodfatherActionsRPC.Ambush:
                                    var gfRole14 = (PromotedGodfather)reader.ReadLayer();
                                    gfRole14.TimeRemaining = CustomGameOptions.AmbushDuration;
                                    gfRole14.AmbushedPlayer = reader.ReadPlayer();
                                    gfRole14.Ambush();
                                    break;

                                default:
                                    LogSomething($"Recieved unknown RPC - {nameof(gfAction)}");
                                    break;
                            }

                            break;

                        case ActionsRPC.RebelAction:
                            var rebAction = (RebelActionsRPC)reader.ReadByte();

                            switch (rebAction)
                            {
                                case RebelActionsRPC.Poison:
                                    var rebRole1 = (PromotedRebel)reader.ReadLayer();
                                    rebRole1.PoisonedPlayer = reader.ReadPlayer();
                                    rebRole1.TimeRemaining = CustomGameOptions.PoisonDuration;
                                    rebRole1.Poison();
                                    break;

                                case RebelActionsRPC.Conceal:
                                    var rebRole2 = (PromotedRebel)reader.ReadLayer();

                                    if (!rebRole2.HoldsDrive)
                                        rebRole2.ConcealedPlayer = reader.ReadPlayer();

                                    rebRole2.TimeRemaining = CustomGameOptions.ConcealDuration;
                                    rebRole2.Conceal();
                                    break;

                                case RebelActionsRPC.Shapeshift:
                                    var rebRole3 = (PromotedRebel)reader.ReadLayer();

                                    if (!rebRole3.HoldsDrive)
                                    {
                                        rebRole3.ShapeshiftPlayer1 = reader.ReadPlayer();
                                        rebRole3.ShapeshiftPlayer2 = reader.ReadPlayer();
                                    }

                                    rebRole3.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                    rebRole3.Shapeshift();
                                    break;

                                case RebelActionsRPC.Warp:
                                    var rebRole4 = Role.GetRole<Warper>(reader.ReadPlayer());
                                    rebRole4.WarpPlayer1 = reader.ReadPlayer();
                                    rebRole4.WarpPlayer2 = reader.ReadPlayer();
                                    Coroutines.Start(rebRole4.WarpPlayers());
                                    break;

                                case RebelActionsRPC.Frame:
                                    ((PromotedRebel)reader.ReadLayer()).Framed.Add(reader.ReadByte());
                                    break;

                                case RebelActionsRPC.Crusade:
                                    var rebRole7 = (PromotedRebel)reader.ReadLayer();
                                    rebRole7.CrusadedPlayer = reader.ReadPlayer();
                                    rebRole7.TimeRemaining = CustomGameOptions.CrusadeDuration;
                                    rebRole7.Crusade();
                                    break;

                                case RebelActionsRPC.Spell:
                                    ((PromotedRebel)reader.ReadLayer()).Spelled.Add(reader.ReadByte());
                                    break;

                                case RebelActionsRPC.Confuse:
                                    var rebRole6 = (PromotedRebel)reader.ReadLayer();

                                    if (!rebRole6.HoldsDrive)
                                        rebRole6.ConfusedPlayer = reader.ReadPlayer();

                                    rebRole6.TimeRemaining = CustomGameOptions.ConfuseDuration;
                                    rebRole6.Confuse();
                                    break;

                                case RebelActionsRPC.TimeControl:
                                    var rebRole5 = (PromotedRebel)reader.ReadLayer();
                                    rebRole5.TimeRemaining = CustomGameOptions.TimeControlDuration;
                                    rebRole5.Control();
                                    break;

                                case RebelActionsRPC.Silence:
                                    ((PromotedRebel)reader.ReadLayer()).SilencedPlayer = reader.ReadPlayer();
                                    break;

                                default:
                                    LogSomething($"Recieved unknown RPC - {nameof(rebAction)}");
                                    break;
                            }

                            break;

                        case ActionsRPC.FreezeDouse:
                            ((Cryomaniac)reader.ReadLayer()).Doused.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.FadeBody:
                            Coroutines.Start(FadeBody(reader.ReadBody()));
                            break;

                        case ActionsRPC.SetExtraVotes:
                            var polRole = (Politician)reader.ReadLayer();
                            polRole.ExtraVotes = reader.ReadBytesAndSize().ToList();
                            polRole.VoteBank -= polRole.ExtraVotes.Count;
                            break;

                        case ActionsRPC.SetSwaps:
                            var swapper = (Swapper)reader.ReadLayer();
                            swapper.Swap1 = reader.ReadVoteArea();
                            swapper.Swap2 = reader.ReadVoteArea();
                            break;

                        case ActionsRPC.Remember:
                            Amnesiac.Remember((Amnesiac)reader.ReadLayer(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.Steal:
                            Thief.Steal((Thief)reader.ReadLayer(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.Shift:
                            Shifter.Shift((Shifter)reader.ReadLayer(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.Convert:
                            RoleGen.Convert(reader.ReadByte(), reader.ReadByte(), (SubFaction)reader.ReadByte(), reader.ReadBoolean());
                            break;

                        case ActionsRPC.Teleport:
                            Teleport(reader.ReadPlayer(), reader.ReadVector3());
                            break;

                        case ActionsRPC.Sidekick:
                            Rebel.Sidekick((Rebel)reader.ReadLayer(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.Frame:
                            ((Framer)reader.ReadLayer()).Framed.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Declare:
                            Godfather.Declare((Godfather)reader.ReadLayer(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.Protect:
                            ((Medic)reader.ReadLayer()).ShieldedPlayer = reader.ReadPlayer();
                            break;

                        case ActionsRPC.BypassKill:
                            MurderPlayer(reader.ReadPlayer(), reader.ReadPlayer(), (DeathReasonEnum)reader.ReadByte(), reader.ReadBoolean());
                            break;

                        case ActionsRPC.AssassinKill:
                            ((Assassin)reader.ReadLayer()).MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.GuesserKill:
                            ((Guesser)reader.ReadLayer()).MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.ThiefKill:
                            ((Thief)reader.ReadLayer()).MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
                            break;

                        case ActionsRPC.ForceKill:
                            var victim = reader.ReadPlayer();
                            var success = reader.ReadBoolean();
                            Role.GetRoles<Enforcer>(RoleEnum.Enforcer).Where(x => x.BombedPlayer == victim).ToList().ForEach(x =>
                            {
                                x.BombSuccessful = success;
                                x.TimeRemaining = 0;
                                x.Unboom();
                            });

                            break;

                        case ActionsRPC.SetBomb:
                            var enfRole = (Enforcer)reader.ReadLayer();
                            enfRole.TimeRemaining = CustomGameOptions.EnforceDuration;
                            enfRole.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                            enfRole.BombedPlayer = reader.ReadPlayer();
                            enfRole.Delay();
                            break;

                        case ActionsRPC.Morph:
                            var morphRole = (Morphling)reader.ReadLayer();
                            morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                            morphRole.MorphedPlayer = reader.ReadPlayer();
                            morphRole.Morph();
                            break;

                        case ActionsRPC.Scream:
                            var bansheeRole2 = (Banshee)reader.ReadLayer();
                            bansheeRole2.TimeRemaining = CustomGameOptions.ScreamDuration;
                            bansheeRole2.Scream();
                            break;

                        case ActionsRPC.Mark:
                            ((Ghoul)reader.ReadLayer()).MarkedPlayer = reader.ReadPlayer();
                            break;

                        case ActionsRPC.Disguise:
                            var disguiseRole = (Disguiser)reader.ReadLayer();
                            disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                            disguiseRole.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                            disguiseRole.CopiedPlayer = reader.ReadPlayer();
                            disguiseRole.DisguisedPlayer = reader.ReadPlayer();
                            disguiseRole.Delay();
                            break;

                        case ActionsRPC.Poison:
                            var poisonerRole = (Poisoner)reader.ReadLayer();
                            poisonerRole.TimeRemaining = CustomGameOptions.PoisonDuration;
                            poisonerRole.PoisonedPlayer = reader.ReadPlayer();
                            poisonerRole.Poison();
                            break;

                        case ActionsRPC.Blackmail:
                            ((Blackmailer)reader.ReadLayer()).BlackmailedPlayer = reader.ReadPlayer();
                            break;

                        case ActionsRPC.Silence:
                            ((Silencer)reader.ReadLayer()).SilencedPlayer = reader.ReadPlayer();
                            break;

                        case ActionsRPC.Mine:
                            AddVent((Role)reader.ReadLayer(), reader.ReadVector3());
                            break;

                        case ActionsRPC.Invis:
                            var wraithRole = (Wraith)reader.ReadLayer();
                            wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                            wraithRole.Invis();
                            break;

                        case ActionsRPC.Alert:
                            var veteranRole = (Veteran)reader.ReadLayer();
                            veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                            veteranRole.Alert();
                            break;

                        case ActionsRPC.Vest:
                            var survRole = (Survivor)reader.ReadLayer();
                            survRole.TimeRemaining = CustomGameOptions.VestDuration;
                            survRole.Vest();
                            break;

                        case ActionsRPC.Ambush:
                            var ambRole = (Ambusher)reader.ReadLayer();
                            ambRole.TimeRemaining = CustomGameOptions.AmbushDuration;
                            ambRole.AmbushedPlayer = reader.ReadPlayer();
                            ambRole.Ambush();
                            break;

                        case ActionsRPC.Crusade:
                            var crusRole = (Crusader)reader.ReadLayer();
                            crusRole.TimeRemaining = CustomGameOptions.CrusadeDuration;
                            crusRole.CrusadedPlayer = reader.ReadPlayer();
                            crusRole.Crusade();
                            break;

                        case ActionsRPC.GAProtect:
                            var ga2Role = (GuardianAngel)reader.ReadLayer();
                            ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                            ga2Role.Protect();
                            break;

                        case ActionsRPC.Transport:
                            var transRole = (Transporter)reader.ReadLayer();
                            transRole.TransportPlayer1 = reader.ReadPlayer();
                            transRole.TransportPlayer2 = reader.ReadPlayer();
                            Coroutines.Start(transRole.TransportPlayers());
                            break;

                        case ActionsRPC.SetUninteractable:
                            try
                            {
                                if (CustomPlayer.Local.Is(RoleEnum.Transporter))
                                    ((Transporter)reader.ReadLayer()).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                                else if (CustomPlayer.Local.Is(RoleEnum.Warper))
                                    ((Warper)reader.ReadLayer()).UnwarpablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                                else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                                    ((Retributionist)reader.ReadLayer()).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                            }
                            catch (Exception e)
                            {
                                LogSomething(e);
                            }

                            break;

                        case ActionsRPC.Warp:
                            var warpRole = (Warper)reader.ReadLayer();
                            warpRole.WarpPlayer1 = reader.ReadPlayer();
                            warpRole.WarpPlayer2 = reader.ReadPlayer();
                            Coroutines.Start(warpRole.WarpPlayers());
                            break;

                        case ActionsRPC.Mediate:
                            var medid = reader.ReadByte();
                            var med = Role.GetRole<Medium>(PlayerById(medid));
                            var playerid = reader.ReadByte();
                            med.MediatedPlayers.Add(playerid);

                            if (CustomPlayer.Local.PlayerId == playerid || (CustomPlayer.LocalCustom.IsDead && CustomGameOptions.ShowMediumToDead == ShowMediumToDead.AllDead))
                                Role.LocalRole.DeadArrows.Add(medid, new(CustomPlayer.Local, Colors.Medium));

                            break;

                        case ActionsRPC.FlashGrenade:
                            var grenadierRole = (Grenadier)reader.ReadLayer();
                            grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                            grenadierRole.Flash();
                            break;

                        case ActionsRPC.Douse:
                            ((Arsonist)reader.ReadLayer()).Doused.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Infect:
                            ((Plaguebearer)reader.ReadLayer()).Infected.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.AltruistRevive:
                            var altruistRole = (Altruist)reader.ReadLayer();
                            altruistRole.RevivingBody = reader.ReadBody();
                            altruistRole.TimeRemaining = CustomGameOptions.AltReviveDuration;
                            altruistRole.Revive();
                            break;

                        case ActionsRPC.NecromancerResurrect:
                            var necroRole = (Necromancer)reader.ReadLayer();
                            necroRole.ResurrectingBody = reader.ReadBody();
                            necroRole.TimeRemaining = CustomGameOptions.NecroResurrectDuration;
                            necroRole.Resurrect();
                            break;

                        case ActionsRPC.WarpAll:
                            var teleports = reader.ReadByte();
                            var coordinates = new Dictionary<byte, Vector2>();

                            for (var i = 0; i < teleports; i++)
                                coordinates.Add(reader.ReadByte(), reader.ReadVector2());

                            WarpPlayersToCoordinates(coordinates);
                            break;

                        case ActionsRPC.Swoop:
                            var chameleonRole = (Chameleon)reader.ReadLayer();
                            chameleonRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                            chameleonRole.Invis();
                            break;

                        case ActionsRPC.BarryButton:
                            if (AmongUsClient.Instance.AmHost)
                            {
                                var buttonBarry = reader.ReadPlayer();
                                MeetingRoomManager.Instance.reporter = buttonBarry;
                                MeetingRoomManager.Instance.target = null;
                                AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                                HUD.OpenMeetingRoom(buttonBarry);
                                buttonBarry.RpcStartMeeting(null);
                            }

                            break;

                        case ActionsRPC.BaitReport:
                            reader.ReadPlayer().ReportDeadBody(reader.ReadPlayer().Data);
                            break;

                        case ActionsRPC.Drag:
                            var jani = reader.ReadPlayer();
                            var janiRole = Role.GetRole<Janitor>(jani);
                            var dragged = reader.ReadBody();
                            janiRole.CurrentlyDragging = dragged;
                            var drag = dragged.gameObject.AddComponent<DragBehaviour>();
                            drag.Source = jani;
                            drag.Dragged = dragged;
                            break;

                        case ActionsRPC.Drop:
                            var dragged1 = reader.ReadBody();
                            dragged1.gameObject.GetComponent<DragBehaviour>().Destroy();
                            ((Janitor)Role.AllRoles.Find(x => x.RoleType == RoleEnum.Janitor && ((Janitor)x).CurrentlyDragging == dragged1)).CurrentlyDragging = null;
                            ((PromotedGodfather)Role.AllRoles.Find(x => x.RoleType == RoleEnum.PromotedGodfather && ((PromotedGodfather)x).CurrentlyDragging == dragged1)).CurrentlyDragging
                                = null;
                            break;

                        case ActionsRPC.Camouflage:
                            var camouflagerRole = (Camouflager)reader.ReadLayer();
                            camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                            camouflagerRole.Camouflage();
                            break;

                        case ActionsRPC.EscRoleblock:
                            var escortRole = (Escort)reader.ReadLayer();
                            escortRole.BlockTarget = reader.ReadPlayer();
                            escortRole.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                            escortRole.Block();
                            break;

                        case ActionsRPC.GlitchRoleblock:
                            var glitchRole = (Glitch)reader.ReadLayer();
                            glitchRole.HackTarget = reader.ReadPlayer();
                            glitchRole.TimeRemaining = CustomGameOptions.HackDuration;
                            glitchRole.Hack();
                            break;

                        case ActionsRPC.ConsRoleblock:
                            var consortRole = (Consort)reader.ReadLayer();
                            consortRole.BlockTarget = reader.ReadPlayer();
                            consortRole.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                            consortRole.Block();
                            break;

                        case ActionsRPC.Mimic:
                            var glitchRole4 = (Glitch)reader.ReadLayer();
                            glitchRole4.MimicTarget = reader.ReadPlayer();
                            glitchRole4.TimeRemaining2 = CustomGameOptions.MimicDuration;
                            glitchRole4.Mimic();
                            break;

                        case ActionsRPC.Conceal:
                            var concealerRole = (Concealer)reader.ReadLayer();

                            if (!concealerRole.HoldsDrive)
                                concealerRole.ConcealedPlayer = reader.ReadPlayer();

                            concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                            concealerRole.Conceal();
                            break;

                        case ActionsRPC.Shapeshift:
                            var ssRole = (Shapeshifter)reader.ReadLayer();

                            if (!ssRole.HoldsDrive)
                            {
                                ssRole.ShapeshiftPlayer1 = reader.ReadPlayer();
                                ssRole.ShapeshiftPlayer2 = reader.ReadPlayer();
                            }

                            ssRole.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                            ssRole.Shapeshift();
                            break;

                        case ActionsRPC.Burn:
                            var arsoRole = (Arsonist)reader.ReadLayer();

                            foreach (var body in AllBodies)
                            {
                                if (arsoRole.Doused.Contains(body.ParentId))
                                {
                                    Coroutines.Start(FadeBody(body));
                                    _ = new Ash(body.TruePosition);
                                }
                            }

                            break;

                        case ActionsRPC.MayorReveal:
                            var mayor = reader.ReadPlayer();
                            var mayorRole = Role.GetRole<Mayor>(mayor);
                            mayorRole.Revealed = true;
                            Flash(Colors.Mayor);
                            Role.BreakShield(mayor, true);
                            break;

                        case ActionsRPC.DictatorReveal:
                            var dictator = reader.ReadPlayer();
                            var dictatorRole = Role.GetRole<Dictator>(dictator);
                            dictatorRole.Revealed = true;
                            Flash(Colors.Dictator);
                            Role.BreakShield(dictator, true);
                            break;

                        case ActionsRPC.SetExiles:
                            var dictRole = (Dictator)reader.ReadLayer();
                            dictRole.ToDie = reader.ReadBoolean();
                            dictRole.ToBeEjected = reader.ReadBytesAndSize().ToList();
                            break;

                        case ActionsRPC.Spell:
                            ((Spellslinger)reader.ReadLayer()).Spelled.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Knight:
                            ((Monarch)reader.ReadLayer()).ToBeKnighted.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Confuse:
                            var drunkRole = (Drunkard)reader.ReadLayer();

                            if (!drunkRole.HoldsDrive)
                                drunkRole.ConfusedPlayer = reader.ReadPlayer();

                            drunkRole.TimeRemaining = CustomGameOptions.ConfuseDuration;
                            drunkRole.Confuse();
                            break;

                        case ActionsRPC.TimeControl:
                            var tkRole = (TimeKeeper)reader.ReadLayer();
                            tkRole.TimeRemaining = CustomGameOptions.TimeControlDuration;
                            tkRole.Control();
                            break;

                        case ActionsRPC.RequestHit:
                            var bh = reader.ReadPlayer();
                            var request = reader.ReadPlayer();
                            Role.GetRole<BountyHunter>(bh).RequestingPlayer = request;
                            Role.GetRole(request).Requesting = true;
                            Role.GetRole(request).Requestor = bh;
                            break;

                        case ActionsRPC.PlaceHit:
                            var requestor = reader.ReadPlayer();
                            Role.GetRole<BountyHunter>(Role.GetRole(requestor).Requestor).TentativeTarget = reader.ReadPlayer();
                            Role.GetRole(requestor).Requesting = false;
                            Role.GetRole(requestor).Requestor = null;
                            break;

                        default:
                            LogSomething($"Recieved unknown RPC - {nameof(action)}");
                            break;
                    }

                    break;

                case CustomRPC.WinLose:
                    var winlose = (WinLoseRPC)reader.ReadByte();

                    switch (winlose)
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
                            var nkRole = (Role)reader.ReadLayer();

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

                            if (winlose == WinLoseRPC.SameNKWins)
                            {
                                foreach (var role in Role.GetRoles(nkRole.RoleType))
                                {
                                    if (!role.Disconnected && role.Faithful)
                                        role.Winner = true;
                                }
                            }

                            nkRole.Winner = true;
                            break;

                        case WinLoseRPC.InfectorsWin:
                            Role.InfectorsWin = true;
                            break;

                        case WinLoseRPC.JesterWin:
                            ((Jester)reader.ReadLayer()).VotedOut = true;
                            break;

                        case WinLoseRPC.CannibalWin:
                            ((Cannibal)reader.ReadLayer()).Eaten = true;
                            break;

                        case WinLoseRPC.ExecutionerWin:
                            ((Executioner)reader.ReadLayer()).TargetVotedOut = true;
                            break;

                        case WinLoseRPC.BountyHunterWin:
                            ((BountyHunter)reader.ReadLayer()).TargetKilled = true;
                            break;

                        case WinLoseRPC.TrollWin:
                            ((Troll)reader.ReadLayer()).Killed = true;
                            break;

                        case WinLoseRPC.ActorWin:
                            ((Actor)reader.ReadLayer()).Guessed = true;
                            break;

                        case WinLoseRPC.GuesserWin:
                            ((Guesser)reader.ReadLayer()).TargetGuessed = true;
                            break;

                        case WinLoseRPC.CorruptedWin:
                            Objectifier.CorruptedWins = true;

                            if (CustomGameOptions.AllCorruptedWin)
                            {
                                foreach (var corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                                    corr.Winner = true;
                            }

                            reader.ReadLayer().Winner = true;
                            break;

                        case WinLoseRPC.LoveWin:
                            Objectifier.LoveWins = true;
                            var lover = (Lovers)reader.ReadLayer();
                            lover.Winner = true;
                            Objectifier.GetObjectifier(lover.OtherLover).Winner = true;
                            break;

                        case WinLoseRPC.OverlordWin:
                            Objectifier.OverlordWins = true;

                            foreach (var ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
                                ov.Winner = true;

                            break;

                        case WinLoseRPC.MafiaWins:
                            Objectifier.MafiaWins = true;
                            break;

                        case WinLoseRPC.TaskmasterWin:
                            Objectifier.TaskmasterWins = true;
                            reader.ReadLayer().Winner = true;
                            break;

                        case WinLoseRPC.RivalWin:
                            Objectifier.RivalWins = true;
                            reader.ReadLayer().Winner = true;
                            break;

                        case WinLoseRPC.PhantomWin:
                            Role.PhantomWins = true;
                            var phantom3 = (Phantom)reader.ReadLayer();
                            phantom3.CompletedTasks = true;
                            phantom3.Winner = true;
                            break;

                        default:
                            LogSomething($"Recieved unknown RPC - {nameof(winlose)}");
                            break;
                    }

                    break;

                default:
                    LogSomething($"Recieved unknown RPC - {nameof(rpc)}");
                    break;
            }
        }
    }
}