namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingHudStart
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
            {
                if (role.BlackmailedPlayer == null)
                    continue;

                if (role.BlackmailedPlayer == PlayerControl.LocalPlayer && !role.BlackmailedPlayer.Data.IsDead)
                    Coroutines.Start(BlackmailShhh());

                role.ShookAlready = false;
            }

            foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
            {
                if (role.BlackmailedPlayer == null || !role.IsBM)
                    continue;

                if (role.BlackmailedPlayer == PlayerControl.LocalPlayer && !role.BlackmailedPlayer.Data.IsDead)
                    Coroutines.Start(BlackmailShhh());

                role.ShookAlready = false;
            }

            foreach (var role in Role.GetRoles<Silencer>(RoleEnum.Silencer))
            {
                if (role.SilencedPlayer == null)
                    continue;

                if (role.SilencedPlayer == PlayerControl.LocalPlayer && !role.SilencedPlayer.Data.IsDead)
                    Coroutines.Start(SilencedShhh());

                role.ShookAlready = false;
            }

            foreach (var role in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
            {
                if (role.SilencedPlayer == null || !role.IsSil)
                    continue;

                if (role.SilencedPlayer == PlayerControl.LocalPlayer && !role.SilencedPlayer.Data.IsDead)
                    Coroutines.Start(BlackmailShhh());

                role.ShookAlready = false;
            }
        }

        public static IEnumerator BlackmailShhh()
        {
            yield return HudManager.Instance.CoFadeFullScreen(UColor.clear, new(0f, 0f, 0f, 0.98f));
            var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
            var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
            var pos = HudManager.Instance.shhhEmblem.transform.localPosition;
            pos.z++;
            HudManager.Instance.shhhEmblem.transform.localPosition = pos;
            HudManager.Instance.shhhEmblem.TextImage.text = "YOU ARE BLACKMAILED";
            HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
            yield return HudManager.Instance.ShowEmblem(true);
            HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
            HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
            yield return HudManager.Instance.CoFadeFullScreen(new(0f, 0f, 0f, 0.98f), UColor.clear);
        }

        public static IEnumerator SilencedShhh()
        {
            yield return HudManager.Instance.CoFadeFullScreen(UColor.clear, new(0f, 0f, 0f, 0.98f));
            var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
            var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
            var pos = HudManager.Instance.shhhEmblem.transform.localPosition;
            pos.z++;
            HudManager.Instance.shhhEmblem.transform.localPosition = pos;
            HudManager.Instance.shhhEmblem.TextImage.text = "YOU ARE SILENCED";
            HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
            yield return HudManager.Instance.ShowEmblem(true);
            HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
            HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
            yield return HudManager.Instance.CoFadeFullScreen(new(0f, 0f, 0f, 0.98f), UColor.clear);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHud_Update
    {
        private static Sprite CachedOverlay;
        private static Color CachedColor;

        public static void Postfix(MeetingHud __instance)
        {
            if (CustomGameOptions.BMRevealed)
            {
                foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
                {
                    if (role.BlackmailedPlayer == null)
                        continue;

                    if (!role.BlackmailedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.BlackmailedPlayer.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);

                        if (CachedOverlay == null)
                            CachedOverlay = playerState.Overlay.sprite;

                        if (CachedColor == default)
                            CachedColor = playerState.Overlay.color;

                        if (role.PrevOverlay == null)
                            role.PrevOverlay = CachedOverlay;

                        if (role.PrevColor == default)
                            role.PrevColor = CachedColor;

                        playerState.Overlay.sprite = AssetManager.GetSprite("Overlay");
                        playerState.Overlay.color = role.BlackmailedPlayer.IsSilenced() ? Colors.What : Colors.Blackmailer;

                        if (__instance.state != MeetingHud.VoteStates.Animating && !role.ShookAlready)
                        {
                            role.ShookAlready = true;
                            __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                        }
                    }
                }

                foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
                {
                    if (role.BlackmailedPlayer == null || !role.IsBM)
                        continue;

                    if (!role.BlackmailedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.BlackmailedPlayer.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);

                        if (CachedOverlay == null)
                            CachedOverlay = playerState.Overlay.sprite;

                        if (CachedColor == default)
                            CachedColor = playerState.Overlay.color;

                        if (role.PrevOverlay == null)
                            role.PrevOverlay = CachedOverlay;

                        if (role.PrevColor == default)
                            role.PrevColor = CachedColor;

                        playerState.Overlay.sprite = AssetManager.GetSprite("Overlay");
                        playerState.Overlay.color = role.BlackmailedPlayer.IsSilenced() ? Colors.What : Colors.Blackmailer;

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
                foreach (var role in Role.GetRoles<Silencer>(RoleEnum.Silencer))
                {
                    if (role.SilencedPlayer == null)
                        continue;

                    if (!role.SilencedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.SilencedPlayer.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);

                        if (CachedOverlay == null)
                            CachedOverlay = playerState.Overlay.sprite;

                        if (CachedColor == default)
                            CachedColor = playerState.Overlay.color;

                        if (role.PrevOverlay == null)
                            role.PrevOverlay = CachedOverlay;

                        if (role.PrevColor == default)
                            role.PrevColor = CachedColor;

                        playerState.Overlay.sprite = AssetManager.GetSprite("Overlay");
                        playerState.Overlay.color = role.SilencedPlayer.IsBlackmailed() ? Colors.What : Colors.Silencer;

                        if (__instance.state != MeetingHud.VoteStates.Animating && !role.ShookAlready)
                        {
                            role.ShookAlready = true;
                            __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                        }
                    }
                }

                foreach (var role in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
                {
                    if (role.SilencedPlayer == null || !role.IsSil)
                        continue;

                    if (!role.SilencedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.SilencedPlayer.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);

                        if (CachedOverlay == null)
                            CachedOverlay = playerState.Overlay.sprite;

                        if (CachedColor == default)
                            CachedColor = playerState.Overlay.color;

                        if (role.PrevOverlay == null)
                            role.PrevOverlay = CachedOverlay;

                        if (role.PrevColor == default)
                            role.PrevColor = CachedColor;

                        playerState.Overlay.sprite = AssetManager.GetSprite("Overlay");
                        playerState.Overlay.color = role.SilencedPlayer.IsBlackmailed() ? Colors.What : Colors.Silencer;

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
}