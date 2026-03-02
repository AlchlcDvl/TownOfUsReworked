namespace TownOfUsReworked.Modules;

[Serializable]
public sealed class SummaryInfo : INetSerializable, INetDeserializable, IDisposable
{
    public readonly List<SummaryInfoModule> Modules = [];

    public void SerializeTo(RpcWriter writer) => writer.WriteList(Modules, RpcWriterDels.NetObject<SummaryInfoModule>.Writer);

    public void DeserializeFrom(RpcReader reader) => reader.PopulateList(Modules, RpcReaderDels.NetObject<SummaryInfoModule>.Reader);

    public string Generate()
    {
        var result = string.Empty;
        Modules.ForEach(x => result += $"{x.Summarise().FullSummary}\n");
        return result;
    }

    public void Dispose() => Modules.FullClear();
}

[Serializable]
public sealed class SummaryInfoModule : INetSerializable, INetDeserializable, IDisposable
{
    public string PlayerName;

    public readonly List<(Layer, Faction)> History = [];
    public readonly List<Layer> OtherLayers = [];

    public bool IsGaTarget;
    public bool IsExeTarget;
    public bool IsBhTarget;
    public bool IsGuessTarget;
    public bool IsDriveHolder;

    public bool CanDoTasks;
    public bool TasksDone;
    public byte CompletedTasks;
    public byte TotalTasks;

    public DeathReasonEnum DeathReason;
    public string KilledBy;

    public bool Disconnected;

    public int ColorId;
    public string HatId;
    public string SkinId;
    public string VisorId;
    public Color32 Color;

    public void SerializeTo(RpcWriter writer)
    {
        writer.WriteString(PlayerName);

        // In an ideal game, the count wouldn't exceed 6 or 7. But since I know some people who like to see me suffer, so I've set the max limit to 2^16 - 1, good luck changing roles and factions this many times
        writer.WriteList(History, RpcWriterDels.Tuple<Layer, Faction>.Writer, CountType.UShort);

        writer.WriteList(OtherLayers, RpcWriterDels.Enum<Layer>.Writer);

        writer.WritePackedBool(IsGaTarget);
        writer.WritePackedBool(IsExeTarget);
        writer.WritePackedBool(IsBhTarget);
        writer.WritePackedBool(IsGuessTarget);
        writer.WritePackedBool(IsDriveHolder);

        writer.WritePackedBool(Disconnected);
        writer.WritePackedBool(CanDoTasks);

        if (CanDoTasks)
            writer.WritePackedBool(TasksDone);
        else
            writer.EndPackingBools();

        if (CanDoTasks && !TasksDone)
        {
            writer.WriteByte(CompletedTasks);
            writer.WriteByte(TotalTasks);
        }

        if (Disconnected)
            return;

        writer.WriteEnum(DeathReason);

        if (DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Suicide or DeathReasonEnum.Ejected or DeathReasonEnum.Misfire or DeathReasonEnum.Escaped))
            writer.WriteString(KilledBy);

        writer.WriteString(HatId);
        writer.WriteString(SkinId);
        writer.WriteString(VisorId);
        writer.WriteInt(ColorId);

