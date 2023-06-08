namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class CompleteTasksPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.Is(AbilityEnum.Snitch) && !__instance.Data.IsDead)
            {
                var role = Ability.GetAbility<Snitch>(__instance);

                if (role.TasksLeft == CustomGameOptions.SnitchTasksRemaining)
                {
                    if (PlayerControl.LocalPlayer == __instance)
                        Utils.Flash(role.Color);
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                        PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    {
                        Utils.Flash(role.Color);
                        Role.LocalRole.AllArrows.Add(role.PlayerId, new(PlayerControl.LocalPlayer, role.Color));
                    }
                }
                else if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && role.TasksDone)
                {
                    if (PlayerControl.LocalPlayer == __instance)
                    {
                        Utils.Flash(Color.green);

                        foreach (var imp in PlayerControl.AllPlayerControls.Where(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || (x.Is(RoleAlignment.NeutralKill) &&
                            CustomGameOptions.SnitchSeesNeutrals)))
                        {
                            Role.LocalRole.AllArrows.Add(imp.PlayerId, new(__instance, role.Color));
                        }
                    }
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                        PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    {
                        Utils.Flash(Color.red);
                    }
                }
            }

            if (__instance.Is(ObjectifierEnum.Traitor) && !__instance.Data.IsDead && AmongUsClient.Instance.AmHost)
            {
                var traitor = Objectifier.GetObjectifier<Traitor>(__instance);

                if (traitor.TasksDone)
                {
                    traitor.TurnTraitor();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnTraitor);
                    writer.Write(__instance.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(ObjectifierEnum.Taskmaster) && !__instance.Data.IsDead)
            {
                var role = Objectifier.GetObjectifier<Taskmaster>(__instance);

                if (role.TasksLeft == CustomGameOptions.TMTasksRemaining)
                {
                    if (PlayerControl.LocalPlayer == __instance || PlayerControl.LocalPlayer.Is(Faction.Crew) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBen) ||
                        PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil))
                    {
                        Utils.Flash(role.Color);
                    }
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                        PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralNeo) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralPros))
                    {
                        Utils.Flash(role.Color);
                        Role.LocalRole.AllArrows.Add(role.PlayerId, new(PlayerControl.LocalPlayer, role.Color));
                    }
                }
                else if (role.TasksDone)
                {
                    if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                        Utils.Flash(role.Color);

                    role.WinTasksDone = true;
                }
            }

            if (__instance.Is(RoleEnum.Phantom))
            {
                var role = Role.GetRole<Phantom>(__instance);

                if (role.TasksLeft == CustomGameOptions.PhantomTasksRemaining && CustomGameOptions.PhantomPlayersAlerted && !role.Caught)
                    Utils.Flash(role.Color);

                if (role.TasksDone && !role.Caught)
                    role.CompletedTasks = true;
            }
            else if (__instance.Is(RoleEnum.Revealer))
            {
                var role = Role.GetRole<Revealer>(__instance);

                if (role.TasksLeft == CustomGameOptions.RevealerTasksRemainingAlert && !role.Caught)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                        Utils.Flash(role.Color);
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) &&
                        CustomGameOptions.RevealerRevealsNeutrals))
                    {
                        role.Revealed = true;
                        Utils.Flash(role.Color);
                        Role.LocalRole.DeadArrows.Add(role.PlayerId, new(PlayerControl.LocalPlayer, role.Color));
                    }
                }
                else if (role.TasksDone && !role.Caught)
                {
                    role.CompletedTasks = role.TasksDone;

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer) || PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                        (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.RevealerRevealsNeutrals))
                    {
                        Utils.Flash(role.Color);
                    }
                }
            }

            var role1 = Role.GetRole(__instance);

            if (role1.TasksDone)
            {
                if (__instance.Is(RoleEnum.Tracker))
                    ((Tracker)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Transporter))
                    ((Transporter)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Vigilante))
                    ((Vigilante)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Veteran))
                    ((Veteran)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Altruist))
                    ((Altruist)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Monarch))
                    ((Monarch)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Chameleon))
                    ((Chameleon)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Engineer))
                    ((Engineer)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Retributionist))
                    ((Retributionist)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Coroner))
                {
                    if (((Coroner)role1).ReferenceBody != null)
                        ((Coroner)role1).UsesLeft++;
                }
            }
        }
    }
}