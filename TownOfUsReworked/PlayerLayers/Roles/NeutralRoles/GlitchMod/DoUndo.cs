using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DoUndo
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var glitch in Role.GetRoles<Glitch>(RoleEnum.Glitch))
            {
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