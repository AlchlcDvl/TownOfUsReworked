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
                Run("<color=#FF00FFFF>⚠ TEST ⚠</color>", "Received RPC!");
                break;

            case CustomRPC.Misc:
                var misc = (MiscRPC)reader.ReadByte();

                switch (misc)
                {
                    case MiscRPC.SetLayer:
                        RoleGen.SetLayer((LayerEnum)reader.ReadByte(), (PlayerLayerEnum)reader.ReadByte()).Start(reader.ReadPlayer());
                        break;

                    case MiscRPC.Whisper:
                        if (!Chat)
                            break;

                        var whisperer = reader.ReadPlayer();
                        var whispered = reader.ReadPlayer();
                        var message = reader.ReadString();

                        if (whispered == CustomPlayer.Local)
                            Run("<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} whispers to you: {message}");
                        else if ((CustomPlayer.Local.Is(LayerEnum.Blackmailer) && CustomGameOptions.WhispersNotPrivate) || DeadSeeEverything || (CustomPlayer.Local.Is(LayerEnum.Silencer)
                            && CustomGameOptions.WhispersNotPrivateSilencer))
                        {
                            Run("<color=#4D4DFFFF>「 Whispers 」</color>", $"{whisperer.name} is whispering to {whispered.name} : {message}");
                        }
                        else if (CustomGameOptions.WhispersAnnouncement)
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

                    case MiscRPC.AttemptSound:
                        Role.BreakShield(reader.ReadPlayer(), CustomGameOptions.ShieldBreaks);
                        break;

                    case MiscRPC.BastionBomb:
                        Role.BastionBomb(reader.ReadVent(), CustomGameOptions.BombRemovedOnKill);
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
                        // VersionHandshake(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadBoolean(), reader.ReadInt32(), reader.ReadBoolean(),
                        //     reader.ReadString(), new(reader.ReadBytesAndSize()), reader.ReadPackedInt32());
                        break;

                    case MiscRPC.SubmergedFixOxygen:
                        RepairOxygen();
                        break;

                    case MiscRPC.FixLights:
                        var lights = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case MiscRPC.FixMixup:
                        var mixup = Ship.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
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
                        TownOfUsReworked.NormalOptions.CrewLightMod = CustomGameOptions.CrewVision;
                        TownOfUsReworked.NormalOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                        TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting != AnonVotes.Disabled;
                        TownOfUsReworked.NormalOptions.VisualTasks = CustomGameOptions.VisualTasks;
                        TownOfUsReworked.NormalOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                        TownOfUsReworked.NormalOptions.NumImpostors = CustomGameOptions.IntruderCount;
                        TownOfUsReworked.NormalOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)reader.ReadByte();
                        // TownOfUsReworked.NormalOptions.TaskBarMode = CustomGameOptions2.TaskBarMode;
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
                        CustomPlayer.AllPlayers.ForEach(x => x.MaxReportDistance = CustomGameOptions.ReportDistance);
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
                        Run("<color=#00CC99FF>【 Loading Preset 】</color>", $"Loading the {reader.ReadString()} preset!");
                        break;

                    case MiscRPC.EndRoleGen:
                        foreach (var player2 in CustomPlayer.AllPlayers)
                        {
                            var role = player2.GetRole() ?? new Roleless().Start<Role>(player2);
                            var mod = player2.GetModifier() ?? new Modifierless().Start<Modifier>(player2);
                            var ab = player2.GetAbility() ?? new Abilityless().Start<Ability>(player2);
                            var obj = player2.GetObjectifier() ?? new Objectifierless().Start<Objectifier>(player2);

                            /*PlayerLayer.LayerLookup[player2.PlayerId] = [ role, mod, ab, obj ];
                            Role.RoleLookup[player2.PlayerId] = role;
                            Modifier.ModifierLookup[player2.PlayerId] = mod;
                            Objectifier.ObjectifierLookup[player2.PlayerId] = obj;
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
                            var faction = (Faction)reader.ReadByte();
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
                                fanatic.TurnFanatic((Faction)reader.ReadByte());
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
                        LogError($"Received unknown RPC - {(int)misc}");
                        break;
                }

                break;

            case CustomRPC.Vanilla:
                var vanilla = (VanillaRPC)reader.ReadByte();

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
                        LogError($"Received unknown RPC - {(int)vanilla}");
                        break;
                }

                break;

            case CustomRPC.Action:
                var action = (ActionsRPC)reader.ReadByte();

                switch (action)
                {
                    case ActionsRPC.FadeBody:
                        FadeBody(reader.ReadBody());
                        break;

                    case ActionsRPC.Convert:
                        RoleGen.Convert(reader.ReadByte(), reader.ReadByte(), (SubFaction)reader.ReadByte(), reader.ReadBoolean());
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
                                    hand.SetPlayerColor(mimicked.GetCurrentOutfit(), PlayerMaterial.MaskType.None);
                                else
                                    hand.SetPlayerColor(playerfromid.GetCurrentOutfit(), PlayerMaterial.MaskType.None);
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e);
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

                        foreach (var body in AllBodies)
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
                        LogError($"Received unknown RPC - {(int)action}");
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

                    case WinLoseRPC.BetrayerWin:
                        Role.BetrayerWins = true;
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
                            foreach (var role in PlayerLayer.GetLayers<Neutral>().Where(x => x.Type == nkRole.Type))
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

                    /*case WinLoseRPC.TrollWin:
                        reader.ReadLayer<Troll>().Killed = true;
                        break;*/

                    case WinLoseRPC.ActorWin:
                        reader.ReadLayer<Actor>().Guessed = true;
                        break;

                    case WinLoseRPC.GuesserWin:
                        reader.ReadLayer<Guesser>().TargetGuessed = true;
                        break;

                    case WinLoseRPC.CorruptedWin:
                        Objectifier.CorruptedWins = true;

                        if (CustomGameOptions.AllCorruptedWin)
                            PlayerLayer.GetLayers<Corrupted>().ForEach(x => x.Winner = true);

                        reader.ReadLayer().Winner = true;
                        break;

                    case WinLoseRPC.LoveWin:
                        Objectifier.LoveWins = true;
                        var lover = reader.ReadLayer<Lovers>();
                        lover.Winner = true;
                        lover.OtherLover.GetObjectifier().Winner = true;
                        break;

                    case WinLoseRPC.OverlordWin:
                        Objectifier.OverlordWins = true;
                        PlayerLayer.GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
                        break;

                    case WinLoseRPC.MafiaWins:
                        Objectifier.MafiaWins = true;
                        break;

                    case WinLoseRPC.DefectorWins:
                        Objectifier.DefectorWins = true;
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
                        LogError($"Received unknown RPC - {(int)winlose}");
                        break;
                }

                break;

            default:
                LogError($"Received unknown RPC - {(int)rpc}");
                break;
        }
    }
}