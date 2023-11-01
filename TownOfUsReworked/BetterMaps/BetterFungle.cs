namespace TownOfUsReworked.BetterMaps;

[HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.UpdateSystem))]
public static class ReactorFungle
{
    public static bool Prefix(ReactorSystemType __instance, ref MessageReader msgReader)
    {
        if (!CustomGameOptions.EnableBetterFungle)
            return true;

        if (ShipStatus.Instance.Type == ShipStatus.MapType.Fungle && msgReader.ReadByte() == 128 && !__instance.IsActive)
        {
            __instance.Countdown = __instance.ReactorDuration = CustomGameOptions.FungleReactorTimer;
            __instance.UserConsolePairs.Clear();
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(MushroomMixupSabotageSystem), nameof(MushroomMixupSabotageSystem.UpdateSystem))]
public static class MushroomFungle
{
    public static bool Prefix(MushroomMixupSabotageSystem __instance, ref MessageReader msgReader)
    {
        if (!CustomGameOptions.EnableBetterFungle)
            return true;

        if ((MushroomMixupSabotageSystem.Operation)msgReader.ReadByte() == MushroomMixupSabotageSystem.Operation.TriggerSabotage && !__instance.IsActive)
        {
            __instance.Host_GenerateRandomOutfits();
            __instance.MushroomMixUp();
            __instance.currentState = MushroomMixupSabotageSystem.State.JustTriggered;
            __instance.secondsForAutoHeal = __instance.currentSecondsUntilHeal = CustomGameOptions.FungleMixupTimer;
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}