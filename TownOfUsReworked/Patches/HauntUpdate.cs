using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
    public static class AbilityButtonUpdatePatch
    {
        public static void Postfix()
        {
            if (!ConstantVariables.IsInGame)
                HudManager.Instance.AbilityButton.gameObject.SetActive(false);
            else if (ConstantVariables.IsHnS)
                HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
            else
            {
                var ghostRole = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                {
                    var revealer = Role.GetRole<Revealer>(PlayerControl.LocalPlayer);
                    ghostRole = !revealer.Caught;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    ghostRole = !phantom.Caught;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Banshee))
                {
                    var banshee = Role.GetRole<Banshee>(PlayerControl.LocalPlayer);
                    ghostRole = !banshee.Caught;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul))
                {
                    var ghoul = Role.GetRole<Ghoul>(PlayerControl.LocalPlayer);
                    ghostRole = !ghoul.Caught;
                }

                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead);
            }
        }
    }
}