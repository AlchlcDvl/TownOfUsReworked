namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    public static class ConfirmEjects
    {
        public static ExileController LastExiled;

        public static void Prefix(ExileController __instance) => LastExiled = __instance;

        public static void Postfix(ExileController __instance)
        {
            var exiled = LastExiled.exiled;

            if (exiled == null)
            {
                __instance.completeString = "Everyone's safe...for now.";
                return;
            }

            var player = exiled.Object;
            var role = Role.GetRole(player);

            var flag = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Engineer) ||
                player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Impostor) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Operative);
            var factionflag = player.Is(Faction.Intruder);
            var subfactionflag = player.Is(SubFaction.Undead);

            var a_or_an = flag ? "an" : "a";
            var a_or_an2 = factionflag ? "an" : "a";
            var a_or_an3 = subfactionflag ? "an" : "a";

            var totalEvilsCount = PlayerControl.AllPlayerControls.Count(x => ((!x.Is(Faction.Crew) && !x.Is(RoleAlignment.NeutralBen) && !x.Is(RoleAlignment.NeutralEvil)) ||
                x.NotOnTheSameSide()) && !(x.Data.IsDead || x.Data.Disconnected));
            var totalEvilsRemaining = ConstantVariables.IsAA ? "an unknown number of" : $"{totalEvilsCount}";
            var evils = totalEvilsCount > 1 ? "evils" : "evil";
            var IsAre = totalEvilsCount > 1 ? "are" : "is";
            var totalEvils = $"There {IsAre} {totalEvilsRemaining} <color=#FF0000FF>{evils}</color> remaining.";

            var ejectString = "";
            PlayerControl target = null;

            if (role == null)
                return;

            role.DeathReason = DeathReasonEnum.Ejected;
            role.KilledBy = " ";

            foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
            {
                if (player == exe.TargetPlayer)
                    target = exe.TargetPlayer;
            }

            if (role == null || !CustomGameOptions.ConfirmEjects)
            {
                if (CustomGameOptions.CustomEject)
                {
                    if (TownOfUsReworked.VanillaOptions.MapId is 0 or 3)
                        ejectString = $"{player.name} is now one with space.";
                    else if (TownOfUsReworked.VanillaOptions.MapId == 1)
                        ejectString = $"{player.name} is now experiencing fatal free fall.";
                    else if (TownOfUsReworked.VanillaOptions.MapId == 2)
                        ejectString = $"{player.name} is now enjoying a hot bath.";
                    else if (TownOfUsReworked.VanillaOptions.MapId == 4)
                        ejectString = $"{player.name} is now experiencing gravity.";
                    else if (TownOfUsReworked.VanillaOptions.MapId == 5)
                        ejectString = $"{player.name} is now off to a scuba adventure.";
                }
                else
                    ejectString = $"{player.name} was ejected.";
            }
            else
            {
                if (CustomGameOptions.EjectionRevealsRole)
                {
                    if (player.Is(RoleEnum.Jester) && CustomGameOptions.JestEjectScreen)
                        ejectString = "The <color=#F7B3DAFF>Jester</color> will get his revenge from beyond the grave!";
                    else if (target != null && CustomGameOptions.ExeEjectScreen)
                        ejectString = "The <color=#CCCCCCFF>Executioner</color> will avenge the fallen crew!";
                    else
                        ejectString = $"{player.name} was {a_or_an} {role.ColorString + role.Name}</color>.";
                }
                else if (!player.Is(SubFaction.None))
                    ejectString = $"{player.name} was {a_or_an3} {role.SubFactionColorString + role.SubFactionName}</color>.";
                else if (player.Is(Faction.Crew) || player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                    ejectString = $"{player.name} was {a_or_an2} {role.FactionColorString + role.FactionName}</color>.";

                __instance.ImpostorText.text = totalEvils;
            }

            __instance.completeString = ejectString;

            foreach (var vigi in Role.GetRoles<Vigilante>(RoleEnum.Vigilante))
            {
                if (vigi.PostMeetingDie)
                {
                    vigi.Player.Exiled();
                    vigi.DeathReason = DeathReasonEnum.Suicide;
                }
            }
        }
    }
}