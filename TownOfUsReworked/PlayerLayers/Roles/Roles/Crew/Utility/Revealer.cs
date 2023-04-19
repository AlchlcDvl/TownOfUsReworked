using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Revealer : CrewRole
    {
        public bool Caught;
        public bool Revealed;
        public bool CompletedTasks;
        public bool Faded;
        public List<ArrowBehaviour> ImpArrows = new();
        public List<PlayerControl> RevealerTargets = new();
        public List<ArrowBehaviour> RevealerArrows = new();
        public Role FormerRole;

        public Revealer(PlayerControl player) : base(player)
        {
            Name = "Revealer";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Revealer : Colors.Crew;
            AbilitiesText = "- You can reveal evils players to the <color=#8BFDFDFF>Crew</color> once you finish your tasks without getting clicked.";
            RoleType = RoleEnum.Revealer;
            RoleAlignment = RoleAlignment.CrewUtil;
            AlignmentName = CU;
            ImpArrows = new();
            RevealerTargets = new();
            RevealerArrows = new();
            InspectorResults = InspectorResults.Ghostly;
            Type = LayerEnum.Revealer;
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
            Player.NameText().color = new(0f, 0f, 0f, 0f);
            Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);
        }
    }
}