using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Phantom : Role
    {
        public bool Caught;
        public bool CompletedTasks;
        public bool Faded;

        public Phantom(PlayerControl player) : base(player)
        {
            Name = "Phantom";
            ImpostorText = () => "Haha Look At You, You're Dead";
            TaskText = () => "Complete all your tasks without being caught!";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Phantom;
            else Color = Patches.Colors.Neutral;
            RoleType = RoleEnum.Phantom;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.PostmortalNeutral;
            AlignmentName = "Neutral (Postmortal)";
            AddToRoleHistory(RoleType);
        }

        public void Loses()
        {
            LostByRPC = true;
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