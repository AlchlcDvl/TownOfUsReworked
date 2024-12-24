namespace TownOfUsReworked.Custom;

public class CustomPlayer
{
    public PlayerControl Player { get; }
    public NetworkedPlayerInfo Data => Player?.Data;
    public Transform Transform => Player.transform;
    public Vector3 Position => Transform.position;
    public bool Dead => Data.IsDead;
    public bool Disconnected => Data.Disconnected;
    public float SpeedFactor => Player.GetBaseSpeed() * Player.GetModifiedSpeed();
    public Vector3 SizeFactor => new(0.7f * Size, 0.7f * Size, 1f);
    public float Size => IsLobby() || Dead || Disconnected || HasTask(TaskTypes.MushroomMixupSabotage) ? 1f : Player.GetModifiedSize();
    public NetworkedPlayerInfo.PlayerOutfit DefaultOutfit => Data.DefaultOutfit;
    public string PlayerName => Player.name;

    public static PlayerControl Local => PlayerControl.LocalPlayer;
    public static CustomPlayer LocalCustom => Custom(Local);
    public static readonly List<CustomPlayer> AllCustomPlayers = [];

    public CustomPlayer(PlayerControl player)
    {
        Player = player;
        AllCustomPlayers.Add(this);
    }

    public static implicit operator bool(CustomPlayer exists) => exists?.Player;

    public static implicit operator PlayerControl(CustomPlayer exists) => exists.Player;

    public static CustomPlayer Custom(PlayerControl player) => AllCustomPlayers.Find(x => x.Player == player) ?? new(player);

    public override string ToString() => Player.ToString();
}