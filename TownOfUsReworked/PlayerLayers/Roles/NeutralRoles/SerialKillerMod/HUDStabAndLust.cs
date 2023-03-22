using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDStabAndLust
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.SerialKiller))
                return;

            var role = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            if (role.BloodlustButton == null)
                role.BloodlustButton = Utils.InstantiateButton();

            if (role.StabButton == null)
                role.StabButton = Utils.InstantiateButton();

            role.BloodlustButton.UpdateButton(role, "BLOODLUST", role.LustTimer(), CustomGameOptions.BloodlustCd, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", role.Lusted,
                role.TimeRemaining, CustomGameOptions.BloodlustDuration, true, !role.Lusted);
            role.StabButton.UpdateButton(role, "STAB", role.KillTimer(), CustomGameOptions.LustKillCd, AssetManager.Stab, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}
