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
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    /// <returns>An instance of <see cref="RpcWriter"/>.</returns>
    public static RpcWriter CreateWriter(CustomRPC rpc, params object[] data) => TownOfUsReworked.MciActive || !LocalPlayer ? null : new(rpc, false, data);

    // /// <summary>
    // /// Creates an instance of <see cref="RpcWriter"/> to potentially write more data to.
    // /// </summary>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // /// <returns>An instance of <see cref="RpcWriter"/>.</returns>
    // private static RpcWriter CreateWriterUsingTypeCodes(CustomRPC rpc, params object[] data) => TownOfUsReworked.MciActive || !LocalPlayer ? null : new(rpc, true, data);

    /// <summary>
    /// Sends an RPC message to all players.
    /// </summary>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    public static void CallRpc(CustomRPC rpc, params object[] data) => CallTargetedRpc(-1, rpc, data);

    // /// <summary>
    // /// Sends a late RPC message to all players.
    // /// </summary>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // public static void CallLateRpc(CustomRPC rpc, params object[] data) => CallLateTargetedRpc(-1, rpc, data);

    // /// <summary>
    // /// Sends an RPC message with type codes to all players.
    // /// </summary>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // public static void CallRpcUsingTypeCodes(CustomRPC rpc, params object[] data) => CallTargetedRpcUsingTypeCodes(-1, rpc, data);

    // /// <summary>
    // /// Sends a late RPC message with type codes to all players.
    // /// </summary>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // public static void CallLateRpcUsingTypeCodes(CustomRPC rpc, params object[] data) => CallLateTargetedRpcUsingTypeCodes(-1, rpc, data);

    /// <summary>
    /// Sends an RPC message to a specific player.
    /// </summary>
    /// <param name="targetClientId">The player to send the data to.</param>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    public static void CallTargetedRpc(int targetClientId, CustomRPC rpc, params object[] data)
    {
        using var writer = CreateWriter(rpc, data);
        writer?.Send(targetClientId);
    }

    // /// <summary>
    // /// Sends a late RPC message to a specific player.
    // /// </summary>
    // /// <param name="targetClientId">The player to send the data to.</param>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // public static void CallLateTargetedRpc(int targetClientId, CustomRPC rpc, params object[] data)
    // {
    //     using var writer = CreateWriter(rpc, data);
    //     writer?.SendLate(targetClientId);
    // }

    // /// <summary>
    // /// Sends an RPC message to a specific player.
    // /// </summary>
    // /// <param name="targetClientId">The player to send the data to.</param>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // private static void CallTargetedRpcUsingTypeCodes(int targetClientId, CustomRPC rpc, params object[] data)
    // {
    //     using var writer = CreateWriterUsingTypeCodes(rpc, data);
    //     writer?.Send(targetClientId);
    // }

    // /// <summary>
    // /// Sends a late RPC message with type codes to a specific player.
    // /// </summary>
    // /// <param name="targetClientId">The player to send the data to.</param>
    // /// <param name="rpc">The main rpc header.</param>
    // /// <param name="data">The data associated to the rpc.</param>
    // private static void CallLateTargetedRpcUsingTypeCodes(int targetClientId, CustomRPC rpc, params object[] data)
    // {
    //     using var writer = CreateWriterUsingTypeCodes(rpc, data);
    //     writer?.SendLate(targetClientId);
    // }

    /// <summary>
    /// Closes and sends the rpc.
    /// </summary>
    /// <param name="writer">The network writer to close and send.</param>
    public static void CloseRpc(this MessageWriter writer)
    {
        if (writer is null)
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

        if (TownOfUsReworked.MciActive || !LocalPlayer || GameData.Instance.PlayerCount <= 1)
            return;

        var options = setting is not null ? [ setting ] : Option.AllOptions.Where(x => !x.ClientOnly && x is not BaseHeaderOption);
        var count = options.Count();
        var split = count > 75 ? (IEnumerable<IEnumerable<Option>>)options.Chunk(75) : [ options ]; // No need to split if less than chunk size, saves from arbitrary computation time, but I'll be honest I like my shit fast
        Info($"Sending {count} options split to {split.Count()} sets to {targetClientId}");

        foreach (var list in split)
        {
            using var writer = CreateWriter(CustomRPC.Misc, MiscRPC.SyncCustomSettings, (byte)list.Count());

            foreach (var option in list)
            {
                // Info($"Sending {option.Name}");
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
                // Info($"Received option: {customOption.Name}");
                customOption.ReadValueRpc(reader);
            }
            else
                Failure($"No option found for id pair: {superId:X2};{id:X2}");
        }

        Option.SaveSettings("LastUsed");
    }

    /// <summary>
    /// Fetches the id associated with the provided type.
    /// </summary>
    /// <param name="type">The type to fetch the id for.</param>
    /// <returns>An id associated with the type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an invalid type was passed.</exception>
    public static byte GetIdFromType(this Type type)
    {
        if (type.IsEnum)
            return Generate.TypeIdMap[typeof(Enum)];
        else if (type.GetGenericTypeDefinition() == typeof(MultiSelectValue<>))
            return Generate.TypeIdMap[typeof(MultiSelectValue<>)];
        else if (typeof(IEnumerable).IsAssignableFrom(type))
            return Generate.TypeIdMap[typeof(IEnumerable)];
        else if (Generate.TypeIdMap.TryGetKey(type, out var dynamicId))
            return dynamicId;
        else
            throw new InvalidOperationException($"Type {type.Name} cannot be serialized with an assigned type code. It must be a primitive, game type, INetSerializable, or a dynamically registered type.");
    }

    /// <summary>
    /// Fetches the type associated with the provided id.
    /// </summary>
    /// <param name="id">The id to fetch the type for.</param>
    /// <returns>A type associated with the .</returns>
    /// <exception cref="InvalidOperationException">Thrown if an invalid id was passed.</exception>
    public static Type GetTypeFromId(this byte id)
    {
        if (Generate.TypeIdMap.TryGetValue(id, out var type))
            return type;
        else
            throw new InvalidOperationException($"ID {id} does not have an associated type.");
    }

    /// <summary>
    /// Centralized handler for all things networking within the mod.
    /// </summary>
    /// <param name="reader">The instance of <see cref="RpcReader"/> containing byte data.</param>
    /// <param name="targetClientId">The id of the client that's supposed to read from the rpc. If the value is <c>-1</c>, then everyone reads from the message.</param>
    public static void HandleRpc(RpcReader reader, int targetClientId)
    {
        if (reader?.DataSize is null or 0)
        {
            Warning("Received no data");
            return;
        }

        if (targetClientId != -1 && LocalPlayer.OwnerId != targetClientId)
            return;

        if (TownOfUsReworked.DebugMode)
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
                        reader.ReadButton().MaxUses = reader.ReadInt();
                        return;
                    }
                    case MiscRPC.SyncUses:
                    {
                        reader.ReadButton().Uses = reader.ReadInt();
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
                        else if ((LocalPlayer.Is<Blackmailer>() && Blackmailer.WhispersNotPrivateB) || DeadSeeEverything() || (LocalPlayer.Is<Silencer>() &&
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
                        reader.ReadPlayer().GetLayer<IGhosty>().Catch(reader.ReadPlayer());
                        return;
                    }
                    case MiscRPC.DoorSyncToilet:
                    {
                        var id2 = reader.ReadInt();
                        UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == id2)?.SetDoorway(true);
                        return;
                    }
                    case MiscRPC.PlayerJoinSync:
                    {
                        SettingsPatches.SetMap(reader.Read<MapEnum>());
                        Summary = reader.Read<SummaryInfo>();

                        if (reader.ReadBool())
                            CachedFirstDead = reader.ReadString();

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
                        AllPlayers().Do(x => RoleManager.Instance.SetRole(x, (RoleTypes)100));
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
                        switch (reader.ReadLayer())
                        {
                            case ITargeter targeter:
                            {
                                targeter.TargetPlayer = reader.ReadPlayer();
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
                        }

                        return;
                    }
                    case MiscRPC.ChangeRoles:
                    {
                        switch (reader.ReadLayer())
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
                    case MiscRPC.Stat:
                    {
                        CustomStatsManager.IncrementStat(reader.Read<StringNames>());
                        return;
                    }
                    // case MiscRPC.SetStatus:
                    // {
                    //     StatusUtils.AddStatusFromRpc(reader);
                    //     return;
                    // }
                    case MiscRPC.WinLose:
                    {
                        WinState = reader.Read<WinLose>();

                        while (reader.BytesRemaining > 0)
                            LayerHandler.Handlers[reader.ReadByte()].Winner = true;

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
                        ConvertPlayer(reader.ReadByte(), reader.ReadByte(), reader.ReadBool());
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
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).Do(x => x.BombSuccessful = success);
                        return;
                    }
                    case ActionsRPC.Infect:
                    {
                        Pestilence.Infected[reader.ReadByte()] = reader.ReadByte();
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
                                PlayerById(playerid).UpdateColor(UObject.FindObjectOfType<ZiplineBehaviour>().playerIdHands[playerid].handRenderer);
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
                        var button = reader.ReadButton();
                        button.ClickedAgain = true;
                        button.OnClickedAgain();
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
            default:
            {
                Failure($"Received unknown custom RPC - {rpc}");
                return;
            }
        }
    }
}