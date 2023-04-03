using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class UpdateArrows
    {
        public static void Postfix()
        {
            foreach (var role in Role.AllRoles.Where(x => x.Type == RoleEnum.Revealer))
            {
                var haunter = (Revealer)role;

                if (PlayerControl.LocalPlayer.Data.IsDead || haunter.Caught || LobbyBehaviour.Instance || MeetingHud.Instance)
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
                        arrow?.Destroy();
                        arrow?.gameObject?.Destroy();
                    }

                    arrow.target = target.transform.position;
                }
            }
        }
    }
}