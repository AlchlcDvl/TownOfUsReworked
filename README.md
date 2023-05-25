
### ***Note: This repo is a fork of [Town of Us Reactivated](https://github.com/eDonnes124/Town-Of-Us-R) and is currently under development.***
### ***Also Note: This mod is NOT for mobile and console versions, do not ask for a port.***

-----------------------

![LOGO](./Images/Reworked.png "Logo")

<p align="center"><a href="https://github.com/AlchlcDvl/TownOfUsReworked/releases/"><img src="https://badgen.net/github/release/AlchlcDvl/TownOfUsReworked?icon=github"></a></p>

An [Among Us](https://store.steampowered.com/app/945360/Among_Us) mod that adds a bunch of roles, modifiers, objectifiers, abilities, improvements and game modes, modifiers and settings.

Join our [Discord](https://discord.gg/cd27aDQDY9) if you have any problems or want to find people to play with!

-----------------------

# Contents

[**Contents**](#contents)

[**Releases**](#releases)

[**Changelogs**](#changelogs)

[**Installation**](#installation)
- [Requirements](#requirements)
- [Steam Guide](#steam-guide)
- [Epic Games Guide](#epic-games-guide)
- [Linux Guide](#linux-guide)
- [Results](#results)
- [Issues](#issues)

[**Uninstallation**](#uninstallation)

[**Differences**](#differences)

[**Player Layers**](#player-layers)

| [Intruders](#intruder-roles) | [Syndicate](#syndicate-roles) | [Crew](#crew-roles)               | [Neutrals](#neutral-roles)        |
|------------------------------|-------------------------------|-----------------------------------|-----------------------------------|
| [Ambusher](#ambusher)        | [Anarchist](#anarchist)       | [Altruist](#altruist)             | [Actor](#actor)                   |
| [Blackmailer](#blackmailer)  | [Banshee](#banshee)           | [Chameleon](#chameleon)           | [Amnesiac](#amnesiac)             |
| [Camouflager](#camouflager)  | [Bomber](#bomber)             | [Coroner](#coroner)               | [Arsonist](#arsonist)             |
| [Consigliere](#consigliere)  | [Collider](#collider)         | [Crewmate](#crewmate)             | [Betrayer](betrayer)              |
| [Consort](#consort)          | [Concealer](#concealer)       | [Detective](#detective)           | [Bounty Hunter](#bounty-hunter)   |
| [Disguiser](#disguiser)      | [Crusader](#crusader)         | [Dictator](#dictator)             | [Cannibal](#cannibal)             |
| [Enforcer](#enforcer)        | [Drunkard](#drunkard)         | [Engineer](#engineer)             | [Cryomaniac](#cryomaniac)         |
| [Ghoul](#ghoul)              | [Framer](#framer)             | [Escort](#escort)                 | [Dracula](#dracula)               |
| [Godfather](#godfather)      | [Poisoner](#poisoner)         | [Inspector](#inspector)           | [Executioner](#executioner)       |
| [Grenadier](#grenadier)      | [Rebel](#rebel)               | [Mayor](#mayor)                   | [Glitch](#glitch)                 |
| [Impostor](#impostor)        | [Shapeshifter](#shapeshifter) | [Medic](#medic)                   | [Guardian Angel](#guardian-angel) |
| [Janitor](#janitor)          | [Sidekick](#sidekick)         | [Medium](#medium)                 | [Guesser](#guesser)               |
| [Mafioso](#mafioso)          | [Silencer](#silencer)         | [Monarch](#monarch)               | [Jackal](#jackal)                 |
| [Miner](#miner)              | [Spellslinger](#spellslinger) | [Mystic](#mystic)                 | [Jester](#jester)                 |
| [Morphling](#morphling)      | [Stalker](#stalker)           | [Operative](#operative)           | [Juggernaut](#juggernaut)         |
| [Teleporter](#teleporter)    | [Time Keeper](time-keeper)    | [Retributionist](#retributionist) | [Murderer](#murderer)             |
| [Wraith](#wraith)            | [Warper](#warper)             | [Revealer](#revealer)             | [Necromancer](#necromancer)       |
|                              |                               | [Seer](#seer)                     | [Pestilence](#pestilence)         |
|                              |                               | [Sheriff](#sheriff)               | [Phantom](#phantom)               |
|                              |                               | [Shifter](#shifter)               | [Plaguebearer](#plaguebearer)     |
|                              |                               | [Tracker](#tracker)               | [Serial Killer](#serial-killer)   |
|                              |                               | [Transporter](#transporter)       | [Survivor](#survivor)             |
|                              |                               | [Vampire Hunter](#vampire-hunter) | [Thief](#thief)                   |
|                              |                               | [Veteran](#veteran)               | [Werewolf](#werewolf)             |
|                              |                               | [Vigilante](#vigilante)           | [Whisperer](#whisperer)           |

| [Modifiers](#modifiers)       | [Abilities](#abilities)       | [Objectifiers](#objectifiers) |
|-------------------------------|-------------------------------|-------------------------------|
| [Bait](#bait)                 | [Assassin](#assassin)         | [Allied](#allied)             |
| [Coward](#coward)             | [Button Barry](#button-barry) | [Corrupted](#corrupted)       |
| [Diseased](#diseased)         | [Insider](#insider)           | [Defector](#defector)         |
| [Drunk](#drunk)               | [Multitasker](#multitasker)   | [Fanatic](#fanatic)           |
| [Dwarf](#dwarf)               | [Ninja](#ninja)               | [Lovers](#lovers)             |
| [Flincher](#flincher)         | [Politician](#politician)     | [Mafia](#mafia)               |
| [Giant](#giant)               | [Radar](#radar)               | [Overlord](#overlord)         |
| [Indomitable](#indomitable)   | [Ruthless](#ruthless)         | [Rivals](#rivals)             |
| [Professional](#professional) | [Snitch](#snitch)             | [Taskmaster](#taskmaster)     |
| [Shy](#shy)                   | [Swapper](#swapper)           | [Traitor](#traitor)           |
| [VIP](#vip)                   | [Tiebreaker](#tiebreaker)     |                               |
| [Volatile](#volatile)         | [Torch](#torch)               |                               |
|                               | [Tunneler](#tunneler)         |                               |
|                               | [Underdog](#underdog)         |                               |

[**Custom Game Settings**](#custom-game-settings)
- [Common Settings](#common-settings)
- [Crew Settings](#crew-settings)
- [Intruder Settings](#intruder-settings)
- [Syndicate Settings](#syndicate-settings)
- [Neutral Settings](#neutral-settings)
- [Game Settings](#game-settings)
- [Game Mode Settings](#game-mode-settings)
- [Killing Only Settings](#killing-only-settings)
- [All Any Settings](#all-any-settings)
- [Game Modifiers](#game-modifiers)
- [Game Announcements](#game-announcements)
- [Quality Changes](#quality-changes)
- [Map Settings](#map-settings)
- [Better Sabotage Settings](#better-sabotage-settings)
- [Better Skeld Settings](#better-skeld-settings)
- [Better Polus Settings](#better-polus-settings)
- [Better Airship Settings](#better-airship-settings)

[**Game Info**](#game-info)
- [Lighter Darker Colors](#lighter-darker-colors)
- [Inspector Results](#inspector-results)
- [Subfactions](#subfactions)
- [Syndicate Chaos Drive Role Priority](#syndicate-chaos-drive-role-priority)
- [Game Modes](#game-modes)

[**Extras**](#extras)
- [New Colors](#new-colors)
- [Custom Cosmetics](#custom-cosmetics)
- [Bug Reports, Suggestions & Additions](#bug--suggestions--additions)

[**Layer Explanation And Assignment**](#layer-explanation-and-assignment)

[**Credits & Resources**](#credits--resources)

[**License**](#license)

-----------------------

# Releases

**Disclaimer: The mod is *not* guaranteed to work on the latest versions of Among Us when it updates.**

| Among Us | Mod Version | Link |
|----------|-------------|------|
| 2023.3.28 (s & e) | v0.2.5 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.2.5/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.2.4 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.2.4/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.2.3 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.2.3/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.2.2 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.2.2/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.2.1 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.2.1/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.2.0 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.2.0/TownOfUsReworked.zip) |

<details>
<summary>Older Versions</summary>
| Among Us | Mod Version | Link |
|----------|-------------|------|
| 2023.3.28 (s & e) | v0.1.0-dev1 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.1.0-dev1/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.4-dev2 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.4-dev2/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.4-dev1 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.4-dev1/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.3-dev3 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.3-dev3/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.3-dev2 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.3-dev2/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.3-dev1 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.3-dev1/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.3 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.3/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.2dev8.75 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev8.75/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.2dev8.5 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev8.5/TownOfUsReworked.zip) |
| 2023.3.28 (s & e) | v0.0.2dev8 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev8/TownOfUsReworked.zip) |
| 2023.2.28 (s & e) | v0.0.2dev7 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev7/TownOfUsReworked.zip) |
| 2023.2.28 (s & e) | v0.0.2dev6 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev6/TownOfUsReworked.zip) |
| 2023.2.28 (s & e) | v0.0.2dev5 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev5/TownOfUsReworked.zip) |
| 2023.2.28 (s & e) | v0.0.2dev4 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev4/TownOfUsReworked.zip) |
| 2023.2.28 (s & e) | v0.0.2dev3 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev3/TownOfUsReworked.zip) |
| 2023.2.28 (s & e) | v0.0.2dev2 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.2-dev2/ToU-Rew-v0.0.2-dev2.zip) |
| 2022.12.14 (s & e) | v0.0.1dev19 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev19/ToU-Rew-v0.0.1-dev19.zip) |
| 2022.12.14 (s & e) | v0.0.1dev18.5 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev18.5/ToU-Rew-v0.0.1-dev18.5.zip) |
| 2022.12.14 (s & e) | v0.0.1dev18 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev18/ToU-Rew-v0.0.1-dev18.zip) |
| 2022.12.14 (s & e) | v0.0.1dev17 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev17/ToU-Rew-v0.0.1-dev17.zip) |
| 2022.12.14 (s & e) | v0.0.1dev16 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev16/ToU-Rew-v0.0.1dev16.rar) |
| 2022.12.14 (s & e) | v0.0.1dev15 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev15/ToU-Rew-v0.0.1dev15.rar) |
| 2022.12.14 (s & e) | v0.0.1dev14 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev14/ToU-Rew-v0.0.1dev14.rar) |
| 2022.12.14 (s & e) | v0.0.1dev13.5 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev13.5/ToU-Rew-v0.0.1dev13.5.rar) |
| 2022.12.14 (s & e) | v0.0.1dev13 | [Download](https://github.com/AlchlcDvl/TownOfUsReworked/releases/download/v0.0.1-dev13/ToU-Rew-v0.0.1dev13.rar) |
</details>

-----------------------

# Changelogs

<details>
<summary>Changelog</summary>
<details>
<summary>v0.2.5</summary>
- New Role: Silencer [Syndicate (Disruption)]
- Return: Time Lord and Time Master are back, and more vengeful then ever as the Time Keeper [Syndicate (Power)]!
- Return: Neutral (Evil) roles now end the game again, but this is togglable
- Addition: Added a credits button to the main menu
- Finished: Role cards are now complete
- Buff: Players with the Torch ability can now see the silhouette of invisible players
- Buff: VIP dying will now create an arrow pointing towards the VIP's body
- Buff: Thief can now guess players as killing roles to steal their roles (yes, I took the idea from The Other Roles)
- Change: Collider's Inspector results are now Unseen rather than MovesAround
- Recode: Medium has been recoded a little bit for better functionality
- Recode: Shapeshifts have been recoded again along with appearances being handled even better than before
- Improvement: Transport and Warp animation sizes now scale with the size of the teleported player
- Improvement: Improved how arrows and map blips are handled
- Improvement: Some parts were improved internally
- Improvement: Improved how kills are handled
- Improvement: Improved Level Impostor compatibility (WIP)
- Visual Fix: Fixed the intro messages being clipped off screen for some roles
- Critical Fix: Fixed Giant and Dwarf crashing the game upon game start
- Critical Fix: Fixed cosmetics not loading correctly
- Bug Fix: Fixed Radar and Tiebreaker not being assigned
- Bug Fix: Fixed only one player being shaken when blackmailed players are revealed
- Bug Fix: Fixed some rpc spamming regarding kills
- Role Fix: Fixed Glitch being unable to hack
- Role Fix: Fixed Drunkard-Rebels not inverting players' controls
- Role Fix: Fixed Drunkard inverting controls of players forever
- Role Fix: Fixed winning Cannibals not showing up on the win screen
- Role Fix: Guardian Angel, Bounty Hunter, Execuioner and Guesser can no longer get their Lover/Rival as targets
- Role Fix: Fixed Dictator not dying for all players upon misfiring on Crew
- Objectifier Fix: Fixed Syndicate defected Intruders being able to see the sabotage button
- Objectifier Fix: Fixed Defectors not letting the games end properly
- Objectifier Fix: Fixed only 1 Mafia spawning
</details>
<details>
<details>
<summary>v0.2.4</summary>
 - New Objectifier: Defector
 - New Alignments: Neutral (Harbinger) and Neutral (Apocalypse)
 - Addition: Added some secret role alignments, try seeing how to get them (hint: betrayal and change)
 - Addition: Some actions are now animated
 - Addition: Added a button to be able to see what settings are there in game
 - Addition: Role cards: this is where the info about your role goes instead of the task list (WIP)
 - Buff: Ruthless and Ninja can now be assigned to Crew (Killing) roles and those with the Corrupted objectifier
 - Nerf: Intruders and Syndicate with Ruthless can no longer convert Fanatics
 - Change: Plaguebearer can no longer get Allied
 - Improvement: Improved how appearance changes are handled
 - Improvement: Added a logo next to the ping tracker (thanks to @Jsushi7)
 - Improvement: Improved the layout of the main menu (inspired by the main menu changes made by @Zeo666 in All The Roles)
 - Improvement: The task counter has been moved from your name to the task tab
 - Improvement: The zooming buttons are now smaller and moved to a different location
 - Improvement: The in-game wiki has recieved a slight recode
 - Improvement: You no longer know the status of players until the meeting after your death (to avoid players coming back to life and ratting everyone out)
 - Improvement: Improved the footprint graphics for Detectives and Retributionist-Detectives
 - Bug Fix: Fixed the spectate button opening up the haunting menu
 - Bug Fix: Fixed being unable to leave vents
 - Bug Fix: Fixed vision not updating correctly when switching between dead and alive players using MCI
 - Bug Fix: Fixed Pestilence spawning instead of Plaguebearer with the Pestilence Spawn option is turned off
 - Visual Fix: Fixed ghosts being unable to see the names and roles of players through walls with the obstruct names option turned on
 - Role Fix: Fixed the Concealer's target staying invisible forever
 - Role Fix: Fixed the Guesser's target's alignment hint having text hexcode color control
 - Role Fix: Fixed Bounty Hunter being shown their target on the intro screen
 - Role Fix: Fixed Dracula and Jackal killing those they are trying to convert
 - Role Fix: Fixed turning betrayer being spammed in the end game summary
 - Ability Fix: Fixed killing Politicians being unable to vote players
 - Removal: The lobby timer because it's kind of useless as most people play on modded servers anyways, which don't expire
</details>
<details>
<summary>v0.2.3</summary>
 - Nerf: Reporting a body while being flashed yields no results for the Coroner and the Retributionist-Coroner
 - Addition: Added "Hide Obstructed Player Names" option so that you don't see players' names through walls
 - Improvement: Improved some rpc handling between roles
 - Improvement: Reduced the mod size even more, god I love doing this
 - Critical Fix: Fixed custom cosmetics not loading
 - Visual Fix: Fixed the hat and visor tabs not displaying cosmetics
 - Ability Fix: Fixed Ninja appearing invisible (ironic, I know)
</details>
<details>
<summary>v0.2.2</summary>
 - Critical Fix: Fixed the sudden issue of players just not dying at all
 - Temporary Removal: Custom cosmetics are temporarily removed till I figure out why they aren't loading
</details>
<details>
<summary>v0.2.1</summary>
 - New Preset: Ranked
 - Return: Drunkard's back, new and improved!
 - Return: Cosmetic sections are back
 - Addition: Added the /summary (or /sum for short) command for being able to see the game summary of the previous game when in lobby
 - Addition: Dwarf and Giant were changed interally, mix and match with the settings to find things out ;)
 - Buff: Completing tasks restores a charge for roles with limited uses
 - Buff: On low player counts, Amnesiac now becomes a Thief
 - Buff: Monarchs with alive knights now cannot be killed as long as one of the alive knights is Crew (Ruthless still bypasses this)
 - Nerf: Clicking a Revealer who has finished their tasks now disables their ability
 - Nerf: Everyone is told who has been spelled during meetings, so at least the dangers of Spellslinger is known
 - Nerf: Misfiring on a framed innocent player will no longer kill the Vigilante, unless the Misfire setting is turned on
 - Recode: Cosmetic handling was recoded to work like how The Other Roles does it
 - Optimisation: Some code was optimsed
 - Improvement: Turned Traitors and Fanatics with the Assassin ability now have their guessing menus updated to their new factions
 - Improvement: Added a paging behaviour for the guessing menu, to prevent guess buttons going off-screen
 - Improvement: The mod's weight has been heavily reduced
 - Improvement: Improved player targetting some more
 - Change: Teleporter's color was changed because it was too close to the Grenadier's green
 - Critical Fix: Fixed Airship crashing players when loading in
 - Bug Fix: Fixed the weird issue with blank abilities appearing
 - Bug Fix: Fixed being able to target dead players and postmortal roles
 - Bug Fix: Fixed loading presets not actually closing the option menu
 - Bug Fix: Changing the screen resolution no longer clips the settings out of bounds
 - Bug Fix: Fixed players being assigned multiple abilities
 - Modifier Fix: Fixed Volatile weirding out the spawn in sequence
 - Objectifier Fix: Fixed Corrupted being unable to kill players
 - Ability Fix: Fixed Multitasker not making the task windows transparent
 - Role + Ability Fix: Fixed the weird scaling of the guessing menu (it looped the size for every button added lmao, making it smaller with each possible guess added)
 - Role Fix: Fixed incorrect distance scaling for a lot of roles
 - Role Fix: Fixed Anarchist's kill cooldown not resetting correctly
 - Role Fix: Fixed Stalker's Stalk button not appearing
 - Role Fix: Fixed (Collider/Spellslinger/Stalker)-Rebels not having their buttons
 - Role Fix: Fixed Executioner being unable to doom players
 - Role Fix: Fixed Jester being unable to haunt players
 - Role Fix: Fixed Bomber and Enforcer bomb kills being marked as suicides
 - Role Fix: Enforcer can no longer place bombs on their teammates
 - Role Fix: Fixed Spellslinger not being able to see who they spelled
 - Role Fix: Fixed Tracker-Retributionist's Track button not updating
 - Role Fix: Retributionist's ability uses now update correctly
 - Potential Fix: Lag on the cosmetics screen and the cosmetic icons not having the hats in them
</details>
<details>
<summary>v0.2.0</summary>
 - New Roles: Monarch, Dictator [Crew (Sovereign)]
 - New Role: Spellslinger [Syndicate (Power)]
 - New Role: Collider [Syndicate (Killing)]
 - New Role: Stalker [Syndicate (Support)]
 - New Objectifier: Mafia
 - New Preset: Last Used - This preset always loads the settings you last played with in your previous Among Us session
 - Addition: 2 new nameplates
 - Buff: Added an optional buff for Arsonist to cremate ignited bodies
 - Buff: Cryomaniac's last killer kill ability has been given its own kill button
 - Nerf: The number of assassinations are now linked between all Assassins
 - Recode: Guesser and Assassin have been recoded to work like in The Other Roles
 - Rework: Mayor now works the same as its Town Of Salem counterpart
 - Rework: Politician has been fused with the original Mayor and was changed into an ability
 - Rework: Swapper is now an ability
 - Change: Changed the Shapeshifter color as it's too dark
 - Completion: Inspector results are now finished
 - Improvement: The Stalemate screen will now include any Neutral (Evil) role that has won
 - Improvement: Improved Chaos Drive assignment between players
 - Improvement: Improved how task completion is handled
 - Improvement: Improved how guesses are handled
 - Improvement: All actions now occur within their respective layers to hopefully reduce the lag
 - Improvement: Increased the size of the changelogs for better readability
 - Improvement: Improved how win conditions are handled to remove any underlying issues
 - Improvement: The main menu buttons are much closer now to look neater
 - Improvement: Swapper swaps don't need to be disabled anymore, they automatically update to swapping the new targets
 - Improvement: Decreased mod weight
 - Improvement: Assassinated players will have buttons generate on them for the Retributionist
 - Improvement: The default settings preset now auto updates
 - Improvement: Arrow targets are now reflected on maps
 - Critical Fix: Fixed being able to select targets through walls
 - Bug Fix: Fixed the wiki not giving the correct results for alignments
 - Bug Fix: Fixed string options occasionally freaking out
 - Bug Fix: Fixed nameplates not being displayed properly
 - Bug Fix: Fixed being able to call meetings when you can't
 - Bug Fix: Fixed chaos drive messages being repeated
 - Layer Fix: Fixed arrows not updating correctly
 - Role Fix: Fixed Retributionist buttons not appearing on the game screen or disappearing when getting assassinated
 - Role Fix: Fixed Sidekick being unable to kill
 - Role Fix: Fixed weird/inconsistent role changing interactions for Thief, Amnesiac and Shifter
 - Role Fix: Fixed Altruist and Necromancer revivals bugging out other players
 - Role Fix: Fixed Promoted Godfather cooldowns not resetting
 - Modifier Fix: Fixed Shy being assigned to Politicians when they cannot button
 - Ability Fix: Fixed Swapper buttons not appearing on the game screen
 - Revert: Reverted flashes back to the original Town Of Us style (but with still some modifications like in The Other Roles)
 - Removed: Removed the test features that I forgot to remove in the previous release
</details>
<details>
<summary>v0.1.0-dev1</summary>
 - Buff: Altruist can now revive players multiple times and sacrifices themselves on the last revive
 - Update: Updated Crowded to fix the double click issue
 - Update: Updated the role info from the recent updates in the in-game wiki
 - Improvement: Improved player targetting
 - Improvement: Settings now open to the last tab you were on, and keybinds for page jumping also work in there
 - Critical Fix: Fixed the Haunt button not working
 - Visual Fix: Fixed Poisoner having 2 buttons
 - Visual Fix: Fixed the sheer amount of ability buttons spawning (hopefully)
 - Visual Fix: Fixed buttons not disappearing when a meeting is called
 - Visual Fix: Fixed the spectate button looking a little cut off
 - Bug Fix: Fixed Vitals immediately closing upon opening
 - Bug Fix: Fixed staying zoomed out during a meeting
 - Bug Fix: Fixed the issue with targets not being higlighted at the times they are supposed to be
 - Bug Fix: Fixed settings looping incorrectly
 - Bug Fix: Fixed the issue with friendly fire
 - Objectifier Fix: Fixed Corrupted not ending the game when it is supposed to
 - Modifier Fix: Fixed continuously walking when being fake killed as Volatile
 - Ability Fix: Button Barry no longer bugs out a meeting for themselves
 - Role + Ability Fix: Fixed not being able to guess Crusader (for Assassin and Guesser)
 - Role Fix: Fixed Arsonist buttons not working
 - Role Fix: Fixed footprints not disappearing after a meeting for Detectives and Retributionist-Detectives
 - Role Fix: Fixed Rebel not being able to promote anyone
 - Role Fix: Fixed Disguiser not being able to disguise players
 - Role Fix: Fixed Syndicate roles having a kill button from the start
 - Role Fix: Fixed Troll screwing with the end game upon dying
 - Role Fix: Fixed Godfather-Janitors not dropping bodies when being warped/transported
 - Removed: Time Lord, Time Master and Drunkard because they break the game often, they'll return when I find a way to fix that
 - Removed: Removed the /setname chat command because it visually bugs out a lot (thanks to a certain group of people)
</details>
<details>
<summary>v0.0.4-dev2</summary>
 - Addition: Added a disconnect handler to hopefully reduce future errors
 - Improvement: Added support for multiple Swappers swapping
 - Visual Fix: Fixed Whisperer not being able to see how much someone is left to be converted for
 - Bug Fix: Fixed a random log spam caused by players leaving
 - Potential Fix: Lag spikes
 - Role Fix: Fixed Medic-Retributionists not actually protecting anyone
 - Role Fix: Fixed Guardian Angel having 2 buttons
 - Role Fix: An attempt at fixing Retributionist and Swapper meeting buttons
 - Role Fix: Fixed Politician-Rebels not having their votes refunded on assassinations
</details>
<details>
<summary>v0.0.4-dev1</summary>
 - Buff: Framer frames work on Vampire Hunters as well and framed Vigilantes can shoot anyone
 - Buff: Coroner-Retributionists also get meeting info like Coroners do
 - Buff: Diseased now applies to every cooldown for the rest of the round
 - Nerf: Reporting bodies no longer spreads douses or infections (because they often break the game rather than be helpful)
 - Nerf: Werewolf mauls can now be stopped if the target is on alert among other things
 - Update: Added changes from v4.0.4 and v4.0.5 of ToU-R
 - Change: Sheriff now sees flashes as indicator for guilt, rather than name changes
 - Recode: The mod was recoded a lot, so you know what that means! more bugs! (kill me please)
 - Improvement: Moved the status text from the intro screen to the too splash screen for better readability
 - Improvement: Improved how buttons work...for like the 20th time but I promise it gets better
 - Improvement: Made the mod a little more lightweight; fingers crossed it also fixes the issue of lag spikes
 - Improvement: Improved spawn code for Revealer, Phantom, Ghoul and Banshee
 - Improvement: Optimised how blackmails work
 - Critical Fix: Fixed not being able to enter codes or names
 - Visual Fix: Fixed player outlines appearing too thin
 - Visual Fix: Fixed the issue with custom buttons not disappearing during meetings
 - Visual Fix: Fixed the Use button having the Haunt sprite when dead
 - Bug Fix: Fixed revived players still having their death reports being sent
 - Bug Fix: Fixed players becoming Revealer, Banshee, Phantom or Ghouls after being revived
 - Bug Fix: Fixed votes being calculated incorrectly
 - Bug Fix: Fixed the end game screen showing the wrong role for players whose roles changed
 - Bug Fix: Fixed the log spam because of the Debugger window
 - Objectifier Fix: Fixed Allied sometimes breaking the game upon start
 - Role Fix: Fixed Godfather being unable to promote Consorts
 - Role Fix: Fixed Godfathers and Rebels being able to promote other Godfather, Rebels, Mafiosos and Sidekicks to...Mafioso or Sidekick
 - Role Fix: Fixed Ambusher/Enforcer-Godfathers from being unable to use their abilities
 - Role Fix: Fixed Bounty Hunter crashing the game on hunting their target
 - Role Fix: Fixed Bounty Hunter cooldowns not resetting upon usage
 - Role Fix: Fixed Bounty Hunter sometimes killing themselves
 - Role Fix: Fixed Cannibal not winning properly
 - Role Fix: Fixed Necromancer's resurrect sometimes crashing the game
 - Role Fix: Fixed the issue with Glitch staying permanently mimicked
</details>
<details>
<summary>v0.0.3-dev3</summary>
 - Addition: All Syndicate roles now have Chaos Drive abilities
 - Merge: Beamer has been merged into Warper
 - Buff: Consort can roleblock a player from anywhere (to differentiate it from Escort)
 - Buff: Godfather can now promote a Consort to eventually become the new Godfather
 - Buff: Rebel can now promote a Politician to eventually become the new Rebel
 - Buff: Retributionists can use the dead bodies as many times as they want and can also use Transporters and Escorts
 - Nerf: Chameleon and Retributionist now have a limited number of uses for their swoop ability
 - Nerf: Bomber now only detonates their latest bomb and the their Chaos Drive ability is to detonate all placed bombs
 - Nerf: Syndicate (Killing) roles not longer get an extra kill button, their kill abilities just become more powerful
 - Nerf: Retributionist does not know if a player is Crew and must attempt to use them to find out
 - Nerf: The Chaos Drive is no longer global and is instead assigned to a random player within a specific order, the global Chaos Drive is instead an option
 - Change: Concealer can now select a player to make them invisible and its chaos drive ability is its former ability
 - Change: Shapeshifter can now select 2 players to swap appearances and its chaos drive ability is its former ability
 - Change: Poisoner has been moved into the Syndicate (Killing) alignment
 - Improvement: Improved how Transporter and Beamer buttons work
 - Improvement: Moved the MCI keybinds into a tiny debug menu of their own (based off of Reactor Debugger code)
 - Improvement: Roles with shapeshifter menus can deselect players by prassing backspace
 - Improvement: Game announcements are heavily improved on
 - Improvement: Godfather and Rebel are now optimised and rewritten for less bugs
 - Improvement: Lobby size has been moved from a chat command to an actual setting
 - Improvement: Improved how conversions work to fix the occasional lag spike
 - Visual Fix: Fixed the issue where the Use button would appear to be deepfried
 - Visual Fix: Fixed the issue with revived players being able to see the spectate button
 - Bug Fix: Fixed Retributionist arrows sometimes popping up in lobby
 - Bug Fix: Potential fix for roles not spawning correctly on lower player counts
 - Bug Fix: Fixed being able to zoom out while in a meeting
 - Bug Fix: Fixed the issue with /setname not letting you change your name properly
 - Bug Fix: Fixed games sometimes crashing on end game
 - Bug Fix: Fixed the issue with multiple Overlord and Chaos Drive related messages being sent
 - Bug Fix: Fixed the issue with votes being doubled
 - Bug Fix: Fixed being able to whisper with alive players
 - Role Fix: Retributionist ability use limits actually work this time
 - Modifier Fix: Fixed Shy sometimes being able to button
 - Role Fix: Fixed the issue where Poisoner would occasionally directly kill their target
 - Role Fix: Prevented Jackal from spawning on lower player counts to prevent errors
 - Role Fix: Fixed Jackal not spawning with recruits
 - Role Fix: Fixed Framer framing players not registering for others
 - Removed: Gorgon because of the countless bugs it has
 - Removed: Custom Airship spawns because they break the game
 - Temporary Removal: Sound effects have been temporarily removed, they'll make a return when there's a sound for everything
</details>
<details>
<summary>v0.0.3-dev2</summary>
 - Critical Fix: Fixed the issue where the game would crash when someone was ejected
</details>
<details>
<summary>v0.0.3.1</summary>
 - Changes: Forgot to do a couple of things
</details>
<details>
<summary>v0.0.3</summary>
 - New Role: Politician [Syndicate (Power)]
 - New Role: Enforcer [Intruder (Killing)]
 - Merge: Agent has been merged into Operative, yet another soldier down :saluting_face:
 - Update: Updated the presets
 - Improvement: Task lists are finally finished
 - Improvement: Improved how certain functions work and are called
 - Improvement: Winning Guessers can now guess anyone after guessing their target's role
 - Bug Fix: Fixed the issue where an Amnesiac/Thief/Shifter changing roles reset everyone's cooldowns
 - Bug Fix: Fixed where bots would spawn in a public lobby after turning on persistance for robots in a local lobby
 - Bug Fix: Fixed the occasional log spam when assassinating which leads to lag spikes
 - Bug Fix: Fixed incorrect Torch assignment
 - Role Fix: Fixed the issue where shapeshift morphed multiple players into the same player
 - Role Fix: Fixed the issue where an assassinated player who is also blackmailed by the Godfather-Blackmailer would bug out
 - Role Fix: Fixed the issue with Murderer being unable to kill
 - Temporary Removal: Removed the second Spectate button till I figure out why you can't haunt
</details>
<details>
<summary>v0.0.2dev8.75</summary>
 - Critical Fix: Fixed the issue of the mod not being recognised by BepInEx
</details>
<details>
<summary>v0.0.2dev8.5</summary>
 - Removed: Removed a couple things I forgot to
</details>
<details>
<summary>v0.0.2dev8</summary>
 - Merge: Janitor and Undertaker were merged because Undertaker was a bit weak, farewell Undertaker, you will be remembered soldier :saluting_face:
 - Update: Updated the mod to v2023.3.28
 - Changes: Made minor changes to how emergency buttons can be called (credits to @whichtwix)
 - Changes: Transporter, Beamer and Glitch had minor changes on how their buttons work
 - Improvement: Reworked layer gen *again* to squash more bugs (and hopefully not create new ones)
 - Improvement: Improved RPC handling between players
 - Improvement: Migrated to using the vanilla Shapeshifter menu over chats for menu usage (thanks to @Zeo666 from All The Roles for help)
 - Improvement: Improved how roleblocks work internally by a bit
 - Improvement: Minor improvements to Amnesiac/Shifter/Thief interactions
 - Visual Fix: Fixed the issue where hidden player layers would still sometimes show up for players
 - Bug Fix: Fixed Button Barry cooldown not setting correctly if they don't have an Objectifier
 - Bug Fix: Fixed the issue with  and odd number of Lovers or Rivals spawning in
 - Bug Fix: Potential fix for ability buttons sometimes disabling with comms being turned on
 - Bug Fix: Potential fix for null errors regarding player layers
 - Role Fix: Fixed buttons not disappearing when changing roles
 - Role Fix: Potential fix for Godfather and Janitor being unable to move when dragging bodies
</details>
<details>
<summary>v0.0.2dev7</summary>
 - Addition: Some Lore
 - Addition: Added framework for translations, feel free to let me know when you want to add a language
 - Addition: 1 new nameplate
 - Code Change: Chat commands have been optimised
 - Code Change: The code was reorganised for better functionality
 - Improvement: The in-game wiki is now complete
 - Improvement: Improved how game cooldowns work by a bit (with lesser lag hopefully)
 - Improvement: Flashes were changed to inclulde text and work like The Other Roles flashes
 - Improvement: Optimised spawn code for Revealers, Phantoms, Ghouls and Banshees
 - Bug Fix: Fixed the win screens not showing up properly
 - Visual Fix: Fixed body outlines not disappearing if they are no longer the main target for roles who interact with dead bodies
 - Visual Fix: Fixed outros sometimes not loading correctly
 - Modifier Fix: Stopped giving Shy to someone with the Button Barry ability
 - Modifier Fix: Fixed the issue where the game nulls out if you kill a Diseased as a Werewolf or Vigilante
 - Ability Fix: Stopped giving Button Barry to someone who cannot button
 - Ability Fix: Stopped giving Torch to Neutrals or Neutral Killers when their respective lights settings are off
 - Role Fix: Fixed Revealers, Phantoms, Ghouls and Banshees sometimes not being able to leave vents
</details>
<details>
<summary>v0.0.2dev6</summary>
 - Critical Fix: Fixed buttons not working
 - Code Change: Changed how outros work
</details>
<details>
<summary>v0.0.2dev5</summary>
 - WARNING THIS UPDATE IS EXPERIMENTAL
 - Changes: Information now passes from one to another when an Amnesiac, Shifter or Thief changes roles
 - Code Change: The code was optimised again for better functionality (this means there might be new bugs :cri:)
 - Critical Fix: Fixed the issue where you could keep clikcing the button and it would perform its function, regardless of cooldown
 - Bug Fix: There were a lot of misc bug fixes, most annoying errors should be gone
 - Role Fix: Fixed the Glitch's mimic list not closing when they start mimicing
 - Role Fix: Fixed players crusaded by a Rebel-Crusader not killing upon interacting
</details>
<details>
<summary>v0.0.2dev4</summary>
 - Addition: Added a zooming button for dead players
 - Code Change: Changed how games end to squash bugs; past, present and future
 - Critical Fix: Fixed the the issue with turned Traitors and Fanatics not letting the game end
 - Critical Fix: Fixed games not ending correctly again
 - Role Fix: Fixed Juggernaut not being able to bypass protections after getting a certain number of kills
 - Role Fix: Clamped Juggernaut kill cooldown so that it does not become too op
</details>
<details>
<summary>v0.0.2dev3</summary>
 - New Role: Ambusher [Intruder (Killing)]
 - New Role: Crusader [Syndicate (Killing)]
 - New Role: Ghoul [Intruder (Utility)]
 - New Role: Beamer [Syndicate (Support)]
 - New Role: Banshee [Syndicate (Utility)]
 - New Role: Betrayer [Neutral (Proselyte)]
 - Addition: New keybind settings
 - Addition: Added a proper in-game wiki that can be accessed from /[type]info chat commands (WIP)
 - Addition: Added setting pages for easier browsing while in lobby
 - Addition: Added a semi functioning profanity filter to the /setname chat command
 - Update: Merged features from v4.0.3 of Town Of Us
 - Changes: Guardian Angel turned Survivor will have the same number of vests as the number of protects they had as Guardian Angels
 - Critical Fix: Fixed Crew roles not spawning under certain conditions
 - Visual Fix: All buttons now have consistent fonts
 - Visual Fix: Fixed the Report button lighting up incorrectly
 - Visual Fix: Potential fix for color flashes preventing reactor/oxygen flashes from showing up
 - Bug Fix: Fixed Button Barry being unable to spawn
 - Bug Fix: Fixed Neutral Killing roles from not being able to win properly, nor their proper win screens showing up
 - Bug Fix: Fixed sound effects not working the way they were supposed to, now some roles have an intro sound
 - Bug Fix: Fixed dynamic lobbies not allowing you to set the lobby max or min to 127 and 1 respectively
 - Role Fix: Fixed Executioner being unable to doom players
 - Role Fix: Fixed Glitch being unable to mimic players
 - Role Fix: Fixed Arsonist breaking the game
 - Role Fix: Fixed Transporter's button not lighting up or working properly
 - Role Fix: Fixed Guardian Angels randomly bugging out
 - Role Fix: Fixed winning Neutrals becoming Phantom when they are the first dead neutral
 - Role Fix: Fixed the limits on Necromancer not decreasing properly
 - Role Fix: Fixed Godfather-Undertaker being unable to drag bodies
 - Role Fix: Fixed Revealer and Phantom being unable to leave vents
 - Role Fix: Fixed Phantom and Revealer clicking themselves into losing
 - Role Fix: Fixed Guesser interactions with other meeting-based roles/abilities
 - Role Fix: Fixed Revealer not being able to reveal players upon finishing their tasks
 - Role Fix: Fixed Rebel-Poisoner from not being able to poison players
 - Removed: Vanilla setting tabs since their options have been moved into custom settings
 - Removed: BepInEx updater because...just no
</details>
<details>
<summary>v0.0.2dev2</summary>
 - Update: Updated to v2023.2.28
 - Addition: New colors
 - Addition: Better sabotages
 - Addition: Some more settings for game customisation
 - Addition: Version control - Having the wrong/modified versions of the mod compared to the host's will boot you out of the lobby
 - Improvement: Role buttons are now dynamic, meaning they appear and disappear based on certain actions
 - Improvement: Camouflages are now a little more fun now
 - Improvement: Ability uses now appear a little differently than how they used to originally
 - Buff: Torch ability has been fused with Lighter (mainly because I couldn't get Imp vision affected by lights to work)
 - Nerf: Retributionist can only use a dead player once
 - Critical Fix: Fixed Killing Only crashing the game for the host and nulling everyone else's roles
 - Critical Fix: Fixed All Any nulling roles when not a single role is turned on
 - Critical Fix: Fixed Modifiers not spawning in Classic or Custom mode
 - Critical Fix: Roleblocks actually work now!
 - Critical Fix: Fixed Intruder roles sometimes killing their targets upon using their abilities
 - Vanilla Fix: Fixed Report button being visible in lobby
 - Visual Fix: Fixed footprints sometimes being stupidly small
 - Visual Fix: Corrected some more color names
 - Visual Fix: Fixed haunting players not showing their objectifier symbols as being colored
 - Visual Fix: Fixed kill buttons not having text
 - Visual Fix: Fixed all nks winning not having a win screen
 - Visual Fix: Fixed Poisoner and Gorgon buttons being in the centre of the screen rather than the side
 - Role Fix: Fixed Bounty Hunter winning regardless of who they kill and then proceeding to be able to kill with no cooldown
 - Role Fix: Fixed Retributionist being unable to use a Mystic or Seer
 - Role Fix: Fixed Bomber placing bomb not reseting the detonate button with the linked cooldown setting being on
 - Role Fix: Fixed Retributionist unable to interact sometimes
 - Role Fix: Fixed Engineer and Retributionist-Engineer unable to fix sabotages on Skeld
 - Role Fix: Fixed Time Master's time freeze not stopping
 - Role Fix: Fixed Drunkard's invert controls staying inverted forever
 - ROle Fix: Fixed Time Master and Drunkard lag spiking the game when their abilities are active
 - Role Fix: Fixed Teleporter clipping through walls if they place their teleport points just right
 - Role Fix: Fixed Godfather and Rebel not being able to promote fellow team mates
 - Role Fix: Attempt no. 23 in fixing Glitch, here's to hoping it works
 - Objectifier Fix: Fixed Amnesiac/Thief Taskmaster turned Intruder/Syndicate being unable to do tasks to win
 - Objectifier Fix: Fixed the bug where alive Overlords did not win together
 - Objectifier Fix: Fixed Lovers and Rivals nulling when spawning in
 - Objectifier Fix: Fixed Traitor, Fanatic and Allied not winning with their selected factions
 - Code Change: Redid layer generation yet again to squash any hidden bugs + optimisation
 - Code Change: Player HUDs were redone again
 - Code Change: Disguiser, Morphling, Teleporter and Undertaker were recoded a little
 - Code Change: MCI was changed up a bit, the controls are redone and for some reason I'm yet to post controls smh
 - Removed: Retributionist can no longer use a Time Lord because of the countless bugs it creates
 - Removed: Lighter ability
</details>
<details>
<summary>v0.0.1dev19</summary>
 - Addition: New 'Default' preset for those who want to reset their settings to default
 - Addition: New 'Vanilla' gamemode, no idea why I added it so just roll with it
 - Addition: Added LevelImpostor compatibility
 - Improvement: Settings now loop (for example if the setting allows for the range 1 to 10, moving beyond 10 will result in the setting becoming 1)
 - Changes: None of the Neutral (Evil) roles end the game anymore, instead making them win brings about a penalty to players
 - Critical Fix: Another attempt at fixing games not ending properly
 - Critical Fix: MCI works now (albiet with some bugs of its own)! Controls will be coming later so until then, just press random buttons
 - Bug Fix: Presets can now be loaded properly
 - Bug Fix: Fixed changelogs appearing as "Fetching..." always
 - Bug Fix: Fixed the Corrupted kill button not lighting up properly
 - Bug Fix: Fixed the Speci vent not having the proper vent connections
 - Visual Fix: Fixed Detective footprints being absolutely huge
 - Role Fix: Fixed Retributionist mediate button not showing up
 - Role Fix: Fixed Poisoner returning an Intruder win rather than a Syndicate one
 - Removal: Removed the 'Footprint Size' settings because it's kind of useless
</details>
<details>
<summary>v0.0.1dev18.5</summary>
 - Critical Fix: Fixed games not ending correctly (hopefully)
</details>
<details>
<summary>v0.0.1dev18</summary>
 - New Ability: Ninja
 - Addition: Chaos drive abilities! Earlier it only added a kill button but now the Syndicate also has buffed abilities (only some roles have it as of right now)
 - Addition: Added an announcements and updates button for those who are too lazy to check the changelogs
 - Buff: Coroner can now autopsy dead bodies and compare them to players to find the killers
 - Changes: Seer now turns into Sheriff if all players whose roles can change/have changed are dead
 - Changes: The Main Menu was changed a bit
 - Improvement: Better task lists
 - Improvement: All the non-role layer details are finally complete
 - Improvement: Removed the "Sabotage and kill everyone" text that all Intruders have (to make way for the new task lists)
 - Bug Fix: Fixed the report button not showing for some reason
 - Bug Fix: Fixed Modifiers, Objectifiers and Abilities from sometimes not spawning
 - Bug Fix: Fixed Shy being assigned to roles who cannot button (like Mayor with the Mayor Button settings turned off)
 - Visual Fix: Fixed Janitor's clean button incorrectly lighting up
 - Visual Fix: Fixed the good recruit not showing up on your screen if you are the Serial Killer recruit
 - Objectifier Fix: Fixed Allied not winning with and displaying their decided faction
 - Objectifier Fix: Fixed Fanatic not changing factions properly
 - Role Fix: Fixed Bounty Hunter not being able to check for targets
 - Role Fix: Fixed Vampire Hunter not killing an Undead interactor
</details>
<details>
<summary>v0.0.1dev17</summary>
 - Addition: Custom nameplates and visors
 - Addition: New 'Casual' preset
 - Addition: You minimap now reflects the color of your role
 - Addition: Polus Reactor countdown can be changed now
 - Improvement: The end game screen shows your role under your name if you win
 - Internal: The code recieved yet another restructuring
 - Internal: The code for custom hats and role names was changed
 - Changes: Some buttons have their art now
 - Bug Fix: Fixed Seer not spawning when Godfather and Plaguebearer are on
 - Bug Fix: Fixed Seer not flashing when someone's role changes
 - Bug Fix: Fixed Revealers and Phantoms being unable to leave vents (WIP)
 - Bug Fix: Fixed Plaguebearer turning Pestilence not appearing on the end game summary
 - Bug Fix: Fixed Indomitable messing with role gen
 - Bug Fix: Fixed Inspector not being able to use their buttons
 - Visual Fix: Fixed Turquoise and Lilac having the wrong names
 - Visual Fix: Fixed role names not appearing under people's names if they are not the player themselves
 - Role Fix: Another attempt at unglitching Glitch
</details>
<details>
<summary>v0.0.1dev16</summary>
 - Addition: Added dynamic lobby size changing
 - Role Fix: Time Lord and Retributionist-Time Lord getting stuck when rewinding
 - Role Fix: Bounty Hunter, Guesser and Actor hints not being sent
 - Bug Fix: Fixed Time Lord and Retributionist-Time Lord rewind not reviving those who were poisoned by the Rebel-Poisoner
 - Bug Fix: Fixed being able to whisper to dead players and dead players being able to whisper to the living
 - Bug Fix: Fixed Ruthless messing with role gen
 - Bug Fix: Conceal not working
 - Visual Fix: Changed Necromancer's color because it was too similar to Impostor Red
 - Buff: Blackmailer and dead players can read whispers now (both of which are togglable)
 - Changes: Changed how do the Undead work
 - Changes: Changed the appearance of footprints
 - Update: Updated CrowdedMod code
 - Removed: Dampyr and Vampire
</details>
<details>
<summary>v0.0.1dev15</summary>
 - Addition: Preset settings (WIP).
 - Addition: Custom intro screen sounds (WIP).
 - Addition: Whispering system.
 - RoleFix: Fixed the issue with Bomber count messing with role gen.
 - Role Fix: Fixed Bounty Hunter not winning.
 - Visual Fix: Fixed Chameleon not appearing semi-invisible for themselves.
</details>
<details>
<summary>v0.0.1dev14</summary>
 - Fixed Necromancer not being able to kill.
 - Fixed the uses count on the Necromancer's buttons.
 - Fixed Necromancer's settings being misplaced.
 - Fixed some more cooldown issues (jeez they are one too many).
 - Bomber now works!
</details>
<details>
<summary>v0.0.1dev13.5</summary>
 - Fixed some cooldowns.
</details>
<details>
<summary>v0.0.1dev13</summary>
 - New Roles, Modifiers and Abilities!
 - Introducing Objectifiers! They are essentially secondary roles for you!
 - New Faction: The Syndicate! Can you survive the chaos they bring?
 - New Subfactions: Undead, Cabal, Reanimated and Sect! Stop them from over taking the mission!
 - New Features! Chat scrolling, chat commands and more!
 - Reworks, renames, buffs and nerfs! A lot of roles are different from the base mod, can you guess which ones were which?
 - New Settings with room for even more customisation!
 - New Mode: Custom! Allow multiple of the same roles to spawn, all at your discretion!
 - Modifiers, Abilities and Objectifiers can now also spawn in Killing Only mode!
 - New Game Modifiers! Spice up your game with settings that can drastically change the outcome of a game!
 - (Most) Vanilla settings have been integrated into the Custom Settings!
 - All Any is now truly All Any!
 - Brought back old and forgotten features from previous versions!
 - Improved features like better end game summaries!
 - Improved the win conditions of a lot of roles!
 - More QoL features!
 - A lot of internal code changes for smoother (and hopefully less buggy) game experiences!
</details>
</details>

-----------------------

# Installation

## Requirements 
- Among Us
- Steam or Epic Games

## Steam Guide
1. [Download](#releases) the Town of Us Reworked file corresponding to the installed Among Us version.
2. Go to your Steam library.
3. Right-click Among Us > click `Manage` > click `Browse local files`.
4. In the File Explorer, delete the entire `Among Us` folder.
5. Go back to your Steam library.
6. Right-Click Among Us > click `Properties...` > click `LOCAL FILES`.
7. Click on `VERIFY INTEGRITY OF GAME FILES...`.
8. Wait for Steam to download a clean version of Among Us.
9. Duplicate the new Among Us Folder.
10. Rename it to `Among Us - ToU-Rew`.
11. Double-click on the zip file you downloaded.
12. Drag all the files from the zip file in the new ToU folder.
13. Finally, launch `Among Us.exe` from that folder.

## Epic Games Guide
1. [Download](#releases) the Town of Us Reworked file corresponding to the installed Among Us version.
2. Go to your Epic Games library.
3. Find Among Us and click on the 3 dots `...` > click `Uninstall`.
4. Confirm you want to Uninstall Among Us.
5. In the Epic library, click on Among Us to install.
6. Copy the Folder Path.
7. Uncheck Auto-Update.
8. Click on Install.
9. Click Yes on the Windows pop up.
10. Paste the folder path in Windows search bar.
11. Click on Enter.
12. Copy or move the contents of the Town Of Us zip file into the AmongUs folder.
13. Finally, launch Among Us from Epic Games library.

## Linux Guide
1. Install Among Us via Steam
2. Download newest [release](https://github.com/AlchlcDvl/TownOfUsReworked/releases) and extract it to ~/.steam/steam/steamapps/common/Among Us
3. Enable `winhttp.dll` via the proton winecfg; [guide](https://docs.bepinex.dev/articles/advanced/steam_interop.html#open-winecfg-for-the-target-game)
4. Launch the game via Steam

## Results
Your game folder should look something like this.

![Image](./Images/Folder.png)

The first launch will take a while, so be patient if it doesn't launch immediately. If the mod still does not work, install [vc_redist](https://aka.ms/vs/16/release/vc_redist.x86.exe) and try again.

## Issues
If you still have issues installing Town of Us Reworked, you can join our [Discord](https://discord.gg/cd27aDQDY9) to receive help.

-----------------------

# Uninstallation

For Epic Games and Linux users, delete these files and you're good to go!

![Delete](./Images/Delete.png)

For Steam users, delete the `Among Us - ToU-Rew` that you created.

-----------------------

# Differences

This is yet another Town Of Us clone, I know. The main reason this mod exists is because I hated why despite me constantly telling the devs to remove the "The" in "The Glitch". After that I simply looked for help from [Det](https://github.com/FERTAILS) and then slowly learned from there.

> "*I simply taught him on how to make an empty role, and then he FUCKING EXPLODED*" - Det

Jokes aside, this mod has now become my own twist on what Town Of Us should have been like. ~~I have nearly 300 roles planned after all ;)~~

In this mod, we have :-
- New Roles, Abilities and Modifiers (which is a given)
- New Layer called Objectifiers
- New Faction: Syndicate
- New Mechanic: Subfactions where your alignment now belongs that that subfaction while wielding the role from your proper faction
- New Mechanic: Your final allegience at the end of the game is no longer guaranteed to stay the same at the start of the game
- A lot of new features
- Reworks for existing layers
- A lot of improvements and qol changes
- Vanilla settings were integrated into the mod along with a lot more customisation
- Improved win conditions so games actually end the way they are supposed to without being unfair
- Brought back old and forgotten features from older versions
- Internal code changes so that it works smoothly/looks nicer
- In-game wiki to get info about every type of layers
- Lore

-----------------------

# Common Settings
## Each role, modifier, objectifier and ability has these settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Role/Modifier/Objectifier/Ability | The percentage probability of the specified type appearing in game | Percentage | 0% |
| Role/Modifier/Objectifier/Ability Count | How many instances of the specified type should spawn in Custom Mode, for Lovers and Rivals it's the number of pairs | Number | 1 |
| Role/Modifier/Objectifier/Ability Is Unique In All Any | Dictates whether only one of the specified type should spawn in All Any | Toggle | Off |

## Each alignment has these settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Max Count | Dictates the maximum number of roles from the alignment will spawn | Number | 5 |
| Min Count | Dictates the minimum number of roles from the alignment will spawn | Number | 5 |

## Each faction has these settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Max Role Count | Dictates the maximum number of roles from the faction will spawn | Number | 5 |
| Min Role Count | Dictates the minimum number of roles from the faction will spawn | Number | 5 |
| Faction Can Vent | Dictates whether members of this faction can vent, this is overriden by the specific venting settings for the roles | Toggle | True if not Crew |
| Custom Faction Colors | Purely a visual thing, dictates whether players want each role color to only match the color of their respective factions | Toggle | True |
| Vision | How far can members of the faction see | Factor | 1x for Crew, 2x for Intruders and Syndicate, 1.5x for Neutrals |

-----------------------

# Player Layers

# Crew Roles

![Crew](./Images/Crew.png)

Each member has a special ability which determines who’s who and can help weed out the evils. The main theme of this faction is deduction and goodwill. This faction is an uninformed majority meaning they make up most of the players and don't who the other members are. The Crew can do tasks which sort of act like a timer for non-Crew roles.

### Crew Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Common Tasks | How many common tasks must be assigned | Number | 2 |
| Long Tasks | How many long tasks must be assigned | Number | 1 |
| Short Tasks | How many short tasks must be assigned | Number | 4 |

## Altruist
### Alignment: Crew (Protective)

The Altruist is capable of reviving dead players. After a set period of time, the player will be resurrected, if the revival isn't interrupted. Once a player is revived, all evil players will be notified of the revival and will have an arrow pointing towards the revived player. Once the Altruist uses up all of their ability charges, they sacrifice themselves on the last use of their ability.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Altruist Revive Duration | The time it takes for the Altruist to revive a dead body | Time | 10s |
| Target's Body Disappears On Beginning Of Revive | Whether the dead body of the player the Altruist is reviving disappears upon revival | Toggle | False |

## Chameleon
### Alignment: Crew (Support)

The Chameleon can go invisible to stalk players and see what they do when no one is around.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Swoop Cooldown | The cooldown on the Chameleon's Swoop button | Time | 25s |
| Swoop Duration | The duration of the Chameleon's Swoop | Time | 10s |
| Swoop Count | The number of times the Chameleon can use Swoop | Number | 5 |

## Coroner
### Alignment: Crew (Investigative)

The Coroner gets an alert when someone dies. On top of this, the Coroner briefly gets an arrow pointing in the direction of the body. They can autopsy bodies to get some information. They can then compare that information with players to see if they killed the body or not. The Coroner also gets a body report from the player they reported. The report will include the cause and time of death, player's faction/role, the killer's faction/role and (according to the settings) the killer's name.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Dead Body Arrow Duration | The duration of the arrows pointing to the bodies | Time | 0.1s |
| Coroner Gets Roles | This dictates whether the Coroner gets the killer's and body's role, otherwise only their factions are revealed | Toggle | False |
| Coroner Gets Killer's Name | This dictates whether the Coroner gets the killer's name from the report | Toggle | False |
| Coroner Gets Killer's Name Under | This dictates how old must a body be for the Coroner to get the killer's name | Time | 1s |
| Compare Cooldown | The cooldown on the Coroner's Compare button | Time | 25s |
| Autopsy Cooldown | The cooldown on the Coroner's Autopsy button | Time | 25s |
| Compare Limit | The number of times the Coroner can compare players to a body before it goes stale | Number | 5 |

## Crewmate
### Alignment: Crew (Utility)

Just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.

## Detective
### Alignment: Crew (Investigative)

The Detective can examine other players for bloody hands. If the examined player has killed recently, the Detective will be alerted about it. The Detective can also see the footprints of players. All footprints disappear after a set amount of time and only the Detective can see them.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Examine Cooldown | The cooldown on the Detective's Examine button | Time | 25s |
| Bloody Duration | How long players remain bloody after a kill | Time | 10s |
| Footprint Interval | The time interval between two footprints | Time | 0.15s |
| Footprint Duration | The amount of time that the footprint stays on the ground for | Time | 10s |
| Anonymous Footprint | When enabled, all footprints are grey instead of the player's colors | Toggle | False |

## Dictator
### Alignment: Crew (Sovereign)

The Dictator has no active ability aside from revealing themselves as the Dictator to all players. When revealed, in the next meeting they can pick up to 3 players to be ejected. All 3 players will be killed at the end of the meeting, along with the chosen 4th player everyone else votes on (if any). If any of the 3 killed players happens to be Crew, the Dictator goes out the airlock with them. After that meeting, the Dictator has no post ejection ability.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Dictator Cannot Reveal Round One | Self explanatory | Toggle | True |
| Dictator Can Dictate After Voting | Whether the Dictator can continue selecting players after voting | Toggle | True |
| Dictator Can Button | Whether the Monarch can call a meeting | Toggle | True |

## Engineer
### Alignment: Crew (Support)

The Engineer can fix sabotages from anywhere on the map. They can also use vents to get across the map easily.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Fix Cooldown | The cooldown on the Engineer's Fix button | Time | 25s |
| Fix Count | How many times can the Engineer fix sabotages | Number | 5 |

## Escort
### Alignment: Crew (Support)

The Escort can roleblock players and prevent them from doing anything for a short while.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Roleblock Cooldown | The cooldown on the Escort's Roleblock button | Time | 25s |
| Roleblock Duration | The duration of the Escort's Roleblock | Time | 10s |

## Inspector
### Alignment: Crew (Investigative)

The Inspector can check players for their roles. Upon being checked, the targets' names will be updated to give a list of what roles could the target possibly be.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Inspect Cooldown | The cooldown on the Inspector's Inspect button. | Time | 25s |

## Mayor
### Alignment: Crew (Sovereign)

The Mayor has no active ability aside from being able to reveal themselves as the Mayor to other players. Upon doing so, their vote counts as extra.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Mayor Votes Count As | The additional number of votes that are added to the Mayor's vote | Number | 2 |
| Mayor Cannot Reveal Round One | Self explanatory | Toggle | True |
| Mayor Can Button | Whether the Mayor can call a meeting | Toggle | True |

## Medic
### Alignment: Crew (Protective)

The Medic can give any player a shield that will make them immortal until the Medic is dead. A shielded player cannot be killed by anyone, unless it's a suicide.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Show Shielded Player | Who should be able to see who is Shielded | String | Self |
| Who Gets Murder Attempt Indicator | Who will receive an indicator when someone tries to Kill them | String | Medic |
| Shield Breaks On Murder Attempt | Whether the Shield breaks when someone attempts to Kill them | Toggle | True |

## Medium
### Alignment: Crew (Investigative)

The Medium can mediate to be able to see ghosts. If the Medium uses this ability, the Medium and the dead player will be able to see each other and communicate from beyond the grave!

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Mediate Cooldown | The cooldown on the Medium's Mediate button | Time | 25s |
| Reveal Appearance Of Mediate Target | Whether the Ghosts will show as themselves, or camouflaged | Toggle | True |
| Reveal The Medium To The Mediate Target | Whether the ghosts can see that the Medium is the Medium | Toggle | True |
| Who Is Revealed | Which players are revealed to the Medium | String | Oldest Dead |

## Monarch
### Alignment: Crew (Sovereign)

The Monarch can appoint players as knights. When the next meeting is called, all knighted players will be announced. Knighted players will have their votes count as extra.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Knighting Cooldown | The cooldown on the Monarch's Knighting button | Time | 10s |
| Monarch Cannot Knight Round One | Self explanatory | Toggle | True |
| Knighted Votes Count As | The additional number of votes that are added to a knighted player's vote | Number | 1 |
| Knighted Count | The number of times the Monarch can knight players | Number | 1 |
| Monarch Can Button | Whether the Monarch can call a meeting | Toggle | True |
| Knights Can Button | Whether knighted players can call a meeting | Toggle | True |

## Mystic
### Alignment: Crew (Auditor)

The Mystic only spawns when there is at least one Neutral (Neophyte) role present in the game. Whenever someone's subfaction is changed, the Mystic will be alerted about it. The Mystic can also investigate players to see if their subfactions have been changed. If the target has a different subfaction, the Mystic's screen will flash red, otherwise it will flash green. It will not, however, work on the Neutral (Neophyte) roles themselves so even people who flashed green might be a converter. Once all subfactions are dead, the Mystic becomes a Seer.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Reveal Cooldown | The cooldown on the Medium's Investigate button | Time | 10s |

## Operative
### Alignment: Crew (Investigative)

The Operative can place bugs around the map. When players enter the range of the bug, they trigger it. In the following meeting, all players who triggered a bug will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the bug, nor what role a specific player is. The Operative also gains more information when on Admin Table and on Vitals. On Admin Table, the Operative can see the colors of every person on the map. When on Vitals, the Operative is shown how long someone has been dead for.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Bug Cooldown | The cooldown on the Operative's Bug button | Time | 25s |
| Minimum Amount Of Time Required For Bug To Trigger | How long a player must stay in the bug for it to trigger | Time | 0s |
| Bugs Removed Each Round | Whether the Operative's bugs are removed after each meeting | Toggle | True |
| Max Bugs | The number of bugs they can place in a game | Number | 5 |
| Bug Range | The size of each trap | Factor | 0.25x |
| Minimum Number Of Roles Required To Trigger Bug | The number of players that must enter the bug for it to be triggered | Number | 1 |
| Who Sees Dead Bodies On Admin | Which players see dead bodies on the admin map | String | Nobody |

## Retributionist
### Alignment: Crew (Support)

The Retributionist can resurrect dead crewmates. During meetings, the Retributionist can select who they are going to ressurect and use for the following round from the dead. They can choose to use each dead players only once. It should be noted the Retributionist can not use all Crew roles and cannot use any Non-Crew role. The cooldowns, limits and everything will be set by the settings for their respective roles.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Retributionist Can Mimic After Voting | Dictates whether the Retributionist can continue reviving players after voting | Toggle | True |
| Max Uses | How many times can the Retributionist use the abilities of roles with limited uses on their abilities | Number | 5 |

## Revealer
### Alignment: Crew (Utility)

The Revealer can reveal evils if they finish all their tasks. Upon finishing all of their tasks, Intruders, Syndicate and sometimes Neutrals are revealed to alive Crew after a meeting is called. However, if the Revealer is clicked they lose their ability to reveal evils and are once again a normal ghost.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| When Revealer Can Be Clicked | The amount of tasks remaining when the Revealer Can Be Clicked | Number | 5 |
| Tasks Remaining When Revealed | The amount of tasks remaining when evils are alerted that the Revealer is nearly finished | Number | 1 |
| Revealer Reveals Neutral Roles | Whether the Revealer also Reveals Neutral Roles | Toggle | False |
| Revealer Reveals Crew Roles | Whether the Revealer also Reveals Crew Roles | Toggle | False |
| Revealer Reveals Exact Roles | Whether the Revealer also Reveals all Roles | Toggle | False |
| Who can Click Revealer | Whether even other Crewmates can click the Revealer | String | All |

## Seer
### Alignment: Crew (Investigative)

The Seer only spawns if there are roles capable of changing their initial roles. The Seer can investigate players to see if their role is different from what they started out as. If a player's role has been changed, the Seer's screen will flash red, otherwise it will flash green. This, however, does not work on those whose subfactions have changed so those who flashed green might still be evil. If all players capable of changing or have changed their initial roles are dead, the Seer becomes a Sheriff.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Seer Cooldown | The cooldown on the Seer's Investigate button | Time | 25s |

## Sheriff
### Alignment: Crew (Investigative)

The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a role is Good or Evil. A player's name will change color according to their results.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Interrogate Cooldown | The cooldown on the Sheriff's Interrogate button | Time | 25s |
| Neutral Evils Show Up As Evil | Neutral Evil roles show up as Red | Toggle | False |
| Neutral Killing Show Up As Evil | Neutral Killing roles show up as Red | Toggle | False |

## Shifter
### Alignment: Crew (Support)

The Shifter can swap roles with someone, as long as they are Crew. If the shift is unsuccessful, the Shifter will die.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Shift Cooldown | The cooldown on the Shifter's Shift button | Time | 25s |
| Shifted Becomes | Dictates what the shift target becomes after getting shifted | String | Shifter |

## Tracker
### Alignment: Crew (Investigative)

The Tracker can track other during a round. Once they track someone, an arrow is continuously pointing to them, which updates in set intervals.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Arrow Update Interval | The time it takes for the arrow to update to the new location of the tracked player | Time | 5s |
| Track Cooldown | The cooldown on the Tracker's Track button | Time | 25s |
| Tracker Arrows Reset Each Round | Whether Tracker Arrows are removed after each meeting | Toggle | False |
| Max Tracks | The number of new people they can track each round | Number | 5 |

## Transporter
### Alignment: Crew (Support)

The Transporter can swap the locations of two players at will. Players who have been transported are alerted with a blue flash on their screen.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Transport Cooldown | The cooldown on the Transporter's Transport ability | Time | 25s |
| Max Transports | The amount of times the Transport ability can be used | Number | 5 |
| Transporter Can Transport Themselves | Self descriptive | Toggle | False |

## Vampire Hunter
### Alignment: Crew (Auditor)

The Vampire Hunter only spawns if there are Undead in the game. They can check players to see if they are an Undead. When the Vampire Hunter finds them, the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a Vigilante.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Stake Cooldown | The cooldown on the Vampire Hunter's Stake button. | Time | 5s |

## Veteran
### Alignment: Crew (Killing)

The Veteran can go on alert. Anyone who interacts with a Veteran on alert will be killed by the Veteran in question.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Alert Cooldown | The cooldown on the Veteran's Alert button. | Time | 5s |
| Alert Duration | The duration of the Veteran's Alert | Time | 25s |
| Max Alerts | The number of times the Veteran can alert throughout the game | Number | 5 |

## Vigilante
### Alignment: Crew (Killing)

The Vigilante can kill. However, if they kill someone they shouldn't, they instead die themselves.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Misfire Kills The Target | Whether the target is killed if the Vigilante misfires | Toggle | False |
| Vigilante Can Kill Again If Target Was Innocent | Whether the Vigilante can continue shooting even after getting a shot wrong | Toggle | False |
| Vigilante Cannot Shoot Round One | Self descriptive | Toggle | True |
| How Is The Vigilante Notified Of Their Target's Innocence | Whether the Vigilante is notified of their target's innocent upon misfire | String | Never |
| How Does Vigilante Die | Dictates how does the Vigilante die, should they kill or attempt to kill someone they shouldn't | String | Immediately |
| Shoot Cooldown | The cooldown on the Vigilante's kill button | Time | 25s |
| Max Bullets | The number of times the Vigilante can shoot players throughout the game | Number | 5 |

# Neutral Roles

![Neutral](./Images/Neutral.png)

Each member of this faction has their own unique way to win, seperate from the other roles in the same faction. The main theme of this faction is free for all. This faction is an uninformed minority of the game, meaning they make up a small part of the crew while not knowing who the other members are. Each role is unique in its own way, some can be helpful, some exist to destroy others and some just exist for the sake of existing.

### Neutral Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Lights Affect Neutral Vision | Whether the lights sabotage affects Neutrals | Toggle | True |
| Neutrals Together, Strong | Whether Neutrals can win together | String | Never |
| Vigilante Kills Neutral Benigns | Whether Neutral Benign roles are considered evil for the Vigilante | Toggle | True |
| Neutral Killers Have Intruder Vision | Whether Neutral (Killing) roles have higher vision or not | Toggle | False |
| Neutral Killers Know Each Other | If Neutrals can win together, this settings lets Neutral Killers know of each other | Toggle | False |

## Actor
### Alignment: Neutral (Evil)
### Win Condition: Get guessed as a role in their target role list

The Actor gets a list of roles at the start of the game. This list of roles depends on which roles are present in the game so that it's easier for the Actor to pretend with certain events. The Actor must pretend to be and get guessed as one of the roles in order to win.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Actor Can Button | Whether the Actor call a meeting | Toggle | True |
| Actor Can Hide In Vents | Whether the Actor Can Vent | Toggle | False |
| Actor Can Switch Vents | Whether the Actor Can Switch while in Vents | Toggle | False |
| Vigilante Kills Actor | Whether the Vigilante is able to kill the Actor | Toggle | False |

## Amnesiac
### Alignment: Neutral (Benign)
### Win Condition: Find a dead body, take their role and then win as that role

The Amnesiac is essentially roleless and cannot win without remembering the role of a dead player.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Amnesiac Gets Arrows To Dead Bodies | Self descriptive | Toggle | False |
| Arrow Appearance Delay | The delay of the arrows appearing after the person died | Time | 5s |
| Amnesiac Can Hide In Vents | Decides whether the Amnesiac can vent | Toggle | False |
| Amnesiac Can Switch Vents | Decides whether the Amnesiac can switch while in vents | Toggle | True |

## Arsonist
### Alignment: Neutral (Killing)
### Win Condition: Ignite anyone who opposes them

The Arsonist can douse other players with gasoline. After dousing, the Arsonist can choose to ignite all doused players which kills all doused players at once. Doused players have an orange Ξ next to their names.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Douse Cooldown | The cooldown on the Arsonist's Douse button | Time | 25s |
| Ignite Cooldown | The cooldown on the Arsonist's Ignite button | Time | 25s |
| Ignite Cooldown Removed When Arsonist Is Last Killer | Decides whether the Arsonist's Ignite cooldown is reduced to 0s if they happen to be the last killer alive | Toggle | False |
| Ignition Ignites All Doused Players | One Arsonist igniting ignites all doused players of other Arsonists as well | Toggle | False |
| Douse And Ignite Cooldowns Are Linked | Decides whether the Arsonist's cooldowns are linked so that dousing resets ignition and vice versa | Toggle | False |
| Ignition Cremates Bodies | Ignited players have their bodies burnt away, leaving behind unreportable ash | Toggle | False |
| Arsonist Can Vent | Decides whether the Arsonist can vent | Toggle | True |

## Betrayer
### Alignment: Neutral (Proselyte)
### Win Condition: Kill anyone who opposes the faction they defected to

The Betrayer is a simple killer, who turned after a turned Traitor/Fanatic was the only member of their new faction remaning. This role does not spawn directly.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Kill Cooldown | The cooldown on the Betrayer's Kill button | Time | 25s |
| Betrayer Can Vent | Decides whether the Betrayer can vent | Toggle | True |

## Bounty Hunter
### Alignment: Neutral (Evil)
### Win Condition: Find and kill thier target

The Bounty Hunter is assigned a target as the start of the game. Every meeting, the Bounty Hunter is given clue to who their target might be. They do not know who the target is and must find them via a series of clues and limited guesses. Upon finding their target within the set amount of guesses, the guess button becomes a kill button. The Bounty Hunter's target always knows that there is a bounty on their head. If the Bounty Hunter is unable to find their target within the number of guesses or their target dies not by the Bounty Hunter's hands, the Bounty Hunter turns into a Troll. The target has a red Θ next to their names.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Guess Cooldown | The cooldown on the Bounty Hunter's Guess button | Time | 25s |
| Max Guesses | The number of times the Bounty Hunter can try to guess players as their target before losing | Number | 5 |
| Bounty Hunter Can Vent | Whether the Bounty Hunter Can Vent | Toggle | False |
| Vigilante Kills Bounty Hunter | Whether the Vigilante is able to kill the Bounty Hunter | Toggle | False |

## Cannibal
### Alignment: Neutral (Evil)
### Win Condition: Eat a certain number of bodies

The Cannibal can eat the body which wipes away the body, like the Janitor.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Eat Cooldown | The cooldown on the Cannibal's eat button | Time | 25s |
| Bodies Needed To Win | Self descriptive | Number | 5 (or changes to half the lobby size if not enough players) |
| Cannibal Can Vent | Decides whether the Cannibal can vent | Toggle | False |
| Cannibal Gets Arrows | Whether the Cannibal has arrows pointing to dead bodies | Toggle | False |
| Time After Death Arrow Appears | The delay of the arrows appearing after the person died | Time | 5s |
| Vigilante Kills Cannibal | Whether the Vigilante is able to kill the Cannibal | Toggle | False |

## Cryomaniac
### Alignment: Neutral (Killing)
### Win Condition: Freeze anyone who opposes them

The Cryomaniac can douse in coolant and freeze players similar to the Arsonist's dousing in gasoline and ignite. Freezing players does not immediately kill doused targets, instead when the next meeting is called, all currently doused players will die. When the Cryomaniac is the last killer or when the final number of players reaches a certain threshold, the Cryomaniac can also directly kill. Doused players have a purple λ next to their names.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Douse Cooldown | The cooldown on the Cryomaniac's Douse button | Time | 25s |
| Freezing Freezes All Doused Players | One Cryomaniac freezing freezes all doused players of other Cryomaniacs as well | Toggle | False |
| Cryomaniac Can Vent | Decides whether the Cryomaniac can vent | Toggle | False |

## Dracula
### Alignment: Neutral (Neophyte)
### Win Condition: Convert or kill anyone who can oppose them

The Dracula is the only Undead that spawns in. The Dracula is the leader of the Undead who can convert others into Undead. If the target cannot be converted, they will be attacked instead. The Dracula must watch out for the Vampire Hunter as attempting to convert them will cause the Vampire Hunter to kill the Dracula.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Bite Cooldown | The cooldown on the Dracula's bite button | Time | 25s |
| Dracula Can Vent | Decides whether the Dracula can vent | Toggle | False |
| Undead Can Vent | Decides whether the Undead can vent, overriding their role's vent settings | Toggle | False |
| Alive Undead Count | Limits the number of Undead that can be alive, attempting to convert player after this limit has been reached will kill the target player | Number | 3 |

## Executioner
### Alignment: Neutral (Evil)
### Win Condition: Live (or die according to the settings) to see their target get ejected

The Executioner has no abilities and instead must use gas-lighting techniques to get their target ejected. The Executioner's target, by default, is always non-Crew Sovereign Crew. Once their target is ejected, the Executioner can doom those who voted for their target. If their target dies before ejected, the Executioner turns into a Jester. Targets have a grey § next to their names.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Doom Cooldown | The cooldown on the Executioner's Guess button | Time | 25s |
| Doom Count | The number of times the Executioner can doom those who voted for their target | Number | 5 (or lower if the number of people who voted for the target is lesser) |
| Executioner Can Button | Whether the Executioner can call a meeting | Toggle | True |
| Executioner Can Hide In Vents | Whether the Executioner Can Vent | Toggle | False |
| Executioner Can Switch Vents | Whether the Executioner Can Switch while in Vents | Toggle | False |
| Target Knows Executioner Exists | Whether the Executioner's target knows that they have an Executioner for them | Toggle | False |
| Executioner Knows Target's Role | Whether the Executioner knows their target's role | Toggle | False |
| Target Ejection Reveals Existence Of Executioner | Decides if the target is ejected, it will reveal the fact that they were an Executioner's target | Toggle | False |
| Executioner Can Have Intruder Targets | Decides if the Executioner can have an Intruder as a target | Toggle | False |
| Executioner Can Have Syndicate Targets | Decides if the Executioner can have a Syndicate as a target | Toggle | False |
| Executioner Can Have Neutral Targets | Decides if the Executioner can have a Neutral as a target | Toggle | False |
| Executioner Can Win After Death | Decides if the Executioner can still win if their target has been ejected after they died | Toggle | False |
| Vigilante Kills Executioner | Whether the Vigilante is able to kill the Executioner | Toggle | False |

## Glitch
### Alignment: Neutral (Killing)
### Win Condition: Neutralise anyone who oppose them

The Glitch can hack players, resulting in them being unable to do anything for a set duration or they can also mimic someone, which results in them looking exactly like the other person. The Glitch can kill normally.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Mimic Cooldown | The cooldown of Glitch's Mimic button | Time | 25s |
| Mimic Duration | How long Glitch can Mimic a player | Time | 10s |
| Hack Cooldown | The cooldown of Glitch's Hack button | Time | 25s |
| Hack Duration | How long Glitch can Hack a player | Time | 10s |
| Neutralise Cooldown | The cooldown of Glitch's Neutralise button | Time | 25s |
| Glitch Can Vent | Whether Glitch can Vent | Toggle | False |

## Guardian Angel
### Alignment: Neutral (Benign)
### WIn Condition: Have your target live to see the end of the game

The Guardian Angel more or less aligns themselves with the faction of their target. The Guardian Angel will win with anyone as long as their target lives to the end of the game, even if their target loses. If the Guardian Angel's target dies, they become a Survivor. Targets have a white ★ and a white η when being protected next to their names.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Protect Cooldown | The cooldown on the Guardian Angel's Protect button | Time | 25s |
| Protect Duration | How long The Guardian Angel's Protect lasts | Time | 10s |
| Cooldown Reset When Protected | The attackers kill cooldown after they attacked the protected target | Time | 2.5s |
| Max Protects | The amount of times the Protect ability can be used | Number | 5 |
| Show Protected Player | Who should be able to see who is Protected | String | Self |
| Target Knows Guardian Angel Exists | Whether the Guardian Angel's Target knows they have a Guardian Angel | Toggle | False |
| Guardian Angel Can Protect After Death | Whether the Guardian Angel can continue to protect their target if they happen to die | Toggle | False |
| Guardian Angel Knows Target's Role | Whether the Guardian Angel knows their target's role | Toggle | False |
| Guardian Angel Can Hide In Vents | Whether the Guardian Angel Can Vent | Toggle | False |
| Guardian Angel Can Switch Vents | Whether the Guardian Angel Can Switch while in Vents | Toggle | False |

## Guesser
### Alignment: Neutral (Evil)
### Win Condition: Guess your target's role

The Guesser has no abilities aside from guessing only their target. Every meeting, the Guesser is told a hint regarding their target's role. Targets have a beige π next to their names.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Guesser Can Button | Whether the Guesser can call a meeting | Toggle | True |
| Guesser Can Hide In Vents | Whether the Guesser Can Vent | Toggle | False |
| Guesser Can Switch Vents | Whether the Guesser Can Switch while in Vents | Toggle | False |
| Target Knows Guesser Exists | Whether the Guesser's target knows that they have a Guesser | Toggle | False |
| Guesser Can Guess Multiple Times | Whether the Guesser's can attempt to guess their target multiple times in a single meeting | Toggle | False |
| Max Guesses | The number of times the Guesser can try to guess their target before losing | Number | 5 |
| Guesser Can Guess After Voting | Whether the Guesser's can continue guessing their target after voting | Toggle | False |
| Vigilante Kills Guesser | Whether the Vigilante is able to kill the Guesser | Toggle | False |

## Jackal
### Alignment: Neutral (Neophyte)
### Win Condition: Bribe the crew into joining your side and eliminate any threats

The Jackal is the leader of the Cabal. They spawn in with 2 recruits at the start of the game. One of the recruits is the "good" one, meaning they are Crew. The other is the "evil" recruit, who can be either Intruder, Syndicate or Neutral (Killing). When both recruits die, the Jackal can then recruit another player to join the Cabal and become the backup recruit. If the target happens to be a member of a rival subfaction, they will be attacked instead.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Recruit Cooldown | The cooldown on the Jackal's Recruit button | Time | 25s |
| Jackal Can Vent | Whether the Jackal Can Vent | Toggle | False |
| Recruits Can Vent | Whether the Jackal's recruits Can Vent, overriding their role's vent settings | Toggle | False |

## Jester
### Alignment: Neutral (Evil)
### Win Condition: Get ejected

The Jester has no abilities and must make themselves appear to be evil to the Crew and get ejected. After getting ejected, the Jester can haunt those who voted for them, killing them from beyond the grave.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Jester Can Button | Whether the Jester can call a meeting | Toggle | True |
| Haunt Cooldown | The cooldown on the Jester's Guess button | Time | 25s |
| Haunt Count | The number of times the Jester can haunt those who voted for them | Number | 5 (or lower if the number of people who voted for the Jester is lesser) |
| Jester Can Hide In Vents | Whether the Jester Can Vent | Toggle | False |
| Jester Can Switch Vents | Whether the Jester Can Switch while in Vents | Toggle | False |
| Ejection Reveals Existence Of Jester | Decides if the Jester is ejected, it will reveal the fact that they were a Jester | Toggle | False |
| Vigilante Kills Jester | Whether the Vigilante is able to kill the Jester | Toggle | False |

## Juggernaut
### Alignment: Neutral (Killing)
### Win Condition: Kill all non-Neutral Benign roles

The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill cooldown no longer decreases and instead gives them other buffs, like bypassing protections.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Assault Cooldown | The initial cooldown on the Juggernaut's Assault button | Time | 25s |
| Assault Bonus | By how much the Juggernaut's assault cooldown decreases | Time | 5s |
| Juggernaut Can Vent | Toggles the Juggernaut's ability to vent | Toggle | False|

## Murderer
### Alignment: Neutral (Killing)
### Win Condition: Kill all non-Neutral Benign roles

The Murderer is a simple Neutral Killer with no special abilities.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Murder Cooldown | The cooldown on the Murderer's Murder button | Time | 25s |
| Murderer Can Vent | Toggles the Murderer's ability to vent | Toggle | False |

## Necromancer
### Alignment: Neutral (Neophyte)
### Win Condition: Bring the undead to your side

The Necromancer is essentially an evil Altruist. They can revive dead players and make them join the Necromancer's team, the Reanimated. There is a limit to how many times can the Necromancer can kill and revive players.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Resurrect Cooldown | The cooldown on the Necromancer's Resurrect button | Time | 25s |
| Resurrect Cooldown Increase | The increase on the Necromancer's Resurrect cooldown with each use | Time | 5s |
| Resurrect Count | The number of times the Necromancer can resurrect | Number | 5 |
| Kill Cooldown | The cooldown on the Necromancer's kill button | Time | 25s |
| Kill Cooldown Increase | The increase on the Necromancer's kill cooldown with each use | Time | 5s |
| Kill Count | The number of times the Necromancer can kill | Number | 5 |
| Necromancer Can Vent | Toggles the Necromancer's ability to vent | Toggle | False |
| Kill And Resurrect Cooldowns Are Linked | Decides whether the Necromancer's cooldowns are linked so that killing resets resurrection and vice versa | Toggle | False |
| Resurrect Duration | The time it takes for the Necromancer to resurrect a dead body | Time | 10s |
| Target's Body Disappears On Beginning Of Resurrect | Whether the dead body of the player the Necromancer is resurrecting disappears upon resurrection | Toggle | False |
| Reanimated Can Vent | Whether the Necromancer's Reanimated Can Vent, overriding their role's vent settings | Toggle | False |

## Pestilence
### Alignment: Neutral (Apocalypse)
### Win Condition: Kill all non-Neutral Benign roles

Pestilence is always on permanent alert, where anyone who tries to interact with them will die. Pestilence does not spawn in-game and instead gets converted from Plaguebearer after they infect everyone. Pestilence cannot die unless they have been voted out, and they can't be guessed (usually). This role does not spawn directly, unless it's set to, in which case it will replace the Plaguebearer.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Pestilence Can Spawn Directly | Dictates whether Pestilence can appear even if no one is infected | Toggle | False |
| Pestilence Transformation Alerts Everyone | Dictates whether everyone is alerted when the Plaguebearer turns into Pestilence | Toggle | False |
| Obliterate Cooldown | The cooldown on Pestilence's Obliterate cooldown | Timer | 25s |
| Pestilence Can Vent | Whether Pestilence Can Vent | Toggle | False |

## Phantom
### Alignment: Neutral (Proselyte)
### Win Condition: Finish your tasks without getting clicked or having the game end

The Phantom spawns when a Neutral player dies withouth accomplishing their objective. They become half-invisible and have to complete all their tasks without getting clicked on to win.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Tasks Remaining When Phantom Can Be Clicked | The amount of tasks remaining when the Phantom Can Be Clicked | Number | 5 |
| Players Are Alerted When Phantom Is Clickable | Whether players are alerted to the Phantom's existence and clickability | Number | 5 |

## Plaguebearer
### Alignment: Neutral (Harbinger)
### Win Condition: Infect everyone and turn into Pestilence or live to the end by killing off anyone who opposes them

The Plaguebearer can infect other players. Once infected, the infected player can go and infect other players via interacting with them. Once all players are infected, the Plaguebearer becomes Pestilence.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Infect Cooldown | The cooldown on the Plaguebearer's Infect button | Time | 25s |
| Plaguebearer Can Vent | Whether the Plaguebearer can Vent | Toggle | False |

## Serial Killer
### Alignment: Neutral (Killing)
### Win Condition: Stab anyone who opposes them

Although the Serial Killer has a kill button, they can't use it unless they are in Bloodlust. Once the Serial Killer is in bloodlust they gain the ability to kill. However, unlike most killers, their kill cooldown is really short for the duration of the bloodlust.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Bloodlust Cooldown | The cooldown on the Serial Killer's Bloodlust button | Time | 25s |
| Bloodlust Duration | The duration of the Serial Killer's Bloodlust | Time | 25s |
| Stab Cooldown | The cooldown on the Serial Killer's Stab button | Time | 10s |
| Serial Killer Can Vent | Whether the Serial Killer can Vent when Rampaged | String | Always |

## Survivor
### Alignment: Neutral (Benign)
### Win Condition: Live to see the end of the game

The Survivor wins by simply surviving. They can vest which makes them immortal for a short duration.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Vest Cooldown | The cooldown on the Survivor's Vest button | Time | 25s |
| Vest Duration | How long The Survivor's Vest lasts | Time | 10s |
| Cooldown Reset When Vested | The attackers kill cooldown after they attack a vested Survivor | Time | 2.5s |
| Max Vests | The amount of times the Vest ability can be used | Number | 5 |
| Survivor Can Hide In Vents | Whether the Survivor Can Vent | Toggle | False |
| Survivor Can Switch Vents | Whether the Survivor Can Switch while in Vents | Toggle | False |

## Thief
### Alignment: Neutral (Benign)
### Win Condition: Kill a killer and win as their role

The Thief can kill players to steal their roles. The player, however, must be a role with the ability to kill otherwise the Thief will die. After stealing their target's role, the Thief can now win as whatever role they have become.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Steals Cooldown | The cooldown on the Thief's Steal button | Time | 25s |
| Thief Assigns Thief Role To Target | Whether the Thief completely steals their target's role or just copies it | Toggle | False |
| Thief Can Vent | Whether Thief Can Vent | Toggle | False |

## Troll
### Alignment: Neutral (Evil)
### Win Condition: Get killed

The Troll just wants to be killed, but not ejected. The Troll can "interact" with players. This interaction does nothing, it just triggers any interaction sensitive roles like Veteran and Pestilence.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Interaction Cooldown | The cooldown on the Troll's Interact button | Time | 25s |
| Troll Can Hide In Vents | Whether the Troll Can Vent | Toggle | False |
| Troll Can Switch Vents | Whether the Troll Can Switch while in Vents | Toggle | False |

## Werewolf
### Alignment: Neutral (Killing)
### Win Condition: Maul anyone who opposes them

The Werewolf can kill all players within a certain radius.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Maul Cooldown | The cooldown on the Werewolf's Maul button | Time | 30s |
| Maul Radius | How far must a player be to survive a Werewolf's attack | Factor | 1x |
| Werewolf Can Vent | Toggles the Murderer's ability to vent | Toggle | False|

## Whisperer
### Alignment: Neutral (Neophyte)
### Win Condition: Persuade others into joining the cult

The Whisperer can whisper to all players within a certain radius. With each whisper, the chances of bringing someone over to the Whisperer's side increases till they do convert.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Whisper Cooldown | The cooldown on the Whisperer's Whisper button | Time | 25s |
| Whisper Cooldown Increase | The increase on the Whisperer's whisper cooldown with each use | Time | 5s |
| Whisper Rate | The inital rate for each whisper | Percentage | 5% |
| Whisper Rate Decrease | Self descriptive | Percentage | 5% |
| Whisper Rate Decreases Each Whisper | Self descriptive | Percentage | 5% |
| Whisper Radius | How far a player must be to avoid a whisper | Factor | 1x |
| Whisperer Can Vent | Toggles the Whisperer's ability to vent | Toggle | False |
| Persuaded Can Vent | Whether the Whisperer's Sect Can Vent, overriding their role's vent settings | Toggle | False |

# Intruder Roles

![Intruder](./Images/Intruder.png)

Each member of this faction has the ability to kill alongside an ability pertaining to their role. The main theme of this faction is destruction and raw power. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. All members can sabotage to distract the Crew from their tasks.

### Intruder Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Intruder Count | How many Intruders can spawn | Number | 1 |
| Kill Cooldown | The cooldown on the all of the Intruder's Kill button | Time | 25s |
| Intruders Can Sabotage | Self descriptive | Toggle | False |

## Ambusher
### Alignment: Intruder (Killing)

The Ambusher can temporaily force anyone to go on alert, killing anyone who interacts with the Ambusher's target.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Ambush Cooldown | The cooldown on the Ambusher's Ambush button | Time | 10s |
| Ambush Duration | The duration of the Ambusher target's Alert | Time | 25s |
| Ambusher Can Ambush Teammates | Dictates whether the Ambusher can target teammates, while also being able to kill them | Toggle | False |

## Blackmailer
### Alignment: Intruder (Concealing)

The Blackmailer can blackmail people. Blackmailed players cannot speak during the next meeting.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Blackmail Cooldown | The cooldown on the Blackmailer's Blackmail button | Time | 25s |
| Blackmailer Can Read Whispers | Self descriptive | Toggle | False |
| Blackmailer Can Blackmail Teammates | Self descriptive | Toggle | False |
| Blackmail Is Revealed To Everyone | Self descriptive | Toggle | False |

## Camouflager
### Alignment: Intruder (Concealing)

The Camouflager does the same thing as the Comms Sabotage, but their camouflage can be stacked on top other sabotages. Camouflaged players can kill in front everyone and no one will know who it is.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Camouflage Cooldown | The cooldown on the Camouflager's Camouflage button | Time | 25s |
| Camouflage Duration | How long the Camouflage lasts for | Time | 10s |
| Camouflage Hides Size | Whether the camouflage can change a player's size to match that of the other players | Toggle | False |
| Camouflage Hides Speed | Whether the camouflage can change a player's speed to match that of the other players | Toggle | False |

## Consigliere
### Alignment: Intruder (Support)

The Consigliere can reveal people's roles. They cannot get Assassin unless they see factions for obvious reasons.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Investigate Cooldown | The cooldown on the Consigliere's Investigate button | Time | 25s |
| Info That Consigliere Sees | Decides what the Consigliere gets as a result of checking someone | String | Role |

## Consort
### Alignment: Intruder (Support)

The Consort can roleblock players and prevent them from doing anything for a short while.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Roleblock Cooldown | The cooldown on the Consort's roleblock button | Time | 25s |
| Roleblock Duration | The duration of the Consort's roleblock | Time | 10s |

## Disguiser
### Alignment: Intruder (Deception)

The Disguiser can disguise into other players. At the beginning of each, they can choose someone to measure. They can then disguise the next nearest person into the measured person for a limited amount of time after a short delay.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Disguise Cooldown | The cooldown on the Disguiser's Disguise button | Time | 25s |
| Disguise Duration | How long the disguise lasts for | Time | 10s |
| Delay Before Disguising | The time it takes for the disguise to take place | Time | 5s |
| Measure Cooldown | The cooldown on the Disguiser's Measure button | Time | 25s |
| Disguise And Measure Cooldowns Are Linked | Decides whether the Disguiser's cooldowns are linked so that measuring resets disguises and vice versa | Toggle | False |
| Disguise Targets | Who can be disguised by the Disguiser | String | Everyone |

## Enforcer
### Alignment: Intruder (Killing)

The Enforcer can plant bombs on players. After a short while, their target will be alerted to the bomb's presence and must kill someone to get rid of it. If they fail to do so in a time limit, the bomb will explode, killing everyone within its vicinity.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Enforce Cooldown | The cooldown on the Enforce button | Time | 25s |
| Enforce Duration | The duration of the bomb's presence | Time | 10s |
| Enforce Delay | Amount of time the target has to kill someone | Time | 5s |
| Enforce Explosion Radius | Self descriptive | Factor | 0.25m |

## Ghoul
### Alignment: Intruder (Utility)

Every round, the Ghoul can mark a player for death. All players are told who is marked and the marked player will die at the end of the next meeting. The only way to save a marked player is to click the Ghoul that marked them.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Mark Cooldown | The cooldown on the Ghoul's Mark button | Time | 25s |

## Godfather
### Alignment: Intruder (Support)

The Godfather can only spawn in 3+ Intruder games. They can choose to promote a fellow Intruder to Mafioso. When the Godfather dies, the Mafioso becomes the new Godfather and has lowered cooldowns.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Cooldown Bonus | How much do the cooldowns decrease by upon promotion to Godfather | Factor | 0.75x |

## Grenadier
### Alignment: Intruder (Concealing)

The Grenadier can throw flash grenades which blinds nearby players. However, a sabotage and a flash grenade can not be active at the same time.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Flash Grenade Cooldown | The cooldown on the Grenadier's Flash button | Time | 25s |
| Flash Grenade Duration | How long the Flash Grenade lasts for | Time | 10s |
| Flash Radius | How wide the flash radius is | Factor | 1x |
| Indicate Flashed Players | Whether the Grenadier can see who has been flashed | Toggle | False |
| Grenadier can Vent | Whether the Grenadier can Vent | Toggle | False |

## Impostor
### Alignment: Intruder (Utility)

Just a plain Intruder with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.

## Janitor
### Alignment: Intruder (Concealing)

The Janitor can drag, drop and clean up bodies. Both their Kill and Clean ability usually have a shared cooldown, meaning they have to choose which one they want to use.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Clean Cooldown | The cooldown on the Janitor's Clean button | Time | 25s |
| Janitor Has Lower Clean Cooldown When Solo | Self explanatory | Toggle | False |
| Kill And Clean Cooldowns Are Linked | Decides whether the Janitor's cooldowns are linked so that killing resets cleaning and vice versa | Toggle | False |
| Drag Cooldown | The cooldown on the Janitor Drag ability | Time | 25s |
| Drag Speed | How fast will the Janitor become when dragging a body | Factor | 0.5x |
| Janitor Can Vent | Whether the Janitor can Vent | String | Never |

## Mafioso
### Alignment: Intruder (Utility)

The Mafioso is promoted from a random non-Godfather Intruder role. The Mafioso by themself is nothing special, but when the Godfather dies, the Mafioso becomes the new Godfather. As a result, the new Godfather has a lower cooldown on all of their original role's abilities.

## Miner
### Alignment: Intruder (Support)

The Miner can create new vents. These vents only connect to each other, forming a new passageway.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Mine Cooldown | The cooldown on the Miner's Mine button | Time | 25s |

## Morphling
### Alignment: Intruder (Deception)

The Morphling can morph into another player. At the beginning of each round, they can choose someone to sample. They can then morph into that person at any time for a limited amount of time.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Morph Cooldown | The cooldown on the Morphling's Morph button | Time | 25s |
| Morph Duration | How long the Morph lasts for | Time | 10s |
| Sample Cooldown | The cooldown on the Morphling's Sample button | Time | 25s |
| Morph And Sample Cooldowns Are Linked | Decides whether the Morphling's cooldowns are linked so that morphing resets sampling and vice versa | Toggle | False |
| Morphling can Vent | Whether the Morphling can Vent | Toggle | False |

## Teleporter
### Alignment: Intruder (Support)

The Teleporter can teleport to a marked positions. The Teleporter can mark a location which they can then teleport to later.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Teleport Cooldown | The cooldown on the Teleporter's Teleport button | Time | 25s |
| Mark Cooldown | The cooldown on the Teleporter's Mark button | Time | 25s |
| Teleport And Mark Cooldowns Are Linked | Decides whether the Teleporter's cooldowns are linked so that marking resets teleportation and vice versa | Toggle | False |
| Teleporter can Vent | Whether the Teleporter can Vent | Toggle | False |

## Wraith
### Alignment: Intruder (Concealing)

The Wraith can temporarily turn invisible.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Invis Cooldown | The cooldown on the Wraith's Invis button | Time | 25s |
| Invis Duration | How duration of the Wraith's invisibility | Time | 10s |
| Wraith can Vent | Whether the Wraith can Vent | Toggle | False |

# Syndicate Roles

![Syndicate](./Images/Syndicate.png)

Each member of this faction has a special ability and then after a certain number of meetings, can also kill. The main theme of this faction is chaos. This faction is an informed minority meaning they make up a tiny fraction of the crew and know who the other members are. After a certain number of meetings, the Syndicate can retreive the "Chaos Drive" which gives the holder the ability to kill (if they couldn't already) while also enhancing their existing abilities.

### Syndicate Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Syndicate Count | How many Syndicate can spawn | Number | 1 |
| Kill Cooldown | The cooldown on the all of the Syndicate's Kill button upon receiving the Chaos Drive | Time | 25s |
| Syndicate Replaces Intruders | Self descriptive | Toggle | False |
| Chaos Drive Timer | How many meetings must pass before the Chaos Drive is handed out | Number | 3 |
| Chaos Drive Is Global | The Chaos Drive is handed out to every member of the Syndicate rather than only one | Toggle | False |
| Bomb Detonation Kills Members Of The Syndicate | Dictates whether members of the Syndicate are immune to bomb detonations | Toggle | True |

## Anarchist
### Alignment: Syndicate (Utility)

Just a plain Syndicate with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode. Its only benefit is its ability to kill from the beginning of the game. With the Chaos Drive, the Anarchist's kill cooldown decreases.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Kill Cooldown | The cooldown on the Anarchist's Kill button before receiving the Chaos Drive | Time | 25s |

## Banshee
### Alignment: Syndicate (Utility)

The Banshee can block every non-Syndicate player every once in a while. This role cannot get the Chaos Drive.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Scream Cooldown | The cooldown on the Banshee's Scream button | Time | 25s |
| Scream Duration | The duration of the scream | Time | 25s |

## Bomber
### Alignment: Syndicate (Killing)"

The Bomber can place a bomb which can be remotely detonated at any time. Anyone caught inside the bomb's radius at the time of detonation will be killed. Only the latest placed bomb will detonate, unless the Bomber holds the Chaos Drive, with which they can detonate all bombs at once.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Bomb Cooldown | The cooldown on the Bomber's Bomb button | Time | 25s |
| Detonate Cooldown | The cooldown on the Bomber's Detonate button | Time | 25s |
| Bomb Radius | The bomb's radius | Distance | 0.25m |
| Chaos Drive Bomb Radius Increase | By how much does the range of the bomb increase | Distance | 0.1m |
| Bomb and Detonate Cooldowns Are Linked | Decides whether the Bomber's cooldowns are linked so that detonating resets placing and vice versa | Toggle | False |
| Bombs Are Cleared Every Meeting | Self descriptive | Toggle | False |
| Bombs Detonate When A Meeting Is Called | Self descriptive | Toggle | False |

## Concealer
### Alignment: Syndicate (Disruption)

The Concealer can make a player invisible for a short while. With the Chaos Drive, this applies to everyone.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Conceal Cooldown | The cooldown on the Concealer's conceal button | Time | 25s |
| Conceal Duration | The duration of the Concealer's conceal | Time | 10s |
| Concealer Can Conceal Teammates | Self descriptive | Toggle | False |

## Crusader
### Alignment: Syndicate (Killing)

The Crusader can temporaily force anyone to go on alert, killing anyone who interacts with the Crusader's target. With the Chaos Drive, attempting to interact with the Crusader's target will cause the target to kill everyone within a certain range, including the target themselves.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Crusade Cooldown | The cooldown on the Crusader's Crusade button | Time | 25s |
| Crusade Duration | The duration of the Crusader target's Crusade | Time | 10s |
| Crusader Can Crusade Teammates | Dictates whether the Crusader can target teammates, while also being able to kill them | Toggle | False |

## Collider
### Alignment: Syndicate (Killing)

The Collider can mark players as positive and negative. If these charged players come within a certain distance of each other, they will die together. With the Chaos Drive, the range of collision increases.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Coolide Cooldown | The cooldown on the Collider's Coolide button | Time | 25s |
| Collide Radius | How much distance must be between the charged players for them to survive | Distance | 1m |
| Chaos Drive Collide Radius Increase | How much does the collide distance increase by upon receiving the Chaos Drive | Distance | 1m |

## Drunkard
### Alignment: Syndicate (Disruption)

The Drunkard can reverse a player's controls for a short while. With the Chaos Drive, this applies to everyone.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Confuse Cooldown | The cooldown on the Drunkard's confuse button | Time | 25s |
| Confuse Duration | The duration of the Drunkard's confusion | Time | 10s |
| Syndicate Are Immune To Confuse | Self descriptive | Toggle | False |

## Framer
### Alignment: Syndicate (Disruption)

The Framer can frame players, making them appear to have wrong results and be easily killed by Vigilantes and Assassins. This effects lasts as long as the Framer is alive. With the Chaos Drive, the Framer can frame players within a certain radius.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Frame Cooldown | The cooldown on the Framer's frame button | Time | 25s |
| Chaos Drive Frame Radius | How much distance must be between the players to not be framed | Distance | 1m |

## Poisoner
### Alignment: Syndicate (Killing)

The Poisoner can poison another player instead of killing. When they poison a player, the poisoned player dies either upon the start of the next meeting or after a set duration. With the Chaos Drive, the Poisoner can poison a player from anywhere.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Poison Cooldown | The cooldown on the Poisoner's Poison button | Time | 25s |
| Poison Kill Delay | The delay of the kill after being poisoned | Time | 5s |

## Rebel
### Alignment: Syndicate (Support)

The Rebel can only spawn in 3+ Syndicate games. They can choose to promote a fellow Syndicate to Sidekick. When the Rebel dies, the Sidekick becomes the new Rebel and has lowered cooldowns. With the Chaos Drive, the Rebel's gains the improved abilities of their former role. A promoted Rebel has the highest priority when recieving the Chaos Drive and the original Rebel as the lowest priority.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Cooldown Bonus | How much do the cooldowns decrease by upon promotion | Factor | 0.75x |

## Shapeshifter
### Alignment: Syndicate (Support)

The Shapeshifter can swap the appearances of 2 players. WIth the Chaos Drive, everyone's appearances are suffled.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Shapeshift Cooldown | The cooldown on the Shapeshifter's Shapeshift button | Time | 25s |
| Shapeshift Duration | The duration of the Shapeshifter's Shapeshift | Time | 10s |
| Shapeshifter Can Shapeshift Teammates | Self descriptive | Toggle | False |

## Sidekick
### Alignment: Syndicate (Utility)

The Sidekick is promoted from a random non-Rebel Syndicate role. The Sidekick by themselves is nothing special, but when the Rebel dies, the Sidekick becomes the new Rebel. As a result, the new Rebel has a lower cooldown on all of their original role's abilities.

## Silencer
### Alignment: Syndicate (Disruption)

The Silencer can silencer people. Silenced plaeyrs cannot see the messages being sent by others but can still talk. Other players can still talk but can't get their info through to the silenced player. With the Chaos Drive, silence prevents everyone except for the silenced player from talking.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Silence Cooldown | The cooldown on the Silencer's Silence button | Time | 25s |
| Silencer Can Read Whispers | Self descriptive | Toggle | False |
| Silencer Can Silence Teammates | Self descriptive | Toggle | False |
| Silence Is Revealed To Everyone | Self descriptive | Toggle | False |

## Spellslinger
### Alignment: Syndicate (Power)

The Spellslinger is a powerful role who can cast curses on players. When all non-Syndicate players are cursed, the game ends in a Syndicate victory. With each curse cast, the spell cooldown increases. This effect is negated by the Chaos Drive.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Spell Cooldown | The cooldown on the Spellslinger's Spell button | Time | 25s |
| Spell Cooldown Increase | By how much does the cooldown increase with each curse cast | Time | 5s |

## Stalker
### Alignment: Syndicate (Support)

The Stalker is a buffed Tracker with no update interval. With the Chaos Drive, the arrows are no longer affected by camouflages and all players instantly have an arrow pointing at them upon receiving the Chaos Drive.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Stalk Cooldown | The cooldown on the Stalker's Stalk button | Time | 25s |

## Time Keeper
### Alignment: Syndicate (Power)

The Time Keeper can control time. Without the Chaos Drive, the Time Keeper can freeze time, making everyone unable to move. With the Chaos Drive, the Time Keeper rewinds players instead.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Time Control Cooldown | The cooldown on the Time Keeper's time control button | Time | 25s |
| Time Control Duration | The duration of the Time Keeper's time control | Time | 10s |
| Syndicate Are Immune To Freeze | Self descriptive | Toggle | False |
| Syndicate Are Immune To Rewind | Self descriptive | Toggle | False |

## Warper
### Alignment: Syndicate (Support)

The Warper can teleport a player to another player. With the Chaos Drive, the Warper teleports everyone to random positions on the map.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Warp Cooldown | The cooldown on the Warper's Warp button | Time | 25s |
| Warper Can Warp Themselves | Self descriptive | Toggle | False |

-----------------------

# Objectifiers
Objectifiers are basically a second objective for the player. They can either choose to win the regular way, or win via their Objectifier's condition.

## Allied
### Applied To: Neutral (Killing)
### Win Condition: Win with whichever faction they are aligned with

An Allied Neutral Killer now sides with either the Crew, the Intruders or the Syndicate. In the case of the latter two, all faction members are shown the Allied player's role, and can no longer kill them.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Allied Faction | Which faction the Allied joins | String | Random |

## Corrupted
### Applied to: Crew
### Win Condition: Kill everyone

The Corrupted is a Crewmate with the alignment of a Neutral Killer. On top of their base role's attributes, they also gain a kill button. Their win condition is so strict that not even Neutral Benigns or Evils can be spared.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Corrupt Cooldown | The cooldown on the Corrupted player's kill button | Time | 25s |
| All Corrupted Win Together | Self descriptive | Toggle | False |
| Corrupted Can Vent | Self descriptive | Toggle | False |

## Defector
### Applied To: Intruders and Syndicate
### Win Condition: Be the last one of thier faction to switch sides

A Defector switches sides when they happen to be the last player alive in their original faction.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Defector Faction | Which faction the Defector joins | String | Random |
| Defector Knows Who They Are | Whether the Defector knows who they are | Toggle | True |

## Fanatic
### Applied To: Crew
### Win Condition: Get attacked by either the Intruders or the Syndicate to join their team

When attacked, the Fanatic joins whichever faction their attacker belongs to. From then on, their alliance sits with said faction.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Fanatic Knows Who They Are | Whether the Fanatic knows who they are | Toggle | True |
| Snitch Sees Turned Fanatic | Whether the Snitch sees a turned Fanatic | Toggle | True |
| Revealer Reveals Turned Fanatic | Whether the Revealer reveals a turned Fanatic | Toggle | True |
| Turned Fanatic Swaps Colours for Investigative Roles | Self descriptive | Toggle | False |

## Lovers
### Applied To: Everyone
### Win Condition: Be 2 of the 3 final players

The Lovers are two players who are linked together. They gain the primary objective to stay alive together. If they are both among the last 3 players, they win as a Lover pair. In order to so, they gain access to a private chat, only visible by them in between meetings. However, they can also win with their respective team, hence why the Lovers do not know the role of the other Lover.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Enable Lovers Chat | Whether the Lovers get a private chat in-between meetings | Toggle | True |
| Both Lovers Die | Whether the other Lover automatically dies if the other does | Toggle | True |
| Lovers Can Be Of The Same Faction | Self descriptive | Toggle | True |
| Lovers Know Each Other's Roles | Self descriptive | Toggle | True |

## Mafia
### Applied To: Everyone
### Win Condition: Kill off anyone who is not a Mafia member

The Mafia are a group of players with a linked win condition. They must kill anyone who is not a member of the Mafia. All Mafia win together.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Mafia Know Each Other's Roles | Self descriptive | Toggle | True |
| Mafia Can Vent | Whether the members of the Mafia can vent, this overrides their role's vent settings | Toggle | True |

## Overlord
### Applied To: Neutrals
### Win Condition: Survive a set amount of meetings

Every meeting, for as long as an Overlord is alive, players will be alerted to their existence. The game ends if the Overlord lives long enough.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Overlord Knows Who They Are | Whether the Overlord knows who they are | Toggle | True |
| Overlord Meeting Timer | How many meetings the Overlord must survive | Number | 2 |

## Rivals
### Applied To: Everyone
### Win condition: Get the other rival killed without directly interfering, then live to the final 2

The Rivals cannot do anything to each other and must get the other one killed.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Enable Rivals Chat | Whether the Rivals get a private chat in-between meetings (this is just a troll setting lol) | Toggle | True |
| Rivals Can Be Of The Same Faction | Self descriptive | Toggle | True |
| Rivals Know Each Other's Roles | Self descriptive | Toggle | True |

## Taskmaster
### Applied To: Neutrals
### Win Condition: Finish tasks before the game ends or dying

The Taskmaster is basically a living Phantom. When a certain number of tasks are remaining, the Taskmaster is revealed to Intruders and the Syndicate and the Crew only sees a flash to indicate the Taskmaster's existence.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Tasks Remaining When Revealed | How many tasks be remain when the Taskmaster's existence is revealed | Number | 1 |

## Traitor
### Applies To: Crew
### Win Condition: Finish tasks to join either the Intruders or Syndicate

The Traitor is a Crewmate who must finish their tasks to switch sides. Upon doing so, they will either join the Intruders or the Syndicate, and will win with that faction. If the Traitor is the only person in their new faction, they become a Betrayer, losing their original role's abilities and gaining the ability to kill in the process.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Traitor Knows Who They Are | Whether the Traitor knows who they are | Toggle | True |
| Snitch Sees Turned Traitor | Whether the Snitch sees a turned Traitor | Toggle | True |
| Revealer Reveals Turned Traitor | Whether the Revealer reveals a turned Traitor | Toggle | True |
| Turned Traitor Swaps Colours for Investigative Roles | Self descriptive | Toggle | False |

-----------------------

# Modifiers
Modifiers are passive afflictions that change a player's gameplay.

## Bait
### Applied To: Everyone

Killing the Bait makes the killer auto self-report.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Bait Knows Who They Are | Whether the Bait knows who they are | Toggle | True |
| Bait Minimum Delay | The minimum time the killer of the Bait reports the body | Time | 0s |
| Bait Maximum Delay | The maximum time the killer of the Bait reports the body | Time | 1s |

## Coward
### Applied To: Everyone

The Coward cannot report bodies.

## Diseased
### Applied To: Everyone

Killing the Diseased increases the killer's kill cooldown.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Diseased Knows Who They Are | Whether the Bait knows who they are | Toggle | True |
| Kill Multiplier | How much the Kill Cooldown of the killer is increased by | Factor | 3x |

## Drunk
### Applied To: Everyone

The Drunk player's controls are inverted.

## Dwarf
### Applied To: Everyone

The Dwarf travels at increased speed and has a much smaller body.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Dwarf Speed | How fast the Dwarf moves in comparison to normal | Factor | 1.3x |
| Dwarf Scale | How small the Dwarf is in comparison to normal | Factor | 0.5x |

## Flincher
### Applied To: Everyone

Every now and then, the Flincher flinches.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Flinch Interval | How much time must pass before the flinch happens | Time | 10s |

## Giant
### Applied To: Everyone

The Giant is a gigantic player that has a decreased walk speed.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Giant Speed | How slow the Giant moves in comparison to normal | Factor | 0.75x |
| Giant Scale | How big the Giant is in comparison to normal | Factor | 1.5x |

## Indomitable
### Applied To: Everyone

You cannot be guessed/assassinated in meetings.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Indomitable Knows Who They Are | Whether a player knows they are Indomitable at the start of a game | Toggle | False |

## Professional
### Applied To: Assassins

You have one extra life used when you guess incorrectly.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Professional Knows Who They Are | Whether a player knows they are a Professional at the start of a game | Toggle | True |

## Shy
### Applied To: Everyone

The Shy player cannot call meetings.

## VIP
### Applied To: Everyone

Everyone is alerted of the VIP's death through a flash of the VIP's role color.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| VIP Knows Who They Are | Whether a player knows they are a VIP at the start of a game | Toggle | True |

## Volatile
### Applied To: Everyone

You see and hear things and might lash out on others.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Volatile Interval | How much time must pass before something happens | Time | 10s |

-----------------------

# Abilities
Abilities give you extra perks on top of your role's regular powers.

## Assassin
### Applied To: Intruders, Crew, Syndicate, Neutral (Killing) and Neutral (Neophyte)

The Assassin is given to a certain number of Intruders, Syndicate and/or Neutral Killers. This ability gives the Intruder, Syndicate or Neutral a chance to kill during meetings by guessing the roles or modifiers of others. If they guess wrong, they die instead.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Number of Intruder Assassins | How many Intruders can Assassinate | Number | 1 |
| Number of Syndicate Assassins | How many Syndicate members can Assassinate | Number | 1 |
| Number of Neutral Assassins | How many Neutrals can Assassinate | Number | 1 |
| Number of Crew Assassins | How many Crewmates can Assassinate | Number | 1 |
| Assassin Guess Limit | The number of kills Assassins can do with his ability | Number | 1 |
| Assassin Multiple Kill | Whether the Assassin can kill more than once per meeting | Toggle | False |
| Assassin Guess Neutral Evil | Whether the Assassin can Guess Neutral Evil roles | Toggle | False |
| Assassin Guess Neutral Benign | Whether the Assassin can Guess Neutral Benign roles | Toggle | False |
| Assassin Guess Pestilence | Whether the Assassin can Guess Pestilence | Toggle | False |
| Assassin Guess Select Modifiers | Whether the Assassin can Guess some non-obvious Modifiers | Toggle | False |
| Assassin Guess Select Objectifiers | Whether the Assassin can Guess some non-obvious Objectifiers | Toggle | False |
| Assassin Guess Select Abilities | Whether the Assassin can Guess some non-obvious Abilities | Toggle | False |
| Assassin Can Guess After Voting | Whether the Assassin can Guess after voting | Toggle | False |

## Button Barry
### Applied To: Everyone

Button Barry has the ability to call a meeting from anywhere on the map, even during sabotages. Calling a meeting during a non-critical sabotage will fix the sabotage.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Button Cooldown | How much time must pass since the last meeting to be able to call a meeting | Percentage | 0% |

## Insider
### Applied To: Crew

The Insider will be able to view everyone's votes in meetings upon finishing their tasks. Only spawns if Anonymous Votes is turn on.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Insider Knows Who They Are | Whether a player knows they are a Insider at the start of a game | Toggle | True |

## Multitasker
### Applied to: Roles with tasks

When doing tasks, the Multitasker's task window is transparent.

### Game Options
| Name | Description | Type | Default |
|------|-------------|------|---------|
| Transperency | Decides how well can someone see when opening a task | Percentage | 50% |

## Ninja
### Applied To: Killers

Ninjas don't lunge when killing.

## Politician
### Applied To: Crew, Intruders, Syndicate, Neutral Killers

The Politician can vote multiple times. If the Politician cannot kill, they gain a new button called the abstain button which stores their vote for later use. On the other hand, if the Politician can kill, they lose the Abstain button ans instead gain a vote for each player they kill.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Politician Initial Vote Bank | The amount of votes the Politician has at the start of the game | Number | 1 |
| Politician Anonymous Votes | Self descriptive | Toggle | False |
| Politician Can Button | Whether the Politician can call a meeting | Toggle | True |

## Radar
### Applied To: Everyone

The Radar always has an arrow pointing towards the nearest player.

## Ruthless
### Applied To: Killers

A Ruthless killer can bypass all forms of protection. Although they bypass alert protection, they will still die to a player on alert.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Ruthless Knows Who They Are | Whether a player knows they are a Ruthless at the start of a game | Toggle | True |

## Snitch
### Applied To: Crew

The Snitch is an ability which allows any Crewmate to get arrows pointing towards the Intruders once all their tasks are finished. The names of the Intruders will also show up as red on their screen. However, when they only have a single task left, the Intruders get an arrow pointing towards the Snitch.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Snitch Knows Who They Are | Whether a player knows they are a Snitch at the start of a game | Toggle | True |
| Snitch Sees Neutral Roles | Whether the Snitch also Reveals Neutral Roles | Toggle | False |
| Snitch Sees Crew Roles | Whether the Snitch also Reveals Crew Roles | Toggle | False |
| Snitch Sees Exact Roles | Whether the Snitch also Reveals all Roles | Toggle | False |
| Tasks Remaining When Revealed | The number of tasks remaining when the Snitch is revealed to Impostors | Number | 1 |
| Snitch Sees Evils in Meetings | Whether the Snitch sees the evil players' names red in Meetings | Toggle | True |

## Swapper
### Applies To: Crew

The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted towards the second player and vice versa.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Swapper Can Button | Whether the Swapper can call a meeting | Toggle | True |
| Swapper Can Swap After Voting | Whether the Swapper can continue players to swap even after they have voted | Toggle | True |
| Swapper Can Swap Themself | Self Descriptive | Toggle | True |

## Tiebreaker
### Applied To: Everyone

If any vote is a draw, the Tiebreaker's vote will go through. If they voted another player, they will get voted out. If the Tiebreaker is the Mayor, it applies to the Mayor's __first__ vote.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Tiebreaker Knows Who They Are | Whether a player knows they are a Tiebreaker at the start of a game | Toggle | True |

## Torch
### Applied To: Non-Killers

The Torch has Intruder vision at all times.

## Tunneler
### Applied To: Crew excluding Engineer

The Tunneler will be able to vent when they finish their tasks.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Tunneler Knows Who They Are | Whether a player knows they are a Tunneler at the start of a game | Toggle | True |

## Underdog
### Applied To: Intruders

The Underdog is an Intruder with a prolonged kill cooldown when with a teammate. When they are the only remaining Intruder, they will have their kill cooldown shortened.

### Game Options

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Underdog Knows Who They Are | Whether a player knows they are a Underdog at the start of a game | Toggle | True |
| Kill Cooldown Bonus | The amount of time added or removed from the Underdog's Kill Cooldown | Time | 5s |
| Increased Kill Cooldown  | Whether the Underdog's Kill Cooldown is Increased when they aren't alone | Toggle | True |

-----------------------

# Custom Game Settings

## Game Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Player Speed | Self Descriptive | Factor | 1x |
| Ghost Speed | Self Descriptive | Factor | 3x |
| Interaction Distance | How far is too far to interact | Distance | 2m |
| Emergency Button Count | How many times can players call meetings | Number | 1 |
| Emergency Button Cooldown | How much time must pass before the next meeting can be called | Time | 25s |
| Discussion Time | Self Descriptive | Time | 30s |
| Voting Time | Self Descriptive | Factor | 60s |
| Taskbar Updates | How does the taskbar update | String | Meeting Only |
| Confirm Ejects | Displays the roles/factions of ejected players | Toggle | False |
| Game Start Cooldowns | Upon the start of the game, overrides the cooldowns of all roles with a cooldown | Time | 10s |
| Player Report Distance | how close must a player be to a body to report it | Distance | 3.5m |
| Chat Cooldown | How much should a player wait to send a message | Time | 3s |
| Lobby Size | Self Descriptive | Number | Auto-updates to whatever you set it as when making the lobby |

## Game Mode Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Game Mode | What game mode the next game will be | String | Classic |

## Killing Only Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Neutrals Count | How many neutral roles will spawn | Number | 1 |
| Add Arsonist | Whether Arsonist will be added to the role list | Toggle | True |
| Add Plaguebearer | Whether Plaguebearer will be added to the role list | Toggle | True |
| Add Cryomaniac | Whether Cryomaniac will be added to the role list | Toggle | True |

## All Any Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Enable Uniques | Decide whether roles can be toggled to have only one spawn or not | Toggle | True |

## Game Modifiers

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Serial Venters | Decides how players vent | String | Default |
| Anonymous Voting | Votes are grayed out so that it's unknown who voted for who | Toggle | True |
| No Skipping | Whether the meeting button is disabled | String | Never |
| Factioned Evils Know The Roles Of Their Team | Self descriptive | Toggle | True |
| Visual Tasks | Disables animations for certain tasks | Toggle | False |
| No Player Names | Self descriptive | Toggle | False |
| PSST Whispers | Toggles the whispering system | Toggle | True |
| Everyone Is Alerted To Whispers | Players are told who's whispering who | Toggle | True |
| Kill Animations Show Modified Player | Toggles whether the player's changed outfit is what appears on the kill animation | Toggle | True |
| Random Player Spawns | Self descriptive | Toggle | False |
| Enable Abilities | Self descriptive | Toggle | True |
| Enable Objectifiers | Self descriptive | Toggle | True |
| Enable Modifiers | Self descriptive | Toggle | True |
| Players In Vents Can Be Targetted | Self descriptive | Toggle | False |

## Game Announcements

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Enable Game Announcements | Toggles messages being sent at the start of the game dictating who died to who and where | Toggle | False |
| Reported Body's Location Is Announced | Slef descriptive | Toggle | False |
| Every Body's Role/Faction Is Announced | Self descriptive | String | Never |
| Every Body's Killer's Role/Faction Is Announced | Self descriptive | String | Never |

## Quality Changes

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Dead Can See Everything | Whether dead players are able to see every little detail about players | Toggle | False |
| Parallel Medbay Scans | Whether players have to wait for others to scan | Toggle | False |
| Disable Level Icons | Whether all level icons are removed in meetings | Toggle | False |
| Disable Player Nameplates | Whether all decorative nameplates are disabled in meetings | Toggle | False |
| See Tasks During Games | Self descriptive | Toggle | False |
| Custom Ejection Messages | Just jokes | Toggle | False |
| Enable Lighter Darker Colors | Whether all players have their color types visible | Toggle | False |

## Map Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Map | Selects the next map | String | Skeld |
| Choose Random Map | Whether the Map is randomly picked at the start of the game | Toggle | False |
| Skeld Chance | The percentage probability of the Skeld map being chosen | Percentage | 0% |
| Mira HQ Chance | The percentage probability of the Mira HQ map being chosen | Percentage | 0% |
| Polus Chance | The percentage probability of the Polus map being chosen | Percentage | 0% |
| Airship Chance | The percentage probability of the Airship map being chosen | Percentage | 0% |
| Submerged Chance | The percentage probability of the Submerged map being chosen | Percentage | 0% |
| Auto Adjust Settings | Whether the Settings of the game are auto adjusted depending on the map | Toggle | False |
| Half Vision on Skeld/Mira HQ | Whether the Vision is automatically halved on Skeld/Mira HQ | Toggle | False |
| Mira HQ Decreased Cooldowns | How much less time the cooldowns are set to for Mira HQ | Time | 0s |
| Airship Increased Cooldowns | How much more time the cooldowns are set to for Airship | Time | 0s |
| Skeld/Mira HQ Increased Short Tasks | How many extra short tasks when the map is Skeld/Mira HQ | Number | 0 |
| Skeld/Mira HQ Increased Long Tasks | How many extra long tasks when the map is Skeld/Mira HQ | Number | 0 |
| Airship Decreased Short Tasks | How many less short tasks when the map is Airship | Number | 0 |
| Airship Decreased Long Tasks | How many less long tasks when the map is Airship | Number | 0 |

## Better Sabotage Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Camouflaged Comms | Whether everyone becomes camouflaged when Comms are sabotaged | Toggle | True |
| Camouflaged Meetings | Whether everyone becomes camouflaged when a meeting is called when Comms are sabotaged | Toggle | False |
| Oxygen Sabotage Slows Down Players | Self descriptive | Toggle | True |
| Reactor Sabotage Shakes The Screen By | Self descriptive | Percentage | 30% |

## Better Skeld Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Changed Vent Layout | Optimizes Vent Layout on Skeld | Toggle | False |

## Better Polus Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Changed Vent Layout | Optimizes Vent Layout on Polus | Toggle | False |
| Vitals Moved to Lab | Whether the Vitals panel is moved into the Laboratory | Toggle | False |
| Cold Temp Moved to Death Valley | Whether the cold temperature task is moved to death valley | Toggle | False |
| Reboot Wifi and Chart Course Swapped | Whether the Reboot Wifi and Chart Course swap locations | Toggle | False |
| Seimic Stabliser Malfunction Countdown | What is the countdown for the seismic sabotage | Time | 60s |

## Better Airship Settings

| Name | Description | Type | Default |
|------|-------------|------|---------|
| Spawn Type | Decides how spawns on Airship are handled | String | Normal |
| Spawn Near Meeting Table | Disable spawns after a meeting | Toggle | False |
| Add Call Platform Button | Adds a console to bring the plaform to the other side | Toggle | False |
| Add Meeting To Security Room Teleporter | Self Descriptive | Toggle | False |
| Move Fuel | Moves the refuel station elsewhere on the map | Toggle | False |
| Move Vitals | Moves vitals elsewhere on the map | Toggle | False |
| Move Divert | Moves the divert power station elsewhere on the map | Toggle | False |
| Move Admin | Moves admin map elsewhere on the map | String | Don't Move |
| Move Electrical | Moves the electrical lights staion elsewhere on the map | String | Don't Move |

-----------------------

# Game Info

## Lighter Darker Colors

- Red - Darker
- Blue - Darker
- Green - Darker
- Pink - Lighter
- Orange - Lighter
- Yellow - Lighter
- Black - Darker
- White - Lighter
- Purple - Darker
- Brown - Darker
- Cyan - Lighter
- Lime - Lighter
- Maroon - Darker
- Rose - Lighter
- Banana - Lighter
- Gray - Darker
- Tan - Darker
- Coral - Lighter
- Watermelon - Darker
- Chocolate - Darker
- Sky Blue - Lighter
- Beige - Lighter
- Hot Pink - Lighter
- Turquoise - Lighter
- Lilac - Lighter
- Olive - Darker
- Azure - Lighter
- Plum - Darker
- Jungle - Darker
- Mint - Lighter
- Chartreuse - Lighter
- Macau - Darker
- Tawny - Darker
- Gold - Lighter
- Rainbow - Lighter
- Panda - Lighter
- Contrast - Darker
- Chroma - Darker
- Galaxy - Lighter
- Fire - Lighter
- Monochrome - Lighter
- Mantle - Darker

### Inspector Results
- Deals With Dead - Coroner, Amnesiac, Retributionist, Janitor, Cannibal
- Preserves Life - Medic, Guardian Angel, Altruist, Necromancer, Crusader
- Leads The Group - Mayor, Godfather (Promoted and Original), Rebel (Promoted and Original), Pestilence, Survivor
- Brings Chaos - Shifter, Thief, Camouflager, Whisperer, Jackal
- Seeks To Destroy - Arsonist, Cryomaniac, Plaguebearer, Spellslinger
- Wants To Explore - Transporter, Teleporter, Warper, Time Keeper
- New Lens - Engineer, Miner, Seer, Dracula, Medium, Monarch
- Gains Information - Sheriff, Consigliere, Blackmailer, Detective, Inspector
- Is Manipulative - Jester, Executioner, Actor, Troll, Framer, Dictator
- Unseen - Chameleon, Wraith, Concealer, Poisoner, Collider
- Is Cold - Veteran, Vigilante, Sidekick, Guesser, Mafioso
- Tracks Others - Tracker, Mystic, Vampire Hunter, Bounty Hunter, Stalker
- Is Aggressive - Betrayer, Werewolf, Juggernaut, Serial Killer
- Creates Confusion - Morphling, Disguiser, Shapeshifter
- Drops Items - Bomber, Operative, Grenadier, Enforcer
- Hinders Others - Escort, Consort, Glitch, Ambusher, Drunkard
- Is Basic - Crewmate, Impostor, Murderer, Anarchist
- Ghostly - Revealer, Phantom, Ghoul, Banshee

## Subfactions

**Undead**
- This subfaction is led by the Dracula
- There is a limit of how many can be alive in the subfaction
- If a Vampire Hunter interacts with an Undead, the Undead will be killed
- Attempting to interact with a Vampire Hunter as an Undead will force them to kill you

**Sect**
- This subfaction is led by the Whisperer
- The Whisperer can bring in as many people as they want, provided they have the cooldown and the % for it
- The whispers are precious so the Whisperer must whisper near as many people as possible

**Cabal**
- This subfaction is led by the Jackal
- There can be a maximum of 4 members in this subfaction, the Jackal and their 3 recruits
- The Cabal starts off strong with 3 guaranteed members with the 4th one coming along if both the original recruits die

**Reanimated**
- This subfaction is led by the Necromancer
- The only condition for becoming a member of this faction is the need to die so that the Necromancer can resurrect you

## Syndicate Chaos Drive Role Priority
### This list shows which Syndicate role alignments/roles gain the Chaos Drive first if "Chaos Drive Is Global" is turned off.

1. Promoted Rebel
2. Syndicate (Disruption)
3. Syndicate (Support)
4. Syndicate (Power)
5. Syndicate (Killing)
6. Original Rebel, Sidekick, Anarchist

**If there are multiple active roles from the same alignment, the Chaos Drive is randomly given to one of those roles.**

## Game Modes

This mod has a lot of different modes which greatly change how the game continues!

**Vanilla**
- Nothing special, eveyone is either a basic [Crewmate](#crewmate) or [Impostor](#impostor)

**Classic**
- This is the main mode of the game
- Any layer can spawn in this mode, but only once

**Killing Only**
- This is a restricted Classic mode where only roles with the capability to kill are the only ones that can spawn
- The Syndicate recieves their Chaos Drive at the start of the game

**All Any**
- This mode has no restrictions on how many instances of a layer can spawn
- Each layer has a property called "Uniqueness" which is basically if only one of that layer can spawn (or two for Lovers and Rivals)

**Custom**
- This mode is basically Classic but you can decide how many instances of the layer can spawn in the game

-----------------------

# Extras

## New Colors!
New colors are added for crewmates to pick from.

## Custom Cosmetics!
Custom hats, nameplates and visors have been added, made by some very talented artists. These are mostly for streamers. You can find these assets in the [ReworkedAssets](https://github.com/AlchlcDvl/ReworkedAssets) repo.

## Bug / Suggestions / Additions
If you have any bugs or any need to contact me, join the [Discord](https://discord.gg/cd27aDQDY9) or create an issue or pull request on GitHub.

-----------------------

# Layer Explanation And Assignment

Since [TheOtherRoles](https://github.com/Eisbison/TheOtherRoles) has a section about this, might as well add my own. So here goes.

A player's identity in the game is divided into 4 classifications, or Player Layers.

The first and most important one is a [Role](#roles). It decides your abilities and goals for the game. Every game, you are guaranteed to have a role as not having one basically means you cannot play the game.

The next one is an [Objectifier](#objectifiers). It provides and alternate way for you to win, and sometimes it may override the your original win condition (see [Corrupted](#corrupted) and [Mafia](#mafia)) or change your win condition mid-game (see [Traitor](#traitor) and [Fanatic](#fanatic)).

The third layer is an [Ability](#abilities). It gives you can additional ability on top of your original abilities, to help boost your chances of winning.

The last layer is a [Modifier](#modifiers). It is a passive affliction, usually negative or benign in nature, that serves no purpose and is there for fun. It cam alter a player's gameplay based on what they might have. For example, [Baits](#bait) and [Diseased](#diseased) players would want to die for their modifiers to take effect.

With the general explanations out of the way, let's begin with how they are assigned.

The Layer assignment is split into 7 phases.

In the first phase, all required lists are cleared and all variables are reset.

At the beginning of the phases 2 to 5, the spawn chances are handled. If the spawn chance is greater than 0%, the relevant layer is added to a list. If the mode is set to Custom, the same layer is added as many times as set by the count setting. For [Lovers](#lovers) and [Rivals](#rivals), they are added twice for each count as they depend on pairs, rather than singluar assignment. So setting them to spawn twice means that the list will contain up to 4 instances of Lovers/Rivals.

The list's size is then modified to whatever the Max and Min settings for the specific layer or its alignment or faction (in case of Roles).

The list is then shuffled around a lot and then sorted. During sorting, the first thing that happens is the addition of layers with 100% spawn chance being added to the list. If the spawn chance is between 0% and 100%, a random number is drawn from 1 to 99. If the number is less than or equal to the spawn chance, it is added to the spawning list.

As a side note, setting a layer to 100% does not guarantee its spawn. This is because if there are multiple layers set to 100% and the number of assignable players is less than the amount of guaranteed layers, some of the layers will be left out.

In All Any mode, however, the sorting is done differently. Any layer with a spawn chance of greater than 0% is guaranteed to be added, regardless to what it actually was set to. With this list, is then sorted on uniqueness. As long as the spawning list's size is lower than the number of players, it will keep adding a random layer from the initial list to the spawning list. If the layer has been set to unique, that layer is then removed from the initial list so that it is never randomly pulled again.

The second phase of layer assignment is Roles. These have no special conditions as having a role is a requirement for other layers to spawn. If the number of assignable roles is less than the number of players, the outlying players will be assigned Crewmate, Impostor or Anarchist based on their faction.

After the roles are assigned, a random Crew aligned player will be designated as the "Pure Crew" who will not recieve any Objectifier or be recruited by the [Jackal](#jackal). The Pure Crew cannot be converted into other subfactions either and would rather die than join them. This is a sort of hacky solution I came up for when there are no one capable of contributing to the task bar. Because of there technically being "0" tasks, the game would just end in no one's victory.

Anyways, for the next 3 phases, the assignment also includes another action. Before the layers are assigned, a list of players is made for every restricted type (like making a list of players which only includes Crew because the layer can only be assigned to Crew). After that, the layers are assigned corresponding to their set player lists, which each assigned player being removed from other player lists.

The next phase is the handing out of targets. During this phase, layers with targets, such as Guesser, Lovers, Rivals and Executioner are given their targets. Lovers or Rivals who are left out will have that objectifier be removed.

The final phase is what I call "The Cleanup". During this phase, roles whose targets could not be assigned will be changed into roles that don't have targets. After that, empty layers are handed out to those who did not get any layer of that type prevent null errors during the game. Finally, certain variables for conversion are set, so that conversions are handled easier.

After that, it's gaming time.

-----------------------

# Credits & Resources

[Reactor](https://github.com/NuclearPowered/Reactor) - The framework of the mod\
[BepInEx](https://github.com/BepInEx) - For hooking game functions\
[Among-Us-Sheriff-Mod](https://github.com/Woodi-dev/Among-Us-Sheriff-Mod) - For the Sheriff role\
[Among-Us-Love-Couple-Mod](https://github.com/Woodi-dev/Among-Us-Love-Couple-Mod) - For the inspiration of Lovers objectifier\
[ExtraRolesAmongUs](https://github.com/NotHunter101/ExtraRolesAmongUs) - For the Engineer & Medic roles\
[TooManyRolesMods](https://github.com/Hardel-DW/TooManyRolesMods) - For the Investigator & Time Lord roles\
[Evan91380](https://github.com/Evan91380/BetterAirShip) & [Hardel](https://github.com/Hardel-DW) - For Better Airship code\
[TorchMod](https://github.com/tomozbot/TorchMod) - For the inspiration of the Torch ability\
[XtraCube](https://github.com/XtraCube) - For the custom colors\
[The Other Roles](https://github.com/Eisbison/TheOtherRoles) - For the inspiration of the Tracker role and the Bait modifier, cosmetics system, teleport animation and version control\
[The Other Roles: Community Edition](https://github.com/JustASysAdmin/TheOtherRoles2) - For the random spawns option\
[Las Monjas](https://github.com/KiraYamato94/LasMonjas) - For the inspiration of the VIP modifier\
[5up](https://www.twitch.tv/5uppp) and the Submarine Team - For the inspiration of the Grenadier role\
[MyDragonBreath](https://github.com/MyDragonBreath) - For Submerged Compatibility, MultiClientInstancing and partially the Operative role\
[Guus](https://github.com/OhMyGuus) - For support for the old Among Us versions (v2021.11.9.5 and v2021.12.15)\
[ItsTheNumberH](https://github.com/itsTheNumberH/Town-Of-H) - For Coward, Volatile and Bait modifiers, Poisoner and Cannibal roles, partially for Tracker and Coroner roles and bug fixes\
[Ruiner](https://github.com/ruiner189/Town-Of-Us-Redux) - For Lovers being changed into an objectifier and Task Tracking\
[Term](https://www.twitch.tv/termboii) - For creating Medium, Blackmailer and Plaguebearer, partially for Transporter and porting v2.5.0 to the new Among Us version (v2021.12.15)\
[Zeo](https://github.com/Zeo666/AllTheRoles) - For the idea of Ruthless and help with migrating to the use of shapeshifter menus\
[BryBry16](https://github.com/Brybry16/BetterPolus) - For the code used for Better Polus\
[Polus.gg Team](https://github.com/SubmergedAmongUs/Submerged) - For the Submerged map\
[Slushigoose](https://github.com/slushiegoose) - For making the mod in the first place\
[eDonnes](https://github.com/eDonnes124/) - For continuing the mod after it was discontinued\
[Det](https://github.com/FERTAILS) - For getting me into modding\
[TownOfHost-TheOtherRoles](https://github.com/music-discussion/TownOfHost-TheOtherRoles) - For a lot of the QoL code plus some chat command ideas\
[Essentials](https://github.com/DorCoMaNdO/Reactor-Essentials) - For creating custom game options which are now embedded into the mod [here](https://github.com/AlchlcDvl/TownOfUsReworked/tree/master/TownOfUsReworked/CustomOptions/Base.cs)\
[VincentVision](https://github.com/VincentVision) - For some code here and there from his version of Town Of Us (which was deleted sadly)\
[Lunastellia](https://github.com/Lunastellia) - For Better Skeld code\
[CrowdedMod](https://github.com/CrowdedMods/CrowdedMod) - For allowing to bypass the 15 player limit and is embedded into the mod\
[Town Of Salem](https://www.blankmediagames.com/TownOfSalem/), [Traitors In Salem](https://www.traitorsinsalem.com) & [Town Of Salem 2](https://store.steampowered.com/app/2140510/Town_of_Salem_2/) - For multiple role ideas

-----------------------

# License
This software is distributed under the GNU GPLv3 License. BepInEx is distributed under LGPL-2.1 License.

-----------------------

#
<p align="center">This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC.</p>
<p align="center">© Innersloth LLC.</p>
