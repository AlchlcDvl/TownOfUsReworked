namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    public static class ClickGhostRole
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null || Utils.Meeting || CustomPlayer.LocalCustom.IsDead || __instance == CustomPlayer.Local || LobbyBehaviour.Instance)
                return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (__instance.Is(RoleEnum.Phantom))
            {
                if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
                {
                    var role = Role.GetRole<Phantom>(__instance);
                    role.Caught = true;
                    __instance.Exiled();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchPhantom, SendOption.Reliable);
                    writer.Write(role.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Revealer))
            {
                if ((CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !(CustomPlayer.Local.Is(Faction.Intruder) ||
                    CustomPlayer.Local.Is(Faction.Syndicate))) || (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.NonCrew &&
                    CustomPlayer.Local.Is(Faction.Crew)))
                {
                    return;
                }

                if (tasksLeft <= CustomGameOptions.RevealerTasksRemainingClicked)
                {
                    var role = Role.GetRole<Revealer>(__instance);
                    role.Caught = true;
                    role.CompletedTasks = false;
                    __instance.Exiled();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchRevealer, SendOption.Reliable);
                    writer.Write(role.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Banshee))
            {
                if (! CustomPlayer.Local.Is(Faction.Syndicate))
                {
                    var role = Role.GetRole<Banshee>(__instance);
                    role.Caught = true;
                    __instance.Exiled();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchBanshee, SendOption.Reliable);
                    writer.Write(role.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(RoleEnum.Ghoul))
            {
                if (! CustomPlayer.Local.Is(Faction.Intruder))
                {
                    var role = Role.GetRole<Ghoul>(__instance);
                    role.Caught = true;
                    __instance.Exiled();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CatchGhoul, SendOption.Reliable);
                    writer.Write(role.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}