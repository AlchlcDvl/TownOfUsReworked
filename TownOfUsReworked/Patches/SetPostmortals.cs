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
                if (CustomPlayer.Local.Is(ModifierEnum.Astral) && !CustomPlayer.LocalCustom.IsDead)
                    Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();
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
            if (CustomPlayer.LocalCustom.Data.Disconnected)
                return;

            if (CustomPlayer.Local.Is(ModifierEnum.Astral) && !CustomPlayer.LocalCustom.IsDead)
                Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();

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

            foreach (var bh in Role.GetRoles<BountyHunter>(RoleEnum.BountyHunter))
            {
                if (bh.TargetKilled && !bh.IsDead)
                {
                    bh.Player.Exiled();
                    bh.DeathReason = DeathReasonEnum.Escaped;
                }
            }

            foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
            {
                if (exe.TargetVotedOut && !exe.IsDead)
                {
                    exe.Player.Exiled();
                    exe.DeathReason = DeathReasonEnum.Escaped;
                }
            }

            foreach (var guess in Role.GetRoles<Guesser>(RoleEnum.Guesser))
            {
                if (guess.TargetGuessed && !guess.IsDead)
                {
                    guess.Player.Exiled();
                    guess.DeathReason = DeathReasonEnum.Escaped;
                }
            }

            foreach (var cann in Role.GetRoles<Cannibal>(RoleEnum.Cannibal))
            {
                if (cann.Eaten && !cann.IsDead)
                {
                    cann.Player.Exiled();
                    cann.DeathReason = DeathReasonEnum.Escaped;
                }
            }

            foreach (var vigi in Role.GetRoles<Vigilante>(RoleEnum.Vigilante))
            {
                if (vigi.PostMeetingDie)
                {
                    vigi.Player.Exiled();
                    vigi.DeathReason = DeathReasonEnum.Suicide;
                }
            }

            BeginPostmortals(exiled);
        }

        private static void BeginPostmortals(PlayerControl player)
        {
            SetRevealers(player);
            SetPhantoms(player);
            SetBanshees(player);
            SetGhouls(player);
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

        private static void SetStartingVent(PlayerControl player)
        {
            var startingVent = ShipStatus.Instance.AllVents[URandom.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
            writer2.Write(player.PlayerId);
            writer2.Write(startingVent.transform.position);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            player.NetTransform.RpcSnapTo(new(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
            player.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static List<PlayerControl> WillBeRevealers = new();
        public static bool RevealerOn;

        private static void SetRevealers(PlayerControl exiled)
        {
            if (!RevealerOn)
                return;

            if (!WillBeRevealers.Contains(exiled) && WillBeRevealers.Count < CustomGameOptions.RevealerCount && exiled.Is(Faction.Crew))
                WillBeRevealers.Add(exiled);

            foreach (var rev in WillBeRevealers)
            {
                if (!rev.Data.IsDead)
                    continue;

                if (!rev.Is(RoleEnum.Revealer))
                {
                    var former = Role.GetRole(rev);
                    var role = new Revealer(rev) { FormerRole = former };
                    role.RoleUpdate(former);
                    Utils.RemoveTasks(CustomPlayer.Local);
                    rev.gameObject.layer = LayerMask.NameToLayer("Players");

                    if (CustomPlayer.Local != rev)
                        CustomPlayer.Local.MyPhysics.ResetMoveState();
                }

                if (rev == CustomPlayer.Local)
                {
                    if (Role.GetRole<Revealer>(rev).Caught)
                        continue;

                    SetStartingVent(rev);
                }
            }
        }

        public static List<PlayerControl> WillBePhantoms = new();
        public static bool PhantomOn;

        private static void SetPhantoms(PlayerControl exiled)
        {
            if (!PhantomOn)
                return;

            if (!WillBePhantoms.Contains(exiled) && WillBePhantoms.Count < CustomGameOptions.PhantomCount && exiled.Is(Faction.Neutral) &&
                !LayerExtentions.NeutralHasUnfinishedBusiness(exiled))
            {
                WillBePhantoms.Add(exiled);
            }

            foreach (var phan in WillBePhantoms)
            {
                if (!phan.Data.IsDead)
                    continue;

                if (!phan.Is(RoleEnum.Phantom))
                {
                    var former = Role.GetRole(phan);
                    var role = new Phantom(phan);
                    role.RoleUpdate(former);
                    Utils.RemoveTasks(phan);
                    phan.gameObject.layer = LayerMask.NameToLayer("Players");

                    if (CustomPlayer.Local != phan)
                        CustomPlayer.Local.MyPhysics.ResetMoveState();
                }

                if (phan == CustomPlayer.Local)
                {
                    if (Role.GetRole<Phantom>(phan).Caught)
                        continue;

                    SetStartingVent(phan);
                }
            }
        }

        public static List<PlayerControl> WillBeBanshees = new();
        public static bool BansheeOn;

        private static void SetBanshees(PlayerControl exiled)
        {
            if (!BansheeOn)
                return;

            if (!WillBeBanshees.Contains(exiled) && WillBeBanshees.Count < CustomGameOptions.BansheeCount && exiled.Is(Faction.Syndicate))
                WillBeBanshees.Add(exiled);

            foreach (var ban in WillBeBanshees)
            {
                if (!ban.Data.IsDead)
                    continue;

                if (!ban.Is(RoleEnum.Banshee))
                {
                    var former = Role.GetRole(ban);
                    var role = new Banshee(ban);
                    role.RoleUpdate(former);
                    ban.gameObject.layer = LayerMask.NameToLayer("Players");

                    if (CustomPlayer.Local != ban)
                        CustomPlayer.Local.MyPhysics.ResetMoveState();
                }

                if (ban == CustomPlayer.Local)
                {
                    if (Role.GetRole<Banshee>(ban).Caught)
                        continue;

                    SetStartingVent(ban);
                }
            }
        }

        public static List<PlayerControl> WillBeGhouls = new();
        public static bool GhoulOn;

        private static void SetGhouls(PlayerControl exiled)
        {
            if (!GhoulOn)
                return;

            if (!WillBeGhouls.Contains(exiled) && WillBeGhouls.Count < CustomGameOptions.GhoulCount && exiled.Is(Faction.Intruder))
                WillBeGhouls.Add(exiled);

            foreach (var ghoul in WillBeGhouls)
            {
                if (!ghoul.Data.IsDead)
                    continue;

                if (!ghoul.Is(RoleEnum.Ghoul))
                {
                    var former = Role.GetRole(ghoul);
                    var role = new Ghoul(ghoul);
                    role.RoleUpdate(former);
                    ghoul.gameObject.layer = LayerMask.NameToLayer("Players");

                    if (CustomPlayer.Local != ghoul)
                        CustomPlayer.Local.MyPhysics.ResetMoveState();
                }

                if (ghoul == CustomPlayer.Local)
                {
                    if (Role.GetRole<Ghoul>(ghoul).Caught)
                        continue;

                    SetStartingVent(ghoul);
                }
            }
        }
    }
}