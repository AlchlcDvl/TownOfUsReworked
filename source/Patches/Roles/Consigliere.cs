using System;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Consigliere : Role
    {
        public List<byte> Investigated = new List<byte>();
        public KillButton _revealButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

        public Consigliere(PlayerControl player) : base(player)
        {
            Name = "Consigliere";
            ImpostorText = () => "Reveal Everyone's Roles";
            TaskText = () => "Investigate the roles of the <color=#8BFDFD>Crew</color>";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Consigliere;
            else Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Consigliere;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.ConsigCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton RevealButton
        {
            get => _revealButton;
            set
            {
                _revealButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}