namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerVoteArea))]
public static class PlayerVoteAreaPatches
{
    [HarmonyPatch(nameof(PlayerVoteArea.Select)), HarmonyPrefix]
    public static bool SelectPrefix(PlayerVoteArea __instance)
    {
        if (!CustomPlayer.Local.TryGetLayer<Politician>(out var pol) || CustomPlayer.LocalCustom.Dead || __instance.AmDead || !__instance.Parent ||
            !__instance.Parent.Select(__instance.TargetPlayerId))
        {
            return true;
        }

        if (pol.CanVote)
        {
            __instance.Buttons.SetActive(true);
            var startPos = __instance.AnimateButtonsFromLeft ? 0.2f : 1.95f;
            Coroutines.Start(PerformTimedAction(0.25f, t => __instance.CancelButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 1.3f, Effects.ExpOut(t))));
            Coroutines.Start(PerformTimedAction(0.35f, t => __instance.ConfirmButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 0.65f, Effects.ExpOut(t))));
            var list = new List<UiElement>() { __instance.CancelButton, __instance.ConfirmButton };
            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.CancelButton, __instance.ConfirmButton, list.ToIl2Cpp(), false);
        }

        return false;
    }

    [HarmonyPatch(nameof(PlayerVoteArea.VoteForMe)), HarmonyPrefix]
    public static bool VoteForMePrefix(PlayerVoteArea __instance)
    {
        if (!CustomPlayer.Local.TryGetLayer<Politician>(out var pol))
            return true;

        if (__instance.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results || !pol.CanVote)
            return false;

        if (__instance == pol.Abstain)
            pol.SelfVote = true;
        else
        {
            pol.VoteBank--;
            pol.VotedOnce = true;
        }

        __instance.Parent.Confirm(__instance.TargetPlayerId);
        return false;
    }

    [HarmonyPatch(nameof(PlayerVoteArea.PreviewNameplate))]
    public static void Postfix(PlayerVoteArea __instance, string plateID)
    {
        if (CustomNameplateManager.CustomNameplateViewDatas.TryGetValue(plateID, out var viewData))
            __instance.Background.sprite = viewData?.Image;
    }

    [HarmonyPatch(nameof(PlayerVoteArea.SetCosmetics)), HarmonyPostfix]
    public static void SetCosmeticsPostfix(PlayerVoteArea __instance)
    {
        if (BetterSabotages.CamouflagedMeetings && HudHandler.Instance.IsCamoed)
        {
            __instance.Background.sprite = Ship().CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            var level = __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>();
            level.enabled = false;
            level.gameObject.SetActive(false);
            __instance.PlayerIcon.SetBodyCosmeticsVisible(false);
            __instance.PlayerIcon.SetBodyColor(16);
        }
        else
        {
            if (ClientOptions.WhiteNameplates)
                __instance.Background.sprite = Ship().CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            if (ClientOptions.NoLevels)
            {
                var level = __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>();
                level.enabled = false;
                level.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPatch(nameof(PlayerVoteArea.Start)), HarmonyPostfix]
    public static void StartPostfix(PlayerVoteArea __instance)
    {
        __instance.EnsureComponent<VoteAreaHandler>();

        if (__instance.transform.Find("PlayerID"))
            return;

        var level = __instance.transform.Find("PlayerLevel");

        if (!level)
            return;

        level.SetLocalZ(-2f);

        var id = UObject.Instantiate(level, level.parent);
        id.SetSiblingIndex(level.GetSiblingIndex() + 1);
        id.name = "PlayerID";
        id.SetLocalZ(-2f);

        var label = id.Find("LevelLabel");
        label.GetComponent<TextTranslatorTMP>().Destroy();
        label.GetComponent<TextMeshPro>().SetText("ID");
        label.name = "IDLabel";

        var number = id.Find("LevelNumber");
        number.GetComponent<TextTranslatorTMP>().Destroy();
        number.GetComponent<TextMeshPro>().SetText($"{__instance.TargetPlayerId}");
        number.name = "IDNumber";

        id.position -= new Vector3(0f, 0.33f, 0f);
    }
}