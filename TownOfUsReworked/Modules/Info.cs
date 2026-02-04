namespace TownOfUsReworked.Modules;

public abstract class Info(string id, UColor color, bool footer = false)
{
    public readonly string ID = $"Wiki.{id}";
    public readonly UColor Color = color;
    public readonly bool Footer = footer;
    protected readonly string DescID = $"Wiki.{id}.Description";

    public static readonly List<Info> AllInfos = [];
    private static readonly List<LayerInfo> AllLayerInfo = [];

    public static void SetAllInfo()
    {
        AllLayerInfo.AddRanges(AllInfo.AllRoles, AllInfo.AllModifiers, AllInfo.AllAbilities, AllInfo.AllDispositions);
        AllInfos.AddRanges(AllLayerInfo, AllInfo.AllFactions, AllInfo.AllModes, AllInfo.AllOthers, AllInfo.AllSymbols);

        if (!TownOfUsReworked.IsDev)
#pragma warning disable CS0162 // Unreachable code detected
            return;
#pragma warning restore CS0162 // Unreachable code detected

        foreach (var info in AllInfos)
        {
            TranslationManager.DebugId(info.ID);
            TranslationManager.DebugId(info.DescID);
            info.Debug();
        }
    }

    public abstract string WikiEntry();

    protected virtual void Debug() {}
}

public abstract class LayerInfo(Layer layer, bool footer = false, UColor color = default) : Info($"{layer}", color == default ? LayerDictionary[layer].Color : color, footer);

public sealed class RoleInfo(Layer role, Alignment alignmentEnum, Faction faction, bool footer = false, UColor color = default) : LayerInfo(role, footer, color)
{
    private readonly string Alignment = $"Wiki.Alignment.{alignmentEnum}";
    private readonly string WinCon = TranslationManager.IdExists($"Wiki.{faction}.WinCon") ? $"Wiki.{faction}.WinCon" : $"Wiki.{role}.WinCon";
    private readonly string Quote = $"Wiki.{role}.Quote";
    private readonly string Attack = TranslationManager.IdExists($"Wiki.{role}.Attack") ? $"Wiki.{role}.Attack" : "Wiki.Attack.None";
    private readonly string Defense = TranslationManager.IdExists($"Wiki.{role}.Defense") ? $"Wiki.{role}.Defense" : "Wiki.Defense.None";

    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Alignment")}: {TranslationManager.Translate(Alignment)}";
        result += $"\n{TranslationManager.Translate("Wiki.Attack")}: {TranslationManager.Translate(Attack)}";
        result += $"\n{TranslationManager.Translate("Wiki.Defense")}: {TranslationManager.Translate(Defense)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.WinCon")}: {TranslationManager.Translate(WinCon)}")}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        result += $"\n\n{WrapText(TranslationManager.Translate(Quote))}";
        return result;
    }

    protected override void Debug()
    {
        TranslationManager.DebugId(Alignment);
        TranslationManager.DebugId(WinCon);
        TranslationManager.DebugId(Quote);
        TranslationManager.DebugId(Attack);
        TranslationManager.DebugId(Defense);
    }
}

