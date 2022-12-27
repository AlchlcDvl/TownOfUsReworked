using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using UnityEngine.UI;

namespace TownOfUsReworked.PlayerLayers.Abilities.InsiderMod
{
    public class ShowHideButtons
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Insider))
                {
                    var insider = Ability.GetAbility<Insider>(PlayerControl.LocalPlayer);

                    foreach (var button in insider.Buttons)
                    {
                        if (button == null)
                            continue;

                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }
            }
        }
    }
}