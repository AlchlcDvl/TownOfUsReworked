using HarmonyLib;
using TMPro;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MayorMod
{
    [HarmonyPatch]
    public static class AddAbstain
    {
        public static void UpdateButton(Mayor role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.Abstain.gameObject.SetActive(skip.gameObject.active && !role.VotedOnce);
            role.Abstain.voteComplete = skip.voteComplete;
            role.Abstain.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            role.Abstain.GetComponentsInChildren<TextMeshPro>()[0].text = "Abstain";
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHudStart
        {
            public static void GenButton(Mayor role, MeetingHud __instance)
            {
                role.Abstain = Object.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
                role.Abstain.Parent = __instance;
                role.Abstain.SetTargetPlayerId(251);
                role.Abstain.transform.localPosition = __instance.SkipVoteButton.transform.localPosition + new Vector3(0f, -0.17f, 0f);
                __instance.SkipVoteButton.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                UpdateButton(role, __instance);
            }

            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                GenButton(mayorRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public static class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                UpdateButton(mayorRole, __instance);
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
                UpdateButton(mayorRole, __instance);
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

                UpdateButton(mayorRole, __instance);
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
                UpdateButton(mayorRole, __instance);
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

                UpdateButton(mayorRole, __instance);
            }
        }
    }
}