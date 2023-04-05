using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PoliticianMod
{
    [HarmonyPatch(typeof(PlayerVoteArea))]
    public static class AllowExtraVotes
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
        public static class Select
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
                    return true;

                var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);

                if (PlayerControl.LocalPlayer.Data.IsDead || __instance.AmDead || !role.CanVote || !__instance.Parent.Select(__instance.TargetPlayerId))
                    return false;

                __instance.Buttons.SetActive(true);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
                    return true;

                var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);

                if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding || __instance.Parent.state == MeetingHud.VoteStates.Results)
                    return false;

                if (!role.CanVote)
                    return false;

                role.VoteBank--;
                role.VotedOnce = true;
                __instance.Parent.Confirm(__instance.TargetPlayerId);
                return false;
            }
        }
    }
}