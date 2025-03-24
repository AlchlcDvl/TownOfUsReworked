namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class RPCHandling
{
    public static void Postfix(byte callId, MessageReader reader)
    {
        if (callId != 254)
            return;

        var rpc = reader.Read<CustomRPC>();

        switch (rpc)
        {
            case CustomRPC.Test:
            {
                Message("Received RPC!");
                var test = reader.Read<TestRPC>();

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
                var misc = reader.Read<MiscRPC>();

                switch (misc)
                {
                    case MiscRPC.SyncMaxUses:
                    {
                        reader.Read<CustomButton>().MaxUses = reader.ReadPackedInt32();
                        return;
                    }
                    case MiscRPC.SyncUses:
                    {
                        reader.Read<CustomButton>().Uses = reader.ReadPackedInt32();
                        return;
                    }
                    case MiscRPC.SetLayer:
                    {
                        RoleGenManager.GetLayer(reader.Read<LayerEnum>(), reader.Read<PlayerLayerEnum>()).Start(reader.Read<PlayerControl>());
                        return;
                    }
                    case MiscRPC.Whisper:
                    {
                        var whisperer = reader.Read<PlayerControl>();
                        var whispered = reader.Read<PlayerControl>();
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
                        Role.BreakShield(reader.Read<PlayerControl>(), Medic.ShieldBreaks);
                        return;
                    }
                    case MiscRPC.BastionBomb:
                    {
                        Role.BastionBomb(reader.Read<Vent>(), Bastion.BombRemovedOnKill);
                        return;
                    }
                    case MiscRPC.Catch:
                    {
                        PlayerControlOnClick.CatchPostmortal(reader.Read<PlayerControl>(), reader.Read<PlayerControl>());
                        return;
                    }
                    case MiscRPC.DoorSyncToilet:
                    {
                        var id2 = reader.ReadPackedInt32();
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
                        Syndicate.DriveHolder = reader.Read<PlayerControl>();
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
                        SettingsPatches.SetMap(reader.Read<MapEnum>());
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
                        reader.Read<DeadBody>().transform.position = reader.ReadVector2();
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
                        RoleGenManager.Pure = reader.Read<PlayerControl>();
                        RoleGenManager.Convertible = reader.ReadByte();
                        BetterAirship.SpawnPoints.AddRange(reader.ReadByteList());
                        AmongUsClient.Instance.StartCoroutine(HUD().CoShowIntro());
                        return;
                    }
                    case MiscRPC.SetTarget:
                    {
                        var layer = reader.Read<PlayerLayer>();

                        switch (layer)
                        {
                            case Executioner exe:
                            {
                                exe.TargetPlayer = reader.Read<PlayerControl>();
                                break;
                            }
                            case Guesser guesser:
                            {
                                guesser.TargetPlayer = reader.Read<PlayerControl>();
                                break;
                            }
                            case GuardianAngel angel:
                            {
                                angel.TargetPlayer = reader.Read<PlayerControl>();
                                break;
                            }
                            case BountyHunter hunter:
                            {
                                hunter.TargetPlayer = reader.Read<PlayerControl>();
                                break;
                            }
                            case Actor actor:
                            {
                                actor.PretendRoles.Clear();
                                actor.PretendRoles.AddRange(reader.ReadLayers<Role>());
                                break;
                            }
                            case Allied ally:
                            {
                                var alliedRole = ally.Player.GetRole();
                                var faction = reader.Read<Faction>();
                                alliedRole.Faction = ally.Side = faction;
                                break;
                            }
                            case Lovers lover1:
                            {
                                var lover2 = reader.Read<Lovers>();
                                lover1.OtherLover = lover2.Player;
                                lover2.OtherLover = lover1.Player;
                                break;
                            }
                            case Rivals rival1:
                            {
                                var rival2 = reader.Read<Rivals>();
                                rival1.OtherRival = rival2.Player;
                                rival2.OtherRival = rival1.Player;
                                break;
                            }
                            case Linked link1:
                            {
                                var link2 = reader.Read<Linked>();
                                link1.OtherLink = link2.Player;
                                link2.OtherLink = link1.Player;
                                break;
                            }
                        }

                        return;
                    }
                    case MiscRPC.ChangeRoles:
                    {
                        var layer2 = reader.Read<PlayerLayer>();

                        switch (layer2)
                        {
                            case Traitor traitor:
                            {
                                if (reader.ReadBoolean())
                                    traitor.TurnBetrayer();
                                else
                                    traitor.TurnTraitor(reader.ReadBoolean(), reader.ReadBoolean());

                                break;
                            }
                            case Defector defector:
                            {
                                defector.TurnSides(reader.ReadBoolean(), reader.ReadBoolean(), reader.ReadBoolean());
                                break;
                            }
                            case Fanatic fanatic:
                            {
                                if (reader.ReadBoolean())
                                    fanatic.TurnBetrayer();
                                else
                                    fanatic.TurnFanatic(reader.Read<Faction>());

                                break;
                            }
                            case Actor act:
                            {
                                act.TurnRole(reader.Read<LayerEnum>());
                                break;
                            }
                        }

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
                var vanilla = reader.Read<VanillaRPC>();

                switch (vanilla)
                {
                    case VanillaRPC.SnapTo:
                    {
                        reader.Read<PlayerControl>().CustomSnapTo(reader.ReadVector2());
                        return;
                    }
                    case VanillaRPC.SetColor:
                    {
                        reader.Read<PlayerControl>().SetColor(reader.ReadByte());
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
                var action = reader.Read<ActionsRPC>();

                switch (action)
                {
                    case ActionsRPC.FadeBody:
                    {
                        FadeBody(reader.Read<DeadBody>());
                        return;
                    }
                    case ActionsRPC.Convert:
                    {
                        ConvertPlayer(reader.ReadByte(), reader.ReadByte(), reader.Read<SubFaction>(), reader.ReadBoolean());
                        return;
                    }
                    case ActionsRPC.BypassKill:
                    {
                        reader.Read<PlayerControl>().MurderPlayer(reader.Read<PlayerControl>(), reader.Read<DeathReasonEnum>(), reader.ReadBoolean());
                        return;
                    }
                    case ActionsRPC.ForceKill:
                    {
                        var victim = reader.Read<PlayerControl>();
                        var success = reader.ReadBoolean();
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).ForEach(x => x.BombSuccessful = success);
                        return;
                    }
                    case ActionsRPC.Infect:
                    {
                        Pestilence.Infected[reader.ReadByte()] = reader.ReadPackedUInt32();
                        return;
                    }
                    case ActionsRPC.SetUninteractable:
                    {
                        try
                        {
                            var playerid = reader.ReadByte();
                            UninteractablePlayers.TryAdd(playerid, Time.time);
                            UninteractablePlayers2.TryAdd(playerid, reader.ReadSingle());

                            if (reader.ReadBoolean())
                            {
                                var hand = UObject.FindObjectOfType<ZiplineBehaviour>().playerIdHands[playerid];
                                var playerFromId = PlayerById(playerid);
                                PlayerMaterial.SetColors(playerFromId.GetCustomOutfitType() switch
                                {
                                    CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.PlayerNameOnly => playerFromId.GetPlayerColor(),
                                    CustomPlayerOutfitType.Camouflage or CustomPlayerOutfitType.Colorblind => UColor.grey,
                                    _ => (playerFromId.IsMimicking(out var mimicked) ? mimicked : playerFromId).GetPlayerColor()
                                }, hand.handRenderer);
                                hand.handRenderer.color = hand.handRenderer.color.SetAlpha(playerFromId.GetAlpha());
                            }
                        } catch {}

                        return;
                    }
                    case ActionsRPC.CallMeeting:
                    {
                        CallMeeting(reader.Read<PlayerControl>());
                        return;
                    }
                    case ActionsRPC.BaitReport:
                    {
                        reader.Read<PlayerControl>().ReportDeadBody(reader.Read<PlayerControl>().Data);
                        return;
                    }
                    case ActionsRPC.Drop:
                    {
                        reader.Read<DeadBody>().GetComponent<DeadBodyHandler>().StopDrag();
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
                        var requestor = reader.Read<PlayerControl>().GetRole();
                        requestor.Requestor.GetLayer<BountyHunter>().TentativeTarget = reader.Read<PlayerControl>();
                        requestor.Requesting = false;
                        requestor.Requestor = null;
                        return;
                    }
                    case ActionsRPC.ButtonAction:
                    {
                        reader.Read<CustomButton>().StartEffectRPC(reader);
                        return;
                    }
                    case ActionsRPC.LayerAction:
                    {
                        reader.Read<PlayerLayer>().ReadRPC(reader);
                        return;
                    }
                    case ActionsRPC.Cancel:
                    {
                        reader.Read<CustomButton>().ClickedAgain = true;
                        return;
                    }
                    case ActionsRPC.PublicReveal:
                    {
                        Role.PublicReveal(reader.Read<PlayerControl>());
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
                WinState = reader.Read<WinLose>();

                switch (WinState)
                {
                    case >= WinLose.ArsonistWins and <= WinLose.WerewolfWins:
                    {
                        var nkRole = reader.Read<Role>();

                        foreach (var role in PlayerLayer.GetLayers<Neutral>().Where(x => x.Type == nkRole.Type))
                        {
                            if (!role.Disconnected && role.Faithful)
                                role.Winner = true;
                        }

                        return;
                    }
                    case WinLose.JesterWins:
                    {
                        reader.Read<Jester>().VotedOut = true;
                        return;
                    }
                    case WinLose.CannibalWins:
                    {
                        reader.Read<Cannibal>().Eaten = true;
                        return;
                    }
                    case WinLose.ExecutionerWins:
                    {
                        reader.Read<Executioner>().TargetVotedOut = true;
                        return;
                    }
                    case WinLose.BountyHunterWins:
                    {
                        reader.Read<BountyHunter>().TargetKilled = true;
                        return;
                    }
                    case WinLose.ActorWins:
                    {
                        reader.Read<Actor>().Guessed = true;
                        return;
                    }
                    case WinLose.GuesserWins:
                    {
                        reader.Read<Guesser>().TargetGuessed = true;
                        return;
                    }
                    case WinLose.CorruptedWins:
                    {
                        if (Corrupted.AllCorruptedWin)
                            PlayerLayer.GetLayers<Corrupted>().ForEach(x => x.Winner = true);

                        reader.Read<PlayerLayer>().Winner = true;
                        return;
                    }
                    case WinLose.LoveWins:
                    {
                        var lover = reader.Read<Lovers>();
                        lover.Winner = true;
                        lover.OtherLover.GetDisposition().Winner = true;
                        return;
                    }
                    case WinLose.OverlordWins:
                    {
                        PlayerLayer.GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
                        return;
                    }
                    case WinLose.TaskmasterWins or WinLose.RivalWins:
                    {
                        reader.Read<PlayerLayer>().Winner = true;
                        return;
                    }
                    case WinLose.TaskRunnerWins:
                    {
                        reader.Read<Runner>().Winner = true;
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