namespace TownOfUsReworked.Custom;

public sealed class CustomPlayer
{
    public PlayerControl Player { get; }
    public float SpeedFactor => Player.GetBaseSpeed() * Player.GetModifiedSpeed();
    public float Size => IsLobby() || Player.HasDied() || HasTask(TaskTypes.MushroomMixupSabotage) ? 1f : Player.GetModifiedSize();

    public static PlayerControl Local => PlayerControl.LocalPlayer;

    public static readonly List<CustomPlayer> AllCustomPlayers = [];

    private CustomPlayer(PlayerControl player)
    {
        Player = player;
        AllCustomPlayers.Add(this);
    }

    public static CustomPlayer Custom(PlayerControl player) => AllCustomPlayers.TryFinding(x => x.Player == player, out var result) ? result : new(player);

    public override string ToString() => Player.name;
}