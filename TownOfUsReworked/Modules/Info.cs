namespace TownOfUsReworked.Modules;

public class Info(string name, string shortF, string description, UColor color, InfoType type, bool footer = false)
{
    public string Name { get; } = name;
    public string Short { get; set; } = shortF;
    public string Description { get; set; } = description;
    public UColor Color { get; set; } = color;
    public InfoType Type { get; } = type;
    public bool Footer { get; } = footer;

    public static readonly List<Info> AllInfo = [];

    public static void SetAllInfo() => AllInfo.AddRanges(LayerInfo.AllRoles, LayerInfo.AllModifiers, LayerInfo.AllAbilities, LayerInfo.AllDispositions, LayerInfo.AllFactions,
        LayerInfo.AllSubFactions, LayerInfo.AllModes, LayerInfo.AllOthers, LayerInfo.AllSymbols);

    public static string ColorIt(string result, bool bold = true)
    {
        foreach (var info in AllInfo.Where(x => x.Type is not (InfoType.Alignment or InfoType.Lore)))
            result = result.Replace(info.Name, $"<b><color=#{info.Color.ToHtmlStringRGBA()}>{info.Name}</color></b>");

        foreach (var info in LayerInfo.AllSymbols)
            result = result.Replace(info.Symbol, $"<b><color=#{info.Color.ToHtmlStringRGBA()}>{info.Symbol}</color></b>");

        for (var i = 0; i < 56; i++)
            result = result.Replace(((Alignment)i).AlignmentName(), $"<b>{((Alignment)i).AlignmentName(true)}</b>");

        result = result.Replace($"<b><color=#758000FF>Drunk</color></b>ard", "Drunkard")
            .Replace($"<b><color=#FFD700FF>Role</color></b> List", "Role List")
            .Replace($"<b><color=#B51E39FF>Bounty</color></b> Hunter", "Bounty Hunter")
            .Replace($"<b><color=#B51E39FF>Bounty</color></b> <b><color=#FF004EFF>Hunter</color></b>", "Bounty Hunter")
            .Replace($"Bounty <b><color=#FF004EFF>Hunter</color></b>", "Bounty Hunter")
            .Replace($"Vampire <b><color=#FF004EFF>Hunter</color></b>", "Vampire Hunter");

        if (!bold)
            result = result.Replace("<b>", "").Replace("</b>", "");

        return result;
    }

    public virtual void WikiEntry(out string result) => result = "";
}

public class RoleInfo(string name, string shortF, string description, Alignment alignmentEnum, Faction faction, string quote, UColor color, LayerEnum role, string attack = "None", string
    defense = "None", string wincon = "", bool footer = false) : Info(name, shortF, description, color, InfoType.Role, footer)
{
    public string Alignment { get; } = alignmentEnum.AlignmentName();
    public string ColoredAlignment { get; } = alignmentEnum.AlignmentName(true);
    public string WinCon { get; } = faction switch
    {
        Faction.Syndicate => SyndicateObjective,
        Faction.Crew => CrewObjective,
        Faction.Intruder => IntruderObjective,
        Faction.Neutral or Faction.GameMode => wincon,
        _ => "Invalid"
    };
    public string Quote { get; } = quote;
    public string Attack { get; } = attack;
    public string Defense { get; } = defense;
    public Faction Faction { get; } = faction;
    public LayerEnum Role { get; } = role;

    private const string IntruderObjective = "Have a critical sabotage set off by the Intruders reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew and opposing Neutrals.";
    private const string SyndicateObjective = "Have a critical sabotage set off by the Syndicate reach 0 seconds or kill off all Intruders, Unfaithful Syndicate, Crew and opposing Neutrals.";
    private const string CrewObjective = "Finish tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, and opposing Neutrals.";

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += $"\nAlignment: {ColoredAlignment}";
        result += $"\nAttack: {Attack}";
        result += $"\nDefense: {Defense}";
        result += "\n" + ColorIt(WrapText($"Win Condition: {WinCon}"));
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
        result += "\n\n" + ColorIt(WrapText(Quote));
    }
}

