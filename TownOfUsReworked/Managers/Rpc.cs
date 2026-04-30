using TownOfUsReworked.Patches.Player;
using TownOfUsReworked.RPCs.Messages;
using TownOfUsReworked.RPCs.Messages.Misc;

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
    /// Creates an instance of <see cref="RpcWriter"/> to potentially write more data to.
    /// </summary>
    /// <param name="message">The message to start the rpc with.</param>
    /// <param name="targetClientId">The owner id of the target client.</param>
    /// <returns>An instance of <see cref="RpcWriter"/>.</returns>
    public static RpcWriter? CreateWriter(INetworkMessage message, int targetClientId = -1)
    {
        if (TownOfUsReworked.MciActive || !LocalPlayer)
            return null;

        var writer = RpcWriter.Borrow();
        writer.SetTargetId(targetClientId);
        message.SerializeTo(writer);
        message.Dispose();
        return writer;
    }

    /// <summary>
    /// Sends an RPC message to all players.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public static void CallRpc(INetworkMessage message) => CallTargetedRpc(message, -1);

    /// <summary>
    /// Sends a late RPC message to all players.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public static void CallLateRpc(INetworkMessage message) => CallLateTargetedRpc(message, -1);

    /// <summary>
    /// Sends an RPC message to a specific player.
    /// </summary>
    /// <param name="message">The message to start the rpc with.</param>
    /// <param name="targetClientId">The owner id of the target client.</param>
    public static void CallTargetedRpc(INetworkMessage message, int targetClientId)
    {
        var writer = CreateWriter(message, targetClientId);
        writer?.SendImmediate();
        RpcWriter.Return(writer!);
    }

    /// <summary>
    /// Sends a late RPC message to a specific player.
    /// </summary>
    /// <param name="message">The message to start the rpc with.</param>
    /// <param name="targetClientId">The owner id of the target client.</param>
    public static void CallLateTargetedRpc(INetworkMessage message, int targetClientId = -1)
    {
        var writer = CreateWriter(message, targetClientId);
        writer?.SendLate();
        RpcWriter.Return(writer!);
    }

    /// <summary>
    /// Closes and sends the rpc.
    /// </summary>
    /// <param name="writer">The network writer to close and send.</param>
    public static void Send(this MessageWriter writer)
    {
        if (writer is null)
            Failure("RPC writer was null");
        else
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    /// <summary>
    /// Creates an instance of the game's <see cref="MessageWriter"/>.
    /// </summary>
    /// <param name="targetClientId">The owner id of the target client.</param>
    /// <returns>An instance of <see cref="MessageWriter"/>.</returns>
    public static MessageWriter CreateMessageWriter(int targetClientId = -1)
        => AmongUsClient.Instance.StartRpcImmediately(LocalPlayer.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);

    /// <summary>
    /// Sends all non-client mod settings to other players.
    /// </summary>
    /// <param name="setting">Specific setting to sync, or <c>null</c> for all settings.</param>
    /// <param name="targetClientId">Target player ID, or <c>-1</c> for all players.</param>
    /// <param name="save">Whether to save settings after sending.</param>
    public static void SendOptionRPC(Option? setting = null, int targetClientId = -1, bool save = true)
    {
        if (save)
            Option.SaveSettings("LastUsed");

        if (TownOfUsReworked.MciActive || !LocalPlayer || GameData.Instance.PlayerCount <= 1)
            return;

        if (targetClientId == -1)
            GameStartManagerPatches.PlayersReady.Keys.Do(x => GameStartManagerPatches.PlayersReady[x] = false);
        else
            GameStartManagerPatches.PlayersReady[AllPlayers().Find(x => x.OwnerId == targetClientId)!.PlayerId] = false;

        var options = setting is not null ? [setting] : Option.AllOptions.Where(x => !x.ClientOnly && x is not BaseHeaderOption).ToArray();
        var split = options.Chunk(100).ToArray();
        Info($"Sending options split to {split.Length} sets to {targetClientId}");

        foreach (var list in split)
            CallLateTargetedRpc(new SyncSettingsMessage(list), targetClientId);
    }

    /// <summary>
    /// Processes received mod settings from other players.
    /// </summary>
    /// <param name="reader">The network message reader.</param>
    private static void ReceiveOptionRPC(RpcReader reader)
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

            if (customOption is not null)
            {
                // Info($"Received option: {customOption.Name}: {superId};{id}");
                customOption.ReadValueRpc(reader);
            }
            else
                Failure($"No option found for id pair: {superId};{id}");
        }

        Option.SaveSettings("LastUsed");
        SettingsPatches.OnValueChanged();
        SettingsPatches.OnValueChangedView();
        Coroutines.Start(TrySendReadyState());
    }

    private static IEnumerator TrySendReadyState()
    {
        while (!LocalPlayer)
            yield return null;

        GameStartManagerPatches.PlayersReady[LocalPlayer.PlayerId] = true;
        CallRpc(new SetPlayerReadyMessage(LocalPlayer));
    }

    /// <summary>
    /// Centralized handler for all things networking within the mod.
    /// </summary>
    /// <param name="reader">The instance of <see cref="RpcReader"/> containing byte data.</param>
    public static void HandleRpc(RpcReader reader)
    {
        if (reader?.DataSize is null or 0)
        {
            Warning("Received no data");
            return;
        }

        if (TownOfUsReworked.DebugMode)
            Message($"Received rpc with {reader.DataSize} bytes");

        var rpc = reader.Read<ReworkedRpc>();

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
            //             Run("<#FF00FFFF>⚠ TEST ⚠</color>", $"Received RPC!\nWith the following message: {reader.ReadString().Trim()}");
            //             return;
            //         }
            //         default:
            //         {
            //             Failure($"Received unknown test RPC - {test}");
            //             return;
            //         }
            //     }
            // }
            case ReworkedRpc.Misc:
            {
                var misc = reader.Read<MiscRpc>();

                switch (misc)
                {
                    case MiscRpc.SyncMaxUses:
                    {
                        reader.ReadButton().MaxUses = reader.ReadPackedInt();
                        return;
                    }
                    case MiscRpc.SyncUses:
                    {
                        reader.ReadButton().Uses = reader.ReadPackedInt();
                        return;
                    }
                    case MiscRpc.SetLayer:
                    {
                        RoleGenManager.GetLayer(reader.Read<Layer>(), reader.Read<PlayerLayerEnum>()).Start(reader.ReadPlayer());
                        return;
                    }
                    case MiscRpc.Whisper:
                    {
                        var whisperer = reader.ReadPlayer();
                        var whispered = reader.ReadPlayer();
                        var message = reader.ReadString().Trim();

                        if (whispered.AmOwner)
                            Run("<#4D4DFFFF>「 Whispers 」</color>", $"#({whisperer.name}) whispers to you: {message}");
                        else if ((LocalPlayer.Is<Blackmailer>() && Blackmailer.WhispersNotPrivateB) || DeadSeeEverything() || (LocalPlayer.Is<Silencer>() && Silencer.WhispersNotPrivateS))
                            Run("<#4D4DFFFF>「 Whispers 」</color>", $"#({whisperer.name}) whispers to #({whispered.name}): {message}");
                        else if (GameModifiers.WhispersAnnouncement)
                            Run("<#4D4DFFFF>「 Whispers 」</color>", $"#({whisperer.name}) is whispering to #({whispered.name}).");

                        return;
                    }
                    case MiscRpc.Start:
                    {
                        RoleGenManager.ResetEverything();
                        return;
                    }
                    case MiscRpc.BreakShield:
                    {
                        BreakShield(reader.ReadPlayer(), Medic.ShieldBreaks);
                        return;
                    }
                    case MiscRpc.BastionBomb:
                    {
                        BastionBomb(reader.ReadVent(), Bastion.BombRemovedOnKill);
                        return;
                    }
                    case MiscRpc.Catch:
                    {
                        reader.ReadPlayer().GetLayer<IGhosty>().Catch(reader.ReadPlayer());
                        return;
                    }
                    case MiscRpc.DoorSyncToilet:
                    {
                        var id2 = reader.ReadInt();
                        UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == id2)?.SetDoorway(true);
                        return;
                    }
                    case MiscRpc.PlayerJoinSync:
                    {
                        SettingsPatches.SetMap(reader.Read<Map>());
                        var summary = reader.ReadPackedBool();
                        var cachedFirstDead = reader.ReadPackedBool();

                        if (summary)
                            Summary = reader.Read<SummaryInfo>();

                        if (cachedFirstDead)
                            CachedFirstDead = reader.ReadString();

                        return;
                    }
                    case MiscRpc.SubmergedFixOxygen:
                    {
                        RepairOxygen();
                        return;
                    }
                    case MiscRpc.FixLights:
                    {
                        var lights = Ship()!.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        return;
                    }
                    case MiscRpc.FixMixup:
                    {
                        var mixup = Ship()!.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                        mixup.secondsForAutoHeal = 0.1f;
                        return;
                    }
                    case MiscRpc.ChaosDrive:
                    {
                        Syndicate.DriveHolder = reader.ReadPlayer();
                        Syndicate.SyndicateHasChaosDrive = Syndicate.DriveHolder;
                        return;
                    }
                    case MiscRpc.SyncCustomSettings:
                    {
                        Holders.EnsureCount();
                        ReceiveOptionRPC(reader);
                        return;
                    }
                    case MiscRpc.SyncMap:
                    {
                        SettingsPatches.SetMap(reader.Read<Map>());
                        return;
                    }
                    case MiscRpc.SetSettings:
                    {
                        MapPatches.CurrentMap = reader.ReadByte();

                        try
                        {
                            TownOfUsReworked.NormalOptions.MapId = MapPatches.CurrentMap;
                        }
                        catch { }

                        try
                        {
                            TownOfUsReworked.HnsOptions.MapId = MapPatches.CurrentMap;
                        }
                        catch { }

                        MapPatches.SetDefaults();
                        MapPatches.AdjustSettings(true);
                        return;
                    }
                    case MiscRpc.BodyLocation:
                    {
                        BodyLocations[reader.ReadByte()] = reader.ReadString();
                        return;
                    }
                    case MiscRpc.LoadPreset:
                    {
                        var preset = reader.ReadString();
                        Run("<#00CC99FF>【 Loading Preset 】</color>", $"Loading the {preset} preset!");
                        SettingsPatches.CurrentPreset = preset;
                        return;
                    }
                    case MiscRpc.EndRoleGen:
                    {
                        SetPostmortals.Revealers = reader.ReadByte();
                        SetPostmortals.Phantoms = reader.ReadByte();
                        SetPostmortals.Banshees = reader.ReadByte();
                        SetPostmortals.Ghouls = reader.ReadByte();
                        RoleGenManager.Pure = reader.ReadPlayer();
                        RoleGenManager.Convertible = reader.ReadByte();
                        Retributionist.Exists = reader.ReadBool();

                        while (reader.BytesRemaining > 0)
                            BetterAirship.SpawnPoints.Add(reader.ReadByte());

                        var allPlayers = AllPlayers();
                        allPlayers.Do(x => RoleManager.Instance.SetRole(x, LayerHandler.Type));
                        Shifter.Originals.AddRange(allPlayers.Where(x => x.Is<Shifter>()));

                        if (BetterSabotages.OxySlow)
                            ISpeedModifier.AllModifiers.Add(new OxySabSpeedModifier());

                        ISpeedModifier.AllModifiers.Add(new BodyDraggingModifier());

                        AmongUsClient.Instance.StartCoroutine(HUD()!.CoShowIntro());
                        return;
                    }
                    case MiscRpc.SetTarget:
                    {
                        var layer = reader.ReadLayer();

                        switch (layer)
                        {
                            case ITargeter targeter:
                            {
                                targeter.TargetPlayer = reader.ReadPlayer();
                                break;
                            }
                            case Actor actor:
                            {
                                reader.PopulateList(actor.PretendRoles, RpcReaderDels.Enum<Layer>.Reader);
                                break;
                            }
                            case Allied ally:
                            {
                                ally.Side = reader.Read<Faction>();
                                break;
                            }
                            case Paired paired1:
                            {
                                var paired2 = reader.Read<Paired>();
                                paired1.Other = paired2.Player;
                                paired2.Other = paired1.Player;
                                break;
                            }
                            default:
                            {
                                Failure($"Received unknown layer - {layer.Type}");
                                break;
                            }
                        }

                        return;
                    }
                    case MiscRpc.ChangeRoles:
                    {
                        var layer = reader.ReadLayer();

                        switch (layer)
                        {
                            case FactionChanger changer:
                            {
                                if (reader.ReadBool())
                                    changer.TurnBetrayer();
                                else
                                    changer.TurnFaction(reader.Read<Faction>());

                                break;
                            }
                            case Defector defector:
                            {
                                defector.TurnSides(reader.Read<Faction>());
                                break;
                            }
                            case Actor act:
                            {
                                act.TurnRole(reader.Read<Layer>());
                                break;
                            }
                            default:
                            {
                                Failure($"Received unknown layer - {layer.Type}");
                                break;
                            }
                        }

                        return;
                    }
                    case MiscRpc.Achievement:
                    {
                        CustomAchievementManager.UnlockAchievement(reader.ReadString());
                        return;
                    }
                    case MiscRpc.Stat:
                    {
                        CustomStatsManager.IncrementStat(reader.Read<StringNames>());
                        return;
                    }
                    // case MiscRPC.SetStatus:
                    // {
                    //     StatusUtils.AddStatusFromRpc(reader);
                    //     return;
                    // }
                    case MiscRpc.WinLose:
                    {
                        WinState = reader.Read<WinLose>();

                        while (reader.BytesRemaining > 0)
                            LayerHandler.Handlers[reader.ReadByte()].Winner = true;

                        return;
                    }
                    case MiscRpc.SetPlayerReady:
                    {
                        GameStartManagerPatches.PlayersReady[reader.ReadByte()] = true;
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown misc RPC - {misc}");
                        return;
                    }
                }
            }
            case ReworkedRpc.Vanilla:
            {
                var vanilla = reader.Read<VanillaRpc>();

                switch (vanilla)
                {
                    case VanillaRpc.SnapTo:
                    {
                        reader.ReadPlayer().CustomSnapTo(reader.ReadVector2());
                        return;
                    }
                    case VanillaRpc.SetColor:
                    {
                        reader.ReadPlayer().SetColor(reader.ReadByte());
                        return;
                    }
                    case VanillaRpc.Report:
                    {
                        PlayerControlPatches.ReportDeadBody(reader.ReadPlayer(), GameData.Instance.GetPlayerById(reader.ReadByte()));
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown vanilla RPC - {vanilla}");
                        return;
                    }
                }
            }
            case ReworkedRpc.Action:
            {
                var action = reader.Read<ActionsRpc>();

                switch (action)
                {
                    case ActionsRpc.FadeBody:
                    {
                        FadeBody(reader.ReadDeadBody());
                        return;
                    }
                    case ActionsRpc.Convert:
                    {
                        ConvertPlayer(reader.ReadByte(), reader.ReadByte(), reader.ReadBool());
                        return;
                    }
                    case ActionsRpc.CustomKill:
                    {
                        reader.ReadPlayer().MurderPlayer(reader.ReadPlayer(), reader.Read<DeathReasonEnum>(), reader.ReadBool());
                        return;
                    }
                    case ActionsRpc.ForceKill:
                    {
                        var victim = reader.ReadPlayer();
                        var success = reader.ReadBool();
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).Do(x => x.BombSuccessful = success);
                        return;
                    }
                    case ActionsRpc.Infect:
                    {
                        Pestilence.Infected[reader.ReadByte()] = reader.ReadByte();
                        return;
                    }
                    case ActionsRpc.SetUninteractable:
                    {
                        try
                        {
                            var playerId = reader.ReadByte();
                            UninteractablePlayers.TryAdd(playerId, Time.time);
                            UninteractablePlayers2.TryAdd(playerId, reader.ReadFloat());

                            if (reader.ReadBool())
                                PlayerById(playerId).UpdateColor(UObject.FindObjectOfType<ZiplineBehaviour>().playerIdHands[playerId].handRenderer);
                        }
                        catch (Exception e)
                        {
                            Error(e);
                        }

                        return;
                    }
                    case ActionsRpc.Drop:
                    {
                        reader.ReadDeadBody().GetComponent<DeadBodyHandler>().StopDrag();
                        return;
                    }
                    case ActionsRpc.Burn:
                    {
                        var bodies = AllBodies();

                        while (reader.BytesRemaining > 0)
                        {
                            var id = reader.ReadByte();

                            if (bodies.TryFinding(x => x.ParentId == id, out var body))
                                Ash.CreateAsh(body);
                        }

                        return;
                    }
                    case ActionsRpc.PlaceHit:
                    {
                        if (LayerHandler.Handlers.TryGetValue(reader.ReadByte(), out var handler))
                        {
                            handler.Requestor.TentativeTarget = reader.ReadPlayer();
                            handler.Requestor = null!;
                        }

                        return;
                    }
                    case ActionsRpc.ButtonAction:
                    {
                        reader.ReadButton().StartEffectRPC(reader);
                        return;
                    }
                    case ActionsRpc.LayerAction:
                    {
                        reader.ReadLayer().ReadRPC(reader);
                        return;
                    }
                    case ActionsRpc.Cancel:
                    {
                        reader.ReadButton().AfterClickedAgain();
                        return;
                    }
                    case ActionsRpc.PublicReveal:
                    {
                        PublicReveal(reader.ReadPlayer());
                        return;
                    }
                    default:
                    {
                        Failure($"Received unknown action RPC - {action}");
                        return;
                    }
                }
            }
            default:
            {
                Failure($"Received unknown custom RPC - {rpc}");
                return;
            }
        }
    }
}