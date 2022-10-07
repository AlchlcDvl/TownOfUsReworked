using UnityEngine;
using System.Collections.Generic;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Haunter : Role
    {
        public bool Caught;
        public bool Revealed;
        public bool CompletedTasks;
        public bool Faded;

        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> HaunterTargets = new List<PlayerControl>();

        public List<ArrowBehaviour> HaunterArrows = new List<ArrowBehaviour>();

        public Haunter(PlayerControl player) : base(player)
        {
            Name = "Haunter";
            ImpostorText = () => "Haha Look At You, You're Dead";
            TaskText = () => "Complete all your tasks to reveal impostors!";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Haunter;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Haunter;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.PostmortalCrew;
            AlignmentName = "Crew (Postmortal)";
            AddToRoleHistory(RoleType);
        }

        public void Fade()
        {
            Faded = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * PlayerControl.GameOptions.CrewLightMod;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.TrueGhostSpeed * 0.13f;
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
            Player.myRend().color = color;
            Player.nameText().color = Color.clear;
            Player.cosmetics.colorBlindText.color = Color.clear;
        }
    }
}