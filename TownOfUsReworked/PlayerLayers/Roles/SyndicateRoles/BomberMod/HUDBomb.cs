using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBomb
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber))
                return;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (role.BombButton == null)
                role.BombButton = Utils.InstantiateButton();

            role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.BombCooldown, TownOfUsReworked.PlantSprite, AbilityTypes.Effect);

            if (role.DetonateButton == null)
                role.DetonateButton = Utils.InstantiateButton();

            role.BombButton.UpdateButton(role, "DETONATE", role.DetonateTimer(), CustomGameOptions.DetonateCooldown, TownOfUsReworked.DetonateSprite, AbilityTypes.Effect,
                role.Bombs.Count > 0);
        }
    }
}
