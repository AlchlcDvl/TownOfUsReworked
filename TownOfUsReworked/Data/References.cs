namespace TownOfUsReworked.Data;

public static class References
{
    public static readonly List<byte> RecentlyKilled = new();
    public static readonly Dictionary<byte, byte> CachedMorphs = new();
    public static readonly List<DeadPlayer> KilledPlayers = new();
    public static readonly Dictionary<byte, float> TransitioningSize = new(); //Wheeze
    public static readonly Dictionary<byte, float> TransitioningSpeed = new(); //Double wheeze
    public static readonly Dictionary<byte, DateTime> UninteractiblePlayers = new();
    public static Dictionary<byte, string> BodyLocations = new();
    public static List<DeadBody> AllBodies => UObject.FindObjectsOfType<DeadBody>().ToList();
    public static List<Vent> AllVents => UObject.FindObjectsOfType<Vent>().ToList();
    public static List<Vent> AllMapVents => Ship.AllVents.ToList();
    public static List<GameObject> AllGameObjects => UObject.FindObjectsOfType<GameObject>().ToList();
    public static List<Console> AllConsoles => UObject.FindObjectsOfType<Console>().ToList();
    public static List<SystemConsole> AllSystemConsoles => UObject.FindObjectsOfType<SystemConsole>().ToList();
    public static List<PlayerVoteArea> AllVoteAreas => Meeting.playerStates.ToList();
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
    public const string Everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ✧¢" +
        "乂⁂¤∮彡个「」人요〖〗ロ米卄王īl【】·ㅇ°◈◆◇◥◤◢◣《》︵︶☆☀☂☹☺♡♩♪♫♬✓☜☞☟☯☃✿❀÷º¿※⁑∞≠";
    public static readonly Dictionary<string, string> KeyWords = new()
    {
        { "%modversion%", TownOfUsReworked.VersionFinal },
        { "%discord%", $"[{TownOfUsReworked.DiscordInvite}]Discord[]" },
        { "%github%", $"[{TownOfUsReworked.GitHubLink}]GitHub[]" },
        { "%assets%", $"[{TownOfUsReworked.AssetsLink}]Assets[]" }
    };
    public static readonly char[] Lowercase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    public static readonly char[] Uppercase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public static readonly Vector2[] SkeldSpawns =
    {
        new(-2.2f, 2.2f), //Cafeteria. botton. top left.
        new(0.7f, 2.2f), //Caffeteria. button. top right.
        new(-2.2f, -0.2f), //Caffeteria. button. bottom left.
        new(0.7f, -0.2f), //Caffeteria. button. bottom right.
        new(4.3f, 0f), //Cafeteria vent
        new(10f, 3f), //Weapons top
        new(9.5f, -1f), //Weapons bottom
        new(6.5f, -3.5f), //O2
        new(11.5f, -3.5f), //O2-nav hall
        new(17, -3.5f), //Navigation top
        new(18.2f, -5.7f), //Navigation bottom
        new(16f, -2f), //Navigation vent
        new(11.5f, -6.5f), //Nav-shields top
        new(9.5f, -8.5f), //Nav-shields bottom
        new(9.2f, -12.2f), //Shields top
        new(8, -14.3f), //Shields bottom
        new(2.5f, -16f), //Comms left
        new(4.2f, -16.4f), //Comms middle
        new(5.5f, -16f), //Comms right
        new(-1.5f, -10), //Storage top
        new(-1.5f, -15.5f), //Storage bottom
        new(-4.5f, -12.5f), //Storrage left
        new(0.3f, -12.5f), //Storrage right
        new(4.5f, -7.5f), //Admin top
        new(4.5f, -9.5f), //Admin bottom
        new(-9, -8), //Elec top left
        new(-6, -8), //Elec top right
        new(-8, -11), //Elec bottom
        new(-12, -13), //Elec-lower hall
        new(-17f, -10f), //Lower engine top
        new(-17, -13), //Lower engine bottom
        new(-21.5f, -3), //Reactor top
        new(-21.5f, -8), //Reactor bottom
        new(-13, -3), //Security top
        new(-12.6f, -5.6f), //Security bottom
        new(-17, 2.5f), //Upper engibe top
        new(-17, -1), //Upper engine bottom
        new(-10.5f, 1), //Upper-mad hall
        new(-10.5f, -2), //Medbay top
        new(-6.5f, -4.5f) //Medbay bottom
    };
    public static readonly Vector2[] MiraSpawns =
    {
        new(-4.5f, 3.5f), //Launchpad top
        new(-4.5f, -1.4f), //Launchpad bottom
        new(8.5f, -1f), //Launchpad- med hall
        new(14f, -1.5f), //Medbay
        new(16.5f, 3f), //Comms
        new(10f, 5f), //Lockers
        new(6f, 1.5f), //Locker room
        new(2.5f, 13.6f), //Reactor
        new(6f, 12f), //Reactor middle
        new(9.5f, 13f), //Lab
        new(15f, 9f), //Bottom left cross
        new(17.9f, 11.5f), //Middle cross
        new(14f, 17.3f), //Office
        new(19.5f, 21f), //Admin
        new(14f, 24f), //Greenhouse left
        new(22f, 24f), //Greenhouse right
        new(21f, 8.5f), //Bottom right cross
        new(28f, 3f), //Caf right
        new(22f, 3f), //Caf left
        new(19f, 4f), //Storage
        new(22f, -2f) //Balcony
    };
    public static readonly Vector2[] PolusSpawns =
    {
        new(16.6f, -1f), //Dropship top
        new(16.6f, -5f), //Dropship bottom
        new(20f, -9f), //Above storrage
        new(22f, -7f), //Right fuel
        new(25.5f, -6.9f), //Drill
        new(29f, -9.5f), //Lab lockers
        new(29.5f, -8f), //Lab weather notes
        new(35f, -7.6f), //Lab table
        new(40.4f, -8f), //Lab scan
        new(33f, -10f), //Lab toilet
        new(39f, -15f), //Specimen hall top
        new(36.5f, -19.5f), //Specimen top
        new(36.5f, -21f), //Specimen bottom
        new(28f, -21f), //Specimen hall bottom
        new(24f, -20.5f), //Admin tv
        new(22f, -25f), //Admin books
        new(16.6f, -17.5f), //Office coffe
        new(22.5f, -16.5f), //Office projector
        new(24f, -17f), //Office figure
        new(27f, -16.5f), //Office lifelines
        new(32.7f, -15.7f), //Lavapool
        new(31.5f, -12f), //Snowmad below lab
        new(10f, -14f), //Below storrage
        new(21.5f, -12.5f), //Storrage vent
        new(19f, -11f), //Storrage toolrack
        new(12f, -7f), //Left fuel
        new(5f, -7.5f), //Above elec
        new(10f, -12f), //Elec fence
        new(9f, -9f), //Elec lockers
        new(5f, -9f), //Elec window
        new(4f, -11.2f), //Elec tapes
        new(5.5f, -16f), //Elec-O2 hall
        new(1f, -17.5f), //O2 tree hayball
        new(3f, -21f), //O2 middle
        new(2f, -19f), //O2 gas
        new(1f, -24f), //O2 water
        new(7f, -24f), //Under O2
        new(9f, -20f), //Right outside of O2
        new(7f, -15.8f), //Snowman under elec
        new(11f, -17f), //Comms table
        new(12.7f, -15.5f), //Comms antenna pult
        new(13f, -24.5f), //Weapons window
        new(15f, -17f), //Between coms-office
        new(17.5f, -25.7f) //Snowman under office
    };
    public static readonly Vector2[] dlekSSpawns =
    {
        new(2.2f, 2.2f), //Cafeteria. botton. top left.
        new(-0.7f, 2.2f), //Caffeteria. button. top right.
        new(2.2f, -0.2f), //Caffeteria. button. bottom left.
        new(-0.7f, -0.2f), //Caffeteria. button. bottom right.
        new(-10, 3), //Weapons top
        new(-9, 1), //Weapons bottom
        new(-6.5f, -3.5f), //O2
        new(-11.5f, -3.5f), //O2-nav hall
        new(-17, -3.5f), //Navigation top
        new(-18.2f, -5.7f), //Navigation bottom
        new(-11.5f, -6.5f), //Nav-shields top
        new(-9.5f, -8.5f), //Nav-shields bottom
        new(-9.2f, -12.2f), //Shields top
        new(-8, -14.3f), //Shields bottom
        new(-2.5f, -16f), //Comms left
        new(-4.2f, -16.4f), //Comms middle
        new(-5.5f, -16f), //Comms right
        new(1.5f, -10), //Storage top
        new(1.5f, -15.5f), //Storage bottom
        new(4.5f, -12.5f), //Storrage left
        new(-0.3f, -12.5f), //Storrage right
        new(-4.5f, -7.5f), //Admin top
        new(-4.5f, -9.5f), //Admin bottom
        new(9, -8), //Elec top left
        new(6, -8), //Elec top right
        new(8, -11), //Elec bottom
        new(12, -13), //Elec-lower hall
        new(17f, -10f), //Lower engine top
        new(17, -13), //Lower engine bottom
        new(21.5f, -3), //Reactor top
        new(21.5f, -8), //Reactor bottom
        new(13, -3), //Security top
        new(12.6f, -5.6f), //Security bottom
        new(17, 2.5f), //Upper engibe top
        new(17, -1), //Upper engine bottom
        new(10.5f, 1), //Upper-mad hall
        new(10.5f, -2), //Medbay top
        new(6.5f, -4.5f) //Medbay bottom
    };
}