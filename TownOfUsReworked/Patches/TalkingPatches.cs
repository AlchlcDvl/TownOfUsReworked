namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MeetingHud))]
public static class TalkingPatches
{
    private static bool BeingBlackmailed;
    private static bool BeingSilenced;

    [HarmonyPatch(nameof(MeetingHud.Start)), HarmonyPostfix]
    public static void StartPostfix()
    {
        BeingBlackmailed = CustomPlayer.Local.IsBlackmailed() && !CustomPlayer.LocalCustom.Dead;
        BeingSilenced = CustomPlayer.Local.IsSilenced() && !CustomPlayer.LocalCustom.Dead;

        if (BeingBlackmailed && BeingSilenced)
            Coroutines.Start(Shhh("RIP YOU ARE BLACKMAILED AND SILENCED"));
        else if (BeingSilenced || BeingBlackmailed)
            Coroutines.Start(Shhh($"YOU ARE {(BeingBlackmailed ? "BLACKMAILED" : "SILENCED")}"));
    }

    private static IEnumerator Shhh(string status)
    {
        var hud = HUD();
        yield return hud.CoFadeFullScreen(UColor.clear, new(0f, 0f, 0f, 0.98f));
        var TempPosition = hud.shhhEmblem.transform.localPosition;
        var TempDuration = hud.shhhEmblem.HoldDuration;
        hud.shhhEmblem.transform.localPosition += new Vector3(0f, 0f, 1f);
        hud.shhhEmblem.TextImage.SetText(status);
        hud.shhhEmblem.HoldDuration = 2.5f;
        yield return hud.ShowEmblem(true);
        hud.shhhEmblem.transform.localPosition = TempPosition;
        hud.shhhEmblem.HoldDuration = TempDuration;
        yield return hud.CoFadeFullScreen(new(0f, 0f, 0f, 0.98f), UColor.clear);
        BeingBlackmailed = false;
        BeingSilenced = false;
    }

    public static Sprite CachedOverlay;
    public static UColor? CachedColor;

    [HarmonyPatch(nameof(MeetingHud.Update)), HarmonyPostfix]
    public static void UpdatePostfix(MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Animating)
            return;

        if (Blackmailer.BMRevealed)
        {
            foreach (var role in PlayerLayer.GetLayers<Blackmailer>())
            {
                if (!role.BlackmailedPlayer)
                    continue;

                if (!role.BlackmailedPlayer.HasDied())
                {
                    var playerState = VoteAreaByPlayer(role.BlackmailedPlayer);
                    playerState.Overlay.gameObject.SetActive(true);
                    CachedOverlay ??= playerState.Overlay.sprite;
                    CachedColor ??= playerState.Overlay.color;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.BlackmailedPlayer.IsSilenced() ? CustomColorManager.What : CustomColorManager.Blackmailer;

                    if (!role.ShookAlready)
                    {
                        role.ShookAlready = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }

            foreach (var role in PlayerLayer.GetLayers<PromotedGodfather>())
            {
                if (!role.BlackmailedPlayer || !role.IsBM)
                    continue;

                if (!role.BlackmailedPlayer.HasDied())
                {
                    var playerState = VoteAreaByPlayer(role.BlackmailedPlayer);
                    playerState.Overlay.gameObject.SetActive(true);
                    CachedOverlay ??= playerState.Overlay.sprite;
                    CachedColor ??= playerState.Overlay.color;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.BlackmailedPlayer.IsSilenced() ? CustomColorManager.What : CustomColorManager.Blackmailer;

                    if (!role.ShookAlready)
                    {
                        role.ShookAlready = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }
        }

        if (Silencer.SilenceRevealed)
        {
            foreach (var role in PlayerLayer.GetLayers<Silencer>())
            {
                if (!role.SilencedPlayer)
                    continue;

                if (!role.SilencedPlayer.HasDied())
                {
                    var playerState = VoteAreaByPlayer(role.SilencedPlayer);
                    playerState.Overlay.gameObject.SetActive(true);
                    CachedOverlay ??= playerState.Overlay.sprite;
                    CachedColor ??= playerState.Overlay.color;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.SilencedPlayer.IsBlackmailed() ? CustomColorManager.What : CustomColorManager.Silencer;

                    if (!role.ShookAlready)
                    {
                        role.ShookAlready = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }

            foreach (var role in PlayerLayer.GetLayers<PromotedRebel>())
            {
                if (!role.SilencedPlayer || !role.IsSil)
                    continue;

                if (!role.SilencedPlayer.HasDied())
                {
                    var playerState = VoteAreaByPlayer(role.SilencedPlayer);
                    playerState.Overlay.gameObject.SetActive(true);
                    CachedOverlay ??= playerState.Overlay.sprite;
                    CachedColor ??= playerState.Overlay.color;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.SilencedPlayer.IsBlackmailed() ? CustomColorManager.What : CustomColorManager.Silencer;

                    if (!role.ShookAlready)
                    {
                        role.ShookAlready = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }
        }
    }
}