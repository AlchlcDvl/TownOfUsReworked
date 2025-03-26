namespace TownOfUsReworked.Managers;

/// <summary>
/// Handles Remote Procedure Calls (RPCs) for network synchronization.
/// </summary>
public static class RpcManager
{
    /// <summary>
    /// Custom RPC identifier for TownOfUsReworked mod messages.
    /// </summary>
    public const byte CustomRPCCallID = 254;

    /// <summary>
    /// Creates a writer instance of <see cref="NetData"/> to potentially write more data to.
    /// </summary>
    /// <param name="data">The data values to serialize.</param>
    /// <returns>A writer instance of <see cref="NetData"/>.</returns>
    public static NetData CreateWriter(CustomRPC rpc, params object[] data) => TownOfUsReworked.MciActive || !CustomPlayer.Local ? null : new(rpc, data);

    /// <summary>
    /// Sends an RPC message to all players.
    /// </summary>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the id.</param>
    public static void CallRpc(CustomRPC rpc, params object[] data) => CallTargetedRpc(-1, rpc, data);

    /// <summary>
    /// Sends an RPC message to a specific player.
    /// </summary>
    /// <param name="targetClientId">The player to send the data to.</param>
    /// <inheritdoc cref="CallRpc"/>
    public static void CallTargetedRpc(int targetClientId, CustomRPC rpc, params object[] data) => CreateWriter(rpc, data)?.Send(targetClientId);

