using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, Faction.Intruder))
                return;

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Allied) || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor) || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Fanatic))
                return;

            var role = Role.GetRole<IntruderRole>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
                role.KillButton = Utils.InstantiateButton();

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.KillButton.UpdateButton(role, "KILL", role.KillTimer(), CustomGameOptions.IntKillCooldown, TownOfUsReworked.IntruderKill, AbilityTypes.Direct, notImp);
        }
    }
}