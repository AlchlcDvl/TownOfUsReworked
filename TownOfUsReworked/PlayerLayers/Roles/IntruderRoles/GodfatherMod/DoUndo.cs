using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class DoUndo
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Godfather))
            {
                var gf = (Godfather)role;

                if (gf.FormerRole == null || gf.FormerRole?.RoleType == RoleEnum.Impostor || !gf.WasMafioso)
                    continue;

                var formerRole = gf.FormerRole.RoleType;

                if (formerRole == RoleEnum.Grenadier)
                {
                    if (gf.Flashed)
                        gf.Flash();
                    else if (gf.FlashEnabled)
                        gf.UnFlash();
                }
                else if (formerRole == RoleEnum.Disguiser)
                {
                    if (gf.DelayActive)
                        gf.Delay();
                    else if (gf.Disguised)
                        gf.Disguise();
                    else if (gf.DisguiserEnabled)
                        gf.UnDisguise();
                }
                else if (formerRole == RoleEnum.Morphling)
                {
                    if (gf.Morphed)
                        gf.Morph();
                    else if (gf.MorphEnabled)
                        gf.Unmorph();
                }
                else if (formerRole == RoleEnum.TimeMaster)
                {
                    if (gf.Frozen)
                        gf.TimeFreeze();
                    else if (gf.FreezeEnabled)
                        gf.Unfreeze();
                }
                else if (formerRole == RoleEnum.Wraith)
                {
                    if (gf.IsInvis)
                        gf.Invis();
                    else if (gf.InvisEnabled)
                        gf.Uninvis();
                }
            }
        }
    }
}