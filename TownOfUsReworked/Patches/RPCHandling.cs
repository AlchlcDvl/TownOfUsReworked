namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class RPCHandling
{
    public static void Postfix(ref byte callId, ref MessageReader reader)
    {
        if (callId != 254)
            return;

        var rpc = (CustomRPC)reader.ReadByte();

        switch (rpc)
        {
            case CustomRPC.Test:
                LogMessage("Received RPC!");
                Run(HUD.Chat, "<color=#FF00FFFF>⚠ TEST ⚠</color>", "Received RPC!");
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
                            Run(HUD.Chat, "<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} whispers to you: {message}");
                        else if ((CustomPlayer.Local.Is(LayerEnum.Blackmailer) && CustomGameOptions.WhispersNotPrivate) || DeadSeeEverything || (CustomPlayer.Local.Is(LayerEnum.Silencer)
                            && CustomGameOptions.WhispersNotPrivateSilencer))
                        {
                            Run(HUD.Chat, "<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} is whispering to {whispered.name} : {message}");
                        }
                        else if (CustomGameOptions.WhispersAnnouncement)
                            Run(HUD.Chat, "<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} is whispering to {whispered.name}.");

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

                    case MiscRPC.SyncPureCrew:
                        RoleGen.PureCrew = reader.ReadPlayer();
                        break;

                    case MiscRPC.AttemptSound:
                        Role.BreakShield(reader.ReadPlayer(), CustomGameOptions.ShieldBreaks);
                        break;

                    case MiscRPC.BastionBomb:
                        Role.BastionBomb(reader.ReadVent(), CustomGameOptions.BombRemovedOnKill);
                        break;

                    case MiscRPC.CatchRevealer:
                        var rev = reader.ReadPlayer();
                        var revRole = Role.GetRole<Revealer>(rev);
                        revRole.Caught = true;
                        revRole.CompletedTasks = false;
                        rev.Exiled();
                        break;

                    case MiscRPC.MeetingStart:
                        AllBodies.ForEach(x => x?.gameObject?.Destroy());
                        CustomPlayer.AllPlayers.ForEach(x => x?.MyPhysics?.ResetAnimState());
                        break;

                    case MiscRPC.DoorSyncToilet:
                        var id2 = reader.ReadInt32();
                        UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == id2)?.SetDoorway(true);
                        break;

                    /*case MiscRPC.SyncPlatform:
                        Role.SyncPlatform();
                        break;*/

                    case MiscRPC.SetColor:
                        var player = reader.ReadPlayer();
                        player.SetColor(reader.ReadByte());
                        UpdateNames.ColorNames[player.PlayerId] = player.Data.ColorName.Replace("(", "").Replace(")", "");
                        break;

                    case MiscRPC.SyncSummary:
                        SaveText("Summary", reader.ReadString());
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

                    case MiscRPC.SetSpawnAirship:
                        BetterAirship.SpawnPoints.Clear();
                        BetterAirship.SpawnPoints.AddRange(reader.ReadByteList());
                        break;

                    case MiscRPC.ChaosDrive:
                        Role.DriveHolder = reader.ReadPlayer();
                        Role.SyndicateHasChaosDrive = true;
                        break;

                    case MiscRPC.SyncCustomSettings:
                        ReceiveOptionRPC(reader);
                        break;

                    case MiscRPC.Notify:
                        ChatCommands.Notify(reader.ReadByte());
                        break;

                    case MiscRPC.SetSettings:
                        var map = reader.ReadByte();
                        TownOfUsReworked.NormalOptions.MapId = MapPatches.CurrentMap = map;
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        TownOfUsReworked.NormalOptions.CrewLightMod = CustomGameOptions.CrewVision;
                        TownOfUsReworked.NormalOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                        TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting != AnonVotes.Disabled;
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
                        TownOfUsReworked.NormalOptions.KillCooldown = CustomGameOptions.IntKillCd;
                        TownOfUsReworked.NormalOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                        TownOfUsReworked.NormalOptions.MaxPlayers = CustomGameOptions.LobbySize;
                        TownOfUsReworked.NormalOptions.NumShortTasks = CustomGameOptions.ShortTasks;
                        TownOfUsReworked.NormalOptions.NumLongTasks = CustomGameOptions.LongTasks;
                        TownOfUsReworked.NormalOptions.NumCommonTasks = CustomGameOptions.CommonTasks;
                        GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.NormalOptions;
                        CustomPlayer.AllPlayers.ForEach(x => x.MaxReportDistance = CustomGameOptions.ReportDistance);
                        MapPatches.AdjustSettings(map);
                        break;

                    case MiscRPC.SetFirstKilled:
                        CachedFirstDead = FirstDead = reader.ReadPlayer();
                        break;

                    /*case MiscRPC.ShareFriendCode:
                        reader.ReadPlayer().FriendCode = reader.ReadString();
                        break;*/

                    case MiscRPC.BodyLocation:
                        var id = reader.ReadByte();
                        var location = reader.ReadString();

                        if (!BodyLocations.TryAdd(id, location))
                            BodyLocations[id] = location;

                        break;

                    default:
                        LogError($"Received unknown RPC - {misc}");
                        break;
                }

                break;

            case CustomRPC.Change:
                var turn = (TurnRPC)reader.ReadByte();

                switch (turn)
                {
                    case TurnRPC.TurnTraitor:
                        reader.ReadLayer<Traitor>().TurnTraitor(reader.ReadBoolean(), reader.ReadBoolean());
                        break;

                    case TurnRPC.TurnSides:
                        reader.ReadLayer<Defector>().TurnSides(reader.ReadBoolean(), reader.ReadBoolean());
                        break;

                    case TurnRPC.TurnFanatic:
                        reader.ReadLayer<Fanatic>().TurnFanatic((Faction)reader.ReadByte());
                        break;

                    case TurnRPC.TurnAct:
                        reader.ReadLayer<Guesser>().TurnAct(reader.ReadLayerList<Role>());
                        break;

                    case TurnRPC.TurnRole:
                        reader.ReadLayer<Actor>().TurnRole(reader.ReadLayer<Role>());
                        break;

                    case TurnRPC.TurnVigilante:
                        reader.ReadLayer<VampireHunter>().TurnVigilante();
                        break;

                    case TurnRPC.TurnPestilence:
                        reader.ReadLayer<Plaguebearer>().TurnPestilence();
                        break;

                    case TurnRPC.TurnTroll:
                        reader.ReadLayer<BountyHunter>().TurnTroll();
                        break;

                    case TurnRPC.TurnSurv:
                        reader.ReadLayer<GuardianAngel>().TurnSurv();
                        break;

                    case TurnRPC.TurnGodfather:
                        reader.ReadLayer<Mafioso>().TurnGodfather();
                        break;

                    case TurnRPC.TurnJest:
                        reader.ReadLayer<Executioner>().TurnJest();
                        break;

                    case TurnRPC.TurnTraitorBetrayer:
                        reader.ReadLayer<Traitor>().TurnBetrayer();
                        break;

                    case TurnRPC.TurnFanaticBetrayer:
                        reader.ReadLayer<Fanatic>().TurnBetrayer();
                        break;

                    case TurnRPC.TurnRebel:
                        reader.ReadLayer<Sidekick>().TurnRebel();
                        break;

                    case TurnRPC.TurnThief:
                        reader.ReadLayer<Amnesiac>().TurnThief();
                        break;

                    case TurnRPC.TurnSheriff:
                        reader.ReadLayer<Seer>().TurnSheriff();
                        break;

                    case TurnRPC.TurnSeer:
                        reader.ReadLayer<Mystic>().TurnSeer();
                        break;

                    default:
                        LogError($"Received unknown RPC - {turn}");
                        break;
                }

                break;

            case CustomRPC.Target:
                var target = (TargetRPC)reader.ReadByte();

                switch (target)
                {
                    case TargetRPC.SetExeTarget:
                        reader.ReadLayer<Executioner>().TargetPlayer = reader.ReadPlayer();
                        break;

                    case TargetRPC.SetGuessTarget:
                        reader.ReadLayer<Guesser>().TargetPlayer = reader.ReadPlayer();
                        break;

                    case TargetRPC.SetGATarget:
                        reader.ReadLayer<GuardianAngel>().TargetPlayer = reader.ReadPlayer();
                        break;

                    case TargetRPC.SetBHTarget:
                        reader.ReadLayer<BountyHunter>().TargetPlayer = reader.ReadPlayer();
                        break;

                    case TargetRPC.SetActPretendList:
                        reader.ReadLayer<Actor>().PretendRoles = reader.ReadLayerList<Role>();
                        break;

                    case TargetRPC.SetAlliedFaction:
                        var player6 = reader.ReadPlayer();
                        var alliedRole = Role.GetRole(player6);
                        var ally = Objectifier.GetObjectifier<Allied>(player6);
                        var faction = (Faction)reader.ReadByte();
                        alliedRole.Faction = ally.Side = faction;
                        player6.Data.SetImpostor(faction is Faction.Intruder or Faction.Syndicate);
                        alliedRole.Alignment = alliedRole.Alignment.GetNewAlignment(faction);

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
                        var lover1 = reader.ReadLayer<Lovers>();
                        var lover2 = reader.ReadLayer<Lovers>();
                        lover1.OtherLover = lover2.Player;
                        lover2.OtherLover = lover1.Player;
                        break;

                    case TargetRPC.SetDuo:
                        var rival1 = reader.ReadLayer<Rivals>();
                        var rival2 = reader.ReadLayer<Rivals>();
                        rival1.OtherRival = rival2.Player;
                        rival2.OtherRival = rival1.Player;
                        break;

                    case TargetRPC.SetLinked:
                        var link1 = reader.ReadLayer<Linked>();
                        var link2 = reader.ReadLayer<Linked>();
                        link1.OtherLink = link2.Player;
                        link2.OtherLink = link1.Player;
                        break;

                    default:
                        LogError($"Received unknown RPC - {target}");
                        break;
                }

                break;

            case CustomRPC.Action:
                var action = (ActionsRPC)reader.ReadByte();

                switch (action)
                {
                    case ActionsRPC.FadeBody:
                        Coroutines.Start(FadeBody(reader.ReadBody()));
                        break;

                    case ActionsRPC.Convert:
                        RoleGen.Convert(reader.ReadByte(), reader.ReadByte(), (SubFaction)reader.ReadByte(), reader.ReadBoolean());
                        break;

                    case ActionsRPC.Teleport:
                        Teleport(reader.ReadPlayer(), reader.ReadVector2());
                        break;

                    case ActionsRPC.BypassKill:
                        MurderPlayer(reader.ReadPlayer(), reader.ReadPlayer(), (DeathReasonEnum)reader.ReadByte(), reader.ReadBoolean());
                        break;

                    case ActionsRPC.ForceKill:
                        var victim = reader.ReadPlayer();
                        var success = reader.ReadBoolean();
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).ForEach(x => x.BombSuccessful = success);
                        PlayerLayer.GetLayers<PromotedGodfather>().Where(x => x.BombedPlayer == victim).ForEach(x => x.BombSuccessful = success);
                        break;

                    case ActionsRPC.Mine:
                        AddVent(reader.ReadLayer<Role>(), reader.ReadVector2());
                        break;

                    case ActionsRPC.SetUninteractable:
                        try
                        {
                            UninteractiblePlayers.TryAdd(reader.ReadByte(), DateTime.UtcNow);
                        }
                        catch (Exception e)
                        {
                            LogError(e);
                        }

                        break;

                    case ActionsRPC.WarpAll:
                        var teleports = reader.ReadByte();
                        var coordinates = new Dictionary<byte, Vector2>();

                        for (var i = 0; i < teleports; i++)
                            coordinates.Add(reader.ReadByte(), reader.ReadVector2());

                        WarpPlayersToCoordinates(coordinates);
                        break;

                    case ActionsRPC.BarryButton:
                        var buttonBarry = reader.ReadPlayer();
                        buttonBarry.RemainingEmergencies++;

                        if (AmongUsClient.Instance.AmHost)
                        {
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

                    case ActionsRPC.Drop:
                        var dragged1 = reader.ReadBody();
                        dragged1.gameObject.GetComponent<DragBehaviour>().Destroy();
                        ((Janitor)Role.AllRoles.Find(x => x.Type == LayerEnum.Janitor && ((Janitor)x).CurrentlyDragging == dragged1)).CurrentlyDragging = null;
                        ((PromotedGodfather)Role.AllRoles.Find(x => x.Type == LayerEnum.PromotedGodfather && ((PromotedGodfather)x).CurrentlyDragging == dragged1)).CurrentlyDragging = null;
                        break;

                    case ActionsRPC.Burn:
                        var arsoRole = reader.ReadLayer<Arsonist>();

                        foreach (var body in AllBodies)
                        {
                            if (arsoRole.Doused.Contains(body.ParentId))
                                Ash.CreateAsh(body);
                        }

                        break;

                    case ActionsRPC.PlaceHit:
                        var requestor = reader.ReadPlayer();
                        Role.GetRole<BountyHunter>(Role.GetRole(requestor).Requestor).TentativeTarget = reader.ReadPlayer();
                        Role.GetRole(requestor).Requesting = false;
                        Role.GetRole(requestor).Requestor = null;
                        break;

                    case ActionsRPC.LayerAction1:
                        reader.ReadButton().StartEffectRPC(reader);
                        break;

                    case ActionsRPC.LayerAction2:
                        reader.ReadLayer().ReadRPC(reader);
                        break;

                    case ActionsRPC.Cancel:
                        reader.ReadButton().ClickedAgain = true;
                        break;

                    default:
                        LogError($"Received unknown RPC - {action}");
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
                        var nkRole = reader.ReadLayer<Role>();

                        switch (nkRole.Type)
                        {
                            case LayerEnum.Glitch:
                                Role.GlitchWins = true;
                                break;

                            case LayerEnum.Arsonist:
                                Role.ArsonistWins = true;
                                break;

                            case LayerEnum.Cryomaniac:
                                Role.CryomaniacWins = true;
                                break;

                            case LayerEnum.Juggernaut:
                                Role.JuggernautWins = true;
                                break;

                            case LayerEnum.Murderer:
                                Role.MurdererWins = true;
                                break;

                            case LayerEnum.Werewolf:
                                Role.WerewolfWins = true;
                                break;

                            case LayerEnum.SerialKiller:
                                Role.SerialKillerWins = true;
                                break;
                        }

                        if (winlose == WinLoseRPC.SameNKWins)
                        {
                            foreach (var role in Role.GetRoles(nkRole.Type))
                            {
                                if (!role.Disconnected && role.Faithful)
                                    role.Winner = true;
                            }
                        }

                        nkRole.Winner = true;
                        break;

                    case WinLoseRPC.ApocalypseWins:
                        Role.ApocalypseWins = true;
                        break;

                    case WinLoseRPC.JesterWin:
                        reader.ReadLayer<Jester>().VotedOut = true;
                        break;

                    case WinLoseRPC.CannibalWin:
                        reader.ReadLayer<Cannibal>().Eaten = true;
                        break;

                    case WinLoseRPC.ExecutionerWin:
                        reader.ReadLayer<Executioner>().TargetVotedOut = true;
                        break;

                    case WinLoseRPC.BountyHunterWin:
                        reader.ReadLayer<BountyHunter>().TargetKilled = true;
                        break;

                    case WinLoseRPC.TrollWin:
                        reader.ReadLayer<Troll>().Killed = true;
                        break;

                    case WinLoseRPC.ActorWin:
                        reader.ReadLayer<Actor>().Guessed = true;
                        break;

                    case WinLoseRPC.GuesserWin:
                        reader.ReadLayer<Guesser>().TargetGuessed = true;
                        break;

                    case WinLoseRPC.CorruptedWin:
                        Objectifier.CorruptedWins = true;

                        if (CustomGameOptions.AllCorruptedWin)
                            Objectifier.GetObjectifiers(LayerEnum.Corrupted).ForEach(x => x.Winner = true);

                        reader.ReadLayer().Winner = true;
                        break;

                    case WinLoseRPC.LoveWin:
                        Objectifier.LoveWins = true;
                        var lover = reader.ReadLayer<Lovers>();
                        lover.Winner = true;
                        Objectifier.GetObjectifier(lover.OtherLover).Winner = true;
                        break;

                    case WinLoseRPC.OverlordWin:
                        Objectifier.OverlordWins = true;
                        Objectifier.GetObjectifiers(LayerEnum.Overlord).Where(ov => ov.IsAlive).ForEach(x => x.Winner = true);
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
                        reader.ReadLayer<Phantom>().CompletedTasks = true;
                        break;

                    case WinLoseRPC.TaskRunnerWins:
                        Role.TaskRunnerWins = true;
                        reader.ReadLayer<Runner>().Winner = true;
                        break;

                    case WinLoseRPC.HuntedWins:
                        Role.HuntedWins = true;
                        break;

                    case WinLoseRPC.HunterWins:
                        Role.HunterWins = true;
                        break;

                    default:
                        LogError($"Received unknown RPC - {winlose}");
                        break;
                }

                break;

            default:
                LogError($"Received unknown RPC - {rpc}");
                break;
        }
    }
}