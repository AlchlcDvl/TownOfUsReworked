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
            {
                Message("Received RPC!");
                var test = reader.ReadEnum<TestRPC>();

                switch (test)
                {
                    case TestRPC.Argless:
                    {
                        Run("<#FF00FFFF>⚠ TEST ⚠</color>", "Received RPC!");
                        return;
                    }
                    case TestRPC.Args:
                    {
                        var message = "";

                        while (reader.BytesRemaining > 0)
                            message += $"{reader.ReadString()} ";

                        Run("<#FF00FFFF>⚠ TEST ⚠</color>", $"Received RPC!\nWith the following message: {message.Trim()}");
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown test RPC - {test}");
                        return;
                    }
                }
            }
            case CustomRPC.Misc:
            {
                var misc = reader.ReadEnum<MiscRPC>();

                switch (misc)
                {
                    case MiscRPC.SyncMaxUses:
                    {
                        reader.ReadButton().MaxUses = reader.ReadInt32();
                        return;
                    }
                    case MiscRPC.SyncUses:
                    {
                        reader.ReadButton().Uses = reader.ReadInt32();
                        return;
                    }
                    case MiscRPC.SetLayer:
                    {
                        RoleGenManager.GetLayer(reader.ReadEnum<LayerEnum>(), reader.ReadEnum<PlayerLayerEnum>()).Start(reader.ReadPlayer());
                        return;
                    }
                    case MiscRPC.Whisper:
                    {
                        var whisperer = reader.ReadPlayer();
                        var whispered = reader.ReadPlayer();
                        var message = reader.ReadString().Trim();

                        if (whispered.AmOwner)
                            Run("<#4D4DFFFF>「 Whispers 」</color>", $"#({whisperer.name}) whispers to you: {message}");
                        else if ((CustomPlayer.Local.Is<Blackmailer>() && Blackmailer.WhispersNotPrivateB) || DeadSeeEverything() || (CustomPlayer.Local.Is<Silencer>() &&
                            Silencer.WhispersNotPrivateS))
                        {
                            Run("<#4D4DFFFF>「 Whispers 」</color>", $"#({whisperer.name}) whispers to #({whispered.name}): {message}");
                        }
                        else if (GameModifiers.WhispersAnnouncement)
                            Run("<#4D4DFFFF>「 Whispers 」</color>", $"#({whisperer.name}) is whispering to #({whispered.name}).");

                        return;
                    }
                    case MiscRPC.Start:
                    {
                        RoleGenManager.ResetEverything();
                        return;
                    }
                    case MiscRPC.BreakShield:
                    {
                        Role.BreakShield(reader.ReadPlayer(), Medic.ShieldBreaks);
                        return;
                    }
                    case MiscRPC.BastionBomb:
                    {
                        Role.BastionBomb(reader.ReadVent(), Bastion.BombRemovedOnKill);
                        return;
                    }
                    case MiscRPC.Catch:
                    {
                        PlayerControlOnClick.CatchPostmortal(reader.ReadPlayer(), reader.ReadPlayer());
                        return;
                    }
                    case MiscRPC.DoorSyncToilet:
                    {
                        var id2 = reader.ReadInt32();
                        UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == id2)?.SetDoorway(true);
                        return;
                    }
                    case MiscRPC.SyncSummary:
                    {
                        SaveText("Summary", reader.ReadString(), TownOfUsReworked.Other);
                        return;
                    }
                    case MiscRPC.SubmergedFixOxygen:
                    {
                        RepairOxygen();
                        return;
                    }
                    case MiscRPC.FixLights:
                    {
                        var lights = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        return;
                    }
                    case MiscRPC.FixMixup:
                    {
                        var mixup = Ship().Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                        mixup.secondsForAutoHeal = 0.1f;
                        return;
                    }
                    case MiscRPC.ChaosDrive:
                    {
                        Syndicate.DriveHolder = reader.ReadPlayer();
                        Syndicate.SyndicateHasChaosDrive = Syndicate.DriveHolder;
                        return;
                    }
                    case MiscRPC.SyncCustomSettings:
                    {
                        ReceiveOptionRPC(reader);
                        return;
                    }
                    case MiscRPC.SyncMap:
                    {
                        SettingsPatches.SetMap(reader.ReadEnum<MapEnum>());
                        return;
                    }
                    case MiscRPC.Notify:
                    {
                        ChatPatches.Notify(reader.ReadByte());
                        return;
                    }
                    case MiscRPC.SetSettings:
                    {
                        TownOfUsReworked.NormalOptions.MapId = MapPatches.CurrentMap = reader.ReadByte();
                        MapPatches.SetDefaults();
                        MapPatches.AdjustSettings();
                        return;
                    }
                    case MiscRPC.SetFirstKilled:
                    {
                        CachedFirstDead = FirstDead = reader.ReadString();
                        return;
                    }
                    case MiscRPC.BodyLocation:
                    {
                        BodyLocations[reader.ReadByte()] = reader.ReadString();
                        return;
                    }
                    case MiscRPC.MoveBody:
                    {
                        reader.ReadBody().transform.position = reader.ReadVector2();
                        return;
                    }
                    case MiscRPC.LoadPreset:
                    {
                        var preset = reader.ReadString();
                        Run("<#00CC99FF>【 Loading Preset 】</color>", $"Loading the {preset} preset!");
                        SettingsPatches.CurrentPreset = preset;
                        return;
                    }
                    case MiscRPC.EndRoleGen:
                    {
                        AllPlayers().ForEach(x => RoleManager.Instance.SetRole(x, (RoleTypes)100));
                        SetPostmortals.Revealers = reader.ReadByte();
                        SetPostmortals.Phantoms = reader.ReadByte();
                        SetPostmortals.Banshees = reader.ReadByte();
                        SetPostmortals.Ghouls = reader.ReadByte();
                        RoleGenManager.Pure = reader.ReadPlayer();
                        RoleGenManager.Convertible = reader.ReadByte();
                        BetterAirship.SpawnPoints.AddRange(reader.ReadByteList());
                        AmongUsClient.Instance.StartCoroutine(HUD().CoShowIntro());
                        return;
                    }
                    case MiscRPC.SetTarget:
                    {
                        var layer = reader.ReadLayer();

                        if (layer is Executioner exe)
                            exe.TargetPlayer = reader.ReadPlayer();
                        else if (layer is Guesser guesser)
                            guesser.TargetPlayer = reader.ReadPlayer();
                        else if (layer is GuardianAngel angel)
                            angel.TargetPlayer = reader.ReadPlayer();
                        else if (layer is BountyHunter hunter)
                            hunter.TargetPlayer = reader.ReadPlayer();
                        else if (layer is Actor actor)
                        {
                            actor.PretendRoles.Clear();
                            actor.PretendRoles.AddRange(reader.ReadLayerList<Role>());
                        }
                        else if (layer is Allied ally)
                        {
                            var alliedRole = ally.Player.GetRole();
                            var faction = reader.ReadEnum<Faction>();
                            alliedRole.Faction = ally.Side = faction;
                        }
                        else if (layer is Lovers lover1)
                        {
                            var lover2 = reader.ReadLayer<Lovers>();
                            lover1.OtherLover = lover2.Player;
                            lover2.OtherLover = lover1.Player;
                        }
                        else if (layer is Rivals rival1)
                        {
                            var rival2 = reader.ReadLayer<Rivals>();
                            rival1.OtherRival = rival2.Player;
                            rival2.OtherRival = rival1.Player;
                        }
                        else if (layer is Linked link1)
                        {
                            var link2 = reader.ReadLayer<Linked>();
                            link1.OtherLink = link2.Player;
                            link2.OtherLink = link1.Player;
                        }

                        return;
                    }
                    case MiscRPC.ChangeRoles:
                    {
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
                        else if (layer2 is Actor act)
                            act.TurnRole(reader.ReadEnum<LayerEnum>());

                        return;
                    }
                    case MiscRPC.Achievement:
                    {
                        CustomAchievementManager.UnlockAchievement(reader.ReadString());
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown misc RPC - {misc}");
                        return;
                    }
                }
            }
            case CustomRPC.Vanilla:
            {
                var vanilla = reader.ReadEnum<VanillaRPC>();

                switch (vanilla)
                {
                    case VanillaRPC.SnapTo:
                    {
                        reader.ReadPlayer().CustomSnapTo(reader.ReadVector2());
                        return;
                    }
                    case VanillaRPC.SetColor:
                    {
                        reader.ReadPlayer().SetColor(reader.ReadByte());
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown vanilla RPC - {vanilla}");
                        return;
                    }
                }
            }
            case CustomRPC.Action:
            {
                var action = reader.ReadEnum<ActionsRPC>();

                switch (action)
                {
                    case ActionsRPC.FadeBody:
                    {
                        FadeBody(reader.ReadBody());
                        return;
                    }
                    case ActionsRPC.Convert:
                    {
                        ConvertPlayer(reader.ReadByte(), reader.ReadByte(), reader.ReadEnum<SubFaction>(), reader.ReadBoolean());
                        return;
                    }
                    case ActionsRPC.BypassKill:
                    {
                        reader.ReadPlayer().MurderPlayer(reader.ReadPlayer(), reader.ReadEnum<DeathReasonEnum>(), reader.ReadBoolean());
                        return;
                    }
                    case ActionsRPC.ForceKill:
                    {
                        var victim = reader.ReadPlayer();
                        var success = reader.ReadBoolean();
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).ForEach(x => x.BombSuccessful = success);
                        PlayerLayer.GetLayers<PromotedGodfather>().Where(x => x.BombedPlayer == victim).ForEach(x => x.BombSuccessful = success);
                        return;
                    }
                    case ActionsRPC.Mine:
                    {
                        AddVent(reader.ReadLayer<Role>() as IDigger, reader.ReadVector2());
                        return;
                    }
                    case ActionsRPC.Infect:
                    {
                        Pestilence.Infected[reader.ReadByte()] = reader.ReadInt32();
                        return;
                    }
                    case ActionsRPC.SetUninteractable:
                    {
                        try
                        {
                            var playerid = reader.ReadByte();
                            UninteractiblePlayers.TryAdd(playerid, Time.time);
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

                        return;
                    }
                    case ActionsRPC.CallMeeting:
                    {
                        CallMeeting(reader.ReadPlayer());
                        return;
                    }
                    case ActionsRPC.BaitReport:
                    {
                        reader.ReadPlayer().ReportDeadBody(reader.ReadPlayer().Data);
                        return;
                    }
                    case ActionsRPC.Drop:
                    {
                        var dragger = reader.ReadPlayer();
                        var dragged1 = BodyById(DragHandler.Instance.Dragging[dragger.PlayerId]);
                        DragHandler.Instance.StopDrag(dragger);
                        PlayerLayer.GetILayers<IDragger>().Where(x => x.CurrentlyDragging == dragged1).ForEach(x => x.CurrentlyDragging = null);
                        return;
                    }
                    case ActionsRPC.Burn:
                    {
                        var disappear = reader.ReadByteList();

                        foreach (var body in AllBodies())
                        {
                            if (disappear.Contains(body.ParentId))
                                Ash.CreateAsh(body);
                        }

                        return;
                    }
                    case ActionsRPC.PlaceHit:
                    {
                        var requestor = reader.ReadPlayer().GetRole();
                        requestor.Requestor.GetLayer<BountyHunter>().TentativeTarget = reader.ReadPlayer();
                        requestor.Requesting = false;
                        requestor.Requestor = null;
                        return;
                    }
                    case ActionsRPC.ButtonAction:
                    {
                        reader.ReadButton().StartEffectRPC(reader);
                        return;
                    }
                    case ActionsRPC.LayerAction:
                    {
                        reader.ReadLayer().ReadRPC(reader);
                        return;
                    }
                    case ActionsRPC.Cancel:
                    {
                        reader.ReadButton().ClickedAgain = true;
                        return;
                    }
                    case ActionsRPC.PublicReveal:
                    {
                        Role.PublicReveal(reader.ReadPlayer());
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown action RPC - {action}");
                        return;
                    }
                }
            }
            case CustomRPC.WinLose:
            {
                WinState = reader.ReadEnum<WinLose>();

                switch (WinState)
                {
                    case >= WinLose.ArsonistWins and <= WinLose.WerewolfWins:
                    {
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

                        return;
                    }
                    case WinLose.JesterWins:
                    {
                        reader.ReadLayer<Jester>().VotedOut = true;
                        return;
                    }
                    case WinLose.CannibalWins:
                    {
                        reader.ReadLayer<Cannibal>().Eaten = true;
                        return;
                    }
                    case WinLose.ExecutionerWins:
                    {
                        reader.ReadLayer<Executioner>().TargetVotedOut = true;
                        return;
                    }
                    case WinLose.BountyHunterWins:
                    {
                        reader.ReadLayer<BountyHunter>().TargetKilled = true;
                        return;
                    }
                    case WinLose.ActorWins:
                    {
                        reader.ReadLayer<Actor>().Guessed = true;
                        return;
                    }
                    case WinLose.GuesserWins:
                    {
                        reader.ReadLayer<Guesser>().TargetGuessed = true;
                        return;
                    }
                    case WinLose.CorruptedWins:
                    {
                        if (Corrupted.AllCorruptedWin)
                            PlayerLayer.GetLayers<Corrupted>().ForEach(x => x.Winner = true);

                        reader.ReadLayer().Winner = true;
                        return;
                    }
                    case WinLose.LoveWins:
                    {
                        var lover = reader.ReadLayer<Lovers>();
                        lover.Winner = true;
                        lover.OtherLover.GetDisposition().Winner = true;
                        return;
                    }
                    case WinLose.OverlordWins:
                    {
                        PlayerLayer.GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
                        return;
                    }
                    case WinLose.TaskmasterWins:
                    {
                        reader.ReadLayer().Winner = true;
                        return;
                    }
                    case WinLose.RivalWins:
                    {
                        reader.ReadLayer().Winner = true;
                        return;
                    }
                    case WinLose.TaskRunnerWins:
                    {
                        reader.ReadLayer<Runner>().Winner = true;
                        return;
                    }
                }

                return;
            }
            default:
            {
                Failure($"Received unknown custom RPC - {rpc}");
                return;
            }
        }
    }
}