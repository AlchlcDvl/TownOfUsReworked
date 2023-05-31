namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExile
    {
        public static void Postfix(AirshipExileController __instance) => SetPostmortals.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), new Type[] { typeof(GameObject) })]
    public static class SubmergedExile
    {
        public static void Prefix(GameObject obj)
        {
            if (!ModCompatibility.SubLoaded || TownOfUsReworked.VanillaOptions.MapId != 5)
                return;

            if (obj.name.Contains("ExileCutscene"))
                SetPostmortals.ExileControllerPostfix(ConfirmEjects.LastExiled);
            else if (obj.name.Contains("SpawnInMinigame"))
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Astral))
                    Modifier.GetModifier<Astral>(PlayerControl.LocalPlayer).SetPosition();
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class SetPostmortals
    {
        public readonly static List<PlayerControl> AssassinatedPlayers = new();

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (PlayerControl.LocalPlayer.Data.Disconnected)
                return;

            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Astral))
                Modifier.GetModifier<Astral>(PlayerControl.LocalPlayer).SetPosition();

            foreach (var player in AssassinatedPlayers)
            {
                if (!player.Data.Disconnected)
                    player.Exiled();
            }

            AssassinatedPlayers.Clear();

            foreach (var ghoul in Role.GetRoles<Ghoul>(RoleEnum.Ghoul))
            {
                if (ghoul.Caught)
                    ghoul.MarkedPlayer = null;
                else if (ghoul.MarkedPlayer != null && !(ghoul.MarkedPlayer.Data.IsDead || ghoul.MarkedPlayer.Data.Disconnected))
                    ghoul.MarkedPlayer.Exiled();
            }

            var exiled = __instance.exiled?.Object;

            if (exiled != null)
            {
                JesterWin(exiled);
                ExecutionerWin(exiled);

                if (exiled.Is(ObjectifierEnum.Lovers))
                {
                    var lover = exiled.GetOtherLover();

                    if (!lover.Is(RoleEnum.Pestilence) && CustomGameOptions.BothLoversDie)
                        lover?.Exiled();
                }

                Reassign(exiled);
            }

            foreach (var dict in Role.GetRoles<Dictator>(RoleEnum.Dictator))
            {
                if (dict.Revealed && dict.ToBeEjected.Count > 0 && !dict.ToBeEjected.Any(x => x == 255))
                {
                    foreach (var exiled1 in dict.ToBeEjected)
                    {
                        var player = Utils.PlayerById(exiled1);

                        if (player == null)
                            continue;

                        player.Exiled();
                        var role = Role.GetRole(player);
                        role.KilledBy = " By " + dict.PlayerName;
                        role.DeathReason = DeathReasonEnum.Dictated;
                    }

                    if (dict.ToDie)
                    {
                        dict.Player.Exiled();
                        dict.DeathReason = DeathReasonEnum.Suicide;
                    }

                    dict.Ejected = true;
                    dict.ToBeEjected.Clear();
                }
            }
        }

        public static void Reassign(PlayerControl player)
        {
            SetRevealer(player);
            SetPhantom(player);
            SetBanshee(player);
            SetGhoul(player);
        }

        private static void JesterWin(PlayerControl player)
        {
            foreach (var jest in Role.GetRoles<Jester>(RoleEnum.Jester))
            {
                if (jest.Player == player)
                {
                    jest.VotedOut = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.JesterWin);
                    writer.Write(jest.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        private static void ExecutionerWin(PlayerControl player)
        {
            foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
            {
                if (exe.TargetPlayer == null || (!CustomGameOptions.ExeCanWinBeyondDeath && exe.IsDead))
                    continue;

                if (player == exe.TargetPlayer)
                {
                    exe.TargetVotedOut = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.ExecutionerWin);
                    writer.Write(exe.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static void SetStartingVent(PlayerControl player)
        {
            var startingVent = ShipStatus.Instance.AllVents[URandom.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
            writer2.Write(player.PlayerId);
            writer2.Write(startingVent.transform.position);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            player.NetTransform.RpcSnapTo(new(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
            player.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void RemoveTasks(PlayerControl player)
        {
            foreach (var task in player.myTasks)
            {
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();
                    var updateArrow = normalPlayerTask.taskStep > 0;
                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();

                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                    {
                        foreach (var console in UObject.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    }

                    normalPlayerTask.taskStep = 0;

                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;

                    if ((normalPlayerTask.TaskType is TaskTypes.EmptyGarbage or TaskTypes.EmptyChute) && (TownOfUsReworked.VanillaOptions.MapId is 0 or 3 or 4))
                        normalPlayerTask.taskStep = 1;

                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
            }
        }

        public static PlayerControl WillBeRevealer;
        public static bool RevealerOn;

        public static void SetRevealer(PlayerControl exiled)
        {
            if (!RevealerOn || !exiled.Is(Faction.Crew))
                return;

            if (WillBeRevealer == null)
                WillBeRevealer = exiled;

            if (WillBeRevealer.Data.Disconnected)
                Utils.ReassignPostmortals(WillBeRevealer);

            if (!WillBeRevealer.Data.IsDead)
                return;

            if (!WillBeRevealer.Is(RoleEnum.Revealer))
            {
                var former = Role.GetRole(WillBeRevealer);
                var role = new Revealer(WillBeRevealer) { FormerRole = former };
                role.RoleUpdate(former);
                RemoveTasks(PlayerControl.LocalPlayer);
                WillBeRevealer.gameObject.layer = LayerMask.NameToLayer("Players");
                WillBeRevealer.MyPhysics.ResetMoveState();
            }

            if (WillBeRevealer == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Revealer>(WillBeRevealer).Caught)
                    return;

                SetStartingVent(WillBeRevealer);
            }
        }

        public static PlayerControl WillBePhantom;
        public static bool PhantomOn;

        public static void SetPhantom(PlayerControl exiled)
        {
            if (!PhantomOn || LayerExtentions.NeutralHasUnfinishedBusiness(exiled) || !exiled.Is(Faction.Neutral))
                return;

            if (WillBePhantom == null)
                WillBePhantom = exiled;

            if (WillBePhantom.Data.Disconnected)
                Utils.ReassignPostmortals(WillBePhantom);

            if (!WillBePhantom.Data.IsDead)
                return;

            if (!WillBePhantom.Is(RoleEnum.Phantom))
            {
                var former = Role.GetRole(WillBePhantom);
                var role = new Phantom(WillBePhantom);
                role.RoleUpdate(former);
                RemoveTasks(WillBePhantom);
                WillBePhantom.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBePhantom)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBePhantom == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Phantom>(WillBePhantom).Caught)
                    return;

                SetStartingVent(WillBePhantom);
            }
        }

        public static PlayerControl WillBeBanshee;
        public static bool BansheeOn;

        public static void SetBanshee(PlayerControl exiled)
        {
            if (!BansheeOn || !exiled.Is(Faction.Syndicate))
                return;

            if (WillBeBanshee == null)
                WillBeBanshee = exiled;

            if (WillBeBanshee == null)
                WillBeBanshee = exiled;

            if (WillBeBanshee.Data.Disconnected)
                Utils.ReassignPostmortals(WillBeBanshee);

            if (!WillBeBanshee.Data.IsDead)
                return;

            if (!WillBeBanshee.Is(RoleEnum.Banshee))
            {
                var former = Role.GetRole(WillBeBanshee);
                var role = new Banshee(WillBeBanshee);
                role.RoleUpdate(former);
                WillBeBanshee.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBeBanshee)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBeBanshee == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Banshee>(WillBeBanshee).Caught)
                    return;

                SetStartingVent(WillBeBanshee);
            }
        }

        public static PlayerControl WillBeGhoul;
        public static bool GhoulOn;

        public static void SetGhoul(PlayerControl exiled)
        {
            if (!GhoulOn || !exiled.Is(Faction.Syndicate))
                return;

            if (WillBeGhoul == null)
                WillBeGhoul = exiled;

            if (WillBeGhoul == null)
                WillBeGhoul = exiled;

            if (WillBeGhoul.Data.Disconnected)
                Utils.ReassignPostmortals(WillBeGhoul);

            if (!WillBeGhoul.Data.IsDead)
                return;

            if (!WillBeGhoul.Is(RoleEnum.Banshee))
            {
                var former = Role.GetRole(WillBeGhoul);
                var role = new Ghoul(WillBeGhoul);
                role.RoleUpdate(former);
                WillBeGhoul.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBeGhoul)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBeGhoul == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Ghoul>(WillBeGhoul).Caught)
                    return;

                SetStartingVent(WillBeGhoul);
            }
        }
    }
}