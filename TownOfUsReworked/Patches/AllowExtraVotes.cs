namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
    public static class Select
    {
        public static bool Prefix(PlayerVoteArea __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Politician))
                return true;

            var flag = PlayerControl.LocalPlayer.Is(AbilityEnum.Politician) && !((Politician)Ability.LocalAbility).CanVote;
            __instance.Buttons.SetActive(!(PlayerControl.LocalPlayer.Data.IsDead || __instance.AmDead || !__instance.Parent.Select(__instance.TargetPlayerId) || flag));
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
    public static class VoteForMe
    {
        public static bool Prefix(PlayerVoteArea __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Politician))
                return true;

            if (__instance.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
                return false;

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Politician))
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
}