using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDRecruit
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jackal))
                return;

            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            if (role.HasRecruited || !role.RecruitsDead)
                return;

            if (role.RecruitButton == null)
                role.RecruitButton = CustomButtons.InstantiateButton();

            var notRecruited = PlayerControl.AllPlayerControls.ToArray().Where(player => player != role.GoodRecruit && player != role.EvilRecruit && player != role.BackupRecruit).ToList();
            role.RecruitButton.UpdateButton(role, "RECRUIT", role.RecruitTimer(), CustomGameOptions.RecruitCooldown, AssetManager.Recruit, AbilityTypes.Direct, "ActionSecondary",
                notRecruited, role.RecruitsDead && !role.HasRecruited);
        }
    }
}