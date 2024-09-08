namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class ShowHostMeetingUpdatePatch
{
    public static void Postfix(MeetingHud __instance)
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
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class ShowHostMeetingStartPatch
{
    public static void Postfix(MeetingHud __instance)
    {
        if (!IsOnlineGame())
            return;

        __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
        __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
        __instance.HostIcon.enabled = true;
        __instance.HostIcon.gameObject.SetActive(true);
        __instance.ProceedButton.gameObject.SetActive(true);
    }
}