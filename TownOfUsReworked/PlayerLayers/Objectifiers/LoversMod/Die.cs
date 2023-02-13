using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.LoversMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
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

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                otherLover.Exiled();
            }
            else if (AmongUsClient.Instance.AmHost && !otherLover.Is(RoleEnum.Pestilence))
                Utils.RpcMurderPlayer(otherLover, otherLover);

            return true;
        }
    }
}