using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UpdateArrows
    {
        public static void Postfix()
        {
            foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Snitch))
            {
                var snitch = (Snitch)role;

                if (PlayerControl.LocalPlayer.Data.IsDead || snitch.Player.Data.IsDead)
                {
                    snitch.SnitchArrows.Values.DestroyAll();
                    snitch.SnitchArrows.Clear();
                    snitch.ImpArrows.DestroyAll();
                    snitch.ImpArrows.Clear();
                }

                foreach (var arrow in snitch.ImpArrows)
                    arrow.target = snitch.Player.transform.position;

                foreach (var arrow in snitch.SnitchArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);

                    if (player == null || player.Data?.IsDead != false || player.Data.Disconnected)
                        snitch.DestroyArrow(arrow.Key);
                    else
                        arrow.Value.target = player.transform.position;
                }
            }
        }
    }
}