    /// <summary>
    /// Closes and sends the rpc.
    /// </summary>
    /// <param name="writer">The network writer to close and send.</param>
    public static void CloseRpc(this MessageWriter writer)
    {
        if (writer == null)
            Failure("RPC writer was null");
        else
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    /// <summary>
    /// Sends all non-client mod settings to other players.
    /// </summary>
    /// <param name="setting">Specific setting to sync, or <c>null</c> for all settings.</param>
    /// <param name="targetClientId">Target player ID, or <c>-1</c> for all players.</param>
    /// <param name="save">Whether to save settings after sending.</param>
    public static void SendOptionRPC(Option setting = null, int targetClientId = -1, bool save = true)
    {
        if (save)
            Option.SaveSettings("LastUsed");

        if (TownOfUsReworked.MciActive || !CustomPlayer.Local || GameData.Instance.PlayerCount <= 1)
            return;

        var options = setting != null ? [ setting ] : Option.AllOptions.Where(x => !x.ClientOnly && x is not BaseHeaderOption);
        var split = options.Split(70);
        Info($"Sending {options.Count()} options split to {split.Count} sets to {targetClientId}");

        foreach (var list in split)
        {
            var writer = CreateWriter(CustomRPC.Misc, MiscRPC.SyncCustomSettings, (byte)list.Count);

            foreach (var option in list)
            {
                // Info($"Sending {option}");
                writer.Write(option.RpcId.Key);
                writer.Write(option.RpcId.Value);
                option.WriteValueRpc(writer);
            }

            writer.Send(targetClientId);
        }
    }

    /// <summary>
    /// Processes received mod settings from other players.
    /// </summary>
    /// <param name="reader">The network message reader.</param>
    public static void ReceiveOptionRPC(NetData reader)
    {
        if (TownOfUsReworked.MciActive)
            return;

        var count = reader.ReadByte();
        Info($"{count} options received:");

        while (count-- > 0)
        {
            var superId = reader.ReadByte();
            var id = reader.ReadByte();
            var customOption = Option.GetOption(superId, id);

            if (customOption != null)
            {
                Info($"Received option: {customOption}");
                customOption.ReadValueRpc(reader);
            }
            else
                Failure($"No option found for id pair: {superId:X2};{id:X2}");
        }

        Option.SaveSettings("LastUsed");
    }

    /// <summary>
    /// Centralized handler for all things networking within the mod.
    /// </summary>
    /// <param name="reader">The reader instance of <see cref="NetData"/> containing byte data.</param>
    public static void HandleRpc(NetData reader)
    {
        if (reader == null || reader.DataSize == 0)
        {
            Warning("Received no data");
            return;
        }

        if (TownOfUsReworked.IsDev)
            Message($"Received rpc with {reader.DataSize} bytes");

        var rpc = reader.Read<CustomRPC>();

        switch (rpc)
        {
            // Debugging stuff, uncomment to test features
            // case CustomRPC.Test:
            // {
            //     Message("Received RPC!");
            //     var test = reader.Read<TestRPC>();

            //     switch (test)
            //     {
            //         case TestRPC.Argless:
            //         {
            //             Run("<#FF00FFFF>⚠ TEST ⚠</color>", "Received RPC!");
            //             return;
            //         }
            //         case TestRPC.Args:
            //         {
            //             var message = "";

            //             while (reader.BytesRemaining > 0)
            //                 message += $"{reader.ReadString()} ";

            //             Run("<#FF00FFFF>⚠ TEST ⚠</color>", $"Received RPC!\nWith the following message: {message.Trim()}");
            //             return;
            //         }
            //         default:
            //         {
            //             Failure($"Received unknown test RPC - {test}");
            //             return;
            //         }
            //     }
            // }
            case CustomRPC.Misc:
            {
                var misc = reader.Read<MiscRPC>();

                switch (misc)
                {
                    case MiscRPC.SyncMaxUses:
                    {
                        reader.Read<CustomButton>().MaxUses = reader.ReadInt();
                        return;
                    }
                    case MiscRPC.SyncUses:
                    {
                        reader.Read<CustomButton>().Uses = reader.ReadInt();
                        return;
                    }
                    case MiscRPC.SetLayer:
                    {
                        RoleGenManager.GetLayer(reader.Read<LayerEnum>(), reader.Read<PlayerLayerEnum>()).Start(reader.ReadPlayer());
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
                        Role.BastionBomb(reader.Read<Vent>(), Bastion.BombRemovedOnKill);
                        return;
                    }
                    case MiscRPC.Catch:
                    {
                        PlayerControlOnClick.CatchPostmortal(reader.ReadPlayer(), reader.ReadPlayer());
                        return;
                    }
                    case MiscRPC.DoorSyncToilet:
                    {
                        var id2 = reader.ReadInt();
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
                        BetterAirship.SpawnPoints.AddRange(reader.ReadValues<byte>());
                        AmongUsClient.Instance.StartCoroutine(HUD().CoShowIntro());
                        return;
                    }
                    case MiscRPC.SetTarget:
                    {
                        switch (reader.Read<PlayerLayer>())
                        {
                            case Executioner exe:
                            {
                                exe.TargetPlayer = reader.ReadPlayer();
                                break;
                            }
                            case Guesser guesser:
                            {
                                guesser.TargetPlayer = reader.ReadPlayer();
                                break;
                            }
                            case GuardianAngel angel:
                            {
                                angel.TargetPlayer = reader.ReadPlayer();
                                break;
                            }
                            case BountyHunter hunter:
                            {
                                hunter.TargetPlayer = reader.ReadPlayer();
                                break;
                            }
                            case Actor actor:
                            {
                                actor.PretendRoles.Clear();
                                actor.PretendRoles.AddRange(reader.ReadValues<Role>());
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
                        switch (reader.Read<PlayerLayer>())
                        {
                            case Traitor traitor:
                            {
                                if (reader.ReadBool())
                                    traitor.TurnBetrayer();
                                else
                                    traitor.TurnTraitor(reader.ReadBool(), reader.ReadBool());

                                break;
                            }
                            case Defector defector:
                            {
                                defector.TurnSides(reader.ReadBool(), reader.ReadBool(), reader.ReadBool());
                                break;
                            }
                            case Fanatic fanatic:
                            {
                                if (reader.ReadBool())
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
                var action = reader.Read<ActionsRPC>();

                switch (action)
                {
                    case ActionsRPC.FadeBody:
                    {
                        FadeBody(reader.ReadBody());
                        return;
                    }
                    case ActionsRPC.Convert:
                    {
                        ConvertPlayer(reader.ReadByte(), reader.ReadByte(), reader.Read<SubFaction>(), reader.ReadBool());
                        return;
                    }
                    case ActionsRPC.BypassKill:
                    {
                        reader.ReadPlayer().MurderPlayer(reader.ReadPlayer(), reader.Read<DeathReasonEnum>(), reader.ReadBool());
                        return;
                    }
                    case ActionsRPC.ForceKill:
                    {
                        var victim = reader.ReadPlayer();
                        var success = reader.ReadBool();
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).ForEach(x => x.BombSuccessful = success);
                        return;
                    }
                    case ActionsRPC.Infect:
                    {
                        Pestilence.Infected[reader.ReadByte()] = reader.ReadUInt();
                        return;
                    }
                    case ActionsRPC.SetUninteractable:
                    {
                        try
                        {
                            var playerid = reader.ReadByte();
                            UninteractablePlayers.TryAdd(playerid, Time.time);
                            UninteractablePlayers2.TryAdd(playerid, reader.ReadFloat());

                            if (reader.ReadBool())
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
                        reader.ReadBody().GetComponent<DeadBodyHandler>().StopDrag();
                        return;
                    }
                    case ActionsRPC.Burn:
                    {
                        var disappear = reader.ReadValues<byte>();

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