namespace TownOfUsReworked.Patches.Core.Player;

[HarmonyPatch(typeof(PlayerOutfit))]
public static class PlayerOutfitPatches
{
    [HarmonyPatch(nameof(PlayerOutfit.Serialize))]
    public static void Postfix(PlayerOutfit __instance, MessageWriter writer)
    {
        if (!__instance.TryCast<CustomOutfit>(out var outfit))
            return;

        writer.Write(outfit.Size);
        writer.Write(outfit.Speed);
        writer.Write(outfit.Alpha);
    }

    [HarmonyPatch(nameof(PlayerOutfit.Deserialize))]
    public static void Postfix(PlayerOutfit __instance, MessageReader reader)
    {
        if (!__instance.TryCast<CustomOutfit>(out var outfit))
            return;

        outfit.Size = reader.ReadSingle();
        outfit.Speed = reader.ReadSingle();
        outfit.Alpha = reader.ReadSingle();
    }
}

[HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.UpdateFromPlayerOutfit))]
public static class PoolablePlayerOutfitPatch
{
    public static void Postfix(PoolablePlayer __instance, PlayerOutfit outfit)
    {
        if (outfit.TryCast<CustomOutfit>(out var custom))
            __instance.transform.localScale *= custom.Size;
    }
}