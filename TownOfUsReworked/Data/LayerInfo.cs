namespace TownOfUsReworked.Data
{
    [HarmonyPatch]
    public static class LayerInfo
    {
        public class RoleInfo
        {
            public string Name;
            public string Short;
            public string Alignment;
            public string Description;
            public string WinCon;
            public string Quote;

            private const string IntruderObjective = "Have a critical sabotage set off by the Intruders reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew and " +
                "opposing Neutrals.";
            private const string SyndicateObjective = "Have a critical sabotage set off by the Syndicate reach 0 seconds or kill off all Intruders, Unfaithful Syndicate, " +
                "Crew and opposing Neutrals.";
            private const string CrewObjective = "Finish tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, and opposing Neutrals.";

            public RoleInfo(string name, string shortF, string description, RoleAlignment alignmentEnum, Faction faction, string quote, string wincon = "")
            {
                Name = name;
                Short = shortF;
                Description = description;
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

            public string InfoMessage()
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

        public class FactionInfo
        {
            public string Name;
            public string Short;
            public string Description;

            private const string SyndicateDescription = "Each member of this faction has a special ability and then after a certain number of meetings, can also kill. The main theme " +
                "of this faction is chaos. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. After a certain " +
                "number of meetings, the Syndicate can retreive the \"Chaos Drive\" which gives the holder the ability to kill (if they couldn't already) while also enhancing their " +
                "existing abilities.";
            private const string CrewDescription = "Each member has a special ability which determines who’s who and can help weed out the evils. The main theme of this faction is " +
                "deduction and goodwill. This faction is an uninformed majority meaning they make up most of the players and don't who the other members are. The Crew can do tasks which " +
                "sort of act like a timer for non-Crew roles.";
            private const string IntruderDescription = "minority meaning they make up a tiny fraction of the crew and know who the other members are. All members can sabotage to " +
                "distract the Crew from their tasks.";
            private const string NeutralDescription = "Neutrals are essentially factionless. Each member of this faction has their own unique way to win, seperate from the other roles " +
                "in the same faction. The main theme of this faction is free for all. This faction is an uninformed minority of the game, meaning they make up a small part of the " +
                "crew while not knowing who the other members are. Each role is unique in its own way, some can be helpful, some exist to destroy others and some just exist for the " +
                "sake of existing.";

            public FactionInfo(Faction faction)
            {
                Name = $"{faction}";
                (Description, Short) = faction switch
                {
                    Faction.Syndicate => (SyndicateDescription, "Syn"),
                    Faction.Crew => (CrewDescription, "Crew"),
                    Faction.Intruder => (IntruderDescription, "Int"),
                    Faction.Neutral => (NeutralDescription, "Neut"),
                    _ => ("Invalid", "Invalid")
                };
            }

            public string InfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class AlignmentInfo
        {
            public string Name;
            public string Short;
            public string Description;

            private const string CPDescription = "Crew (Protective) roles have the capability to stop someone from losing their life or bring back the dead.";
            private const string CIDescription = "Crew (Investigative) roles have the ability to gain information via special methods. Using the acquired info, " +
                "Crew (Investigative) roles can deduce who is good and who is not.";
            private const string CUDescription = "Crew (Utility) roles usually don't appear under regaular spawn conditions.";
            private const string CSDescription = "Crew (Support) roles are roles with miscellaneous abilities. Try not to get lost because if you are not paying " +
                "attention, your chances of winning will be severely decreased because of them.";
            private const string CADescription = "Crew (Auditor) roles are special roles that spawn under certain conditions. They exist for the demise of rival subfactions.";
            private const string CKDescription = "Crew (Killing) roles have no aversion to killing like the rest of the Crew and if left alone and potentially wreck the chances of " +
                "evil subfactions winning.";
            private const string CSvDescription = "Crew (Sovereign) roles are democratic roles with powers over votes. They are the most powerful during a meeting, so avoid too many " +
                "meetings while they are active.";
            private const string CDDescription = "Crew (Deception) roles are defected Intruder (Deception) roles who have sided with the Crew.";
            private const string CCDescription = "Crew (Concealing) roles are defected Intruder (Concealing) roles who have sided with the Crew.";
            private const string CDiDescription = "Crew (Disruption) roles are defected Syndicate (Disruption) roles who have sided with the Crew.";
            private const string CPowDescription = "Crew (Power) roles are defected Syndicate (Power) roles who have sided with the Crew.";

            private const string ISDescription = "Intruder (Support) roles have miscellaneous abilities. These roles can delay players' chances of winning by" +
                " either gaining enough info to stop them or forcing players to do things they can't.";
            private const string ICDescription = "Intruder (Concealing) roles specialise in hiding information from others. If there is no new " +
                "information, it's probably their work.";
            private const string IDDescription = "Intruder (Deception) roles are built to spread misinformation. Never trust your eyes, for the killer you " +
                "saw in front of you might not be the one who they seem to be.";
            private const string IUDescription = "Intruder (Utility) roles usually don't appear under regaular spawn conditions.";
            private const string IKDescription = "Intruder (Killing) roles have an addition ability to kill. Be careful of them as large numbers of dead bpdies will start to pile up" +
                " with them around.";
            private const string IPDescription = "Intruder (Protective) roles are Crew (Protective) roles that have betrayed the Crew to join the Intruders.";
            private const string IIDescription = "Intruder (Investigative) roles are Crew (Investigative) roles that have betrayed the Crew to join the Intruders.";
            private const string IADescription = "Intruder (Auditor) roles are Crew (Auditor) roles that have betrayed the Crew to join the Intruders.";
            private const string ISvDescription = "Intruder (Sovereign) roles are Crew (Sovereign) roles that have betrayed the Crew to join the Intruders.";
            private const string IDiDescription = "Intruder (Disruption) roles are defected Syndicate (Disruption) roles who have sided with the Intruders.";
            private const string IPowDescription = "Intruder (Power) roles are defected Syndicate (Power) roles who have sided with the Intruders.";

            private const string SUDescription = "Syndicate (Utility) roles usually don't appear under regaular spawn conditions.";
            private const string SSuDescription = "Syndicate (Support) roles have miscellaneous abilities. They are detrimental to the Syndicate's cause and if" +
                " used right, can greatly affect how the game continues.";
            private const string SDDescription = "Syndicate (Disruption) roles are designed to change the flow of the game, via changing some mechanic.";
            private const string SyKDescription = "Syndicate (Killing) roles are powerful killers unique ways to rack up body counts.";
            private const string SPDescription = "Syndicate (Power) roles are powerful disruptors with a knack for chaos and destruction.";
            private const string SProtDescription = "Syndicate (Protective) roles are Crew (Protective) roles that have betrayed the Crew to join the Syndicate.";
            private const string SIDescription = "Syndicate (Investigative) roles are Crew (Investigative) roles that have betrayed the Crew to join the Syndicate.";
            private const string SADescription = "Syndicate (Auditor) roles are Crew (Auditor) roles that have betrayed the Crew to join the Syndicate.";
            private const string SSvDescription = "Syndicate (Sovereign) roles are Crew (Sovereign) roles that have betrayed the Crew to join the Syndicate.";
            private const string SDeDescription = "Syndicate (Deception) roles are defected Intruder (Deception) roles who have sided with the Syndicate.";
            private const string SCDescription = "Syndicate (Concealing) roles are defected Intruder (Concealing) roles who have sided with the Syndicate.";

            private const string NBDescription = "Neutral (Benign) roles are special roles that have the capability to win with anyone, as long as a certain " +
                "condition is fulfilled by the end of the game.";
            private const string NKDescription = "Neutral (Killing) roles are roles that have the ability to kill and do not side with anyone. Each role has a special way" +
                " to kill and gain large body counts in one go. Steer clear of them if you don't want to die.";
            private const string NEDescription = "Neutral (Evil) roles are roles whose objectives clash with those of other roles. As such, you need to ensure they don't have a chance" +
                " at winning or when they do win, you have their cooperation.";
            private const string NPDescription = "Neutral (Proselyte) roles are special roles that have their specific ways to win. Each role here is unique in its own way and " +
                "more often than not they are against you.";
            private const string NNDescription = "Neutral (Neophyte) roles are roles that can convert someone to side with them. Be careful of them, as they can easily overrun you " +
                "with their numbers.";
            private const string NADescription = "Neutral (Apocalypse) roles are powerful roles that appear when a Neutral (Harbinger) role has completed their objective. You will be " +
                "alerted when a Neutral (Apocalypse) role appears.";
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

            public AlignmentInfo(RoleAlignment alignmentEnum)
            {
                Name = alignmentEnum.AlignmentName();
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

            public string InfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class ModifierInfo
        {
            public string Name;
            public string Short;
            public string Description;
            public string AppliesTo;

            public ModifierInfo(string name, string shortF, string description, string applies)
            {
                Name = name;
                Short = shortF;
                Description = description;
                AppliesTo = applies;
            }

            public string InfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Applies To: ").AppendLine(AppliesTo)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class ObjectifierInfo
        {
            public string Name;
            public string Short;
            public string AppliesTo;
            public string Description;
            public string WinCon;
            public string Symbol;

            public ObjectifierInfo(string name, string shortF, string description, string wincon, string applies, string symbol)
            {
                Name = name;
                Short = shortF;
                AppliesTo = applies;
                Description = description;
                WinCon = wincon;
                Symbol = symbol;
            }

            public string InfoMessage()
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

        public class AbilityInfo
        {
            public string Name;
            public string Short;
            public string AppliesTo;
            public string Description;

            public AbilityInfo(string name, string shortF, string description, string applies)
            {
                Name = name;
                Short = shortF;
                AppliesTo = applies;
                Description = description;
            }

            public string InfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Applies To: ").AppendLine(AppliesTo)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class Lore
        {
            public string Name;
            public string Short;
            public string Story;

            public Lore(string name, string story, string shortF)
            {
                Name = name;
                Story = story;
                Short = shortF;
            }

            public string InfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name)
                    .Append("Lore: ").AppendLine(Story);
                return builder.ToString();
            }
        }

        public readonly static List<RoleInfo> AllRoles = new()
        {
            new("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new("Altruist", "Alt", "The Altruist is capable of reviving dead players. After a set period of time, the player will be resurrected, if the revival isn't " +
                "interrupted. Once a player is revived, all evil players will be notified of the revival and will have an arrow pointing towards the revived player. Once the Altruist " +
                "uses up all of their ability charges, they sacrifice themselves on the last use of their ability.", RoleAlignment.CrewProt, Faction.Crew, "I know what I have to do but " +
                "I don't know if I have the strength to do it."),
            new("Chameleon", "Cham", "The Chameleon can go invisible to stalk players and see what they do when no one is around.", RoleAlignment.CrewSupport,
                Faction.Crew, "Are you sure you can see me?"),
            new("Coroner", "Cor", "The Coroner gets an alert when someone dies. On top of this, the Coroner briefly gets an arrow pointing in the direction of " +
                "the body. They can autopsy bodies to get some information. They can then compare that information with players to see if they killed the body or not. The Coroner also " +
                "gets a body report from the player they reported. The report will include the cause and time of death, player's faction/role, the killer's faction/role and (according " +
                "to the settings) the killer's name.", RoleAlignment.CrewInvest, Faction.Crew, "A body? Where? I need it for...scientific purposes."),
            new("Crewmate", "Crew", "Just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.",
                RoleAlignment.CrewUtil, Faction.Crew, "I once made a pencil using 2 erasers...they were pointless, just like me."),
            new("Engineer", "Engi", "The Engineer can fix sabotages from anywhere on the map. They can also use vents to get across the map easily.",
                RoleAlignment.CrewSupport, Faction.Crew, "How am I going to stop some big mean mother hubbard from tearing me a structurally superfluous new behind? The solution? A " +
                "wrench."),
            new("Escort", "Esc", "The Escort can roleblock players and prevent them from doing anything for a short while.", RoleAlignment.CrewSupport, Faction.Crew,
                "Today, I will make you a man."),
            new("Inspector", "Insp", "The Inspector can check players for their roles. Upon being checked, the targets' names will be updated to give a list of what" +
                " roles could the target possibly be.", RoleAlignment.CrewInvest, Faction.Crew, "THAT'S THE GODFATHER! YOU GOTTA BELIEVE ME."),
            new("Mayor", "Mayo (XD)", "The Mayor has no active ability aside from being able to reveal themselves as the Mayor to other players. Upon doing so, their vote " +
                "counts as extra.", RoleAlignment.CrewSov, Faction.Crew, "Um, those votes are legitimate. No, the votes are not rigged."),
            new("Medic", "Medic", "The Medic can give any player a shield that will make them immortal until the Medic is dead. A shielded player cannot be " +
                "killed by anyone, unless it's a suicide. Shielded players have a green ✚ next to their names.", RoleAlignment.CrewProt, Faction.Crew, "Where does it hurt?"),
            new("Medium", "Med", "The Medium can mediate to be able to see ghosts. If the Medium uses this ability, the Medium and the dead player will be able to " +
                "see each other and communicate from beyond the grave!", RoleAlignment.CrewInvest, Faction.Crew, "The voices...they are telling me...my breath stinks? Can ghosts even " +
                "smell?"),
            new("Mystic", "Mys", "The Mystic only spawns when there is at least one Neutral (Neophyte) role present in the game. Whenever someone's subfaction is " +
                "changed, the Mystic will be alerted about it. The Mystic can also investigate players to see if their subfactions have been changed. If the target has a different " +
                "subfaction, the Mystic's screen will flash red, otherwise it will flash green. It will not, however, work on the Neutral (Neophyte) roles themselves so even people who" +
                " flashed green might be a converter. Once all subfactions are dead, the Mystic becomes a Seer.", RoleAlignment.CrewAudit, Faction.Crew, "There's a hint of corruption."),
            new("Operative", "Op", "The Operative can place bugs around the map. When players enter the range of the bug, they trigger it. In the following meeting, all " +
                "players who triggered a bug will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the bug, nor what role " +
                "a specific player is. The Operative also gains more information when on Admin Table and on Vitals. On Admin Table, the Operative can see the colors of every person on " +
                "the map. When on Vitals, the Operative is shown how long someone has been dead for.", RoleAlignment.CrewInvest, Faction.Crew, "The only thing you need to find out " +
                "information is good placement and amazing levels of patience."),
            new("Retributionist", "Ret", "The Retributionist can mimic dead crewamtes. During meetings, the Retributionist can select who they are going to " +
                "ressurect and use for the following round from the dead. They can choose to use each dead players as many times as they wish. It should be noted the Retributionist " +
                "can not use all Crew roles and cannot use any Non-Crew role. The cooldowns, limits and everything will be set by the settings for their respective roles.",
                RoleAlignment.CrewSupport, Faction.Crew, "Bodies...bodies...I NEED BODIES."),
            new("Seer", "Seer", "The Seer only spawns if there are roles capable of changing their initial roles. The Seer can investigate players to see if their" +
                " role is different from what they started out as. If a player's role has been changed, the Seer's screen will flash red, otherwise it will flash green. This, however, " +
                "does not work on those whose subfactions have changed so those who flashed green might still be evil. If all players capable of changing or have changed their initial " +
                "roles are dead, the Seer becomes a Sheriff.", RoleAlignment.CrewInvest, Faction.Crew, "You've got quite the history."),
            new("Sheriff", "Sher", "The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a role is Good or " +
                "Evil. A player's name will change color according to their results.", RoleAlignment.CrewInvest, Faction.Crew, "Guys I promise I'm not an Executioner, I checked Blue and" +
                " they're sus."),
            new("Shifter", "Shift", "The Shifter can swap roles with someone, as long as they are Crew. If the shift is unsuccessful, the Shifter will die.",
                RoleAlignment.CrewSupport, Faction.Crew, "GET BACK HERE I WANT YOUR ROLE."),
            new("Tracker", "Track", "InvaThe Tracker can track other during a round. Once they track someone, an arrow is continuously pointing to them, which " +
                "updates in set intervals.lid", RoleAlignment.CrewInvest, Faction.Crew, "I only took up this job because the others were full. Yes it's a proper job. No, I'm not a " +
                "stalker."),
            new("Transporter", "Trans", "The Transporter can swap the locations of two players at will. Players who have been transported are alerted with a blue " +
                "flash on their screen.", RoleAlignment.CrewSupport, Faction.Crew, "You're here and you're there. Where will you go? That's for me to decide."),
            new("Vampire Hunter", "VH", "The Vampire Hunter only spawns if there are Undead in the game. They can check players to see if they are an Undead. When " +
                "the Vampire Hunter finds them, the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a " +
                "Vigilante.", RoleAlignment.CrewAudit, Faction.Crew, "The Dracula could be anywhere! He could be you! He could be me! He could even be- *gets voted off*"),
            new("Veteran", "Vet", "The Veteran can go on alert. When the Veteran is on alert, anyone, even if Crew, who interacts with the Veteran dies.",
                RoleAlignment.CrewKill, Faction.Crew, "Touch me, I dare you."),
            new("Vigilante", "Vig", "The Vigilante can kill. However, if they kill someone they shouldn't, they instead die themselves.", RoleAlignment.CrewKill,
                Faction.Crew, "I AM THE HAND OF JUSTICE."),
            new("Actor", "Act", "The Actor gets a list of roles at the start of the game. The Actor must pretend to be and get guessed as one of the roles in order" +
                " to win.", RoleAlignment.NeutralEvil, Faction.Neutral, "I am totally what you think of me as.", "Get guessed as a role in your target role list."),
            new("Amnesiac", "Amne", "The Amnesiac is essentially roleless and cannot win without remembering the role of a dead player.", RoleAlignment.NeutralBen,
                Faction.Neutral, "I forgor :skull:", "Find a dead body, take their role and then win as that role."),
            new("Arsonist", "Arso", "The Arsonist can douse other players with gasoline. After dousing, the Arsonist can choose to ignite all doused players which " +
                "kills all doused players at once. Doused players have an orange Ξ next to their names", RoleAlignment.NeutralKill, Faction.Neutral, "I like my meat well done.",
                "Douse and ignite anyone who can oppose them"),
            new("Bounty Hunter", "BH", "The Bounty Hunter is assigned a target as the start of the game. Every meeting, the Bounty Hunter is given clue to who their target " +
                "might be. They do not know who the target is and must find them via a series of clues and limited guesses. Upon finding their target within the set amount of guesses," +
                " the guess button becomes a kill button. The Bounty Hunter's target always knows that there is a bounty on their head. If the Bounty Hunter is unable to find their " +
                "target within the number of guesses or their target dies not by the Bounty Hunter's hands, the Bounty Hunter turns into a Troll. The target has a red Θ next to " +
                "their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "You can run, but you can't hide.", "Find and kill their bounty."),
            new("Cannibal", "Cann", "The Cannibal can eat the body which wipes away the body, like the Janitor.", RoleAlignment.NeutralEvil, Faction.Neutral, "How " +
                "do you survive with no food but with a lot of people? Improvise, adapt, overcome.", "Eat a certain number of bodies."),
            new("Cryomaniac", "Cryo", "The Cryomaniac can douse in coolant and freeze players similar to the Arsonist's dousing in gasoline and ignite. Freezing " +
                "players does not immediately kill doused targets, instead when the next meeting is called, all currently doused players will die. When the Cryomaniac is the last killer" +
                " or when the final number of players reaches a certain threshold, the Cryomaniac can also directly kill. Doused players have a purple λ next to their names.",
                RoleAlignment.NeutralKill, Faction.Neutral, "Anybody wants ice scream?", "Douse and freeze anyone who can oppose them."),
            new("Dracula", "Drac", "The Dracula is the only Undead that spawns in. The Dracula is the leader of the Undead who can convert others into Undead. If " +
                "the target cannot be converted, they will be attacked instead. The Dracula must watch out for the Vampire Hunter as attempting to convert them will cause the Vampire " +
                "Hunter to kill the Dracula.", RoleAlignment.NeutralNeo, Faction.Neutral, "Everyone calls me a pain in the neck.", "Convert or kill anyone who can oppose the Undead."),
            new("Executioner", "Exe", "The Executioner has no abilities and instead must use gas-lighting techniques to get their target ejected. The Executioner's" +
                " target, by default, is always non-Crew Sovereign Crew. Once their target is ejected, the Executioner can doom those who voted for their target. If their target dies " +
                "before ejected, the Executioner turns into a Jester. Targets have a grey § next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "Source: trust me bro.",
                "Live to see their target ejected."),
            new("Glitch", "Gli", "The Glitch can hack players, resulting in them being unable to do anything for a set duration or they can also mimic someone, " +
                "which results in them looking exactly like the other person. The Glitch can kill normally.", RoleAlignment.NeutralKill, Faction.Neutral, "Hippity hoppity, your code is " +
                "now my property.", "Hippity hoppity, your code is now my property."),
            new("Guesser", "Guess", "The Guesser has no abilities aside from guessing only their target. Every meeting, the Guesser is told a hint regarding their " +
                "target's role. Targets have a beige π next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "I want to know what you are.", "Guess their target's role."),
            new("Jackal", "Jack", "The Jackal is the leader of the Cabal. They spawn in with 2 recruits at the start of the game. One of the recruits is the " +
                "'good' one, meaning they are Crew. The other is the 'evil' recruit, who can be either Intruder, Syndicate or Neutral (Killing). When both recruits die, the Jackal can " +
                "then recruit another player to join the Cabal and become the backup recruit. If the target happens to be a member of a rival subfaction, they will be attacked instead " +
                "and the Jackal will still lose their ability to recruit. Members of the Cabal have a dark grey $ next to their names.", RoleAlignment.NeutralNeo, Faction.Neutral,
                "I've got money.", "Recruit or kill anyone who can oppose Cabal."),
            new("Guardian Angel", "GA", "The Guardian Angel more or less aligns themselves with the faction of their target. The Guardian Angel will win with anyone" +
                " as long as their target lives to the end of the game, even if their target loses. If the Guardian Angel's target dies, they become a Survivor. Targets have a white ★ " +
                "and a white η when being protected next to their names.", RoleAlignment.NeutralBen, Faction.Neutral, "Hush child...Mama's here.", "Have their target live to the end of " +
                "the game."),
            new("Jester", "Jest", "The Jester has no abilities and must make themselves appear to be evil to the Crew and get ejected. After getting ejected, the " +
                "Jester can haunt those who voted for them, killing them from beyond the grave.", RoleAlignment.NeutralEvil, Faction.Neutral, "Hehehe I wonder if I do this...",
                "Get ejected."),
            new("Juggernaut", "Jugg", "The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill " +
                "cooldown no longer decreases and instead gives them other buffs, like bypassing protections.", RoleAlignment.NeutralKill, Faction.Neutral, "The doctor told me bones grow" +
                " stronger when recieving damage. But then why did he kick me out when I picked up a hammer?", "Assault anyone who can oppose them."),
            new("Revealer", "Rev", "The Revealer can reveal evils if they finish all their tasks. Upon finishing all of their tasks, Intruders, Syndicate and " +
                "sometimes Neutrals are revealed to alive Crew after a meeting is called. However, if the Revealer is clicked they lose their ability to reveal evils and are once again " +
                "a normal ghost.", RoleAlignment.CrewUtil, Faction.Crew, "I have no idea who I am or what I do, the only thing I see is bad guys who I must reveal."),
            new("Murderer", "Murd", "The Murderer is a simple Neutral Killer with no special abilities.", RoleAlignment.NeutralKill, Faction.Neutral, "I like my " +
                "women like how I like my knives, sharp and painful.", "Murder anyone who can oppose them."),
            new("Necromancer", "Necro", "The Necromancer is essentially an evil Altruist. They can revive dead players and make them join the Necromancer's " +
                "team, the Reanimated. There is a limit to how many times can the Necromancer can kill and revive players.", RoleAlignment.NeutralNeo, Faction.Neutral, "I like the dead," +
                " they do a lot of things I like. For example, staying dead.", "Resurrect the dead and kill off anyone who can oppose the Reanimated."),
            new("Pestilence", "Pest", "Pestilence is always on permanent alert, where anyone who tries to interact with them will die. Pestilence does not spawn " +
                "in-game and instead gets converted from Plaguebearer after they infect everyone. Pestilence cannot die unless they have been voted out, and they can't be guessed " +
                "(usually). This role does not spawn directly, unless it's set to, in which case it will replace the Plaguebearer.", RoleAlignment.NeutralApoc, Faction.Neutral,
                "I am the god of disease, nothing can kill me. *Voice from the distance* Ejections can!", "Obliterate anyone who can oppose them."),
            new("Phantom", "Phan", "The Phantom spawns when a Neutral player dies withouth accomplishing their objective. They become half-invisible and have to complete all " +
                "their tasks without getting clicked on to win.", RoleAlignment.NeutralPros, Faction.Neutral, "I'm the one who you should not have killed. *Voice from the distance* " +
                "Get outta here! This is not FNAF!", "Finish tasks without getting caught."),
            new("Plaguebearer", "PB", "The Plaguebearer can infect other players. Once infected, the infected player can go and infect other players via interacting with them. " +
                "Once all players are infected, the Plaguebearer becomes Pestilence.", RoleAlignment.NeutralHarb, Faction.Neutral, "*Cough* This should surely work right? *Cough* I sure" +
                " hope it does.", "Infect everyone to become pestilence or kill anyone who can oppose them."),
            new("Serial Killer", "SK", "Although the Serial Killer has a kill button, they can't use it unless they are in Bloodlust. Once the Serial Killer is in bloodlust " +
                "they gain the ability to kill. However, unlike most killers, their kill cooldown is really short for the duration of the bloodlust.", RoleAlignment.NeutralKill,
                Faction.Neutral, "My knife, WHERE'S MY KNIFE?!", "Stab anyone who can oppose them."),
            new("Survivor", "Surv", "The Survivor wins by simply surviving. They can vest which makes them immortal for a short duration.", RoleAlignment.NeutralBen,
                Faction.Neutral, "Hey listen man, I mind my own business and you mind yours. Everyone wins!", "Live to the end of the game."),
            new("Thief", "Thief", "The Thief can kill players to steal their roles. The player, however, must be a role with the ability to kill otherwise the Thief will " +
                "die. After stealing their target's role, the Thief can now win as whatever role they have become.", RoleAlignment.NeutralBen, Faction.Neutral, "Now it's mine.", "Kill" +
                " and steal someone's role."),
            new("Troll", "Troll", "The Troll just wants to be killed, but not ejected. The Troll can \"interact\" with players. This interaction does nothing, it just triggers" +
                " any interaction sensitive roles like Veteran and Pestilence.", RoleAlignment.NeutralEvil, Faction.Neutral, "Kill me. The Impostor: Later.", "Get killed."),
            new("Werewolf", "WW", "The Werewolf can kill all players within a certain radius.", RoleAlignment.NeutralKill, Faction.Neutral, "AWOOOOOOOOOOOOOOOOOOOO", "Maul" +
                " anyone who can oppose them."),
            new("Whisperer", "Whisp", "The Whisperer can whisper to all players within a certain radius. With each whisper, the chances of bringing someone over to the " +
                "Sect increases till they do convert.", RoleAlignment.NeutralNeo, Faction.Neutral, "Psst.", "Persuade or kill anyone who can oppose the Sect."),
            new("Ambusher", "Amb", "The Ambusher can temporaily force anyone to go on alert, killing anyone who interacts with the Ambusher's target.", RoleAlignment.IntruderKill,
                Faction.Intruder, "BOO"),
            new("Blackmailer", "BM", "The Blackmailer can silence people. During each round, the Blackmailer can go up to someone and blackmail them. This prevents" +
                " the blackmailed person from speaking during the next meeting.", RoleAlignment.IntruderConceal, Faction.Intruder, "Shush."),
            new("Camouflager", "Camo", "The Camouflager does the same thing as the Comms Sabotage, but their camouflage can be stacked on top other sabotages. Camouflaged " +
                "players can kill in front everyone and no one will know who it is.", RoleAlignment.IntruderConceal, Faction.Intruder, "Good luck telling others apart."),
            new("Consigliere", "Consig", "The Consigliere can reveal people's roles. They cannot get Assassin unless they see factions for obvious reasons.",
                RoleAlignment.IntruderSupport, Faction.Intruder, "What...are you?"),
            new("Consort", "Cons", "The Consort can roleblock players and prevent them from doing anything for a short while.", RoleAlignment.IntruderSupport, Faction.Intruder,
                "I'm like the first slice of bread, everyone touches me but no one likes me."),
            new("Disguiser", "Disg", "The Disguiser can disguise into other players. At the beginning of each, they can choose someone to measure. They can then disguise the " +
                "next nearest person into the measured person for a limited amount of time after a short delay.", RoleAlignment.IntruderDecep, Faction.Intruder, "Here, wear this for" +
                " me please. I promise I won't do anything to you."),
            new("Ghoul", "Ghoul", "Every round, the Ghoul can mark a player for death. All players are told who is marked and the marked player will die at the end of the " +
                "next meeting. The only way to save a marked player is to click the Ghoul that marked them.", RoleAlignment.IntruderUtil, Faction.Intruder, "I CURSE YOU!"),
            new("Godfather", "GF", "The Godfather can only spawn in 3+ Intruder games. They can choose to promote a fellow Intruder to Mafioso. When the Godfather dies, " +
                "the Mafioso becomes the new Godfather and has lowered cooldowns.", RoleAlignment.IntruderSupport, Faction.Intruder, "I'm going to make an offer they can't refuse."),
            new("Grenadier", "Gren", "The Grenadier can throw flash grenades which blinds nearby players. However, a sabotage and a flash grenade can not be active at the same" +
                " time.", RoleAlignment.IntruderConceal, Faction.Intruder, "AAAAAAAAAAAAA YOUR EYES"),
            new("Impostor", "Imp", "Just a plain Intruder with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.",
                RoleAlignment.IntruderUtil, Faction.Intruder, "If you ever feel useless, just remember I exist."),
            new("Janitor", "Jani", "The Janitor can drag, drop and clean up bodies. Both their Kill and Clean usually ability have a shared cooldown, meaning they have to choose "
                + "which one they want to use.", RoleAlignment.IntruderConceal, Faction.Intruder, "I'm the guy you call to clean up after you."),
            new("Mafioso", "Mafi", "The Mafioso is promoted from a random non-Godfather Intruder role. The Mafioso by themself is nothing special, but when the Godfather dies," +
                " the Mafioso becomes the new Godfather. As a result, the new Godfather has a lower cooldown on all of their original role's abilities.", RoleAlignment.IntruderUtil,
                Faction.Intruder, "Yes, boss. Got it, boss."),
            new("Miner", "Miner", "The Miner can create new vents. These vents only connect to each other, forming a new passageway.", RoleAlignment.IntruderSupport, Faction.Intruder,
                "Dig, dig, diggin' some rave; making some loud sound waves; the only thing you'll be diggin' is your own grave."),
            new("Morphling", "Morph", "The Morphling can morph into another player. At the beginning of each round, they can choose someone to sample. They can then morph " +
                "into that person at any time for a limited amount of time.", RoleAlignment.IntruderDecep, Faction.Intruder, "*Casually observing the chaos over Green seeing Red kill." +
                "* It was me."),
            new("Teleporter", "Tele", "The Teleporter can teleport to a marked positions. The Teleporter can mark a location which they can then teleport to later.",
                RoleAlignment.IntruderSupport, Faction.Intruder, "He's here, he's there, he's everywhere. Who are ya gonna call? Psychic friend fr-"),
            new("Wraith", "Wraith", "The Wraith can temporarily turn invisible.", RoleAlignment.IntruderDecep, Faction.Intruder, "Now you see me, now you don't."),
            new("Anarchist", "Anarch", "Just a plain Syndicate with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode. Its only " +
                "benefit is its ability to kill from the beginning of the game. With the Chaos Drive, the Anarchist's kill cooldown decreases.", RoleAlignment.SyndicateUtil,
                Faction.Syndicate, "If you ever feel useless, just remember that I also exist."),
            new("Banshee", "Bansh", "The Banshee can block every non-Syndicate player every once in a while. This role cannot get the Chaos Drive.", RoleAlignment.SyndicateUtil,
                Faction.Syndicate, "AAAAAAAAAAAAAAA"),
            new("Concealer", "Conceal", "The Concealer can make a player invisible for a short while. With the Chaos Drive, this applies to everyone.",
                RoleAlignment.SyndicateDisrup, Faction.Syndicate, "Invalid"),
            new("Crusader", "Crus", "The Crusader can temporaily force anyone to go on alert, killing anyone who interacts with the Crusader's target. With the Chaos Drive," +
                " attempting to interact with the Crusader's target will cause the target to kill everyone within a certain range, including the target themselves.",
                RoleAlignment.SyndicateKill, Faction.Syndicate, "I WILL PURGE THIS UNHOLY LAND!"),
            new("Framer", "Framer", "The Framer can frame players, making them appear to have wrong results and be easily killed by Vigilantes and Assassins. This effects " +
                "lasts as long as the Framer is alive. With the Chaos Drive, the Framer can frame players within a certain radius.", RoleAlignment.SyndicateDisrup, Faction.Syndicate,
                "Who knew old documents can get people into trouble?"),
            new("Poisoner", "Pois", "The Poisoner can poison another player instead of killing. When they poison a player, the poisoned player dies either upon the start of the" +
                " next meeting or after a set duration. With the Chaos Drive, the Poisoner can poison a player from anywhere.", RoleAlignment.SyndicateKill, Faction.Syndicate,
                "So now if you mix these together, you end up creating this...thing."),
            new("Rebel", "Reb", "The Rebel can only spawn in 3+ Syndicate games. They can choose to promote a fellow Syndicate to Sidekick. When the Rebel dies, the Sidekick" +
                " becomes the new Rebel and has lowered cooldowns. With the Chaos Drive, the Rebel's gains the improved abilities of their former role. A promoted Rebel has the highest" +
                " priority when recieving the Chaos Drive and the original Rebel as the lowest priority.", RoleAlignment.SyndicateSupport, Faction.Syndicate, "DOWN WITH THE GOVERNMENT!"),
            new("Shapeshifter", "SS", "The Shapeshifter can swap the appearances of 2 players. WIth the Chaos Drive, everyone's appearances are suffled.",
                RoleAlignment.SyndicateDisrup, Faction.Syndicate, "Everyone! We will be playing dress up! TOGETHER!"),
            new("Sidekick", "Side", "The Sidekick is promoted from a random non-Rebel Syndicate role. The Sidekick by themselves is nothing special, but when the Rebel " +
                "dies, the Sidekick becomes the new Rebel. As a result, the new Rebel has a lower cooldown on all of their original role's abilities.", RoleAlignment.SyndicateUtil,
                Faction.Syndicate, "Learning new things."),
            new("Warper", "Warp", "The Warper can teleport a player to another player. With the Chaos Drive, the Warper teleports everyone to random positions on the map.",
                RoleAlignment.SyndicateSupport, Faction.Syndicate, "BEGONE!"),
            new("Enforcer", "Enf", "The Enforcer can plant bombs on players. After a short while, the target will be alerted to the bomb's presence and must kill someone to " +
                "get rid of it. If they fail to kill someone within a certain time limit, tje bomb will explode, killing everyone within its vicinity.", RoleAlignment.IntruderKill,
                Faction.Intruder, "You will do as I say...unless you want to be the painting on the walls."),
            new("Bomber", "Bomb", "The Bomber can place a bomb which can be remotely detonated at any time. Anyone caught inside the bomb's radius at the time of detonation will" +
                " be killed. Only the latest placed bomb will detonate, unless the Bomber holds the Chaos Drive, with which they can detonate all bombs at once.",
                RoleAlignment.SyndicateKill, Faction.Syndicate, "KABOOM!!"),
            new("Detective", "Det", "The Detective can examine other players for bloody hands. If the examined player has killed recently, the Detective will be alerted about " +
                "it. The Detective can also see the footprints of players. All footprints disappear after a set amount of time and only the Detective can see them.",
                RoleAlignment.CrewInvest, Faction.Crew, "I am skilled in identifying blood...yup that's defintely blood."),
            new("Betrayer", "Bet", "The Betrayer is a simple killer, who turned after a turned Traitor/Fanatic was the only member of their new faction remaning. This role does" +
                " not spawn directly.", RoleAlignment.NeutralPros, Faction.Neutral, "The back that trusts me the most is the sweetest to stab", "Kill anyone who opposes the faction " +
                "they defected to"),
            new("Dictator", "Dict", "The Dictator has no active ability aside from revealing themselves as the Dictator to all players. When revealed, in the next meeting " +
                "they can pick up to 3 players to be ejected. All selected players will be killed at the end of the meeting, along with the chosen 4th player everyone else votes on (if " +
                "any). If any of the killed players happens to be Crew, the Dictator goes out the dies with them. After that meeting, the Dictator has no post ejection ability.",
                RoleAlignment.CrewSov, Faction.Crew, "Out you go!"),
            new("Monarch", "Mon", "The Monarch can appoint players as knights. When the next meeting is called, all knighted players will be announced. Knighted players will " +
                "have their votes count as extra.", RoleAlignment.CrewSov, Faction.Crew, "Doth thou solemnly swear your allegiance to the lord?"),
            new("Stalker", "Stalk", "The Stalker is a buffed Tracker with no update interval. With the Chaos Drive, the arrows are no longer affected by camouflages and all " +
                "players instantly have an arrow pointing at them upon receiving the Chaos Drive.", RoleAlignment.SyndicateSupport, Faction.Syndicate, "I'll follow you"),
            new("Spellslinger", "Spell", "The Spellslinger is a powerful role who can cast curses on players. When all non-Syndicate players are cursed, the game ends in a " +
                "Syndicate victory. With each curse cast, the spell cooldown increases. This effect is negated by the Chaos Drive.", RoleAlignment.SyndicatePower, Faction.Syndicate,
                "I CURSE YOU TO SUCK ONE THOUSAND D-"),
            new("Collider", "Col", "The Collider can mark players as positive and negative. If these charged players come within a certain distance of each other, they will " +
                "die together. With the Chaos Drive, the range of collision increases.", RoleAlignment.SyndicateKill, Faction.Syndicate, "I'm a great matchmaker, trust me."),
            new("Time Keeper", "TK", "The Time Keeper can control time. Without the Chaos Drive, the Time Keeper can freeze time, making everyone unable to move. With the Chaos Drive, the"
                + "Time Keeper rewinds players instead.", RoleAlignment.SyndicatePower, Faction.Syndicate, "IT'S TIME TO STOP. NO MORE."),
            new("Silencer", "Sil", "The Silencer can silencer people. Silenced plaeyrs cannot see the messages being sent by others but can still talk. Other players can still talk but " +
                "can't get their info through to the silenced player. With the Chaos Drive, silence prevents everyone except for the silenced player from talking.",
                RoleAlignment.SyndicateDisrup, Faction.Syndicate, "QUIET."),
            new("Drunkard", "Drunk", "The Drunkard can reverse a player's controls. With the Chaos Drive, this effect applies to everyone.", RoleAlignment.SyndicateDisrup,
                Faction.Syndicate, "*Burp*")
        };

        public readonly static List<ModifierInfo> AllModifiers = new()
        {
            new("Invalid", "Invalid", "Invalid", "Invalid"),
            new("Bait", "Bait", "The Bait's killer will be forced to self-report.", "Everyone except Troll, Vigilate, Altruist, Thief and Shifter"),
            new("Coward", "Coward", "The Coward cannot report bodies.", "Everyone"),
            new("Diseased", "Diseased", "The Diseased's killer's kill cooldown will be increased for the next attack.", "Everyone except Troll and Altruist"),
            new("Drunk", "Drunk", "The Drunk's controls are inverted.", "Everyone"),
            new("Dwarf", "Dwarf", "The Dwarf's body is smaller and they are faster.", "Everyone"),
            new("Giant", "Giant", "The Giant's body is bigger and they are slower.", "Everyone"),
            new("Flincher", "FLinch", "The Flincher will randomly twitch backwards. Fun Fact: The Flincher is actually a bug which I turned into a modifier.", "Everyone"),
            new("Shy", "Shy", "The Shy player cannot call meetings.", "Everyone except Button Barries and roles who cannot call meetings"),
            new("Indomitable", "Ind", "The Indomitable cannot be guessed.", "Everyone except Actor"),
            new("VIP", "VIP", "When the VIP dies, everyone is alerted to their death and their screen will flash in the color of the VIP's role.", "Everyone"),
            new("Professional", "Prof", "The Professional has an extra life when guessing.", "Assassins"),
            new("Volatile", "Vol", "The Volatile will always see/hear random things.", "Everyone")
        };

        public readonly static List<ObjectifierInfo> AllObjectifiers = new()
        {
            new("Invalid", "Invalid", "Invalid", "Invalid", "Invalid", "φ"),
            new("Taskmaster", "TMer", "The Taskmaster is basically a living Phantom. When a certain number of tasks are remaining, the Taskmaster is revealed" +
                " to Intruders and the Syndicate and the Crew only sees a flash to indicate the Taskmaster's existence.", "Finish tasks without dying or game ending", "Neutrals", "µ"),
            new("Lovers", "Lover", "The Lovers are two players who are linked together. They gain the primary objective to stay alive together. If they are both among " +
                "the last 3 players, they win as a Lover pair. In order to so, they gain access to a private chat, only visible by them in between meetings. However, they can also win" +
                " with their respective team, hence why the Lovers do not know the role of the other Lover", "Live to the final 3 with both Lvoers still alive", "Everyone", "♥"),
            new("Rivals", "Rival", "The Rivals cannot do anything to each other and must get the other one killed.", "Get the other rival killed without directly " +
                "interfering, then live to the final 2.", "Everyone", "α"),
            new("Allied", "Ally", "An Allied Neutral Killer now sides with either the Crew, the Intruders or the Syndicate. In the case of the latter two, all " +
                "faction members are shown the Allied player's role, and can no longer kill them.", "Win with whichever faction they are aligned with", "Neutral Killers", "ζ"),
            new("Fanatic", "CF (means Crew Fanatic)", "When attacked, the Fanatic joins whichever faction their attacker belongs to. From then on, their alliance sits with said faction.",
                "Get attacked by either the Intruders or the Syndicate to join their team", "Crew", "♠"),
            new("Overlord", "Ov", "Every meeting, for as long as an Overlord is alive, players will be alerted to their existence. The game ends if the Overlord lives " +
                "long enough.", "Survive a set amount of meetings", "Neutrals", "β"),
            new("Corrupted", "Corr", "The Corrupted is a Crewmate with the alignment of a Neutral Killer. On top of their base role's attributes, they also gain a " +
                "kill button. Their win condition is so strict that not even Neutral Benigns or Evils can be spared", "Kill everyone", "Crew", "δ"),
            new("Traitor", "CT (means Crew Traitor)", "The Traitor is a Crewmate who must finish their tasks to switch sides. Upon doing so, they will either join " +
                "the Intruders or the Syndicate, and will win with that faction. If the Traitor is the only person in their new faction, they become a Betrayer, losing their original" +
                " role's abilities and gaining the ability to kill in the process.", "Finish tasks to join either the Intruders or Syndicate", "Crew", "♣"),
            new("Mafia", "Maf", "The Mafia are a group of players with a linked win condition. They must kill anyone who is not a member of the Mafia. All Mafia win" +
                " together.", "Kill anyone who is not a member of the Mafia", "Everyone", "ω"),
            new("Defector", "Defect", "The Defector changes sides when they are the last player of their faction remaining.", "Kill off anyone who opposes their new faction",
                "Intruders And Syndicate", "ε")
        };

        public readonly static List<AbilityInfo> AllAbilities = new()
        {
            new("Invalid", "Invalid", "Invalid", "Invalid"),
            new("Assassin", "Assassin", "The Assassin is given to a certain number of Intruders, Syndicate and/or Neutral Killers. This ability gives the Intruder, Syndicate" +
                " or Neutral a chance to kill during meetings by guessing the roles or modifiers of others. If they guess wrong, they die instead.", "Intruders, Crew, Syndicate, " +
                "Neutral (Killing) and Neutral (Neophyte)"),
            new("Button Barry", "BB", "Button Barry has the ability to call a meeting from anywhere on the map, even during sabotages. Calling a meeting during a non-" +
                "critical sabotage will fix the sabotage.", "Everyone except roles who cannot call meetings"),
            new("Insider", "Ins", "The Insider will be able to view everyone's votes in meetings upon finishing their tasks. Only spawns if Anonymous Votes is turn on.", "Crew"),
            new("Multitasker", "MT", "When doing tasks, the Multitasker's task window is transparent.", "Roles with tasks"),
            new("Ninja", "Nin", "Ninjas don't lunge when killing.", "Killing roles"),
            new("Radar", "Radar", "The Radar always has an arrow pointing towards the nearest player.", "Everyone"),
            new("Ruthless", "Ruth", "A Ruthless killer can bypass all forms of protection. Although they bypass alert protection, they will still die to a player on alert.",
                "Killing roles"),
            new("Snitch", "Snitch", "The Snitch is an ability which allows any Crewmate to get arrows pointing towards the Intruders once all their tasks are finished. " +
                "The names of the Intruders will also show up as red on their screen. However, when they only have a single task left, the Intruders get an arrow pointing towards " +
                "the Snitch.", "non-Traitor or Fanatic Crew"),
            new("Tiebreaker", "TB", "If any vote is a draw, the Tiebreaker's vote will go through. If they voted another player, they will get voted out. If the Tiebreaker " +
                "is the Mayor, it applies to the Mayor's first vote.", "Everyone"),
            new("Torch", "Torch", "The Torch's has Intruder vision at all times.", "Crew, Neutral Evil and Benign roles, Neutrals and Neutral Killers when their respective " +
                "lights are off"),
            new("Tunneler", "Tun", "The Tunneler will be able to vent when they finish their tasks.", "Crew except Engineer"),
            new("Underdog", "UD", "The Underdog is an Intruder with a prolonged kill cooldown when with a teammate. When they are the only remaining Intruder, they will " +
                "have their kill cooldown shortened.", "Intruders and Syndicate"),
            new("Politician", "Pol", "TThe Politician can vote multiple times. If the Politician cannot kill, they gain a new button called the abstain button which stores " +
                "their vote for later use. On the other hand, if the Politician can kill, they lose the Abstain button ans instead gain a vote for each player they kill", "Crew, " +
                "Intruders, Syndicate and Neutral Killers"),
            new("Swapper", "Swap", "The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted " +
                "towards the second player and vice versa.", "Crew")
        };

        public readonly static List<FactionInfo> AllFactions = new()
        {
            new(Faction.None),
            new(Faction.Crew),
            new(Faction.Intruder),
            new(Faction.Syndicate)
        };

        public readonly static List<AlignmentInfo> AllAlignments = new()
        {
            new(RoleAlignment.None),
            new(RoleAlignment.CrewSupport),
            new(RoleAlignment.CrewInvest),
            new(RoleAlignment.CrewProt),
            new(RoleAlignment.CrewKill),
            new(RoleAlignment.CrewUtil),
            new(RoleAlignment.CrewSov),
            new(RoleAlignment.CrewAudit),
            new(RoleAlignment.CrewConceal),
            new(RoleAlignment.CrewDecep),
            new(RoleAlignment.CrewPower),
            new(RoleAlignment.CrewDisrup),
            new(RoleAlignment.IntruderSupport),
            new(RoleAlignment.IntruderConceal),
            new(RoleAlignment.IntruderDecep),
            new(RoleAlignment.IntruderKill),
            new(RoleAlignment.IntruderUtil),
            new(RoleAlignment.IntruderInvest),
            new(RoleAlignment.IntruderProt),
            new(RoleAlignment.IntruderSov),
            new(RoleAlignment.IntruderAudit),
            new(RoleAlignment.IntruderPower),
            new(RoleAlignment.IntruderDisrup),
            new(RoleAlignment.NeutralKill),
            new(RoleAlignment.NeutralNeo),
            new(RoleAlignment.NeutralEvil),
            new(RoleAlignment.NeutralBen),
            new(RoleAlignment.NeutralPros),
            new(RoleAlignment.NeutralApoc),
            new(RoleAlignment.NeutralHarb),
            new(RoleAlignment.NeutralInvest),
            new(RoleAlignment.NeutralAudit),
            new(RoleAlignment.NeutralSov),
            new(RoleAlignment.NeutralProt),
            new(RoleAlignment.NeutralSupport),
            new(RoleAlignment.NeutralUtil),
            new(RoleAlignment.NeutralConceal),
            new(RoleAlignment.NeutralDecep),
            new(RoleAlignment.NeutralDisrup),
            new(RoleAlignment.NeutralPower),
            new(RoleAlignment.SyndicateKill),
            new(RoleAlignment.SyndicateSupport),
            new(RoleAlignment.SyndicateDisrup),
            new(RoleAlignment.SyndicatePower),
            new(RoleAlignment.SyndicateUtil),
            new(RoleAlignment.SyndicateInvest),
            new(RoleAlignment.SyndicateProt),
            new(RoleAlignment.SyndicateSov),
            new(RoleAlignment.SyndicateAudit),
            new(RoleAlignment.SyndicateConceal),
            new(RoleAlignment.SyndicateDecep)
        };

        public readonly static List<Lore> AllLore = new()
        {
            new("All", "The story takes place several hundred years in the future with is a gigantic leap in scientific advancement. As a result, space travel is a common thing and" +
                " is commonly used for mining resources from nearby celestial bodies. A multi-billion dollar government space exploration corporation called \"Mira\" just recently " +
                "discovered a new habitable planet several light years away. They named it \"Polus\". Soon enough, they build a ship that can carry over 200 passengers with plenty of " +
                "storage for items, later deemed as the \"Skeld\". Mira begins an exploration trip to Polus, recruiting special individuals from within their agency to set up camp and" +
                " continue the research of the planet. Little do they know, people whose motives conflict with that of Mira have managed to get onboard the Skeld. Not only that, some of " +
                "the Crew have been mysteriously replaced by shapeshifting parasites hellbent on conquering Earth but unfortunately got swept up in this project. Skeld is soon " +
                "launched from the Mira HQ. After launch, the Skeld becomes a battlefield of death and mind games while the Crew tries to get rid of these evils when stranded in space. " +
                "These are the stories of said passengers aboard the Skeld.", "All"),
            new("Swapper", "There was once a Crewmate who made the voting software in light of the recent events occurring aboard the Skeld. The software would tally up the votes " +
                "against someone, use a mechanical arm to detain the voted person and transport said voted person to the airlock for ejection. He later witnessed the Mafioso kill the " +
                "Medic and the Janitor clean up the mess in front of him and calls a meeting. The Intruders, using their silver tongues, slip out of the blame and instead push it on " +
                "the Crewmate. The Crewmate knew he had only one thing to do. He told everyone to vote for him and then vote for the others. In the moment of chaos and confusion, the " +
                "rest of the Crew voted for him. The Mafioso and the Janitor, with villainous smirks on their faces, watched the votes get tallied. Soon the faces of confidence turn " +
                "into faces of pure shock, as the votes are tallied not against the Crewmate, but against the Mafioso. A mechanical arm juts out from the walls and grabs the Mafioso " +
                "and starts moving towards the airlock. The Crew, held aback by the sudden change in air, looked at the Crewmate. The Mafioso looked at the Crewmate with a face of " +
                "horror through the window in the airlock door. The Crewmate simply meets his gaze with a face of pure joy and confidence, and mouths, \"I'm the Swapper, bitch.\"", "Swap"),
            new("Amnesiac", "A lonely Mystic walked down the hallway. His head was aching and the bandages around his head were starting to loosen. The Mystic just wanted to " +
                "finish his checklist of tasks assigned to him by the Mayor. But he forgot how to do them. He sat at the panel, confused. The Mystic fumbled with the wires a bit. He " +
                "felt a sudden tingle but he paid it no mind. Soon a loud alarm rang. A meeting was in order. Blue had just died, and Green said that the body was in Navigation. " +
                "Everyone started accusing each other, but the Mystic sat at the table with fuzzy memories. He tried to remember what roles he, Blue or Green were, but to no avail. The" +
                " meeting ended with a voting tie between Red and Green. The Mystic with no memories looked at Red, he noticed a sinister glare on his face. The Amnesiac suddenly felt a " +
                "repressed memory try to make its way back to the surface, but he still couldn't completely remember it. The Amnesiac moved to the kitchen to make himself a sandwich. He" +
                " just looked at the knife, enamoured by it. He picked it and sliced the tomato in half. A certain joy filled the Amnesiac's heart. Just behind him, Red, the Grenadier, " +
                "was being murdered by Green, the Glitch. Green ran away, unnoticed by the Amnesiac. The Amnesiac's sense rang loudly as he turned around to see the wheezing and dying " +
                "Red. He soon realised what he wanted in life, what his true goal was. He had finally remembered what his purpose was. A creepy smile began to set onto the Amnesiac's " +
                "face as he raised his kitchen knife into the air. A meeting was called as Red's body was discovered to have multiple stab wounds, his backpack emptied. The Glitch was " +
                "confused, he only remembered stabbing the Grenadier once. The Amnesiac sat there in silence, playing around with the toys he had found. He enjoyed the clicking sounds " +
                "the new toys in his hand made and was itching to use them. Green was being ejected thanks to the close tie he had with Red in the last meeting. Soon the meeting came to " +
                "a close. Just as everyone was dispersing, the spirit of the Grenadier looked at the Amnesiac, with a cold smile on his face. He had raised his successor well.", "Amne"),
            new("Glitch", "The Operative woke up to see a dreary yellow maze-like world around him. The maze seemed infinitely large, as there was no limit to be seen. He tried his " +
                "best to survive in this maze, preserving what was left of his sanity. Several years passed by and by some miracle, he found a vent. Not even thinking that it might be " +
                "leading to nowhere, he hopped into it. He was nearing death so he was not at liberty to decide. As he fell further down, he questioned his choice, thinking it might " +
                "just be his end. But then there he was, lying on the floor of the laboratory, in front of the vitals screen. His body seemed fine, heck, even better. The Operative " +
                "was shocked, it was as if he was not gone for a second in the main world. Everything was as it was when he \"glitched\" through. What greeted him wasn't any of the " +
                "Crew, it was a parasite. The parasite entered the Operative's body. The parasite and the Operative struggled to gain control of the body, and the Operative succeeded. " +
                "But he felt...new. Powerful, even. In his mind, all he could think of was the pain and suffering he went through. He couldn't forgive the Crew who didn't even bother " +
                "to look for him, let alone save him. He needed to make the world aware of what he went through. And he had just the right tools with him. Controlling the dead parasite " +
                "within his body and harnessing its power, along with his warped anatomy thanks to reality breaking, the Operative fused himself with a lot of the technology around him, " +
                "in an attempt to get stronger. He invented devices to hack the Crew's and Intruders' systems alike, and an illusion device to change his appearance. It was showtime. " +
                "The best thing the Operative could do right now was kill everyone and hope they get transported to the maze, to feel what he did. He was the one who transcended reality, "
                + "he was the one with the knowledge to break through the universe's strongest barrier. He was...the Glitch.", "Gli"),
            new("Juggernaut", "A paranoid Veteran watched his loved one die to the hands of the Crew. He couldn't bear to think he wasn't there to save her. \"If only I was stronger" +
                ".\" was the thought that plagued his mind. Day in and day out, he pursued strength, in his ultimate goal to destroy Mira, the very company that killed his wife in cold " +
                "blood. But, he just couldn't shake off the paranoia from the war. No amount of self healing or meditating could take away those horrid memories from wartime. His wife " +
                "was his only way to support himself, his lifeline. Everytime he thought of her, he would be engulfed in deadly rage, unable to calm down until his fists bled from " +
                "punching the walls. One day, he saw a job listing to explore a newly discovered planet, Polus. The advertiser? Mira. \"Perfect.\" thought the Veteran as he lifted himself"
                + " up from his couch, and readied his uniform to go to the application site. He got to the site, to only see that Mira wasn't even performing background checks on the " +
                "applicants. \"That lax behaviour will get you killed, Mira.\" After a few days, he received an acceptance letter. He was accepted! He boarded the ship to see familiar " +
                "faces, as well suspicious ones. The Mayor, the one who led the team that killed his wife and the Godfather, who he suspected was the cause. Aboard the ship he met new " +
                "faces, the weak Crewmate, the just Sheriff and the lovely Medic. She reminded him of his wife. But he could not spare his feelings for her. She was affiliated with Mira, "
                + "making her his enemy. A couple days later, he set off on the journey to Polus. He thought this was his time to shine, but he couldn't bring himself to kill anyone. " +
                "Most of the people here were innocent, forced to go on the mission with him. He couldn't hurt these poor souls. The Godfather paid mind to the Veteran's antics, and " +
                "struck a deal with him. They met up in the infirmary to talk business. As they were discussing, the Godfather let loose a parasite, to take over and control the " +
                "Veteran. Just as he began, the Veteran felt a tingle, but thought nothing of it. The infirmary doors burst open with the Sheriff and the Seer entering with full " +
                "force. \"AHA! I KNEW YOU WERE UP TO NO GOOD!\" said the robust Sheriff, preparing his gun to fire. But that declaration triggered something within the Veteran. He " +
                "heard an audible snap and then he felt bliss. A couple minutes later, a meeting was called by the Medic. \"3 bodies were found in the infirmary...brutally mangled " +
                "beyond recognition.\" she declared, holding back the urge to puke. As the rest of the Crew gasped in horror, only the Juggernaut smiled.", "Jugg"),
            new("Medic", "The Medic came from a highly evaluated university, with the highest grades possible. She was the best in the field, until an accident took her ability to " +
                "heal others. Ashamed of her incident and only being able to perform rudimentary first aid, the Medic sought for a job on the Polus mission. She was accepted! This was " +
                "her turning point, one where she would be known for something else, and hopefully heal herself in the process. The Medic's dreams fell short as people started dying " +
                "to mysterious killers among the Crew. She tried her best to find people to protect, but her arrivals were too little too late. The only things waiting for her were the " +
                "bodies of those she swore to protect and a lingering sense of dread. She couldn't get by just looking for people to heal and instead decided to concentrate on one " +
                "person, so at least they'd be safe. It was the Crewmate, a loveable simpleton who only stood for justice. She would know when he would be attacked. All she had to do " +
                "was just sit and lie in wait, patiently waiting for a killer to slip up and attack the Crewmate and alerting her. But on her way to the cafeteria, she heard wheezing. " +
                "It was the corrupt Mayor, the one who was the sole reason behind the Medic's accident. Her entire being said let him die, but only her heart said to save him, for she " +
                "was not a monster, but the barrier between life and death for the Crew. She slowly approached the Mayor, pushing down her hatred for him. \"Where does it hurt?\"",
                "Medic"),
            new("Crewmate", "Nothing fruitful ever happened in the Crewmate's life. He was just lucky enough to get a spot in the exploration trip to Polus. Only useful for " +
                "finishing tasks and basic repairs, he decided to make the most of his time aboard the Skeld. Getting acquainted with all those famous celebrities from Mira, the " +
                "Crewmate felt a sense of bliss and happiness. He was going to make history. He was finally going to be able to stand with the celebrities like the Mayor. That would'" +
                "ve happened, if it were not for the Intruder aboard their ship. First it was the disruption of their tasks, then it was the sabotages and then finally the Intruders " +
                "took a step further. Killing. The Crewmate feared the loss of his life and went into hiding. He knew one thing, he had to ensure the killers got thrown out of the ship" +
                " in order for him to survive.", "Crew"),
            new("Operative", "There was once a Crewmate who always had a knack for invention. He made special devices he deemed as “bugs”. He could spy on others with these and " +
                "later realised, he could make these bugs detect more than just players. He figured that upgrading the Admin table would be worth it. He slowly put trackers on his " +
                "fellow crew and assigned a colour to each. He then connected the trackers to the admin table to see where everyone was. He then linked these trackers to his bugs, " +
                "which fed him info. And it worked perfectly. He couldn't wait to show it to the rest of the crew. But that was when the assassinations started to happen. Crewmates " +
                "dropped dead during discussions, so the Crewmate resolved to not tell the crew about it, fearing the worst. Slowly but surely, the Crewmate began spying on others, " +
                "his wall of distrust getting higher and higher. And that when he watched as Red and Blue went into the Medbay together but only Red came back out. The Crewmate sat " +
                "in Admin, waiting for Blue to leave, but he didn't and that was when the Crewmate knew something was up. He dashed straight to Medbay and saw Blue's body, slashed in " +
                "half. The Crewmate reported the body and called out Red. Red was flabbergasted, not knowing how the Crewmate figured out his identity. In a last ditch attempt, Red, " +
                "the Undertaker, guessed the Crewmate as the Engineer. That didn’t work as the Undertaker started to lose his breath. The Crewmate looked across the table and met " +
                "gazes with the dying Undertaker. He was the Operative all along.", "Op")
        };
    }
}