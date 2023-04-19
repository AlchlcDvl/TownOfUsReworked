using System.Linq;
using Hazel;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch]
    public static class GuesserKill
    {
        public static void RpcMurderPlayer(Guesser assassin, PlayerControl player, string guess)
        {
            MurderPlayer(assassin, player, guess);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.GuesserKill);
            writer.Write(player.PlayerId);
            writer.Write(guess);
            writer.Write(assassin.Player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(Guesser assassin, PlayerControl player, string guess)
        {
            var voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            var hudManager = HudManager.Instance;
            var assassinPlayer = assassin.Player;

            try
            {
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
            } catch {}

            if (PlayerControl.LocalPlayer == player)
                hudManager.KillOverlay.ShowKillAnimation(assassinPlayer.Data, player.Data);

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
                    writer.Write(player.PlayerId);
                    writer.Write(sbyte.MaxValue);
                    writer.Write(sbyte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    swapper.Swap1 = null;
                    swapper.Swap2 = null;

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

            if (assassinPlayer != player)
            {
                player.Die(DeathReason.Kill, false);

                var role2 = Role.GetRole(player);
                role2.DeathReason = DeathReasonEnum.Guessed;
                role2.KilledBy = " By " + assassin.PlayerName;

                if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
                {
                    var otherLover = Objectifier.GetObjectifier<Lovers>(PlayerControl.LocalPlayer).Player;

                    if (!otherLover.Is(RoleEnum.Pestilence))
                        MurderPlayer(assassin, otherLover, guess);
                }

                assassin.TargetGuessed = true;

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

                foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
                {
                    if (role.BlackmailedPlayer != null && voteArea.TargetPlayerId == role.BlackmailedPlayer.PlayerId && BlackmailMeetingUpdate.PrevXMark != null &&
                        BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                            voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset, voteArea.XMark.transform.localPosition.z);
                    }
                }

                foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
                {
                    if (role.BlackmailedPlayer != null && voteArea.TargetPlayerId == role.BlackmailedPlayer.PlayerId && BlackmailMeetingUpdate.PrevXMark != null &&
                        BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                            voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset, voteArea.XMark.transform.localPosition.z);
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

                        if (swapper.Swap1 == voteArea)
                            swapper.Swap1 = null;

                        if (swapper.Swap2 == voteArea)
                            swapper.Swap2 = null;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.SetSwaps);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
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
                    foreach (var mayor in Role.GetRoles<Mayor>(RoleEnum.Mayor))
                    {
                        if (mayor.Player == player)
                            mayor.ExtraVotes.Clear();
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

                    foreach (var pol in Role.GetRoles<Politician>(RoleEnum.Politician))
                    {
                        if (pol.Player == player)
                            pol.ExtraVotes.Clear();
                        else
                        {
                            var votesRegained = pol.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                            if (pol.Player == PlayerControl.LocalPlayer)
                                pol.VoteBank += votesRegained;

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddPoliticianVoteBank, SendOption.Reliable);
                            writer.Write(pol.Player.PlayerId);
                            writer.Write(votesRegained);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                    }

                    foreach (var reb in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
                    {
                        if (!reb.IsPol)
                            continue;

                        if (reb.Player == player)
                            reb.ExtraVotes.Clear();
                        else
                        {
                            var votesRegained = reb.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                            if (reb.Player == PlayerControl.LocalPlayer)
                                reb.VoteBank += votesRegained;

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddRebPoliticianVoteBank, SendOption.Reliable);
                            writer.Write(reb.Player.PlayerId);
                            writer.Write(votesRegained);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                    }

                    if (SetPostmortals.RevealerOn && SetPostmortals.WillBeRevealer == null && player.Is(Faction.Crew))
                    {
                        SetPostmortals.WillBeRevealer = player;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable, -1);
                        writer.Write(player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (SetPostmortals.PhantomOn && SetPostmortals.WillBePhantom == null && player.Is(Faction.Neutral) && !LayerExtentions.NeutralHasUnfinishedBusiness(player))
                    {
                        SetPostmortals.WillBePhantom = player;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                        writer.Write(player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (SetPostmortals.BansheeOn && SetPostmortals.WillBeBanshee == null && player.Is(Faction.Syndicate))
                    {
                        SetPostmortals.WillBeBanshee = player;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable, -1);
                        writer.Write(player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (SetPostmortals.GhoulOn && SetPostmortals.WillBeGhoul == null && player.Is(Faction.Intruder))
                    {
                        SetPostmortals.WillBeGhoul = player;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable, -1);
                        writer.Write(player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    SetPostmortals.AssassinatedPlayers.Add(player);
                    meetingHud.CheckForEndVoting();
                }
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