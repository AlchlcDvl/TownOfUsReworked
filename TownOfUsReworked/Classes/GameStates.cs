using AmongUs.GameOptions;

namespace TownOfUsReworked.Classes
{
    //Thanks to Town Of Host for this code
    public static class GameStates
    {
        public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
        public static bool IsInGame => AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && !LobbyBehaviour.Instance;
        public static bool IsLobby => AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined || LobbyBehaviour.Instance;
        public static bool IsEnded => AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended;
        public static bool IsHnS => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
        public static bool IsNormal => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
        public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
        public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
        public static bool IsRoaming => IsInGame && !MeetingHud.Instance && !Minigame.Instance;
        public static bool IsMeeting => IsInGame && MeetingHud.Instance;
    }
}