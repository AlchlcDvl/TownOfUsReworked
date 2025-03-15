namespace TownOfUsReworked.Patches.BetterStuff;

[HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.UpdateSystem))]
public static class ReactorPatch
{
    public static bool Prefix(ReactorSystemType __instance, PlayerControl player, MessageReader msgReader)
    {
        if (!(MapPatches.CurrentMap switch
        {
            0 or 3 => BetterSkeld.EnableBetterSkeld,
            1 => BetterMiraHq.EnableBetterMiraHq,
            2 => BetterPolus.EnableBetterPolus,
            5 => BetterFungle.EnableBetterFungle,
            _ => false
        }))
        {
            return true;
        }

        var b = msgReader.ReadByte();

        switch (b)
        {
            case 128 when !__instance.IsActive:
            {
                __instance.Countdown = MapPatches.CurrentMap switch
                {
                    0 or 3 => BetterSkeld.SkeldReactorTimer,
                    1 => BetterMiraHq.MiraReactorTimer,
                    2 => BetterPolus.SeismicTimer,
                    5 => BetterFungle.FungleReactorTimer,
                    _ => __instance.ReactorDuration
                };
                __instance.UserConsolePairs.Clear();
                break;
            }
            case 16:
            {
                __instance.Countdown = 10000f;
                break;
            }
            default:
            {
                var num = (byte)(b & 3);

                if (b.HasAnyBit(64))
                {
                    __instance.UserConsolePairs.Add(new(player.PlayerId, num));

                    if (__instance.UserCount >= 2)
                        __instance.Countdown = 10000f;
                }
                else if (b.HasAnyBit(32))
                    __instance.UserConsolePairs.Remove(new(player.PlayerId, num));

                break;
            }
        }

        __instance.IsDirty = true;
        return false;
    }
}

[HarmonyPatch(typeof(LifeSuppSystemType), nameof(LifeSuppSystemType.UpdateSystem))]
public static class O2Patch
{
    public static bool Prefix(LifeSuppSystemType __instance, MessageReader msgReader)
    {
        if (!(MapPatches.CurrentMap switch
        {
            0 or 3 => BetterSkeld.EnableBetterSkeld,
            1 => BetterMiraHq.EnableBetterMiraHq,
            _ => false
        }))
        {
            return true;
        }

        var b = msgReader.ReadByte();

        switch (b)
        {
            case 128 when !__instance.IsActive:
            {
                __instance.Countdown = MapPatches.CurrentMap switch
                {
                    0 or 3 => BetterSkeld.SkeldO2Timer,
                    1 => BetterMiraHq.MiraO2Timer,
                    _ => __instance.LifeSuppDuration
                };
                __instance.CompletedConsoles.Clear();
                break;
            }
            case 16:
            {
                __instance.Countdown = 10000f;
                break;
            }
            default:
            {
                if (b.HasAnyBit(64))
                    __instance.CompletedConsoles.Add(b & 3);

                break;
            }
        }

        __instance.IsDirty = true;
        return false;
    }
}