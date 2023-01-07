using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static class DeadSeeVoteColorsPatch
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] GameData.PlayerInfo voterPlayer, [HarmonyArgument(1)] int index,
            [HarmonyArgument(2)] Transform parent)
        {
            SpriteRenderer spriteRenderer = Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);

            if (CustomGameOptions.AnonymousFootPrint && (!CustomGameOptions.DeadSeeEverything || !PlayerControl.LocalPlayer.Data.IsDead))
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
            else
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);

            spriteRenderer.transform.SetParent(parent);
            spriteRenderer.transform.localScale = Vector3.zero;
            
            var Base = __instance as MonoBehaviour;
            Base.StartCoroutine(Effects.Bloop((float)index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }
}