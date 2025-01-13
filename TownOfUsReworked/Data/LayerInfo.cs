namespace TownOfUsReworked.Data;

public static class AllInfo
{
    public static readonly IEnumerable<RoleInfo> AllRoles =
    [
        new("Invalid", "Invalid", "Invalid", Alignment.None, Faction.None, "Invalid", CustomColorManager.Role, LayerEnum.NoneRole, "Invalid"),

        new("Altruist", "Alt", "The Altruist is capable of reviving dead players. After a set period of time, the player will be revived, if the revival isn't interrupted. Once a player is "
            + "revived, all evil players will be notified of the revival and will have an arrow pointing towards the revived player. The Altruist sacrifices themselves on the last use of " +
            "their ability. If the revived player has a Lover and both die together, the Lover is also revived.", Alignment.CrewProt, Faction.Crew, "I know what I have to do but I don't know"
            + " if I have the strength to do it.", CustomColorManager.Altruist, LayerEnum.Altruist),
        new("Bastion", "Bast", "The Bastion can place bombs in vents. Anyone who tries to interact with the bombed vent will die.", Alignment.CrewKill, Faction.Crew, "Hop in there, I dare " +
            "ya.", CustomColorManager.Bastion, LayerEnum.Bastion, "Powerful"),
        new("Chameleon", "Cham", "The Chameleon can go invisible to stalk players and see what they do when no one is around.", Alignment.CrewSupport, Faction.Crew, "Are you sure you" +
            " can see me?", CustomColorManager.Chameleon, LayerEnum.Chameleon),
        new("Coroner", "Cor", "The Coroner gets an alert when someone dies and briefly gets an arrow pointing in the direction of the body. They can autopsy bodies to get some information. "
            + "The Coroner can then compare that information with players to see if they killed the body or not. They also gets body reports from the bodies they report. The report will " +
            "include the cause and time of death, the body's faction/role, the killer's faction/role and (according to the settings) the killer's name. The Coroner will not get any " +
            "information if they report a body when blinded by a Grenadier and framed players will always appear to be the killer.", Alignment.CrewInvest, Faction.Crew, "A body? Where? I " +
            "need it for...scientific purposes.", CustomColorManager.Coroner, LayerEnum.Coroner),
        new("Crewmate", "Crew", "Just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.", Alignment.CrewUtil, Faction.Crew,
            "I once made a pencil using 2 erasers...they were pointless, just like me.", CustomColorManager.Crew, LayerEnum.Crewmate),
        new("Detective", "Det", "The Detective can examine other players for bloody hands and footprints. If the examined player has killed recently, the Detective will be alerted about it. "
            + "Once examined, the player's footprints will be visible to the Detective. Framed players will always appear to have bloody hands.", Alignment.CrewInvest, Faction.Crew, "I am " +
            "skilled in identifying blood...yup that's not ketchup.", CustomColorManager.Detective, LayerEnum.Detective),
        new("Dictator", "Dict", "The Dictator has no active ability aside from revealing themselves as the Dictator to all players. When revealed, in the next meeting they can pick up to 3 "
            + "players to be ejected. All 3 players will be killed at the end of the meeting, along with the chosen 4th player everyone else votes on (if any). If any of the 3 killed " +
            "players happens to be Crew, the Dictator dies with them. The Dictator has no post ejection ability. A revealed Dictator cannot be protected, but can be revived.",
            Alignment.CrewSov, Faction.Crew, "Out you go!", CustomColorManager.Dictator, LayerEnum.Dictator),
        new("Engineer", "Engi", "The Engineer can fix sabotages from anywhere on the map and use vents to get across the map easily.", Alignment.CrewSupport, Faction.Crew, "How am I going " +
            "to stop some big mean mother hubbard from tearing me a structurally superfluous new behind? The solution? Use a wrench. And if that don't work, use more wrench.",
            CustomColorManager.Engineer, LayerEnum.Engineer),
        new("Escort", "Esc", "The Escort can roleblock players and prevent them from doing anything for a short while.", Alignment.CrewSupport, Faction.Crew, "Today, I will make you a "
            + "man.", CustomColorManager.Escort, LayerEnum.Escort),
        new("Mayor", "Mayo (XD)", "The Mayor can reveal themselves as the Mayor to other players. Upon doing so, the value of their vote increases. A revealed Mayor cannot be protected, " +
            "but can be revived.", Alignment.CrewSov, Faction.Crew, "Um, those votes are legitimate. No, the votes are not rigged.", CustomColorManager.Mayor, LayerEnum.Mayor),
        new("Medic", "Medic", "The Medic can give any player a shield that will grant them Powerful defense. The Medic can choose who they give thier shield to, and can take their shield " +
            "back if needed. Shielded players have a green ✚ next to their names.", Alignment.CrewProt, Faction.Crew, "Where does it hurt?", CustomColorManager.Medic, LayerEnum.Medic),
        new("Medium", "Med", "The Medium can mediate to be able to see ghosts. If the Medium uses this ability, the Medium and the dead player will be able to see each other and communicate "
            + "with actions from beyond the grave!", Alignment.CrewInvest, Faction.Crew, "The voices...they are telling me...my breath stinks? Can ghosts even smell?",
            CustomColorManager.Medium, LayerEnum.Medium),
        new("Monarch", "Mon", "The Monarch can appoint players as knights. When the next meeting is called, all knighted players will be announced. Knighted players will have the value of " +
            "their votes increased. For as long as a Knight is alive, the Monarch gets basic defense. Knighted players have a pinkish red κ next to their names. The Monarch is alerted when "
            + "a knight dies.", Alignment.CrewSov, Faction.Crew, "Doth thou solemnly swear your allegiance to the lord?", CustomColorManager.Monarch, LayerEnum.Monarch, "None", "Basic with "
            + "an alive knight"),
        new("Mystic", "Mys", "The Mystic only spawns when there is at least one Neutral (Neophyte) role present in the game. Whenever someone's subfaction is changed, the Mystic will be " +
            "alerted about it. The Mystic can also investigate players to see if their subfactions have been changed. If the target belongs to a subfaction, or that the target's subfaction "
            + "is different from the Mystic's (in the case of a converted Mystic), the Mystic's screen will flash red, otherwise it will flash green. It will not, however, work on the " +
            "Neutral (Neophyte) roles themselves so even people who flashed green might still be evil. Once all subfactions are dead, the Mystic becomes a Seer. Framed players will appear " +
            "to have their subfactions changed.", Alignment.CrewAudit, Faction.Crew, "There's a hint of corruption.", CustomColorManager.Mystic, LayerEnum.Mystic),
        new("Operative", "Op", "The Operative can place bugs around the map. When players enter the range of the bug and stay within it for a certain amount of time, they trigger it. In " +
            "the following meeting, all players who triggered a bug will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the"
            + "bug, nor what role a specific player is. The Operative also gains more information when on Admin Table and on Vitals. On Admin Table, the Operative can see the colors of " +
            "every person on the map. When on Vitals, the Operative is shown how long someone has been dead for.", Alignment.CrewInvest, Faction.Crew, "The only thing you need to find"
            + " out information is good placement and amazing levels of patience.", CustomColorManager.Operative, LayerEnum.Operative),
        new("Retributionist", "Ret", "The Retributionist can mimic dead crewamtes. During meetings, the Retributionist can select who they are going to mimic for the following round from" +
            " the dead. It should be noted the Retributionist can not use all Crew roles and cannot use any Non-Crew role. The cooldowns, limits and everything will be set by the settings"
            + " for their respective roles.", Alignment.CrewSupport, Faction.Crew, "Bodies...bodies...I NEED BODIES.", CustomColorManager.Retributionist, LayerEnum.Retributionist),
        new("Revealer", "Rev", "The Revealer is the first dead Crew. Upon finishing all their tasks, the evils, and possibly their roles, will be revealed to all other alive players. "
            + "However, if the Revealer is clicked they lose their ability to reveal evils and are once again a normal ghost.", Alignment.CrewUtil, Faction.Crew, "I have no idea "
            + "who I am or what I do, the only thing I know is to finish my job.", CustomColorManager.Revealer, LayerEnum.Revealer),
        new("Seer", "Seer", "The Seer only spawns if there are roles capable of changing their initial roles or if there's a Traitor or Fanatic in the game. The Seer can investigate players "
            + "to see if their role is different from what they started out as. If a player's role has changed, the Seer's screen will flash red, otherwise it will flash green. If all " +
            "players capable of changing or have changed their initial roles are dead, the Seer becomes a Sheriff. Framed players will appear to have their role changed.",
            Alignment.CrewInvest, Faction.Crew, "You've got quite the history.", CustomColorManager.Seer, LayerEnum.Seer),
        new("Sheriff", "Sher", "The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a player is Good or Evil. The Sheriff's screen will"
            + " flash green or red depending on the results and settings. Framed players will appear to be evil. Promoted Godfathers and Rebels will not appear to be evil.",
            Alignment.CrewInvest, Faction.Crew, "Guys I promise I'm not an Executioner, I checked Blue and they're sus.", CustomColorManager.Sheriff, LayerEnum.Sheriff),
        new("Shifter", "Shift", "The Shifter can swap roles with someone, as long as they are a valid Crew. If the shift is unsuccessful or the target is framed, the Shifter dies.",
            Alignment.CrewSupport, Faction.Crew, "GET BACK HERE I WANT YOUR ROLE.", CustomColorManager.Shifter, LayerEnum.Shifter),
        new("Tracker", "Track", "InvaThe Tracker can track others during a round. Once they track someone, an arrow is continuously pointing to them, which updates in set intervals.",
            Alignment.CrewInvest, Faction.Crew, "I only took up this job because the others were full. Yes it's a proper job. No, I'm not a stalker.", CustomColorManager.Tracker,
            LayerEnum.Tracker),
        new("Transporter", "Trans", "The Transporter can swap the locations of two players at will. Being transported plays an animation that's visible to all players and renderers the" +
            " targets immobile for the duration of the transportation. During the transportation, they can be targeted by anyone, even those of their own team. This means that the " +
            "Transporter is capable of making evils attack each other.", Alignment.CrewSupport, Faction.Crew, "You're here and you're there. Where will you go? That's for me to decide."
            + ".", CustomColorManager.Transporter, LayerEnum.Transporter),
        new("Trapper", "Trap", "The Trapper can build traps and place them on other players. These traps will either register an interacter's role, or attack an attacker. At the start of the"
            + " next meeting, the Trapper is told whether they attacked someone or not. If not, the Trapper gets a list of roles that interacted with the trapped targets in a random order. "
            + "Trapped players have a pink ∮ next to their names.", Alignment.CrewProt, Faction.Crew, "Just step over there kind sir.", CustomColorManager.Trapper, LayerEnum.Trapper,
            "Powerful"),
        new("Vampire Hunter", "VH", "The Vampire Hunter only spawns if there are Undead in the game. They can check players to see if they are an Undead. When the Vampire Hunter finds them,"
            + " the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a Vigilante. Interacting with a Vampire "
            + "Hunter as an Undead will force the Vampire Hunter to kill you. Framed players will behave like ann Undead to the Vampire Hunter. The Vampire Hunter knows if they target they "
            + "actually killed is an Undead or not.", Alignment.CrewAudit, Faction.Crew, "The Dracula could be any one of us! He could be you! He could be me! He could even be- <i>gets " +
            "voted off</i>", CustomColorManager.VampireHunter, LayerEnum.VampireHunter, "Unstoppable against Undead only", "Invincible against Undead only"),
        new("Veteran", "Vet", "The Veteran can go on alert. Anyone who interacts with a Veteran on alert will be killed by the Veteran in question.", Alignment.CrewKill, Faction.Crew,
            "Touch me, I dare you.", CustomColorManager.Veteran, LayerEnum.Veteran, "Powerful", "Basic when on alert"),
        new("Vigilante", "Vig", "The Vigilante can kill. However, if they kill someone they shouldn't, they instead kill themselves. Framed players are guaranteed to die without the risk of "
            + "the Vigilante misfiring.", Alignment.CrewKill, Faction.Crew, "I AM THE HAND OF JUSTICE.", CustomColorManager.Vigilante, LayerEnum.Vigilante, "Basic", footer: true),

        new("Actor", "Act", "The Actor gets a list of roles at the start of the game. Alternatively, the Actor can pick a player to add their role to the role list and the rest of the role "
            + "list is filtered and filled. This list of roles depends on which roles are present in the game. The Actor must pretend to be and get guessed as one of the roles in order to " +
            "win.", Alignment.NeutralEvil, Faction.Neutral, "I am totally what you think of me as.", CustomColorManager.Actor, LayerEnum.Actor, "Unstoppable", "None", "Get guessed as a role "
            + "in their target role list"),
        new("Amnesiac", "Amne", "The Amnesiac is essentially roleless and cannot win without remembering the role of a dead player. When there is only 4 players left, the Amnesiac becomes" +
            "a Thief.", Alignment.NeutralBen, Faction.Neutral, "I forgor :skull:", CustomColorManager.Amnesiac, LayerEnum.Amnesiac, "None", "None", "Find a dead body, take their role and then"
            + " win as that role."),
        new("Arsonist", "Arso", "The Arsonist can douse players with gasoline. After dousing, the Arsonist can choose to ignite all doused players which kills all doused players at once. " +
            "Doused players have an orange Ξ next to their names. Upon igniting, the Arsonist may cremate the body, leaving behind unreportable ash that is visible to all players. The " +
            "Arsonist has basic defense when they have at most 2 players doused.", Alignment.NeutralKill, Faction.Neutral, "I like my meat well done.", CustomColorManager.Arsonist,
            LayerEnum.Arsonist, "Unstoppable", "Basic when up to 2 players are doused", "Douse and ignite anyone who can oppose them"),
        new("Betrayer", "Bet", "The Betrayer is a simple killer, who appears after a turned Traitor/Fanatic was the only member of their new faction remaning. This role does not spawn"
            + " directly.", Alignment.NeutralPros, Faction.Neutral, "The back that trusts me the most is the sweetest to stab", CustomColorManager.Betrayer, LayerEnum.Betrayer, "Basic",
            "None", "Kill anyone who opposes the faction they defected to"),
        new("Bounty Hunter", "BH", "The Bounty Hunter is assigned a target as the start of the game. Alternatively, the Bounty Hunter can request a target from someone. That someone must " +
            "pick a target with the next two rounds, or else they will become the Bounty Hunter's target instead. Every meeting, the Bounty Hunter is given clue to who their target might be."
            + " They do not know who the target is and must find them via a series of clues and limited guesses. Upon finding their target within the set amount of guesses, the guess button"
            + " becomes a kill button. The Bounty Hunter's target always knows that there is a bounty on their head. If the Bounty Hunter is unable to find their target within the number of "
            + "guesses or their target dies not by the Bounty Hunter's hands, the Bounty Hunter becomes a Troll. The target has a red Θ next to their names.", Alignment.NeutralEvil,
            Faction.Neutral, "You can run, but you can't hide.", CustomColorManager.BountyHunter, LayerEnum.BountyHunter, "Unstoppable", "None", "Find and kill their bounty"),
        new("Cannibal", "Cann", "The Cannibal can eat the body which wipes away the body, like the Janitor.", Alignment.NeutralEvil, Faction.Neutral, "How do you survive with no " +
            "food but with a lot of people? Improvise, adapt, overcome.", CustomColorManager.Cannibal, LayerEnum.Cannibal, "None", "None", "Eat a certain number of bodies"),
        new("Cryomaniac", "Cryo", "The Cryomaniac can douse players in coolant and freeze them similar to the Arsonist's dousing in gasoline and ignite. Freezing players does not immediately"
            + " kill doused targets, instead when the next meeting is called, all currently doused players will die. When the Cryomaniac is the last killer or when the final number of " +
            "players reaches a certain threshold, the Cryomaniac may also directly kill. Doused players have a purple λ next to their names. The Cryomaniac has basic defense when they have "
            + "at most 2 players doused.", Alignment.NeutralKill, Faction.Neutral, "Anybody wants ice scream?", CustomColorManager.Cryomaniac, LayerEnum.Cryomaniac, "Unstoppable", "Basic " +
            "when up to 2 players are doused", "Douse and freeze anyone who can oppose them"),
        new("Dracula", "Drac", "The Dracula is the only Undead that spawns in. The Dracula is the leader of the Undead who can convert others into an Undead. If the target cannot be " +
            "converted, they will be attacked instead. The Dracula must watch out for the Vampire Hunter as attempting to convert them will cause the Vampire Hunter to kill the " +
            "Dracula. Members of the Undead have a grey γ next to their names.", Alignment.NeutralNeo, Faction.Neutral, "Everyone calls me a pain in the neck.", CustomColorManager.Dracula,
            LayerEnum.Dracula, "Basic", "None", "Convert or kill anyone who can oppose the Undead"),
        new("Executioner", "Exe", "The Executioner has no abilities and instead must use gas-lighting techniques to get their target ejected. The Executioner's target, by default, is always "
            + "non-CSv Crew. Alternatively, the Executioner can pick their own target to try and eject. Once their target is ejected, the Executioner can doom those who voted for their " +
            "target. If their target dies before getting ejected, the Executioner turns into a Jester. Targets have a grey § next to their names.", Alignment.NeutralEvil, Faction.Neutral,
            "Source: trust me bro.", CustomColorManager.Executioner, LayerEnum.Executioner, "Unstoppable", "None", "Have their target ejected by any means"),
        new("Glitch", "Gli", "The Glitch can mimic someone, which results in them looking exactly like the other person. They can kill normally. The Glitch can also hack a player, rendering "
            + "them unable to use their ability or perform tasks and disabling their defense while gaining Powerful defense in return.", Alignment.NeutralKill, Faction.Neutral, "Hippity " +
            "hoppity, your code is now my property.", CustomColorManager.Glitch, LayerEnum.Glitch, "Basic", "Powerful when hacking", "Neutralise anyone who can oppose them"),
        new("Guardian Angel", "GA", "The Guardian Angel more or less aligns themselves with the faction of their target. Alternatively, the Guardian Angel can pick their own target to " +
            "protect. The Guardian Angel will win with anyone as long as their target lives to the end of the game, even if their target loses. If the Guardian Angel's target dies, they " +
            "become a Survivor. Targets have a white ★ and a white η when protected next to their names. The Guardian Angel provides Powerful defense to their target when protecting.",
            Alignment.NeutralBen, Faction.Neutral, "Hush child...Mama's here.", UColor.white, LayerEnum.GuardianAngel, "None", "None", "Have their target live to the end of the game"),
        new("Guesser", "Guess", "The Guesser has no abilities aside from guessing only their target. Every meeting, the Guesser is told a hint regarding their target's role. If the " +
            "target dies not by the Gusser's hands, the Guesser becomes an Actor with the target role list that of their target's role. Upon guessing their target, the Guesser can " +
            "freely guess anyone. Targets have a beige π next to their names.", Alignment.NeutralEvil, Faction.Neutral, "I want to know what you are.", CustomColorManager.Guesser,
            LayerEnum.Guesser, "Unstoppable", "None", "Guess their target's role"),
        new("Jackal", "Jack", "The Jackal is the leader of the Cabal. They spawn in with 2 recruits at the start of the game. One of the recruits is the 'good' one, meaning they are " +
            "Crew or Neutral (Benign). The other is the 'evil' recruit, who can be either Intruder, Syndicate or Neutral (Killing) or (Harbinger). When both recruits die, the Jackal " +
            "can then recruit another player to join the Cabal and become the backup recruit. If the target happens to be a member of a rival subfaction, they will be attacked instead"
            + " and the Jackal will still lose their ability to recruit. Members of the Cabal have a dark grey $ next to their names.", Alignment.NeutralNeo, Faction.Neutral, "I've"
            + " got money.", CustomColorManager.Jackal, LayerEnum.Jackal, "Basic", "None", "Recruit or kill anyone who can oppose Cabal"),
        new("Jester", "Jest", "The Jester has no abilities and must make themselves appear to be evil to the Crew and get ejected. After getting ejected, the Jester can haunt those " +
            "who voted for them, killing them from beyond the grave.", Alignment.NeutralEvil, Faction.Neutral, "Hehehe I wonder if I do this...", CustomColorManager.Jester, LayerEnum.Jester,
            "Unstoppable", "None", "Get ejected"),
        new("Juggernaut", "Jugg", "The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill cooldown no longer decreases and " +
            "instead gives them other buffs, like bypassing protections. The Juggernaut's kill power increases with each kill and caps at Unstoppable.", Alignment.NeutralKill,
            Faction.Neutral, "The doctor told me bones grow stronger when recieving damage. But then why did he kick me out when I picked up a hammer?", CustomColorManager.Juggernaut,
            LayerEnum.Juggernaut, "Starts with Basic, Ends with Unstoppable", "Basic with at least 3 kills", "Assault anyone who can oppose them"),
        new("Murderer", "Murd", "The Murderer is a simple Neutral Killer with no special abilities. The only role that has both passive defense and attack.", Alignment.NeutralKill,
            Faction.Neutral, "I like my women like how I like my knives, sharp and painful.", CustomColorManager.Murderer, LayerEnum.Murderer, "Basic", "Basic", "Murder anyone who can " +
            "oppose them"),
        new("Necromancer", "Necro", "The Necromancer is essentially an evil Altruist. They can resurrect dead players and make them join the Necromancer's team, the Reanimated. There is a " +
            "limit to how many times can the Necromancer can kill and resurrect players. Reviving someone with a Lover when both lovers die will revive that Lover too. The Necromancer " +
            "cannot revive the same person twice. Members of the Reanimated have a dark pink Σ next to their names.", Alignment.NeutralNeo, Faction.Neutral, "I like the dead, they do a lot "
            + "of things I like. For example, staying dead.", CustomColorManager.Necromancer, LayerEnum.Necromancer, "Basic", "None", "Resurrect the dead and kill off anyone who can oppose the"
            + " Reanimated"),
        new("Pestilence", "Pest", "When Pestilence first emerges, every player is given one stack of a deadly plague. For every time a player interacts with someone, they will give their " +
            "target a stack of the deadly plague. However, targetting Pestilence will give the interactor a stack instead. If a player gets 3 or more stacks, they succumb to the disease. " +
            "Pestilence can spread the disease directly to a player, overriding their stacks and directly giving their target 4 stacks, kill them instantly. Stacks cannot be spread to " +
            "Neutral (Harbinger) or (Apocalypse) roles. Stacks on players are denoted by a dark grey 米 in their names.", Alignment.NeutralApoc, Faction.Neutral, "I am the god of disease, " +
            "nothing can kill me. <i>Voice from the distance</i> Ejections can!", CustomColorManager.Pestilence, LayerEnum.Pestilence, "Unstoppable", "Invincible", "Obliterate anyone who " +
            "can oppose them"),
        new("Phantom", "Phan", "The Phantom spawns when a Neutral player dies without accomplishing their objective. They become half-invisible and have to complete all their tasks without" +
            " getting clicked on to win.", Alignment.NeutralPros, Faction.Neutral, "I'm the one who you should not have killed. <i>Voice from the distance</i> Get outta here! This is not " +
            "FNAF!", CustomColorManager.Phantom, LayerEnum.Phantom, "None", "None", "Finish tasks without getting caught"),
        new("Plaguebearer", "PB", "The Plaguebearer can infect other players. Once infected, the infected player can go and infect other players via interacting with them. Once all " +
            "players are infected, the Plaguebearer becomes Pestilence. Infected players have a pale lime ρ next to their names.", Alignment.NeutralHarb, Faction.Neutral,
            "<i>Cough</i> This should surely work, right? <i>Cough</i> I sure hope it does.", CustomColorManager.Plaguebearer, LayerEnum.Plaguebearer, "None", "Basic for as long as half " +
            "the players are infected", "Infect everyone to become Pestilence or kill off anyone who can oppose them"),
        new("Serial Killer", "SK", "Although the Serial Killer has a kill button, they can't use it unless they are in Bloodlust. Once the Serial Killer is in bloodlust they gain the ability"
            + " to kill. However, unlike most killers, their kill cooldown is really short for the duration of the bloodlust. The Serial Killer gets Basic defense when in bloodlust.",
            Alignment.NeutralKill, Faction.Neutral, "I always come back.", CustomColorManager.SerialKiller, LayerEnum.SerialKiller, "Powerful", "Basic when in bloodlust", "Stab anyone who " +
            "can oppose them"),
        new("Survivor", "Surv", "The Survivor wins by simply surviving. They can vest which makes them immortal for a short duration. Vesting Survivors have a yellow υ next to their " +
            "names.", Alignment.NeutralBen, Faction.Neutral, "Hey listen man, I mind my own business and you mind yours. Everyone wins!", CustomColorManager.Survivor, LayerEnum.Survivor,
            "None", "Basic when vesting", "Live to the end of the game"),
        new("Thief", "Thief", "The Thief can kill players to steal their roles. The player, however, must be a role with the ability to kill otherwise the Thief will die. After " +
            "stealing their target's role, the Thief can now win as whatever role they have become. The Thief can also guess players in-meeting to steal their roles.",
            Alignment.NeutralBen, Faction.Neutral, "Now it's mine.", CustomColorManager.Thief, LayerEnum.Thief, "Powerful", "Basic", "Kill and steal someone's role"),
        new("Troll", "Troll", "The Troll just wants to be killed, but not ejected. The Troll can \"interact\" with players. This interaction does nothing, it just triggers any " +
            "interaction sensitive roles like Veteran and Pestilence. Killing the Troll makes the Troll kill their killer.", Alignment.NeutralEvil, Faction.Neutral, "Kill me. " +
            "Impostor: Later.", CustomColorManager.Troll, LayerEnum.Troll, "Unstoppable", "None", "Get killed"),
        new("Werewolf", "WW", "The Werewolf can kill all players within a certain radius. The Werewolf cannot attack on the first and third rounds, but gets Basic defense in exchange.",
            Alignment.NeutralKill, Faction.Neutral, "AWOOOOOOOOOOOOOOOOOOOO", CustomColorManager.Werewolf, LayerEnum.Werewolf, "Powerful", "Basic when not attacking", "Maul anyone who can " +
            "oppose them"),
        new("Whisperer", "Whisp", "The Whisperer can whisper to all players within a certain radius. With each whisper, the chances of bringing someone over to the Sect increases " +
            "till they do convert. Members of the Sect have a pink Λ next to their names.", Alignment.NeutralNeo, Faction.Neutral, "PSST.", CustomColorManager.Whisperer, LayerEnum.Whisperer,
            "Basic", "None", "Persuade or kill anyone who can oppose the Sect", true),

        new("Ambusher", "Amb", "The Ambusher can temporarily force anyone to go on alert, killing anyone who interacts with the Ambusher's target. Attacks via ambush deal a Basic attack to "
            + "the interactor. Ambushers also give their targets Powerful defense.", Alignment.IntruderKill, Faction.Intruder, "BOO", CustomColorManager.Ambusher, LayerEnum.Ambusher, "Basic"),
        new("Blackmailer", "BM", "The Blackmailer can blackmail people. Blackmailed players cannot speak during the next meeting and have a green Φ next to their names.",
            Alignment.IntruderConceal, Faction.Intruder, "Shush.", CustomColorManager.Blackmailer, LayerEnum.Blackmailer),
        new("Camouflager", "Camo", "The Camouflager can camouflage all players at once, turning them all grey and anonymous.", Alignment.IntruderConceal, Faction.Intruder, "Good luck " +
            "telling me apart.", CustomColorManager.Camouflager, LayerEnum.Camouflager),
        new("Consigliere", "Consig", "The Consigliere can reveal people's roles. They cannot guess those they revealed for obvious reasons.", Alignment.IntruderSupport,
            Faction.Intruder, "What are you?", CustomColorManager.Consigliere, LayerEnum.Consigliere),
        new("Consort", "Cons", "The Consort can roleblock players and prevent them from doing anything for a short while. They behave just like an Escort but the Consort can roleblock "
            + "from any range.", Alignment.IntruderSupport, Faction.Intruder, "I'm like the first slice of bread, everyone touches me but no one likes me.", CustomColorManager.Consort,
            LayerEnum.Consort),
        new("Disguiser", "Disg", "The Disguiser can disguise other players. At the beginning of each, they can choose someone to measure. They can then disguise the next nearest " +
            "person into the measured person for a limited amount of time after a short delay.", Alignment.IntruderDecep, Faction.Intruder, "Here, wear this for me please. I " +
            "promise I won't do anything to you.", CustomColorManager.Disguiser, LayerEnum.Disguiser),
        new("Enforcer", "Enf", "The Enforcer can plant bombs on players. After a short while, their target will be alerted to the bomb's presence and must kill someone to get rid of it. If "
            + "they fail to do so within a certain time limit, the bomb will explode, killing everyone within its vicinity. Bombs explosions deal Powerful attacks to all victims and a " +
            "bombed player will deal a Basic attack to their target.", Alignment.IntruderKill, Faction.Intruder, "You will do as I say...unless you want to be the painting on the walls.",
            CustomColorManager.Enforcer, LayerEnum.Enforcer, "Powerful via bomb/Basic via indirect kill"),
        new("Ghoul", "Ghoul", "The Ghoul is the first dead Intruder. Every round, the Ghoul can mark a player for death. All players are told who is marked and the marked player will "
            + "die at the end of the next meeting. The only way to save a marked player is to click the Ghoul that marked them. Marked players have a yellow χ next to their names.",
            Alignment.IntruderUtil, Faction.Intruder, "I CURSE YOU!", CustomColorManager.Ghoul, LayerEnum.Ghoul),
        new("Godfather", "GF", "The Godfather can only spawn in 3+ Intruder games. They can choose to promote a fellow Intruder to Mafioso. When the Godfather dies, the Mafioso " +
            "becomes the new Godfather and has lowered cooldowns.", Alignment.IntruderHead, Faction.Intruder, "I'm going to make an offer they can't refuse.", CustomColorManager.Godfather,
            LayerEnum.Godfather),
        new("Grenadier", "Gren", "The Grenadier can throw flash grenades which blinds nearby players. However, a sabotage and a flash grenade can not be active at the same time.",
            Alignment.IntruderConceal, Faction.Intruder, "AAAAAAAAAAAAA YOUR EYES.", CustomColorManager.Grenadier, LayerEnum.Grenadier),
        new("Impostor", "Imp", "Just a plain Intruder with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.", Alignment.IntruderUtil,
            Faction.Intruder, "If you ever feel useless, just remember I exist.", CustomColorManager.Intruder, LayerEnum.Impostor),
        new("Janitor", "Jani", "The Janitor can drag, drop and clean up bodies. Both their Kill and Clean usually ability have a shared cooldown, meaning they have to choose which one"
            + " they want to use.", Alignment.IntruderConceal, Faction.Intruder, "I'm the guy you call to clean up after you.", CustomColorManager.Janitor, LayerEnum.Janitor),
        new("Mafioso", "Mafi", "The Mafioso is promoted from a non-Godfather Intruder role. The Mafioso by themself is nothing special, but when the Godfather dies, the Mafioso becomes the "
            + "new Godfather. As a result, the new Godfather has a lower cooldown on all of their original role's abilities.", Alignment.IntruderUtil, Faction.Intruder, "Yes, boss. Got it," +
            " boss.", CustomColorManager.Mafioso, LayerEnum.Mafioso),
        new("Miner", "Miner", "The Miner can create new vents. These vents only connect to each other, forming a new passageway.", Alignment.IntruderSupport, Faction.Intruder,
            "Dig, dig, diggin' some rave; making some loud sound waves; the only thing you'll be diggin' is your own grave.", CustomColorManager.Miner, LayerEnum.Miner),
        new("Morphling", "Morph", "The Morphling can morph into another player. During the round, they can choose someone to sample. They can then morph into the sampled person at any"
            + " time for a limited amount of time.", Alignment.IntruderDecep, Faction.Intruder, "<i>Casually observing the chaos over Green seeing Red kill.</i> It was me.",
            CustomColorManager.Morphling, LayerEnum.Morphling),
        new("Teleporter", "Tele", "The Teleporter can mark a location which they can then teleport to later.", Alignment.IntruderSupport, Faction.Intruder, "He's here, he's there, "
            + "he's everywhere. Who are ya gonna call? Psychic friend Fr-", CustomColorManager.Teleporter, LayerEnum.Teleporter),
        new("Wraith", "Wraith", "The Wraith can temporarily turn invisible.", Alignment.IntruderDecep, Faction.Intruder, "Now you see me, now you don't.", CustomColorManager.Wraith,
            LayerEnum.Wraith, footer: true),

        new("Anarchist", "Anarch", "Just a plain Syndicate with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode. Its only benefit is its " +
            "ability to kill from the beginning of the game. With the Chaos Drive, the Anarchist's kill cooldown decreases.", Alignment.SyndicateUtil, Faction.Syndicate, "If you " +
            "ever feel useless, just remember that I also exist.", CustomColorManager.Syndicate, LayerEnum.Anarchist, "Basic", "None", ""),
        new("Banshee", "Bansh", "The Banshee is the first dead Syndicate. The Banshee can block every non-Syndicate player every once in a while. This role cannot get the Chaos Drive.",
            Alignment.SyndicateUtil, Faction.Syndicate, "AAAAAAAAAAAAAAA", CustomColorManager.Banshee, LayerEnum.Banshee),
        new("Bomber", "Bomb", "The Bomber can place a bomb which can be remotely detonated at any time. Anyone caught inside the bomb's radius at the time of detonation will be killed. Only "
            + "the latest placed bomb will detonate, unless the Bomber holds the Chaos Drive, with which they can detonate all bombs at once. Bomb detonation deals a Powerful attack to " +
            "victims.", Alignment.SyndicateKill, Faction.Syndicate, "KABOOM!!", CustomColorManager.Bomber, LayerEnum.Bomber, "Powerful"),
        new("Concealer", "Conceal", "The Concealer can make a player invisible for a short while. With the Chaos Drive, this applies to everyone.", Alignment.SyndicateDisrup,
            Faction.Syndicate, "HAHAHA YOU CAN'T SEE VERY WELL CAN YOU NOW?", CustomColorManager.Concealer, LayerEnum.Concealer),
        new("Crusader", "Crus", "The Crusader can temporaily force anyone to go on alert, killing anyone who interacts with the Crusader's target. With the Chaos Drive, attempting to " +
            "interact with the Crusader's target will cause the target to kill everyone within a certain range, including the target themselves. Crusades give the target Powerful defense " +
            "and attack interactors with a Basic attack. A Radial crusade attack via the Chaos Drive deals a Powerful attack to all victims.", Alignment.SyndicateKill, Faction.Syndicate,
            "I WILL PURGE THIS UNHOLY LAND!", CustomColorManager.Crusader, LayerEnum.Crusader, "Powerful via radial crusade/Basic via indirect attack"),
        new("Collider", "Col", "The Collider can mark players as positive and negative. If these charged players come within a certain distance of each other, they will die together. With " +
            "the Chaos Drive, the Collider can charge themselves to collide with the other charged players. This only kills the charged victim. The range of collision also increases with " +
            "the Chaos Drive. Collisions deal a Powerful attack to charged targets.", Alignment.SyndicateKill, Faction.Syndicate, "I'm a great matchmaker, trust me.",
            CustomColorManager.Collider, LayerEnum.Collider, "Powerful"),
        new("Drunkard", "Drunk", "The Drunkard can reverse a player's controls. With the Chaos Drive, this effect applies to everyone.", Alignment.SyndicateDisrup, Faction.Syndicate,
            "<i>Burp</i>", CustomColorManager.Drunkard, LayerEnum.Drunkard),
        new("Framer", "Framer", "The Framer can frame players, making them appear to be evil or have wrong results. This effects lasts as long as the Framer is alive. With the Chaos Drive, "
            + "the Framer can frame players within a certain radius. Framed will have their results altered to investigative roles and allows them to die to killers like Vigilante and " +
            "Assassin easily. and have a cyan ς next to their names", Alignment.SyndicateDisrup, Faction.Syndicate, "Who knew old documents can get people into trouble?",
            CustomColorManager.Framer, LayerEnum.Framer),
        new("Poisoner", "Pois", "The Poisoner can poison a player instead of killing them. When they poison a player, the poisoned player dies either upon the start of the next meeting or " +
            "after a set duration. With the Chaos Drive, the Poisoner can poison a player from anywhere. Poisoned players are dealt a Basic attack astrally.", Alignment.SyndicateKill,
            Faction.Syndicate, "So now if you mix these together, you end up creating this...thing.", CustomColorManager.Poisoner, LayerEnum.Poisoner, "Basic"),
        new("Rebel", "Reb", "The Rebel can only spawn in 3+ Syndicate games. They can choose to promote a fellow Syndicate to Sidekick. When the Rebel dies, the Sidekick becomes " +
            "the new Rebel and has lowered cooldowns. With the Chaos Drive, the Rebel's gains the improved abilities of their former role.", Alignment.SyndicatePower, Faction.Syndicate,
            "DOWN WITH THE GOVERNMENT!", CustomColorManager.Rebel, LayerEnum.Rebel),
        new("Shapeshifter", "SS", "The Shapeshifter can swap the appearances of 2 players. With the Chaos Drive, everyone's appearances are suffled.", Alignment.SyndicateDisrup,
            Faction.Syndicate, "Everyone! We will be playing dress up! TOGETHER!", CustomColorManager.Shapeshifter, LayerEnum.Shapeshifter),
        new("Sidekick", "Side", "The Sidekick is promoted from a non-Rebel Syndicate role. The Sidekick by themselves is nothing special, but when the Rebel dies, the Sidekick becomes the " +
            "new Rebel. As a result, the new Rebel has a lower cooldown on all of their original role's abilities.", Alignment.SyndicateUtil, Faction.Syndicate, "Learning new things.",
            CustomColorManager.Sidekick, LayerEnum.Sidekick, "Basic"),
        new("Silencer", "Sil", "The Silencer can silencer people. Silenced plaeyrs cannot see the messages being sent by others but can still talk. Other players can still talk but " +
            "can't get their info through to the silenced player. With the Chaos Drive, silence prevents everyone except for the silenced player from talking. Silencer players have a pale " +
            " green 乂 next to their names.", Alignment.SyndicateDisrup, Faction.Syndicate, "QUIET.", CustomColorManager.Silencer, LayerEnum.Silencer),
        new("Stalker", "Stalk", "The Stalker is a buffed Tracker with no update interval. With the Chaos Drive, the arrows are no longer affected by camouflages and all players " +
            "instantly have an arrow pointing at them.", Alignment.SyndicateSupport, Faction.Syndicate, "I'll follow you.", CustomColorManager.Stalker, LayerEnum.Stalker),
        new("Spellslinger", "Spell", "The Spellslinger is a powerful role who can cast curses on players. When all non-Syndicate players are cursed, the game ends in a Syndicate " +
            "victory. With each spell cast, the spellbind cooldown increases. This effect is negated by the Chaos Drive. Spelled players have a blue ø next to their names during a meeting.",
            Alignment.SyndicatePower, Faction.Syndicate, "I CURSE YOU TO SUCK ONE THOUSAND D-", CustomColorManager.Spellslinger, LayerEnum.Spellslinger),
        new("Timekeeper", "TK", "The Timekeeper can control time. Without the Chaos Drive, the Timekeeper can freeze time, making everyone unable to move and with it, the Timekeeper" +
            " rewinds players instead.", Alignment.SyndicateDisrup, Faction.Syndicate, "IT'S TIME TO STOP. NO MORE.", CustomColorManager.Timekeeper, LayerEnum.Timekeeper),
        new("Warper", "Warp", "The Warper can teleport a player to another player. With the Chaos Drive, the Warper teleports everyone to random positions on the map. Warping a player "
            + "makes them unable to move and play an animation. During warping, they can be targeted by anyone, opening up the possibility of team killing.", Alignment.SyndicateSupport,
            Faction.Syndicate, "BEGONE!", CustomColorManager.Warper, LayerEnum.Warper, footer: true),

        new("Hunter", "Hunter", "The Hunter is one of the two roles that spawn in this mod's version of Hide And Seek. The Hunter can kill Hunted, or convert them to Hunters. The Hunters " +
            "also have a higher run speed than Hunters but can barely see in exchange. All attempts at killing deal a Basic attack.", Alignment.GameModeHideAndSeek, Faction.GameMode,
            "You can run, but you can't hide", CustomColorManager.Hunter, LayerEnum.Hunter, "Basic", "None", "Kill or convert all of the Hunted"),
        new("Hunted", "Hunted", "The Hunted is one of the two roles that spawn in this mod's version of Hide And Seek. The Hunted has no active abilities and just has a higher vision than " +
            "the Hunters and might be able to chat.", Alignment.GameModeHideAndSeek, Faction.GameMode, "I am running and hiding", CustomColorManager.Hunted, LayerEnum.Hunted, "None", "None",
            "Finish all tasks before the Hunters catch them"),
        new("Runner", "Runner", "The Runner is a role that only spawns in the Task Race game mode. It has no abilities and just ends the game when they are the first to finish.",
            Alignment.GameModeTaskRace, Faction.GameMode, "Gotta go fast", CustomColorManager.Runner, LayerEnum.Runner, "None", "None", "Be the first to finish their tasks", footer: true)
    ];

