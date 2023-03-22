using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mafioso : IntruderRole
    {
        public Role FormerRole = null;
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
            var mafioso = Role.GetRole<Mafioso>(Player);
            var formerRole = mafioso.FormerRole;
            var role = new Godfather(Player);
            role.WasMafioso = true;
            role.HasDeclared = !CustomGameOptions.PromotedMafiosoCanPromote;
            role.FormerRole = formerRole;
            role.RoleUpdate(formerRole);
            role.RoleBlockImmune = formerRole.RoleBlockImmune;
            Player.RegenTask();

            if (Player == PlayerControl.LocalPlayer)
                Coroutines.Start(Utils.FlashCoroutine(Color));

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}