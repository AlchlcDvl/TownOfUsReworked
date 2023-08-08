namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public static class LobbyBehaviourPatch
{
    public static void Postfix()
    {
        //Fix Grenadier and screwed blind in lobby
        HUD.FullScreen.gameObject.active = false;
        DataManager.Settings.Gameplay.ScreenShake = false;
        GameSettings.SettingsPage = 0;
        RoleGen.ResetEverything();
        PlayerLayer.DeleteAll();
        TownOfUsReworked.IsTest = IsLocalGame && (TownOfUsReworked.IsDev || TownOfUsReworked.MCIActive);
        StopAll();
        DefaultOutfitAll();

        if (!IsLocalGame)
            return;

        if (MCIUtils.Clients.Count != 0 && TownOfUsReworked.MCIActive && IsLocalGame)
        {
            var count = MCIUtils.Clients.Count;
            TownOfUsReworked.Debugger.TestWindow.Enabled = true;
            MCIUtils.Clients.Clear();
            MCIUtils.PlayerIdClientId.Clear();

            if (TownOfUsReworked.Persistence)
            {
                for (var i = 0; i < count; i++)
                    MCIUtils.CreatePlayerInstance();
            }
        }
    }
}