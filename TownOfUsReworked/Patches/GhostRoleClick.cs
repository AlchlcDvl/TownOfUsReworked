using HarmonyLib;
using Hazel;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    public class ClickGhostRole
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (MeetingHud.Instance)
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (__instance.Is(RoleEnum.Phantom))
            {
                if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
                {
                    var role = Role.GetRole<Phantom>(__instance);
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchPhantom, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Revealer))
            {
                if (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.ImpsOnly && !PlayerControl.LocalPlayer.Is(Faction.Intruder))
                    return;

                if (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.NonCrew && !(PlayerControl.LocalPlayer.Is(Faction.Intruder) ||
                    PlayerControl.LocalPlayer.Is(Faction.Neutral)))
                    return;

                if (tasksLeft <= CustomGameOptions.RevealerTasksRemainingClicked)
                {
                    var role = Role.GetRole<Revealer>(__instance);
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchRevealer, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return;
        }
    }
}