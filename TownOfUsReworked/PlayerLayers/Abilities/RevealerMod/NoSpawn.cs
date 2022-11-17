using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.RevealerMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Revealer))
            {
                var caught = Ability.GetAbility<Revealer>(PlayerControl.LocalPlayer).Caught;
                
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