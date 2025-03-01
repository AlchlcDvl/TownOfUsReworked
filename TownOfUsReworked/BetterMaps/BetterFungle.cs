namespace TownOfUsReworked.BetterMaps;

/// <summary>
/// Provides enhanced functionality and customization options for the Fungle map's sabotage systems.
/// </summary>
[HeaderOption(MultiMenu.Main), HarmonyPatch(typeof(MushroomMixupSabotageSystem))]
public static class BetterFungle
{
    /// <summary>
    /// Enables or disables all BetterFungle modifications.
    /// </summary>
    [ToggleOption]
    public static bool EnableBetterFungle = true;

    /// <summary>
    /// Time until the reactor meltdown occurs during sabotage.
    /// </summary>
    /// <remarks>
    /// Default: <c>60</c>s<br/>
    /// Range: <c>30</c> to <c>90</c>s<br/>
    /// Increment: <c>5</c>s
    /// </remarks>
    [NumberOption(30f, 90f, 5f, Format.Time)]
    public static Number FungleReactorTimer = 60;

    /// <summary>
    /// Duration of the Mushroom Mixup effect when triggered.
    /// </summary>
    /// <remarks>
    /// Default: <c>8</c>s<br/>
    /// Range: <c>4</c> to <c>20</c>s<br/>
    /// Increment: <c>1</c>s
    /// </remarks>
    [NumberOption(4f, 20f, 1f, Format.Time)]
    private static Number FungleMixupTimer = 8;

    [HarmonyPatch(nameof(MushroomMixupSabotageSystem.UpdateSystem))]
    public static bool Prefix(MushroomMixupSabotageSystem __instance, MessageReader msgReader)
    {
        // Skip modifications if disabled or not on the Fungle map (map ID 5)
        if (!EnableBetterFungle || MapPatches.CurrentMap != 5)
            return true;

        // Read the operation type from the network message
        var operation = (MushroomMixupSabotageSystem.Operation)msgReader.ReadByte();

        // Handle mushroom mixup sabotage trigger
        if (operation == MushroomMixupSabotageSystem.Operation.TriggerSabotage)
        {
            // Generate and apply random outfits to all players
            __instance.Host_GenerateRandomOutfits();
            __instance.MushroomMixUp();

            // Update sabotage state with a custom timer
            __instance.currentState = MushroomMixupSabotageSystem.State.JustTriggered;
            __instance.currentSecondsUntilHeal = FungleMixupTimer;
            __instance.IsDirty = true;
        }
        else
        {
            // Log error for unexpected operations
            Logger.GlobalInstance.Error($"Unexpected operation {operation} to MushroomMixupSabotageSystem");
        }

        return false;
    }

    [HarmonyPatch(nameof(MushroomMixupSabotageSystem.GenerateRandomOutfit))]
    public static void Postfix(MushroomMixupSabotageSystem __instance, ref MushroomMixupSabotageSystem.CondensedOutfit __result)
    {
        // Create a list of all IDs from the cached outfits
        List<byte> list = [ .. __instance.cachedOutfitsByPlayerId.keys ];

        // Remove players with changing colors
        list.RemoveAll(x => __instance.cachedOutfitsByPlayerId[x].ColorId.IsChanging());

        // Select a random ID from the filtered list for color assignment
        __result.ColorPlayerId = list.Random();
    }
}