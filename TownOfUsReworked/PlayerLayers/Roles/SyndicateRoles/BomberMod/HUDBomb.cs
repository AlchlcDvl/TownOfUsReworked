using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBomb
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber))
                return;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (role.BombButton == null)
                role.BombButton = Utils.InstantiateButton();

            role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.BombCooldown, AssetManager.Plant, AbilityTypes.Effect, "Secondary");

            if (role.DetonateButton == null)
                role.DetonateButton = Utils.InstantiateButton();

            role.DetonateButton.UpdateButton(role, "DETONATE", role.DetonateTimer(), CustomGameOptions.DetonateCooldown, AssetManager.Detonate, AbilityTypes.Effect, "Tertiary",
                role.Bombs.Count > 0);
        }
    }
}
