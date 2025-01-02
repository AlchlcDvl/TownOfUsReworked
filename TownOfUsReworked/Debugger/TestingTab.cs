namespace TownOfUsReworked.Debugger;

public class TestingTab : BaseTab
{
    public override string Name => "Testing";

    public override void OnGUI()
    {
        TownOfUsReworked.Persistence.Value = GUILayout.Toggle(TownOfUsReworked.Persistence.Value, "Bot Persistence");
        TownOfUsReworked.AutoPlayAgain.Value = GUILayout.Toggle(TownOfUsReworked.AutoPlayAgain.Value, "Auto Play Again");
        TownOfUsReworked.DisableTimeout.Value = GUILayout.Toggle(TownOfUsReworked.DisableTimeout.Value, "Disable Lobby Timeout");
        TownOfUsReworked.RedirectLogger.Value = GUILayout.Toggle(TownOfUsReworked.RedirectLogger.Value, "Redirect Logger");

        if (CustomPlayer.Local)
            CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Player Collider");

        if (Lobby())
        {
            if (IsLocalGame())
            {
                if (GUILayout.Button("Spawn Bot"))
                {
                    if (GameData.Instance.PlayerCount < GameSettings.LobbySize)
                    {
                        MCIUtils.CleanUpLoad();
                        MCIUtils.CreatePlayerInstance();
                        TownOfUsReworked.MCIActive = true;
                    }
                }

                if (GUILayout.Button("Remove Last Bot"))
                {
                    MCIUtils.RemovePlayer((byte)MCIUtils.Clients.Count);
                    DebuggerBehaviour.Instance.ControllingFigure = 0;

                    if (MCIUtils.Clients.Count == 0)
                        TownOfUsReworked.MCIActive = false;
                }

                if (GUILayout.Button("Remove All Bots"))
                {
                    MCIUtils.RemoveAllPlayers();
                    DebuggerBehaviour.Instance.ControllingFigure = 0;
                    TownOfUsReworked.MCIActive = false;
                }
            }
        }

        if (GUILayout.Button("Test Achievement"))
            CustomAchievementManager.UnlockAchievement("Test");
    }
}