using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDMorph
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Morphling))
                return;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (role.MorphButton == null)
                role.MorphButton = Utils.InstantiateButton();

            role.MorphButton.UpdateButton(role, "MORPH", role.MorphTimer(), CustomGameOptions.MorphlingCd, TownOfUsReworked.MorphSprite, AbilityTypes.Effect, role.Morphed,
                role.TimeRemaining, CustomGameOptions.MorphlingDuration, role.SampledPlayer != null, !role.Morphed);

            if (role.SampleButton == null)
                role.SampleButton = Utils.InstantiateButton();

            var notSampled = PlayerControl.AllPlayerControls.ToArray().Where(x => role.SampledPlayer?.PlayerId != x.PlayerId).ToList();
            role.SampleButton.UpdateButton(role, "SAMPLE", role.SampleTimer(), CustomGameOptions.SampleCooldown, TownOfUsReworked.SampleSprite, AbilityTypes.Direct, notSampled);
        }
    }
}
