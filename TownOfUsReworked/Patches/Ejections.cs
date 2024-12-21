namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
public static class ConfirmEjects
{
    public static void Postfix(ExileController __instance, ExileController.InitProperties init)
    {
        var exiled = init?.networkedPlayer;

        if (!exiled)
        {
            if (ClientOptions.CustomEjects)
                __instance.completeString = $"Everyone's safe...for now. ({(init.voteTie ? "Tie" : "Skipped")})";

            return;
        }

        var player = exiled.Object;
        var role = player.GetRole();

        if (!role)
            return;

        var ejectString = "";
        role.DeathReason = DeathReasonEnum.Ejected;
        role.KilledBy = "";

        if (!GameSettings.ConfirmEjects)
        {
            if (ClientOptions.CustomEjects)
            {
                ejectString = $"{player.Data.PlayerName} " + (MapPatches.CurrentMap switch
                {
                    1 => "is now learning about free fall",
                    2 => "is now enjoying a hot bath",
                    4 => "is now experiencing gravity",
                    5 => "has decided to leave for a new journey",
                    6 => "is now off to a scuba adventure",
                    7 => "has decided to explore newer frontiers",
                    _ => "is now one with space",
                }) + ".";
            }
            else
                ejectString = $"{player.Data.PlayerName} was ejected.";
        }
        else
        {
            if (GameSettings.EjectionRevealsRoles)
            {
                if (player.Is<Jester>() && Jester.JestEjectScreen)
                    ejectString = "The <#F7B3DAFF>Jester</color> will get his revenge from beyond the grave!";
                else if (PlayerLayer.GetLayers<Executioner>().Any(x => x.TargetPlayer == player) && Executioner.ExeEjectScreen)
                    ejectString = "The <#CCCCCCFF>Executioner</color> will avenge the fallen crew!";
                else
                    ejectString = $"{player.Data.PlayerName} was the {role.ColorString}{role.Name}</color>.";
            }
            else if (!player.Is(SubFaction.None))
                ejectString = $"{player.Data.PlayerName} was {role.SubFactionColorString}{role.SubFactionName}</color>.";
            else
                ejectString = $"{player.Data.PlayerName} was {role.FactionColorString}{role.FactionName}</color>.";

            var totalEvilsCount = AllPlayers().Count(x => ((!x.Is(Faction.Crew) && !x.Is(Alignment.NeutralBen) && !x.Is(Alignment.NeutralEvil)) || x.NotOnTheSameSide()) &&
                !x.HasDied());
            var totalEvilsRemaining = IsAA() ? "an unknown number of" : $"{totalEvilsCount}";
            var s = totalEvilsCount > 1 ? "s" : "";
            var isAre = totalEvilsCount > 1 ? "are" : "is";
            __instance.ImpostorText.SetText($"There {isAre} {totalEvilsRemaining} <#FF0000FF>evil{s}</color> remaining.");
        }

        __instance.completeString = ejectString;
    }
}