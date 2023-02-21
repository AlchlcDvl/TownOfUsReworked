using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class DoUndo
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;

                if (glitch.IsUsingMimic)
                    Utils.Morph(glitch.Player, glitch.MimicTarget);
                else if (glitch.MimicTarget)
                    Utils.DefaultOutfit(glitch.Player);
            }
        }
    }
}