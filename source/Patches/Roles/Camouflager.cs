using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Camouflager : Role

    {
        public KillButton _camouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged { get; set; }
        public float TimeRemaining;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            ImpostorText = () => "Throw paint everywhere and blend in with the <color=#8BFDFDFF>Crew</color>";
            TaskText = () => "Camouflage among everyone and kill in front of them";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Camouflager;
            else Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Camouflager;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }

        public bool Camouflaged => TimeRemaining > 0f;

        public KillButton CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.UnCamouflage();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}