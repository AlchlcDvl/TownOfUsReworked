namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingHudStart
{
    private static bool BeingBlackmailed;
    private static bool BeingSilenced;

    public static void Postfix()
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
        yield return HUD.CoFadeFullScreen(UColor.clear, new(0f, 0f, 0f, 0.98f));
        var TempPosition = HUD.shhhEmblem.transform.localPosition;
        var TempDuration = HUD.shhhEmblem.HoldDuration;
        var pos = HUD.shhhEmblem.transform.localPosition;
        pos.z++;
        HUD.shhhEmblem.transform.localPosition = pos;
        HUD.shhhEmblem.TextImage.text = status;
        HUD.shhhEmblem.HoldDuration = 2.5f;
        yield return HUD.ShowEmblem(true);
        HUD.shhhEmblem.transform.localPosition = TempPosition;
        HUD.shhhEmblem.HoldDuration = TempDuration;
        yield return HUD.CoFadeFullScreen(new(0f, 0f, 0f, 0.98f), UColor.clear);
        BeingBlackmailed = false;
        BeingSilenced = false;
        yield break;
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingHud_Update
{
    private static Sprite CachedOverlay;
    private static UColor? CachedColor;

    public static void Postfix(MeetingHud __instance)
    {
        if (CustomGameOptions.BMRevealed)
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
                    role.PrevOverlay ??= CachedOverlay;
                    role.PrevColor ??= CachedColor;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.BlackmailedPlayer.IsSilenced() ? CustomColorManager.What : CustomColorManager.Blackmailer;

                    if (__instance.state != MeetingHud.VoteStates.Animating && !role.ShookAlready)
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
                    role.PrevOverlay ??= CachedOverlay;
                    role.PrevColor ??= CachedColor;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.BlackmailedPlayer.IsSilenced() ? CustomColorManager.What : CustomColorManager.Blackmailer;

                    if (__instance.state != MeetingHud.VoteStates.Animating && !role.ShookAlready)
                    {
                        role.ShookAlready = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }
        }

        if (CustomGameOptions.SilenceRevealed)
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
                    role.PrevOverlay ??= CachedOverlay;
                    role.PrevColor ??= CachedColor;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.SilencedPlayer.IsBlackmailed() ? CustomColorManager.What : CustomColorManager.Silencer;

                    if (__instance.state != MeetingHud.VoteStates.Animating && !role.ShookAlready)
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
                    role.PrevOverlay ??= CachedOverlay;
                    role.PrevColor ??= CachedColor;
                    playerState.Overlay.sprite = GetSprite("Overlay");
                    playerState.Overlay.color = role.SilencedPlayer.IsBlackmailed() ? CustomColorManager.What : CustomColorManager.Silencer;

                    if (__instance.state != MeetingHud.VoteStates.Animating && !role.ShookAlready)
                    {
                        role.ShookAlready = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }
        }
    }
}