public class FactionInfo : Info
{
    private const string SyndicateDescription = "Each member of this faction has a special ability and then after a certain number of meetings, can also kill. The main theme of this faction "
        + "is chaos. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. After a certain number of meetings, the " +
        "Syndicate can retreive the \"Chaos Drive\" which gives the holder the ability to kill (if they couldn't already) while also enhancing their existing abilities.";
    private const string CrewDescription = "Each member has a special ability which determines who’s who and can help weed out the evils. The main theme of this faction is deduction and " +
        "goodwill. This faction is an uninformed majority meaning they make up most of the players and don't who the other members are. The Crew can do tasks which sort of act like a timer "
        + "for non-Crew roles.";
    private const string IntruderDescription = "Each member of this faction has the ability to kill alongside an ability pertaining to their role. The main theme of this faction is " +
        "destruction and raw power. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. All members can sabotage to " +
        "distract the Crew from their tasks.";
    private const string NeutralDescription = "Neutrals are essentially factionless. Each member of this faction has their own unique way to win, seperate from the other roles in the same " +
        "faction. The main theme of this faction is free for all. This faction is an uninformed minority of the game, meaning they make up a small part of the crew while not knowing who the "
        + "other members are. Each role is unique in its own way, some can be helpful, some exist to destroy others and some just exist for the sake of existing.";
    private const string GameModeDescription = "Game Mode roles only spawn in certain special game modes. They have their own special abilities and objectives that are not seen in other " +
        "factions.";

    public Faction Faction { get; }

