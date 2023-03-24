using System.Collections.Generic;
using System.Text;
using TownOfUsReworked.Enums;
using HarmonyLib;

namespace TownOfUsReworked.Classes
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
            public Faction Faction { get; set; }

            private readonly string SyndicateDescription = "Syndicate is an \"evil\" faction that is an informed minority of the game. They have special abilities specifically geared towards" +
                " slowing down the progress of other or causing chaos. Syndicate members, unless they are Syndicate (Killing), Anarchist or Sidekick, cannot kill by default. Instead" +
                " they gain the ability to kill by obtaining a powerup called the Chaos Drive. The Chaos Drive not only boosts each member's abilities but also gives them the ability" +
                " to kill if they didn't already.";
            private readonly string CrewDescription = "The Crew is the uninformed majority of the game. They are the \"good guys\". It is their job to deduce who is evil and who is not and vote" +
                " them out.";
            private readonly string IntruderDescription = "Intruders are the main \"bad guys\" of the game and an informed minority of the game. All roles have the capability to kill and" +
                " sabotage, making them a pain to deal with.";
            private readonly string NeutralDescription = "Neutrals are essentially factionless. They are the uninformed minority of the game and can only win by themselves.";

            private readonly string IntruderObjective = "Have a critical sabotage reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew, Neutral Killers, Proselytes and " +
                "Neophytes.";
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

            private readonly string CrewObjective = "Finish your tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, Neutral Killers, Proselytes and Neophytes.";
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
                Faction = faction;
                Quote = quote;

                switch (faction)
                {
                    case Faction.Syndicate:
                        FactionDescription = SyndicateDescription;
                        WinCon = SyndicateObjective;
                        FactionShort = "Syn";
                        break;
                    case Faction.Intruder:
                        FactionDescription = IntruderDescription;
                        WinCon = IntruderObjective;
                        FactionShort = "Int";
                        break;
                    case Faction.Crew:
                        FactionDescription = CrewDescription;
                        WinCon = CrewObjective;
                        FactionShort = "Crew";
                        break;
                    case Faction.Neutral:
                        FactionDescription = NeutralDescription;
                        WinCon = wincon;
                        FactionShort = "Neut";
                        break;
                    case Faction.None:
                        FactionDescription = "Invalid";
                        WinCon = "Invalid";
                        FactionShort = "Invalid";
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
                builder.Append("Name: ").Append(Faction).AppendLine()
                    .Append("Short Form: ").AppendLine(FactionShort)
                    .Append("Description: ").AppendLine(FactionDescription);
                return builder.ToString();
            }

            public string QuoteInfoMessage() => Quote;
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
                builder.Append("Role Name: ").AppendLine(Name)
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
                builder.Append("Role Name: ").AppendLine(Name)
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
                builder.Append("Role Name: ").AppendLine(Name)
                    .Append("Short Form: ").AppendLine(Short)
                    .Append("Applies To: ").AppendLine(AppliesTo)
                    .Append("Description: ").AppendLine(Description);
                return builder.ToString();
            }
        }

        public class Lore
        {
            public string Name { get; set; }
            public string Story { get; set; }

            public Lore(string name, string story)
            {
                Name = name;
                Story = story;
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
            new RoleInfo("Agent", "Ag", "The Agent gains more information when on Admin Table and on Vitals. On Admin Table, the Agent can see the colors of every person" +
                " on the map. When on Vitals, the Agent is shown how long someone has been dead for.", RoleAlignment.CrewInvest, Faction.Crew, "Hippity hoppity, your privacy is now " +
                "my property."),
            new RoleInfo("Altruist", "Alt", "The Altruist is capable of reviving dead players. Upon finding a dead body, the Altruist can hit their revive button, " +
                "sacrificing themselves for the revival of the dead player. If enabled, the dead body disappears, so only they Altruist's body remains at the scene. After a set period" +
                " of time, the player will be resurrected, if the revival isn't interrupted. Once revived all evil players will be notified of the revival and will have an arrow " +
                "pointing towards the revived player.", RoleAlignment.CrewProt, Faction.Crew, "I know what I have to do but I don't know if I have the strength to do it."),
            new RoleInfo("Chameleon", "Cham", "The Chameleon can go invisible to stalk players and see what they do when no one is around.", RoleAlignment.CrewSupport,
                Faction.None, "He's here he's there he's everywhere! Who're you gonna call? Psychic friend Chameleon!"),
            new RoleInfo("Coroner", "Cor", "The Coroner gets an alert when someone dies. On top of this, the Coroner briefly gets an arrow pointing in the direction of " +
                "the body. They can autopsy bodies to get some information. They can then compare that information with players to see if they killed the body or not. The Coroner also " +
                "gets a body report from the player they reported. The report will include the cause and time of death, player's faction/role, the killer's faction/role and (according " +
                "to the settings) the killer's name.", RoleAlignment.CrewInvest, Faction.Crew, "A body? Where? I need it for...scientific purposes."),
            new RoleInfo("Crewmate", "Crew", "Just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.",
                RoleAlignment.CrewUtil, Faction.Crew, "I once made a pencil using 2 erasers...they were pointless, just like me."),
            new RoleInfo("Engineer", "Engi", "The Engineer can fix sabotages from anywhere on the map. They can also use vents to get across the map easily.",
                RoleAlignment.CrewSupport, Faction.Crew, "How am I going to stop some big mean mother hubbard from tearing me a structurally superfluous new behind? The solution? A wrench."),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid"),
            new RoleInfo("Invalid", "Invalid", "Invalid", RoleAlignment.None, Faction.None, "Invalid", "Invalid")
        };

        public readonly static List<ModifierInfo> AllModifiers = new();
        public readonly static List<ObjectifierInfo> AllObjectifiers = new();
        public readonly static List<AbilityInfo> AllAbilities = new();
        public readonly static List<Lore> AllLore = new();

        public static void LoadInfo()
        {
            AllRoles.Clear();
            AllModifiers.Clear();
            AllAbilities.Clear();
            AllObjectifiers.Clear();

            AllRoles.Add(new RoleInfo("Escort", "Esc", "The Escort can roleblock players and prevent them from doing anything for a short while.", RoleAlignment.CrewSupport, Faction.Crew,
                "Today, I will make you a man."));
            AllRoles.Add(new RoleInfo("Inspector", "Insp", "The Inspector can check players for their roles. Upon being checked, the targets' names will be updated to give a list of what" +
                " roles could the target possibly be.", RoleAlignment.CrewInvest, Faction.Crew, "THAT'S THE GODFATHER! YOU GOTTA BELIEVE ME."));
            AllRoles.Add(new RoleInfo("Mayor", "Mayo (XD)", "The Mayor can vote multiple times. The Mayor has a Vote Bank, which is the number of times they can vote. They have the option" +
                " to abstain their vote during a meeting, adding that vote to the Vote Bank. As long as not everyone has voted, the Mayor can use as many votes from their Vote Bank as they" +
                " please.", RoleAlignment.CrewSov, Faction.Crew, "Um, those votes are legitimate. No, I'm not rigging the votes."));
            AllRoles.Add(new RoleInfo("Medic", "Medic", "The Medic can give any player a shield that will make them immortal until the Medic is dead. A shielded player cannot be " +
                "killed by anyone, unless it's a suicide. Shielded players have a green ✚ next to their names.", RoleAlignment.CrewProt, Faction.Crew, "Where does it hurt?"));
            AllRoles.Add(new RoleInfo("Medium", "Med", "The Medium can mediate to be able to see ghosts. If the Medium uses this ability, the Medium and the dead player will be able to " +
                "see each other and communicate from beyond the grave!", RoleAlignment.CrewInvest, Faction.Crew, "The voices...they are telling me...my breath stinks? Can ghosts even smell?"));
            AllRoles.Add(new RoleInfo("Mystic", "Mys", "The Mystic only spawns when there is at least one Neutral (Neophyte) role present in the game. Whenever someone's subfaction is " +
                "changed, the Mystic will be alerted about it. The Mystic can also investigate players to see if their subfactions have been changed. If the target has a different " +
                "subfaction, the Mystic's screen will flash red, otherwise it will flash green. It will not, however, work on the Neutral (Neophyte) roles themselves so even people who" +
                " flashed green might be a converter. Once all subfactions are dead, the Mystic becomes a Seer.", RoleAlignment.CrewAudit, Faction.Crew, "There's a hint of corruption."));
            AllRoles.Add(new RoleInfo("Operative", "Op", "The Operative can place bugs around the map. When players enter the range of the bug, they trigger it. In the following meeting," +
                " all players who triggered a bug will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the bug, nor what " +
                "role a specific player is.", RoleAlignment.CrewInvest, Faction.Crew, "The only thing you need to find out information is good placement and amazing levels of prediction."));
            AllRoles.Add(new RoleInfo("Retributionist", "Ret", "The Retributionist can mimic dead crewamtes. During meetings, the Retributionist can select who they are going to " +
                "ressurect and use for the following round from the dead. They can choose to use each dead players as many times as they wish. It should be noted the Retributionist " +
                "can not use all Crew roles and cannot use any Non-Crew role. The cooldowns, limits and everything will be set by the settings for their respective roles.",
                RoleAlignment.CrewSupport, Faction.Crew, "Bodies...bodies...I NEED BODIES."));
            AllRoles.Add(new RoleInfo("Seer", "Seer", "The Seer only spawns if there are roles capable of changing their initial roles. The Seer can investigate players to see if their" +
                " role is different from what they started out as. If a player's role has been changed, the Seer's screen will flash red, otherwise it will flash green. This, however, " +
                "does not work on those whose subfactions have changed so those who flashed green might still be evil. If all players capable of changing or have changed their initial " +
                "roles are dead, the Seer becomes a Sheriff.", RoleAlignment.CrewInvest, Faction.Crew, "You've got quite the history."));
            AllRoles.Add(new RoleInfo("Sheriff", "Sher", "The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a role is Good or " +
                "Evil. A player's name will change color according to their results.", RoleAlignment.CrewInvest, Faction.Crew, "Guys I promise I'm not an Executioner, I checked Blue and" +
                " they're sus."));
            AllRoles.Add(new RoleInfo("Shifter", "Shift", "The Shifter can swap roles with someone, as long as they are Crew. If the shift is unsuccessful, the Shifter will die.",
                RoleAlignment.CrewSupport, Faction.Crew, "GET BACK HERE I WANT YOUR ROLE."));
            AllRoles.Add(new RoleInfo("Swapper", "Swap", "The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted " +
                "towards the second player and vice versa.", RoleAlignment.CrewSov, Faction.Crew, "Oh no, they totally voted the other guy off. I have no idea why is everyone denying it."));
            AllRoles.Add(new RoleInfo("Time Lord", "TL", "The Time Lord can rewind time and reverse the positions of all players. If enabled, any players killed during this time will be" +
                " revived. Nothing but movements and kills are affected.", RoleAlignment.CrewSupport, Faction.Crew, "What's better than an Altruist? An Altruist that dosen't die!"));
            AllRoles.Add(new RoleInfo("Tracker", "Track", "InvaThe Tracker can track other during a round. Once they track someone, an arrow is continuously pointing to them, which " +
                "updates in set intervals.lid", RoleAlignment.CrewInvest, Faction.Crew, "I only took up this job because the others were full. Yes it's a proper job. No, I'm not a stalker."));
            AllRoles.Add(new RoleInfo("Transporter", "Trans", "The Transporter can swap the locations of two players at will. Players who have been transported are alerted with a blue " +
                "flash on their screen.", RoleAlignment.CrewSupport, Faction.Crew, "You're here and you're there. Where will you go? That's for me to decide."));
            AllRoles.Add(new RoleInfo("Vampire Hunter", "VH", "The Vampire Hunter only spawns if there are Undead in the game. They can check players to see if they are an Undead. When " +
                "the Vampire Hunter finds them, the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a " +
                "Vigilante.", RoleAlignment.CrewAudit, Faction.Crew, "The Dracula could be anywhere! He could be you! He could be me! He could even be- *gets voted off*"));
            AllRoles.Add(new RoleInfo("Veteran", "Vet", "The Veteran can go on alert. When the Veteran is on alert, anyone, even if Crew, who interacts with the Veteran dies.",
                RoleAlignment.CrewKill, Faction.Crew, "Touch me, I dare you."));
            AllRoles.Add(new RoleInfo("Vigilante", "Vig", "The Vigilante can kill. However, if they kill someone they shouldn't, they instead die themselves.", RoleAlignment.CrewKill,
                Faction.Crew, "I AM THE HAND OF JUSTICE."));
            AllRoles.Add(new RoleInfo("Actor", "Act", "The Actor gets a list of roles at the start of the game. The Actor must pretend to be and get guessed as one of the roles in order" +
                " to win.", RoleAlignment.NeutralEvil, Faction.Neutral, "I am totally what you think of me as.", "Get guessed as a role in your target role list."));
            AllRoles.Add(new RoleInfo("Amnesiac", "Amne", "The Amnesiac is essentially roleless and cannot win without remembering the role of a dead player.", RoleAlignment.NeutralBen,
                Faction.Neutral, "I forgor :skull:", "Find a dead body, take their role and then win as that role."));
            AllRoles.Add(new RoleInfo("Arsonist", "Arso", "The Arsonist can douse other players with gasoline. After dousing, the Arsonist can choose to ignite all doused players which " +
                "kills all doused players at once. Doused players have an orange Ξ next to their names", RoleAlignment.NeutralKill, Faction.Neutral, "I like my meat well done.",
                "Douse and ignite anyone who can oppose them"));
            AllRoles.Add(new RoleInfo("Bounty Hunter", "BH", "The Bounty Hunter is assigned a target as the start of the game. They do not know who the target is and must find them via " +
                "a series of clues and limited guesses. Upon finding their target within the set amount of guesses, the guess button becomes a kill button after the next meeting. The " +
                "Bounty Hunter's target always knows that there is a bounty on their head. If the Bounty Hunter is unable to find their target within the number of guesses or their target" +
                " dies not by the Bounty Hunter's hands, the Bounty Hunter turns into a Troll. The target has a red Θ next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral,
                "You can run, but you can't hide.", "Find and kill their bounty."));
            AllRoles.Add(new RoleInfo("Cannibal", "Cann", "The Cannibal can eat the body which wipes away the body, like the Janitor.", RoleAlignment.NeutralEvil, Faction.Neutral, "How " +
                "do you survive with no food but with a lot of people? Improvise, adapt, overcome.", "Eat a certain number of bodies."));
            AllRoles.Add(new RoleInfo("Cryomaniac", "Cryo", "The Cryomaniac can douse in coolant and freeze players similar to the Arsonist's dousing in gasoline and ignite. Freezing " +
                "players does not immediately kill doused targets, instead when the next meeting is called, all currently doused players will die. When the Cryomaniac is the last killer" +
                " or when the final number of players reaches a certain threshold, the Cryomaniac can also directly kill. Doused players have a purple λ next to their names.",
                RoleAlignment.NeutralKill, Faction.Neutral, "Anybody wants ice scream?", "Douse and freeze anyone who can oppose them."));
            AllRoles.Add(new RoleInfo("Dracula", "Drac", "The Dracula is the only Undead that spawns in. The Dracula is the leader of the Undead who can convert others into Undead. If " +
                "the target cannot be converted, they will be attacked instead. The Dracula must watch out for the Vampire Hunter as attempting to convert them will cause the Vampire " +
                "Hunter to kill the Dracula.", RoleAlignment.NeutralNeo, Faction.Neutral, "Everyone calls me a pain in the neck.", "Convert or kill anyone who can oppose them."));
            AllRoles.Add(new RoleInfo("Executioner", "Exe", "The Executioner has no abilities and instead must use gas-lighting techniques to get their target ejected. The Executioner's" +
                " target, by default, is always non-Crew Sovereign Crew. Once their target is ejected, the Executioner can doom those who voted for their target. If their target dies " +
                "before ejected, the Executioner turns into a Jester. Targets have a grey § next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "Source: trust me bro.",
                "Live to see their target ejected."));
            AllRoles.Add(new RoleInfo("Glitch", "Gli", "The Glitch can hack players, resulting in them being unable to do anything for a set duration or they can also mimic someone, " +
                "which results in them looking exactly like the other person. The Glitch can kill normally.", RoleAlignment.NeutralKill, Faction.Neutral, "Hippity hoppity, your code is " +
                "now my property.", "Hippity hoppity, your code is now my property."));
            AllRoles.Add(new RoleInfo("Guesser", "Guess", "The Guesser has no abilities aside from guessing only their target. Every meeting, the Guesser is told a hint regarding their " +
                "target's role. Targets have a beige π next to their names.", RoleAlignment.NeutralEvil, Faction.Neutral, "I want to know what you are.", "Guess their target's role."));
            AllRoles.Add(new RoleInfo("Jackal", "Jack", "The Jackal is the leader of the Cabal. They spawn in with 2 recruits at the start of the game. One of the recruits is the " +
                "'good' one, meaning they are Crew. The other is the 'evil' recruit, who can be either Intruder, Syndicate or Neutral (Killing). When both recruits die, the Jackal can " +
                "then recruit another player to join the Cabal and become the backup recruit. If the target happens to be a member of a rival subfaction, they will be attacked instead " +
                "and the Jackal will still lose their ability to recruit. Members of the Cabal have a dark grey $ next to their names.", RoleAlignment.NeutralNeo, Faction.Neutral,
                "I've got money.", "Recruit or kill anyone who can oppose them."));
            AllRoles.Add(new RoleInfo("Guardian Angel", "GA", "The Guardian Angel more or less aligns themselves with the faction of their target. The Guardian Angel will win with anyone" +
                " as long as their target lives to the end of the game, even if their target loses. If the Guardian Angel's target dies, they become a Survivor. Targets have a white ★ and " +
                "white η when being protected next to their names.", RoleAlignment.NeutralBen, Faction.Neutral, "Hush child...Mama's here.", "Have their target live to the end of the game."));
            AllRoles.Add(new RoleInfo("Jester", "Jest", "The Jester has no abilities and must make themselves appear to be evil to the Crew and get ejected. After getting ejected, the " +
                "Jester can haunt those who voted for them, killing them from beyond the grave.", RoleAlignment.NeutralEvil, Faction.Neutral, "Hehehe I wonder if I do this...", "Get ejected."));
            AllRoles.Add(new RoleInfo("Juggernaut", "Jugg", "The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill " +
                "cooldown no longer decreases and instead gives them other buffs, like bypassing protections.", RoleAlignment.NeutralKill, Faction.Neutral, "The doctor told me bones grow" +
                " stronger when recieving damage. But then why did he kick me out when I picked up a hammer?", "Assault anyone who can oppose them."));
            AllRoles.Add(new RoleInfo("Revealer", "Rev", "The Revealer can reveal evils if they finish all their tasks. Upon finishing all of their tasks, Intruders, Syndicate and " +
                "sometimes Neutrals are revealed to alive Crew after a meeting is called. However, if the Revealer is clicked they lose their ability to reveal evils and are once again " +
                "a normal ghost.", RoleAlignment.CrewUtil, Faction.Crew, "I have no idea who I am or what I do, the only thing I see is bad guys who I must reveal."));
            AllRoles.Add(new RoleInfo("Murderer", "Murd", "The Murderer is a simple Neutral Killer with no special abilities.", RoleAlignment.NeutralKill, Faction.Neutral, "I like my " +
                "women like how I like my knives, sharp and painful.", "Murder anyone who can oppose them."));
            AllRoles.Add(new RoleInfo("Necromancer", "Necro", "The Necromancer is essentially an evil Altruist. They can revive dead players and make them join the Necromancer's " +
                "team, the Reanimated. There is a limit to how many times can the Necromancer can kill and revive players.", RoleAlignment.NeutralNeo, Faction.Neutral, "I like the dead," +
                " they do a lot of things I like. For example, staying dead.", "Resurrect the dead and kill off anyone who can oppose them."));
            AllRoles.Add(new RoleInfo("Pestilence", "Pest", "Pestilence is always on permanent alert, where anyone who tries to interact with them will die. Pestilence does not spawn " +
                "in-game and instead gets converted from Plaguebearer after they infect everyone. Pestilence cannot die unless they have been voted out, and they can't be guessed " +
                "(usually).", RoleAlignment.NeutralPros, Faction.Neutral, "I am the god of disease, nothing can kill me. *voice from the distance* Ejections can!", "Obliterate anyone " +
                "who can oppose them."));
        }
    }
}