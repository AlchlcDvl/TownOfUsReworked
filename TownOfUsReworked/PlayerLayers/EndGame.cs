namespace TownOfUsReworked.PlayerLayers;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
public static class EndGame
{
    public static void Reset()
    {
        foreach (var role in Role.AllRoles)
        {
            role.AllPrints.ForEach(x => x.Destroy());
            role.AllPrints.Clear();
        }
    }

    public static void Prefix() => Reset();
}