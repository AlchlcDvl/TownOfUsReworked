using System.Linq;
using Hazel;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod;
using Reactor.Utilities;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Abilities.AssassinMod
{
    public static class AssassinKill
    {
        public static void RpcMurderPlayer(Assassin assassin, PlayerControl player, string guess)
        {
            var voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            RpcMurderPlayer(assassin, voteArea, player, guess);
        }

        public static void RpcMurderPlayer(Assassin assassin, PlayerVoteArea voteArea, PlayerControl player, string guess)
        {
            MurderPlayer(assassin, voteArea, player, guess);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.AssassinKill);
            writer.Write(player.PlayerId);
            writer.Write(guess);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(Assassin assassin, PlayerControl player, string guess, bool checkLover = true)
        {
            var voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            MurderPlayer(assassin, voteArea, player, guess, checkLover);
        }

        public static void MurderPlayer(Assassin assassin, PlayerVoteArea voteArea, PlayerControl player, string guess, bool checkLover = true)
        {
            var hudManager = HudManager.Instance;
            var assassinPlayer = assassin.Player;

            if (player != assassinPlayer && player.Is(ModifierEnum.Indomitable) && !player.Is(RoleEnum.Actor) && player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Indomitable));
                return;
            }

            if (checkLover)
            {
                try
                {
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                } catch {}

                if (PlayerControl.LocalPlayer == player)
                    hudManager.KillOverlay.ShowKillAnimation(assassinPlayer.Data, player.Data);
            }

            assassinPlayer.RegenTask();
            player.RegenTask();

            if (player.AmOwner)
            {
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    swapper.ListOfActives.Clear();
                    swapper.MoarButtons.Clear();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetSwaps);
                    writer.Write(sbyte.MaxValue);
                    writer.Write(sbyte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    SwapVotes.Swap1 = null;
                    SwapVotes.Swap2 = null;

                    foreach (var button in Role.GetRole<Swapper>(player).MoarButtons)
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

            player.Die(DeathReason.Kill, false);
            var role2 = Role.GetRole(player);
            role2.DeathReason = DeathReasonEnum.Guessed;
            role2.KilledBy = " By " + assassin.PlayerName;

            if (checkLover && player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Objectifier.GetObjectifier<Lovers>(PlayerControl.LocalPlayer).Player;

                if (!otherLover.Is(RoleEnum.Pestilence))
                    MurderPlayer(assassin, otherLover, guess, false);
            }

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);

            if (voteArea == null)
                return;

            if (voteArea.DidVote)
                voteArea.UnsetVote();

            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = MeetingHud.Instance;

            if (player.AmOwner)
                meetingHud.SetForegroundForDead();

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer).Cast<Blackmailer>())
            {
                if (role.BlackmailedPlayer != null && voteArea.TargetPlayerId == role.BlackmailedPlayer.PlayerId)
                {
                    if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                            voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset, voteArea.XMark.transform.localPosition.z);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var assassin2 = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin2, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                var button = swapper.MoarButtons[voteArea.TargetPlayerId];

                if (button.GetComponent<SpriteRenderer>().sprite == AssetManager.SwapperSwitch)
                {
                    swapper.ListOfActives[voteArea.TargetPlayerId] = false;

                    if (SwapVotes.Swap1 == voteArea)
                        SwapVotes.Swap1 = null;

                    if (SwapVotes.Swap2 == voteArea)
                        SwapVotes.Swap2 = null;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetSwaps);
                    writer.Write(sbyte.MaxValue);
                    writer.Write(sbyte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                button.SetActive(false);
                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                swapper.MoarButtons[voteArea.TargetPlayerId] = null;
            }

            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (playerVoteArea.VotedFor != player.PlayerId)
                    continue;

                playerVoteArea.UnsetVote();
                var voteAreaPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);

                if (!voteAreaPlayer.AmOwner)
                    continue;

                meetingHud.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Mayor))
                {
                    if (role is Mayor mayor)
                    {
                        if (role.Player == player)
                        {
                            mayor.ExtraVotes.Clear();
                        }
                        else
                        {
                            var votesRegained = mayor.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                            if (mayor.Player == PlayerControl.LocalPlayer)
                                mayor.VoteBank += votesRegained;

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddMayorVoteBank, SendOption.Reliable);
                            writer.Write(mayor.Player.PlayerId);
                            writer.Write(votesRegained);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                    }
                }

                AddHauntPatch.AssassinatedPlayers.Add(player);
                meetingHud.CheckForEndVoting();
            }

            if (assassinPlayer == PlayerControl.LocalPlayer)
            {
                if (assassinPlayer != player)
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You guessed {player.name} as {guess}!");
                else
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You incorrectly guessed {player.name} as {guess} and died!");
            }
            else if (assassinPlayer != player && PlayerControl.LocalPlayer == player)
            {
                hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{assassinPlayer.name} guessed you as {guess}!");
            }
            else
            {
                string something = "";

                if ((assassinPlayer.GetFaction() == PlayerControl.LocalPlayer.GetFaction() && (assassinPlayer.GetFaction() == Faction.Intruder || assassinPlayer.GetFaction() ==
                    Faction.Syndicate)) || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
                {
                    if (assassinPlayer != player)
                        something = $"{assassinPlayer.name} guessed {player.name} as {player}!";
                    else
                        something = $"{assassinPlayer.name} incorrectly guessed {player.name} as {player} and died!";
                }
            }
        }
    }
}