using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Snitch))
            {
                var snitch = (Snitch)role;

                if (PlayerControl.LocalPlayer.Data.IsDead | snitch.Player.Data.IsDead)
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

                    if (player == null | player.Data == null | player.Data.IsDead | player.Data.Disconnected)
                    {
                        snitch.DestroyArrow(arrow.Key);
                        continue;
                    }
                    
                    arrow.Value.target = player.transform.position;
                }
            }
        }
    }
}