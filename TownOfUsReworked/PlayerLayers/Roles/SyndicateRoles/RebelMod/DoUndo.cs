using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class DoUndo
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Rebel))
            {
                var reb = (Rebel)role;

                if (reb.FormerRole == null || reb.FormerRole?.Type == RoleEnum.Anarchist || !reb.WasSidekick)
                    continue;

                var formerRole = reb.FormerRole.Type;

                if (formerRole == RoleEnum.Concealer)
                {
                    if (reb.Concealed)
                        reb.Conceal();
                    else if (reb.ConcealEnabled)
                        reb.UnConceal();
                }
                else if (formerRole == RoleEnum.Poisoner)
                {
                    if (reb.Poisoned)
                        reb.Poison();
                    else if (reb.PoisonEnabled)
                        reb.PoisonKill();
                }
                else if (formerRole == RoleEnum.Drunkard)
                {
                    if (reb.Confused)
                        reb.Confuse();
                    else if (reb.ConfuseEnabled)
                        reb.Unconfuse();
                }
                else if (formerRole == RoleEnum.Shapeshifter)
                {
                    if (reb.Shapeshifted)
                        reb.Shapeshift();
                    else if (reb.ShapeshiftEnabled)
                        reb.UnShapeshift();
                }
            }
        }
    }
}