namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
[HarmonyPriority(Priority.First)]
public static class ConfirmEjects
{
    public static ExileController LastExiled;

    public static void Prefix(ExileController __instance) => LastExiled = __instance;

    public static void Postfix(ExileController __instance, ref bool tie)
    {
        var exiled = LastExiled.exiled;

        if (exiled == null)
        {
            if (ClientGameOptions.CustomEjects)
                __instance.completeString = $"Everyone's safe...for now. ({(tie ? "Tie" : "Skipped")})";

            return;
        }

        var player = exiled.Object;
        var role = Role.GetRole(player);

        var totalEvilsCount = CustomPlayer.AllPlayers.Count(x => ((!x.Is(Faction.Crew) && !x.Is(RoleAlignment.NeutralBen) && !x.Is(RoleAlignment.NeutralEvil)) ||
            x.NotOnTheSameSide()) && !(x.Data.IsDead || x.Data.Disconnected));
        var totalEvilsRemaining = IsAA ? "an unknown number of" : $"{totalEvilsCount}";
        var evils = totalEvilsCount > 1 ? "evils" : "evil";
        var IsAre = totalEvilsCount > 1 ? "are" : "is";
        var totalEvils = $"There {IsAre} {totalEvilsRemaining} <color=#FF0000FF>{evils}</color> remaining.";

        var ejectString = "";
        PlayerControl target = null;

        if (role == null)
            return;

        role.DeathReason = DeathReasonEnum.Ejected;
        role.KilledBy = " ";

        foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
        {
            if (player == exe.TargetPlayer)
                target = exe.TargetPlayer;
        }

        if (role == null || !CustomGameOptions.ConfirmEjects)
        {
            if (ClientGameOptions.CustomEjects)
            {
                if (TownOfUsReworked.NormalOptions.MapId is 0 or 3)
                    ejectString = $"{player.name} is now one with space.";
                else if (TownOfUsReworked.NormalOptions.MapId == 1)
                    ejectString = $"{player.name} is now experiencing fatal free fall.";
                else if (TownOfUsReworked.NormalOptions.MapId == 2)
                    ejectString = $"{player.name} is now enjoying a hot bath.";
                else if (TownOfUsReworked.NormalOptions.MapId == 4)
                    ejectString = $"{player.name} is now experiencing gravity.";
                else if (TownOfUsReworked.NormalOptions.MapId == 5)
                    ejectString = $"{player.name} is now off to a scuba adventure.";
                else if (TownOfUsReworked.NormalOptions.MapId == 6)
                    ejectString = $"{player.name} has decided to leave for a new journey.";
            }
            else
                ejectString = $"{player.name} was ejected.";
        }
        else
        {
            if (CustomGameOptions.EjectionRevealsRole)
            {
                if (player.Is(LayerEnum.Jester) && CustomGameOptions.JestEjectScreen)
                    ejectString = "The <color=#F7B3DAFF>Jester</color> will get his revenge from beyond the grave!";
                else if (target != null && CustomGameOptions.ExeEjectScreen)
                    ejectString = "The <color=#CCCCCCFF>Executioner</color> will avenge the fallen crew!";
                else
                    ejectString = $"{player.name} was the {role.ColorString + role.Name}</color>.";
            }
            else if (!player.Is(SubFaction.None))
                ejectString = $"{player.name} was the {role.SubFactionColorString + role.SubFactionName}</color>.";
            else if (player.Is(Faction.Crew) || player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                ejectString = $"{player.name} was the {role.FactionColorString + role.FactionName}</color>.";

            __instance.ImpostorText.text = totalEvils;
        }

        __instance.completeString = ejectString;

        foreach (var vigi in Role.GetRoles<Vigilante>(LayerEnum.Vigilante))
        {
            if (vigi.PostMeetingDie)
            {
                vigi.Player.Exiled();
                vigi.DeathReason = DeathReasonEnum.Suicide;
            }
        }
    }
}