    public static readonly IEnumerable<ModifierInfo> AllModifiers =
    [
        new("Invalid", "Invalid", "Invalid", "Invalid", CustomColorManager.Modifier, LayerEnum.NoneModifier),
        new("Bait", "Bait", "The Bait's killer will be forced to self-report the Bait's body.", "Everyone except Troll, Vigilate, Altruist, Thief and Shifter", CustomColorManager.Bait,
            LayerEnum.Bait),
        new("Coward", "Coward", "The Coward cannot report bodies.", "Everyone", CustomColorManager.Coward, LayerEnum.Coward),
        new("Diseased", "Diseased", "Killing the Diseased increases all of the killer's cooldowns.", "Everyone except Troll and Altruist", CustomColorManager.Diseased, LayerEnum.Diseased),
        new("Drunk", "Drunk", "The Drunk's controls are inverted.", "Everyone", CustomColorManager.Drunk, LayerEnum.Drunk),
        new("Dwarf", "Dwarf", "The Dwarf's body is smaller. The name changes based on its relevant settings.", "Everyone", CustomColorManager.Dwarf, LayerEnum.Dwarf),
        new("Giant", "Giant", "The Giant's body is bigger. The name changes based on its relevant settings.", "Everyone", CustomColorManager.Giant, LayerEnum.Giant),
        new("Useless", "UL", "The Useless modifier only appears when the Dwarf or Giant's speed and size multipliers have been set to 1. It literally does nothing.", "Everyone",
            CustomColorManager.Modifier, LayerEnum.NoneModifier),
        new("Shy", "Shy", "The Shy player cannot call meetings.", "Everyone except Button Barries and roles who cannot call meetings", CustomColorManager.Shy, LayerEnum.Shy),
        new("Indomitable", "Ind", "The Indomitable player cannot be guessed.", "Everyone", CustomColorManager.Indomitable, LayerEnum.Indomitable),
        new("VIP", "VIP", "Everyone is alerted of the VIP's death through a flash of the VIP's role color and will have an arrow poiting towards the VIP's body.", "Everyone",
            CustomColorManager.VIP, LayerEnum.VIP),
        new("Professional", "Prof", "The Professional has an extra life when guessing.", "Assassins", CustomColorManager.Professional, LayerEnum.Professional),
        new("Astral", "Astral", "An Astral player is not teleported to the meeting table.", "Everyone", CustomColorManager.Astral, LayerEnum.Astral),
        new("Yeller", "Yell", "The Yeller's location is revealed to everyone at all times.", "Everyone", CustomColorManager.Yeller, LayerEnum.Yeller),
        new("Colorblind", "CB", "A colorblind player cannot tell the difference between players.", "Everyone", CustomColorManager.Colorblind, LayerEnum.Colorblind),
        new("Volatile", "Vol", "A Volatile player will see random things happen to them and cannot distinguish real kill and flashes from the fake ones.", "Everyone",
            CustomColorManager.Volatile, LayerEnum.Volatile, true)
    ];

