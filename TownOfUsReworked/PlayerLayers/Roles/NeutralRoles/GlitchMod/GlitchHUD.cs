using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class GlitchHUD
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
                role.KillButton = CustomButtons.InstantiateButton();

            if (role.HackButton == null)
                role.HackButton = CustomButtons.InstantiateButton();

            if (role.MimicButton == null)
                role.MimicButton = CustomButtons.InstantiateButton();

            role.HackButton.UpdateButton(role, "HACK", role.HackTimer(), CustomGameOptions.HackCooldown, AssetManager.Hack, AbilityTypes.Direct, "Tertiary", role.IsUsingHack,
                role.TimeRemaining, CustomGameOptions.HackDuration, true, !role.IsUsingHack);
            role.MimicButton.UpdateButton(role, role.MimicTarget == null ? "SAMPLE" : "MIMIC", role.MimicTimer(), CustomGameOptions.MimicCooldown, AssetManager.Mimic, AbilityTypes.Effect,
                "Secondary", role.IsUsingMimic, role.TimeRemaining2, CustomGameOptions.MimicDuration, true, !role.IsUsingMimic);
            role.KillButton.UpdateButton(role, "NEUTRALISE", role.KillTimer(), CustomGameOptions.GlitchKillCooldown, AssetManager.Neutralise, AbilityTypes.Direct, "ActionSecondary");

            if (Input.GetKeyDown(KeyCode.Backspace) && !role.IsUsingMimic)
            {
                if (role.MimicTarget != null)
                    role.MimicTarget = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}