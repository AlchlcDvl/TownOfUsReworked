using HarmonyLib;
using Random = UnityEngine.Random;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    public static class GetAdjustedImposters
    {
        public static bool Prefix(ref int __result)
        {
            if (ConstantVariables.IsHnS)
                return true;

            if (ConstantVariables.IsAA)
            {
                var players = GameData.Instance.PlayerCount;
                var random = Random.RandomRangeInt(0, 100);
                int impostors;

                if (players <= 6)
                {
                    if (random <= 5)
                        impostors = 0;
                    else
                        impostors = 1;
                }
                else if (players == 7)
                {
                    if (random < 5)
                        impostors = 0;
                    else if (random < 20)
                        impostors = 2;
                    else
                        impostors = 1;
                }
                else if (players == 8)
                {
                    if (random < 5)
                        impostors = 0;
                    else if (random < 40)
                        impostors = 2;
                    else
                        impostors = 1;
                }
                else if (players == 9)
                {
                    if (random < 5)
                        impostors = 0;
                    else if (random < 50)
                        impostors = 2;
                    else
                        impostors = 1;
                }
                else if (players == 10)
                {
                    if (random < 5)
                        impostors = 0;
                    else if (random < 10)
                        impostors = 3;
                    else if (random < 60)
                        impostors = 2;
                    else
                        impostors = 1;
                }
                else if (players == 11)
                {
                    if (random < 10)
                        impostors = 0;
                    else if (random < 60)
                        impostors = 2;
                    else if (random < 70)
                        impostors = 3;
                    else
                        impostors = 1;
                }
                else if (players == 12)
                {
                    if (random < 10)
                        impostors = 0;
                    else if (random < 60)
                        impostors = 2;
                    else if (random < 80)
                        impostors = 3;
                    else
                        impostors = 1;
                }
                else if (players == 13)
                {
                    if (random < 10)
                        impostors = 0;
                    else if (random < 60)
                        impostors = 2;
                    else if (random < 90)
                        impostors = 3;
                    else
                        impostors = 1;
                }
                else if (players == 14)
                {
                    if (random < 5)
                        impostors = 0;
                    else if (random < 25)
                        impostors = 1;
                    else if (random < 50)
                        impostors = 3;
                    else
                        impostors = 2;
                }
                else if (random < 5)
                    impostors = 0;
                else if (random < 20)
                    impostors = 1;
                else if (random < 60)
                    impostors = 3;
                else if (random < 90)
                    impostors = 2;
                else
                    impostors = 4;

                __result = impostors;
                return false;
            }

            __result = CustomGameOptions.IntruderCount == 0 ? CustomGameOptions.SyndicateCount : CustomGameOptions.IntruderCount;
            return true;
        }
    }
}
