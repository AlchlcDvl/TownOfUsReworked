using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine.UI;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public class ShowHideGuessButtons
    {
        public static void HideButtons(Guesser role)
        {
            foreach (var (_, (cycleBack, cycleForward, guess, guessText)) in role.MoarButtons)
            {
                if (cycleBack == null || cycleForward == null)
                    continue;

                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                role.GuessedThisMeeting = true;
            }
        }

        public static void HideSingle(Guesser role, byte targetId)
        {
            if (role.RemainingGuesses == 0 || !CustomGameOptions.MultipleGuesses)
            {
                HideButtons(role);
                return;
            }

            var (cycleBack, cycleForward, guess, guessText) = role.MoarButtons[targetId];

            if (cycleBack == null || cycleForward == null)
                return;

            cycleBack.SetActive(false);
            cycleForward.SetActive(false);
            guess.SetActive(false);
            guessText.gameObject.SetActive(false);

            cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            role.MoarButtons[targetId] = (null, null, null, null);
            role.Guesses.Remove(targetId);
        }

        public static void Prefix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guesser))
                return;

            var assassin = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);

            if (!CustomGameOptions.GuesserAfterVoting)
                HideButtons(assassin);
        }
    }
}
