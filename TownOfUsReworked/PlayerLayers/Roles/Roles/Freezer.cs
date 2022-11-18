using TownOfUsReworked.Enums;
using System.Collections.Generic;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using System;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Freezer : Role
    {
        public KillButton _freezeButton;
        public Dictionary<byte, float> freezeList = new Dictionary<byte, float>();
        public PlayerControl ClosestPlayer;
        public DateTime LastFrozen;
        public bool Enabled = false;
        public float TimeRemaining;
        public PlayerControl FrozenPlayer;
        public bool Frozen => TimeRemaining > 0f;

        public Freezer(PlayerControl player) : base(player)
        {
            Name = "Freezer";
            ImpostorText = () => "Freeze the crewmates";
            TaskText = () => "Freeze a crewmate to stick them in place and kill them";
            Color = Colors.Freezer;
            RoleType = RoleEnum.Freezer;
            Faction = Faction.Syndicate;
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
            if (!FrozenPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(Player, FrozenPlayer);

                if (!FrozenPlayer.Data.IsDead)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.5f);
            }

            FrozenPlayer = null;
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
        }
    }
}