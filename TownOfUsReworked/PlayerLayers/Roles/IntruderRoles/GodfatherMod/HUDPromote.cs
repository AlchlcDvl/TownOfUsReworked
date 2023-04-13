using HarmonyLib;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDEverything
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (role.DeclareButton == null)
                role.DeclareButton = CustomButtons.InstantiateButton();

            var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder) && !x.Is(RoleEnum.Consort)).ToList();
            role.DeclareButton.UpdateButton(role, "PROMOTE", 0, 1, AssetManager.Promote, AbilityTypes.Direct, "Secondary", Imp, !role.HasDeclared);
        }
    }
}