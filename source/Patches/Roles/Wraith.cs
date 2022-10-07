using System;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Wraith : Role
    {
        public KillButton _invisButton;
        public bool Enabled;
        public DateTime LastInvis;
        public float TimeRemaining;

        public Wraith(PlayerControl player) : base(player)
        {
            Name = "Wraith";
            ImpostorText = () => "Turn Invisible Temporarily";
            TaskText = () => "Turn invisible and kill undetected";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Wraith;
            else Color = Patches.Colors.Impostor;
            LastInvis = DateTime.UtcNow;
            RoleType = RoleEnum.Wraith;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderConceal;
            AlignmentName = "Intruder (Concealing)";
            AddToRoleHistory(RoleType);
        }

        public bool IsInvis => TimeRemaining > 0f;

        public KillButton InvisButton
        {
            get => _invisButton;
            set
            {
                _invisButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float InvisTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvis;
            ;
            var num = CustomGameOptions.InvisCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
            var color = Color.clear;
            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Data.IsDead) color.a = 0.1f;

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                {
                    ColorId = Player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " "
                });
                Player.myRend().color = color;
                Player.nameText().color = Color.clear;
                Player.cosmetics.colorBlindText.color = Color.clear;
            }

        }

        public void Uninvis()
        {
            Enabled = false;
            LastInvis = DateTime.UtcNow;
            Utils.Unmorph(Player);
            Player.myRend().color = Color.white;
        }
    }
}