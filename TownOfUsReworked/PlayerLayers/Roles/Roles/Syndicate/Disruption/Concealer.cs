using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Hazel;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Concealer : Role
    {
        private KillButton _concealButton;
        private KillButton _killButton;
        public bool Enabled;
        public DateTime LastConcealed { get; set; }
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;
        public PlayerControl ClosestPlayer = null;
        public DateTime LastKilled { get; set; }

        public Concealer(PlayerControl player) : base(player)
        {
            Name = "Concealer";
            StartText = "Make The <color=#8BFDFDFF>Crew</color> Invisible For Some Chaos";
            AbilitiesText = "- You can turn off player's ability to see things properly, making all other players appear invisible.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
            RoleType = RoleEnum.Concealer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            RoleDescription = "You are a Concealer! You can turn everyone invisible to everyone else but themselves by making them unable to see things properly. " +
                "Use this to get away from crime scenes as fast as possible!";
            Objectives = SyndicateWinCon;
            InspectorResults = InspectorResults.Unseen;
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

        public KillButton ConcealButton
        {
            get => _concealButton;
            set
            {
                _concealButton = value;
                AddToAbilityButtons(value, this);
            }
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

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var color = new Color32(0, 0, 0, 0);

                if (PlayerControl.LocalPlayer.Is(Faction) || PlayerControl.LocalPlayer.Data.IsDead)
                    color.a = 26;

                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                    {
                        ColorId = player.CurrentOutfit.ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " "
                    });

                    player.myRend().color = color;
                    player.nameText().color = new Color32(0, 0, 0, 0);
                    player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
                }
            }
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConcealed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ConcealCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();
                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                CabalWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                SectWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                UndeadWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                ReanimatedWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.SyndicateWins() && NotDefective)
            {
                SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}