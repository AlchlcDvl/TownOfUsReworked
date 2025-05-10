namespace TownOfUsReworked.Utils;

// Thanks to Town Of Host parts of this code
public static class GameStateUtils
{
    public static bool IsCountDown() => GameStartManager.Instance?.startState == GameStartManager.StartingStates.Countdown;

    public static bool IsInGame() => !Lobby() && (AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started || GameManager.Instance?.GameHasStarted == true ||
        AmongUsClient.Instance?.IsGameStarted == true);

    public static bool IsLobby() => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Joined || Lobby();

    public static bool IsEnded() => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Ended || AmongUsClient.Instance?.IsGameOver == true;

    public static bool IsHnS() => GameOptionsManager.Instance?.CurrentGameOptions?.GameMode == GameModes.HideNSeek;

    public static bool IsNormal() => GameOptionsManager.Instance?.CurrentGameOptions?.GameMode == GameModes.Normal;

    public static bool IsOnlineGame() => AmongUsClient.Instance?.NetworkMode == NetworkModes.OnlineGame;

    public static bool IsLocalGame() => AmongUsClient.Instance?.NetworkMode == NetworkModes.LocalGame;

    public static bool IsFreePlay() => AmongUsClient.Instance?.NetworkMode == NetworkModes.FreePlay;

    public static bool IsAllAny() => GameModeSettings.GameMode == Mode.AllAny;

    public static bool IsClassic() => GameModeSettings.GameMode == Mode.Classic;

    public static bool IsList() => GameModeSettings.GameMode == Mode.List;

    public static bool IsTaskRace() => GameModeSettings.GameMode == Mode.TaskRace;

    public static bool IsCustomHnS() => GameModeSettings.GameMode == Mode.HideAndSeek;

    public static bool NoLobby() => !(IsInGame() || IsLobby() || IsEnded() || Meeting());

    public static bool Last(Faction faction) => AllPlayers().Count(x => x.Is(faction) && !x.HasDied()) == 1;

    public static bool NoPlayers() => !AllPlayers().Any() || !CustomPlayer.Local || !CustomPlayer.Local.Data || NoLobby();

    public static bool LocalBlocked() => CustomPlayer.Local.IsBlocked();

    public static bool DeadSeeEverything()
    {
        if (!GameModifiers.DeadSeeEverything || !CustomPlayer.Local.HasDied() || !CustomPlayer.Local.Is<Role>(out var role) || !role.TrulyDead)
            return false;

        return role switch
        {
            Jester jest => !jest.CanHaunt,
            GuardianAngel ga => ga.Failed || !GuardianAngel.ProtectBeyondTheGrave || !ga.GraveProtectButton.Usable(),
            _ => true
        };
    }
}