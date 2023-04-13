using System.Linq;
using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDFramer
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer))
                return;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (role.FrameButton == null)
                role.FrameButton = CustomButtons.InstantiateButton();

            var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
            role.FrameButton.UpdateButton(role, "FRAME", role.FrameTimer(), CustomGameOptions.FrameCooldown, AssetManager.Placeholder, role.HoldsDrive ? AbilityTypes.Effect :
                AbilityTypes.Direct, "Secondary", notFramed);
        }
    }
}