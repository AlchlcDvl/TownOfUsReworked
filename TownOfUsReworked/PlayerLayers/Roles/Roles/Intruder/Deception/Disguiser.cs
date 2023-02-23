using System;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Modifiers;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Disguiser : Role, IVisualAlteration
    {
        private KillButton _disguiseButton;
        private KillButton _killButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastDisguised;
        public PlayerControl MeasuredPlayer;
        public float TimeBeforeDisguised { get; set; }
        public float DisguiseTimeRemaining { get; set; }
        public float TimeRemaining;
        public bool Disguised => TimeRemaining > 0f;
        public DateTime LastKilled { get; set; }
        public bool Enabled = false;
        public Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;

        public Disguiser(PlayerControl player) : base(player)
        {
            Name = "Disguiser";
            StartText = "Disguise The <color=#8BFDFDFF>Crew</color> To Frame Them";
            AbilitiesText = "- You can disguise a player into someone else's appearance.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Disguiser : Colors.Intruder;
            RoleType = RoleEnum.Disguiser;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
            Objectives = IntrudersWinCon;
            RoleDescription = "You are a Disguiser! Cause some chaos by changing people's appearances and fooling everyone around you!";
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.IntKillCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
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

        public KillButton DisguiseButton
        {
            get => _disguiseButton;
            set
            {
                _disguiseButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Disguise()
        {
            if (ClosestPlayer == null || MeasuredPlayer == null)
                return;

            TimeRemaining -= Time.deltaTime;
            Utils.Morph(MeasuredPlayer, ClosestPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void UnDisguise()
        {
            Utils.DefaultOutfit(MeasuredPlayer);
            MeasuredPlayer = null;
            LastDisguised = DateTime.UtcNow;
            DisguiseButton.graphic.sprite = MeasureSprite;
        }

        public float DisguiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDisguised;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.DisguiseCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Disguised)
            {
                appearance = MeasuredPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MeasuredPlayer);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
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

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else
                IntruderWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.IntrudersWin() && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}