public sealed class FactionInfo(Faction faction, bool footer = false) : Info($"{faction}", faction switch
{
    Faction.Syndicate => CustomColorManager.Syndicate,
    Faction.Crew => CustomColorManager.Crew,
    Faction.Intruder => CustomColorManager.Intruder,
    Faction.Outcast => CustomColorManager.Outcast,
    Faction.GameMode => CustomColorManager.GameMode,
    Faction.Illuminati => CustomColorManager.Illuminati,
    Faction.Pandorica => CustomColorManager.Pandorica,
    Faction.Compliance => CustomColorManager.Compliance,
    Faction.Apocalypse => CustomColorManager.Apocalypse,
    Faction.Arsonist => CustomColorManager.Arsonist,
    Faction.Cryomaniac => CustomColorManager.Cryomaniac,
    Faction.Glitch => CustomColorManager.Glitch,
    Faction.Juggernaut => CustomColorManager.Juggernaut,
    Faction.Murderer => CustomColorManager.Murderer,
    Faction.SerialKiller => CustomColorManager.SerialKiller,
    Faction.Werewolf => CustomColorManager.Werewolf,
    Faction.Defector => CustomColorManager.Defector,
    Faction.Betrayer => CustomColorManager.Betrayer,
    Faction.Mafia => CustomColorManager.Mafia,
    Faction.Shifter => CustomColorManager.Mafia,
    Faction.Cabal => CustomColorManager.Cabal,
    Faction.Cult => CustomColorManager.Cult,
    Faction.Followers => CustomColorManager.Followers,
    Faction.Reanimated => CustomColorManager.Reanimated,
    Faction.Undead => CustomColorManager.Undead,
    _ => CustomColorManager.Faction
}, footer)
{
    private readonly string WinCon = $"Wiki.{faction}.WinCon";

    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"{TranslationManager.Translate("Wiki.Objectives")}: {TranslationManager.Translate(WinCon)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }

    protected override void Debug() => TranslationManager.DebugId(WinCon);
}

public sealed class AlignmentInfo(Alignment alignmentEnum, bool footer = false) : Info($"{alignmentEnum}", CustomColorManager.Alignment, footer)
{
    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public abstract class ApplicableLayer(Layer layer, bool footer = false, UColor color = default) : LayerInfo(layer, footer, color)
{
    protected readonly string AppliesTo = $"Wiki.{layer}.AppliesTo";

    protected override void Debug() => TranslationManager.DebugId(AppliesTo);
}

public sealed class ModifierInfo(Layer modifier, bool footer = false, UColor color = default) : ApplicableLayer(modifier, footer, color)
{
    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"{TranslationManager.Translate("Wiki.AppliesTo")}: {TranslationManager.Translate(AppliesTo)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public sealed class DispositionInfo(Layer disposition, string symbol, bool footer = false, UColor color = default) : ApplicableLayer(disposition, footer, color)
{
    private readonly string WinCon = $"Wiki.{disposition}.WinCon";
    private readonly string Symbol = symbol;

    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Symbol")}: {Symbol}";
        result += $"{TranslationManager.Translate("Wiki.AppliesTo")}: {TranslationManager.Translate(AppliesTo)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.WinCon")}: {TranslationManager.Translate(WinCon)}")}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }

    protected override void Debug()
    {
        base.Debug();
        TranslationManager.DebugId(WinCon);
    }
}

public sealed class AbilityInfo(Layer ability, bool footer = false, UColor color = default) : ApplicableLayer(ability, footer, color)
{
    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"{TranslationManager.Translate("Wiki.AppliesTo")}: {TranslationManager.Translate(AppliesTo)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public sealed class OtherInfo(string id, UColor color, string otherNotes = "", bool footer = false) : Info(id, color, footer)
{
    private readonly string OtherNotes = otherNotes?.Length is null or 0 ? string.Empty : $"Wiki.{otherNotes}.Notes";
    private readonly bool NotesExist = otherNotes?.Length is not (null or 0);

    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";

        if (NotesExist)
            result += $"\n{WrapTexts(TranslationManager.Translate(OtherNotes).TrueSplit('\n'))}";

        return result;
    }

    protected override void Debug()
    {
        if (NotesExist)
            TranslationManager.DebugId(OtherNotes);
    }
}

public sealed class GameModeInfo(Mode mode, bool footer = false) : Info($"{mode}", mode switch
{
    Mode.Vanilla => UColor.white,
    Mode.Classic => CustomColorManager.Classic,
    Mode.List => CustomColorManager.List,
    Mode.HideAndSeek => CustomColorManager.HideAndSeek,
    Mode.TaskRace => CustomColorManager.TaskRace,
    Mode.AllAny => CustomColorManager.AllAny,
    _ => CustomColorManager.GameMode
}, footer)
{
    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public sealed class SymbolInfo(string id, string symbol, UColor color, bool footer = false) : Info(id, color, footer)
{
    public readonly string Symbol = symbol;

    public override string WikiEntry()
    {
        var result = string.Empty;
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Symbol")}: {Symbol}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}