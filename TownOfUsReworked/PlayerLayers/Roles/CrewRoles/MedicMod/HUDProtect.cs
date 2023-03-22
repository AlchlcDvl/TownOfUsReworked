using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDProtect
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Medic))
                return;

            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);

            if (role.ShieldButton == null)
                role.ShieldButton = Utils.InstantiateButton();

            var notShielded = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.ShieldedPlayer).ToList();
            role.ShieldButton.UpdateButton(role, "SHIELD", 0, 1, AssetManager.Shield, AbilityTypes.Direct, "ActionSecondary", notShielded, !role.UsedAbility);
        }
    }
}
