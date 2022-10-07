using System;
using UnityEngine;
using TMPro;

namespace TownOfUs.Roles
{
    public class GuardianAngel : Role
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public PlayerControl target;

        public bool GAWins { get; set; }

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            ImpostorText = () =>
                target == null 
                    ? "You don't have a target for some reason... weird..."
                    : $"Protect {target.name} With Your Life!";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Protect {target.name}!";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.GuardianAngel;
            else Color = Patches.Colors.Neutral;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            Faction = Faction.Neutral;
            Scale = 1.4f;
            FactionName = "Neutral";
            UsesLeft = CustomGameOptions.MaxProtects;
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralBen;
            AlignmentName = "Neutral (Benign)";
            AddToRoleHistory(RoleType);
        }

        public bool Protecting => TimeRemaining > 0f;

        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastProtected;
            var num = CustomGameOptions.ProtectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnProtect()
        {
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }

        public void Wins()
        {
            if (!target.Data.IsDead)
                GAWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var gaTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            gaTeam.Add(PlayerControl.LocalPlayer);
            gaTeam.Add(target);
            __instance.teamToShow = gaTeam;
        }
    }
}