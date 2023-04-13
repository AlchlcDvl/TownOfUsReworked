using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Sidekick : SyndicateRole
    {
        public Role FormerRole;
        public Rebel Rebel;
        public bool CanPromote => Rebel.Player.Data.IsDead || Rebel.Player.Data.Disconnected;

        public Sidekick(PlayerControl player) : base(player)
        {
            Name = "Sidekick";
            RoleType = RoleEnum.Sidekick;
            StartText = "Succeed The <color=#FFFCCEFF>Rebel</color>";
            AbilitiesText = "- When the <color=#FFFCCEFF>Rebel</color> dies, you will become the new <color=#FFFCCEFF>Rebel</color> with boosted abilities of your former role.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Sidekick : Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = SU;
        }

        public void TurnRebel()
        {
            var role = new PromotedRebel(Player) { FormerRole = FormerRole };
            role.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Rebel, "You have been promoted to <color=#FFFCCEFF>Rebel</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}