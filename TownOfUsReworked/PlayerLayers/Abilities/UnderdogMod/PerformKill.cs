using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class PerformKill
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var ability = Ability.GetAbility(__instance);

            if (ability?.AbilityType == AbilityEnum.Underdog)
                ((Underdog)ability).SetKillTimer();
        }

        internal static bool IncreasedKC()
        {
            if (CustomGameOptions.UnderdogIncreasedKC)
                return false;
            else
                return true;
        }
    }
}
