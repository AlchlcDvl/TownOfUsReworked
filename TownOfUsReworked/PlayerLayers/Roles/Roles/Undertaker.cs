using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Undertaker : Role
    {
        public KillButton _dragDropButton;
        public DateTime LastDragged { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public Undertaker(PlayerControl player) : base(player)
        {
            Name = "Undertaker";
            ImpostorText = () => "Drag Bodies And Hide Them";
            TaskText = () => "Drag bodies around to hide them from being reported";
            Color = CustomGameOptions.CustomImpColors? Colors.Undertaker : Colors.Intruder;
            SubFaction = SubFaction.None;
            LastDragged = DateTime.UtcNow;
            RoleType = RoleEnum.Undertaker;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = () => "Intruder (Concealing)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.CoroJaniUTMed;
            AddToRoleHistory(RoleType);
        }

        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }
    }
}