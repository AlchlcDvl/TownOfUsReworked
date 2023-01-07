using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix()
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Underdog))
                return;

            var underdog = Ability.GetAbility<Underdog>(PlayerControl.LocalPlayer);
            underdog.SetKillTimer();
        }
    }
}
