using HarmonyLib;
using Hazel;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    public static class ClickGhostRole
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null || MeetingHud.Instance || !PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (__instance == PlayerControl.LocalPlayer)
                return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (__instance.Is(RoleEnum.Phantom))
            {
                if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
                {
                    var role = Role.GetRole<Phantom>(__instance);
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchPhantom, SendOption.Reliable);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Revealer))
            {
                if (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !(PlayerControl.LocalPlayer.Is(Faction.Intruder) ||
                    PlayerControl.LocalPlayer.Is(Faction.Syndicate)))
                {
                    return;
                }

                if (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.NonCrew && !(PlayerControl.LocalPlayer.Is(Faction.Intruder)||
                    PlayerControl.LocalPlayer.Is(Faction.Syndicate) || PlayerControl.LocalPlayer.Is(Faction.Neutral)))
                {
                    return;
                }

                if (tasksLeft <= CustomGameOptions.RevealerTasksRemainingClicked)
                {
                    var role = Role.GetRole<Revealer>(__instance);
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchRevealer, SendOption.Reliable);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Banshee))
            {
                if (!PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                {
                    var role = Role.GetRole<Banshee>(__instance);
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchBanshee, SendOption.Reliable);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Ghoul))
            {
                if (!PlayerControl.LocalPlayer.Is(Faction.Intruder))
                {
                    var role = Role.GetRole<Ghoul>(__instance);
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchGhoul, SendOption.Reliable);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}