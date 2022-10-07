using System.Collections.Generic;
using System.Linq;
using GameObject = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Mystic : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();
        public List<byte> Reported = new List<byte>();

        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            ImpostorText = () => "Understand When, Where And How Kills Happen";
            TaskText = () => "You get information from the dead";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Mystic;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Mystic;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);

            LightDarkColors.Add(0, "darker"); //Red
            LightDarkColors.Add(1, "darker"); //Blue
            LightDarkColors.Add(2, "darker"); //Green
            LightDarkColors.Add(3, "lighter"); //Pink
            LightDarkColors.Add(4, "lighter"); //Orange
            LightDarkColors.Add(5, "lighter"); //Yellow
            LightDarkColors.Add(6, "darker"); //Black
            LightDarkColors.Add(7, "lighter"); //White
            LightDarkColors.Add(8, "darker"); //Purple
            LightDarkColors.Add(9, "darker"); //Brown
            LightDarkColors.Add(10, "lighter"); //Cyan
            LightDarkColors.Add(11, "lighter"); //Lime
            LightDarkColors.Add(12, "darker"); //Maroon
            LightDarkColors.Add(13, "lighter"); //Rose
            LightDarkColors.Add(14, "lighter"); //Banana
            LightDarkColors.Add(15, "darker"); //Grey
            LightDarkColors.Add(16, "darker"); //Tan
            LightDarkColors.Add(17, "lighter"); //Coral
            LightDarkColors.Add(18, "darker"); //Watermelon
            LightDarkColors.Add(19, "darker"); //Chocolate
            LightDarkColors.Add(20, "lighter"); //Sky Blue
            LightDarkColors.Add(21, "darker"); //Biege
            LightDarkColors.Add(22, "lighter"); //Hot Pink
            LightDarkColors.Add(23, "lighter"); //Turquoise
            LightDarkColors.Add(24, "lighter"); //Lilac
            LightDarkColors.Add(25, "darker"); //Olive
            LightDarkColors.Add(26, "lighter"); //Azure
            LightDarkColors.Add(27, "lighter"); //Tomato
            LightDarkColors.Add(28, "darker"); //backrooms
            LightDarkColors.Add(29, "lighter"); //Gold
            LightDarkColors.Add(30, "darker"); //Space
            LightDarkColors.Add(31, "lighter"); //Ice
            LightDarkColors.Add(32, "lighter"); //Mint
            LightDarkColors.Add(33, "darker"); //BTS
            LightDarkColors.Add(34, "darker"); //Forest Green
            LightDarkColors.Add(35, "lighter"); //Donation
            LightDarkColors.Add(36, "darker"); //Cherry
            LightDarkColors.Add(37, "lighter"); //Toy
            LightDarkColors.Add(38, "lighter"); //Pizzaria
            LightDarkColors.Add(39, "lighter"); //Starlight
            LightDarkColors.Add(40, "lighter"); //Softball
            LightDarkColors.Add(41, "darker"); //Dark Jester
            LightDarkColors.Add(42, "darker"); //FRESH
            LightDarkColors.Add(43, "darker"); //Goner
            LightDarkColors.Add(44, "lighter"); //Psychic Friend
            LightDarkColors.Add(45, "lighter"); //Frost
            LightDarkColors.Add(46, "darker"); //Abyss Green
            LightDarkColors.Add(47, "darker"); //Midnight
            LightDarkColors.Add(48, "darker"); //<3
            LightDarkColors.Add(49, "lighter"); //Heat From Fire
            LightDarkColors.Add(50, "lighter"); //Fire From Heat
            LightDarkColors.Add(51, "lighter"); //Determination
            LightDarkColors.Add(52, "lighter"); //Patience
            LightDarkColors.Add(53, "darker"); //Bravery
            LightDarkColors.Add(54, "darker"); //Integrity
            LightDarkColors.Add(55, "darker"); //Perserverance
            LightDarkColors.Add(56, "darker"); //Kindness
            LightDarkColors.Add(57, "lighter"); //Bravery
            LightDarkColors.Add(58, "darker"); //Purple Plumber
            LightDarkColors.Add(59, "lighter"); //Rainbow
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                GameObject.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                GameObject.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
    }
}