using System;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Wraith : IntruderRole
    {
        public AbilityButton InvisButton;
        public bool Enabled;
        public DateTime LastInvis;
        public float TimeRemaining;
        public bool IsInvis => TimeRemaining > 0f;

        public Wraith(PlayerControl player) : base(player)
        {
            Name = "Wraith";
            StartText = "Sneaky Sneaky";
            AbilitiesText = "Turn invisible and kill undetected";
            Color = CustomGameOptions.CustomIntColors ? Colors.Wraith : Colors.Intruder;
            LastInvis = DateTime.UtcNow;
            RoleType = RoleEnum.Wraith;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
        }

        public float InvisTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvis;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InvisCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            
            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;

            var color = new Color32(0, 0, 0, 0);

            if (PlayerControl.LocalPlayer.Is(Faction) || PlayerControl.LocalPlayer.Data.IsDead)
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
            LastInvis = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.myRend().color = new Color32(255, 255, 255, 255);
        }
    }
}