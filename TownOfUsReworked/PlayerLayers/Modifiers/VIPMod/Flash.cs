using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Modifiers.VIPMod
{
    public class Flash
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class PlayerControl_MurderPlayer
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                if (target.Is(ModifierEnum.VIP))
                {
                    var targetRole = Role.GetRole(target);

                    foreach (var player in PlayerControl.AllPlayerControls)
                        Coroutines.Start(Utils.FlashCoroutine(targetRole.Color));
                }
            }
        }
    }
}