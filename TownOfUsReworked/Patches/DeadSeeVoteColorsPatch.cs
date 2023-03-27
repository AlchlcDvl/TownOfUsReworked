using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static class DeadSeeVoteColorsPatch
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] GameData.PlayerInfo voterPlayer, [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
        {
            var spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);
            var insiderFlag = false;
            var deadFlag = CustomGameOptions.DeadSeeEverything && PlayerControl.LocalPlayer.Data.IsDead;

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Insider))
                insiderFlag = Role.GetRole(PlayerControl.LocalPlayer).TasksDone;

            if (CustomGameOptions.AnonymousVoting && !(deadFlag || insiderFlag))
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
            else
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);

            spriteRenderer.transform.SetParent(parent);
            spriteRenderer.transform.localScale = Vector3.zero;

            var Base = __instance as MonoBehaviour;
            Base.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }
}