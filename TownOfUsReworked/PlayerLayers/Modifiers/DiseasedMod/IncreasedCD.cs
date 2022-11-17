using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.CowardMod
{
    public class IncreasedCD
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class PlayerControl_MurderPlayer
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                if (target.Is(ModifierEnum.Diseased))
                    __instance.SetKillTimer(PlayerControl.GameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier);
            }
        }
    }
}