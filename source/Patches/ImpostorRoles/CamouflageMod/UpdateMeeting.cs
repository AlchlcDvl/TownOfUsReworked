using HarmonyLib;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CamouflageMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public class UpdateMeeting
    {
        private static void Postfix(MeetingHud __instance)
        {
            if (CustomGameOptions.MeetingColourblind)

                foreach (var state in __instance.playerStates)
                {
                    state.PlayerIcon.SetName("1");
                    state.NameText.color = Palette.White;
                    state.PlayerIcon.SetBodyColor(6);
                    state.PlayerIcon.SetHat("None", 0);
                    state.PlayerIcon.SetSkin("None", 0);
                }
        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
    public class PlayerStates
    {
        private static void Postfix(PlayerVoteArea __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo)
        {
            if (CustomGameOptions.MeetingColourblind)
            {
                __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                __instance.NameText.color = Palette.White;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
    public class PlayerPreviews
    {
        private static void Postfix(PlayerVoteArea __instance, [HarmonyArgument(0)] string plateId)
        {
            if (CustomGameOptions.MeetingColourblind)
            {
                __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                __instance.NameText.color = Palette.White;
            }
        }
    }
}
