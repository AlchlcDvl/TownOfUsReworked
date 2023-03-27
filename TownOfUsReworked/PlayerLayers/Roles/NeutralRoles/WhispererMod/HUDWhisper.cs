using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDWhisper
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            if (role.WhisperButton == null)
                role.WhisperButton = CustomButtons.InstantiateButton();

            role.WhisperButton.UpdateButton(role, "WHISPER", role.WhisperTimer(), CustomGameOptions.WhisperCooldown + (role.WhisperCount * CustomGameOptions.WhisperCooldownIncrease),
                AssetManager.Whisper, AbilityTypes.Effect, "ActionSecondary");
        }
    }
}