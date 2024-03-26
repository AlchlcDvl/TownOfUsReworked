namespace TownOfUsReworked.Custom;

public class CustomPlayer
{
    public PlayerControl Player { get; }
    public GameData.PlayerInfo Data => Player.Data;
    public Transform Transform => Player.transform;
    public Vector3 Position => Transform.position;
    public bool Dead => Data.IsDead;
    public bool Disconnected => Data.Disconnected;
    public float SpeedFactor => Player.GetBaseSpeed() * Player.GetModifiedSpeed();
    public Vector3 SizeFactor => new(0.7f * Size, 0.7f * Size, 1f);
    public float Size => IsLobby || Dead || Disconnected ? 1f : Player.GetModifiedSize();
    public GameData.PlayerOutfit DefaultOutfit => Data.DefaultOutfit;
    public string PlayerName => Data.PlayerName;

    public static PlayerControl Local => PlayerControl.LocalPlayer;
    public static CustomPlayer LocalCustom => Custom(Local);
    public static List<PlayerControl> AllPlayers => PlayerControl.AllPlayerControls.Il2CppToSystem();
    public static readonly List<CustomPlayer> AllCustomPlayers = [];

    public CustomPlayer(PlayerControl player)
    {
        Player = player;
        AllCustomPlayers.Add(this);
    }

    public static implicit operator bool(CustomPlayer exists) => exists != null && exists.Player;

    public static implicit operator PlayerControl(CustomPlayer exists) => exists.Player;

    public static CustomPlayer Custom(PlayerControl player) => AllCustomPlayers.Find(x => x.Player == player) ?? new(player);

    public override string ToString() => Player.ToString();
}