namespace TownOfUsReworked.Custom;

public sealed class CustomPlayer
{
    public PlayerControl Player { get; }
    public NetworkedPlayerInfo Data => Player?.Data;
    public Vector3 Position => Player.transform.position;
    public bool Dead => Data?.IsDead ?? true;
    public float SpeedFactor => Player.GetBaseSpeed() * Player.GetModifiedSpeed();
    public float Size => IsLobby() || Dead || HasTask(TaskTypes.MushroomMixupSabotage) ? 1f : Player.GetModifiedSize();
    public NetworkedPlayerInfo.PlayerOutfit DefaultOutfit => Data?.DefaultOutfit;

    public static PlayerControl Local => PlayerControl.LocalPlayer;
    public static CustomPlayer LocalCustom => Custom(Local);

    public static readonly List<CustomPlayer> AllCustomPlayers = [];

    private CustomPlayer(PlayerControl player)
    {
        Player = player;
        AllCustomPlayers.Add(this);
    }

    public static CustomPlayer Custom(PlayerControl player) => AllCustomPlayers.TryFinding(x => x.Player == player, out var result) ? result : new(player);

    public override string ToString() => Player.name;
}