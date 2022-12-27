namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for this code
    public static class GameStates
    {
        public static bool InGame = false;
        public static bool IsLobby => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Joined;
        public static bool IsInGame => InGame;
        public static bool IsEnded => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Ended;
        public static bool IsNotJoined => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.NotJoined;
        public static bool IsOnlineGame => AmongUsClient.Instance.GameMode == GameModes.OnlineGame;
        public static bool IsLocalGame => AmongUsClient.Instance.GameMode == GameModes.LocalGame;
        public static bool IsFreePlay => AmongUsClient.Instance.GameMode == GameModes.FreePlay;
        public static bool IsInTask => InGame && !MeetingHud.Instance;
        public static bool IsMeeting => InGame && MeetingHud.Instance;
        public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState ==
            GameStartManager.StartingStates.Countdown;
    }
}