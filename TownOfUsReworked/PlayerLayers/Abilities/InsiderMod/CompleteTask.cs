using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
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

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var ability = Ability.GetAbility<Insider>(__instance);

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Insider) && tasksLeft == 0)
                Coroutines.Start(Utils.FlashCoroutine(ability.Color));
        }
    }
}