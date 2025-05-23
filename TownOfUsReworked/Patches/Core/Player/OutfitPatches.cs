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

        if (outfit.ColorId != -2)
            return;

        // writer.Write(outfit.Color); For when my reactor pr is approved
        writer.Write(outfit.Color.r);
        writer.Write(outfit.Color.g);
        writer.Write(outfit.Color.b);
        writer.Write(outfit.Color.a);
    }

    [HarmonyPatch(nameof(PlayerOutfit.Deserialize))]
    public static void Postfix(PlayerOutfit __instance, MessageReader reader)
    {
        if (!__instance.TryCast<CustomOutfit>(out var outfit))
            return;

        outfit.Size = reader.ReadSingle();
        outfit.Speed = reader.ReadSingle();
        outfit.Alpha = reader.ReadSingle();

        if (outfit.ColorId != -2)
            return;

        // outfit.Color = reader.ReadColor(); For when my reactor pr is approved
        var color = (Color32)default;
        color.r = reader.ReadByte();
        color.g = reader.ReadByte();
        color.b = reader.ReadByte();
        color.a = reader.ReadByte();
        outfit.Color = color;
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