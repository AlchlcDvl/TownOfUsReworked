using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDConvert
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Dracula))
                return;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (role.BiteButton == null)
                role.BiteButton = Utils.InstantiateButton();

            var notVamp = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.Converted.Contains(player.PlayerId)).ToList();
            role.BiteButton.UpdateButton(role, "BITE", role.ConvertTimer(), CustomGameOptions.BiteCd, AssetManager.Bite, AbilityTypes.Direct, "ActionSecondary", notVamp,
                role.Converted.Count < CustomGameOptions.AliveVampCount);
        }
    }
}
