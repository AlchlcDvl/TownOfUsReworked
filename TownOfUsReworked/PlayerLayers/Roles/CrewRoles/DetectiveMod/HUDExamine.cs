using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDExamine
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Detective))
                return;

            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            if (role.ExamineButton == null)
                role.ExamineButton = Utils.InstantiateButton();

            role.ExamineButton.UpdateButton(role, "EXAMINE", role.ExamineTimer(), CustomGameOptions.ExamineCd, AssetManager.Examine, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}