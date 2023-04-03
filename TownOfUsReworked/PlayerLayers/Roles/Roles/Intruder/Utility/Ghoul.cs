using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ghoul : IntruderRole
    {
        public AbilityButton MarkButton;
        public bool Caught;
        public bool Faded;
        public DateTime LastMarked;
        public PlayerControl MarkedPlayer;
        public PlayerControl ClosestMark;

        public Ghoul(PlayerControl player) : base(player)
        {
            Name = "Ghoul";
            Type = RoleEnum.Ghoul;
            StartText = "BOO!";
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = IU;
            InspectorResults = InspectorResults.Ghostly;
            Color = CustomGameOptions.CustomIntColors ? Colors.Ghoul : Colors.Intruder;
            MarkedPlayer = null;
        }

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMarked;
            var num = CustomGameOptions.GhoulMarkCd * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Fade()
        {
            Faded = true;
            Player.Visible = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            {
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new GameData.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = ""
                });
            }

            Player.MyRend().color = color;
            Player.NameText().color = new Color(0f, 0f, 0f, 0f);
            Player.cosmetics.colorBlindText.color = new Color(0f, 0f, 0f, 0f);
        }
    }
}