    public static readonly IEnumerable<DispositionInfo> AllDispositions =
    [
        new("Invalid", "Invalid", "Invalid", "Invalid", "Invalid", "φ", CustomColorManager.Disposition, LayerEnum.NoneDisposition),
        new("Taskmaster", "TM", "The Taskmaster is basically a living Phantom. When a certain number of tasks are remaining, the Taskmaster is revealed to Intruders and the Syndicate;" +
            " and the Crew only sees a flash to indicate the Taskmaster's existence.", "Finish tasks without dying or game ending", "Neutrals", "µ", CustomColorManager.Taskmaster,
            LayerEnum.Taskmaster),
        new("Lovers", "Lover", "The Lovers are two players who are linked together. They gain the primary objective to stay alive together. In order to so, they gain access to a private" +
            " chat, only visible by them in between meetings. However, they can also win with their respective teams.", "Live to the final 3 with both Lovers still alive", "Everyone", "♥",
            CustomColorManager.Lovers, LayerEnum.Lovers),
        new("Rivals", "Rival", "The Rivals cannot do anything to each other and must get the other one killed.", "Get the other rival killed without directly interfering, then live to the " +
            "final 2", "Everyone", "α", CustomColorManager.Rivals, LayerEnum.Rivals),
        new("Allied", "Ally", "An Allied Neutral Killer now sides with either the Crew, Intruders or the Syndicate. In the case of the latter two, all faction members are shown who is "
            + "their Ally, and can no longer kill them. A Crew-Allied will have tasks that they must complete.", "Win with whichever faction they are aligned with", "Neutral Killers", "ζ",
            CustomColorManager.Allied, LayerEnum.Allied),
        new("Fanatic", "CF (means Crew Fanatic)", "When attacked, the Fanatic joins whichever faction their attacker belongs to. From then on, their alliance sits with said faction.",
            "Get attacked by either the Intruders or the Syndicate to join their team", "Crew", "♠", CustomColorManager.Fanatic, LayerEnum.Fanatic),
        new("Overlord", "Ov", "Every meeting, for as long as an Overlord is alive, players will be alerted to their existence. The game ends if the Overlord lives long enough. All " +
            "alive Overlords win together.", "Survive a set amount of meetings", "Neutrals", "β", CustomColorManager.Overlord, LayerEnum.Overlord),
        new("Corrupted", "Corr", "The Corrupted is a member of the Crew with the alignment of a Neutral Killer. On top of their base role's attributes, they also gain a kill button. Their " +
            "win condition is so strict that not even Neutral Benigns or Evils can be spared. Corrupted adds a level of attack to the applied crewmate.", "Kill everyone", "Crew", "δ",
            CustomColorManager.Corrupted, LayerEnum.Corrupted),
        new("Traitor", "CT (means Crew Traitor)", "The Traitor is a member of the Crew who must finish their tasks to switch sides. Upon doing so, they will either join the Intruders "
            + "or the Syndicate, and will win with that faction. If the Traitor is the only person in their new faction, they become a Betrayer, losing their original role's abilities "
            + "and gaining the ability to kill in the process.", "Finish tasks to join either the Intruders or Syndicate", "Crew", "♣", CustomColorManager.Traitor, LayerEnum.Traitor),
        new("Mafia", "Maf", "The Mafia are a group of players with a linked win condition. They must kill anyone who is not a member of the Mafia. All Mafia win together.", "Kill anyone" +
            " who is not a member of the Mafia", "Everyone", "ω", CustomColorManager.Mafia, LayerEnum.Mafia),
        new("Linked", "Link", "The Linked players are a watered down pair of Lovers. They can help the other player win. As long as one of the links wins, the other does too, regardless " +
            "of how they won.", "Help the other link win", "Neutrals", "Ψ", CustomColorManager.Linked, LayerEnum.Linked),
        new("Defector", "Defect", "A Defector switches sides when they happen to be the last player alive in their original faction.", "Kill off anyone who opposes their new faction",
            "Intruders And Syndicate", "ε", CustomColorManager.Defector, LayerEnum.Defector, true)
    ];

