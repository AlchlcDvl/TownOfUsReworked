using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFramer
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer))
                return;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (role.FrameButton == null)
                role.FrameButton = Utils.InstantiateButton();

            var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
            role.FrameButton.UpdateButton(role, "FRAME", role.FrameTimer(), CustomGameOptions.FrameCooldown, AssetManager.Placeholder, Role.SyndicateHasChaosDrive ? AbilityTypes.Effect
                : AbilityTypes.Direct, "Secondary", notFramed);
        }
    }
}