using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDHaunt
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jester))
                return;

            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            if (!role.VotedOut)
                return;

            if (role.HauntButton == null)
                role.HauntButton = Utils.InstantiateButton();

            var ToBeHaunted = PlayerControl.AllPlayerControls.ToArray().Where(x => role.ToHaunt.Contains(x.PlayerId)).ToList();
            role.HauntButton.UpdateButton(role, "HAUNT", role.HauntTimer(), CustomGameOptions.HauntCooldown, AssetManager.Haunt, AbilityTypes.Direct, "ActionSecondary", ToBeHaunted,
                !role.HasHaunted && role.CanHaunt, role.CanHaunt, false, 0, 1, true, role.MaxUses, true);
        }
    }
}