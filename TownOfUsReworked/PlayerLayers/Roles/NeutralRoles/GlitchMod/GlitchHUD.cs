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

            if (role.SampleButton == null)
                role.SampleButton = CustomButtons.InstantiateButton();

            role.HackButton.UpdateButton(role, "HACK", role.HackTimer(), CustomGameOptions.HackCooldown, AssetManager.Hack, AbilityTypes.Direct, "Tertiary", role.IsUsingHack,
                role.TimeRemaining, CustomGameOptions.HackDuration, true, !role.IsUsingHack);
            role.MimicButton.UpdateButton(role, "MIMIC", 0, 1, AssetManager.Mimic, AbilityTypes.Effect, "Secondary", role.IsUsingMimic, role.TimeRemaining2, CustomGameOptions.MimicDuration,
                role.MimicTarget != null, !role.IsUsingMimic);
            role.SampleButton.UpdateButton(role, "SAMPLE", role.MimicTimer(), CustomGameOptions.MimicCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary",
                !role.IsUsingMimic && role.MimicTarget == null);
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