    public static readonly IEnumerable<AbilityInfo> AllAbilities =
    [
        new("Invalid", "Invalid", "Invalid", "Invalid", CustomColorManager.Ability, LayerEnum.NoneAbility),
        new("Assassin", "Assassin", "The Assassin can guess the layers of others. If they guess right, the target is killed mid-meeting and if they guess wrong, they die instead. The name " +
            "of the Assassin ability depends on the faction it affects. Bullseye is for Crew, Hitman is for Intruders, Slayer is for Neutrals and Sniper is for the Syndicate. Assassin adds "
            + "a level of attack to the applied crewmate.", "Intruders, Crew, Syndicate, Neutral (Killing), Neutral (Harbinger) and Neutral (Neophyte)", CustomColorManager.Assassin,
            LayerEnum.Assassin),
        new("Button Barry", "BB", "The Button Barry can call a meeting from anywhere on the map, even during sabotages. Calling a meeting during a sabotage will fix the sabotage.",
            "Everyone except roles who cannot call meetings", CustomColorManager.ButtonBarry, LayerEnum.ButtonBarry),
        new("Insider", "Ins", "The Insider will be able to view everyone's votes in meetings upon finishing their tasks. Only spawns if Anonymous Votes is turn on.", "Crew",
            CustomColorManager.Insider, LayerEnum.Insider),
        new("Multitasker", "MT", "When doing tasks, the Multitasker's task window is transparent.", "Roles with tasks", CustomColorManager.Multitasker, LayerEnum.Multitasker),
        new("Ninja", "Nin", "Ninjas don't lunge when killing.", "Killing roles", CustomColorManager.Ninja, LayerEnum.Ninja),
        new("Radar", "Radar", "The Radar always has an arrow pointing towards the nearest player.", "Everyone", CustomColorManager.Radar, LayerEnum.Radar),
        new("Ruthless", "Ruth", "A Ruthless killer can bypass all forms of protection. Although they bypass alert protection, they will still die to a player on alert.", "Killing roles",
            CustomColorManager.Ruthless, LayerEnum.Ruthless),
        new("Snitch", "Snitch", "The Snitch is an ability which allows any member of the Crew to get arrows pointing towards the Intruders and the Syndicate once all their tasks are " +
            "finished. The names of the Intruders and Syndicate will also show up as red on their screen. However, when they only have a certain amount of tasks left, the Intruders and"
            + " the Syndicate get an arrow pointing towards the Snitch.", "non-Traitor or Fanatic Crew", CustomColorManager.Snitch, LayerEnum.Snitch),
        new("Tiebreaker", "TB", "During the event of a tie vote, the tied player who the Tiebreaker voted for will be ejected. In the case of a Politician, this applies to their first vote.",
            "Everyone", CustomColorManager.Tiebreaker, LayerEnum.Tiebreaker),
        new("Torch", "Torch", "The Torch has Intruder vision at all times and can see the silhouettes of invisible players.", "Crew, Neutral Evil and Benign roles, Neutrals and Neutral"
            + " Killers when their respective lights are off", CustomColorManager.Torch, LayerEnum.Torch),
        new("Tunneler", "Tun", "The Tunneler will be able to vent when they finish their tasks.", "Crew excluding Engineer", CustomColorManager.Tunneler, LayerEnum.Tunneler),
        new("Underdog", "UD", "The Underdog is an Intruder or Syndicate with prolonged cooldowns when with a teammate. When they are the only remaining member, they will have their " +
            "cooldowns shortened.", "Intruders and Syndicate", CustomColorManager.Underdog, LayerEnum.Underdog),
        new("Politician", "Pol", "The Politician can vote multiple times. If the Politician cannot kill, they gain a new button called the abstain button which stores their vote for " +
            "later use. On the other hand, if the Politician can kill, they lose the Abstain button and instead gain a vote for each player they kill.", "Crew, Intruders, Syndicate " +
            "and Neutral Killers", CustomColorManager.Politician, LayerEnum.Politician),
        new("Swapper", "Swap", "The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted towards the second player and "
            + "vice versa.", "Crew", CustomColorManager.Swapper, LayerEnum.Swapper, true)
    ];

