using System.Linq;
using Hazel;
using UnityEngine;
using UnityEngine.UI;
using Swaps = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    public class GuesserKill
    {
        public static void RpcMurderPlayer(Guesser assassin, PlayerControl player, string guess)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            RpcMurderPlayer(assassin, voteArea, player, guess);
        }

        public static void RpcMurderPlayer(Guesser assassin, PlayerVoteArea voteArea, PlayerControl player, string guess)
        {
            MurderPlayer(assassin, voteArea, player, guess);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.GuesserKill);
            writer.Write(player.PlayerId);
            writer.Write(guess);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(Guesser assassin, PlayerControl player, string guess)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            MurderPlayer(assassin, voteArea, player, guess);
        }

        public static void MurderPlayer(Guesser assassin, PlayerVoteArea voteArea, PlayerControl player, string guess)
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var assassinPlayer = assassin.Player;

            try
            {
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
            } catch {}

            if (PlayerControl.LocalPlayer == player)
                hudManager.KillOverlay.ShowKillAnimation(assassinPlayer.Data, player.Data);

            var amOwner = player.AmOwner;

            if (amOwner)
            {
                //Utils.ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);

                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostIgnoreTasks, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }
                else
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostDoTasks, new Il2CppReferenceArray<Il2CppSystem.Object>(0));

                player.myTasks.Insert(0, importantTextTask);

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    swapper.ListOfActives.Clear();
                    swapper.MoarButtons.Clear();
                    Swaps.SwapVotes.Swap1 = null;
                    Swaps.SwapVotes.Swap2 = null;
                    var buttons = Role.GetRole<Swapper>(player).MoarButtons;
                    
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Guesser))
                {
                    var assassin2 = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);
                    ShowHideGuessButtons.HideButtons(assassin2);
                }

                if (player.Is(AbilityEnum.Assassin))
                {
                    var assassin2 = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                    ShowHideButtons.HideButtons(assassin2);
                }
            }

            if (assassinPlayer != player)
            {
                player.Die(DeathReason.Kill, false);
                
                var role2 = Role.GetRole(player);
                role2.DeathReason = DeathReasonEnum.Guessed;
                role2.KilledBy = " By " + assassin.PlayerName;

                assassin.TargetGuessed = true;
                assassin.Wins();

                var meetingHud = MeetingHud.Instance;

                if (amOwner)
                    meetingHud.SetForegroundForDead();

                if (voteArea == null)
                    return;

                if (voteArea.DidVote)
                    voteArea.UnsetVote();

                voteArea.AmDead = true;
                voteArea.Overlay.gameObject.SetActive(true);
                voteArea.Overlay.color = Color.white;
                voteArea.XMark.gameObject.SetActive(true);
                voteArea.XMark.transform.localScale = Vector3.one;
            }
            else
            {
                assassin.RemainingGuesses--;

                foreach (var player2 in PlayerControl.AllPlayerControls)
                {
                    if (player2.Data.IsDead && assassin.Player != player2)
                    {
                        if (assassin.Player == player)
                            hudManager.Chat.AddChat(player2, $"{assassin.PlayerName} incorrectly guessed {player.name} as {guess} and lost a guess!");
                    }
                    else if (assassin.Player == player2 && assassin.Player == PlayerControl.LocalPlayer)
                    {
                        if (assassin.Player != player)
                            hudManager.Chat.AddChat(player2, $"You incorrectly guessed {player.name} as {guess} and lost a guess!");
                    }
                }
            }
        }
    }
}
