namespace TownOfUsReworked.Modules;

public abstract class Info(string id, UColor color, bool footer = false)
{
    public string ID { get; } = $"Wiki.{id}";
    public string DescID { get; } = $"Wiki.{id}.Description";
    public UColor Color { get; set; } = color;
    public bool Footer { get; } = footer;

    public static readonly List<Info> AllInfo = [];
    public static readonly List<LayerInfo> AllLayerInfo = [];

    public static void SetAllInfo()
    {
        AllLayerInfo.AddRanges(Data.AllInfo.AllRoles, Data.AllInfo.AllModifiers, Data.AllInfo.AllAbilities, Data.AllInfo.AllDispositions);
        AllInfo.AddRanges(AllLayerInfo, Data.AllInfo.AllFactions, Data.AllInfo.AllSubFactions, Data.AllInfo.AllModes, Data.AllInfo.AllOthers, Data.AllInfo.AllSymbols);
    }

    public abstract string WikiEntry();
}

public abstract class LayerInfo(LayerEnum layer, bool footer = false, UColor color = default) : Info($"{layer}", color == default ? LayerDictionary[layer].Color : color, footer)
{
    public LayerEnum Layer { get; } = layer;
}

public class RoleInfo(LayerEnum role, Alignment alignmentEnum, Faction faction, bool footer = false, UColor color = default) : LayerInfo(role, footer, color)
{
    public string Alignment { get; } = $"Wiki.Alignment.{alignmentEnum}";
    public string WinCon { get; } = TranslationManager.IdExists($"Wiki.{faction}.WinCon") ? $"Wiki.{faction}.WinCon" : $"Wiki.{role}.WinCon";
    public string Quote { get; } = $"Wiki.{role}.Quote";
    public string Attack { get; } = TranslationManager.IdExists($"Wiki.{role}.Attack") ? $"Wiki.{role}.Attack" : "Wiki.Attack.None";
    public string Defense { get; } = TranslationManager.IdExists($"Wiki.{role}.Defense") ? $"Wiki.{role}.Defense" : "Wiki.Defense.None";

