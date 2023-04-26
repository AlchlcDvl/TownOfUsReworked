using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public static class ShowHideButtons
    {
        public static bool Prefix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                return true;

            var imitator = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            foreach (var button in imitator.OtherButtons.Where(button => button != null))
            {
                if (button.GetComponent<SpriteRenderer>().sprite == AssetManager.GetSprite("RetDeselect"))
                    button.SetActive(false);

                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            }

            if (imitator.ListOfActives.Count == 1)
            {
                for (var i = 0; i < imitator.ListOfActives.Count; i++)
                {
                    if (!imitator.ListOfActives[i])
                        continue;

                    VotingComplete.Imitate = __instance.playerStates[i];
                }
            }

            return true;
        }
    }
}