using TownOfUsReworked.Enums;
using System.Collections.Generic;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using System;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Gorgon : Role
    {
        public KillButton _gazeButton;
        public Dictionary<byte, float> gazeList = new Dictionary<byte, float>();
        public PlayerControl ClosestPlayer;
        public DateTime LastGazed;
        public bool Enabled = false;
        public float TimeRemaining;
        public PlayerControl StonedPlayer;
        public bool Stoned => TimeRemaining > 0f;
        public bool SyndicateWin;

        public Gorgon(PlayerControl player) : base(player)
        {
            Name = "Gorgon";
            ImpostorText = () => "Freeze the crewmates";
            TaskText = () => "Freeze a crewmate to stick them in place and kill them";
            Color = Colors.Gorgon;
            RoleType = RoleEnum.Gorgon;
            Faction = Faction.Syndicate;
        }
        
        public KillButton GazeButton
        {
            get => _gazeButton;
            set
            {
                _gazeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastGazed;
            var num = CustomGameOptions.PoisonCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public void Freeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0;
                
            if (TimeRemaining <= 0)
                FreezeKill();
        }

        public void FreezeKill()
        {
            if (!StonedPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(Player, StonedPlayer);

                if (!StonedPlayer.Data.IsDead)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.5f);
            }

            StonedPlayer = null;
            Enabled = false;
            LastGazed = DateTime.UtcNow;
        }

        public void Wins()
        {
            SyndicateWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | x.Is(Faction.Crew) | x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) |
                x.Is(RoleAlignment.NeutralPros))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}