        if (ColorId == -2)
            writer.WriteColor32(Color);
    }

    public void DeserializeFrom(RpcReader reader)
    {
        PlayerName = reader.ReadString();

        reader.PopulateList(History, RpcReaderDels.Tuple<Layer, Faction>.Reader, CountType.UShort);

        reader.PopulateList(OtherLayers, RpcReaderDels.Enum<Layer>.Reader);

        IsGaTarget = reader.ReadPackedBool();
        IsExeTarget = reader.ReadPackedBool();
        IsBhTarget = reader.ReadPackedBool();
        IsGuessTarget = reader.ReadPackedBool();
        IsDriveHolder = reader.ReadPackedBool();

        Disconnected = reader.ReadPackedBool();
        CanDoTasks = reader.ReadPackedBool();

        if (CanDoTasks)
            TasksDone = reader.ReadPackedBool();

        if (CanDoTasks && !TasksDone)
        {
            CompletedTasks = reader.ReadByte();
            TotalTasks = reader.ReadByte();
        }

        if (Disconnected)
            return;

        DeathReason = reader.ReadEnum<DeathReasonEnum>();

        if (DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Suicide or DeathReasonEnum.Ejected or DeathReasonEnum.Misfire or DeathReasonEnum.Escaped))
            KilledBy = reader.ReadString();

        HatId = reader.ReadString();
        SkinId = reader.ReadString();
        VisorId = reader.ReadString();
        ColorId = reader.ReadInt();
        Color = ColorId switch
        {
            -2 => reader.ReadColor32(),
            -1 => UColor.white,
            _ => ColorId.GetColor(false)
        };
    }

    public void PopulateFromPlayer(PlayerControl player, bool disconnected)
    {
        if (!player || !player.Data)
            return;

        PlayerName = player.Data.PlayerName;

        var handler = LayerHandler.Handlers[player.PlayerId];
        var role = handler.CurrentRole;
        var modifier = handler.CurrentModifier;
        var ability = handler.CurrentAbility;
        var disposition = handler.CurrentDisposition;

        foreach (var (role2, faction) in handler.History)
        {
            if (role2 != Layer.NoneRole)
                History.Add((role2, faction));
        }

        History.Add((role.Type, handler.CurrentFaction));

        OtherLayers.Add(disposition.Type);
        OtherLayers.Add(modifier.Type);
        OtherLayers.Add(ability.Type);

        IsGaTarget = player.IsGaTarget();
        IsExeTarget = player.IsExeTarget();
        IsBhTarget = player.IsBhTarget();
        IsGuessTarget = player.IsGuessTarget();
        IsDriveHolder = player == Syndicate.DriveHolder;

        CanDoTasks = player.CanDoTasks() && role;

        if (CanDoTasks)
        {
            TasksDone = role.TasksDone;

            if (!TasksDone)
            {
                CompletedTasks = (byte)role.TasksCompleted;
                TotalTasks = (byte)role.TotalTasks;
            }
        }

        Disconnected = disconnected;

        if (!Disconnected)
        {
            DeathReason = handler.DeathReason;

            if (handler.DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Ejected or DeathReasonEnum.Suicide or DeathReasonEnum.Escaped) && !IsNullEmptyOrWhiteSpace(handler.KilledBy))
                KilledBy = handler.KilledBy;
        }

        var appearance = player.GetComponent<AppearanceHandler>();
        HatId = appearance.Default.HatId;
        SkinId = appearance.Default.SkinId;
        VisorId = appearance.Default.VisorId;
        ColorId = appearance.Default.ColorId;
        Color = appearance.Default.Color;
    }

    public (string FullSummary, string Summary) Summarise()
    {
        var full = string.Empty;
        var summary = string.Empty;

        foreach (var (role, faction) in History)
        {
            if (!LayerDictionary.TryGetValue(role, out var entry))
                continue;

            var part2 = string.Empty;

            if (FactionDictionary.TryGetValue(faction, out var entry2))
                part2 = $"<#{entry2.Color.ToHtmlStringRGBA()}>{entry2.Name}</color>";

            full += $"<#{entry.Color.ToHtmlStringRGBA()}>{entry.Name}</color> ({part2}) → ";
        }

        if (History.Count > 0)
        {
            var (role, faction) = History[^1];

            if (LayerDictionary.TryGetValue(role, out var entry))
            {
                var factionName = string.Empty;

                if (FactionDictionary.TryGetValue(faction, out var entry2))
                    factionName = $"<#{entry2.Color.ToHtmlStringRGBA()}>{entry2.Name}</color>";

                summary += $"<#{entry.Color.ToHtmlStringRGBA()}>{entry.Name}</color> ({factionName})";
            }
        }

        var part = string.Empty;

        foreach (var layer in OtherLayers)
        {
            if (!LayerDictionary.TryGetValue(layer, out var entry))
                continue;

            var type = layer.GetLayerType();
            var (opening, ending) = type switch
            {
                PlayerLayerEnum.Disposition => (string.Empty, string.Empty),
                PlayerLayerEnum.Ability => ("(", ")"),
                _ => ("{", "}"),
            };
            part += $" {opening}<#{entry.Color.ToHtmlStringRGBA()}>{(type == PlayerLayerEnum.Disposition ? entry.Symbol : entry.Name)}</color>{ending}";
        }

        if (IsGaTarget)
            part += " <#FFFFFFFF>★</color>";

        if (IsExeTarget)
            part += " <#CCCCCCFF>§</color>";

        if (IsBhTarget)
            part += " <#B51E39FF>Θ</color>";

        if (IsGuessTarget)
            part += " <#EEE5BEFF>π</color>";

        if (IsDriveHolder)
            part += " <#008000FF>Δ</color>";

        part += " ";

        if (CanDoTasks)
            part += TasksDone ? $"{(char)0x25A0}" : $"<{CompletedTasks}/{TotalTasks}>";

        if (DeathReason != DeathReasonEnum.Alive && !Disconnected)
        {
            part += $" | {DeathReason}";

            if (DeathReason is not (DeathReasonEnum.Ejected or DeathReasonEnum.Suicide or DeathReasonEnum.Escaped) && !IsNullEmptyOrWhiteSpace(KilledBy))
                part += KilledBy;
        }

        full += part;
        summary += part;

        return (full, summary);
    }

    public void Dispose()
    {
        History.Clear();
        OtherLayers.Clear();
    }
}