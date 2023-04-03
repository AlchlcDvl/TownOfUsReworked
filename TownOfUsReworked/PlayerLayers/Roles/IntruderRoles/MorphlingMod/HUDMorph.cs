using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMorph
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Morphling))
                return;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (role.MorphButton == null)
                role.MorphButton = CustomButtons.InstantiateButton();

            role.MorphButton.UpdateButton(role, "MORPH", role.MorphTimer(), CustomGameOptions.MorphlingCd, AssetManager.Morph, AbilityTypes.Effect, "Secondary", role.Morphed,
                role.TimeRemaining, CustomGameOptions.MorphlingDuration, role.SampledPlayer != null, !role.Morphed);

            if (role.SampleButton == null)
                role.SampleButton = CustomButtons.InstantiateButton();

            var notSampled = PlayerControl.AllPlayerControls.ToArray().Where(x => role.SampledPlayer?.PlayerId != x.PlayerId).ToList();
            role.SampleButton.UpdateButton(role, "SAMPLE", role.SampleTimer(), CustomGameOptions.SampleCooldown, AssetManager.Sample, AbilityTypes.Direct, "Tertiary", notSampled);
        }
    }
}
