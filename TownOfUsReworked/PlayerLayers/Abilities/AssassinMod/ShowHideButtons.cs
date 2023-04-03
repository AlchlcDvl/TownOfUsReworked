using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using UnityEngine.UI;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public static class ShowHideButtons
    {
        public static void HideButtons(Assassin role)
        {
            foreach (var (_, (cycleBack, cycleForward, guess, guessText)) in role.Buttons)
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

        public static void HideSingle(Assassin role, byte targetId, bool killedSelf, bool doubleshot)
        {
            if ((killedSelf || role.RemainingKills == 0 || !CustomGameOptions.AssassinMultiKill) && !doubleshot)
            {
                HideButtons(role);
                var role2 = Role.GetRole(role.Player);
                role2.DeathReason = DeathReasonEnum.Guessed;
                role2.KilledBy = " Via Misfire";
            }
            else
                HideTarget(role, targetId);
        }

        public static void HideTarget(Assassin role, byte targetId)
        {
            var (cycleBack, cycleForward, guess, guessText) = role.Buttons[targetId];

            if (cycleBack == null || cycleForward == null)
                return;

            cycleBack.SetActive(false);
            cycleForward.SetActive(false);
            guess.SetActive(false);
            guessText.gameObject.SetActive(false);

            cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            role.Buttons[targetId] = (null, null, null, null);
            role.Guesses.Remove(targetId);
        }

        public static void Prefix()
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin))
                return;

            var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);

            if (!CustomGameOptions.AssassinateAfterVoting)
                HideButtons(assassin);
        }
    }
}