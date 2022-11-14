using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Miner : Role
    {
        public readonly System.Collections.Generic.List<Vent> Vents = new System.Collections.Generic.List<Vent>();
        public KillButton _mineButton;
        public DateTime LastMined;
        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            ImpostorText = () => "From The Top, Make It Drop, Boom, That's A Vent";
            TaskText = () => "Place vents around the map";
            Color = CustomGameOptions.CustomImpColors ? Colors.Miner : Colors.Intruder;
            SubFaction = SubFaction.None;
            LastMined = DateTime.UtcNow;
            RoleType = RoleEnum.Miner;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = () => "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.MineMafiSideDamp;
            AddToRoleHistory(RoleType);
        }

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

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }
    }
}