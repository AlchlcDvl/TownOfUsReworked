using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mafioso : IntruderRole
    {
        public Role FormerRole;
        public Godfather Godfather;
        public bool CanPromote => Godfather.Player.Data.IsDead || Godfather.Player.Data.Disconnected;

        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            RoleType = RoleEnum.Mafioso;
            StartText = "Succeed The <color=#404C08FF>Godfather</color>";
            AbilitiesText = "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities of your former role.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Mafioso : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = IU;
        }

        public void TurnGodfather()
        {
            var role = new Godfather(Player)
            {
                WasMafioso = true,
                HasDeclared = !CustomGameOptions.PromotedMafiosoCanPromote,
                FormerRole = FormerRole,
                RoleBlockImmune = FormerRole.RoleBlockImmune
            };

            role.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Godfather, "You have been promoted to <color=#404C08FF>Godfather</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}