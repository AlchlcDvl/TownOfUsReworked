namespace TownOfUsReworked.Modules
{
    public class Info
    {
        public string Name;
        public string Short;
        public string Description;

        public static readonly List<Info> AllInfo = new();

        public Info(string name, string shortF, string description)
        {
            Name = name;
            Short = shortF;
            Description = description;
            AllInfo.Add(this);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Short Form: ").AppendLine(Short)
                .Append("Description: ").AppendLine(Description);
            return builder.ToString();
        }
    }

    public class RoleInfo : Info
    {
        public string Alignment;
        public string WinCon;
        public string Quote;

        private const string IntruderObjective = "Have a critical sabotage set off by the Intruders reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew and opposing "
            + "Neutrals.";
        private const string SyndicateObjective = "Have a critical sabotage set off by the Syndicate reach 0 seconds or kill off all Intruders, Unfaithful Syndicate, Crew and opposing "
            + "Neutrals.";
        private const string CrewObjective = "Finish tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, and opposing Neutrals.";

        public RoleInfo(string name, string shortF, string description, RoleAlignment alignmentEnum, Faction faction, string quote, string wincon = "") : base(name, shortF, description)
        {
            Quote = quote;
            Alignment = alignmentEnum.AlignmentName();
            WinCon = faction switch
            {
                Faction.Syndicate => SyndicateObjective,
                Faction.Crew => CrewObjective,
                Faction.Intruder => IntruderObjective,
                Faction.Neutral => wincon,
                _ => "Invalid"
            };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Short Form: ").AppendLine(Short)
                .Append("Alignment: ").AppendLine(Alignment)
                .Append("Win Condition: ").AppendLine(WinCon)
                .Append("Description: ").AppendLine(Description)
                .AppendLine()
                .Append('"').Append(Quote).AppendLine("\"");
            return builder.ToString();
        }
    }

    public class FactionInfo : Info
    {
        private const string SyndicateDescription = "Each member of this faction has a special ability and then after a certain number of meetings, can also kill. The main theme of " +
            "this faction is chaos. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. After a certain number " +
            "of meetings, the Syndicate can retreive the \"Chaos Drive\" which gives the holder the ability to kill (if they couldn't already) while also enhancing their existing " +
            "abilities.";
        private const string CrewDescription = "Each member has a special ability which determines who’s who and can help weed out the evils. The main theme of this faction is " +
            "deduction and goodwill. This faction is an uninformed majority meaning they make up most of the players and don't who the other members are. The Crew can do tasks which " +
            "sort of act like a timer for non-Crew roles.";
        private const string IntruderDescription = "minority meaning they make up a tiny fraction of the crew and know who the other members are. All members can sabotage to distract "
            + "the Crew from their tasks.";
        private const string NeutralDescription = "Neutrals are essentially factionless. Each member of this faction has their own unique way to win, seperate from the other roles in" +
            " the same faction. The main theme of this faction is free for all. This faction is an uninformed minority of the game, meaning they make up a small part of the crew " +
            "while not knowing who the other members are. Each role is unique in its own way, some can be helpful, some exist to destroy others and some just exist for the sake of " +
            "existing.";

        public FactionInfo(Faction faction) : base($"{faction}", "", "")
        {
            (Description, Short) = faction switch
            {
                Faction.Syndicate => (SyndicateDescription, "Syn"),
                Faction.Crew => (CrewDescription, "Crew"),
                Faction.Intruder => (IntruderDescription, "Int"),
                Faction.Neutral => (NeutralDescription, "Neut"),
                _ => ("Invalid", "Invalid")
            };
        }
    }

    public class SubFactionInfo : Info
    {
        private const string CabalDescription = "The Cabal is an oraganisation that's similar to the Syndicate. They, however, operate covertly by secretly recruiting people to join their"
            + " group. The Cabal starts of very strong so they are of a higher priority when dealing with enemies. The Cabal is led by the Jackal.";
        private const string UndeadDescription = "The Undead are a group of bloodthirsty vampires who closely grow their numbers. The longer the game goes on, the higher their priority" +
            " on the elimination list. If a member of this subfaction interacts with a Vapire Hunter, the interactor will be killed by the Vampire Hunter in question. The Undead is led " +
            "by the Dracula.";
        private const string ReanimatedDescription = "The Reanimated are a bunch of players who have died yet hold a grudge agaisnt the living. Their grudge is made possible by the " +
            "Necromancer, who leads them. The longer the game goes on with no deaths, the higher the chances of a Necromancer at work.";
        private const string SectDescription = "The Sect is a cult which can gain massive amounts of followers in one go. It may be weak at the start, but do not understimate their " +
            "powerful growth as it may overrun you. The Sect is led by the Whisperer.";

        public SubFactionInfo(SubFaction sub) : base($"{sub}", "", "")
        {
            (Description, Short) = sub switch
            {
                SubFaction.Undead => (UndeadDescription, "Und"),
                SubFaction.Reanimated => (ReanimatedDescription, "RA"),
                SubFaction.Cabal => (CabalDescription, "Cab"),
                SubFaction.Sect => (SectDescription, "Sect"),
                _ => ("Invalid", "Invalid")
            };
        }
    }

    public class AlignmentInfo : Info
    {
        private const string CPDescription = "Crew (Protective) roles have the capability to stop someone from losing their life or bring back the dead.";
        private const string CIDescription = "Crew (Investigative) roles have the ability to gain information via special methods. Using the acquired info, Crew (Investigative) roles "
            + "can deduce who is good and who is not.";
        private const string CUDescription = "Crew (Utility) roles usually don't appear under regaular spawn conditions.";
        private const string CSDescription = "Crew (Support) roles are roles with miscellaneous abilities. Try not to get lost because if you are not paying attention, your chances " +
            "of winning will be severely decreased because of them.";
        private const string CADescription = "Crew (Auditor) roles are special roles that spawn under certain conditions. They exist for the demise of rival subfactions.";
        private const string CKDescription = "Crew (Killing) roles have no aversion to killing like the rest of the Crew and if left alone and potentially wreck the chances of evils " +
            "winning.";
        private const string CSvDescription = "Crew (Sovereign) roles are democratic roles with powers over votes. They are the most powerful during a meeting, so avoid too many " +
            "meetings while they are active.";
        private const string CDDescription = "Crew (Deception) roles are defected Intruder (Deception) roles who have sided with the Crew.";
        private const string CCDescription = "Crew (Concealing) roles are defected Intruder (Concealing) roles who have sided with the Crew.";
        private const string CDiDescription = "Crew (Disruption) roles are defected Syndicate (Disruption) roles who have sided with the Crew.";
        private const string CPowDescription = "Crew (Power) roles are defected Syndicate (Power) roles who have sided with the Crew.";

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

        private const string NBDescription = "Neutral (Benign) roles are special roles that have the capability to win with anyone, as long as a certain condition is fulfilled by the" +
            " end of the game.";
        private const string NKDescription = "Neutral (Killing) roles are roles that have the ability to kill and do not side with anyone. Each role has a special way to kill and gain "
            + "large body counts in one go. Steer clear of them if you don't want to die.";
        private const string NEDescription = "Neutral (Evil) roles are roles whose objectives clash with those of other roles. As such, you need to ensure they don't have a chance at" +
            " winning or when they do win, you have their cooperation.";
        private const string NPDescription = "Neutral (Proselyte) roles are special roles that have their specific ways to win and do not spawn in naturally. Each role here is unique in " +
            "its own way and more often than not they are against you.";
        private const string NNDescription = "Neutral (Neophyte) roles are roles that can convert someone to side with them. Be careful of them, as they can easily overrun you with their" +
            " numbers.";
        private const string NADescription = "Neutral (Apocalypse) roles are powerful roles that appear when a Neutral (Harbinger) role has completed their objective. You will be " +
            "optionally alerted when a Neutral (Apocalypse) role appears.";
        private const string NHDescription = "Neutral (Harbinger) roles are weak roles who only have one goal, complete their objective and become a Neutral (Apocalypse) role.";
        private const string NDiDescription = "Neutral (Disruption) roles are defected Syndicate (Disruption) roles who have broken away from the Syndicate.";
        private const string NPowDescription = "Neutral (Power) roles are defected Syndicate (Power) roles who have broken away from the Syndicate.";
        private const string NProtDescription = "Neutral (Protective) roles are Crew (Protective) roles that have betrayed the Crew.";
        private const string NIDescription = "Neutral (Investigative) roles are Crew (Investigative) roles that have betrayed the Crew.";
        private const string NAudDescription = "Neutral (Auditor) roles are Crew (Auditor) roles that have betrayed the Crew.";
        private const string NSvDescription = "Neutral (Sovereign) roles are Crew (Sovereign) roles that have betrayed the Crew.";
        private const string NDDescription = "Neutral (Deception) roles are defected Intruder (Deception) roles who have broken away from the Intruders.";
        private const string NCDescription = "Neutral (Concealing) role are defected Intruder (Concealing) roles who have broken away from the Intruders.";
        private const string NUDescription = "Neutral (Utility) roles are defected Crew, Intruder or Syndicate (Utility) roles who have broken away from their respective faction.";
        private const string NSDescription = "Neutral (Support) roles are defected Crew, Intruder or Syndicate (Support) roles who have broken away from their respective faction.";

        public AlignmentInfo(RoleAlignment alignmentEnum) : base(alignmentEnum.AlignmentName(), "", "")
        {
            (Short, Description) = alignmentEnum switch
            {
                RoleAlignment.CrewSupport => ("CS", CSDescription),
                RoleAlignment.CrewInvest => ("CI", CIDescription),
                RoleAlignment.CrewProt => ("CP", CPDescription),
                RoleAlignment.CrewKill => ("CK", CKDescription),
                RoleAlignment.CrewUtil => ("CU", CUDescription),
                RoleAlignment.CrewSov => ("CSv", CSvDescription),
                RoleAlignment.CrewAudit => ("CA", CADescription),
                RoleAlignment.CrewConceal => ("CC", CCDescription),
                RoleAlignment.CrewDecep => ("CD", CDDescription),
                RoleAlignment.CrewPower => ("CPow", CPowDescription),
                RoleAlignment.CrewDisrup => ("CDi", CDiDescription),
                RoleAlignment.IntruderSupport => ("IS", ISDescription),
                RoleAlignment.IntruderConceal => ("IC", ICDescription),
                RoleAlignment.IntruderDecep => ("ID", IDDescription),
                RoleAlignment.IntruderKill => ("IK", IKDescription),
                RoleAlignment.IntruderUtil => ("IU", IUDescription),
                RoleAlignment.IntruderInvest => ("II", IIDescription),
                RoleAlignment.IntruderProt => ("IP", IPDescription),
                RoleAlignment.IntruderSov => ("ISv", ISvDescription),
                RoleAlignment.IntruderAudit => ("IA", IADescription),
                RoleAlignment.IntruderPower => ("IPow", IPowDescription),
                RoleAlignment.IntruderDisrup => ("IDi", IDiDescription),
                RoleAlignment.NeutralKill => ("NK", NKDescription),
                RoleAlignment.NeutralNeo => ("NN", NNDescription),
                RoleAlignment.NeutralEvil => ("NE", NEDescription),
                RoleAlignment.NeutralBen => ("NB", NBDescription),
                RoleAlignment.NeutralPros => ("NP", NPDescription),
                RoleAlignment.NeutralApoc => ("NA", NADescription),
                RoleAlignment.NeutralHarb => ("NH", NHDescription),
                RoleAlignment.NeutralInvest => ("NI", NIDescription),
                RoleAlignment.NeutralAudit => ("NAud", NAudDescription),
                RoleAlignment.NeutralSov => ("NSv", NSvDescription),
                RoleAlignment.NeutralProt => ("NProt", NProtDescription),
                RoleAlignment.NeutralSupport => ("NS", NSDescription),
                RoleAlignment.NeutralUtil => ("NU", NUDescription),
                RoleAlignment.NeutralConceal => ("NC", NCDescription),
                RoleAlignment.NeutralDecep => ("ND", NDDescription),
                RoleAlignment.NeutralDisrup => ("NDi", NDiDescription),
                RoleAlignment.NeutralPower => ("NPow", NPowDescription),
                RoleAlignment.SyndicateKill => ("SyK", SyKDescription),
                RoleAlignment.SyndicateSupport => ("SSu", SSuDescription),
                RoleAlignment.SyndicateDisrup => ("SD", SDDescription),
                RoleAlignment.SyndicatePower => ("SP", SPDescription),
                RoleAlignment.SyndicateUtil => ("SU", SUDescription),
                RoleAlignment.SyndicateInvest => ("SI", SIDescription),
                RoleAlignment.SyndicateProt => ("SProt", SProtDescription),
                RoleAlignment.SyndicateSov => ("SSv", SSvDescription),
                RoleAlignment.SyndicateAudit => ("SA", SADescription),
                RoleAlignment.SyndicateConceal => ("SC", SCDescription),
                RoleAlignment.SyndicateDecep => ("SDe", SDeDescription),
                _ => ("Invalid", "Invalid")
            };
        }
    }

    public class ModifierInfo : Info
    {
        public string AppliesTo;

        public ModifierInfo(string name, string shortF, string description, string applies) : base(name, shortF, description) => AppliesTo = applies;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Short Form: ").AppendLine(Short)
                .Append("Applies To: ").AppendLine(AppliesTo)
                .Append("Description: ").AppendLine(Description);
            return builder.ToString();
        }
    }

    public class ObjectifierInfo : Info
    {
        public string AppliesTo;
        public string WinCon;
        public string Symbol;

        public ObjectifierInfo(string name, string shortF, string description, string wincon, string applies, string symbol) : base(name, shortF, description)
        {
            AppliesTo = applies;
            WinCon = wincon;
            Symbol = symbol;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Short Form: ").AppendLine(Short)
                .Append("Symbol: ").AppendLine(Symbol)
                .Append("Applies To: ").AppendLine(AppliesTo)
                .Append("Win Condition: ").AppendLine(WinCon)
                .Append("Description: ").AppendLine(Description);
            return builder.ToString();
        }
    }

    public class AbilityInfo : Info
    {
        public string AppliesTo;

        public AbilityInfo(string name, string shortF, string description, string applies) : base(name, shortF, description) => AppliesTo = applies;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Short Form: ").AppendLine(Short)
                .Append("Applies To: ").AppendLine(AppliesTo)
                .Append("Description: ").AppendLine(Description);
            return builder.ToString();
        }
    }

    public class Lore : Info
    {
        public string Story;

        public Lore(string name, string story, string shortF) : base(name, shortF, "") => Story = story;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Lore: ").AppendLine(Story);
            return builder.ToString();
        }
    }

    public class OtherInfo : Info
    {
        public string OtherNotes;

        public OtherInfo(string name, string shortF, string description, string otherNotes = "") : base(name, shortF, description) => OtherNotes = otherNotes;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Name: ").AppendLine(Name)
                .Append("Short Form: ").AppendLine(Short)
                .Append("Description: ").AppendLine(Description);

            if (OtherNotes != "")
                builder.AppendLine(OtherNotes);

            return builder.ToString();
        }
    }
}