namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MeetingHud))]
public static class ShowHostPatches
{
    [HarmonyPatch(nameof(MeetingHud.Update)), HarmonyPostfix]
    public static void UpdatePostfix(MeetingHud __instance)
    {
        if (!IsOnlineGame())
            return;

        var host = GameData.Instance.GetHost();

        if (host != null)
        {
            PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);
            __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>().text = $"HOST: {host.PlayerName}";
        }
    }

    [HarmonyPatch(nameof(MeetingHud.Start)), HarmonyPostfix]
    public static void StartPostfix(MeetingHud __instance)
    {
        if (!IsOnlineGame())
            return;

        __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
        __instance.ProceedButton.GetComponent<SpriteRenderer>().enabled = false;
        __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
        __instance.HostIcon.enabled = true;
        __instance.HostIcon.gameObject.SetActive(true);
        __instance.ProceedButton.gameObject.SetActive(true);
    }
}