using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.InsiderMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(AbilityEnum.Insider))
                return;

            if (__instance.Data.IsDead)
                return;

            var ability = Ability.GetAbility<Insider>(__instance);
            var role = Role.GetRole(ability.Player);

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Insider) && role.TasksDone)
                Coroutines.Start(Utils.FlashCoroutine(ability.Color));
        }
    }
}