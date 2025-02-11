namespace TownOfUsReworked.Modules;

public abstract class Info(string id, UColor color, bool footer = false)
{
    public string ID { get; } = $"Wiki.{id}";
    public UColor Color { get; } = color;
    public bool Footer { get; } = footer;
    protected string DescID { get; } = $"Wiki.{id}.Description";

    public static readonly List<Info> AllInfo = [];
    private static readonly List<LayerInfo> AllLayerInfo = [];

    public static void SetAllInfo()
    {
        AllLayerInfo.AddRanges(Data.AllInfo.AllRoles, Data.AllInfo.AllModifiers, Data.AllInfo.AllAbilities, Data.AllInfo.AllDispositions);
        AllInfo.AddRanges(AllLayerInfo, Data.AllInfo.AllFactions, Data.AllInfo.AllSubFactions, Data.AllInfo.AllModes, Data.AllInfo.AllOthers, Data.AllInfo.AllSymbols);
    }

    public abstract string WikiEntry();
}

public abstract class LayerInfo(LayerEnum layer, bool footer = false, UColor color = default) : Info($"{layer}", color == default ? LayerDictionary[layer].Color : color, footer);

public class RoleInfo(LayerEnum role, Alignment alignmentEnum, Faction faction, bool footer = false, UColor color = default) : LayerInfo(role, footer, color)
{
    private string Alignment { get; } = $"Wiki.Alignment.{alignmentEnum}";
    private string WinCon { get; } = TranslationManager.IdExists($"Wiki.{faction}.WinCon") ? $"Wiki.{faction}.WinCon" : $"Wiki.{role}.WinCon";
    private string Quote { get; } = $"Wiki.{role}.Quote";
    private string Attack { get; } = TranslationManager.IdExists($"Wiki.{role}.Attack") ? $"Wiki.{role}.Attack" : "Wiki.Attack.None";
    private string Defense { get; } = TranslationManager.IdExists($"Wiki.{role}.Defense") ? $"Wiki.{role}.Defense" : "Wiki.Defense.None";

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Alignment")}: {TranslationManager.Translate(Alignment)}";
        result += $"\n{TranslationManager.Translate("Wiki.Attack")}: {TranslationManager.Translate(Attack)}";
        result += $"\n{TranslationManager.Translate("Wiki.Defense")}: {TranslationManager.Translate(Defense)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.WinCon")}: {TranslationManager.Translate(WinCon)}")}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        result += $"\n\n{WrapText(TranslationManager.Translate(Quote))}";
        return result;
    }
}

public class FactionInfo(Faction faction, bool footer = false) : Info($"{faction}", faction switch
{
    Faction.Syndicate => CustomColorManager.Syndicate,
    Faction.Crew => CustomColorManager.Crew,
    Faction.Intruder => CustomColorManager.Intruder,
    Faction.Neutral => CustomColorManager.Neutral,
    Faction.GameMode => CustomColorManager.GameMode,
    Faction.Illuminati => CustomColorManager.Illuminati,
    Faction.Pandorica => CustomColorManager.Pandorica,
    Faction.Compliance => CustomColorManager.Compliance,
    _ => CustomColorManager.Faction
}, footer)
{
    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class SubFactionInfo(SubFaction sub, bool footer = false) : Info($"{sub}", sub switch
{
    SubFaction.Undead => CustomColorManager.Undead,
    SubFaction.Reanimated => CustomColorManager.Reanimated,
    SubFaction.Cabal => CustomColorManager.Cabal,
    SubFaction.Cult => CustomColorManager.Cult,
    _ => CustomColorManager.SubFaction
}, footer)
{
    private string Symbol { get; } = sub switch
    {
        SubFaction.Undead => "γ",
        SubFaction.Reanimated => "Σ",
        SubFaction.Cabal => "$",
        SubFaction.Cult => "Λ",
        _ => "φ"
    };

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Symbol")}: {Symbol}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class AlignmentInfo(Alignment alignmentEnum, bool footer = false) : Info($"{alignmentEnum}", CustomColorManager.Alignment, footer)
{
    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class ModifierInfo(LayerEnum modifier, bool footer = false, UColor color = default) : LayerInfo(modifier, footer, color)
{
    private string AppliesTo { get; } = $"Wiki.{modifier}.AppliesTo";

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"{TranslationManager.Translate("Wiki.AppliesTo")}: {TranslationManager.Translate(AppliesTo)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class DispositionInfo(LayerEnum disposition, string symbol, bool footer = false, UColor color = default) : LayerInfo(disposition, footer, color)
{
    private string AppliesTo { get; } = $"Wiki.{disposition}.AppliesTo";
    private string WinCon { get; } = $"Wiki.{disposition}.WinCon";
    private string Symbol { get; } = symbol;

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Symbol")}: {Symbol}";
        result += $"{TranslationManager.Translate("Wiki.AppliesTo")}: {TranslationManager.Translate(AppliesTo)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.WinCon")}: {TranslationManager.Translate(WinCon)}")}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class AbilityInfo(LayerEnum ability, bool footer = false, UColor color = default) : LayerInfo(ability, footer, color)
{
    private string AppliesTo { get; } = $"Wiki.{ability}.AppliesTo";

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"{TranslationManager.Translate("Wiki.AppliesTo")}: {TranslationManager.Translate(AppliesTo)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class OtherInfo(string id, UColor color, string otherNotes = "", bool footer = false) : Info(id, color, footer)
{
    private string OtherNotes { get; } = $"Wiki.{otherNotes}.Notes";

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        result += $"\n{WrapTexts(TranslationManager.Translate(OtherNotes).TrueSplit('\n'))}";
        return result;
    }
}

public class GameModeInfo(GameMode mode, bool footer = false) : Info($"{mode}", mode switch
{
    GameMode.Vanilla => UColor.white,
    GameMode.Classic => CustomColorManager.Classic,
    GameMode.RoleList => CustomColorManager.RoleList,
    GameMode.HideAndSeek => CustomColorManager.HideAndSeek,
    GameMode.TaskRace => CustomColorManager.TaskRace,
    GameMode.AllAny => CustomColorManager.AllAny,
    _ => CustomColorManager.GameMode
}, footer)
{
    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}

public class SymbolInfo(string id, string symbol, UColor color, bool footer = false) : Info(id, color, footer)
{
    public string Symbol { get; } = symbol;

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{TranslationManager.Translate("Wiki.Symbol")}: {Symbol}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        return result;
    }
}