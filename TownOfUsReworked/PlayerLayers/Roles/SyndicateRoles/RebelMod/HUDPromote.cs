using HarmonyLib;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PromotedRebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDPromote
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (role.DeclareButton == null)
                role.DeclareButton = CustomButtons.InstantiateButton();

            var Syn = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate)).ToList();
            role.DeclareButton.UpdateButton(role, "SIDEKICK", 0, 1, AssetManager.Sidekick, AbilityTypes.Direct, "Secondary", Syn, !role.HasDeclared);
        }
    }
}