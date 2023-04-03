using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBeam
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                return;

            var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);

            if (role.BeamButton == null)
                role.BeamButton = CustomButtons.InstantiateButton();

            if (role.SetBeamButton1 == null)
                role.SetBeamButton1 = CustomButtons.InstantiateButton();

            if (role.SetBeamButton2 == null)
                role.SetBeamButton2 = CustomButtons.InstantiateButton();

            var flag1 = role.BeamPlayer1 == null;
            var flag2 = role.BeamPlayer2 == null;
            role.BeamButton.UpdateButton(role, "BEAM", role.BeamTimer(), CustomGameOptions.BeamCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "ActionSecondary", null,
                !(flag1 || flag2), !(flag1 || flag2));
            role.SetBeamButton1.UpdateButton(role, "FIRST TARGET", role.BeamTimer(), CustomGameOptions.BeamCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "ActionSecondary",
                null, flag1, flag1);
            role.SetBeamButton2.UpdateButton(role, "SECOND TARGET", role.BeamTimer(), CustomGameOptions.BeamCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "ActionSecondary",
                null, flag2, flag2);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.BeamPlayer2 != null)
                    role.BeamPlayer2 = null;
                else if (role.BeamPlayer1 != null)
                    role.BeamPlayer1 = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}