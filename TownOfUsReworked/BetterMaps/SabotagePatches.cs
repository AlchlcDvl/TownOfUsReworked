namespace TownOfUsReworked.BetterMaps;

[HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.UpdateSystem))]
public static class ReactorPatch
{
    public static bool Prefix(ReactorSystemType __instance, ref PlayerControl player, ref MessageReader msgReader)
    {
        var flag = MapPatches.CurrentMap switch
        {
            0 or 3 => CustomGameOptions.EnableBetterSkeld,
            1 => CustomGameOptions.EnableBetterMiraHQ,
            2 => CustomGameOptions.EnableBetterPolus,
            5 => CustomGameOptions.EnableBetterFungle,
            _ => false
        };

        if (!flag)
            return true;

        var b = msgReader.ReadByte();
        var num = (byte)(b & 3);

        if (b == 128 && !__instance.IsActive)
        {
            __instance.Countdown = MapPatches.CurrentMap switch
            {
                0 or 3 => CustomGameOptions.SkeldReactorTimer,
                1 => CustomGameOptions.MiraReactorTimer,
                2 => CustomGameOptions.SeismicTimer,
                5 => CustomGameOptions.FungleReactorTimer,
                _ => __instance.ReactorDuration
            };
            __instance.UserConsolePairs.Clear();
        }
        else if (b == 16)
            __instance.Countdown = 10000f;
        else if (b.HasAnyBit(64))
        {
            __instance.UserConsolePairs.Add(new(player.PlayerId, num));

            if (__instance.UserCount >= 2)
                __instance.Countdown = 10000f;
        }
        else if (b.HasAnyBit(32))
            __instance.UserConsolePairs.Remove(new(player.PlayerId, num));

        __instance.IsDirty = true;
        return false;
    }
}

[HarmonyPatch(typeof(LifeSuppSystemType), nameof(LifeSuppSystemType.UpdateSystem))]
public static class O2Patch
{
    public static bool Prefix(LifeSuppSystemType __instance, ref MessageReader msgReader)
    {
        var flag = MapPatches.CurrentMap switch
        {
            0 or 3 => CustomGameOptions.EnableBetterSkeld,
            1 => CustomGameOptions.EnableBetterMiraHQ,
            _ => false
        };

        if (!flag)
            return true;

        var b = msgReader.ReadByte();
        var num = b & 3;

        if (b == 128 && !__instance.IsActive)
        {
            __instance.Countdown = MapPatches.CurrentMap switch
            {
                0 or 3 => CustomGameOptions.SkeldO2Timer,
                1 => CustomGameOptions.MiraO2Timer,
                _ => __instance.LifeSuppDuration
            };
            __instance.CompletedConsoles.Clear();
        }
        else if (b == 16)
            __instance.Countdown = 10000f;
        else if (b.HasAnyBit(64))
            __instance.CompletedConsoles.Add(num);

        __instance.IsDirty = true;
        return false;
    }
}