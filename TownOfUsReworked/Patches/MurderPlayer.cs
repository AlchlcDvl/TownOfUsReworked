namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class Murder
    {
        public readonly static List<DeadPlayer> KilledPlayers = new();

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var deadBody = new DeadPlayer
            {
                PlayerId = target.PlayerId,
                KillerId = __instance.PlayerId,
                KillTime = DateTime.UtcNow
            };

            KilledPlayers.Add(deadBody);
        }
    }
}