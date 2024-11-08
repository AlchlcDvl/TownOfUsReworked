namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class RPCHandling
{
    public static void Postfix(byte callId, MessageReader reader)
    {
        if (callId != 254)
            return;

        var rpc = reader.ReadEnum<CustomRPC>();

        switch (rpc)
        {
            case CustomRPC.Test:
                Message("Received RPC!");
                Run("<color=#FF00FFFF>⚠ TEST ⚠</color>", "Received RPC!");
                break;

            case CustomRPC.Misc:
                var misc = reader.ReadEnum<MiscRPC>();

                switch (misc)
                {
                    case MiscRPC.SetLayer:
                        RoleGen.SetLayer(reader.ReadEnum<LayerEnum>(), reader.ReadEnum<PlayerLayerEnum>()).Start(reader.ReadPlayer());
                        break;

                    case MiscRPC.Whisper:
                        if (!Chat())
                            break;

                        var whisperer = reader.ReadPlayer();
                        var whispered = reader.ReadPlayer();
                        var message = reader.ReadString();

                        if (whispered.AmOwner)
                            Run("<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} whispers to you: {message}");
                        else if ((CustomPlayer.Local.Is(LayerEnum.Blackmailer) && Blackmailer.WhispersNotPrivateB) || DeadSeeEverything() || (CustomPlayer.Local.Is(LayerEnum.Silencer) &&
                            Silencer.WhispersNotPrivateS))
                        {
                            Run("<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} is whispering to {whispered.name} : {message}");
                        }
                        else if (GameModifiers.WhispersAnnouncement)
                            Run("<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} is whispering to {whispered.name}.");

                        break;

                    case MiscRPC.Start:
                        RoleGen.ResetEverything();
                        break;

                    case MiscRPC.SyncPureCrew:
                        RoleGen.PureCrew = reader.ReadPlayer();
                        break;

                    case MiscRPC.SyncConvertible:
                        RoleGen.Convertible = reader.ReadInt32();
                        break;

                    case MiscRPC.BreakShield:
                        Role.BreakShield(reader.ReadPlayer(), Medic.ShieldBreaks);
                        break;

                    case MiscRPC.BastionBomb:
                        Role.BastionBomb(reader.ReadVent(), Bastion.BombRemovedOnKill);
                        break;

                    case MiscRPC.Catch:
                        PlayerControlOnClick.CatchPostmortal(reader.ReadPlayer(), reader.ReadPlayer());
                        break;

                    case MiscRPC.DoorSyncToilet:
                        var id2 = reader.ReadInt32();
                        UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == id2)?.SetDoorway(true);
                        break;

                    case MiscRPC.SyncSummary:
                        SaveText("Summary", reader.ReadString(), TownOfUsReworked.Other);
                        break;

                    case MiscRPC.VersionHandshake:
                        VersionHandshake(reader.ReadPlayerVersion(), reader.ReadPackedInt32());
                        break;

                    case MiscRPC.SubmergedFixOxygen:
                        RepairOxygen();
                        break;

                    case MiscRPC.FixLights:
                        var lights = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case MiscRPC.FixMixup:
                        var mixup = Ship().Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                        mixup.secondsForAutoHeal = 0.1f;
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
                        TownOfUsReworked.NormalOptions.MapId = MapPatches.CurrentMap = reader.ReadByte();
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Noisemaker, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Phantom, 0, 0);
                        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Tracker, 0, 0);
                        TownOfUsReworked.NormalOptions.CrewLightMod = CrewSettings.CrewVision;
                        TownOfUsReworked.NormalOptions.ImpostorLightMod = IntruderSettings.IntruderVision;
                        TownOfUsReworked.NormalOptions.AnonymousVotes = GameModifiers.AnonymousVoting != AnonVotes.Disabled;
                        TownOfUsReworked.NormalOptions.VisualTasks = GameModifiers.VisualTasks;
                        TownOfUsReworked.NormalOptions.PlayerSpeedMod = GameSettings.PlayerSpeed;
                        TownOfUsReworked.NormalOptions.NumImpostors = IntruderSettings.IntruderCount;
                        TownOfUsReworked.NormalOptions.TaskBarMode = GameSettings.TaskBarMode;
                        TownOfUsReworked.NormalOptions.ConfirmImpostor = GameSettings.ConfirmEjects;
                        TownOfUsReworked.NormalOptions.VotingTime = GameSettings.VotingTime;
                        TownOfUsReworked.NormalOptions.DiscussionTime = GameSettings.DiscussionTime;
                        TownOfUsReworked.NormalOptions.EmergencyCooldown = GameSettings.EmergencyButtonCooldown;
                        TownOfUsReworked.NormalOptions.NumEmergencyMeetings = GameSettings.EmergencyButtonCount;
                        TownOfUsReworked.NormalOptions.KillCooldown = IntruderSettings.IntKillCd;
                        TownOfUsReworked.NormalOptions.GhostsDoTasks = CrewSettings.GhostTasksCountToWin;
                        TownOfUsReworked.NormalOptions.MaxPlayers = GameSettings.LobbySize;
                        TownOfUsReworked.NormalOptions.NumShortTasks = TaskSettings.ShortTasks;
                        TownOfUsReworked.NormalOptions.NumLongTasks = TaskSettings.LongTasks;
                        TownOfUsReworked.NormalOptions.NumCommonTasks = TaskSettings.CommonTasks;
                        AllPlayers().ForEach(x => x.MaxReportDistance = GameSettings.ReportDistance);
                        MapPatches.AdjustSettings();
                        break;

                    case MiscRPC.SetFirstKilled:
                        CachedFirstDead = FirstDead = reader.ReadString();
                        break;

                    case MiscRPC.BodyLocation:
                        BodyLocations[reader.ReadByte()] = reader.ReadString();
                        break;

                    case MiscRPC.MoveBody:
                        reader.ReadBody().transform.position = reader.ReadVector2();
                        break;

                    case MiscRPC.LoadPreset:
                        var preset = reader.ReadString();
                        Run("<color=#00CC99FF>【 Loading Preset 】</color>", $"Loading the {preset} preset!");
                        SettingsPatches.CurrentPreset = preset;
                        break;

                    case MiscRPC.EndRoleGen:
                        foreach (var player2 in AllPlayers())
                        {
                            var role = player2.GetRole() ?? new Roleless().Start<Role>(player2);
                            var mod = player2.GetModifier() ?? new Modifierless().Start<Modifier>(player2);
                            var ab = player2.GetAbility() ?? new Abilityless().Start<Ability>(player2);
                            var disp = player2.GetDisposition() ?? new Dispositionless().Start<Disposition>(player2);

                            /*PlayerLayer.LayerLookup[player2.PlayerId] = [ role, mod, ab, disp ];
                            Role.RoleLookup[player2.PlayerId] = role;
                            Modifier.ModifierLookup[player2.PlayerId] = mod;
                            Disposition.DispositionLookup[player2.PlayerId] = disp;
                            Ability.AbilityLookup[player2.PlayerId] = ab;*/
                        }

                        break;

                    case MiscRPC.SetTarget:
                        var layer = reader.ReadLayer();

                        if (layer.Type == LayerEnum.Executioner)
                            ((Executioner)layer).TargetPlayer = reader.ReadPlayer();
                        else if (layer.Type == LayerEnum.Guesser)
                            ((Guesser)layer).TargetPlayer = reader.ReadPlayer();
                        else if (layer.Type == LayerEnum.GuardianAngel)
                            ((GuardianAngel)layer).TargetPlayer = reader.ReadPlayer();
                        else if (layer.Type == LayerEnum.BountyHunter)
                            ((BountyHunter)layer).TargetPlayer = reader.ReadPlayer();
                        else if (layer.Type == LayerEnum.Actor)
                            ((Actor)layer).PretendRoles = reader.ReadLayerList<Role>();
                        else if (layer.Type == LayerEnum.Allied)
                        {
                            var ally = (Allied)layer;
                            var alliedRole = ally.Player.GetRole();
                            var faction = reader.ReadEnum<Faction>();
                            alliedRole.Faction = ally.Side = faction;
                            ally.Player.SetImpostor(faction is Faction.Intruder or Faction.Syndicate);
                            alliedRole.Alignment = alliedRole.Alignment.GetNewAlignment(faction);
                            alliedRole.FactionColor = faction switch
                            {
                                Faction.Crew => CustomColorManager.Crew,
                                Faction.Intruder => CustomColorManager.Intruder,
                                Faction.Syndicate => CustomColorManager.Syndicate,
                                _ => CustomColorManager.Neutral,
                            };
                        }
                        else if (layer.Type == LayerEnum.Actor)
                        {
                            var lover1 = (Lovers)layer;
                            var lover2 = reader.ReadLayer<Lovers>();
                            lover1.OtherLover = lover2.Player;
                            lover2.OtherLover = lover1.Player;
                        }
                        else if (layer.Type == LayerEnum.Actor)
                        {
                            var rival1 = (Rivals)layer;
                            var rival2 = reader.ReadLayer<Rivals>();
                            rival1.OtherRival = rival2.Player;
                            rival2.OtherRival = rival1.Player;
                        }
                        else if (layer.Type == LayerEnum.Actor)
                        {
                            var link1 = (Linked)layer;
                            var link2 = reader.ReadLayer<Linked>();
                            link1.OtherLink = link2.Player;
                            link2.OtherLink = link1.Player;
                        }

                        break;

                    case MiscRPC.ChangeRoles:
                        var layer2 = reader.ReadLayer();

                        if (layer2 is Traitor traitor)
                        {
                            if (reader.ReadBoolean())
                                traitor.TurnBetrayer();
                            else
                                traitor.TurnTraitor(reader.ReadBoolean(), reader.ReadBoolean());
                        }
                        else if (layer2 is Defector defector)
                            defector.TurnSides(reader.ReadBoolean(), reader.ReadBoolean(), reader.ReadBoolean());
                        else if (layer2 is Fanatic fanatic)
                        {
                            if (reader.ReadBoolean())
                                fanatic.TurnBetrayer();
                            else
                                fanatic.TurnFanatic(reader.ReadEnum<Faction>());
                        }
                        else if (layer2 is Guesser guess)
                            guess.TurnAct();
                        else if (layer2 is Actor act)
                            act.TurnRole(reader.ReadLayer<Role>());
                        else if (layer2 is VampireHunter vh)
                            vh.TurnVigilante();
                        else if (layer2 is Plaguebearer pb)
                            pb.TurnPestilence();
                        else if (layer2 is BountyHunter bh)
                            bh.TurnTroll();
                        else if (layer2 is GuardianAngel ga)
                            ga.TurnSurv();
                        else if (layer2 is Mafioso mafioso)
                            mafioso.TurnGodfather();
                        else if (layer2 is Executioner exe)
                            exe.TurnJest();
                        else if (layer2 is Sidekick sidekick)
                            sidekick.TurnRebel();
                        else if (layer2 is Amnesiac amne)
                            amne.TurnThief();
                        else if (layer2 is Seer seer)
                            seer.TurnSheriff();
                        else if (layer2 is Mystic mystic)
                            mystic.TurnSeer();

                        break;

                    default:
                        Error($"Received unknown RPC - {(int)misc}");
                        break;
                }

                break;

            case CustomRPC.Vanilla:
                var vanilla = reader.ReadEnum<VanillaRPC>();

                switch (vanilla)
                {
                    case VanillaRPC.SnapTo:
                        reader.ReadPlayer().CustomSnapTo(reader.ReadVector2());
                        break;

                    case VanillaRPC.SetColor:
                        var player = reader.ReadPlayer();
                        player.SetColor(reader.ReadByte());
                        PlayerHandler.Instance.ColorNames[player.PlayerId] = player.Data.ColorName.Replace("(", "").Replace(")", "");
                        break;

                    default:
                        Error($"Received unknown RPC - {(int)vanilla}");
                        break;
                }

                break;

            case CustomRPC.Action:
                var action = reader.ReadEnum<ActionsRPC>();

                switch (action)
                {
                    case ActionsRPC.FadeBody:
                        FadeBody(reader.ReadBody());
                        break;

                    case ActionsRPC.Convert:
                        RoleGen.Convert(reader.ReadByte(), reader.ReadByte(), reader.ReadEnum<SubFaction>(), reader.ReadBoolean());
                        break;

                    case ActionsRPC.BypassKill:
                        MurderPlayer(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadEnum<DeathReasonEnum>(), reader.ReadBoolean());
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

                    case ActionsRPC.Infect:
                        Pestilence.Infected[reader.ReadByte()] = reader.ReadInt32();
                        break;

                    case ActionsRPC.SetUninteractable:
                        try
                        {
                            var playerid = reader.ReadByte();
                            UninteractiblePlayers.TryAdd(playerid, DateTime.UtcNow);
                            UninteractiblePlayers2.TryAdd(playerid, reader.ReadSingle());

                            if (reader.ReadBoolean())
                            {
                                var zipline = UObject.FindObjectOfType<ZiplineBehaviour>();
                                var hand = zipline.playerIdHands[playerid];
                                var playerfromid = PlayerById(playerid);

                                if (playerfromid.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.PlayerNameOnly)
                                    hand.handRenderer.color.SetAlpha(playerfromid.MyRend().color.a);
                                else if (playerfromid.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
                                    PlayerMaterial.SetColors(UColor.grey, hand.handRenderer);
                                else if (playerfromid.GetCustomOutfitType() == CustomPlayerOutfitType.Colorblind)
                                    hand.handRenderer.color = UColor.grey;
                                else if (playerfromid.IsMimicking(out var mimicked))
                                    hand.SetPlayerColor(mimicked.GetCurrentOutfit(), PlayerMaterial.MaskType.None, mimicked.cosmetics.GetPhantomRoleAlpha());
                                else
                                    hand.SetPlayerColor(playerfromid.GetCurrentOutfit(), PlayerMaterial.MaskType.None, playerfromid.cosmetics.GetPhantomRoleAlpha());
                            }
                        }
                        catch (Exception e)
                        {
                            Error(e);
                        }

                        break;

                    case ActionsRPC.CallMeeting:
                        CallMeeting(reader.ReadPlayer());
                        break;

                    case ActionsRPC.BaitReport:
                        reader.ReadPlayer().ReportDeadBody(reader.ReadPlayer().Data);
                        break;

                    case ActionsRPC.Drop:
                        var dragger = reader.ReadPlayer();
                        var dragged1 = BodyById(DragHandler.Instance.Dragging[dragger.PlayerId]);
                        DragHandler.Instance.StopDrag(dragger);
                        PlayerLayer.GetLayers<Janitor>().Where(x => x.CurrentlyDragging == dragged1).ToList().ForEach(x => x.CurrentlyDragging = null);
                        PlayerLayer.GetLayers<PromotedGodfather>().Where(x => x.IsJani && x.CurrentlyDragging == dragged1).ToList().ForEach(x => x.CurrentlyDragging = null);
                        break;

                    case ActionsRPC.Burn:
                        var arsoRole = reader.ReadLayer<Arsonist>();

                        foreach (var body in AllBodies())
                        {
                            if (arsoRole.Doused.Contains(body.ParentId))
                                Ash.CreateAsh(body);
                        }

                        break;

                    case ActionsRPC.PlaceHit:
                        var requestor = reader.ReadPlayer().GetRole();
                        requestor.Requestor.GetLayer<BountyHunter>().TentativeTarget = reader.ReadPlayer();
                        requestor.Requesting = false;
                        requestor.Requestor = null;
                        break;

                    case ActionsRPC.ButtonAction:
                        reader.ReadButton().StartEffectRPC(reader);
                        break;

                    case ActionsRPC.LayerAction:
                        reader.ReadLayer().ReadRPC(reader);
                        break;

                    case ActionsRPC.Cancel:
                        reader.ReadButton().ClickedAgain = true;
                        break;

                    case ActionsRPC.PublicReveal:
                        Role.PublicReveal(reader.ReadPlayer());
                        break;

                    default:
                        Error($"Received unknown RPC - {(int)action}");
                        break;
                }

                break;

            case CustomRPC.WinLose:
                WinState = reader.ReadEnum<WinLose>();

                switch (WinState)
                {
                    case WinLose.ArsonistWins or WinLose.CryomaniacWins or WinLose.GlitchWins or WinLose.MurdererWins or WinLose.JuggernautWins or WinLose.SerialKillerWins or
                        WinLose.WerewolfWins:
                        var nkRole = reader.ReadLayer<Role>();

                        if (NeutralSettings.NoSolo == NoSolo.SameNKs)
                        {
                            foreach (var role in PlayerLayer.GetLayers<Neutral>().Where(x => x.Type == nkRole.Type))
                            {
                                if (!role.Disconnected && role.Faithful)
                                    role.Winner = true;
                            }
                        }
                        else
                            nkRole.Winner = true;

                        break;

                    case WinLose.JesterWins:
                        reader.ReadLayer<Jester>().VotedOut = true;
                        break;

                    case WinLose.CannibalWins:
                        reader.ReadLayer<Cannibal>().Eaten = true;
                        break;

                    case WinLose.ExecutionerWins:
                        reader.ReadLayer<Executioner>().TargetVotedOut = true;
                        break;

                    case WinLose.BountyHunterWins:
                        reader.ReadLayer<BountyHunter>().TargetKilled = true;
                        break;

                    /*case WinLose.TrollWins:
                        reader.ReadLayer<Troll>().Killed = true;
                        break;*/

                    case WinLose.ActorWins:
                        reader.ReadLayer<Actor>().Guessed = true;
                        break;

                    case WinLose.GuesserWins:
                        reader.ReadLayer<Guesser>().TargetGuessed = true;
                        break;

                    case WinLose.CorruptedWins:

                        if (Corrupted.AllCorruptedWin)
                            PlayerLayer.GetLayers<Corrupted>().ForEach(x => x.Winner = true);

                        reader.ReadLayer().Winner = true;
                        break;

                    case WinLose.LoveWins:
                        var lover = reader.ReadLayer<Lovers>();
                        lover.Winner = true;
                        lover.OtherLover.GetDisposition().Winner = true;
                        break;

                    case WinLose.OverlordWins:
                        PlayerLayer.GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
                        break;

                    case WinLose.TaskmasterWins:
                        reader.ReadLayer().Winner = true;
                        break;

                    case WinLose.RivalWins:
                        reader.ReadLayer().Winner = true;
                        break;

                    case WinLose.TaskRunnerWins:
                        reader.ReadLayer<Runner>().Winner = true;
                        break;
                }

                break;

            default:
                Error($"Received unknown RPC - {(int)rpc}");
                break;
        }
    }
}