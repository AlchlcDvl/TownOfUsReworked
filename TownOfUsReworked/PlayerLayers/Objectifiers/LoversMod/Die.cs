using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.LoversMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public static class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;
            var flag3 = __instance.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie;

            if (!flag3)
                return true;

            var otherLover = Objectifier.GetObjectifier<Lovers>(__instance).OtherLover;

            if (otherLover.Data.IsDead)
                return true;

            if (!otherLover.Is(RoleEnum.Pestilence))
            {
                if (reason == DeathReason.Exile)
                    otherLover.Exiled();
                else if (AmongUsClient.Instance.AmHost)
                    Utils.RpcMurderPlayer(otherLover, otherLover);
            }

            return true;
        }
    }
}