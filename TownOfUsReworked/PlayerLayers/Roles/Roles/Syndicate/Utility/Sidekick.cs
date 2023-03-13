using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Sidekick : SyndicateRole
    {
        public Role FormerRole = null;
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
            var sidekick = Role.GetRole<Sidekick>(Player);
            var formerRole = sidekick.FormerRole;
            var role = new Rebel(Player);
            role.WasSidekick = true;
            role.HasDeclared = !CustomGameOptions.PromotedSidekickCanPromote;
            role.FormerRole = formerRole;
            role.RoleUpdate(formerRole);
            Player.RegenTask();

            if (Player == PlayerControl.LocalPlayer)
                Coroutines.Start(Utils.FlashCoroutine(Color));

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}