    public static readonly IEnumerable<FactionInfo> AllFactions =
    [
        new(Faction.None),
        new(Faction.Crew),
        new(Faction.Intruder),
        new(Faction.Neutral),
        new(Faction.Syndicate),
        new(Faction.GameMode, true)
    ];

    public static readonly IEnumerable<SubFactionInfo> AllSubFactions =
    [
        new(SubFaction.None),
        new(SubFaction.Sect),
        new(SubFaction.Cabal),
        new(SubFaction.Undead),
        new(SubFaction.Reanimated, true)
    ];

    public static readonly IEnumerable<AlignmentInfo> AllAlignments =
    [
        new(Alignment.None),
        new(Alignment.CrewSupport),
        new(Alignment.CrewInvest),
        new(Alignment.CrewProt),
        new(Alignment.CrewKill),
        new(Alignment.CrewUtil),
        new(Alignment.CrewSov),
        new(Alignment.CrewAudit),
        new(Alignment.CrewConceal),
        new(Alignment.CrewDecep),
        new(Alignment.CrewPower),
        new(Alignment.CrewDisrup),
        new(Alignment.CrewHead),
        new(Alignment.IntruderSupport),
        new(Alignment.IntruderConceal),
        new(Alignment.IntruderDecep),
        new(Alignment.IntruderKill),
        new(Alignment.IntruderUtil),
        new(Alignment.IntruderInvest),
        new(Alignment.IntruderProt),
        new(Alignment.IntruderSov),
        new(Alignment.IntruderAudit),
        new(Alignment.IntruderPower),
        new(Alignment.IntruderDisrup),
        new(Alignment.IntruderHead),
        new(Alignment.NeutralKill),
        new(Alignment.NeutralNeo),
        new(Alignment.NeutralEvil),
        new(Alignment.NeutralBen),
        new(Alignment.NeutralPros),
        new(Alignment.NeutralApoc),
        new(Alignment.NeutralHarb),
        new(Alignment.NeutralInvest),
        new(Alignment.NeutralAudit),
        new(Alignment.NeutralSov),
        new(Alignment.NeutralProt),
        new(Alignment.NeutralSupport),
        new(Alignment.NeutralUtil),
        new(Alignment.NeutralConceal),
        new(Alignment.NeutralDecep),
        new(Alignment.NeutralDisrup),
        new(Alignment.NeutralPower),
        new(Alignment.NeutralHead),
        new(Alignment.SyndicateKill),
        new(Alignment.SyndicateSupport),
        new(Alignment.SyndicateDisrup),
        new(Alignment.SyndicatePower),
        new(Alignment.SyndicateUtil),
        new(Alignment.SyndicateInvest),
        new(Alignment.SyndicateProt),
        new(Alignment.SyndicateSov),
        new(Alignment.SyndicateAudit),
        new(Alignment.SyndicateConceal),
        new(Alignment.SyndicateDecep),
        new(Alignment.SyndicateHead),
        new(Alignment.GameModeHideAndSeek),
        new(Alignment.GameModeTaskRace, true)
    ];

