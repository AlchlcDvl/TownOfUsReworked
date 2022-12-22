using System;
using System.Linq;
using Hazel;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class SerialKiller : Role
    {
        private KillButton _bloodlustButton;
        public bool Enabled;
        public bool SerialKillerWins;
        public PlayerControl ClosestPlayer;
        public DateTime LastLusted;
        public DateTime LastKilled;
        public float TimeRemaining;

        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = "You Like To Play With Knives";
            AbilitiesText = "Engage in your bloodlust to kill everyone\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.SerialKiller : Colors.Neutral;
            LastLusted = DateTime.UtcNow;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.SerialKiller;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            IntroText = "Stab those who oppose you";
            Results = InspResults.ShiftSwapSKDrac;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
            AddToRoleHistory(RoleType);
        }

        public KillButton BloodlustButton
        {
            get => _bloodlustButton;
            set
            {
                _bloodlustButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | (x.Is(RoleAlignment.NeutralKill) && !x.Is(RoleEnum.SerialKiller)) | x.Is(RoleAlignment.NeutralNeo) |
                x.Is(RoleAlignment.NeutralPros) | x.Is(Faction.Crew))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SerialKillerWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }


        public override void Wins()
        {
            SerialKillerWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var skTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            skTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = skTeam;
        }

        public bool Lusted => TimeRemaining > 0f;

        public float LustTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastLusted;
            var num = CustomGameOptions.BloodlustCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Bloodlust()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Unbloodlust()
        {
            Enabled = false;
            LastLusted = DateTime.UtcNow;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.LustKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
