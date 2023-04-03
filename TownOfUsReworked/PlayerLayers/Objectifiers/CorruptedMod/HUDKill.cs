using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted))
                return;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
                role.KillButton = CustomButtons.InstantiateButton();

            role.KillButton.UpdateButton(role, role.KillTimer(), CustomGameOptions.CorruptedKillCooldown, "KILL", AssetManager.CorruptedKill, AbilityTypes.Direct, "Quarternary");
        }
    }
}