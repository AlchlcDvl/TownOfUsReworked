using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.RevealerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Revealer))
            {
                var haunter = (Revealer)role;

                if (PlayerControl.LocalPlayer.Data.IsDead || haunter.Caught)
                {
                    haunter.RevealerArrows.DestroyAll();
                    haunter.RevealerArrows.Clear();
                    haunter.ImpArrows.DestroyAll();
                    haunter.ImpArrows.Clear();
                }

                foreach (var arrow in haunter.ImpArrows)
                    arrow.target = haunter.Player.transform.position;

                foreach (var (arrow, target) in Utils.Zip(haunter.RevealerArrows, haunter.RevealerTargets))
                {
                    if (target.Data.IsDead)
                    {
                        arrow.Destroy();
                        if (arrow.gameObject != null) arrow.gameObject.Destroy();
                    }

                    arrow.target = target.transform.position;
                }
            }
        }
    }
}