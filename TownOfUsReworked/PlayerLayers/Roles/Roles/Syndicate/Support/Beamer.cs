using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Utilities;
using System.Linq;
using TMPro;
using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using System.Collections;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Beamer : Role
    {
        public DateTime LastBeamed { get; set; }
        public bool PressedButton;
        public bool MenuClick;
        public bool LastMouse;
        public ChatController BeamList { get; set; }
        public PlayerControl BeamPlayer1 { get; set; }
        public PlayerControl BeamPlayer2 { get; set; }
        public Dictionary<byte, DateTime> UnbeamablePlayers = new Dictionary<byte, DateTime>();
        private KillButton _beamButton;
        public DateTime LastKilled { get; set; }
        private KillButton _killButton;
        public PlayerControl ClosestPlayer = null;

        public Beamer(PlayerControl player) : base(player)
        {
            Name = "Beamer";
            StartText = "Choose Two Players To Swap Locations";
            AbilitiesText = "Choose two players to swap locations";
            Color = CustomGameOptions.CustomSynColors ? Colors.Beamer : Colors.Syndicate;
            LastBeamed = DateTime.UtcNow;
            RoleType = RoleEnum.Beamer;
            Faction = Faction.Syndicate;
            PressedButton = false;
            MenuClick = false;
            LastMouse = false;
            BeamList = null;
            BeamPlayer1 = null;
            BeamPlayer2 = null;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = "Syndicate (Support)";
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ChaosDriveKillCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public KillButton BeamButton
        {
            get => _beamButton;
            set
            {
                _beamButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float BeamTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBeamed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BeamCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Update(HudManager __instance)
        {
            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {
            if (PressedButton && BeamList == null)
            {
                BeamPlayer1 = null;
                BeamPlayer2 = null;
                __instance.Chat.SetVisible(false);
                BeamList = Object.Instantiate(__instance.Chat);
                BeamList.transform.SetParent(Camera.main.transform);
                BeamList.SetVisible(true);
                BeamList.Toggle();
                BeamList.TextBubble.enabled = false;
                BeamList.TextBubble.gameObject.SetActive(false);
                BeamList.TextArea.enabled = false;
                BeamList.TextArea.gameObject.SetActive(false);
                BeamList.BanButton.enabled = false;
                BeamList.BanButton.gameObject.SetActive(false);
                BeamList.CharCount.enabled = false;
                BeamList.CharCount.gameObject.SetActive(false);
                BeamList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                BeamList.OpenKeyboardButton.Destroy();
                BeamList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                BeamList.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                BeamList.BackgroundImage.enabled = false;

                foreach (var rend in BeamList.Content.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }
                }

                foreach (var bubble in BeamList.chatBubPool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                BeamList.chatBubPool.activeChildren.Clear();

                foreach (var TempPlayer in PlayerControl.AllPlayerControls)
                {
                    if (TempPlayer != null && TempPlayer.Data != null && !TempPlayer.Data.IsDead && !TempPlayer.Data.Disconnected &&
                        TempPlayer.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                    {
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (player != null && player.Data != null && ((!player.Data.Disconnected && !player.Data.IsDead) ||
                                Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == player.PlayerId)))
                            {
                                BeamList.AddChat(TempPlayer, "Click here");
                                BeamList.chatBubPool.activeChildren[BeamList.chatBubPool.activeChildren._size - 1].Cast<ChatBubble>().SetName(player.Data.PlayerName,
                                    false, false, PlayerControl.LocalPlayer.PlayerId == player.PlayerId ? Color : new Color32(255, 255, 255, 255));
                                var IsDeadTemp = player.Data.IsDead;
                                player.Data.IsDead = false;
                                BeamList.chatBubPool.activeChildren[BeamList.chatBubPool.activeChildren._size - 1].Cast<ChatBubble>().SetCosmetics(player.Data);
                                player.Data.IsDead = IsDeadTemp;
                            }
                        }

                        break;
                    }
                }
            }

            if (BeamList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (!BeamList.IsOpen || MeetingHud.Instance || Input.GetKeyInt(KeyCode.Escape) || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    BeamList.Toggle();
                    BeamList.SetVisible(false);
                    BeamList = null;
                    PressedButton = false;
                    BeamPlayer1 = null;
                }
                else
                {
                    foreach (var bubble in BeamList.chatBubPool.activeChildren)
                    {
                        if (BeamTimer() == 0f && BeamList != null)
                        {
                            Vector2 ScreenMin = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);

                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x)
                            {
                                if (Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                                {
                                    if (!Input.GetMouseButtonDown(0) && LastMouse)
                                    {
                                        LastMouse = false;
                                        
                                        foreach (var player in PlayerControl.AllPlayerControls)
                                        {
                                            if (player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                                            {
                                                if (BeamPlayer1 == null)
                                                {
                                                    BeamPlayer1 = player;
                                                    bubble.Cast<ChatBubble>().Background.color = Colors.Beamer;
                                                }
                                                else if (player.PlayerId == BeamPlayer1.PlayerId)
                                                {
                                                    BeamPlayer1 = null;
                                                    bubble.Cast<ChatBubble>().Background.color = new Color32(255, 255, 255, 255);
                                                }
                                                else
                                                {
                                                    PressedButton = false;
                                                    BeamList.Toggle();
                                                    BeamList.SetVisible(false);
                                                    BeamList = null;
                                                    BeamPlayer2 = player;

                                                    if (!UnbeamablePlayers.ContainsKey(BeamPlayer1.PlayerId) && !UnbeamablePlayers.ContainsKey(BeamPlayer2.PlayerId))
                                                    {
                                                        var interaction = Utils.Interact(Player, BeamPlayer1);
                                                        var interaction2 = Utils.Interact(Player, BeamPlayer2);

                                                        Utils.Spread(Player, BeamPlayer1);
                                                        Utils.Spread(Player, BeamPlayer2);

                                                        var transRole = GetRole<Beamer>(Player);

                                                        if (BeamPlayer1.Is(RoleEnum.Pestilence) || BeamPlayer1.IsOnAlert())
                                                        {
                                                            if (Player.IsShielded())
                                                            {
                                                                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                                                    (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                                                                writer2.Write(Player.GetMedic().Player.PlayerId);
                                                                writer2.Write(Player.PlayerId);
                                                                AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                                if (CustomGameOptions.ShieldBreaks)
                                                                    transRole.LastBeamed = DateTime.UtcNow;

                                                                StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                                return;
                                                            }
                                                            else if (!Player.IsProtected())
                                                            {
                                                                Coroutines.Start(BeamPlayers(BeamPlayer1.PlayerId, Player.PlayerId, true));
                                                                var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                    SendOption.Reliable, -1);
                                                                write2.Write((byte)ActionsRPC.Beam);
                                                                write2.Write(BeamPlayer1.PlayerId);
                                                                write2.Write(Player.PlayerId);
                                                                write2.Write(true);
                                                                AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                                return;
                                                            }

                                                            transRole.LastBeamed = DateTime.UtcNow;
                                                            return;
                                                        }
                                                        else if (BeamPlayer2.Is(RoleEnum.Pestilence) || BeamPlayer2.IsOnAlert())
                                                        {
                                                            if (Player.IsShielded())
                                                            {
                                                                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                    SendOption.Reliable, -1);
                                                                writer2.Write((byte)ActionsRPC.Beam);
                                                                writer2.Write(Player.GetMedic().Player.PlayerId);
                                                                writer2.Write(Player.PlayerId);
                                                                AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                                if (CustomGameOptions.ShieldBreaks)
                                                                    transRole.LastBeamed = DateTime.UtcNow;

                                                                StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                                return;
                                                            }
                                                            else if (!Player.IsProtected())
                                                            {
                                                                Coroutines.Start(BeamPlayers(BeamPlayer2.PlayerId, Player.PlayerId, true));
                                                                var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                    SendOption.Reliable, -1);
                                                                write2.Write((byte)ActionsRPC.Beam);
                                                                write2.Write(BeamPlayer2.PlayerId);
                                                                write2.Write(Player.PlayerId);
                                                                write2.Write(true);
                                                                AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                                return;
                                                            }

                                                            transRole.LastBeamed = DateTime.UtcNow;
                                                            return;
                                                        }

                                                        LastBeamed = DateTime.UtcNow;
                                                        Coroutines.Start(BeamPlayers(BeamPlayer1.PlayerId, BeamPlayer2.PlayerId, false));
                                                        var write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                            SendOption.Reliable, -1);
                                                        write.Write((byte)ActionsRPC.Beam);
                                                        write.Write(BeamPlayer1.PlayerId);
                                                        write.Write(BeamPlayer2.PlayerId);
                                                        write.Write(false);
                                                        AmongUsClient.Instance.FinishRpcImmediately(write);
                                                    }
                                                    else
                                                        (__instance as MonoBehaviour).StartCoroutine(Effects.SwayX(__instance.KillButton.transform));

                                                    BeamPlayer1 = null;
                                                    BeamPlayer2 = null;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!Input.GetMouseButtonDown(0) && LastMouse)
                    {
                        if (MenuClick)
                            MenuClick = false;
                        else
                        {
                            BeamList.Toggle();
                            BeamList.SetVisible(false);
                            BeamList = null;
                            PressedButton = false;
                            BeamPlayer1 = null;
                        }
                    }

                    LastMouse = Input.GetMouseButtonDown(0);
                }
            }
        }

        public static IEnumerator BeamPlayers(byte player1, byte player2, bool die)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer) || PlayerControl.LocalPlayer == Utils.PlayerById(player1) || PlayerControl.LocalPlayer == Utils.PlayerById(player2))
            {
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                } catch {}
            }

            var TP1 = Utils.PlayerById(player1);
            var TP2 = Utils.PlayerById(player2);
            var deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;

            if (TP1.Data.IsDead)
            {
                foreach (var body in deadBodies)
                {
                    if (body.ParentId == TP1.PlayerId)
                        Player1Body = body;
                }
            }

            if (TP2.Data.IsDead)
            {
                foreach (var body in deadBodies)
                {
                    if (body.ParentId == TP2.PlayerId)
                        Player2Body = body;
                }
            }

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                    yield return null;

                TP1.MyPhysics.ExitAllVents();
            }

            if (TP2.inVent && PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                    yield return null;

                TP2.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                var TempFacing = TP1.myRend().flipX;
                TP1.NetTransform.SnapTo(new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f));
                TP1.myRend().flipX = TP2.myRend().flipX;

                if (die)
                    Utils.MurderPlayer(TP1, TP2);
                else
                {
                    TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));
                    TP2.myRend().flipX = TempFacing;
                }

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }

                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TP2.GetTruePosition();
                TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                StopDragging(Player2Body.ParentId);
                TP1.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                TP1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                StopDragging(Player2Body.ParentId);
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = Player2Body.TruePosition;
                Player2Body.transform.position = TempPosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId || PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Beamer));

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }

            TP1.moveable = true;
            TP2.moveable = true;
            TP1.Collider.enabled = true;
            TP2.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.NetTransform.enabled = true;
        }

        public static void StopDragging(byte PlayerId)
        {
            var Undertaker = (Undertaker) Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Undertaker);

            if (Undertaker != null && Undertaker.CurrentlyDragging != null && Undertaker.CurrentlyDragging.ParentId == PlayerId)
                Undertaker.CurrentlyDragging = null;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntTraitor || IsIntFanatic)
                IntruderWin = true;
            else if (IsSynTraitor || IsSynFanatic)
                SyndicateWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else
                CrewWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsIntTraitor || IsIntFanatic)
            {
                if (Utils.IntrudersWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsSynTraitor || IsSynFanatic)
            {
                if (Utils.SyndicateWins())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsPersuaded)
            {
                if (Utils.SectWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsResurrected)
            {
                if (Utils.ReanimatedWin())
                {
                   Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.CrewWins())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}