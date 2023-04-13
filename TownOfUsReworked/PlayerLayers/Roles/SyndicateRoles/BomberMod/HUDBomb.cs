using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

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
                role.BombButton = CustomButtons.InstantiateButton();

            if (role.DetonateButton == null)
                role.DetonateButton = CustomButtons.InstantiateButton();

            role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.BombCooldown, AssetManager.Plant, AbilityTypes.Effect, "Secondary");
            role.DetonateButton.UpdateButton(role, "DETONATE", role.DetonateTimer(), CustomGameOptions.DetonateCooldown, AssetManager.Detonate, AbilityTypes.Effect, "Tertiary",
                role.Bombs.Count > 0);
        }
    }
}
