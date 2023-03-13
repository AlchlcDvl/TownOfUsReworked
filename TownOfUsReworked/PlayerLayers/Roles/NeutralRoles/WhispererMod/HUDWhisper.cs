using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDWhisper
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            if (role.WhisperButton == null)
                role.WhisperButton = Utils.InstantiateButton();

            role.WhisperButton.UpdateButton(role, "WHISPER", role.WhisperTimer(), CustomGameOptions.WhisperCooldown + (role.WhisperCount * CustomGameOptions.WhisperCooldownIncrease), 
                TownOfUsReworked.WhisperSprite, AbilityTypes.Effect);
        }
    }
}