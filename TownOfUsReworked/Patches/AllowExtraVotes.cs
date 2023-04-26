using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
    public static class Select
    {
        public static bool Prefix(PlayerVoteArea __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor) && !PlayerControl.LocalPlayer.Is(RoleEnum.Politician) && !PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
                return true;

            var flag = (PlayerControl.LocalPlayer.Is(RoleEnum.Mayor) && !Role.GetRole<Mayor>(PlayerControl.LocalPlayer).CanVote) || (PlayerControl.LocalPlayer.Is(RoleEnum.Politician) &&
                !Role.GetRole<Politician>(PlayerControl.LocalPlayer).CanVote) || (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel) &&
                (!Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer).IsPol || (Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer).IsPol &&
                !Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer).CanVote)));

            if (!(PlayerControl.LocalPlayer.Data.IsDead || __instance.AmDead || !__instance.Parent.Select(__instance.TargetPlayerId) || flag))
                __instance.Buttons.SetActive(true);

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
    public static class VoteForMe
    {
        public static bool Prefix(PlayerVoteArea __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor) && !PlayerControl.LocalPlayer.Is(RoleEnum.Politician) && !PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
                return true;

            if (__instance.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
                return false;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
            {
                var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

                if (!role.CanVote)
                    return false;

                if (__instance != role.Abstain)
                {
                    role.VoteBank--;
                    role.VotedOnce = true;
                }
                else
                    role.SelfVote = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
            {
                var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);

                if (!role.CanVote)
                    return false;

                role.VoteBank--;
                role.VotedOnce = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
            {
                var role = Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer);

                if (!role.IsPol || !role.CanVote)
                    return false;

                role.VoteBank--;
                role.VotedOnce = true;
            }

            __instance.Parent.Confirm(__instance.TargetPlayerId);
            return false;
        }
    }
}