    // private const string IntruderObjective = "Have a critical sabotage set off by the Intruders reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew and opposing Neutrals.";
    // private const string SyndicateObjective = "Have a critical sabotage set off by the Syndicate reach 0 seconds or kill off all Intruders, Unfaithful Syndicate, Crew and opposing Neutrals.";
    // private const string CrewObjective = "Finish tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, and opposing Neutrals.";

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
    _ => CustomColorManager.Faction
}, footer)
{
    // private const string SyndicateDescription = "Each member of this faction has a special ability and then after a certain number of meetings, can also kill. The main theme of this faction "
    //     + "is chaos. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. After a certain number of meetings, the " +
    //     "Syndicate can retreive the \"Chaos Drive\" which gives the holder the ability to kill (if they couldn't already) while also enhancing their existing abilities.";
    // private const string CrewDescription = "Each member has a special ability which determines who’s who and can help weed out the evils. The main theme of this faction is deduction and " +
    //     "goodwill. This faction is an uninformed majority meaning they make up most of the players and don't who the other members are. The Crew can do tasks which sort of act like a timer "
    //     + "for non-Crew roles.";
    // private const string IntruderDescription = "Each member of this faction has the ability to kill alongside an ability pertaining to their role. The main theme of this faction is " +
    //     "destruction and raw power. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. All members can sabotage to " +
    //     "distract the Crew from their tasks.";
    // private const string NeutralDescription = "Neutrals are essentially factionless. Each member of this faction has their own unique way to win, seperate from the other roles in the same " +
    //     "faction. The main theme of this faction is free for all. This faction is an uninformed minority of the game, meaning they make up a small part of the crew while not knowing who the "
    //     + "other members are. Each role is unique in its own way, some can be helpful, some exist to destroy others and some just exist for the sake of existing.";
    // private const string GameModeDescription = "Game Mode roles only spawn in certain special game modes. They have their own special abilities and objectives that are not seen in other " +
    //     "factions.";

    public Faction Faction { get; } = faction;

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
    // private const string CabalDescription = "The Cabal is an oraganisation that's similar to the Syndicate. They, however, operate covertly by secretly recruiting people to join their group"
    //     + ". The Cabal starts off very strong so they are of a higher priority when dealing with enemies. The Cabal is led by the Jackal.";
    // private const string UndeadDescription = "The Undead are a group of bloodthirsty vampires who slowly grow their numbers. The longer the game goes on, the higher their priority on the " +
    //     "elimination list. The Undead are led by the Dracula.";
    // private const string ReanimatedDescription = "The Reanimated are a bunch of people who have died yet hold a grudge agaisnt the living. This is made possible by the Necromancer, who leads"
    //     + " them. The longer the game goes on with no deaths, the higher the chances of a Necromancer at work.";
    // private const string CultDescription = "The Cult is a cult which can gain massive amounts of followers in one go. It may be weak at the start, but do not understimate their powerful " +
    //     "growth as it may overrun you. The Cult is led by the Whisperer.";

    public string Symbol { get; } = sub switch
    {
        SubFaction.Undead => "γ",
        SubFaction.Reanimated => "Σ",
        SubFaction.Cabal => "$",
        SubFaction.Cult => "Λ",
        _ => "φ"
    };
    public SubFaction SubFaction { get; } = sub;

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
    public string AppliesTo { get; } = $"Wiki.{modifier}.AppliesTo";

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
    public string AppliesTo { get; } = $"Wiki.{disposition}.AppliesTo";
    public string WinCon { get; } = $"Wiki.{disposition}.WinCon";
    public string Symbol { get; } = symbol;

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
    public string AppliesTo { get; } = $"Wiki.{ability}.AppliesTo";

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
    public string OtherNotes { get; } = $"Wiki.{otherNotes}.Notes";

    public override string WikiEntry()
    {
        var result = "";
        result += $"{TranslationManager.Translate("Wiki.Name")}: {TranslationManager.Translate(ID)}";
        result += $"\n{WrapText($"{TranslationManager.Translate("Wiki.Description")}: {TranslationManager.Translate(DescID)}")}";
        result += $"\n{WrapTexts(TranslationManager.Translate(OtherNotes).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))}";
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
    _ => CustomColorManager.GameMode
}, footer)
{
    // private const string ClassicDescription = "This is the main mode of the game. Any layer can spawn in this mode, but only once.";
    // private const string VanillaDescription = "This mode is nothing special, everyone is either a basic Crewmate or Impostor (or Anarchist if alternate intruders is turned on).";
    // private const string KODescription = "This is a restricted Classic mode where only roles with the capability to kill can spawn and the Syndicate recieves their Chaos Drive at the start "
    //     + "of the game.";
    // private const string AADescription = "This mode has no restrictions on how many instances of a layer can spawn. Each layer has a property called \"Uniqueness\" which is basically if only"
    //     + " one of that layer can spawn (or two for Lovers, Rivals, Mafia or Linked).";
    // private const string RLDescription = "In this mode, you can make a set list of what roles can spawn. You can decide the exact number of a certain alignment/faction. However, other layers"
    //     + " like modifiers, abilities and dispositions cannot spawn in this mode. All Any mode's \"Uniqueness\" property of roles also applies here.";
    // private const string CustomDescription = "This mode is basically Classic but you can decide how many instances of the layer can spawn in the game.";
    // private const string HnSDescription = "This mode is the unofficial addition of the Hide And Seek game mode that people used to play before the vanilla Hide And Seek was added. Only two "
    //     + "roles spawn and this mode can have two types. The Classic type makes it so that the Hunters have to kill everyone else, but their numbers do not increase. In the Infection type, "
    //     + "however, the Hunters turn the Hunted into their own teammates.";
    // private const string TRDescription = "This mode is a skill check mode to see who's the best at planning their task path and finishing tasks. No one can kill each other and must race to "
    //     + "be the first one to finish their tasks.";

    public GameMode Mode { get; } = mode;

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