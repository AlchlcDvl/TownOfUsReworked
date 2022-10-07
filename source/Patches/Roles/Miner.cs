using System;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Miner : Role
    {
        public readonly List<Vent> Vents = new List<Vent>();

        public KillButton _mineButton;
        public DateTime LastMined;


        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            ImpostorText = () => "From The Top, Make It Drop, Boom, That's A Vent";
            TaskText = () => "Place vents around the map";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Miner;
            else Color = Patches.Colors.Impostor;
            LastMined = DateTime.UtcNow;
            RoleType = RoleEnum.Miner;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }

        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public KillButton MineButton
        {
            get => _mineButton;
            set
            {
                _mineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = CustomGameOptions.MineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}