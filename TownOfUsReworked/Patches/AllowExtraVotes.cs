namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
public static class Select
{
    public static bool Prefix(PlayerVoteArea __instance)
    {
        if (!CustomPlayer.Local.Is(LayerEnum.Politician) || CustomPlayer.LocalCustom.Dead || __instance.AmDead || !__instance.Parent || !__instance.Parent.Select(__instance.TargetPlayerId))
            return true;

        if (((Politician)Ability.LocalAbility).CanVote)
        {
            __instance.Buttons.SetActive(true);
            var startPos = __instance.AnimateButtonsFromLeft ? 0.2f : 1.95f;
            __instance.StartCoroutine(PerformTimedAction(0.25f, t => __instance.CancelButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 1.3f,
                Effects.ExpOut(t))));
            __instance.StartCoroutine(PerformTimedAction(0.35f, t => __instance.ConfirmButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 0.65f,
                Effects.ExpOut(t))));
            var list = new List<UiElement>() { __instance.CancelButton, __instance.ConfirmButton };
            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.CancelButton, __instance.ConfirmButton, list.SystemToIl2Cpp(), false);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
public static class VoteForMe
{
    public static bool Prefix(PlayerVoteArea __instance)
    {
        if (!CustomPlayer.Local.Is(LayerEnum.Politician))
            return true;

        if (__instance.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
            return false;

        if (CustomPlayer.Local.Is(LayerEnum.Politician))
        {
            var pol = (Politician)Ability.LocalAbility;

            if (!pol.CanVote)
                return false;

            if (__instance != pol.Abstain)
            {
                pol.VoteBank--;
                pol.VotedOnce = true;
            }
            else
                pol.SelfVote = true;
        }

        __instance.Parent.Confirm(__instance.TargetPlayerId);
        return false;
    }
}