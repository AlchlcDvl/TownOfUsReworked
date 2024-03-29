namespace TownOfUsReworked.BetterMaps;

[HarmonyPatch(typeof(MushroomMixupSabotageSystem), nameof(MushroomMixupSabotageSystem.UpdateSystem))]
public static class MushroomFungle
{
    public static bool Prefix(MushroomMixupSabotageSystem __instance, ref MessageReader msgReader)
    {
        if (!CustomGameOptions.EnableBetterFungle || MapPatches.CurrentMap != 5)
            return true;

        var operation = (MushroomMixupSabotageSystem.Operation)msgReader.ReadByte();

        if (operation == MushroomMixupSabotageSystem.Operation.TriggerSabotage)
        {
            __instance.Host_GenerateRandomOutfits();
            __instance.MushroomMixUp();
            __instance.currentState = MushroomMixupSabotageSystem.State.JustTriggered;
            __instance.currentSecondsUntilHeal = CustomGameOptions.FungleMixupTimer;
            __instance.IsDirty = true;
            return false;
        }

        Logger.GlobalInstance.Error($"Unexpected operation {operation} to MushroomMixupSabotageSystem");
        return false;
    }
}