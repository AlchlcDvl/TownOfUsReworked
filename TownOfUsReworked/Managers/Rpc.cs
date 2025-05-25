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
    /// Gets the custom type code associated with the value's type.
    /// </summary>
    /// <param name="value">The value whose custom type code must be fetched.</param>
    /// <returns>A type code enum value that represents the type of the value.</returns>
    public static CustomTypeCode GetCustomTypeCode(this object value) => value switch
    {
        // Types from the base game
        PlayerControl => CustomTypeCode.PlayerControl,
        DeadBody => CustomTypeCode.DeadBody,
        PlayerVoteArea => CustomTypeCode.PlayerVoteArea,
        Vent => CustomTypeCode.Vent,
        Vector2 => CustomTypeCode.Vector2,

        // Custom types using the interface
        INetSerializable i => i.TypeCode,

        // Base C# types
        char => CustomTypeCode.Char,
        bool => CustomTypeCode.Boolean,
        byte => CustomTypeCode.Byte,
        sbyte => CustomTypeCode.SByte,
        ushort => CustomTypeCode.UShort,
        short => CustomTypeCode.Short,
        int => CustomTypeCode.Int,
        uint => CustomTypeCode.UInt,
        ulong => CustomTypeCode.ULong,
        long => CustomTypeCode.Long,
        float => CustomTypeCode.Float,
        double => CustomTypeCode.Double,
        Half => CustomTypeCode.Half,
        decimal => CustomTypeCode.Decimal,
        string => CustomTypeCode.String,
        Enum => CustomTypeCode.Enum,
        Type => CustomTypeCode.Type,

        // WIP
        // IDictionary i => CustomTypeCode.IDictionary,
        // ICollection i => CustomTypeCode.ICollection,

        // Special cases
        Array => CustomTypeCode.Array,
        IEnumerable => CustomTypeCode.IEnumerable,

        // Edge cases
        null => throw new ArgumentNullException(nameof(value)),
        _ => throw new NotSupportedException($"Either {value.GetType().Name} does not extend INetSerializable, or does not have a relevant custom type code for it")
    };

    /// <summary>
    /// Gets the type associated with a custom type code.
    /// </summary>
    /// <param name="typeCode">The custom type code to resolve.</param>
    /// <returns>The type associated with the custom type code.</returns>
    /// <exception cref="NotSupportedException">Thrown when the type code cannot be resolved to a type.</exception>
    public static Type ToType(this CustomTypeCode typeCode) => typeCode switch
    {
        // Types from the base game
        CustomTypeCode.PlayerControl => typeof(PlayerControl),
        CustomTypeCode.DeadBody => typeof(DeadBody),
        CustomTypeCode.PlayerVoteArea => typeof(PlayerVoteArea),
        CustomTypeCode.Vent => typeof(Vent),
        CustomTypeCode.Vector2 => typeof(Vector2),

        // Base C# types
        CustomTypeCode.Char => typeof(char),
        CustomTypeCode.Boolean => typeof(bool),
        CustomTypeCode.Byte => typeof(byte),
        CustomTypeCode.SByte => typeof(sbyte),
        CustomTypeCode.UShort => typeof(ushort),
        CustomTypeCode.Short => typeof(short),
        CustomTypeCode.Int => typeof(int),
        CustomTypeCode.UInt => typeof(uint),
        CustomTypeCode.ULong => typeof(ulong),
        CustomTypeCode.Long => typeof(long),
        CustomTypeCode.Float => typeof(float),
        CustomTypeCode.Double => typeof(double),
        CustomTypeCode.Half => typeof(Half),
        CustomTypeCode.Decimal => typeof(decimal),
        CustomTypeCode.String => typeof(string),
        CustomTypeCode.Enum => typeof(Enum),
        CustomTypeCode.Type => typeof(Type),

        // Custom classes
        CustomTypeCode.NetData => typeof(NetData),
        CustomTypeCode.Button => typeof(CustomButton),
        CustomTypeCode.Number => typeof(Number),
        CustomTypeCode.RoleOptionData => typeof(RoleOptionData),
        CustomTypeCode.PlayerLayer => typeof(PlayerLayer),

        // Edge cases
        _ => throw new NotSupportedException($"Custom type code {typeCode} cannot be resolved to a type")
    };

    /// <summary>
    /// Creates a writer instance of <see cref="NetData"/> to potentially write more data to.
    /// </summary>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    /// <returns>A writer instance of <see cref="NetData"/>.</returns>
    public static NetData CreateWriter(CustomRPC rpc, params object[] data) => TownOfUsReworked.MciActive || !CustomPlayer.Local ? null : new(rpc, false, data);

    /// <summary>
    /// Creates a writer instance of <see cref="NetData"/> to potentially write more data to.
    /// </summary>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    /// <returns>A writer instance of <see cref="NetData"/>.</returns>
    public static NetData CreateWriterUsingTypeCodes(CustomRPC rpc, params object[] data) => TownOfUsReworked.MciActive || !CustomPlayer.Local ? null : new(rpc, true, data);

    /// <summary>
    /// Sends an RPC message to all players.
    /// </summary>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    public static void CallRpc(CustomRPC rpc, params object[] data) => CallTargetedRpc(-1, rpc, data);

    /// <summary>
    /// Sends an RPC message to all players.
    /// </summary>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    public static void CallRpcUsingTypeCodes(CustomRPC rpc, params object[] data) => CallTargetedRpcUsingTypeCodes(-1, rpc, data);

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

    /// <summary>
    /// Sends an RPC message to a specific player.
    /// </summary>
    /// <param name="targetClientId">The player to send the data to.</param>
    /// <param name="rpc">The main rpc header.</param>
    /// <param name="data">The data associated to the rpc.</param>
    public static void CallTargetedRpcUsingTypeCodes(int targetClientId, CustomRPC rpc, params object[] data)
    {
        using var writer = CreateWriterUsingTypeCodes(rpc, data);
        writer?.Send(targetClientId);
    }

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

        if (TownOfUsReworked.MciActive || !CustomPlayer.Local || GameData.Instance.PlayerCount <= 1)
            return;

        var options = setting is not null ? [ setting ] : Option.AllOptions.Where(x => !x.ClientOnly && x is not BaseHeaderOption);
        var count = options.Count();
        var split = count > 75 ? options.Chunk(75) : [ [.. options] ]; // No need to split if less than chunk size, saves from arbitrary computation time, but I'll be honest I like my shit fast
        Info($"Sending {count} options split to {split.Count()} sets to {targetClientId}");

        foreach (var list in split)
        {
            using var writer = CreateWriter(CustomRPC.Misc, MiscRPC.SyncCustomSettings, (byte)list.Length);

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
    private static void ReceiveOptionRPC(NetData reader)
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
    /// Centralized handler for all things networking within the mod.
    /// </summary>
    /// <param name="reader">The reader instance of <see cref="NetData"/> containing byte data.</param>
    public static void HandleRpc(NetData reader)
    {
        if (reader is null || reader.DataSize == 0)
        {
            Warning("Received no data");
            return;
        }

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
                    case MiscRPC.SetFirstKilled:
                    {
                        CachedFirstDead = reader.ReadString();
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
                                lover1.Other = lover2.Player;
                                lover2.Other = lover1.Player;
                                break;
                            }
                            case Rivals rival1:
                            {
                                var rival2 = reader.Read<Rivals>();
                                rival1.Other = rival2.Player;
                                rival2.Other = rival1.Player;
                                break;
                            }
                            case Linked link1:
                            {
                                var link2 = reader.Read<Linked>();
                                link1.Other = link2.Player;
                                link2.Other = link1.Player;
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
                    case MiscRPC.SetStatus:
                    {
                        StatusUtils.AddStatusFromRpc(reader);
                        return;
                    }
                    case MiscRPC.WinLose:
                    {
                        WinState = reader.Read<WinLose>();

                        while (reader.BytesRemaining > 0)
                            reader.ReadPlayer().GetLayers().Do(x => x.Winner = true);

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
                        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == victim).Do(x => x.BombSuccessful = success);
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
            default:
            {
                Failure($"Received unknown custom RPC - {rpc}");
                return;
            }
        }
    }
}