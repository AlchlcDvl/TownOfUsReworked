namespace TownOfUsReworked.Data;

public static class References
{
    public static readonly HashSet<byte> RecentlyKilled = [];
    public static readonly HashSet<byte> Cleaned = [];
    public static readonly HashSet<byte> Moving = [];
    public static readonly List<DeadPlayer> KilledPlayers = [];
    public static readonly Dictionary<byte, float> UninteractablePlayers = [];
    public static readonly Dictionary<byte, float> UninteractablePlayers2 = [];
    public static readonly Dictionary<byte, string> BodyLocations = [];
    public static readonly Dictionary<byte, int> KillCounts = [];
    public static PlayerControl LocalPlayer => PlayerControl.LocalPlayer;
    public static IEnumerable<DeadBody> AllBodies() => UObject.FindObjectsOfType<DeadBody>();
    public static IEnumerable<Vent> AllVents() => UObject.FindObjectsOfType<Vent>();
    public static IEnumerable<Vent> AllMapVents() => Ship().AllVents;
    public static IEnumerable<GameObject> AllGameObjects() => UObject.FindObjectsOfType<GameObject>();
    public static IEnumerable<Console> AllConsoles() => UObject.FindObjectsOfType<Console>();
    public static IEnumerable<SystemConsole> AllSystemConsoles() => UObject.FindObjectsOfType<SystemConsole>();
    public static IEnumerable<PlayerVoteArea> AllVoteAreas() => Meeting().playerStates;
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
    public static WinLose WinState { get; set; } = WinLose.None;
    public static SummaryInfo Summary;
    public static bool HiddenBlock
    {
        get;
        set
        {
            field = value;

            if (!value)
                BlockExposed = false;
        }
    }
    public static bool BlockExposed
    {
        get;
        set
        {
            if (field == value)
                return;

            if (value)
                CameraEffectHandler.AddEffect("GlitchedMaterial");
            else
                CameraEffectHandler.RemoveEffect("GlitchedMaterial");

            field = value;
        }
    }
    public const string Everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ✧¢" +
        "乂⁂¤∮彡个「」人요〖〗ロ米卄王īl【】·ㅇ°◈◆◇◥◤◢◣《》︵︶☆☀☂☹☺♡♩♪♫♬✓☜☞☟☯☃✿❀÷º¿※⁑∞≠";
    // As much as I hate to do this, people will take advantage, so we're better off doing this early
    public static readonly string[] Profanities = [ "nigg", "whore", "negro", "yiff", "rape", "rapist" ];
    public const string Disallowed = "@^[{(_-;:\"'.,\\|)}]+$!#$%^&&*?/";
    // public static readonly char[] Lowercase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    // public static readonly char[] Uppercase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public static readonly IEnumerable<Vector2> SkeldSpawns =
    [
        new(-2.2f, 2.2f), // Cafeteria. Bottom. Top left
        new(0.7f, 2.2f), // Cafeteria. Button. Top right
        new(-2.2f, -0.2f), // Cafeteria. Button. Bottom left
        new(0.7f, -0.2f), // Cafeteria. Button. Bottom right
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
        new(-17, 2.5f), // Upper engine top
        new(-17, -1), // Upper engine bottom
        new(-10.5f, 1), // Upper-mad hall
        new(-10.5f, -2), // Medbay top
        new(-6.5f, -4.5f) // Medbay bottom
    ];
    public static readonly IEnumerable<Vector2> MiraSpawns =
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
    public static readonly IEnumerable<Vector2> PolusSpawns =
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
        new(16.6f, -17.5f), // Office coffee
        new(22.5f, -16.5f), // Office projector
        new(24f, -17f), // Office figure
        new(27f, -16.5f), // Office lifelines
        new(32.7f, -15.7f), // Lava pool
        new(31.5f, -12f), // Snowman below lab
        new(10f, -14f), // Below storage
        new(19f, -11f), // Storage tool rack
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
        new(9f, -20f), // Right outside O2
        new(6.9f, -13.8f), // Snowman under elec
        new(11f, -17f), // Comms table
        new(12.7f, -15.5f), // Comms antenna
        new(12f, -21.5f), // Weapons window
        new(15f, -17f), // Between comms-office
        new(17.5f, -25.7f) // Snowman under office
    ];
    // ReSharper disable once InconsistentNaming
    public static readonly IEnumerable<Vector2> dlekSSpawns =
    [
        new(2.2f, 2.2f), // Cafeteria. Bottom. Top left
        new(-0.7f, 2.2f), // Cafeteria. Button. Top right
        new(2.2f, -0.2f), // Cafeteria. Button. Bottom left
        new(-0.7f, -0.2f), // Cafeteria. Button. Bottom right
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
        new(17, 2.5f), // Upper engine top
        new(17, -1), // Upper engine bottom
        new(10.5f, 1), // Upper-mad hall
        new(10.5f, -2), // Medbay top
        new(6.5f, -4.5f) // Medbay bottom
    ];
    public static readonly Dictionary<Layer, LayerDictionaryEntry> LayerDictionary = new()
    {
        { Layer.Altruist, new(typeof(Altruist), CustomColorManager.Altruist, Layer.Altruist) },
        { Layer.Bastion, new(typeof(Bastion), CustomColorManager.Bastion, Layer.Bastion) },
        { Layer.Chameleon, new(typeof(Chameleon), CustomColorManager.Chameleon, Layer.Chameleon) },
        { Layer.Coroner, new(typeof(Coroner), CustomColorManager.Coroner, Layer.Coroner) },
        { Layer.Crewmate, new(typeof(Crewmate), CustomColorManager.Crew, Layer.Crewmate) },
        { Layer.Detective, new(typeof(Detective), CustomColorManager.Detective, Layer.Detective) },
        { Layer.Dictator, new(typeof(Dictator), CustomColorManager.Dictator, Layer.Dictator) },
        { Layer.Engineer, new(typeof(Engineer), CustomColorManager.Engineer, Layer.Engineer) },
        { Layer.Escort, new(typeof(Escort), CustomColorManager.Escort, Layer.Escort) },
        { Layer.Mayor, new(typeof(Mayor), CustomColorManager.Mayor, Layer.Mayor) },
        { Layer.Medic, new(typeof(Medic), CustomColorManager.Medic, Layer.Medic) },
        { Layer.Medium, new(typeof(Medium), CustomColorManager.Medium, Layer.Medium) },
        { Layer.Monarch, new(typeof(Monarch), CustomColorManager.Monarch, Layer.Monarch) },
        { Layer.Mystic, new(typeof(Mystic), CustomColorManager.Mystic, Layer.Mystic) },
        { Layer.Operative, new(typeof(Operative), CustomColorManager.Operative, Layer.Operative) },
        { Layer.Retributionist, new(typeof(Retributionist), CustomColorManager.Retributionist, Layer.Retributionist) },
        { Layer.Revealer, new(typeof(Revealer), CustomColorManager.Revealer, Layer.Revealer) },
        { Layer.Seer, new(typeof(Seer), CustomColorManager.Seer, Layer.Seer) },
        { Layer.Sheriff, new(typeof(Sheriff), CustomColorManager.Sheriff, Layer.Sheriff) },
        { Layer.Tracker, new(typeof(Tracker), CustomColorManager.Tracker, Layer.Tracker) },
        { Layer.Transporter, new(typeof(Transporter), CustomColorManager.Transporter, Layer.Transporter) },
        { Layer.Trapper, new(typeof(Trapper), CustomColorManager.Trapper, Layer.Trapper) },
        { Layer.Democrat, new(typeof(Democrat), CustomColorManager.Democrat, Layer.Democrat) },
        { Layer.Veteran, new(typeof(Veteran), CustomColorManager.Veteran, Layer.Veteran) },
        { Layer.Vigilante, new(typeof(Vigilante), CustomColorManager.Vigilante, Layer.Vigilante) },
        { Layer.Actor, new(typeof(Actor), CustomColorManager.Actor, Layer.Actor) },
        { Layer.Amnesiac, new(typeof(Amnesiac), CustomColorManager.Amnesiac, Layer.Amnesiac) },
        { Layer.Arsonist, new(typeof(Arsonist), CustomColorManager.Arsonist, Layer.Arsonist) },
        { Layer.Betrayer, new(typeof(Betrayer), CustomColorManager.Betrayer, Layer.Betrayer) },
        { Layer.BountyHunter, new(typeof(BountyHunter), CustomColorManager.BountyHunter, Layer.BountyHunter) },
        { Layer.Cryomaniac, new(typeof(Cryomaniac), CustomColorManager.Cryomaniac, Layer.Cryomaniac) },
        { Layer.Dracula, new(typeof(Dracula), CustomColorManager.Dracula, Layer.Dracula) },
        { Layer.Executioner, new(typeof(Executioner), CustomColorManager.Executioner, Layer.Executioner) },
        { Layer.Glitch, new(typeof(Glitch), CustomColorManager.Glitch, Layer.Glitch) },
        { Layer.GuardianAngel, new(typeof(GuardianAngel), CustomColorManager.GuardianAngel, Layer.GuardianAngel) },
        { Layer.Guesser, new(typeof(Guesser), CustomColorManager.Guesser, Layer.Guesser) },
        { Layer.Jackal, new(typeof(Jackal), CustomColorManager.Jackal, Layer.Jackal) },
        { Layer.Jester, new(typeof(Jester), CustomColorManager.Jester, Layer.Jester) },
        { Layer.Juggernaut, new(typeof(Juggernaut), CustomColorManager.Juggernaut, Layer.Juggernaut) },
        { Layer.Murderer, new(typeof(Murderer), CustomColorManager.Murderer, Layer.Murderer) },
        { Layer.Necromancer, new(typeof(Necromancer), CustomColorManager.Necromancer, Layer.Necromancer) },
        { Layer.Phantom, new(typeof(Phantom), CustomColorManager.Phantom, Layer.Phantom) },
        { Layer.SerialKiller, new(typeof(SerialKiller), CustomColorManager.SerialKiller, Layer.SerialKiller) },
        { Layer.Shifter, new(typeof(Shifter), CustomColorManager.Shifter, Layer.Shifter) },
        { Layer.Survivor, new(typeof(Survivor), CustomColorManager.Survivor, Layer.Survivor) },
        { Layer.Thief, new(typeof(Thief), CustomColorManager.Thief, Layer.Thief) },
        { Layer.Troll, new(typeof(Troll), CustomColorManager.Troll, Layer.Troll) },
        { Layer.Werewolf, new(typeof(Werewolf), CustomColorManager.Werewolf, Layer.Werewolf) },
        { Layer.Whisperer, new(typeof(Whisperer), CustomColorManager.Whisperer, Layer.Whisperer) },
        { Layer.Zealot, new(typeof(Zealot), CustomColorManager.Zealot, Layer.Zealot) },
        { Layer.Ambusher, new(typeof(Ambusher), CustomColorManager.Ambusher, Layer.Ambusher) },
        { Layer.Blackmailer, new(typeof(Blackmailer), CustomColorManager.Blackmailer, Layer.Blackmailer) },
        { Layer.Camouflager, new(typeof(Camouflager), CustomColorManager.Camouflager, Layer.Camouflager) },
        { Layer.Consigliere, new(typeof(Consigliere), CustomColorManager.Consigliere, Layer.Consigliere) },
        { Layer.Consort, new(typeof(Consort), CustomColorManager.Consort, Layer.Consort) },
        { Layer.Disguiser, new(typeof(Disguiser), CustomColorManager.Disguiser, Layer.Disguiser) },
        { Layer.Enforcer, new(typeof(Enforcer), CustomColorManager.Enforcer, Layer.Enforcer) },
        { Layer.Ghoul, new(typeof(Ghoul), CustomColorManager.Ghoul, Layer.Ghoul) },
        { Layer.Godfather, new(typeof(Godfather), CustomColorManager.Godfather, Layer.Godfather) },
        { Layer.Grenadier, new(typeof(Grenadier), CustomColorManager.Grenadier, Layer.Grenadier) },
        { Layer.Impostor, new(typeof(Impostor), CustomColorManager.Intruder, Layer.Impostor) },
        { Layer.Janitor, new(typeof(Janitor), CustomColorManager.Janitor, Layer.Janitor) },
        { Layer.Mafioso, new(null, CustomColorManager.Mafioso, Layer.Mafioso) },
        { Layer.Miner, new(typeof(Miner), CustomColorManager.Miner, Layer.Miner) },
        { Layer.Morphling, new(typeof(Morphling), CustomColorManager.Morphling, Layer.Morphling) },
        { Layer.Teleporter, new(typeof(Teleporter), CustomColorManager.Teleporter, Layer.Teleporter) },
        { Layer.Wraith, new(typeof(Wraith), CustomColorManager.Wraith, Layer.Wraith) },
        { Layer.Anarchist, new(typeof(Anarchist), CustomColorManager.Syndicate, Layer.Anarchist) },
        { Layer.Banshee, new(typeof(Banshee), CustomColorManager.Banshee, Layer.Banshee) },
        { Layer.Bomber, new(typeof(Bomber), CustomColorManager.Bomber, Layer.Bomber) },
        { Layer.Collider, new(typeof(PlayerLayers.Roles.Collider), CustomColorManager.Collider, Layer.Collider) },
        { Layer.Concealer, new(typeof(Concealer), CustomColorManager.Concealer, Layer.Concealer) },
        { Layer.Crusader, new(typeof(Crusader), CustomColorManager.Crusader, Layer.Crusader) },
        { Layer.Drunkard, new(typeof(Drunkard), CustomColorManager.Drunkard, Layer.Drunkard) },
        { Layer.Framer, new(typeof(Framer), CustomColorManager.Framer, Layer.Framer) },
        { Layer.Poisoner, new(typeof(Poisoner), CustomColorManager.Poisoner, Layer.Poisoner) },
        { Layer.Rebel, new(typeof(Rebel), CustomColorManager.Rebel, Layer.Rebel) },
        { Layer.Shapeshifter, new(typeof(Shapeshifter), CustomColorManager.Shapeshifter, Layer.Shapeshifter) },
        { Layer.Sidekick, new(null, CustomColorManager.Sidekick, Layer.Sidekick) },
        { Layer.Silencer, new(typeof(Silencer), CustomColorManager.Silencer, Layer.Silencer) },
        { Layer.Spellslinger, new(typeof(Spellslinger), CustomColorManager.Spellslinger, Layer.Spellslinger) },
        { Layer.Stalker, new(typeof(Stalker), CustomColorManager.Stalker, Layer.Stalker) },
        { Layer.Timekeeper, new(typeof(Timekeeper), CustomColorManager.Timekeeper, Layer.Timekeeper) },
        { Layer.Warper, new(typeof(Warper), CustomColorManager.Warper, Layer.Warper) },
        { Layer.Cannibal, new(typeof(Cannibal), CustomColorManager.Cannibal, Layer.Cannibal) },
        { Layer.Cultist, new(typeof(Cultist), CustomColorManager.Apocalypse, Layer.Cultist) },
        { Layer.Gluttony, new(typeof(Gluttony), CustomColorManager.Gluttony, Layer.Gluttony) },
        { Layer.Pestilence, new(typeof(Pestilence), CustomColorManager.Pestilence, Layer.Pestilence) },
        { Layer.Plaguebearer, new(typeof(Plaguebearer), CustomColorManager.Plaguebearer, Layer.Plaguebearer) },
        { Layer.Void, new(typeof(PlayerLayers.Roles.Void), CustomColorManager.Void, Layer.Void) },
        { Layer.Hunter, new(typeof(Hunter), CustomColorManager.Hunter, Layer.Hunter) },
        { Layer.Hunted, new(typeof(Hunted), CustomColorManager.Hunted, Layer.Hunted) },
        { Layer.Runner, new(typeof(Runner), CustomColorManager.Runner, Layer.Runner) },
        { Layer.Astral, new(typeof(Astral), CustomColorManager.Astral, Layer.Astral) },
        { Layer.Bait, new(typeof(Bait), CustomColorManager.Bait, Layer.Bait) },
        { Layer.Colorblind, new(typeof(Colorblind), CustomColorManager.Colorblind, Layer.Colorblind) },
        { Layer.Coward, new(typeof(Coward), CustomColorManager.Coward, Layer.Coward) },
        { Layer.Diseased, new(typeof(Diseased), CustomColorManager.Diseased, Layer.Diseased) },
        { Layer.Drunk, new(typeof(Drunk), CustomColorManager.Drunk, Layer.Drunk) },
        { Layer.Dwarf, new(typeof(Dwarf), CustomColorManager.Dwarf, Layer.Dwarf) },
        { Layer.Giant, new(typeof(Giant), CustomColorManager.Giant, Layer.Giant) },
        { Layer.Indomitable, new(typeof(Indomitable), CustomColorManager.Indomitable, Layer.Indomitable) },
        { Layer.Shy, new(typeof(Shy), CustomColorManager.Shy, Layer.Shy) },
        { Layer.Vip, new(typeof(Vip), CustomColorManager.Vip, Layer.Vip) },
        { Layer.Volatile, new(typeof(Volatile), CustomColorManager.Volatile, Layer.Volatile) },
        { Layer.Yeller, new(typeof(Yeller), CustomColorManager.Yeller, Layer.Yeller) },
        { Layer.Allied, new(typeof(Allied), CustomColorManager.Allied, Layer.Allied, "ζ") },
        { Layer.Corrupted, new(typeof(Corrupted), CustomColorManager.Corrupted, Layer.Corrupted, "δ") },
        { Layer.Defector, new(typeof(Defector), CustomColorManager.Defector, Layer.Defector, "ε") },
        { Layer.Fanatic, new(typeof(Fanatic), CustomColorManager.Fanatic, Layer.Fanatic, "♠") },
        { Layer.Linked, new(typeof(Linked), CustomColorManager.Linked, Layer.Linked, "Ψ") },
        { Layer.Lovers, new(typeof(Lovers), CustomColorManager.Lovers, Layer.Lovers, "♥") },
        { Layer.Mafia, new(typeof(Mafia), CustomColorManager.Mafia, Layer.Mafia, "ω") },
        { Layer.Overlord, new(typeof(Overlord), CustomColorManager.Overlord, Layer.Overlord, "β") },
        { Layer.Rivals, new(typeof(Rivals), CustomColorManager.Rivals, Layer.Rivals, "α") },
        { Layer.Taskmaster, new(typeof(Taskmaster), CustomColorManager.Taskmaster, Layer.Taskmaster, "µ") },
        { Layer.Traitor, new(typeof(Traitor), CustomColorManager.Traitor, Layer.Traitor, "♣") },
        { Layer.Assassin, new(typeof(Assassin), CustomColorManager.Assassin, Layer.Assassin) },
        { Layer.Bullseye, new(typeof(Bullseye), CustomColorManager.Crew, Layer.Bullseye) },
        { Layer.ButtonBarry, new(typeof(ButtonBarry), CustomColorManager.ButtonBarry, Layer.ButtonBarry) },
        { Layer.Hitman, new(typeof(Hitman), CustomColorManager.Intruder, Layer.Hitman) },
        { Layer.Insider, new(typeof(Insider), CustomColorManager.Insider, Layer.Insider) },
        { Layer.Multitasker, new(typeof(Multitasker), CustomColorManager.Multitasker, Layer.Multitasker) },
        { Layer.Ninja, new(typeof(Ninja), CustomColorManager.Ninja, Layer.Ninja) },
        { Layer.Politician, new(typeof(Politician), CustomColorManager.Politician, Layer.Politician) },
        { Layer.Radar, new(typeof(Radar), CustomColorManager.Radar, Layer.Radar) },
        { Layer.Ritualist, new(typeof(Ritualist), CustomColorManager.Apocalypse, Layer.Ritualist) },
        { Layer.Ruthless, new(typeof(Ruthless), CustomColorManager.Ruthless, Layer.Ruthless) },
        { Layer.Slayer, new(typeof(Slayer), CustomColorManager.Outcast, Layer.Slayer) },
        { Layer.Sniper, new(typeof(Sniper), CustomColorManager.Syndicate, Layer.Sniper) },
        { Layer.Snitch, new(typeof(Snitch), CustomColorManager.Snitch, Layer.Snitch) },
        { Layer.Swapper, new(typeof(Swapper), CustomColorManager.Swapper, Layer.Swapper) },
        { Layer.Tiebreaker, new(typeof(Tiebreaker), CustomColorManager.Tiebreaker, Layer.Tiebreaker) },
        { Layer.Torch, new(typeof(Torch), CustomColorManager.Torch, Layer.Torch) },
        { Layer.Tunneler, new(typeof(Tunneler), CustomColorManager.Tunneler, Layer.Tunneler) },
        { Layer.Underdog, new(typeof(Underdog), CustomColorManager.Underdog, Layer.Underdog) },
    };
    public static readonly Dictionary<Faction, FactionDictionaryEntry> FactionDictionary = new()
    {
        { Faction.Crew, new(Faction.Crew, CustomColorManager.Crew) },
        { Faction.Intruder, new(Faction.Intruder, CustomColorManager.Intruder) },
        { Faction.Syndicate, new(Faction.Syndicate, CustomColorManager.Syndicate) },
        { Faction.Apocalypse, new(Faction.Apocalypse, CustomColorManager.Apocalypse) },
        { Faction.Outcast, new(Faction.Outcast, CustomColorManager.Outcast) },
        { Faction.Pandorica, new(Faction.Pandorica, CustomColorManager.Pandorica) },
        { Faction.Compliance, new(Faction.Compliance, CustomColorManager.Compliance) },
        { Faction.GameMode, new(Faction.GameMode, CustomColorManager.GameMode) },
        { Faction.Arsonist, new(Faction.Arsonist, CustomColorManager.Arsonist) },
        { Faction.Cryomaniac, new(Faction.Cryomaniac, CustomColorManager.Cryomaniac) },
        { Faction.Glitch, new(Faction.Glitch, CustomColorManager.Glitch) },
        { Faction.Juggernaut, new(Faction.Juggernaut, CustomColorManager.Juggernaut) },
        { Faction.Murderer, new(Faction.Murderer, CustomColorManager.Murderer) },
        { Faction.SerialKiller, new(Faction.SerialKiller, CustomColorManager.SerialKiller) },
        { Faction.Werewolf, new(Faction.Werewolf, CustomColorManager.Werewolf) },
        { Faction.Cabal, new(Faction.Cabal, CustomColorManager.Cabal) },
        { Faction.Cult, new(Faction.Cult, CustomColorManager.Cult) },
        { Faction.Followers, new(Faction.Followers, CustomColorManager.Followers) },
        { Faction.Reanimated, new(Faction.Reanimated, CustomColorManager.Reanimated) },
        { Faction.Undead, new(Faction.Undead, CustomColorManager.Undead) },
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
        "Push to production is my motto. Bugs? meh public release go brr",
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