using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

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

            role.MimicButton.UpdateButton(role, "MIMIC", role.MimicTimer(), CustomGameOptions.MimicCooldown, AssetManager.Mimic, AbilityTypes.Effect, "Secondary", role.IsUsingMimic,
                role.TimeRemaining2, CustomGameOptions.MimicDuration, true, !role.IsUsingMimic);
            role.HackButton.UpdateButton(role, "HACK", role.HackTimer(), CustomGameOptions.HackCooldown, AssetManager.Hack, AbilityTypes.Direct, "Tertiary", role.IsUsingHack,
                role.TimeRemaining, CustomGameOptions.HackDuration, true, !role.IsUsingHack);
            role.KillButton.UpdateButton(role, "NEUTRALISE", role.KillTimer(), CustomGameOptions.GlitchKillCooldown, AssetManager.Neutralise, AbilityTypes.Direct, "ActionSecondary");
            role.MimicListUpdate();
        }
    }
}