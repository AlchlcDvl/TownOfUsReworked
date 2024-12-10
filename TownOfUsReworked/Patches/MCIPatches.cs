namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
public static class OnGameStart
{
    public static void Prefix(AmongUsClient __instance)
    {
        if (!TownOfUsReworked.MCIActive)
            return;

        foreach (var client in __instance.allClients)
        {
            client.IsReady = true;
            client.Character.GetComponent<DummyBehaviour>().enabled = false;
        }
    }
}