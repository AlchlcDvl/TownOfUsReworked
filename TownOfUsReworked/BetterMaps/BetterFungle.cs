namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterFungle
{
    [ToggleOption(MultiMenu.Main)]
    public static bool EnableBetterFungle { get; set; } = true;

    [NumberOption(MultiMenu.Main, 30f, 90f, 5f, Format.Time)]
    public static Number FungleReactorTimer { get; set; } = new(60);

    [NumberOption(MultiMenu.Main, 4f, 20f, 1f, Format.Time)]
    public static Number FungleMixupTimer { get; set; } = new(8);

    [HarmonyPatch(typeof(MushroomMixupSabotageSystem), nameof(MushroomMixupSabotageSystem.UpdateSystem))]
    public static class MushroomFungle
    {
        public static bool Prefix(MushroomMixupSabotageSystem __instance, MessageReader msgReader)
        {
            if (!EnableBetterFungle || MapPatches.CurrentMap != 5)
                return true;

            var operation = (MushroomMixupSabotageSystem.Operation)msgReader.ReadByte();

            if (operation == MushroomMixupSabotageSystem.Operation.TriggerSabotage)
            {
                __instance.Host_GenerateRandomOutfits();
                __instance.MushroomMixUp();
                __instance.currentState = MushroomMixupSabotageSystem.State.JustTriggered;
                __instance.currentSecondsUntilHeal = FungleMixupTimer;
                __instance.IsDirty = true;
                return false;
            }

            Logger.GlobalInstance.Error($"Unexpected operation {operation} to MushroomMixupSabotageSystem");
            return false;
        }
    }

    [HarmonyPatch(typeof(MushroomMixupSabotageSystem), nameof(MushroomMixupSabotageSystem.GenerateRandomOutfit))]
    public static class MushroomMixupSabFix
    {
        public static void Postfix(MushroomMixupSabotageSystem __instance, MushroomMixupSabotageSystem.CondensedOutfit __result)
        {
            List<byte> list = [ .. __instance.cachedOutfitsByPlayerId.keys ];
            list.RemoveAll(x => __instance.cachedOutfitsByPlayerId[x].ColorId.IsChanging());
            __result.ColorPlayerId = list.Random();
        }
    }
}