using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBlackmail
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Blackmailer))
                return;

            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            if (role.BlackmailButton == null)
                role.BlackmailButton = CustomButtons.InstantiateButton();

            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.BlackmailedPlayer != player).ToList();
            role.BlackmailButton.UpdateButton(role, role.Blackmailed ? "BLACKMAILED" : "BLACKMAIL", role.BlackmailTimer(), CustomGameOptions.BlackmailCd, role.Blackmailed ?
                AssetManager.BlackmailLetter : AssetManager.Blackmail, AbilityTypes.Direct, "Secondary", notBlackmailed);
        }
    }
}