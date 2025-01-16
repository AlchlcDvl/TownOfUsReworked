namespace TownOfUsReworked.Data;

public static class References
{
    public static readonly List<byte> RecentlyKilled = [];
    public static readonly List<byte> Cleaned = [];
    public static readonly List<byte> Moving = [];
    public static readonly Dictionary<byte, byte> CachedMorphs = [];
    public static readonly List<DeadPlayer> KilledPlayers = [];
    public static readonly Dictionary<byte, float> TransitioningSize = []; // Wheeze
    public static readonly Dictionary<byte, float> TransitioningSpeed = []; // Double wheeze
    public static readonly Dictionary<byte, float> UninteractiblePlayers = [];
    public static readonly Dictionary<byte, float> UninteractiblePlayers2 = [];
    public static readonly Dictionary<byte, string> BodyLocations = [];
    public static readonly Dictionary<byte, int> KillCounts = [];
    public static DeadBody[] AllBodies() => UObject.FindObjectsOfType<DeadBody>();
    public static Vent[] AllVents() => UObject.FindObjectsOfType<Vent>();
    public static Vent[] AllMapVents() => Ship().AllVents;
    public static GameObject[] AllGameObjects() => UObject.FindObjectsOfType<GameObject>();
    public static Console[] AllConsoles() => UObject.FindObjectsOfType<Console>();
    public static SystemConsole[] AllSystemConsoles() => UObject.FindObjectsOfType<SystemConsole>();
    public static PlayerVoteArea[] AllVoteAreas() => Meeting().playerStates;
    public static IEnumerable<PlayerControl> AllPlayers() => PlayerControl.AllPlayerControls.ToSystem();
    public static HudManager HUD() => HudManager.Instance;
    public static MeetingHud Meeting() => MeetingHud.Instance;
    public static ExileController Ejection() => ExileController.Instance;
    public static ShipStatus Ship() => ShipStatus.Instance;
    public static MapBehaviour Map() => MapBehaviour.Instance;
    public static Minigame ActiveTask() => Minigame.Instance;
    public static LobbyBehaviour Lobby() => LobbyBehaviour.Instance;
    public static ChatController Chat() => HUD()?.Chat;
    public static string FirstDead { get; set; }
    public static string CachedFirstDead { get; set; }
    public static string MostRecentKiller { get; set; }
    public static bool Shapeshifted { get; set; }
    public static WinLose WinState { get; set; } = WinLose.None;
    public static StringNames ReworkedStart { get; set; }
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
    public static readonly string[] Profanities = [ "nigg", "whore", "negro", "yiff", "rape", "rapist" ];
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
        { LayerEnum.Altruist, new(typeof(Altruist), CustomColorManager.Altruist, LayerEnum.Altruist) },
        { LayerEnum.Bastion, new(typeof(Bastion), CustomColorManager.Bastion, LayerEnum.Bastion) },
        { LayerEnum.Chameleon, new(typeof(Chameleon), CustomColorManager.Chameleon, LayerEnum.Chameleon) },
        { LayerEnum.Coroner, new(typeof(Coroner), CustomColorManager.Coroner, LayerEnum.Coroner) },
        { LayerEnum.Crewmate, new(typeof(Crewmate), CustomColorManager.Crew, LayerEnum.Crewmate) },
        { LayerEnum.Detective, new(typeof(Detective), CustomColorManager.Detective, LayerEnum.Detective) },
        { LayerEnum.Dictator, new(typeof(Dictator), CustomColorManager.Dictator, LayerEnum.Dictator) },
        { LayerEnum.Engineer, new(typeof(Engineer), CustomColorManager.Engineer, LayerEnum.Engineer) },
        { LayerEnum.Escort, new(typeof(Escort), CustomColorManager.Escort, LayerEnum.Escort) },
        { LayerEnum.Mayor, new(typeof(Mayor), CustomColorManager.Mayor, LayerEnum.Mayor) },
        { LayerEnum.Medic, new(typeof(Medic), CustomColorManager.Medic, LayerEnum.Medic) },
        { LayerEnum.Medium, new(typeof(Medium), CustomColorManager.Medium, LayerEnum.Medium) },
        { LayerEnum.Monarch, new(typeof(Monarch), CustomColorManager.Monarch, LayerEnum.Monarch) },
        { LayerEnum.Mystic, new(typeof(Mystic), CustomColorManager.Mystic, LayerEnum.Mystic) },
        { LayerEnum.Operative, new(typeof(Operative), CustomColorManager.Operative, LayerEnum.Operative) },
        { LayerEnum.Retributionist, new(typeof(Retributionist), CustomColorManager.Retributionist, LayerEnum.Retributionist) },
        { LayerEnum.Revealer, new(typeof(Revealer), CustomColorManager.Revealer, LayerEnum.Revealer) },
        { LayerEnum.Seer, new(typeof(Seer), CustomColorManager.Seer, LayerEnum.Seer) },
        { LayerEnum.Sheriff, new(typeof(Sheriff), CustomColorManager.Sheriff, LayerEnum.Sheriff) },
        { LayerEnum.Shifter, new(typeof(Shifter), CustomColorManager.Shifter, LayerEnum.Shifter) },
        { LayerEnum.Tracker, new(typeof(Tracker), CustomColorManager.Tracker, LayerEnum.Tracker) },
        { LayerEnum.Transporter, new(typeof(Transporter), CustomColorManager.Transporter, LayerEnum.Transporter) },
        { LayerEnum.Trapper, new(typeof(Trapper), CustomColorManager.Trapper, LayerEnum.Trapper) },
        { LayerEnum.Trickster, new(typeof(Trickster), CustomColorManager.Trickster, LayerEnum.Trickster) },
        { LayerEnum.Veteran, new(typeof(Veteran), CustomColorManager.Veteran, LayerEnum.Veteran) },
        { LayerEnum.Vigilante, new(typeof(Vigilante), CustomColorManager.Vigilante, LayerEnum.Vigilante) },
        { LayerEnum.Actor, new(typeof(Actor), CustomColorManager.Actor, LayerEnum.Actor) },
        { LayerEnum.Amnesiac, new(typeof(Amnesiac), CustomColorManager.Amnesiac, LayerEnum.Amnesiac) },
        { LayerEnum.Arsonist, new(typeof(Arsonist), CustomColorManager.Arsonist, LayerEnum.Arsonist) },
        { LayerEnum.Betrayer, new(typeof(Betrayer), CustomColorManager.Betrayer, LayerEnum.Betrayer) },
        { LayerEnum.BountyHunter, new(typeof(BountyHunter), CustomColorManager.BountyHunter, LayerEnum.BountyHunter) },
        { LayerEnum.Cannibal, new(typeof(Cannibal), CustomColorManager.Cannibal, LayerEnum.Cannibal) },
        { LayerEnum.Cryomaniac, new(typeof(Cryomaniac), CustomColorManager.Cryomaniac, LayerEnum.Cryomaniac) },
        { LayerEnum.Dracula, new(typeof(Dracula), CustomColorManager.Dracula, LayerEnum.Dracula) },
        { LayerEnum.Executioner, new(typeof(Executioner), CustomColorManager.Executioner, LayerEnum.Executioner) },
        { LayerEnum.Glitch, new(typeof(Glitch), CustomColorManager.Glitch, LayerEnum.Glitch) },
        { LayerEnum.GuardianAngel, new(typeof(GuardianAngel), CustomColorManager.GuardianAngel, LayerEnum.GuardianAngel) },
        { LayerEnum.Guesser, new(typeof(Guesser), CustomColorManager.Guesser, LayerEnum.Guesser) },
        { LayerEnum.Jackal, new(typeof(Jackal), CustomColorManager.Jackal, LayerEnum.Jackal) },
        { LayerEnum.Jester, new(typeof(Jester), CustomColorManager.Jester, LayerEnum.Jester) },
        { LayerEnum.Juggernaut, new(typeof(Juggernaut), CustomColorManager.Juggernaut, LayerEnum.Juggernaut) },
        { LayerEnum.Murderer, new(typeof(Murderer), CustomColorManager.Murderer, LayerEnum.Murderer) },
        { LayerEnum.Necromancer, new(typeof(Necromancer), CustomColorManager.Necromancer, LayerEnum.Necromancer) },
        { LayerEnum.Pestilence, new(typeof(Pestilence), CustomColorManager.Pestilence, LayerEnum.Pestilence) },
        { LayerEnum.Phantom, new(typeof(Phantom), CustomColorManager.Phantom, LayerEnum.Phantom) },
        { LayerEnum.Plaguebearer, new(typeof(Plaguebearer), CustomColorManager.Plaguebearer, LayerEnum.Plaguebearer) },
        { LayerEnum.SerialKiller, new(typeof(SerialKiller), CustomColorManager.SerialKiller, LayerEnum.SerialKiller) },
        { LayerEnum.Survivor, new(typeof(Survivor), CustomColorManager.Survivor, LayerEnum.Survivor) },
        { LayerEnum.Thief, new(typeof(Thief), CustomColorManager.Thief, LayerEnum.Thief) },
        { LayerEnum.Troll, new(typeof(Troll), CustomColorManager.Troll, LayerEnum.Troll) },
        { LayerEnum.Werewolf, new(typeof(Werewolf), CustomColorManager.Werewolf, LayerEnum.Werewolf) },
        { LayerEnum.Whisperer, new(typeof(Whisperer), CustomColorManager.Whisperer, LayerEnum.Whisperer) },
        { LayerEnum.Ambusher, new(typeof(Ambusher), CustomColorManager.Ambusher, LayerEnum.Ambusher) },
        { LayerEnum.Blackmailer, new(typeof(Blackmailer), CustomColorManager.Blackmailer, LayerEnum.Blackmailer) },
        { LayerEnum.Camouflager, new(typeof(Camouflager), CustomColorManager.Camouflager, LayerEnum.Camouflager) },
        { LayerEnum.Consigliere, new(typeof(Consigliere), CustomColorManager.Consigliere, LayerEnum.Consigliere) },
        { LayerEnum.Consort, new(typeof(Consort), CustomColorManager.Consort, LayerEnum.Consort) },
        { LayerEnum.Disguiser, new(typeof(Disguiser), CustomColorManager.Disguiser, LayerEnum.Disguiser) },
        { LayerEnum.Enforcer, new(typeof(Enforcer), CustomColorManager.Enforcer, LayerEnum.Enforcer) },
        { LayerEnum.Ghoul, new(typeof(Ghoul), CustomColorManager.Ghoul, LayerEnum.Ghoul) },
        { LayerEnum.Godfather, new(typeof(Godfather), CustomColorManager.Godfather, LayerEnum.Godfather) },
        { LayerEnum.Grenadier, new(typeof(Grenadier), CustomColorManager.Grenadier, LayerEnum.Grenadier) },
        { LayerEnum.Impostor, new(typeof(Impostor), CustomColorManager.Intruder, LayerEnum.Impostor) },
        { LayerEnum.Janitor, new(typeof(Janitor), CustomColorManager.Janitor, LayerEnum.Janitor) },
        { LayerEnum.Mafioso, new(typeof(Mafioso), CustomColorManager.Mafioso, LayerEnum.Mafioso) },
        { LayerEnum.Miner, new(typeof(Miner), CustomColorManager.Miner, LayerEnum.Miner) },
        { LayerEnum.Morphling, new(typeof(Morphling), CustomColorManager.Morphling, LayerEnum.Morphling) },
        { LayerEnum.Teleporter, new(typeof(Teleporter), CustomColorManager.Teleporter, LayerEnum.Teleporter) },
        { LayerEnum.Wraith, new(typeof(Wraith), CustomColorManager.Wraith, LayerEnum.Wraith) },
        { LayerEnum.Anarchist, new(typeof(Anarchist), CustomColorManager.Syndicate, LayerEnum.Anarchist) },
        { LayerEnum.Banshee, new(typeof(Banshee), CustomColorManager.Banshee, LayerEnum.Banshee) },
        { LayerEnum.Bomber, new(typeof(Bomber), CustomColorManager.Bomber, LayerEnum.Bomber) },
        { LayerEnum.Collider, new(typeof(PlayerLayers.Roles.Collider), CustomColorManager.Collider, LayerEnum.Collider) },
        { LayerEnum.Concealer, new(typeof(Concealer), CustomColorManager.Concealer, LayerEnum.Concealer) },
        { LayerEnum.Crusader, new(typeof(Crusader), CustomColorManager.Crusader, LayerEnum.Crusader) },
        { LayerEnum.Drunkard, new(typeof(Drunkard), CustomColorManager.Drunkard, LayerEnum.Drunkard) },
        { LayerEnum.Framer, new(typeof(Framer), CustomColorManager.Framer, LayerEnum.Framer) },
        { LayerEnum.Poisoner, new(typeof(Poisoner), CustomColorManager.Poisoner, LayerEnum.Poisoner) },
        { LayerEnum.Rebel, new(typeof(Rebel), CustomColorManager.Rebel, LayerEnum.Rebel) },
        { LayerEnum.Shapeshifter, new(typeof(Shapeshifter), CustomColorManager.Shapeshifter, LayerEnum.Shapeshifter) },
        { LayerEnum.Sidekick, new(typeof(Sidekick), CustomColorManager.Sidekick, LayerEnum.Sidekick) },
        { LayerEnum.Silencer, new(typeof(Silencer), CustomColorManager.Silencer, LayerEnum.Silencer) },
        { LayerEnum.Spellslinger, new(typeof(Spellslinger), CustomColorManager.Spellslinger, LayerEnum.Spellslinger) },
        { LayerEnum.Stalker, new(typeof(Stalker), CustomColorManager.Stalker, LayerEnum.Stalker) },
        { LayerEnum.Timekeeper, new(typeof(Timekeeper), CustomColorManager.Timekeeper, LayerEnum.Timekeeper) },
        { LayerEnum.Warper, new(typeof(Warper), CustomColorManager.Warper, LayerEnum.Warper) },
        { LayerEnum.Hunter, new(typeof(Hunter), CustomColorManager.Hunter, LayerEnum.Hunter) },
        { LayerEnum.Hunted, new(typeof(Hunted), CustomColorManager.Hunted, LayerEnum.Hunted) },
        { LayerEnum.Runner, new(typeof(Runner), CustomColorManager.Runner, LayerEnum.Runner) },
        { LayerEnum.Astral, new(typeof(Astral), CustomColorManager.Astral, LayerEnum.Astral) },
        { LayerEnum.Bait, new(typeof(Bait), CustomColorManager.Bait, LayerEnum.Bait) },
        { LayerEnum.Colorblind, new(typeof(Colorblind), CustomColorManager.Colorblind, LayerEnum.Colorblind) },
        { LayerEnum.Coward, new(typeof(Coward), CustomColorManager.Coward, LayerEnum.Coward) },
        { LayerEnum.Diseased, new(typeof(Diseased), CustomColorManager.Diseased, LayerEnum.Diseased) },
        { LayerEnum.Drunk, new(typeof(Drunk), CustomColorManager.Drunk, LayerEnum.Drunk) },
        { LayerEnum.Dwarf, new(typeof(Dwarf), CustomColorManager.Dwarf, LayerEnum.Dwarf) },
        { LayerEnum.Giant, new(typeof(Giant), CustomColorManager.Giant, LayerEnum.Giant) },
        { LayerEnum.Indomitable, new(typeof(Indomitable), CustomColorManager.Indomitable, LayerEnum.Indomitable) },
        { LayerEnum.Professional, new(typeof(Professional), CustomColorManager.Professional, LayerEnum.Professional) },
        { LayerEnum.Shy, new(typeof(Shy), CustomColorManager.Shy, LayerEnum.Shy) },
        { LayerEnum.VIP, new(typeof(VIP), CustomColorManager.VIP, LayerEnum.VIP) },
        { LayerEnum.Volatile, new(typeof(Volatile), CustomColorManager.Volatile, LayerEnum.Volatile) },
        { LayerEnum.Yeller, new(typeof(Yeller), CustomColorManager.Yeller, LayerEnum.Yeller) },
        { LayerEnum.Allied, new(typeof(Allied), CustomColorManager.Allied, LayerEnum.Allied) },
        { LayerEnum.Corrupted, new(typeof(Corrupted), CustomColorManager.Corrupted, LayerEnum.Corrupted) },
        { LayerEnum.Defector, new(typeof(Defector), CustomColorManager.Defector, LayerEnum.Defector) },
        { LayerEnum.Fanatic, new(typeof(Fanatic), CustomColorManager.Fanatic, LayerEnum.Fanatic) },
        { LayerEnum.Linked, new(typeof(Linked), CustomColorManager.Linked, LayerEnum.Linked) },
        { LayerEnum.Lovers, new(typeof(Lovers), CustomColorManager.Lovers, LayerEnum.Lovers) },
        { LayerEnum.Mafia, new(typeof(Mafia), CustomColorManager.Mafia, LayerEnum.Mafia) },
        { LayerEnum.Overlord, new(typeof(Overlord), CustomColorManager.Overlord, LayerEnum.Overlord) },
        { LayerEnum.Rivals, new(typeof(Rivals), CustomColorManager.Rivals, LayerEnum.Rivals) },
        { LayerEnum.Taskmaster, new(typeof(Taskmaster), CustomColorManager.Taskmaster, LayerEnum.Taskmaster) },
        { LayerEnum.Traitor, new(typeof(Traitor), CustomColorManager.Traitor, LayerEnum.Traitor) },
        { LayerEnum.Assassin, new(typeof(Assassin), CustomColorManager.Assassin, LayerEnum.Assassin) },
        { LayerEnum.Bullseye, new(typeof(Bullseye), CustomColorManager.Crew, LayerEnum.Bullseye) },
        { LayerEnum.ButtonBarry, new(typeof(ButtonBarry), CustomColorManager.ButtonBarry, LayerEnum.ButtonBarry) },
        { LayerEnum.Hitman, new(typeof(Hitman), CustomColorManager.Intruder, LayerEnum.Hitman) },
        { LayerEnum.Insider, new(typeof(Insider), CustomColorManager.Insider, LayerEnum.Insider) },
        { LayerEnum.Multitasker, new(typeof(Multitasker), CustomColorManager.Multitasker, LayerEnum.Multitasker) },
        { LayerEnum.Ninja, new(typeof(Ninja), CustomColorManager.Ninja, LayerEnum.Ninja) },
        { LayerEnum.Politician, new(typeof(Politician), CustomColorManager.Politician, LayerEnum.Politician) },
        { LayerEnum.Radar, new(typeof(Radar), CustomColorManager.Radar, LayerEnum.Radar) },
        { LayerEnum.Ruthless, new(typeof(Ruthless), CustomColorManager.Ruthless, LayerEnum.Ruthless) },
        { LayerEnum.Slayer, new(typeof(Slayer), CustomColorManager.Neutral, LayerEnum.Slayer) },
        { LayerEnum.Sniper, new(typeof(Sniper), CustomColorManager.Syndicate, LayerEnum.Sniper) },
        { LayerEnum.Snitch, new(typeof(Snitch), CustomColorManager.Snitch, LayerEnum.Snitch) },
        { LayerEnum.Swapper, new(typeof(Swapper), CustomColorManager.Swapper, LayerEnum.Swapper) },
        { LayerEnum.Tiebreaker, new(typeof(Tiebreaker), CustomColorManager.Tiebreaker, LayerEnum.Tiebreaker) },
        { LayerEnum.Torch, new(typeof(Torch), CustomColorManager.Torch, LayerEnum.Torch) },
        { LayerEnum.Tunneler, new(typeof(Tunneler), CustomColorManager.Tunneler, LayerEnum.Tunneler) },
        { LayerEnum.Underdog, new(typeof(Underdog), CustomColorManager.Underdog, LayerEnum.Underdog) },
        { LayerEnum.Cabal, new(null, CustomColorManager.Cabal, LayerEnum.Cabal) },
        { LayerEnum.Sect, new(null, CustomColorManager.Sect, LayerEnum.Sect) },
        { LayerEnum.Reanimated, new(null, CustomColorManager.Reanimated, LayerEnum.Reanimated) },
        { LayerEnum.Undead, new(null, CustomColorManager.Undead, LayerEnum.Undead) }
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
        "If it's not a bug, it's a feature",
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
        "I got murder on my mind",
        "Space is big. You just won't believe how vastly, hugely, mind-bogglingly big it is",
        "To infinity and beyond!",
        "Houston, we have a problem",
        "In space, no one can hear you scream",
        "One small step for man, one giant leap for mankind",
        "May the force be with you",
        "Live long and prosper",
        "That's no moon. It's a space station",
        "Beam me up, Scotty",
        "Ground control to Major Tom",
        "Open the pod bay doors, HAL",
        "Fly me to the moon",
        "Space: the final frontier",
        "Is there life on Mars?",
        "Take me to your leader",
        "Resistance is futile",
        "Set phasers to stun",
        "Engage!",
        "By Grabthar's hammer, you shall be avenged!",
        "Space madness!",
        "LET. HIM. COOK."
    ];
}