    public FactionInfo(Faction faction, bool footer = false) : base($"{faction}", "", "", default, InfoType.Faction, footer)
    {
        Faction = faction;
        (Description, Short, Color) = faction switch
        {
            Faction.Syndicate => (SyndicateDescription, "Syn", CustomColorManager.Syndicate),
            Faction.Crew => (CrewDescription, "Crew", CustomColorManager.Crew),
            Faction.Intruder => (IntruderDescription, "Int", CustomColorManager.Intruder),
            Faction.Neutral => (NeutralDescription, "Neut", CustomColorManager.Neutral),
            Faction.GameMode => (GameModeDescription, "GM", CustomColorManager.GameMode),
            _ => ("Invalid", "Invalid", CustomColorManager.Faction)
        };
    }

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class SubFactionInfo : Info
{
    private const string CabalDescription = "The Cabal is an oraganisation that's similar to the Syndicate. They, however, operate covertly by secretly recruiting people to join their group"
        + ". The Cabal starts off very strong so they are of a higher priority when dealing with enemies. The Cabal is led by the Jackal.";
    private const string UndeadDescription = "The Undead are a group of bloodthirsty vampires who slowly grow their numbers. The longer the game goes on, the higher their priority on the " +
        "elimination list. If a member of this subfaction interacts with a Vampire Hunter, the interactor will be killed by the Vampire Hunter in question. The Undead are led by the " +
        "Dracula.";
    private const string ReanimatedDescription = "The Reanimated are a bunch of people who have died yet hold a grudge agaisnt the living. This is made possible by the Necromancer, who leads"
        + " them. The longer the game goes on with no deaths, the higher the chances of a Necromancer at work.";
    private const string SectDescription = "The Sect is a cult which can gain massive amounts of followers in one go. It may be weak at the start, but do not understimate their powerful " +
        "growth as it may overrun you. The Sect is led by the Whisperer.";

    public string Symbol { get; }
    public SubFaction SubFaction { get; }

    public SubFactionInfo(SubFaction sub, bool footer = false) : base($"{sub}", "", "", default, InfoType.SubFaction, footer)
    {
        SubFaction = sub;
        (Description, Short, Color, Symbol) = sub switch
        {
            SubFaction.Undead => (UndeadDescription, "Und", CustomColorManager.Undead, "γ"),
            SubFaction.Reanimated => (ReanimatedDescription, "RA", CustomColorManager.Reanimated, "Σ"),
            SubFaction.Cabal => (CabalDescription, "Cab", CustomColorManager.Cabal, "$"),
            SubFaction.Sect => (SectDescription, "Sect", CustomColorManager.Sect, "Λ"),
            _ => ("Invalid", "Invalid", CustomColorManager.SubFaction, "φ")
        };
    }

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt($"Symbol: {Symbol}");
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class AlignmentInfo : Info
{
    private const string CrPDescription = "Crew (Protective) roles have the capability to stop someone from losing their life or bring back the dead.";
    private const string CIDescription = "Crew (Investigative) roles have the ability to gain information via special methods. Using the acquired info, Crew (Investigative) roles "
        + "can deduce who is good and who is not.";
    private const string CUDescription = "Crew (Utility) roles usually don't appear under regaular spawn conditions.";
    private const string CSDescription = "Crew (Support) roles are roles with miscellaneous abilities. Try not to get lost because if you are not paying attention, your chances " +
        "of winning will be severely decreased because of them.";
    private const string CADescription = "Crew (Auditor) roles are special roles that spawn under certain conditions. They exist for the demise of rival subfactions.";
    private const string CKDescription = "Crew (Killing) roles have no aversion to killing like the rest of the Crew and if left alone and potentially wreck the chances of evils winning.";
    private const string CSvDescription = "Crew (Sovereign) roles are democratic roles with powers over votes. They are the most powerful during a meeting, so avoid too many " +
        "meetings while they are active.";
    private const string CDDescription = "Crew (Deception) roles are defected Intruder (Deception) roles who have sided with the Crew.";
    private const string CCDescription = "Crew (Concealing) roles are defected Intruder (Concealing) roles who have sided with the Crew.";
    private const string CDiDescription = "Crew (Disruption) roles are defected Syndicate (Disruption) roles who have sided with the Crew.";
    private const string CrPowDescription = "Crew (Power) roles are defected Syndicate (Power) roles who have sided with the Crew.";
    private const string CHDescription = "Crew (Head) roles are defected Intruder (Head) roles who have sided with the Crew.";

    private const string ISDescription = "Intruder (Support) roles have miscellaneous abilities. These roles can delay players' chances of winning by either gaining enough info to"
        + " stop them or forcing players to do things they can't.";
    private const string ICDescription = "Intruder (Concealing) roles specialise in hiding information from others. If there is no new information, it's probably their work.";
    private const string IDDescription = "Intruder (Deception) roles are built to spread misinformation. Never trust your eyes, for the killer you saw in front of you might not be"
        + " the one who they seem to be.";
    private const string IUDescription = "Intruder (Utility) roles usually don't appear under regaular spawn conditions.";
    private const string IKDescription = "Intruder (Killing) roles have an addition ability to kill. Be careful of them as large numbers of dead bpdies will start to pile up with "
        + "them around.";
    private const string IPDescription = "Intruder (Protective) roles are Crew (Protective) roles that have betrayed the Crew to join the Intruders.";
    private const string IIDescription = "Intruder (Investigative) roles are Crew (Investigative) roles that have betrayed the Crew to join the Intruders.";
    private const string IADescription = "Intruder (Auditor) roles are Crew (Auditor) roles that have betrayed the Crew to join the Intruders.";
    private const string ISvDescription = "Intruder (Sovereign) roles are Crew (Sovereign) roles that have betrayed the Crew to join the Intruders.";
    private const string IDiDescription = "Intruder (Disruption) roles are defected Syndicate (Disruption) roles who have sided with the Intruders.";
    private const string IPowDescription = "Intruder (Power) roles are defected Syndicate (Power) roles who have sided with the Intruders.";
    private const string IHDescription = "Intruder (Head) roles are powerful roles that lead the Intruders.";

    private const string SUDescription = "Syndicate (Utility) roles usually don't appear under regaular spawn conditions.";
    private const string SSuDescription = "Syndicate (Support) roles have miscellaneous abilities. They are detrimental to the Syndicate's cause and if used right, can greatly " +
        "affect how the game continues.";
    private const string SDDescription = "Syndicate (Disruption) roles are designed to change the flow of the game, via changing some mechanic.";
    private const string SyKDescription = "Syndicate (Killing) roles are powerful killers unique ways to rack up body counts.";
    private const string SPDescription = "Syndicate (Power) roles are powerful disruptors with a knack for chaos and destruction.";
    private const string SProtDescription = "Syndicate (Protective) roles are Crew (Protective) roles that have betrayed the Crew to join the Syndicate.";
    private const string SIDescription = "Syndicate (Investigative) roles are Crew (Investigative) roles that have betrayed the Crew to join the Syndicate.";
    private const string SADescription = "Syndicate (Auditor) roles are Crew (Auditor) roles that have betrayed the Crew to join the Syndicate.";
    private const string SSvDescription = "Syndicate (Sovereign) roles are Crew (Sovereign) roles that have betrayed the Crew to join the Syndicate.";
    private const string SDeDescription = "Syndicate (Deception) roles are defected Intruder (Deception) roles who have sided with the Syndicate.";
    private const string SCDescription = "Syndicate (Concealing) roles are defected Intruder (Concealing) roles who have sided with the Syndicate.";
    private const string SHDescription = "Syndicate (Head) roles are defected Intruder (Head) roles who have sided with the Syndicate.";

    private const string NBDescription = "Neutral (Benign) roles are special roles that have the capability to win with anyone, as long as a certain condition is fulfilled by the" +
        " end of the game.";
    private const string NKDescription = "Neutral (Killing) roles are roles that have the ability to kill and do not side with anyone. Each role has a special way to kill and gain "
        + "large body counts in one go. Steer clear of them if you don't want to die.";
    private const string NEDescription = "Neutral (Evil) roles are roles whose objectives clash with those of other roles and factions. As such, you need to ensure they don't have a chance at"
        + " winning or when they do win, you have their cooperation.";
    private const string NPDescription = "Neutral (Proselyte) roles are special roles that have their specific ways to win and do not spawn in naturally. Each role here is unique in " +
        "its own way and more often than not they are against you.";
    private const string NNDescription = "Neutral (Neophyte) roles are roles that can convert someone to side with them. Be careful of them, as they can easily overrun you with their numbers.";
    private const string NADescription = "Neutral (Apocalypse) roles are powerful roles that appear when a Neutral (Harbinger) role has completed their objective. You will be optionally" +
        " alerted when a Neutral (Apocalypse) role appears. The presence of even one these roles can change the flow of the game.";
    private const string NHDescription = "Neutral (Harbinger) roles are weak roles who only have one goal, complete their objective and become a Neutral (Apocalypse) role.";
    private const string NDiDescription = "Neutral (Disruption) roles are defected Syndicate (Disruption) roles who have broken away from the Syndicate.";
    private const string NPowDescription = "Neutral (Power) roles are defected Syndicate (Power) roles who have broken away from the Syndicate.";
    private const string NProtDescription = "Neutral (Protective) roles are Crew (Protective) roles that have betrayed the Crew.";
    private const string NIDescription = "Neutral (Investigative) roles are Crew (Investigative) roles that have betrayed the Crew.";
    private const string NAudDescription = "Neutral (Auditor) roles are Crew (Auditor) roles that have betrayed the Crew.";
    private const string NSvDescription = "Neutral (Sovereign) roles are Crew (Sovereign) roles that have betrayed the Crew.";
    private const string NDDescription = "Neutral (Deception) roles are defected Intruder (Deception) roles who have broken away from the Intruders.";
    private const string NCDescription = "Neutral (Concealing) role are defected Intruder (Concealing) roles who have broken away from the Intruders.";
    private const string NHeadDescription = "Neutral (Head) role are defected Intruder (Head) roles who have broken away from the Intruders.";
    private const string NUDescription = "Neutral (Utility) roles are defected Crew, Intruder or Syndicate (Utility) roles who have broken away from their respective factions.";
    private const string NSDescription = "Neutral (Support) roles are defected Crew, Intruder or Syndicate (Support) roles who have broken away from their respective factions.";

    private const string HnSDescription = "These roles spawn in the Hide And Seek game mode.";
    private const string TRDescription = "These roles spawn in the Task Race game mode.";

    public string AlignmentName { get; }
    public Alignment Alignment { get; }

    public AlignmentInfo(Alignment alignmentEnum, bool footer = false) : base(alignmentEnum.AlignmentName(), "", "", CustomColorManager.Alignment, InfoType.Alignment, footer)
    {
        Alignment = alignmentEnum;
        (Short, Description, AlignmentName) = Alignment switch
        {
            Alignment.CrewSupport => ("CS", CSDescription, "Support"),
            Alignment.CrewInvest => ("CI", CIDescription, "Investigative"),
            Alignment.CrewProt => ("CrP", CrPDescription, "Protective"),
            Alignment.CrewKill => ("CK", CKDescription, "Killing"),
            Alignment.CrewUtil => ("CU", CUDescription, "Utility"),
            Alignment.CrewSov => ("CSv", CSvDescription, "Sovereign"),
            Alignment.CrewAudit => ("CA", CADescription, "Auditor"),
            Alignment.CrewConceal => ("CC", CCDescription, "Concealing"),
            Alignment.CrewDecep => ("CD", CDDescription, "Deception"),
            Alignment.CrewPower => ("CrPow", CrPowDescription, "Power"),
            Alignment.CrewDisrup => ("CDi", CDiDescription, "Disruption"),
            Alignment.IntruderSupport => ("IS", ISDescription, "Support"),
            Alignment.IntruderConceal => ("IC", ICDescription, "Conceal"),
            Alignment.IntruderDecep => ("ID", IDDescription, "Depection"),
            Alignment.IntruderKill => ("IK", IKDescription, "Killing"),
            Alignment.IntruderUtil => ("IU", IUDescription, "Utility"),
            Alignment.IntruderInvest => ("II", IIDescription, "Investigative"),
            Alignment.IntruderProt => ("IP", IPDescription, "Protective"),
            Alignment.IntruderSov => ("ISv", ISvDescription, "Sovereign"),
            Alignment.IntruderAudit => ("IA", IADescription, "Auditor"),
            Alignment.IntruderPower => ("IPow", IPowDescription, "Power"),
            Alignment.IntruderDisrup => ("IDi", IDiDescription, "Disruption"),
            Alignment.NeutralKill => ("NK", NKDescription, "Killing"),
            Alignment.NeutralNeo => ("NN", NNDescription, "Neophyte"),
            Alignment.NeutralEvil => ("NE", NEDescription, "Evil"),
            Alignment.NeutralBen => ("NB", NBDescription, "Benign"),
            Alignment.NeutralPros => ("NP", NPDescription, "Proselyte"),
            Alignment.NeutralApoc => ("NA", NADescription, "Apocalypse"),
            Alignment.NeutralHarb => ("NH", NHDescription, "Harbinger"),
            Alignment.NeutralInvest => ("NI", NIDescription, "Investigative"),
            Alignment.NeutralAudit => ("NAud", NAudDescription, "Auditor"),
            Alignment.NeutralSov => ("NSv", NSvDescription, "Sovereign"),
            Alignment.NeutralProt => ("NProt", NProtDescription, "Protective"),
            Alignment.NeutralSupport => ("NS", NSDescription, "Support"),
            Alignment.NeutralUtil => ("NU", NUDescription, "Utility"),
            Alignment.NeutralConceal => ("NC", NCDescription, "Conceal"),
            Alignment.NeutralDecep => ("ND", NDDescription, "Deception"),
            Alignment.NeutralDisrup => ("NDi", NDiDescription, "Disruption"),
            Alignment.NeutralPower => ("NPow", NPowDescription, "Power"),
            Alignment.SyndicateKill => ("SyK", SyKDescription, "Killing"),
            Alignment.SyndicateSupport => ("SSu", SSuDescription, "Support"),
            Alignment.SyndicateDisrup => ("SD", SDDescription, "Disruption"),
            Alignment.SyndicatePower => ("SP", SPDescription, "Power"),
            Alignment.SyndicateUtil => ("SU", SUDescription, "Utility"),
            Alignment.SyndicateInvest => ("SI", SIDescription, "Investigative"),
            Alignment.SyndicateProt => ("SProt", SProtDescription, "Protective"),
            Alignment.SyndicateSov => ("SSv", SSvDescription, "Soveriegn"),
            Alignment.SyndicateAudit => ("SA", SADescription, "Auditor"),
            Alignment.SyndicateConceal => ("SC", SCDescription, "Concealing"),
            Alignment.SyndicateDecep => ("SDe", SDeDescription, "Deception"),
            Alignment.CrewHead => ("CH", CHDescription, "Head"),
            Alignment.IntruderHead => ("IH", IHDescription, "Head"),
            Alignment.SyndicateHead => ("SH", SHDescription, "Head"),
            Alignment.NeutralHead => ("NHead", NHeadDescription, "Head"),
            Alignment.GameModeHideAndSeek => ("HnS", HnSDescription, "Hide And Seek"),
            Alignment.GameModeTaskRace => ("TR", TRDescription, "Task Race"),
            _ => ("Invalid", "Invalid", "Invalid")
        };
    }

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += $"Name: {Alignment.AlignmentName(true)}";
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class ModifierInfo(string name, string shortF, string description, string applies, UColor color, LayerEnum modifier, bool footer = false) : Info(name, shortF, description, color,
    InfoType.Modifier, footer)
{
    public string AppliesTo { get; } = applies;
    public LayerEnum Modifier { get; } = modifier;

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt(WrapText($"Applies To: {AppliesTo}"));
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class DispositionInfo(string name, string shortF, string description, string wincon, string applies, string symbol, UColor color, LayerEnum disposition, bool footer = false) :
    Info(name, shortF, description, color, InfoType.Disposition, footer)
{
    public string AppliesTo { get; } = applies;
    public string WinCon { get; } = wincon;
    public string Symbol { get; } = symbol;
    public LayerEnum Disposition { get; } = disposition;

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt($"Symbol: {Symbol}");
        result += "\n" + ColorIt(WrapText($"Applies To: {AppliesTo}"));
        result += "\n" + ColorIt(WrapText($"Win Condition: {WinCon}"));
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class AbilityInfo(string name, string shortF, string description, string applies, UColor color, LayerEnum ability, bool footer = false) : Info(name, shortF, description, color,
    InfoType.Ability, footer)
{
    public string AppliesTo { get; } = applies;
    public LayerEnum Ability { get; } = ability;

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt(WrapText($"Applies To: {AppliesTo}"));
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class Lore(string name, string story, string shortF, UColor color) : Info(name, shortF, story, color, InfoType.Lore)
{
    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += "\n" + ColorIt(WrapText(Description));
    }
}

public class OtherInfo(string name, string shortF, string description, UColor color, string otherNotes = "", bool footer = false) : Info(name, shortF, description, color, InfoType.Other,
    footer)
{
    public string OtherNotes { get; } = otherNotes;

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
        result += "\n" + ColorIt(WrapTexts(OtherNotes.Split('\n')));
    }
}

public class GameModeInfo : Info
{
    private const string ClassicDescription = "This is the main mode of the game. Any layer can spawn in this mode, but only once.";
    private const string VanillaDescription = "This mode is nothing special, everyone is either a basic Crewmate or Impostor (or Anarchist if alternate intruders is turned on).";
    private const string KODescription = "This is a restricted Classic mode where only roles with the capability to kill can spawn and the Syndicate recieves their Chaos Drive at the start "
        + "of the game.";
    private const string AADescription = "This mode has no restrictions on how many instances of a layer can spawn. Each layer has a property called \"Uniqueness\" which is basically if only"
        + " one of that layer can spawn (or two for Lovers, Rivals, Mafia or Linked).";
    private const string RLDescription = "In this mode, you can make a set list of what roles can spawn. You can decide the exact number of a certain alignment/faction. However, other layers"
        + " like modifiers, abilities and dispositions cannot spawn in this mode. All Any's \"Uniqueness\" property of roles also applies here.";
    private const string CustomDescription = "This mode is basically Classic but you can decide how many instances of the layer can spawn in the game.";
    private const string HnSDescription = "This mode is the unofficial addition of the Hide And Seek game mode that people used to play before the vanilla Hide And Seek was added. Only two "
        + "roles spawn and this mode can have two types. The Classic type makes it so that the Hunters have to kill everyone else, but their numbers do not increase. In the Infection type, "
        + "however, the Hunters turn the Hunted into their own teammates.";
    private const string TRDescription = "This mode is a skill check mode to see who's the best at planning their task path and finishing tasks. No one can kill each other and must race to "
        + "be the first one to finish their tasks.";

    public GameMode Mode { get; }

    public GameModeInfo(GameMode mode, bool footer = false) : base(mode.GameModeName(), "", "", default, InfoType.GameMode, footer)
    {
        Mode = mode;
        (Description, Short, Color) = mode switch
        {
            GameMode.Vanilla => (VanillaDescription, "Vanilla", UColor.white),
            GameMode.Classic => (ClassicDescription, "Classic", CustomColorManager.Classic),
            GameMode.KillingOnly => (KODescription, "KO", CustomColorManager.KillingOnly),
            GameMode.AllAny => (AADescription, "AA", CustomColorManager.AllAny),
            GameMode.RoleList => (RLDescription, "RL", CustomColorManager.RoleList),
            GameMode.Custom => (CustomDescription, "Custom", CustomColorManager.Custom),
            GameMode.HideAndSeek => (HnSDescription, "HnS", CustomColorManager.HideAndSeek),
            GameMode.TaskRace => (TRDescription, "TR", CustomColorManager.TaskRace),
            _ => ("Invalid", "Invalid", CustomColorManager.GameMode)
        };
    }

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Mode.GameModeName(true)}");
        result += "\n" + ColorIt($"Short Form: {Short}");
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}

public class SymbolInfo(string name, string symbol, string description, UColor color, bool footer = false) : Info(name, "", description, color, InfoType.GameMode, footer)
{
    public string Symbol { get; } = symbol;

    public override void WikiEntry(out string result)
    {
        base.WikiEntry(out result);
        result += ColorIt($"Name: {Name}");
        result += "\n" + ColorIt($"Symbol: {Symbol}");
        result += "\n" + ColorIt(WrapText($"Description: {Description}"));
    }
}