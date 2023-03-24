using System;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class AddButton
    {
        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead)
                return true;

            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            return player?.Data.IsDead != false || player.Data.Disconnected;
        }

        public static void GenButton(Guesser role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;

            if (IsExempt(voteArea) || Utils.PlayerById(targetId) != role.TargetPlayer)
            {
                role.MoarButtons[targetId] = (null, null, null, null);
                return;
            }

            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;

            var nameText = Object.Instantiate(voteArea.NameText, voteArea.transform);
            voteArea.NameText.transform.localPosition = new Vector3(0.55f, 0.12f, -0.1f);
            nameText.transform.localPosition = new Vector3(0.55f, -0.12f, -0.1f);
            nameText.text = "Guess";

            var cycleBack = Object.Instantiate(confirmButton, voteArea.transform);
            var cycleRendererBack = cycleBack.GetComponent<SpriteRenderer>();
            cycleRendererBack.sprite = AssetManager.CycleBack;
            cycleBack.transform.localPosition = new Vector3(-0.5f, 0.15f, -2f);
            cycleBack.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            cycleBack.layer = 5;
            cycleBack.transform.parent = parent;
            var cycleEventBack = new Button.ButtonClickedEvent();
            cycleEventBack.AddListener(Cycle(role, voteArea, nameText, false));
            cycleBack.GetComponent<PassiveButton>().OnClick = cycleEventBack;
            var cycleColliderBack = cycleBack.GetComponent<BoxCollider2D>();
            cycleColliderBack.size = cycleRendererBack.sprite.bounds.size;
            cycleColliderBack.offset = Vector2.zero;
            cycleBack.transform.GetChild(0).gameObject.Destroy();

            var cycleForward = Object.Instantiate(confirmButton, voteArea.transform);
            var cycleRendererForward = cycleForward.GetComponent<SpriteRenderer>();
            cycleRendererForward.sprite = AssetManager.CycleForward;
            cycleForward.transform.localPosition = new Vector3(-0.2f, 0.15f, -2f);
            cycleForward.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            cycleForward.layer = 5;
            cycleForward.transform.parent = parent;
            var cycleEventForward = new Button.ButtonClickedEvent();
            cycleEventForward.AddListener(Cycle(role, voteArea, nameText, true));
            cycleForward.GetComponent<PassiveButton>().OnClick = cycleEventForward;
            var cycleColliderForward = cycleForward.GetComponent<BoxCollider2D>();
            cycleColliderForward.size = cycleRendererForward.sprite.bounds.size;
            cycleColliderForward.offset = Vector2.zero;
            cycleForward.transform.GetChild(0).gameObject.Destroy();

            var guess = Object.Instantiate(confirmButton, voteArea.transform);
            var guessRenderer = guess.GetComponent<SpriteRenderer>();
            guessRenderer.sprite = AssetManager.Guess;
            guess.transform.localPosition = new Vector3(-0.35f, -0.15f, -2f);
            guess.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            guess.layer = 5;
            guess.transform.parent = parent;
            var guessEvent = new Button.ButtonClickedEvent();
            guessEvent.AddListener(Guess(role, voteArea));
            guess.GetComponent<PassiveButton>().OnClick = guessEvent;
            var bounds = guess.GetComponent<SpriteRenderer>().bounds;
            bounds.size = new Vector3(0.52f, 0.3f, 0.16f);
            var guessCollider = guess.GetComponent<BoxCollider2D>();
            guessCollider.size = guessRenderer.sprite.bounds.size;
            guessCollider.offset = Vector2.zero;
            guess.transform.GetChild(0).gameObject.Destroy();

            role.Guesses.Add(targetId, "None");
            role.MoarButtons[targetId] = (cycleBack, cycleForward, guess, nameText);
        }

        private static Action Cycle(Guesser role, PlayerVoteArea voteArea, TextMeshPro nameText, bool forwardsCycle = true)
        {
            void Listener()
            {
                if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion)
                    return;

                var currentGuess = role.Guesses[voteArea.TargetPlayerId];
                var guessIndex = currentGuess == "None" ? -1 : role.PossibleGuesses.IndexOf(currentGuess);

                if (forwardsCycle)
                {
                    if (++guessIndex >= role.PossibleGuesses.Count)
                        guessIndex = 0;
                }
                else
                {
                    if (--guessIndex < 0)
                        guessIndex = role.PossibleGuesses.Count - 1;
                }

                var newGuess = role.Guesses[voteArea.TargetPlayerId] = role.PossibleGuesses[guessIndex];
                nameText.text = newGuess == "None" ? "Guess" : $"<color=#{role.SortedColorMapping[newGuess].ToHtmlStringRGBA()}>{newGuess}</color>";
            }

            return Listener;
        }

        private static Action Guess(Guesser role, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea) || PlayerControl.LocalPlayer.Data.IsDead)
                    return;

                var targetId = voteArea.TargetPlayerId;
                var currentGuess = role.Guesses[targetId];
                var targetPlayer = Utils.PlayerById(targetId);

                if (currentGuess == "None")
                    return;

                var playerRole = Role.GetRole(voteArea);

                var roleflag = playerRole != null && playerRole.Name == currentGuess;
                var recruitflag = targetPlayer.IsRecruit() && currentGuess == "Recruit";
                var sectflag = targetPlayer.IsPersuaded() && currentGuess == "Persuaded";
                var reanimatedflag = targetPlayer.IsResurrected() && currentGuess == "Resurrected";
                var undeadflag = targetPlayer.IsBitten() && currentGuess == "Bitten";

                var flag = roleflag || recruitflag || sectflag || reanimatedflag || undeadflag;
                var toDie = flag ? playerRole.Player : role.Player;
                role.TargetGuessed = flag;
                GuesserKill.RpcMurderPlayer(role, toDie, currentGuess);
                ShowHideGuessButtons.HideSingle(role, targetId);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Guesser))
            {
                var guesser = (Guesser)role;
                guesser.Guesses.Clear();
                guesser.MoarButtons.Clear();
                guesser.GuessedThisMeeting = false;
            }

            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Guesser) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var guesserRole = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);

            if (guesserRole.RemainingGuesses <= 0)
                return;

            foreach (var voteArea in __instance.playerStates)
                GenButton(guesserRole, voteArea);
        }
    }
}
