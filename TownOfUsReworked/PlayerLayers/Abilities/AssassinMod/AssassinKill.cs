using System.Linq;
using Hazel;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.PlayerLayers.Abilities.AssassinMod
{
    public class AssassinKill
    {
        public static void RpcMurderPlayer(Assassin assassin, PlayerControl player)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            RpcMurderPlayer(assassin, voteArea, player);
        }

        public static void RpcMurderPlayer(Assassin assassin, PlayerVoteArea voteArea, PlayerControl player)
        {
            MurderPlayer(assassin, voteArea, player);

            unchecked
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AssassinKill,
                    SendOption.Reliable, -1);
                writer.Write(player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public static void MurderPlayer(Assassin assassin, PlayerControl player, bool checkLover = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);
            MurderPlayer(assassin, voteArea, player, checkLover);
        }

        public static void MurderPlayer(Assassin assassin, PlayerVoteArea voteArea, PlayerControl player, bool checkLover = true)
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var assassinPlayer = assassin.Player;

            if (checkLover)
            {
                try
                {
                    SoundManager.Instance.PlaySound(TownOfUsReworked.KillSFX, false, 1f);
                } catch {}

                if (PlayerControl.LocalPlayer == player)
                    hudManager.KillOverlay.ShowKillAnimation(assassinPlayer.Data, player.Data);
            }

            var amOwner = player.AmOwner;

            if (amOwner)
            {
                Utils.ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);

                if (!PlayerControl.GameOptions.GhostsDoTasks)
                {
                    for (int i = 0;i < player.myTasks.Count;i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    swapper.ListOfActives.Clear();
                    swapper.Buttons.Clear();
                    SwapVotes.Swap1 = null;
                    SwapVotes.Swap2 = null;
                    var buttons = Role.GetRole<Swapper>(player).Buttons;
                    
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(AbilityEnum.Assassin))
                {
                    ShowHideButtons.HideButtons(assassin);
                }
            }

            player.Die(DeathReason.Kill);

            if (checkLover && player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Objectifier.GetObjectifier<Lovers>(PlayerControl.LocalPlayer).Player;
                
                if (!otherLover.Is(RoleEnum.Pestilence))
                    MurderPlayer(assassin, otherLover, false);
            }

            var meetingHud = MeetingHud.Instance;

            if (amOwner)
                meetingHud.SetForegroundForDead();

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

            var blackmailers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();

            foreach (var role in blackmailers)
            {
                if (role.Blackmailed != null && voteArea.TargetPlayerId == role.Blackmailed.PlayerId)
                {
                    if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(voteArea.XMark.transform.localPosition.x -
                            BlackmailMeetingUpdate.LetterXOffset, voteArea.XMark.transform.localPosition.y -
                            BlackmailMeetingUpdate.LetterYOffset, voteArea.XMark.transform.localPosition.z);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !player.AmOwner && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                
                SwapVotes.Swap1 = voteArea == SwapVotes.Swap1 ? null : SwapVotes.Swap1;
                SwapVotes.Swap2 = voteArea == SwapVotes.Swap2 ? null : SwapVotes.Swap2;

                if (SwapVotes.Swap1 == null | SwapVotes.Swap2 == null)
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSwaps,
                        SendOption.Reliable, -1);
                    writer2.Write(sbyte.MaxValue);
                    writer2.Write(sbyte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                }

                var swapperrole = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);

                int index;

                for (index = 0; index < MeetingHud.Instance.playerStates.Length; index++)
                {
                    if (MeetingHud.Instance.playerStates[index].TargetPlayerId == voteArea.TargetPlayerId)
                        break;
                }

                swapperrole.Buttons[index].GetComponent<SpriteRenderer>().sprite = Roles.CrewRoles.SwapperMod.AddButton.DisabledSprite;
                swapperrole.ListOfActives[index] = false;
                swapperrole.Buttons[index].SetActive(false);
                swapperrole.Buttons[index].GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
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
                            mayor.ExtraVotes.Clear();
                        else
                        {
                            var votesRegained = mayor.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                            if (mayor.Player == PlayerControl.LocalPlayer)
                                mayor.VoteBank += votesRegained;
                                
                            unchecked
                            {
                                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                    (byte) CustomRPC.AddMayorVoteBank, SendOption.Reliable, -1);
                                writer.Write(mayor.Player.PlayerId);
                                writer.Write(votesRegained);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                            }
                        }
                    }
                }
                
                meetingHud.CheckForEndVoting();
            }
        }
    }
}
