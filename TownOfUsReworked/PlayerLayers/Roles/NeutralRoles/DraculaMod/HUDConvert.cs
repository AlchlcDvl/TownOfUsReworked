using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDConvert
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Dracula))
                return;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (role.BiteButton == null)
                role.BiteButton = CustomButtons.InstantiateButton();

            var notVamp = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.Converted.Contains(player.PlayerId)).ToList();
            role.BiteButton.UpdateButton(role, "BITE", role.ConvertTimer(), CustomGameOptions.BiteCd, AssetManager.Bite, AbilityTypes.Direct, "ActionSecondary", notVamp,
                role.Converted.Count < CustomGameOptions.AliveVampCount);
        }
    }
}