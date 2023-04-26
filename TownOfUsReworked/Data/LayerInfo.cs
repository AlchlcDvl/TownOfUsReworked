using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace TownOfUsReworked.Data
{
    [HarmonyPatch]
    public static class LayerInfo
    {
        public class RoleInfo
        {
            public string Name { get; set; }
            public string Short { get; set; }
            public string Alignment { get; set; }
            public string Description { get; set; }
            public string AlignmentDescription { get; set; }
            public string AlignmentShort { get; set; }
            public string FactionDescription { get; set; }
            public string FactionShort { get; set; }
            public string WinCon { get; set; }
            public string Quote { get; set; }
            public string FactionS { get; set; }

            private readonly string SyndicateDescription = "Syndicate is an \"evil\" faction that is an informed minority of the game. They have special abilities specifically geared towards" +
                " slowing down the progress of other or causing chaos. Syndicate members, unless they are Syndicate (Killing), Anarchist or Sidekick, cannot kill by default. Instead" +
                " they gain the ability to kill by obtaining a powerup called the Chaos Drive. The Chaos Drive not only boosts each member's abilities but also gives them the ability" +
                " to kill if they didn't already.";
            private readonly string CrewDescription = "The Crew is the uninformed majority of the game. They are the \"good guys\". It is their job to deduce who is evil and who is not " +
                "and vote them out.";
            private readonly string IntruderDescription = "Intruders are the main \"bad guys\" of the game and an informed minority of the game. All roles have the capability to kill and" +
                " sabotage, making them a pain to deal with.";
            private readonly string NeutralDescription = "Neutrals are essentially factionless. They are the uninformed minority of the game and can only win by themselves.";

            private readonly string IntruderObjective = "Have a critical sabotage set off by the Intruders reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew, Neutral" +
                " Killers, Proselytes and Neophytes.";
            private readonly string ISDescription = "Intruder (Support) roles have miscellaneous abilities. These roles can delay players' chances of winning by" +
                " either gaining enough info to stop them or forcing players to do things they can't.";
            private readonly string ICDescription = "Intruder (Concealing) roles specialise in hiding information from others. If there is no new " +
                "information, it's probably their work.";
            private readonly string IDDescription = "Intruder (Deception) roles are built to spread misinformation. Never trust your eyes, for the killer you " +
                "saw in front of you might not be the one who they seem to be.";
            private readonly string IUDescription = "Intruder (Utility) roles usually don't appear under regaular spawn conditions.";
            private readonly string IKDescription = "Intruder (Killing) role! You have a ruthless ability to kill people with no mercy. Kill off the crew as fast as possible " +
                "with your abilities!";

            private readonly string SyndicateObjective = "Have a critical sabotage set off by the Syndicate reach 0 seconds or kill off all Intruders, Unfaithful Syndicate, " +
                "Crew and Neutral Killers, Proselytes and Neophytes.";
            private readonly string SUDescription = "Syndicate (Utility) roles usually don't appear under regaular spawn conditions.";
            private readonly string SSuDescription = "Syndicate (Support) roles have miscellaneous abilities. They are detrimental to the Syndicate's cause and if" +
                " used right, can greatly affect how the game continues.";
            private readonly string SDDescription = "Syndicate (Disruption) roles are designed to change the flow of the game, via changing some mechanic.";
            private readonly string SyKDescription = "Syndicate (Killing) roles are powerful killers unique ways to rack up body counts.";
            private readonly string SPDescription = "Syndicate (Power) roles are powerful disruptors with a knack for chaos and destruction.";

            private readonly string CrewObjective = "Finish tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, Neutral Killers, Proselytes and Neophytes.";
            private readonly string CPDescription = "Crew (Protective) roles have the capability to stop someone from losing their life or bring back the dead.";
            private readonly string CIDescription = "Crew (Investigative) roles have the ability to gain information via special methods. Using the acquired info, " +
                "Crew (Investigative) roles can deduce who is good and who is not.";
            private readonly string CUDescription = "Crew (Utility) roles usually don't appear under regaular spawn conditions.";
            private readonly string CSDescription = "Crew (Support) roles are roles with miscellaneous abilities. Try not to get lost because if you are not paying " +
                "attention, your chances of winning will be severely decreased because of them.";
            private readonly string CADescription = "Crew (Auditor) roles are special roles that spawn under certain conditions. They exist for the demise of rival subfactions.";
            private readonly string CKDescription = "Crew (Killing) roles have no aversion to killing like the rest of the Crew and if left alone and potentially wreck the chances of " +
                "evil subfactions winning.";
            private readonly string CSvDescription = "Crew (Sovereign) roles are democratic roles with powers over votes. They are the most powerful during a meeting, so avoid too many " +
                "meetings while they are active.";

            private readonly string NBDescription = "Neutral (Benign) roles are special roles that have the capability to win with anyone, as long as a certain " +
                "condition is fulfilled by the end of the game.";
            private readonly string NKDescription = "Neutral (Killing) roles are roles that have the ability to kill and do not side with anyone. Each role has a special way" +
                " to kill and gain large body counts in one go. Steer clear of them if you don't want to die.";
            private readonly string NEDescription = "Neutral (Evil) roles are roles whose objectives clash with those of other roles. As such, you need to ensure they don't have a chance" +
                " at winning or when they do win, you have their cooperation.";
            private readonly string NPDescription = "Neutral (Proselyte) roles are special roles that have their specific ways to win. Each role here is unique in its own way and more often than not" +
                " they are against you.";
            private readonly string NNDescription = "Neutral (Neophyte) roles are roles that can convert someone to side with them. Be careful of them, as they can easily overrun you with their" +
                " numbers.";

            public RoleInfo(string name, string shortF, string description, RoleAlignment alignmentEnum, Faction faction, string quote, string wincon = "")
            {
                Name = name;
                Short = shortF;
                Description = description;
                Quote = quote;

                switch (faction)
                {
                    case Faction.Syndicate:
                        FactionDescription = SyndicateDescription;
                        WinCon = SyndicateObjective;
                        FactionShort = "Syn";
                        FactionS = "Syndicate";
                        break;
                    case Faction.Intruder:
                        FactionDescription = IntruderDescription;
                        WinCon = IntruderObjective;
                        FactionShort = "Int";
                        FactionS = "Intruder";
                        break;
                    case Faction.Crew:
                        FactionDescription = CrewDescription;
                        WinCon = CrewObjective;
                        FactionShort = "Crew";
                        FactionS = "Crew";
                        break;
                    case Faction.Neutral:
                        FactionDescription = NeutralDescription;
                        WinCon = wincon;
                        FactionShort = "Neut";
                        FactionS = "Syn";
                        break;
                    case Faction.None:
                        FactionDescription = "Invalid";
                        WinCon = "Invalid";
                        FactionShort = "Invalid";
                        FactionS = "Invalid";
                        break;
                }

                switch (alignmentEnum)
                {
                    case RoleAlignment.CrewSupport:
                        AlignmentDescription = CSDescription;
                        Alignment = "Crew (Support)";
                        AlignmentShort = "CS";
                        break;
                    case RoleAlignment.CrewInvest:
                        AlignmentDescription = CIDescription;
                        Alignment = "Crew (Investigative)";
                        AlignmentShort = "CI";
                        break;
                    case RoleAlignment.CrewKill:
                        AlignmentDescription = CKDescription;
                        Alignment = "Crew (Killing)";
                        AlignmentShort = "CK";
                        break;
                    case RoleAlignment.CrewProt:
                        AlignmentDescription = CPDescription;
                        Alignment = "Crew (Protective)";
                        AlignmentShort = "CP";
                        break;
                    case RoleAlignment.CrewSov:
                        AlignmentDescription = CSvDescription;
                        Alignment = "Crew (Sovereign)";
                        AlignmentShort = "CSv";
                        break;
                    case RoleAlignment.CrewAudit:
                        AlignmentDescription = CADescription;
                        Alignment = "Crew (Auditor)";
                        AlignmentShort = "CA";
                        break;
                    case RoleAlignment.CrewUtil:
                        AlignmentDescription = CUDescription;
                        Alignment = "Crew (Utility)";
                        AlignmentShort = "CU";
                        break;
                    case RoleAlignment.IntruderSupport:
                        AlignmentDescription = ISDescription;
                        Alignment = "Intruder (Support)";
                        AlignmentShort = "CS";
                        break;
                    case RoleAlignment.IntruderConceal:
                        AlignmentDescription = ICDescription;
                        Alignment = "Intruder (Concealing)";
                        AlignmentShort = "IS";
                        break;
                    case RoleAlignment.IntruderDecep:
                        AlignmentDescription = IDDescription;
                        Alignment = "Intuder (Deception)";
                        AlignmentShort = "IS";
                        break;
                    case RoleAlignment.IntruderKill:
                        AlignmentDescription = IKDescription;
                        Alignment = "Intruder (Killing)";
                        AlignmentShort = "IK";
                        break;
                    case RoleAlignment.IntruderUtil:
                        AlignmentDescription = IUDescription;
                        Alignment = "Intruder (Utility)";
                        AlignmentShort = "IU";
                        break;
                    case RoleAlignment.NeutralKill:
                        AlignmentDescription = NKDescription;
                        Alignment = "Neutral (Killing)";
                        AlignmentShort = "NK";
                        break;
                    case RoleAlignment.NeutralNeo:
                        AlignmentDescription = NNDescription;
                        Alignment = "Neutral (Neophyte)";
                        AlignmentShort = "NN";
                        break;
                    case RoleAlignment.NeutralEvil:
                        AlignmentDescription = NEDescription;
                        Alignment = "Neutral (Evil)";
                        AlignmentShort = "NE";
                        break;
                    case RoleAlignment.NeutralBen:
                        AlignmentDescription = NBDescription;
                        Alignment = "Crew (Benign)";
                        AlignmentShort = "NB";
                        break;
                    case RoleAlignment.NeutralPros:
                        AlignmentDescription = NPDescription;
                        Alignment = "Neutral (Proselyte)";
                        AlignmentShort = "NP";
                        break;
                    case RoleAlignment.SyndicateKill:
                        AlignmentDescription = SyKDescription;
                        Alignment = "Syndicate (Killing)";
                        AlignmentShort = "SyK";
                        break;
                    case RoleAlignment.SyndicateSupport:
                        AlignmentDescription = SSuDescription;
                        Alignment = "Syndicate (Support)";
                        AlignmentShort = "SSu";
                        break;
                    case RoleAlignment.SyndicateDisruption:
                        AlignmentDescription = SDDescription;
                        Alignment = "Syndicate (Disruption)";
                        AlignmentShort = "SD";
                        break;
                    case RoleAlignment.SyndicatePower:
                        AlignmentDescription = SPDescription;
                        Alignment = "Syndicate (power)";
                        AlignmentShort = "SP";
                        break;
                    case RoleAlignment.SyndicateUtil:
                        AlignmentDescription = SUDescription;
                        Alignment = "Syndicate (Utility)";
                        AlignmentShort = "SU";
                        break;
                    case RoleAlignment.None:
                        AlignmentDescription = "Invalid";
                        Alignment = "Invalid";
                        AlignmentShort = "Invalid";
                        break;
                }
            }

            public string RoleInfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Alignment: ").AppendLine(Alignment)
                    .Append("Win Condition: ").AppendLine(WinCon)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }

            public string AlignmentInfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Alignment)
                    .Append("Short Form: ").AppendLine(AlignmentShort)
                    .Append("Description: ").AppendLine(AlignmentDescription);
                return builder.ToString();
            }

            public string FactionInfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(FactionS)
                    .Append("Short Form: ").AppendLine(FactionShort)
                    .Append("Description: ").AppendLine(FactionDescription);
                return builder.ToString();
            }

            public string FullInfo() => $"{RoleInfoMessage}\n{AlignmentInfoMessage}\n{FactionInfoMessage}";
        }

        public class ModifierInfo
        {
            public string Name { get; set; }
            public string Short { get; set; }
            public string Description { get; set; }
            public string AppliesTo { get; set; }

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
                builder.Append("Modifier Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Applies To: ").AppendLine(AppliesTo)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class ObjectifierInfo
        {
            public string Name { get; set; }
            public string Short { get; set; }
            public string AppliesTo { get; set; }
            public string Description { get; set; }
            public string WinCon { get; set; }
            public string Symbol { get; set; }

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
                builder.Append("Objectifier Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Symbol: ").AppendLine(Symbol)
                    .Append("Applies To: ").AppendLine(AppliesTo)
                    .Append("Win Con: ").AppendLine(WinCon)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class AbilityInfo
        {
            public string Name { get; set; }
            public string Short { get; set; }
            public string AppliesTo { get; set; }
            public string Description { get; set; }

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
                builder.Append("Ability Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Applies To: ").AppendLine(AppliesTo)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class Lore
        {
            public string Name { get; set; }
            public string Short { get; set; }
            public string Story { get; set; }

            public Lore(string name, string story, string shortF)
            {
                Name = name;
                Story = story;
                Short = shortF;
            }

            public string InfoMessage()
            {
                var builder = new StringBuilder();
                builder.Append("Name: ").AppendLine(Name);
                builder.Append("Lore: ").AppendLine(Story);
                return builder.ToString();
            }
        }

        public readonly static List<RoleInfo> AllRoles = new()
        {
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Altruist", "Alt", "The Altruist is capable of reviving dead players. After a set period of time, the player will be resurrected, if the revival isn't " +
                "interrupted. Once a player is revived, all evil players will be notified of the revival and will have an arrow pointing towards the revived player. Once the Altruist " +
                "uses up all of their ability charges, they sacrifice themselves on the last use of their ability.", RoleAlignment.CrewProt, Faction.Crew, "I know what I have to do but " +
                "I don't know if I have the strength to do it."),
            new RoleInfo("Chameleon", "Cham", "The Chameleon can go invisible to stalk players and see what they do when no one is around.", RoleAlignment.CrewSupport,
                Faction.Crew, "Are you sure you can see me?"),
            new RoleInfo("Coroner", "Cor", "The Coroner gets an alert when someone dies. On top of this, the Coroner briefly gets an arrow pointing in the direction of " +
                "the body. They can autopsy bodies to get some information. They can then compare that information with players to see if they killed the body or not. The Coroner also " +
                "gets a body report from the player they reported. The report will include the cause and time of death, player's faction/role, the killer's faction/role and (according " +
                "to the settings) the killer's name.", RoleAlignment.CrewInvest, Faction.Crew, "A body? Where? I need it for...scientific purposes."),
            new RoleInfo("Crewmate", "Crew", "Just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.",
                RoleAlignment.CrewUtil, Faction.Crew, "I once made a pencil using 2 erasers...they were pointless, just like me."),
            new RoleInfo("Engineer", "Engi", "The Engineer can fix sabotages from anywhere on the map. They can also use vents to get across the map easily.",
                RoleAlignment.CrewSupport, Faction.Crew, "How am I going to stop some big mean mother hubbard from tearing me a structurally superfluous new behind? The solution? A " +
                "wrench."),
            new RoleInfo("Escort", "Esc", "The Escort can roleblock players and prevent them from doing anything for a short while.", RoleAlignment.CrewSupport, Faction.Crew,
                "Today, I will make you a man."),
            new RoleInfo("Inspector", "Insp", "The Inspector can check players for their roles. Upon being checked, the targets' names will be updated to give a list of what" +
                " roles could the target possibly be.", RoleAlignment.CrewInvest, Faction.Crew, "THAT'S THE GODFATHER! YOU GOTTA BELIEVE ME."),
            new RoleInfo("Mayor", "Mayo (XD)", "The Mayor can vote multiple times. The Mayor has a Vote Bank, which is the number of times they can vote. They have the option to abstain" +
                " their vote during a meeting, adding that vote to the Vote Bank. As long as not everyone has voted, the Mayor can use as many votes from their Vote Bank as they please.",
                RoleAlignment.CrewSov, Faction.Crew, "Um, those votes are legitimate. No, the votes are not rigged."),
            new RoleInfo("Medic", "Medic", "The Medic can give any player a shield that will make them immortal until the Medic is dead. A shielded player cannot be " +
                "killed by anyone, unless it's a suicide. Shielded players have a green ✚ next to their names.", RoleAlignment.CrewProt, Faction.Crew, "Where does it hurt?"),
            new RoleInfo("Medium", "Med", "The Medium can mediate to be able to see ghosts. If the Medium uses this ability, the Medium and the dead player will be able to " +
                "see each other and communicate from beyond the grave!", RoleAlignment.CrewInvest, Faction.Crew, "The voices...they are telling me...my breath stinks? Can ghosts even " +
                "smell?"),
            new RoleInfo("Mystic", "Mys", "The Mystic only spawns when there is at least one Neutral (Neophyte) role present in the game. Whenever someone's subfaction is " +
                "changed, the Mystic will be alerted about it. The Mystic can also investigate players to see if their subfactions have been changed. If the target has a different " +
                "subfaction, the Mystic's screen will flash red, otherwise it will flash green. It will not, however, work on the Neutral (Neophyte) roles themselves so even people who" +
                " flashed green might be a converter. Once all subfactions are dead, the Mystic becomes a Seer.", RoleAlignment.CrewAudit, Faction.Crew, "There's a hint of corruption."),
            new RoleInfo("Operative", "Op", "The Operative can place bugs around the map. When players enter the range of the bug, they trigger it. In the following meeting, all " +
                "players who triggered a bug will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the bug, nor what role " +
                "a specific player is. The Operative also gains more information when on Admin Table and on Vitals. On Admin Table, the Operative can see the colors of every person on " +
                "the map. When on Vitals, the Operative is shown how long someone has been dead for.", RoleAlignment.CrewInvest, Faction.Crew, "The only thing you need to find out " +
                "information is good placement and amazing levels of patience."),
            new RoleInfo("Retributionist", "Ret", "The Retributionist can mimic dead crewamtes. During meetings, the Retributionist can select who they are going to " +
                "ressurect and use for the following round from the dead. They can choose to use each dead players as many times as they wish. It should be noted the Retributionist " +
                "can not use all Crew roles and cannot use any Non-Crew role. The cooldowns, limits and everything will be set by the settings for their respective roles.",
                RoleAlignment.CrewSupport, Faction.Crew, "Bodies...bodies...I NEED BODIES."),
            new RoleInfo("Seer", "Seer", "The Seer only spawns if there are roles capable of changing their initial roles. The Seer can investigate players to see if their" +
                " role is different from what they started out as. If a player's role has been changed, the Seer's screen will flash red, otherwise it will flash green. This, however, " +
                "does not work on those whose subfactions have changed so those who flashed green might still be evil. If all players capable of changing or have changed their initial " +
                "roles are dead, the Seer becomes a Sheriff.", RoleAlignment.CrewInvest, Faction.Crew, "You've got quite the history."),
            new RoleInfo("Sheriff", "Sher", "The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a role is Good or " +
                "Evil. A player's name will change color according to their results.", RoleAlignment.CrewInvest, Faction.Crew, "Guys I promise I'm not an Executioner, I checked Blue and" +
                " they're sus."),
            new RoleInfo("Shifter", "Shift", "The Shifter can swap roles with someone, as long as they are Crew. If the shift is unsuccessful, the Shifter will die.",
                RoleAlignment.CrewSupport, Faction.Crew, "GET BACK HERE I WANT YOUR ROLE."),
            new RoleInfo("Swapper", "Swap", "The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted " +
                "towards the second player and vice versa.", RoleAlignment.CrewSov, Faction.Crew, "Oh no, they totally voted the other guy off. I have no idea why is everyone denying it."),
            new RoleInfo("Tracker", "Track", "InvaThe Tracker can track other during a round. Once they track someone, an arrow is continuously pointing to them, which " +
                "updates in set intervals.lid", RoleAlignment.CrewInvest, Faction.Crew, "I only took up this job because the others were full. Yes it's a proper job. No, I'm not a " +
                "stalker."),
            new RoleInfo("Transporter", "Trans", "The Transporter can swap the locations of two players at will. Players who have been transported are alerted with a blue " +
                "flash on their screen.", RoleAlignment.CrewSupport, Faction.Crew, "You're here and you're there. Where will you go? That's for me to decide."),
            new RoleInfo("Vampire Hunter", "VH", "The Vampire Hunter only spawns if there are Undead in the game. They can check players to see if they are an Undead. When " +
                "the Vampire Hunter finds them, the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a " +
                "Vigilante.", RoleAlignment.CrewAudit, Faction.Crew, "The Dracula could be anywhere! He could be you! He could be me! He could even be- *gets voted off*"),
            new RoleInfo("Veteran", "Vet", "The Veteran can go on alert. When the Veteran is on alert, anyone, even if Crew, who interacts with the Veteran dies.",
                RoleAlignment.CrewKill, Faction.Crew, "Touch me, I dare you."),
            new RoleInfo("Vigilante", "Vig", "The Vigilante can kill. However, if they kill someone they shouldn't, they instead die themselves.", RoleAlignment.CrewKill,
                Faction.Crew, "I AM THE HAND OF JUSTICE."),
            new RoleInfo("Actor", "Act", "The Actor gets a list of roles at the start of the game. The Actor must pretend to be and get guessed as one of the roles in order" +
                " to win.", RoleAlignment.NeutralEvil, Faction.Neutral, "I am totally what you think of me as.", "Get guessed as a role in your target role list."),
            new RoleInfo("Amnesiac", "Amne", "The Amnesiac is essentially roleless and cannot win without remembering the role of a dead player.", RoleAlignment.NeutralBen,
                Faction.Neutral, "I forgor :skull:", "Find a dead body, take their role and then win as that role."),
            new RoleInfo("Arsonist", "Arso", "The Arsonist can douse other players with gasoline. After dousing, the Arsonist can choose to ignite all doused players which " +
                "kills all doused players at once. Doused players have an orange Ξ next to their names", RoleAlignment.NeutralKill, Faction.Neutral, "I like my meat well done.",
                "Douse and ignite anyone who can oppose them"),
            new RoleInfo("Bounty Hunter", "BH", "The Bounty Hunter is assigned a target as the start of the game. They do not know who the target is and must find them via " +
                "a series of clues and limited guesses. Upon finding their target within the set amount of guesses, the guess button becomes a kill button after the next meeting. The " +
                "Bounty Hunter's target always knows that there is a bounty on their head. If the Bounty Hunter is unable to find their target within the number of guesses or their target" +
                " dies not by the Bounty Hunter's hands, the Bounty Hunter turns into a Troll. The target has a red Θ next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral,
                "You can run, but you can't hide.", "Find and kill their bounty."),
            new RoleInfo("Cannibal", "Cann", "The Cannibal can eat the body which wipes away the body, like the Janitor.", RoleAlignment.NeutralEvil, Faction.Neutral, "How " +
                "do you survive with no food but with a lot of people? Improvise, adapt, overcome.", "Eat a certain number of bodies."),
            new RoleInfo("Cryomaniac", "Cryo", "The Cryomaniac can douse in coolant and freeze players similar to the Arsonist's dousing in gasoline and ignite. Freezing " +
                "players does not immediately kill doused targets, instead when the next meeting is called, all currently doused players will die. When the Cryomaniac is the last killer" +
                " or when the final number of players reaches a certain threshold, the Cryomaniac can also directly kill. Doused players have a purple λ next to their names.",
                RoleAlignment.NeutralKill, Faction.Neutral, "Anybody wants ice scream?", "Douse and freeze anyone who can oppose them."),
            new RoleInfo("Dracula", "Drac", "The Dracula is the only Undead that spawns in. The Dracula is the leader of the Undead who can convert others into Undead. If " +
                "the target cannot be converted, they will be attacked instead. The Dracula must watch out for the Vampire Hunter as attempting to convert them will cause the Vampire " +
                "Hunter to kill the Dracula.", RoleAlignment.NeutralNeo, Faction.Neutral, "Everyone calls me a pain in the neck.", "Convert or kill anyone who can oppose the Undead."),
            new RoleInfo("Executioner", "Exe", "The Executioner has no abilities and instead must use gas-lighting techniques to get their target ejected. The Executioner's" +
                " target, by default, is always non-Crew Sovereign Crew. Once their target is ejected, the Executioner can doom those who voted for their target. If their target dies " +
                "before ejected, the Executioner turns into a Jester. Targets have a grey § next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "Source: trust me bro.",
                "Live to see their target ejected."),
            new RoleInfo("Glitch", "Gli", "The Glitch can hack players, resulting in them being unable to do anything for a set duration or they can also mimic someone, " +
                "which results in them looking exactly like the other person. The Glitch can kill normally.", RoleAlignment.NeutralKill, Faction.Neutral, "Hippity hoppity, your code is " +
                "now my property.", "Hippity hoppity, your code is now my property."),
            new RoleInfo("Guesser", "Guess", "The Guesser has no abilities aside from guessing only their target. Every meeting, the Guesser is told a hint regarding their " +
                "target's role. Targets have a beige π next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "I want to know what you are.", "Guess their target's role."),
            new RoleInfo("Jackal", "Jack", "The Jackal is the leader of the Cabal. They spawn in with 2 recruits at the start of the game. One of the recruits is the " +
                "'good' one, meaning they are Crew. The other is the 'evil' recruit, who can be either Intruder, Syndicate or Neutral (Killing). When both recruits die, the Jackal can " +
                "then recruit another player to join the Cabal and become the backup recruit. If the target happens to be a member of a rival subfaction, they will be attacked instead " +
                "and the Jackal will still lose their ability to recruit. Members of the Cabal have a dark grey $ next to their names.", RoleAlignment.NeutralNeo, Faction.Neutral,
                "I've got money.", "Recruit or kill anyone who can oppose Cabal."),
            new RoleInfo("Guardian Angel", "GA", "The Guardian Angel more or less aligns themselves with the faction of their target. The Guardian Angel will win with anyone" +
                " as long as their target lives to the end of the game, even if their target loses. If the Guardian Angel's target dies, they become a Survivor. Targets have a white ★ " +
                "and white η when being protected next to their names.", RoleAlignment.NeutralBen, Faction.Neutral, "Hush child...Mama's here.", "Have their target live to the end of " +
                "the game."),
            new RoleInfo("Jester", "Jest", "The Jester has no abilities and must make themselves appear to be evil to the Crew and get ejected. After getting ejected, the " +
                "Jester can haunt those who voted for them, killing them from beyond the grave.", RoleAlignment.NeutralEvil, Faction.Neutral, "Hehehe I wonder if I do this...",
                "Get ejected."),
            new RoleInfo("Juggernaut", "Jugg", "The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill " +
                "cooldown no longer decreases and instead gives them other buffs, like bypassing protections.", RoleAlignment.NeutralKill, Faction.Neutral, "The doctor told me bones grow" +
                " stronger when recieving damage. But then why did he kick me out when I picked up a hammer?", "Assault anyone who can oppose them."),
            new RoleInfo("Revealer", "Rev", "The Revealer can reveal evils if they finish all their tasks. Upon finishing all of their tasks, Intruders, Syndicate and " +
                "sometimes Neutrals are revealed to alive Crew after a meeting is called. However, if the Revealer is clicked they lose their ability to reveal evils and are once again " +
                "a normal ghost.", RoleAlignment.CrewUtil, Faction.Crew, "I have no idea who I am or what I do, the only thing I see is bad guys who I must reveal."),
            new RoleInfo("Murderer", "Murd", "The Murderer is a simple Neutral Killer with no special abilities.", RoleAlignment.NeutralKill, Faction.Neutral, "I like my " +
                "women like how I like my knives, sharp and painful.", "Murder anyone who can oppose them."),
            new RoleInfo("Necromancer", "Necro", "The Necromancer is essentially an evil Altruist. They can revive dead players and make them join the Necromancer's " +
                "team, the Reanimated. There is a limit to how many times can the Necromancer can kill and revive players.", RoleAlignment.NeutralNeo, Faction.Neutral, "I like the dead," +
                " they do a lot of things I like. For example, staying dead.", "Resurrect the dead and kill off anyone who can oppose the Reanimated."),
            new RoleInfo("Pestilence", "Pest", "Pestilence is always on permanent alert, where anyone who tries to interact with them will die. Pestilence does not spawn " +
                "in-game and instead gets converted from Plaguebearer after they infect everyone. Pestilence cannot die unless they have been voted out, and they can't be guessed " +
                "(usually). This role does not spawn directly, unless it's set to, in which case it will replace the Plaguebearer.", RoleAlignment.NeutralPros, Faction.Neutral,
                "I am the god of disease, nothing can kill me. *Voice from the distance* Ejections can!", "Obliterate anyone who can oppose them."),
            new RoleInfo("Phantom", "Phan", "The Phantom spawns when a Neutral player dies withouth accomplishing their objective. They become half-invisible and have to complete all " +
                "their tasks without getting clicked on to win.", RoleAlignment.NeutralPros, Faction.Neutral, "I'm the one who you should not have killed. *Voice from the distance* " +
                "Get outta here! This is not FNAF!", "Finish tasks without getting caught."),
            new RoleInfo("Plaguebearer", "PB", "The Plaguebearer can infect other players. Once infected, the infected player can go and infect other players via interacting with them. " +
                "Once all players are infected, the Plaguebearer becomes Pestilence.", RoleAlignment.NeutralKill, Faction.Neutral, "*Cough* This should surely work right? *Cough* I sure" +
                " hope it does.", "Infect everyone to become pestilence or kill anyone who can oppose them."),
            new RoleInfo("Serial Killer", "SK", "Although the Serial Killer has a kill button, they can't use it unless they are in Bloodlust. Once the Serial Killer is in bloodlust " +
                "they gain the ability to kill. However, unlike most killers, their kill cooldown is really short for the duration of the bloodlust.", RoleAlignment.NeutralKill,
                Faction.Neutral, "My knife, WHERE'S MY KNIFE?!", "Stab anyone who can oppose them."),
            new RoleInfo("Survivor", "Surv", "The Survivor wins by simply surviving. They can vest which makes them immortal for a short duration.", RoleAlignment.NeutralBen,
                Faction.Neutral, "Hey listen man, I mind my own business and you mind yours. Everyone wins!", "Live to the end of the game."),
            new RoleInfo("Thief", "Thief", "The Thief can kill players to steal their roles. The player, however, must be a role with the ability to kill otherwise the Thief will " +
                "die. After stealing their target's role, the Thief can now win as whatever role they have become.", RoleAlignment.NeutralBen, Faction.Neutral, "Now it's mine.", "Kill" +
                " and steal someone's role."),
            new RoleInfo("Troll", "Troll", "The Troll just wants to be killed, but not ejected. The Troll can \"interact\" with players. This interaction does nothing, it just triggers" +
                " any interaction sensitive roles like Veteran and Pestilence.", RoleAlignment.NeutralEvil, Faction.Neutral, "Kill me. The Impostor: Later.", "Get killed."),
            new RoleInfo("Werewolf", "WW", "The Werewolf can kill all players within a certain radius.", RoleAlignment.NeutralKill, Faction.Neutral, "AWOOOOOOOOOOOOOOOOOOOO", "Maul" +
                " anyone who can oppose them."),
            new RoleInfo("Whisperer", "Whisp", "The Whisperer can whisper to all players within a certain radius. With each whisper, the chances of bringing someone over to the " +
                "Sect increases till they do convert.", RoleAlignment.NeutralNeo, Faction.Neutral, "Psst.", "Persuade or kill anyone who can oppose the Sect."),
            new RoleInfo("Ambusher", "Amb", "The Ambusher can temporaily force anyone to go on alert, killing anyone who interacts with the Ambusher's target.", RoleAlignment.IntruderKill,
                Faction.Intruder, "BOO"),
            new RoleInfo("Blackmailer", "BM", "The Blackmailer can silence people in meetings. During each round, the Blackmailer can go up to someone and blackmail them. This prevents" +
                " the blackmailed person from speaking during the next meeting.", RoleAlignment.IntruderConceal, Faction.Intruder, "Shush."),
            new RoleInfo("Camouflager", "Camo", "The Camouflager does the same thing as the Comms Sabotage, but their camouflage can be stacked on top other sabotages. Camouflaged " +
                "players can kill in front everyone and no one will know who it is.", RoleAlignment.IntruderConceal, Faction.Intruder, "Good luck telling others apart."),
            new RoleInfo("Consigliere", "Consig", "The Consigliere can reveal people's roles. They cannot get Assassin unless they see factions for obvious reasons.",
                RoleAlignment.IntruderSupport, Faction.Intruder, "What...are you?"),
            new RoleInfo("Consort", "Cons", "The Consort can roleblock players and prevent them from doing anything for a short while.", RoleAlignment.IntruderSupport, Faction.Intruder,
                "I'm like the first slice of bread, everyone touches me but no one likes me."),
            new RoleInfo("Disguiser", "Disg", "The Disguiser can disguise into other players. At the beginning of each, they can choose someone to measure. They can then disguise the " +
                "next nearest person into the measured person for a limited amount of time after a short delay.", RoleAlignment.IntruderDecep, Faction.Intruder, "Here, wear this for" +
                " me please. I promise I won't do anything to you."),
            new RoleInfo("Ghoul", "Ghoul", "Every round, the Ghoul can mark a player for death. All players are told who is marked and the marked player will die at the end of the " +
                "next meeting. The only way to save a marked player is to click the Ghoul that marked them.", RoleAlignment.IntruderUtil, Faction.Intruder, "I CURSE YOU!"),
            new RoleInfo("Godfather", "GF", "The Godfather can only spawn in 3+ Intruder games. They can choose to promote a fellow Intruder to Mafioso. When the Godfather dies, " +
                "the Mafioso becomes the new Godfather and has lowered cooldowns.", RoleAlignment.IntruderSupport, Faction.Intruder, "I'm going to make an offer they can't refuse."),
            new RoleInfo("Grenadier", "Gren", "The Grenadier can throw flash grenades which blinds nearby players. However, a sabotage and a flash grenade can not be active at the same" +
                " time.", RoleAlignment.IntruderConceal, Faction.Intruder, "AAAAAAAAAAAAA YOUR EYES"),
            new RoleInfo("Impostor", "Imp", "Just a plain Intruder with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.",
                RoleAlignment.IntruderUtil, Faction.Intruder, "If you ever feel useless, just remember I exist."),
            new RoleInfo("Janitor", "Jani", "The Janitor can drag, drop and clean up bodies. Both their Kill and Clean usually ability have a shared cooldown, meaning they have to choose " +
                "which one they want to use.", RoleAlignment.IntruderConceal, Faction.Intruder, "I'm the guy you call to clean up after you."),
            new RoleInfo("Mafioso", "Mafi", "The Mafioso is promoted from a random non-Godfather Intruder role. The Mafioso by themself is nothing special, but when the Godfather dies," +
                " the Mafioso becomes the new Godfather. As a result, the new Godfather has a lower cooldown on all of their original role's abilities.", RoleAlignment.IntruderUtil,
                Faction.Intruder, "Yes, boss. Got it, boss."),
            new RoleInfo("Miner", "Miner", "The Miner can create new vents. These vents only connect to each other, forming a new passageway.", RoleAlignment.IntruderSupport,
                Faction.Intruder, "Dig, dig, diggin' some rave; making some loud sound waves; the only thing you'll be diggin' is your own grave."),
            new RoleInfo("Morphling", "Morph", "The Morphling can morph into another player. At the beginning of each round, they can choose someone to sample. They can then morph " +
                "into that person at any time for a limited amount of time.", RoleAlignment.IntruderDecep, Faction.Intruder, "*Casually observing the chaos over Green seeing Red kill." +
                "* It was me."),
            new RoleInfo("Teleporter", "Tele", "The Teleporter can teleport to a marked positions. The Teleporter can mark a location which they can then teleport to later.",
                RoleAlignment.IntruderSupport, Faction.Intruder, "He's here, he's there, he's everywhere. Who are ya gonna call? Psychic friend fr-"),
            new RoleInfo("Wraith", "Wraith", "The Wraith can temporarily turn invisible.", RoleAlignment.IntruderDecep, Faction.Intruder, "Now you see me, now you don't."),
            new RoleInfo("Anarchist", "Anarch", "Just a plain Syndicate with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode. Its only " +
                "benefit is its ability to kill from the beginning of the game. With the Chaos Drive, the Anarchist's kill cooldown decreases.", RoleAlignment.SyndicateUtil,
                Faction.Syndicate, "If you ever feel useless, just remember that I also exist."),
            new RoleInfo("Banshee", "Bansh", "The Banshee can block every non-Syndicate player every once in a while. This role cannot get the Chaos Drive.", RoleAlignment.SyndicateUtil,
                Faction.Syndicate, "AAAAAAAAAAAAAAA"),
            new RoleInfo("Concealer", "Conceal", "The Concealer can make a player invisible for a short while. With the Chaos Drive, this applies to everyone.",
                RoleAlignment.SyndicateDisruption, Faction.Syndicate, "Invalid"),
            new RoleInfo("Crusader", "Crus", "The Crusader can temporaily force anyone to go on alert, killing anyone who interacts with the Crusader's target. With the Chaos Drive," +
                " attempting to interact with the Crusader's target will cause the target to kill everyone within a certain range, including the target themselves.",
                RoleAlignment.SyndicateKill, Faction.Syndicate, "I WILL PURGE THIS UNHOLY LAND!"),
            new RoleInfo("Framer", "Framer", "The Framer can frame players, making them appear to have wrong results and be easily killed by Vigilantes and Assassins. This effects " +
                "lasts as long as the Framer is alive. With the Chaos Drive, the Framer can frame players within a certain radius.", RoleAlignment.SyndicateDisruption, Faction.Syndicate,
                "Who knew old documents can get people into trouble?"),
            new RoleInfo("Poisoner", "Pois", "The Poisoner can poison another player instead of killing. When they poison a player, the poisoned player dies either upon the start of the" +
                " next meeting or after a set duration. With the Chaos Drive, the Poisoner can poison a player from anywhere.", RoleAlignment.SyndicateKill, Faction.Syndicate,
                "So now if you mix these together, you end up creating this...thing."),
            new RoleInfo("Rebel", "Reb", "The Rebel can only spawn in 3+ Syndicate games. They can choose to promote a fellow Syndicate to Sidekick. When the Rebel dies, the Sidekick" +
                " becomes the new Rebel and has lowered cooldowns. With the Chaos Drive, the Rebel's gains the improved abilities of their former role. A promoted Rebel has the highest" +
                " priority when recieving the Chaos Drive and the original Rebel as the lowest priority.", RoleAlignment.SyndicateSupport, Faction.Syndicate, "DOWN WITH THE GOVERNMENT!"),
            new RoleInfo("Shapeshifter", "SS", "The Shapeshifter can swap the appearances of 2 players. WIth the Chaos Drive, everyone's appearances are suffled.",
                RoleAlignment.SyndicateDisruption, Faction.Syndicate, "Everyone! We will be playing dress up! TOGETHER!"),
            new RoleInfo("Sidekick", "Side", "The Sidekick is promoted from a random non-Rebel Syndicate role. The Sidekick by themselves is nothing special, but when the Rebel " +
                "dies, the Sidekick becomes the new Rebel. As a result, the new Rebel has a lower cooldown on all of their original role's abilities.", RoleAlignment.SyndicateUtil,
                Faction.Syndicate, "Learning new things."),
            new RoleInfo("Warper", "Warp", "The Warper can teleport a player to another player. With the Chaos Drive, the Warper teleports everyone to random positions on the map.",
                RoleAlignment.SyndicateSupport, Faction.Syndicate, "BEGONE!"),
            new RoleInfo("Politician", "Pol", "The Politician can vote multiple times like the Mayor. However, unlike the Mayor, the Politician does not gain votes every meeting " +
                "without the Chaos Drive and must kill players to gain votes", RoleAlignment.SyndicatePower, Faction.Syndicate, "The votes are rigged...by me."),
            new RoleInfo("Enforcer", "Enf", "The Enforcer can plant bombs on players. After a short while, the target will be alerted to the bomb's presence and must kill someone to " +
                "get rid of it. If they fail to kill someone within a certain time limit, tje bomb will explode, killing everyone within its vicinity.", RoleAlignment.IntruderKill,
                Faction.Intruder, "You will do as I say...unless you want to be the painting on the walls."),
            new RoleInfo("Bomber", "Bomb", "The Bomber can place a bomb which can be remotely detonated at any time. Anyone caught inside the bomb's radius at the time of detonation will" +
                " be killed. Only the latest placed bomb will detonate, unless the Bomber holds the Chaos Drive, with which they can detonate all bombs at once.",
                RoleAlignment.SyndicateKill, Faction.Syndicate, "KABOOM!!"),
            new RoleInfo("Detective", "Det", "The Detective can examine other players for bloody hands. If the examined player has killed recently, the Detective will be alerted about " +
                "it. The Detective can also see the footprints of players. All footprints disappear after a set amount of time and only the Detective can see them.",
                RoleAlignment.CrewInvest, Faction.Crew, "I am skilled in identifying blood...yup that's defintely blood."),
            new RoleInfo("Betrayer", "Bet", "The Betrayer is a simple killer, who turned after a turned Traitor/Fanatic was the only member of their new faction remaning. This role does" +
                " not spawn directly.", RoleAlignment.NeutralPros, Faction.Neutral, "Kill anyone who opposes the faction they defected to")
        };

        public readonly static List<ModifierInfo> AllModifiers = new()
        {
            new ModifierInfo("Invalid", "Invalid", "Invalid", "Invalid"),
            new ModifierInfo("Bait", "Bait", "The Bait's killer will be forced to self-report.", "Everyone except Troll, Vigilate, Altruist, Thief and Shifter"),
            new ModifierInfo("Coward", "Coward", "The Coward cannot report bodies.", "Everyone"),
            new ModifierInfo("Diseased", "Diseased", "The Diseased's killer's kill cooldown will be increased for the next attack.", "Everyone except Troll and Altruist"),
            new ModifierInfo("Drunk", "Drunk", "The Drunk's controls are inverted.", "Everyone"),
            new ModifierInfo("Dwarf", "Dwarf", "The Dwarf's body is smaller and they are faster.", "Everyone"),
            new ModifierInfo("Giant", "Giant", "The Giant's body is bigger and they are slower.", "Everyone"),
            new ModifierInfo("Flincher", "FLinch", "The Flincher will randomly twitch backwards. Fun Fact: The Flincher is actually a bug which I turned into a modifier.", "Everyone"),
            new ModifierInfo("Shy", "Shy", "The Shy player cannot call meetings.", "Everyone except Button Barries and those who cannot call meetings (like Mayors when the Mayor Button" +
                " setting is off)"),
            new ModifierInfo("Indomitable", "Ind", "The Indomitable cannot be guessed.", "Everyone except Actor"),
            new ModifierInfo("VIP", "VIP", "When the VIP dies, everyone is alerted to their death and their screen will flash in the color of the VIP's role.", "Everyone"),
            new ModifierInfo("Professional", "Prof", "The Professional has an extra life when guessing.", "Assassins"),
            new ModifierInfo("Volatile", "Vol", "The Volatile will always see/hear random things.", "Everyone")
        };

        public readonly static List<ObjectifierInfo> AllObjectifiers = new()
        {
            new ObjectifierInfo("Invalid", "Invalid", "Invalid", "Invalid", "Invalid", "Invalid"),
            new ObjectifierInfo("Taskmaster", "TMer", "The Taskmaster is basically a living Phantom. When a certain number of tasks are remaining, the Taskmaster is revealed" +
                " to Intruders and the Syndicate and the Crew only sees a flash to indicate the Taskmaster's existence.", "Finish tasks without dying or game ending", "Neutrals", "µ"),
            new ObjectifierInfo("Lovers", "Lover", "The Lovers are two players who are linked together. They gain the primary objective to stay alive together. If they are both among " +
                "the last 3 players, they win as a Lover pair. In order to so, they gain access to a private chat, only visible by them in between meetings. However, they can also win" +
                " with their respective team, hence why the Lovers do not know the role of the other Lover", "Live to the final 3 with both Lvoers still alive", "Everyone", "♥"),
            new ObjectifierInfo("Rivals", "Rival", "The Rivals cannot do anything to each other and must get the other one killed.", "Get the other rival killed without directly " +
                "interfering, then live to the final 2.", "Everyone", "α"),
            new ObjectifierInfo("Allied", "Ally", "An Allied Neutral Killer now sides with either the Crew, the Intruders or the Syndicate. In the case of the latter two, all " +
                "faction members are shown the Allied player's role, and can no longer kill them.", "Win with whichever faction they are aligned with", "Neutral Killers", "ζ"),
            new ObjectifierInfo("Fanatic", "Fan", "When attacked, the Fanatic joins whichever faction their attacker belongs to. From then on, their alliance sits with said faction.",
                "Get attacked by either the Intruders or the Syndicate to join their team", "Crew", "♠"),
            new ObjectifierInfo("Overlord", "Ov", "Every meeting, for as long as an Overlord is alive, players will be alerted to their existence. The game ends if the Overlord lives " +
                "long enough.", "Survive a set amount of meetings", "Neutrals", "β"),
            new ObjectifierInfo("Corrupted", "Corr", "The Corrupted is a Crewmate with the alignment of a Neutral Killer. On top of their base role's attributes, they also gain a " +
                "kill button. Their win condition is so strict that not even Neutral Benigns or Evils can be spared", "Kill everyone", "Crew", "δ"),
            new ObjectifierInfo("Traitor", "CT (means Crew Traitor)", "The Traitor is a Crewmate who must finish their tasks to switch sides. Upon doing so, they will either join " +
                "the Intruders or the Syndicate, and will win with that faction. If the Traitor is the only person in their new faction, they become a Betrayer, losing their original" +
                " role's abilities and gaining the ability to kill in the process.", "Finish tasks to join either the Intruders or Syndicate", "Crew", "♣")
        };

        public readonly static List<AbilityInfo> AllAbilities = new()
        {
            new AbilityInfo("Invalid", "Invalid", "Invalid", "Invalid"),
            new AbilityInfo("Assassin", "Assassin", "The Assassin is given to a certain number of Intruders, Syndicate and/or Neutral Killers. This ability gives the Intruder, Syndicate" +
                " or Neutral Killer a chance to kill during meetings by guessing the roles or modifiers of others. If they guess wrong, they die instead.", "Everyone except Neutral Evil," +
                " Proselyte and Benign roles"),
            new AbilityInfo("Button Barry", "BB", "Button Barry has the ability to call a meeting from anywhere on the map, even during sabotages. Calling a meeting during a non-" +
                "critical sabotage will fix the sabotage.", "Everyone except those who cannot call meetings (like Mayors when the Mayor Button setting is off"),
            new AbilityInfo("Insider", "Ins", "The Insider will be able to view everyone's votes in meetings upon finishing their tasks. Only spawns if Anonymous Votes is turn on.",
                "Crew"),
            new AbilityInfo("Multitasker", "MT", "When doing tasks, the Multitasker's task window is transparent.", "Roles with tasks"),
            new AbilityInfo("Ninja", "Nin", "Ninjas don't lunge when killing.", "Killing roles"),
            new AbilityInfo("Radar", "Radar", "The Radar always has an arrow pointing towards the nearest player.", "Everyone"),
            new AbilityInfo("Ruthless", "Ruth", "A Ruthless killer can bypass all forms of protection. Although they bypass alert protection, they will still die to a player on alert.",
                "Killing roles"),
            new AbilityInfo("Snitch", "Snitch", "The Snitch is an ability which allows any Crewmate to get arrows pointing towards the Intruders once all their tasks are finished. " +
                "The names of the Intruders will also show up as red on their screen. However, when they only have a single task left, the Intruders get an arrow pointing towards " +
                "the Snitch.", "non-Traitor or Fanatic Crew"),
            new AbilityInfo("Tiebreaker", "TB", "If any vote is a draw, the Tiebreaker's vote will go through. If they voted another player, they will get voted out. If the Tiebreaker " +
                "is the Mayor, it applies to the Mayor's first vote.", "Everyone"),
            new AbilityInfo("Torch", "Torch", "The Torch's has Intruder vision at all times.", "Crew, Neutral Evil and Benign roles, Neutrals and Neutral Killers when their respective " +
                "lights are off"),
            new AbilityInfo("Tunneler", "Tun", "The Tunneler will be able to vent when they finish their tasks.", "Crew except Engineer"),
            new AbilityInfo("Underdog", "UD", "The Underdog is an Intruder with a prolonged kill cooldown when with a teammate. When they are the only remaining Intruder, they will " +
                "have their kill cooldown shortened.", "Intruders and Syndicate")
        };

        public readonly static List<Lore> AllLore = new()
        {
            new Lore("All", "The story takes place several hundred years in the future. There is a giant leap in scientific advancement. As a result, space travel is a common thing and" +
                " is commonly used for mining resources from nearby celestial bodies. A multi-billion dollar government space exploration corporation called \"Mira\" just recently " +
                "discovered a new habitable planet several light years away. They named it \"Polus\". Soon enough, they build a ship that can carry over 200 passengers with plenty of " +
                "storage for items. The ship is called the \"Skeld\". Mira begins an exploration trip to Polus, recruiting special individuals from within their agency to set up camp and" +
                " continue the research of the planet. Little do they know, all sorts of people whose motives conflict with that of Mira have managed to get onboard the Skeld. Not only " +
                "that, some of the Crew have been mysteriously replaced by shapeshifting parasites hellbent on conquering Earth but unfortunately got swept up in this project. Skeld is " +
                "soon launched from the Mira HQ. After launch, the Skeld becomes a battlefield of death and mind games while the Crew tries to get rid of these evils when stranded in " +
                "space. These are the stories of said passengers aboard the Skeld.", "All"),
            new Lore("Swapper", "There was once a Crewmate who made the voting software in light of the recent events occurring aboard the Skeld. The software would tally up the votes " +
                "against someone, use a mechanical arm to detain the voted person and transport said voted person to the airlock for ejection. He decided, since he was the maker of " +
                "the voting software, he has full reign. And so he does. He witnesses the Godfather kill the Medic and the Janitor clean up the mess in front of him and calls a meeting." +
                " The Intruders, using their silver tongues, slip out of the blame and instead push it on the Crewmate. The Crewmate knew he had only one thing to do. He told everyone to" +
                " vote for him and then vote for the others. Confused, the rest of the Crew voted for him. The Godfather and the Janitor, with villainous smirks on their faces, watched " +
                "the votes get tallied. Soon the faces of confidence turn into faces of pure shock, as the votes are tallied not against the Crewmate, but against the Godfather. A " +
                "mechanical arm juts out from the walls and grabs the Godfather and starts moving towards the airlock. The Crew, held aback by the sudden change in air, looked at the " +
                "Crewmate. The Godfather looked at the Crewmate with a face of horror through the window in the airlock door. The Crewmate simply meets his gaze with a face of pure joy " +
                "and confidence, and mouths, \"I'm the Swapper, bitch.\"", "Swap"),
            new Lore("Amnesiac", "A lonely Mystic walked down the hallway. His head was aching and the bandages around his head were starting to loosen. The Mystic just wanted to " +
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
            new Lore("Glitch", "The Operative woke up to see a dreary yellow maze-like world around him. The maze seemed infinitely large, as there was no limit to be seen. He tried his " +
                "best to survive in this maze, preserving what was left of his sanity. Several years passed by and by some miracle, he found a vent. Not even thinking that it might be " +
                "leading to nowhere, he hopped into it. He was nearing death so he was not at liberty to decide. As he fell further down, he questioned his choice, thinking it might " +
                "just be his end. But then there he was, lying on the floor of the laboratory, in front of the vitals screen. His body seemed fine, heck, even better. The Operative " +
                "was shocked, it was as if he was not gone for a second in the main world. Everything was as it was when he \"glitched\" through. What greeted him wasn't any of the " +
                "Crew, it was a parasite. The parasite entered the Operative's body. The parasite and the Operative struggled to gain control of the body, and the Operative succeeded. " +
                "But he felt...new. Powerful, even. In his mind, all he could think of was the pain and suffering he went through. He couldn't forgive the Crew who didn't even bother " +
                "to look for him, let alone save him. He needed to make the world aware of what he went through. And he had just the right tools with him. Controlling the dead parasite " +
                "within his body and harnessing its power, along with his warped anatomy thanks to reality breaking, the Operative fused himself with a lot of the technology around him, " +
                "in an attempt to get stronger. He invented devices to hack the Crew's and Intruders' systems alike, and an illusion device to change his appearance. It was showtime. " +
                "The best thing the Operative could do right now was kill everyone and hope they get transported to the maze, to feel what he did. He was the one who transcended reality, " +
                "he was the one with the knowledge to break through the universe's strongest barrier. He was...the Glitch.", "Gli"),
            new Lore("Juggernaut", "A paranoid Veteran watched his loved one die to the hands of the Crew. He couldn't bear to think he wasn't there to save her. \"If only I was stronger" +
                ".\" was the thought that plagued his mind. Day in and day out, he pursued strength, in his ultimate goal to destroy Mira, the very company that killed his wife in cold " +
                "blood. But, he just couldn't shake off the paranoia from the war. No amount of self healing or meditating could take away those horrid memories from wartime. His wife " +
                "was his only way to support himself, his lifeline. Everytime he thought of her, he would be engulfed in deadly rage, unable to calm down until his fists bled from " +
                "punching the walls. One day, he saw a job listing to explore a newly discovered planet, Polus. The advertiser? Mira. \"Perfect.\" thought the Veteran as he lifted himself" +
                " up from his couch, and readied his uniform to go to the application site. He got to the site, to only see that Mira wasn't even performing background checks on the " +
                "applicants. \"That lax behaviour will get you killed, Mira.\" After a few days, he received an acceptance letter. He was accepted! He boarded the ship to see familiar " +
                "faces, as well suspicious ones. The Mayor, the one who led the team that killed his wife and the Godfather, who he suspected was the cause. Aboard the ship he met new " +
                "faces, the weak Crewmate, the just Sheriff and the lovely Medic. She reminded him of his wife. But he could not spare his feelings for her. She was affiliated with Mira, " +
                "making her his enemy. A couple days later, he set off on the journey to Polus. He thought this was his time to shine, but he couldn't bring himself to kill anyone. Most " +
                "of the people here were innocent, forced to go on the mission with him. He couldn't hurt these poor souls. The Godfather paid mind to the Veteran's antics, and struck a " +
                "deal with him. They met up in the infirmary to talk business. As they were discussing, the Godfather let loose a parasite, to take over and control the Veteran. Just as " +
                "he began, the Veteran felt a tingle, but thought nothing of it. The infirmary doors burst open with the Sheriff and the Seer entering with full force. \"AHA! I KNEW YOU " +
                "WERE UP TO NO GOOD!\" said the robust Sheriff, preparing his gun to fire. But that declaration triggered something within the Veteran. He heard an audible snap and then " +
                "he felt bliss. A couple minutes later, a meeting was called by the Medic. \"3 bodies were found in the infirmary...brutally mangled beyond recognition.\" she declared, " +
                "holding back the urge to puke. As the rest of the Crew gasped in horror, only the Juggernaut smiled.", "Jugg"),
            new Lore("Medic", "The Medic came from a highly evaluated university, with the highest grades possible. She was the best in the field, until an accident took her ability to " +
                "heal others. Ashamed of her incident and only being able to perform rudimentary first aid, the Medic sought for a job on the Polus mission. She was accepted! This was " +
                "her turning point, one where she would be known for something else, and hopefully heal herself in the process. The Medic's dreams fell short as people started dying " +
                "to mysterious killers among the Crew. She tried her best to find people to protect, but her arrivals were too little too late. The only things waiting for her were the " +
                "bodies of those she swore to protect and a lingering sense of dread. She couldn't get by just looking for people to heal and instead decided to concentrate on one " +
                "person, so at least they'd be safe. It was the Crewmate, a loveable simpleton who only stood for justice. She would know when he would be attacked. All she had to do " +
                "was just sit and lie in wait, patiently waiting for a killer to slip up and attack the Crewmate and alerting her. But on her way to the cafeteria, she heard wheezing. " +
                "It was the corrupt Mayor, the one who was the sole reason behind the Medic's accident. Her entire being said let him die, but only her heart said to save him, for she " +
                "was not a monster, but a barrier between life and death for the Crew. She slowly approached the Mayor, pushing down her hatred for him. \"Where does it hurt?\"", "Medic"),
            new Lore("Crewmate", "Nothing fruitful ever happened in the Crewmate's life. He was just lucky enough to get a spot in the exploration trip to Polus. Only useful for " +
                "finishing tasks and basic repairs, he decided to make the most of his time aboard the Skeld. Getting acquainted with all those famous celebrities from Mira, the " +
                "Crewmate felt a sense of bliss and happiness. He was going to make history. He was finally going to be able to stand with the celebrities like the Mayor. That would'" +
                "ve happened, if it were not for the Intruder aboard their ship. First it was the disruption of their tasks, then it was the sabotages and then finally the Intruders " +
                "took a step further. Killing. The Crewmate feared the loss of his life and went into hiding. He knew one thing, he had to ensure the killers got thrown out of the ship" +
                " in order for him to survive.", "Crew")
        };
    }
}