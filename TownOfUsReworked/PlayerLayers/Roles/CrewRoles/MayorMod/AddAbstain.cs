using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MayorMod
{
    [HarmonyPatch]
    public static class AddAbstain
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public static class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                Mayor.UpdateButton(mayorRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                mayorRole.Abstain.ClearButtons();
                Mayor.UpdateButton(mayorRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public static class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

                if (__0 != 251)
                    mayorRole.Abstain.ClearButtons();

                Mayor.UpdateButton(mayorRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                Mayor.UpdateButton(mayorRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

                if (__instance.state == MeetingHud.VoteStates.Discussion)
                {
                    if (__instance.discussionTimer < CustomGameOptions.DiscussionTime)
                        mayorRole.Abstain.SetDisabled();
                    else
                        mayorRole.Abstain.SetEnabled();
                }

                Mayor.UpdateButton(mayorRole, __instance);
            }
        }
    }
}