    public static readonly IEnumerable<GameModeInfo> AllModes =
    [
        new(GameMode.None),
        new(GameMode.Classic),
        new(GameMode.Vanilla),
        new(GameMode.AllAny),
        new(GameMode.KillingOnly),
        new(GameMode.TaskRace),
        new(GameMode.RoleList),
        new(GameMode.HideAndSeek),
        new(GameMode.Custom, true)
    ];

    public static readonly IEnumerable<Lore> AllLore =
    [
        /*new("All", "The story takes place several hundred years in the future with is a gigantic leap in scientific advancement. As a result, space travel is a common thing and" +
            " is commonly used for mining resources from nearby celestial bodies. A multi-billion dollar government space exploration corporation called \"Mira\" just recently " +
            "discovered a new habitable planet several light years away. They named it \"Polus\". Soon enough, they build a ship that can carry over 200 passengers with plenty of " +
            "storage for items, later deemed as the \"Skeld\". Mira begins an exploration trip to Polus, recruiting special individuals from within their agency to set up camp and" +
            " continue the research of the planet. Little do they know, people whose motives conflict with that of Mira have managed to get onboard the Skeld. Not only that, some of " +
            "the Crew have been mysteriously replaced by shapeshifting parasites hellbent on conquering Earth but unfortunately got swept up in this project. Skeld is soon " +
            "launched from the Mira HQ. After launch, the Skeld becomes a battlefield of death and mind games while the Crew tries to get rid of these evils when stranded in space. " +
            "These are the stories of said passengers aboard the Skeld.", "All", UColor.black),*/
        new("Swapper", "There was once a Crewmate who made the voting software in light of the recent events occurring aboard the Skeld. The software would tally up the votes " +
            "against someone, use a mechanical arm to detain the voted person and transport said voted person to the airlock for ejection. He later witnessed the Mafioso kill the " +
            "Medic and the Janitor clean up the mess in front of him and calls a meeting. The Intruders, using their silver tongues, slip out of the blame and instead push it on " +
            "the Crewmate. The Crewmate knew he had only one thing to do. He told everyone to vote for him and then vote for the others. In the moment of chaos and confusion, the " +
            "rest of the Crew voted for him. The Mafioso and the Janitor, with villainous smirks on their faces, watched the votes get tallied. Soon the faces of confidence turn " +
            "into faces of pure shock, as the votes are tallied not against the Crewmate, but against the Mafioso. A mechanical arm juts out from the walls and grabs the Mafioso " +
            "and starts moving towards the airlock. The Crew, held aback by the sudden change in air, looked at the Crewmate. The Mafioso looked at the Crewmate with a face of " +
            "horror through the window in the airlock door. The Crewmate simply meets his gaze with a face of pure joy and confidence, and mouths, \"I'm the Swapper, bitch.\"", "Swap",
            CustomColorManager.Swapper),
        new("Amnesiac", "A lonely Coroner walked down the hallway. His head was aching and the bandages around his head were starting to loosen. The Coroner just wanted to " +
            "finish his checklist of tasks assigned to him by the Mayor. But he forgot how to do them. He sat at the panel, confused. The Coroner fumbled with the wires a bit. He " +
            "felt a sudden tingle but he paid it no mind. Soon a loud alarm rang. A meeting was in order. Blue had just died, and Green said that the body was in Navigation. " +
            "Everyone started accusing each other, but the Coroner sat at the table with fuzzy memories. He tried to remember what roles he, Blue or Green were, but to no avail. The" +
            " meeting ended with a voting tie between Red and Green. The Coroner with no memories looked at Red, he noticed a sinister glare on his face. The Amnesiac suddenly felt a "
            + "repressed memory try to make its way back to the surface, but he still couldn't completely remember it. The Amnesiac moved to the kitchen to make himself a sandwich. He"
            + " just looked at the knife, enamoured by it. He picked it and sliced the tomato in half. A certain joy filled the Amnesiac's heart. Just behind him, Red, the Grenadier, "
            + "was being murdered by Green, the Glitch. Green ran away, unnoticed by the Amnesiac. The Amnesiac's sense rang loudly as he turned around to see the wheezing and dying " +
            "Red. He soon realised what he wanted in life, what his true goal was. He had finally remembered what his purpose was. A creepy smile began to set onto the Amnesiac's " +
            "face as he raised his kitchen knife into the air. A meeting was called as Red's body was discovered to have multiple stab wounds, his backpack emptied. The Glitch was " +
            "confused, he only remembered stabbing the Grenadier once. The Amnesiac sat there in silence, playing around with the toys he had found. He enjoyed the clicking sounds " +
            "the new toys in his hand made and was itching to use them. Green was being ejected thanks to the close tie he had with Red in the last meeting. Soon the meeting came to " +
            "a close. Just as everyone was dispersing, the spirit of the Grenadier looked at the Amnesiac, with a cold smile on his face. He had raised his successor well.", "Amne",
            CustomColorManager.Amnesiac),
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
            + "he was the one with the knowledge to break through the universe's strongest barrier. He was the Glitch after all.", "Gli", CustomColorManager.Glitch),
        new("Juggernaut", "A paranoid Veteran watched his loved one die to the hands of the Crew. He couldn't bear to think he wasn't there to save her. \"If only I was stronger" +
            ".\" was the thought that plagued his mind. Day in and day out, he pursued strength, in his ultimate goal to destroy Mira, the very company that killed his wife in cold " +
            "blood. But, he just couldn't shake off the paranoia from the war. No amount of self healing or meditating could take away those horrid memories from wartime. His wife " +
            "was his only way to support himself, his lifeline. Everytime he thought of her, he would be engulfed in deadly rage, unable to calm down until his fists bled from " +
            "punching the walls. One day, he saw a job listing to explore a newly discovered planet, Polus. The advertiser? Mira. \"Perfect.\" thought the Veteran as he lifted himself"
            + " up from his couch, and readied his uniform to go to the application site. He got to the site, to only see that Mira wasn't even performing background checks on the " +
            "applicants. \"That lax behaviour will get you killed, Mira.\" After a few days, he received an acceptance letter. He was accepted! He boarded the ship to see familiar " +
            "faces, as well suspicious ones. The Mayor, the one who led the team that killed his wife and the Godfather, who he suspected was the cause. Aboard the ship he met new " +
            "faces, the weak Crewmate, the just Vigilante and the lovely Medic. She reminded him of his late wife. But he could not spare his feelings for her. She was affiliated with "
            + "Mira, making her his enemy. A couple days later, he set off on the journey to Polus. He thought this was his time to shine, but he couldn't bring himself to kill anyone"
            + ". Most of the people here were innocent, forced to go on the mission with him. He couldn't hurt these poor souls. The Godfather paid mind to the Veteran's antics, and " +
            "struck a deal with him. They met up in the infirmary to talk business. As they were discussing, the Godfather let loose a parasite, to take over and control the " +
            "Veteran. Just as he began, the Veteran felt a tingle, but thought nothing of it. The infirmary doors burst open with the Vigilante and the Seer entering with full " +
            "force. \"AHA! I KNEW YOU WERE UP TO NO GOOD!\" said the robust Vigilante, preparing his gun to fire. But that declaration triggered something within the Veteran. He " +
            "heard an audible snap and then he felt bliss. A couple minutes later, a meeting was called by the Medic. \"3 bodies were found in the infirmary...brutally mangled " +
            "beyond recognition.\" she declared, holding back the urge to puke. As the rest of the Crew gasped in horror, only the Juggernaut smiled.", "Jugg", CustomColorManager.Juggernaut),
        new("Medic", "The Medic came from a highly evaluated university, with the highest grades possible. She was the best in the field, until an accident took her ability to " +
            "heal others. Ashamed of her incident and only being able to perform rudimentary first aid, the Medic sought for a job on the Polus mission. She was accepted! This was " +
            "her turning point, one where she would be known for something else, and hopefully heal herself in the process. The Medic's dreams fell short as people started dying " +
            "to mysterious killers among the Crew. She tried her best to find people to protect, but her arrivals were too little too late. The only things waiting for her were the " +
            "bodies of those she swore to protect and a lingering sense of dread. She couldn't get by just looking for people to heal and instead decided to concentrate on one " +
            "person, so at least they'd be safe. It was the Crewmate, a loveable simpleton who only stood for justice. She would know when he would be attacked. All she had to do " +
            "was just sit and lie in wait, patiently waiting for a killer to slip up and attack the Crewmate and alerting her. But on her way to the cafeteria, she heard wheezing. " +
            "It was the corrupt Mayor, the one who was the sole reason behind the Medic's accident. Her entire being said let him die, but only her heart said to save him, for she " +
            "was not a monster, but the barrier between life and death for the Crew. She slowly approached the Mayor, pushing down her hatred for him. \"Where does it hurt?\"", "Medic",
            CustomColorManager.Medic),
        new("Crewmate", "Nothing fruitful ever happened in the Crewmate's life. He was just lucky enough to get a spot in the exploration trip to Polus. Only useful for " +
            "finishing tasks and basic repairs, he decided to make the most of his time aboard the Skeld. Getting acquainted with all those famous celebrities from Mira, the " +
            "Crewmate felt a sense of bliss and happiness. He was going to make history. He was finally going to be able to stand with the celebrities like the Mayor. That would'" +
            "ve happened, if it were not for the Intruder aboard their ship. First it was the disruption of their tasks, then it was the sabotages and then finally the Intruders " +
            "took a step further. Killing. The Crewmate feared the loss of his life and went into hiding. He knew only one thing, he had to ensure the killers got thrown out of the " +
            "ship in order for him to survive.", "Crew", CustomColorManager.Crew),
        new("Operative", "There was once a Crewmate who always had a knack for invention. He made special devices he deemed as \"bugs\". He could spy on others with these and " +
            "later realised, he could make these bugs detect more than just players. He figured that upgrading the Admin table would be worth it. He slowly put trackers on his " +
            "fellow crew and assigned a colour to each. He then connected the trackers to the admin table to see where everyone was. He then linked these trackers to his bugs, " +
            "which fed him info. And it worked perfectly. He couldn't wait to show it to the rest of the crew. But that was when the assassinations started to happen. Crewmates " +
            "dropped dead during discussions, so the Crewmate resolved to not tell the crew about it, fearing the worst. Slowly but surely, the Crewmate began spying on others, " +
            "his wall of distrust getting higher and higher. And that when he watched as Red and Blue went into the Medbay together but only Red came back out. The Crewmate sat " +
            "in Admin, waiting for Blue to leave, but he didn't and that was when the Crewmate knew something was up. He dashed straight to Medbay and saw Blue's body, slashed in " +
            "half. The Crewmate reported the body and called out Red. Red was flabbergasted, not knowing how the Crewmate figured out his identity. In a last ditch attempt, Red, " +
            "the Janitor, guessed the Crewmate as the Engineer. That didn’t work as the Undertaker started to lose his breath. The Crewmate looked across the table and met gazes with "
            + "the dying Janitor. He was the Operative all along.", "Op", CustomColorManager.Operative),
        new("Warper", "The Rebel watched on as the Godfather was sent out the airlock because of the Transporter’s antics. He was intrigued by this technology and sought to add the " +
            "Transporter to his own group. Of course, he was yet to recruit him. The Transporter was not bending so easily. And so, the Rebel thought of a plan and set gears into " +
            "motion. Life for the Transporter was not going well. Instead of loving him for finding the Godfather, they hated him for his mischievous antics. This only grew worse as " +
            "time went on, as things started going wrong for him. He accidentally warped someone into space and killed them. Of course, the Crew still did not know he was the " +
            "Transporter, but their hate for him only grew more as they tried to find him. The Rebel kept worsening the Transporter’s rumors, to the point the Transporter had had " +
            "enough. He went back to the Rebel, who was eagerly waiting for him. \"Fine, I’ll take you up on your deal. Those damn bastards will soon learn that hating someone with " +
            "power is not something they should do of their own leisure.\", said the Transporter, to the Rebel and his Sidekick, who was sitting in the darkness.  The Rebel began, " +
            "\"Alright then! The deal’s closed. Your job is to just bring people to us. Simple enough right? To help you, we’ll provide you with materials to even improve your " +
            "technology! We have this device you see...\" The Transporter listened on, as he soon realised who he had joined forces with. But, strangely enough, he was left unaffected,"
            + " as if he wanted this to happen. After all, he just couldn’t wait to get his revenge on the Crew for not realising his potential. \"Alright then, where do I begin?\" " +
            "asks the Transporter with a serious, yet expectant face. \"Patience my dear Warper, patience.\", said the Rebel.", "Warp", CustomColorManager.Warper),
        new("Transporter", "A Crewmate, using his genius ideas, invented a device called the Warpstone, which transported a person to a set coordinate. The Crewmate, with his " +
            "mischievous side, used his new invention to the fullest and his antics were hated by the Crew. None of them knew who was the one transporting them. But it was only a " +
            "matter of time before the identity of the mystery devil came to light. The Crewmate was walking down the hall when he noticed a suspicious person running up to someone " +
            "else. Just as he got close, the Crewmate saw the knife in the suspicious person's hands. Using his quick typing skills, the Crewmate transported the target with someone " +
            "else. \"This is it.\", the Godfather thought as he lunged towards his target. But just as he was about to attack, his target was surrounded by blinding light followed by,"
            + " \"STOP! IT'S M-\" <i>slash</i> The Godfather dropped the knife down and knelt down in horror as he stared at the body. He had slashed his Janitor, his last surviving " +
            "teammate. The Crewmate knew what he had seen the Godfather do. The Crewmate quickly reported the body and called out the Godfather for killing someone. The Godfather, " +
            "unable to form words due to the horror of what happened, couldn’t defend himself. Just as the Godfather was getting thrown into the airlock, he locked eyes with the " +
            "Crewmate. The Crewmate, with a smirk, gave the Godfather a V from his fingers and only then did the Godfather realise who the Transporter was.", "Trans",
            CustomColorManager.Transporter)
    ];

    public static readonly IEnumerable<OtherInfo> AllOthers =
    [
        new("Invalid", "Invalid", "Invalid", UColor.red, "Invalid"),
        new("Chaos Drive", "CD", "The Chaos Drive is an ability boosting device that the Syndicate receives after a certain number of meetings. When the Chaos Drive is discovered, it "
            + "is handed to members of the Syndicate in a particular order. The holder of the Chaos Drive gains the ability to kill (if they didn't already) and have stronger variations/" +
            "buffs to their abilities.", CustomColorManager.Syndicate, "Order Of Receiving The Chaos Drive:\nPromoted Rebel\nSyndicate (Disruption)\nSyndicate (Support)\nSyndicate (Power)" +
            "\nSyndicate (Killing)\nOriginal Rebel, Sidekick, Anarchist"),
        new("Role", "Role", "Roles decide your abilities and goals for the game. Every game, you are guaranteed to have a role as not having one basically means you cannot play the game.",
            CustomColorManager.Role),
        new("Disposition", "Disp", "Dispositions provide an alternate way for you to win, and sometimes they may override the your original win condition (see Corrupted and Mafia) or change" +
            " your win condition mid-game (see Traitor and Fanatic).", CustomColorManager.Disposition),
        new("Modifier", "Mod", "Modifiers are passive afflictions, usually negative or benign in nature, that serve no purpose and are there for fun. It cam alter a player's gameplay based"
            + " on what they might have. For example, Baits and Diseased players would want to die for their modifiers to take effect.", CustomColorManager.Modifier),
        new("Ability", "Ab", "Abilities give you an additional ability on top of your original abilities, to help boost your chances of winning.", CustomColorManager.Ability),
        new("Faction", "Faction", "A faction is the team a role, by default, belongs to. There can be situations where one player can jump ship to another team. Roles in the Neutral faction"
            + " don't have a team, namely the Neutral (Evil) roles. Neutral (Benign) roles can side with whoever, and Neutral (Neophyte) roles form their own mini faction called a subfaction"
            + ". Neutral (Killing) roles are generally solo, but can be toggled to side with each other.", CustomColorManager.Faction),
        new("Alignment", "Alignment", "Alignments are a general umbrella where certain roles from other factions fall under. Alignments are generally only used for classification purposes " +
            "and a role cannot change alignments at any point in the game under normal circumstances.", CustomColorManager.Alignment, "", true)
    ];

    public static readonly IEnumerable<SymbolInfo> AllSymbols =
    [
        new("Invalid", "Invalid", "Invalid", UColor.red),
        new("Null", "φ", "This symbol is a placeholder for when one doesn't have a disposition or subfaction that they are a part of.", CustomColorManager.Status),
        new("Shield", "✚", "Players who have shield deployed on them through a Medic or a Retributionist-Medic.", CustomColorManager.Medic),
        new("Knighted", "κ", "Players who have been knighted by a Monarch recieve this symbol.", CustomColorManager.Monarch),
        new("Trapped", "∮", "Players that have been trapped by a Trapper or a Retributionist-Trapper. Trapped players will have their interactor's roles logged and sent to the relevant " +
            "Trapper. If the trapped target is attacked, the trap will remove all of its logged roles and attack the attacker. The Trapper is notified if the trap attacked someone, and if " +
            "it didn't then they get a list of roles that interacted with their targets.", CustomColorManager.Trapper),
        new("Arsonist Doused", "Ξ", "Players who have been doused by an Arsonist. Doused players can be ignited by the Arsonist from anywhere.", CustomColorManager.Arsonist),
        new("Bounty", "Θ", "Targets of a Bounty Hunter will have this symbol on their name. All targets know that there is a bounty on their head.", CustomColorManager.BountyHunter),
        new("Cryomaniac Doused", "λ", "Players who have been doused by a Cryomaniac. Doused players can be frozen by the Arsonist from anywhere.", CustomColorManager.Cryomaniac),
        new("Bitton", "γ", "The symbol that means one is a part of the Undead subfaction.", CustomColorManager.Undead),
        new("Tormented", "§", "Players being tormented by an Executioner. Avoid ejecting them to increases your chances of winning.", CustomColorManager.Executioner),
        new("Watched", "★", "Players being watched by a Guardian Angel. Try to recruit them, or their target to your side to increase you chances of winnign.", UColor.white),
        new("Protected", "η", "Players being protected by their Guardian Angel.", UColor.white),
        new("Agonised", "π", "Players being agonised by a Guesser. If the player happens to be on your team, avoid revealing their role publicly to increase your chances of winning.",
            CustomColorManager.Guesser),
        new("Recruited", "$", "The symbol that means one is a part of the Cabal subfaction.", CustomColorManager.Cabal),
        new("Resurrected", "Σ", "The symbol that means one is a part of the Reanimated subfaction.", CustomColorManager.Reanimated),
        new("Infected", "ρ", "Players that have been infected by a Plaguebearer. Once everyone is infected, the Plaguebearer becomes Pestilence.", CustomColorManager.Plaguebearer),
        new("Plague", "米", "This symbol denotes how many stacks of the plague a certain player has.", CustomColorManager.Pestilence),
        new("Vested", "υ", "Survivors who currently have their vests active.", CustomColorManager.Survivor),
        new("Persuaded", "Λ", "The symbol that means one is a part of the Sect subfaction.", CustomColorManager.Sect),
        new("Framed", "ς", "Players who are framed by a Framer or Rebel-Framer. Framed players can die without triggering penalties for killers like Vigilante and Assassin.",
            CustomColorManager.Framer),
        new("Spellbound", "ø", "Players who have been spellbound by a Spellslinger or Rebel-Spellslinger. When all non-Syndicate players are spellbound, the game will immediately end in" +
            "the Spellslinger's victory.", CustomColorManager.Spellslinger),
        new("Blackmailed", "Φ", "Players who have been blackmailed by a Blackmailer or Godfather-Blackmailer. Blackmailed players cannot speak in chat.", CustomColorManager.Blackmailer),
        new("Silenced", "乂", "Players who have been silenced by a Silencer or Rebel-Silencer. Silenced plaeyrs cannot see anything in chat aside from system messages.",
            CustomColorManager.Silencer),
        new("Flashed", "ㅇ", "Players who have been blinded by a Grenadier or a Godfather-Grenadier. Flashed players cannot see anything as their screen turns white. Players exempted from " +
            "being flashed will have their screen dull out a little to indicate that they got flashed.", CustomColorManager.Grenadier),
        new("Drived", "Δ", "The player who currently wields the Chaos Drive within the Syndicate faction.", CustomColorManager.Syndicate),
        new("Friend", "ξ", "This symbols indicates that they are on the same faction as you, but beware them as they could still backstab you.", CustomColorManager.Faction),
        new("Marked", "χ", "Players who have been marked by the Ghoul. Marked players will die at the end of the next meeting.", CustomColorManager.Ghoul),
        new("First Dead", "Γ", "When the setting is enabled, the first person to die in the previous game gets a shield of protection in the next game for the first round.",
            CustomColorManager.FirstShield),
        new("Alerting", "σ", "Players who are on high alert. Players on alert will attack anyone who interacts with them.", CustomColorManager.Veteran),
        new("Crusaded", "τ", "Players being targeted by a Crusader. Crusaded players will attack anyone who interact with them, or if the Crusader holds the drive, everyone in a certain " +
            "radius.", CustomColorManager.Crusader),
        new("Ambushed", "人", "Players being targeted by an Ambusher. AMbushed players will attack anyone who interact with them.", CustomColorManager.Ambusher),
        new("Allied", "ζ", "Use to denote those who have the Allied disposition. The color of this symbol denotes which faction they are a part of.", CustomColorManager.Allied),
        new("Corrupted", "δ", "Use to denote those who have the Corrupted disposition.", CustomColorManager.Corrupted),
        new("Defector", "ε", "Use to denote those who have the Defector disposition. The color of this symbol denotes which faction they are a part of.", CustomColorManager.Defector),
        new("Fanatic", "♠", "Use to denote those who have the Fanatic disposition. The color of this symbol denotes which faction they are a part of.", CustomColorManager.Fanatic),
        new("Linked", "Ψ", "Use to denote those who have the Linked disposition.", CustomColorManager.Linked),
        new("Lovers", "♥", "Use to denote those who have the Lovers disposition.", CustomColorManager.Lovers),
        new("Mafia", "ω", "Use to denote those who have the Mafia disposition.", CustomColorManager.Mafia),
        new("Overlord", "β", "Use to denote those who have the Overlord disposition.", CustomColorManager.Overlord),
        new("Rivals", "α", "Use to denote those who have the Rivals disposition.", CustomColorManager.Rivals),
        new("Taskmaster", "µ", "Use to denote those who have the Taskmaster disposition.", CustomColorManager.Taskmaster),
        new("Traitor", "♣", "Use to denote those who have the Traitor disposition. The color of this symbol denotes which faction they are a part of.", CustomColorManager.Traitor, true)
    ];
}