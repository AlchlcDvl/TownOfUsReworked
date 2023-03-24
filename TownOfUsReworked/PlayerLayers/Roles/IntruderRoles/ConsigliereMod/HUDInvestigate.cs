using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDInvestigate
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consigliere))
                return;

            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (role.InvestigateButton == null)
                role.InvestigateButton = Utils.InstantiateButton();

            var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Investigated.Contains(x.PlayerId) && !(x.Is(Faction.Intruder) &&
                CustomGameOptions.FactionSeeRoles)).ToList();
            role.InvestigateButton.UpdateButton(role, "INVESTIGATE", role.ConsigliereTimer(), CustomGameOptions.ConsigCd, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary",
                notInvestigated);
        }
    }
}