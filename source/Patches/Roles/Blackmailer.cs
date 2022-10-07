using System;

namespace TownOfUs.Roles
{
    public class Blackmailer : Role
    {
        public KillButton _blackmailButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public DateTime LastBlackmailed { get; set; }

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            ImpostorText = () => "You know their dirty little secrets";
            TaskText = () => "Use your information to silence players for the next meeting";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Blackmailer;
            else Color = Patches.Colors.Impostor;
            LastBlackmailed = DateTime.UtcNow;
            RoleType = RoleEnum.Blackmailer;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = CustomGameOptions.BlackmailCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}