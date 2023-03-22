using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlackmail
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Blackmailer))
                return;

            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            if (role.BlackmailButton == null)
                role.BlackmailButton = Utils.InstantiateButton();

            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.BlackmailedPlayer != player).ToList();
            role.BlackmailButton.UpdateButton(role, role.Blackmailed ? "BLACKMAILED" : "BLACKMAIL", role.BlackmailTimer(), CustomGameOptions.BlackmailCd, role.Blackmailed ?
                AssetManager.BlackmailLetter : AssetManager.Blackmail, AbilityTypes.Direct, "Secondary", notBlackmailed);
        }
    }
}