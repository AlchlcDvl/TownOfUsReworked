using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class TimeMaster : Role
    {
        private KillButton _freezeButton;
        private KillButton _killButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastFrozen { get; set; }
        public bool Frozen => TimeRemaining > 0f;

        public TimeMaster(PlayerControl player) : base(player)
        {
            Name = "Time Master";
            StartText = "Freeze Time To Stop The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = "Freeze time to stop the <color=#8BFDFDFF>Crew</color> from moving";
            Color = CustomGameOptions.CustomIntColors ? Colors.TimeMaster : Colors.Intruder;
            LastFrozen = DateTime.UtcNow;
            RoleType = RoleEnum.TimeMaster;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            Results = InspResults.TLAltTMCann;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public KillButton FreezeButton
        {
            get => _freezeButton;
            set
            {
                _freezeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFrozen;
            var num = CustomGameOptions.FreezeDuration * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Freeze.PlayerPhysics_FixedUpdate.FreezeAll();
        }

        public void Unfreeze()
        {
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
            Freeze.PlayerPhysics_FixedUpdate.UnfreezeAll();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
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
            else
                IntruderWin = true;
        }

        internal override bool GameEnd(ShipStatus __instance)
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
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}