using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Godfather : IntruderRole
    {
        public PlayerControl ClosestIntruder;
        public bool HasDeclared;
        public Mafioso Mafioso;
        public AbilityButton DeclareButton;
        public DateTime LastDeclared;

        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF0000FF>Intruder</color> turns them " +
                "into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>Godfather</color>\nand inherits better " +
                $"abilities of their former role\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
        }
    }
}