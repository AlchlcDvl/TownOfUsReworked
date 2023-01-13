using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.PhantomMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Phantom))
            {
                var phantom = Objectifier.GetObjectifier<Phantom>(PlayerControl.LocalPlayer);

                if (!phantom.HasDied)
                    return false;

                var caught = phantom.Caught;

                if (!caught)
                {
                    __instance.Close();
                    return false;
                }
            }

            return true;
        }
    }
}