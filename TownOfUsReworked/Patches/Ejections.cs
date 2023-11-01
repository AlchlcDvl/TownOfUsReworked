namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
public static class ConfirmEjects
{
    public static void Postfix(ExileController __instance, ref bool tie)
    {
        var exiled = __instance.exiled;

        if (exiled == null)
        {
            if (ClientGameOptions.CustomEjects)
                __instance.completeString = $"Everyone's safe...for now. ({(tie ? "Tie" : "Skipped")})";

            return;
        }

        var player = exiled.Object;
        var role = Role.GetRole(player);

        var totalEvilsCount = CustomPlayer.AllPlayers.Count(x => ((!x.Is(Faction.Crew) && !x.Is(Alignment.NeutralBen) && !x.Is(Alignment.NeutralEvil)) || x.NotOnTheSameSide()) &&
            !x.HasDied());
        var totalEvilsRemaining = IsAA ? "an unknown number of" : $"{totalEvilsCount}";
        var s = totalEvilsCount > 1 ? "s" : "";
        var isAre = totalEvilsCount > 1 ? "are" : "is";
        var totalEvils = $"There {isAre} {totalEvilsRemaining} <color=#FF0000FF>evil{s}</color> remaining.";

        var ejectString = "";
        var target = PlayerLayer.GetLayers<Executioner>().First(x => x.TargetPlayer == player)?.TargetPlayer;

        if (role == null)
            return;

        role.DeathReason = DeathReasonEnum.Ejected;
        role.KilledBy = " ";

        if (role == null || !CustomGameOptions.ConfirmEjects)
        {
            if (ClientGameOptions.CustomEjects)
            {
                if (MapPatches.CurrentMap is 0 or 3)
                    ejectString = $"{player.name} is now one with space.";
                else if (MapPatches.CurrentMap == 1)
                    ejectString = $"{player.name} is now experiencing fatal free fall.";
                else if (MapPatches.CurrentMap == 2)
                    ejectString = $"{player.name} is now enjoying a hot bath.";
                else if (MapPatches.CurrentMap == 4)
                    ejectString = $"{player.name} is now experiencing gravity.";
                else if (MapPatches.CurrentMap == 5)
                    ejectString = $"{player.name} has decided to leave for a new journey.";
                else if (MapPatches.CurrentMap == 6)
                    ejectString = $"{player.name} is now off to a scuba adventure.";
                else if (MapPatches.CurrentMap == 7)
                    ejectString = $"{player.name} has decided to explore newer frontiers.";
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
                    ejectString = $"{player.name} was the {role.ColorString}{role.Name}</color>.";
            }
            else if (!player.Is(SubFaction.None))
                ejectString = $"{player.name} was {role.SubFactionColorString}{role.SubFactionName}</color>.";
            else
                ejectString = $"{player.name} was {role.FactionColorString}{role.FactionName}</color>.";

            __instance.ImpostorText.text = totalEvils;
        }

        __instance.completeString = ejectString;
    }
}