using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Rebel : SyndicateRole
    {
        public PlayerControl ClosestSyndicate;
        public bool HasDeclared;
        public AbilityButton DeclareButton;
        public DateTime LastDeclared;
        public Sidekick Sidekick;

        public Rebel(PlayerControl player) : base(player)
        {
            Name = "Rebel";
            RoleType = RoleEnum.Rebel;
            StartText = "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting an <color=#008000FF>Syndicate</color> turns them " +
                "into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\nand inherits better " +
                $"abilities of their former role\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = SSu;
        }
    }
}