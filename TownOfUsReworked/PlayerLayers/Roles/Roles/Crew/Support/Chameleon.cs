using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Chameleon : CrewRole
    {
        public bool Enabled;
        public DateTime LastSwooped;
        public float TimeRemaining;
        public bool IsSwooped => TimeRemaining > 0f;
        public AbilityButton SwoopButton;

        public Chameleon(PlayerControl player) : base(player)
        {
            Name = "Chameleon";
            StartText = "Go Invisible To Stalk Players";
            AbilitiesText = "- You can turn invisible.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Chameleon : Colors.Crew;
            LastSwooped = DateTime.UtcNow;
            RoleType = RoleEnum.Chameleon;
            AlignmentName = CS;
            InspectorResults = InspectorResults.Unseen;
        }

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSwooped;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.SwoopCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead)
                TimeRemaining = 0f;

            var color = new Color32(0, 0, 0, 0);

            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer == Player)
                color.a = 26;

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
                Player.NameText().color = new Color32(0, 0, 0, 0);
                Player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public void Uninvis()
        {
            Enabled = false;
            LastSwooped = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.myRend().color = new Color32(255, 255, 255, 255);
        }
    }
}