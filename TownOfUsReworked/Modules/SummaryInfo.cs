namespace TownOfUsReworked.Modules;

[Serializable]
public sealed class SummaryInfo : INetSerializable, INetDeserializable, IDisposable
{
    ~SummaryInfo() => Modules.Clear();

    public readonly List<SummaryInfoModule> Modules = [];

    public IEnumerable<byte> GetBytes() => [(byte)Modules.Count, .. Modules.SelectMany(x => x.GetBytes())];

    public void FromBytes(RpcReader netData)
    {
        var count = netData.ReadByte();

        while (count-- > 0)
        {
            var module = new SummaryInfoModule();
            module.FromBytes(netData);
            Modules.Add(module);
        }
    }

    public string Generate()
    {
        var result = string.Empty;
        Modules.ForEach(x => result += $"{x.Summarise().FullSummary}\n");
        return result;
    }

    public void Dispose()
    {
        Modules.Clear();
        GC.SuppressFinalize(this);
    }
}

[Serializable]
public record struct SummaryInfoModule() : INetSerializable, INetDeserializable
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

    public readonly IEnumerable<byte> GetBytes()
    {
        foreach (var val in RpcWriter.GetBytes(PlayerName))
            yield return val;

        // In an ideal game, the count wouldn't exceed 6 or 7. But since I know some people who like to see me suffer, so I've set the max limit to 2^16 - 1, good luck changing roles and factions this
            // many times
        foreach (var val in RpcWriter.GetBytes((ushort)History.Count))
            yield return val;

        foreach (var (role, faction) in History)
        {
            yield return (byte)role;
            yield return (byte)faction;
        }

        foreach (var layer in OtherLayers)
            yield return (byte)layer;

        yield return (byte)(IsGaTarget ? 1 : 0);
        yield return (byte)(IsExeTarget ? 1 : 0);
        yield return (byte)(IsBhTarget ? 1 : 0);
        yield return (byte)(IsGuessTarget ? 1 : 0);
        yield return (byte)(IsDriveHolder ? 1 : 0);

        yield return (byte)(CanDoTasks ? 1 : 0);

        if (CanDoTasks)
        {
            yield return (byte)(TasksDone ? 1 : 0);

            if (!TasksDone)
            {
                yield return CompletedTasks;
                yield return TotalTasks;
            }
        }

        yield return (byte)(Disconnected ? 1 : 0);

        if (Disconnected)
            yield break;

        yield return (byte)DeathReason;

        if (DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Suicide or DeathReasonEnum.Ejected or DeathReasonEnum.Misfire or DeathReasonEnum.Escaped))
        {
            foreach (var val in RpcWriter.GetBytes(KilledBy))
                yield return val;
        }

        foreach (var val in RpcWriter.GetBytes(HatId))
            yield return val;

        foreach (var val in RpcWriter.GetBytes(SkinId))
            yield return val;

        foreach (var val in RpcWriter.GetBytes(VisorId))
            yield return val;

        foreach (var val in RpcWriter.GetBytes(ColorId))
            yield return val;

        if (ColorId != -2)
            yield break;

        foreach (var val in RpcWriter.GetBytes(Color))
            yield return val;
    }

    public void FromBytes(RpcReader netData)
    {
        PlayerName = netData.ReadString();

        var count = netData.ReadByte();

        while (count-- > 0)
            History.Add((netData.Read<Layer>(), netData.Read<Faction>()));

        count = 3;

        while (count-- > 0)
            OtherLayers.Add(netData.Read<Layer>());

        IsGaTarget = netData.ReadBool();
        IsExeTarget = netData.ReadBool();
        IsBhTarget = netData.ReadBool();
        IsGuessTarget = netData.ReadBool();
        IsDriveHolder = netData.ReadBool();

        CanDoTasks = netData.ReadBool();

        if (CanDoTasks)
        {
            TasksDone = netData.ReadBool();

            if (!TasksDone)
            {
                CompletedTasks = netData.ReadByte();
                TotalTasks = netData.ReadByte();
            }
        }

        Disconnected = netData.ReadBool();

        if (!Disconnected)
        {
            DeathReason = netData.Read<DeathReasonEnum>();

            if (DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Suicide or DeathReasonEnum.Ejected or DeathReasonEnum.Misfire or DeathReasonEnum.Escaped))
                KilledBy = netData.ReadString();
        }

        HatId = netData.ReadString();
        SkinId = netData.ReadString();
        VisorId = netData.ReadString();
        ColorId = netData.ReadInt();
        Color = ColorId switch
        {
            -2 => netData.ReadColor32(),
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

    public readonly (string FullSummary, string Summary) Summarise()
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
}