namespace TownOfUsReworked.MultiClientInstancing;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
public static class OnGameStart
{
    public static void Prefix(AmongUsClient __instance)
    {
        if (TownOfUsReworked.MCIActive)
        {
            foreach (var client in __instance.allClients)
            {
                client.IsReady = true;
                client.Character.gameObject.GetComponent<DummyBehaviour>().enabled = false;
            }
        }
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
public static class SameVoteAll
{
    public static void Postfix(MeetingHud __instance, ref byte suspectStateIdx)
    {
        if (!IsLocalGame || !TownOfUsReworked.MCIActive || !TownOfUsReworked.SameVote)
            return;

        var sus = suspectStateIdx;
        CustomPlayer.AllPlayers.ForEach(x => __instance.CmdCastVote(x.PlayerId, sus));
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
public static class CacheGlassSprite
{
    public static Sprite Cache;

    public static void Prefix(MeetingHud __instance) => Cache ??= __instance.Glass.sprite;
}