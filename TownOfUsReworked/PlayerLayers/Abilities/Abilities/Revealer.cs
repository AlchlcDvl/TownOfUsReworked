using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Revealer : Ability
    {
        public bool Caught;
        public bool Revealed;
        public bool CompletedTasks;
        public bool Faded;
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
        public List<PlayerControl> RevealerTargets = new List<PlayerControl>();
        public List<ArrowBehaviour> RevealerArrows = new List<ArrowBehaviour>();

        public Revealer(PlayerControl player) : base(player)
        {
            Name = "Revealer";
            TaskText = () => "Complete all your tasks to reveal impostors!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Revealer : Colors.Ability;
            AbilityType = AbilityEnum.Revealer;
            AddToAbilityHistory(AbilityType);
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