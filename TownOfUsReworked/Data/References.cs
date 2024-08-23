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
    public static List<DeadBody> AllBodies => [ .. UObject.FindObjectsOfType<DeadBody>() ];
    public static List<Vent> AllVents => [ .. UObject.FindObjectsOfType<Vent>() ];
    public static List<Vent> AllMapVents => [ .. Ship.AllVents ];
    public static List<GameObject> AllGameObjects => [ .. UObject.FindObjectsOfType<GameObject>() ];
    public static List<Console> AllConsoles => [ .. UObject.FindObjectsOfType<Console>() ];
    public static List<SystemConsole> AllSystemConsoles => [ .. UObject.FindObjectsOfType<SystemConsole>() ];
    public static List<PlayerVoteArea> AllVoteAreas => [ .. Meeting.playerStates ];
    public static string FirstDead;
    public static string CachedFirstDead;
    public static HudManager HUD => HudManager.Instance;
    public static MeetingHud Meeting => MeetingHud.Instance;
    public static ExileController Ejection => ExileController.Instance;
    public static ShipStatus Ship => ShipStatus.Instance;
    public static MapBehaviour Map => MapBehaviour.Instance;
    public static Minigame ActiveTask => Minigame.Instance;
    public static LobbyBehaviour Lobby => LobbyBehaviour.Instance;
    public static ChatController Chat => HUD.Chat;
    public static bool Shapeshifted;
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
        new(-2.2f, 2.2f), // Cafeteria. botton. top left.
        new(0.7f, 2.2f), // Caffeteria. button. top right.
        new(-2.2f, -0.2f), // Caffeteria. button. bottom left.
        new(0.7f, -0.2f), // Caffeteria. button. bottom right.
        new(4.3f, 0f), // Cafeteria vent
        new(10f, 3f), // Weapons top
        new(9.5f, -1f), // Weapons bottom
        new(6.5f, -3.5f), // O2
        new(11.5f, -3.5f), // O2-nav hall
        new(17, -3.5f), // Navigation top
        new(18.2f, -5.7f), // Navigation bottom
        new(16f, -2f), // Navigation vent
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
        new(21.5f, -12.5f), // Storage vent
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
        new(2.2f, 2.2f), // Cafeteria. botton. top left.
        new(-0.7f, 2.2f), // Caffeteria. button. top right.
        new(2.2f, -0.2f), // Caffeteria. button. bottom left.
        new(-0.7f, -0.2f), // Caffeteria. button. bottom right.
        new(-10, 3), // Weapons top
        new(-9, 1), // Weapons bottom
        new(-6.5f, -3.5f), // O2
        new(-11.5f, -3.5f), // O2-nav hall
        new(-17, -3.5f), // Navigation top
        new(-18.2f, -5.7f), // Navigation bottom
        new(-11.5f, -6.5f), // Nav-shields top
        new(-9.5f, -8.5f), // Nav-shields bottom
        new(-9.2f, -12.2f), // Shields top
        new(-8, -14.3f), // Shields bottom
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
    public static readonly Dictionary<LayerEnum, Type> LayerEnumToType = new()
    {
        { LayerEnum.Altruist, typeof(Altruist) },
        { LayerEnum.Bastion, typeof(Bastion) },
        { LayerEnum.Chameleon, typeof(Chameleon) },
        { LayerEnum.Coroner, typeof(Coroner) },
        { LayerEnum.Crewmate, typeof(Crewmate) },
        { LayerEnum.Detective, typeof(Detective) },
        { LayerEnum.Dictator, typeof(Dictator) },
        { LayerEnum.Engineer, typeof(Engineer) },
        { LayerEnum.Escort, typeof(Escort) },
        { LayerEnum.Mayor, typeof(Mayor) },
        { LayerEnum.Medic, typeof(Medic) },
        { LayerEnum.Medium, typeof(Medium) },
        { LayerEnum.Monarch, typeof(Monarch) },
        { LayerEnum.Mystic, typeof(Mystic) },
        { LayerEnum.Operative, typeof(Operative) },
        { LayerEnum.Retributionist, typeof(Retributionist) },
        { LayerEnum.Revealer, typeof(Revealer) },
        { LayerEnum.Seer, typeof(Seer) },
        { LayerEnum.Sheriff, typeof(Sheriff) },
        { LayerEnum.Shifter, typeof(Shifter) },
        { LayerEnum.Tracker, typeof(Tracker) },
        { LayerEnum.Transporter, typeof(Transporter) },
        { LayerEnum.Trapper, typeof(Trapper) },
        { LayerEnum.VampireHunter, typeof(VampireHunter) },
        { LayerEnum.Veteran, typeof(Veteran) },
        { LayerEnum.Vigilante, typeof(Vigilante) },
        { LayerEnum.Actor, typeof(Actor) },
        { LayerEnum.Amnesiac, typeof(Amnesiac) },
        { LayerEnum.Arsonist, typeof(Arsonist) },
        { LayerEnum.Betrayer, typeof(Betrayer) },
        { LayerEnum.BountyHunter, typeof(BountyHunter) },
        { LayerEnum.Cannibal, typeof(Cannibal) },
        { LayerEnum.Cryomaniac, typeof(Cryomaniac) },
        { LayerEnum.Dracula, typeof(Dracula) },
        { LayerEnum.Executioner, typeof(Executioner) },
        { LayerEnum.Glitch, typeof(Glitch) },
        { LayerEnum.GuardianAngel, typeof(GuardianAngel) },
        { LayerEnum.Guesser, typeof(Guesser) },
        { LayerEnum.Jackal, typeof(Jackal) },
        { LayerEnum.Jester, typeof(Jester) },
        { LayerEnum.Juggernaut, typeof(Juggernaut) },
        { LayerEnum.Murderer, typeof(Murderer) },
        { LayerEnum.Necromancer, typeof(Necromancer) },
        { LayerEnum.Pestilence, typeof(Pestilence) },
        { LayerEnum.Phantom, typeof(Phantom) },
        { LayerEnum.Plaguebearer, typeof(Plaguebearer) },
        { LayerEnum.SerialKiller, typeof(SerialKiller) },
        { LayerEnum.Survivor, typeof(Survivor) },
        { LayerEnum.Thief, typeof(Thief) },
        { LayerEnum.Troll, typeof(Troll) },
        { LayerEnum.Werewolf, typeof(Werewolf) },
        { LayerEnum.Whisperer, typeof(Whisperer) },
        { LayerEnum.Ambusher, typeof(Ambusher) },
        { LayerEnum.Blackmailer, typeof(Blackmailer) },
        { LayerEnum.Camouflager, typeof(Camouflager) },
        { LayerEnum.Consigliere, typeof(Consigliere) },
        { LayerEnum.Consort, typeof(Consort) },
        { LayerEnum.Disguiser, typeof(Disguiser) },
        { LayerEnum.Enforcer, typeof(Enforcer) },
        { LayerEnum.Ghoul, typeof(Ghoul) },
        { LayerEnum.Godfather, typeof(Godfather) },
        { LayerEnum.Grenadier, typeof(Grenadier) },
        { LayerEnum.Impostor, typeof(Impostor) },
        { LayerEnum.Janitor, typeof(Janitor) },
        { LayerEnum.Mafioso, typeof(Mafioso) },
        { LayerEnum.Miner, typeof(Miner) },
        { LayerEnum.Morphling, typeof(Morphling) },
        { LayerEnum.PromotedGodfather, typeof(PromotedGodfather) },
        { LayerEnum.Teleporter, typeof(Teleporter) },
        { LayerEnum.Wraith, typeof(Wraith) },
        { LayerEnum.Anarchist, typeof(Anarchist) },
        { LayerEnum.Banshee, typeof(Banshee) },
        { LayerEnum.Bomber, typeof(Bomber) },
        { LayerEnum.Collider, typeof(PlayerLayers.Roles.Collider) },
        { LayerEnum.Concealer, typeof(Concealer) },
        { LayerEnum.Crusader, typeof(Crusader) },
        { LayerEnum.Drunkard, typeof(Drunkard) },
        { LayerEnum.Framer, typeof(Framer) },
        { LayerEnum.Poisoner, typeof(Poisoner) },
        { LayerEnum.PromotedRebel, typeof(PromotedRebel) },
        { LayerEnum.Rebel, typeof(Rebel) },
        { LayerEnum.Shapeshifter, typeof(Shapeshifter) },
        { LayerEnum.Sidekick, typeof(Sidekick) },
        { LayerEnum.Silencer, typeof(Silencer) },
        { LayerEnum.Spellslinger, typeof(Spellslinger) },
        { LayerEnum.Stalker, typeof(Stalker) },
        { LayerEnum.Timekeeper, typeof(Timekeeper) },
        { LayerEnum.Warper, typeof(Warper) },
        { LayerEnum.Hunter, typeof(Hunter) },
        { LayerEnum.Hunted, typeof(Hunted) },
        { LayerEnum.Runner, typeof(Runner) },
        { LayerEnum.Astral, typeof(Astral) },
        { LayerEnum.Bait, typeof(Bait) },
        { LayerEnum.Colorblind, typeof(Colorblind) },
        { LayerEnum.Coward, typeof(Coward) },
        { LayerEnum.Diseased, typeof(Diseased) },
        { LayerEnum.Drunk, typeof(Drunk) },
        { LayerEnum.Dwarf, typeof(Dwarf) },
        { LayerEnum.Giant, typeof(Giant) },
        { LayerEnum.Indomitable, typeof(Indomitable) },
        { LayerEnum.Professional, typeof(Professional) },
        { LayerEnum.Shy, typeof(Shy) },
        { LayerEnum.VIP, typeof(VIP) },
        { LayerEnum.Volatile, typeof(Volatile) },
        { LayerEnum.Yeller, typeof(Yeller) },
        { LayerEnum.Allied, typeof(Allied) },
        { LayerEnum.Corrupted, typeof(Corrupted) },
        { LayerEnum.Defector, typeof(Defector) },
        { LayerEnum.Fanatic, typeof(Fanatic) },
        { LayerEnum.Linked, typeof(Linked) },
        { LayerEnum.Lovers, typeof(Lovers) },
        { LayerEnum.Mafia, typeof(Mafia) },
        { LayerEnum.Overlord, typeof(Overlord) },
        { LayerEnum.Rivals, typeof(Rivals) },
        { LayerEnum.Taskmaster, typeof(Taskmaster) },
        { LayerEnum.Traitor, typeof(Traitor) },
        { LayerEnum.Bullseye, typeof(Bullseye) },
        { LayerEnum.ButtonBarry, typeof(ButtonBarry) },
        { LayerEnum.Hitman, typeof(Hitman) },
        { LayerEnum.Insider, typeof(Insider) },
        { LayerEnum.Multitasker, typeof(Multitasker) },
        { LayerEnum.Ninja, typeof(Ninja) },
        { LayerEnum.Politician, typeof(Politician) },
        { LayerEnum.Radar, typeof(Radar) },
        { LayerEnum.Ruthless, typeof(Ruthless) },
        { LayerEnum.Slayer, typeof(Slayer) },
        { LayerEnum.Sniper, typeof(Sniper) },
        { LayerEnum.Snitch, typeof(Snitch) },
        { LayerEnum.Swapper, typeof(Swapper) },
        { LayerEnum.Tiebreaker, typeof(Tiebreaker) },
        { LayerEnum.Torch, typeof(Torch) },
        { LayerEnum.Tunneler, typeof(Tunneler) },
        { LayerEnum.Underdog, typeof(Underdog) },
    };
}