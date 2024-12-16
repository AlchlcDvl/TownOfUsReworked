namespace TownOfUsReworked.Data;

public static class References
{
    public static readonly List<byte> RecentlyKilled = [];
    public static readonly Dictionary<byte, byte> CachedMorphs = [];
    public static readonly List<DeadPlayer> KilledPlayers = [];
    public static readonly Dictionary<byte, float> TransitioningSize = []; // Wheeze
    public static readonly Dictionary<byte, float> TransitioningSpeed = []; // Double wheeze
    public static readonly Dictionary<byte, DateTime> UninteractiblePlayers = [];
    public static readonly Dictionary<byte, float> UninteractiblePlayers2 = [];
    public static readonly Dictionary<byte, string> BodyLocations = [];
    public static DeadBody[] AllBodies() => UObject.FindObjectsOfType<DeadBody>();
    public static Vent[] AllVents() => UObject.FindObjectsOfType<Vent>();
    public static Vent[] AllMapVents() => Ship().AllVents;
    public static GameObject[] AllGameObjects() => UObject.FindObjectsOfType<GameObject>();
    public static Console[] AllConsoles() => UObject.FindObjectsOfType<Console>();
    public static SystemConsole[] AllSystemConsoles() => UObject.FindObjectsOfType<SystemConsole>();
    public static PlayerVoteArea[] AllVoteAreas() => Meeting().playerStates;
    public static List<PlayerControl> AllPlayers() => PlayerControl.AllPlayerControls.ToSystem();
    public static HudManager HUD() => HudManager.Instance;
    public static MeetingHud Meeting() => MeetingHud.Instance;
    public static ExileController Ejection() => ExileController.Instance;
    public static ShipStatus Ship() => ShipStatus.Instance;
    public static MapBehaviour Map() => MapBehaviour.Instance;
    public static Minigame ActiveTask() => Minigame.Instance;
    public static LobbyBehaviour Lobby() => LobbyBehaviour.Instance;
    public static ChatController Chat() => HUD().Chat;
    public static string FirstDead { get; set; }
    public static string CachedFirstDead { get; set; }
    public static bool Shapeshifted { get; set; }
    public static WinLose WinState { get; set; } = WinLose.None;
    public const string Everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ✧¢" +
        "乂⁂¤∮彡个「」人요〖〗ロ米卄王īl【】·ㅇ°◈◆◇◥◤◢◣《》︵︶☆☀☂☹☺♡♩♪♫♬✓☜☞☟☯☃✿❀÷º¿※⁑∞≠";
    public static readonly Dictionary<string, string> KeyWords = new()
    {
        { "%modversion%", TownOfUsReworked.VersionFinal },
        { "%discord%", $"[{TownOfUsReworked.DiscordInvite}]Discord[]" },
        { "%github%", $"[{TownOfUsReworked.GitHubLink}]GitHub[]" },
        { "%assets%", $"[{TownOfUsReworked.AssetsLink}]Assets[]" }
    };
    // As much as I hate to do this, people will take advantage so we're better off doing this early
    public static readonly string[] Profanities = [ "fuck", "bastard", "cunt", "nigg", "nig", "neg", "whore", "negro", "yiff", "rape", "rapist" ];
    public const string Disallowed = "@^[{(_-;:\"'.,\\|)}]+$!#$%^&&*?/";
    // public static readonly char[] Lowercase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    // public static readonly char[] Uppercase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public static readonly Vector2[] SkeldSpawns =
    [
        new(-2.2f, 2.2f), // Cafeteria. botton. top left
        new(0.7f, 2.2f), // Caffeteria. button. top right
        new(-2.2f, -0.2f), // Caffeteria. button. bottom left
        new(0.7f, -0.2f), // Caffeteria. button. bottom right
        new(10f, 3f), // Weapons top
        new(9.5f, -1f), // Weapons bottom
        new(6.5f, -3.5f), // O2
        new(11.5f, -3.5f), // O2-nav hall
        new(17, -3.5f), // Navigation top
        new(18.2f, -5.7f), // Navigation bottom
        new(11.5f, -6.5f), // Nav-shields top
        new(9.5f, -8.5f), // Nav-shields bottom
        new(9.2f, -12.2f), // Shields top
        new(8, -14.3f), // Shields bottom
        new(2.5f, -16f), // Comms left
        new(4.2f, -16.4f), // Comms middle
        new(5.5f, -16f), // Comms right
        new(-1.5f, -10), // Storage top
        new(-1.5f, -15.5f), // Storage bottom
        new(-4.5f, -12.5f), // Storage left
        new(0.3f, -12.5f), // Storage right
        new(4.5f, -7.5f), // Admin top
        new(4.5f, -9.5f), // Admin bottom
        new(-9, -8), // Elec top left
        new(-6, -8), // Elec top right
        new(-8, -11), // Elec bottom
        new(-12, -13), // Elec-lower hall
        new(-17f, -10f), // Lower engine top
        new(-17, -13), // Lower engine bottom
        new(-21.5f, -3), // Reactor top
        new(-21.5f, -8), // Reactor bottom
        new(-13, -3), // Security top
        new(-12.6f, -5.6f), // Security bottom
        new(-17, 2.5f), // Upper engibe top
        new(-17, -1), // Upper engine bottom
        new(-10.5f, 1), // Upper-mad hall
        new(-10.5f, -2), // Medbay top
        new(-6.5f, -4.5f) // Medbay bottom
    ];
    public static readonly Vector2[] MiraSpawns =
    [
        new(-4.5f, 3.5f), // Launchpad top
        new(-4.5f, -1.4f), // Launchpad bottom
        new(8.5f, -1f), // Launchpad- med hall
        new(14f, -1.5f), // Medbay
        new(16.5f, 3f), // Comms
        new(10f, 5f), // Lockers
        new(6f, 1.5f), // Locker room
        new(2.5f, 13.6f), // Reactor
        new(6f, 12f), // Reactor middle
        new(9.5f, 13f), // Lab
        new(15f, 9f), // Bottom left cross
        new(17.9f, 11.5f), // Middle cross
        new(14f, 17.3f), // Office
        new(19.5f, 21f), // Admin
        new(14f, 24f), // Greenhouse left
        new(22f, 24f), // Greenhouse right
        new(21f, 8.5f), // Bottom right cross
        new(28f, 3f), // Caf right
        new(22f, 3f), // Caf left
        new(19f, 4f), // Storage
        new(22f, -2f) // Balcony
    ];
    public static readonly Vector2[] PolusSpawns =
    [
        new(16.6f, -1f), // Dropship top
        new(16.6f, -5f), // Dropship bottom
        new(20f, -9f), // Above storage
        new(22f, -7f), // Right fuel
        new(25.5f, -6.9f), // Drill
        new(29f, -9.5f), // Lab lockers
        new(29.5f, -8f), // Lab weather notes
        new(35f, -7.6f), // Lab table
        new(40.4f, -8f), // Lab scan
        new(33f, -10f), // Lab toilet
        new(39f, -15f), // Specimen hall top
        new(36.5f, -19.5f), // Specimen top
        new(36.5f, -21f), // Specimen bottom
        new(28f, -21f), // Specimen hall bottom
        new(24f, -20.5f), // Admin tv
        new(22f, -25f), // Admin books
        new(16.6f, -17.5f), // Office coffe
        new(22.5f, -16.5f), // Office projector
        new(24f, -17f), // Office figure
        new(27f, -16.5f), // Office lifelines
        new(32.7f, -15.7f), // Lavapool
        new(31.5f, -12f), // Snowmad below lab
        new(10f, -14f), // Below storage
        new(19f, -11f), // Storage toolrack
        new(12f, -7f), // Left fuel
        new(5f, -7.5f), // Above elec
        new(10f, -12f), // Elec fence
        new(9f, -9f), // Elec lockers
        new(5f, -9f), // Elec window
        new(4f, -11.2f), // Elec tapes
        new(5.5f, -16f), // Elec-O2 hall
        new(1f, -17.5f), // O2 tree
        new(3f, -21f), // O2 middle
        new(2f, -19f), // O2 gas
        new(1f, -24f), // O2 water
        new(7f, -24f), // Under O2
        new(9f, -20f), // Right outside of O2
        new(6.9f, -13.8f), // Snowman under elec
        new(11f, -17f), // Comms table
        new(12.7f, -15.5f), // Comms antenna pult
        new(12f, -21.5f), // Weapons window
        new(15f, -17f), // Between coms-office
        new(17.5f, -25.7f) // Snowman under office
    ];
    public static readonly Vector2[] dlekSSpawns =
    [
        new(2.2f, 2.2f), // Cafeteria. botton. top left
        new(-0.7f, 2.2f), // Caffeteria. button. top right
        new(2.2f, -0.2f), // Caffeteria. button. bottom left
        new(-0.7f, -0.2f), // Caffeteria. button. bottom right
        new(-10, 3f), // Weapons top
        new(-9, 1f), // Weapons bottom
        new(-6.5f, -3.5f), // O2
        new(-11.5f, -3.5f), // O2-nav hall
        new(-17, -3.5f), // Navigation top
        new(-18.2f, -5.7f), // Navigation bottom
        new(-11.5f, -6.5f), // Nav-shields top
        new(-9.5f, -8.5f), // Nav-shields bottom
        new(-9.2f, -12.2f), // Shields top
        new(-8f, -14.3f), // Shields bottom
        new(-2.5f, -16f), // Comms left
        new(-4.2f, -16.4f), // Comms middle
        new(-5.5f, -16f), // Comms right
        new(1.5f, -10), // Storage top
        new(1.5f, -15.5f), // Storage bottom
        new(4.5f, -12.5f), // Storage left
        new(-0.3f, -12.5f), // Storage right
        new(-4.5f, -7.5f), // Admin top
        new(-4.5f, -9.5f), // Admin bottom
        new(9, -8), // Elec top left
        new(6, -8), // Elec top right
        new(8, -11), // Elec bottom
        new(12, -13), // Elec-lower hall
        new(17f, -10f), // Lower engine top
        new(17, -13), // Lower engine bottom
        new(21.5f, -3), // Reactor top
        new(21.5f, -8), // Reactor bottom
        new(13, -3), // Security top
        new(12.6f, -5.6f), // Security bottom
        new(17, 2.5f), // Upper engibe top
        new(17, -1), // Upper engine bottom
        new(10.5f, 1), // Upper-mad hall
        new(10.5f, -2), // Medbay top
        new(6.5f, -4.5f) // Medbay bottom
    ];
    public static readonly Dictionary<LayerEnum, LayerDictionaryEntry> LayerDictionary = new()
    {
        { LayerEnum.Altruist, new(typeof(Altruist), CustomColorManager.Altruist, "Altruist") },
        { LayerEnum.Bastion, new(typeof(Bastion), CustomColorManager.Bastion, "Bastion") },
        { LayerEnum.Chameleon, new(typeof(Chameleon), CustomColorManager.Chameleon, "Chameleon") },
        { LayerEnum.Coroner, new(typeof(Coroner), CustomColorManager.Coroner, "Coroner") },
        { LayerEnum.Crewmate, new(typeof(Crewmate), CustomColorManager.Crew, "Crewmate") },
        { LayerEnum.Detective, new(typeof(Detective), CustomColorManager.Detective, "Detective") },
        { LayerEnum.Dictator, new(typeof(Dictator), CustomColorManager.Dictator, "Dictator") },
        { LayerEnum.Engineer, new(typeof(Engineer), CustomColorManager.Engineer, "Engineer") },
        { LayerEnum.Escort, new(typeof(Escort), CustomColorManager.Escort, "Escort") },
        { LayerEnum.Mayor, new(typeof(Mayor), CustomColorManager.Mayor, "Mayor") },
        { LayerEnum.Medic, new(typeof(Medic), CustomColorManager.Medic, "Medic") },
        { LayerEnum.Medium, new(typeof(Medium), CustomColorManager.Medium, "Medium") },
        { LayerEnum.Monarch, new(typeof(Monarch), CustomColorManager.Monarch, "Monarch") },
        { LayerEnum.Mystic, new(typeof(Mystic), CustomColorManager.Mystic, "Mystic") },
        { LayerEnum.Operative, new(typeof(Operative), CustomColorManager.Operative, "Operative") },
        { LayerEnum.Retributionist, new(typeof(Retributionist), CustomColorManager.Retributionist, "Retributionist") },
        { LayerEnum.Revealer, new(typeof(Revealer), CustomColorManager.Revealer, "Revealer") },
        { LayerEnum.Seer, new(typeof(Seer), CustomColorManager.Seer, "Seer") },
        { LayerEnum.Sheriff, new(typeof(Sheriff), CustomColorManager.Sheriff, "Sheriff") },
        { LayerEnum.Shifter, new(typeof(Shifter), CustomColorManager.Shifter, "Shifter") },
        { LayerEnum.Tracker, new(typeof(Tracker), CustomColorManager.Tracker, "Tracker") },
        { LayerEnum.Transporter, new(typeof(Transporter), CustomColorManager.Transporter, "Transporter") },
        { LayerEnum.Trapper, new(typeof(Trapper), CustomColorManager.Trapper, "Trapper") },
        { LayerEnum.VampireHunter, new(typeof(VampireHunter), CustomColorManager.VampireHunter, "Vampire Hunter") },
        { LayerEnum.Veteran, new(typeof(Veteran), CustomColorManager.Veteran, "Veteran") },
        { LayerEnum.Vigilante, new(typeof(Vigilante), CustomColorManager.Vigilante, "Vigilante") },
        { LayerEnum.Actor, new(typeof(Actor), CustomColorManager.Actor, "Actor") },
        { LayerEnum.Amnesiac, new(typeof(Amnesiac), CustomColorManager.Amnesiac, "Amnesiac") },
        { LayerEnum.Arsonist, new(typeof(Arsonist), CustomColorManager.Arsonist, "Arsonist") },
        { LayerEnum.Betrayer, new(typeof(Betrayer), CustomColorManager.Betrayer, "Betrayer") },
        { LayerEnum.BountyHunter, new(typeof(BountyHunter), CustomColorManager.BountyHunter, "Bounty Hunter") },
        { LayerEnum.Cannibal, new(typeof(Cannibal), CustomColorManager.Cannibal, "Cannibal") },
        { LayerEnum.Cryomaniac, new(typeof(Cryomaniac), CustomColorManager.Cryomaniac, "Cryomaniac") },
        { LayerEnum.Dracula, new(typeof(Dracula), CustomColorManager.Dracula, "Dracula") },
        { LayerEnum.Executioner, new(typeof(Executioner), CustomColorManager.Executioner, "Executioner") },
        { LayerEnum.Glitch, new(typeof(Glitch), CustomColorManager.Glitch, "Glitch") },
        { LayerEnum.GuardianAngel, new(typeof(GuardianAngel), CustomColorManager.GuardianAngel, "Guardian Angel") },
        { LayerEnum.Guesser, new(typeof(Guesser), CustomColorManager.Guesser, "Guesser") },
        { LayerEnum.Jackal, new(typeof(Jackal), CustomColorManager.Jackal, "Jackal") },
        { LayerEnum.Jester, new(typeof(Jester), CustomColorManager.Jester, "Jester") },
        { LayerEnum.Juggernaut, new(typeof(Juggernaut), CustomColorManager.Juggernaut, "Juggernaut") },
        { LayerEnum.Murderer, new(typeof(Murderer), CustomColorManager.Murderer, "Murderer") },
        { LayerEnum.Necromancer, new(typeof(Necromancer), CustomColorManager.Necromancer, "Necromancer") },
        { LayerEnum.Pestilence, new(typeof(Pestilence), CustomColorManager.Pestilence, "Pestilence") },
        { LayerEnum.Phantom, new(typeof(Phantom), CustomColorManager.Phantom, "Phantom") },
        { LayerEnum.Plaguebearer, new(typeof(Plaguebearer), CustomColorManager.Plaguebearer, "Plaguebearer") },
        { LayerEnum.SerialKiller, new(typeof(SerialKiller), CustomColorManager.SerialKiller, "Serial Killer") },
        { LayerEnum.Survivor, new(typeof(Survivor), CustomColorManager.Survivor, "Survivor") },
        { LayerEnum.Thief, new(typeof(Thief), CustomColorManager.Thief, "Thief") },
        { LayerEnum.Troll, new(typeof(Troll), CustomColorManager.Troll, "Troll") },
        { LayerEnum.Werewolf, new(typeof(Werewolf), CustomColorManager.Werewolf, "Werewolf") },
        { LayerEnum.Whisperer, new(typeof(Whisperer), CustomColorManager.Whisperer, "Whisperer") },
        { LayerEnum.Ambusher, new(typeof(Ambusher), CustomColorManager.Ambusher, "Ambusher") },
        { LayerEnum.Blackmailer, new(typeof(Blackmailer), CustomColorManager.Blackmailer, "Blackmailer") },
        { LayerEnum.Camouflager, new(typeof(Camouflager), CustomColorManager.Camouflager, "Camouflager") },
        { LayerEnum.Consigliere, new(typeof(Consigliere), CustomColorManager.Consigliere, "Consigliere") },
        { LayerEnum.Consort, new(typeof(Consort), CustomColorManager.Consort, "Consort") },
        { LayerEnum.Disguiser, new(typeof(Disguiser), CustomColorManager.Disguiser, "Disguiser") },
        { LayerEnum.Enforcer, new(typeof(Enforcer), CustomColorManager.Enforcer, "Enforcer") },
        { LayerEnum.Ghoul, new(typeof(Ghoul), CustomColorManager.Ghoul, "Ghoul") },
        { LayerEnum.Godfather, new(typeof(Godfather), CustomColorManager.Godfather, "Godfather") },
        { LayerEnum.Grenadier, new(typeof(Grenadier), CustomColorManager.Grenadier, "Grenadier") },
        { LayerEnum.Impostor, new(typeof(Impostor), CustomColorManager.Intruder, "Impostor") },
        { LayerEnum.Janitor, new(typeof(Janitor), CustomColorManager.Janitor, "Janitor") },
        { LayerEnum.Mafioso, new(typeof(Mafioso), CustomColorManager.Mafioso, "Mafioso") },
        { LayerEnum.Miner, new(typeof(Miner), CustomColorManager.Miner, "Miner") },
        { LayerEnum.Morphling, new(typeof(Morphling), CustomColorManager.Morphling, "Morphling") },
        { LayerEnum.Teleporter, new(typeof(Teleporter), CustomColorManager.Teleporter, "Teleporter") },
        { LayerEnum.Wraith, new(typeof(Wraith), CustomColorManager.Wraith, "Wraith") },
        { LayerEnum.Anarchist, new(typeof(Anarchist), CustomColorManager.Syndicate, "Anarchist") },
        { LayerEnum.Banshee, new(typeof(Banshee), CustomColorManager.Banshee, "Banshee") },
        { LayerEnum.Bomber, new(typeof(Bomber), CustomColorManager.Bomber, "Bomber") },
        { LayerEnum.Collider, new(typeof(PlayerLayers.Roles.Collider), CustomColorManager.Collider, "Collider") },
        { LayerEnum.Concealer, new(typeof(Concealer), CustomColorManager.Concealer, "Concealer") },
        { LayerEnum.Crusader, new(typeof(Crusader), CustomColorManager.Crusader, "Crusader") },
        { LayerEnum.Drunkard, new(typeof(Drunkard), CustomColorManager.Drunkard, "Drunkard") },
        { LayerEnum.Framer, new(typeof(Framer), CustomColorManager.Framer, "Framer") },
        { LayerEnum.Poisoner, new(typeof(Poisoner), CustomColorManager.Poisoner, "Poisoner") },
        { LayerEnum.Rebel, new(typeof(Rebel), CustomColorManager.Rebel, "Rebel") },
        { LayerEnum.Shapeshifter, new(typeof(Shapeshifter), CustomColorManager.Shapeshifter, "Shapeshifter") },
        { LayerEnum.Sidekick, new(typeof(Sidekick), CustomColorManager.Sidekick, "Sidekick") },
        { LayerEnum.Silencer, new(typeof(Silencer), CustomColorManager.Silencer, "Silencer") },
        { LayerEnum.Spellslinger, new(typeof(Spellslinger), CustomColorManager.Spellslinger, "Spellslinger") },
        { LayerEnum.Stalker, new(typeof(Stalker), CustomColorManager.Stalker, "Stalker") },
        { LayerEnum.Timekeeper, new(typeof(Timekeeper), CustomColorManager.Timekeeper, "Timekeeper") },
        { LayerEnum.Warper, new(typeof(Warper), CustomColorManager.Warper, "Warper") },
        { LayerEnum.Hunter, new(typeof(Hunter), CustomColorManager.Hunter, "Hunter") },
        { LayerEnum.Hunted, new(typeof(Hunted), CustomColorManager.Hunted, "Hunted") },
        { LayerEnum.Runner, new(typeof(Runner), CustomColorManager.Runner, "Runner") },
        { LayerEnum.Astral, new(typeof(Astral), CustomColorManager.Astral, "Astral") },
        { LayerEnum.Bait, new(typeof(Bait), CustomColorManager.Bait, "Bait") },
        { LayerEnum.Colorblind, new(typeof(Colorblind), CustomColorManager.Colorblind, "Colorblind") },
        { LayerEnum.Coward, new(typeof(Coward), CustomColorManager.Coward, "Coward") },
        { LayerEnum.Diseased, new(typeof(Diseased), CustomColorManager.Diseased, "Diseased") },
        { LayerEnum.Drunk, new(typeof(Drunk), CustomColorManager.Drunk, "Drunk") },
        { LayerEnum.Dwarf, new(typeof(Dwarf), CustomColorManager.Dwarf, "Dwarf") },
        { LayerEnum.Giant, new(typeof(Giant), CustomColorManager.Giant, "Giant") },
        { LayerEnum.Indomitable, new(typeof(Indomitable), CustomColorManager.Indomitable, "Indomitable") },
        { LayerEnum.Professional, new(typeof(Professional), CustomColorManager.Professional, "Professional") },
        { LayerEnum.Shy, new(typeof(Shy), CustomColorManager.Shy, "Shy") },
        { LayerEnum.VIP, new(typeof(VIP), CustomColorManager.VIP, "VIP") },
        { LayerEnum.Volatile, new(typeof(Volatile), CustomColorManager.Volatile, "Volatile") },
        { LayerEnum.Yeller, new(typeof(Yeller), CustomColorManager.Yeller, "Yeller") },
        { LayerEnum.Allied, new(typeof(Allied), CustomColorManager.Allied, "Allied") },
        { LayerEnum.Corrupted, new(typeof(Corrupted), CustomColorManager.Corrupted, "Corrupted") },
        { LayerEnum.Defector, new(typeof(Defector), CustomColorManager.Defector, "Defector") },
        { LayerEnum.Fanatic, new(typeof(Fanatic), CustomColorManager.Fanatic, "Fanatic") },
        { LayerEnum.Linked, new(typeof(Linked), CustomColorManager.Linked, "Linked") },
        { LayerEnum.Lovers, new(typeof(Lovers), CustomColorManager.Lovers, "Lovers") },
        { LayerEnum.Mafia, new(typeof(Mafia), CustomColorManager.Mafia, "Mafia") },
        { LayerEnum.Overlord, new(typeof(Overlord), CustomColorManager.Overlord, "Overlord") },
        { LayerEnum.Rivals, new(typeof(Rivals), CustomColorManager.Rivals, "Rivals") },
        { LayerEnum.Taskmaster, new(typeof(Taskmaster), CustomColorManager.Taskmaster, "Taskmaster") },
        { LayerEnum.Traitor, new(typeof(Traitor), CustomColorManager.Traitor, "Traitor") },
        { LayerEnum.Assassin, new(typeof(Assassin), CustomColorManager.Assassin, "Assassin") },
        { LayerEnum.Bullseye, new(typeof(Bullseye), CustomColorManager.Crew, "Bullseye") },
        { LayerEnum.ButtonBarry, new(typeof(ButtonBarry), CustomColorManager.ButtonBarry, "Button Barry") },
        { LayerEnum.Hitman, new(typeof(Hitman), CustomColorManager.Intruder, "Hitman") },
        { LayerEnum.Insider, new(typeof(Insider), CustomColorManager.Insider, "Insider") },
        { LayerEnum.Multitasker, new(typeof(Multitasker), CustomColorManager.Multitasker, "Multitasker") },
        { LayerEnum.Ninja, new(typeof(Ninja), CustomColorManager.Ninja, "Ninja") },
        { LayerEnum.Politician, new(typeof(Politician), CustomColorManager.Politician, "Politician") },
        { LayerEnum.Radar, new(typeof(Radar), CustomColorManager.Radar, "Radar") },
        { LayerEnum.Ruthless, new(typeof(Ruthless), CustomColorManager.Ruthless, "Ruthless") },
        { LayerEnum.Slayer, new(typeof(Slayer), CustomColorManager.Neutral, "Slayer") },
        { LayerEnum.Sniper, new(typeof(Sniper), CustomColorManager.Syndicate, "Sniper") },
        { LayerEnum.Snitch, new(typeof(Snitch), CustomColorManager.Snitch, "Snitch") },
        { LayerEnum.Swapper, new(typeof(Swapper), CustomColorManager.Swapper, "Swapper") },
        { LayerEnum.Tiebreaker, new(typeof(Tiebreaker), CustomColorManager.Tiebreaker, "Tiebreaker") },
        { LayerEnum.Torch, new(typeof(Torch), CustomColorManager.Torch, "Torch") },
        { LayerEnum.Tunneler, new(typeof(Tunneler), CustomColorManager.Tunneler, "Tunneler") },
        { LayerEnum.Underdog, new(typeof(Underdog), CustomColorManager.Underdog, "Underdog") },
        { LayerEnum.Cabal, new(null, CustomColorManager.Cabal, "Cabal") },
        { LayerEnum.Sect, new(null, CustomColorManager.Sect, "Sect") },
        { LayerEnum.Reanimated, new(null, CustomColorManager.Reanimated, "Reanimated") },
        { LayerEnum.Undead, new(null, CustomColorManager.Undead, "Undead") }
    };
    public static readonly string[] Splashes =
    [
        "Oh boy, here I go killing again",
        "Screwed up since 2069",
        "We were bad, but now we're good",
        "Count the bodies",
        "I need my knife, where is it?",
        "You son of a trash can, I'm in",
        "real",
        "bous",
        "My life be like",
        "Man I'm ded",
        "gaming",
        "WHO LET BRO COOK?",
        "",
        "Let me introduce you to our sponsor, Raid-",
        "Push to production is my motto. Bugs? meh public release go brrrr",
        "It's not a bug, it's a feature",
        "My life like a movie",
        "real",
        "WHERE AM I?!",
        "OOOOO a nice view :)",
        "What dis",
        "O_O",
        "o_O",
        "O_o",
        "o_o",
        "._o",
        "o_.",
        "._.",
        "._O",
        "O_.",
        "ehehehehehehehehe",
        "Think fast chuckle nuts",
        "LET. HIM. COOK."
    ];
}