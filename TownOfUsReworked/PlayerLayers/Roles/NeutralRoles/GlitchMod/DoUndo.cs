using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DoUndo
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;

                if (glitch.IsUsingMimic)
                    glitch.Mimic();
                else if (glitch.MimicEnabled)
                    glitch.UnMimic();

                if (glitch.IsUsingHack)
                    glitch.Hack();
                else if (glitch.HackEnabled)
                    glitch.UnHack();
            }
        }
    }
}