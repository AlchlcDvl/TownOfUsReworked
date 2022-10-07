using HarmonyLib;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Modifiers.LoversMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;


            var flag3 = __instance.IsLover() && CustomGameOptions.BothLoversDie;
            if (!flag3) return true;
            var otherLover = Modifier.GetModifier<Lover>(__instance).OtherLover.Player;
            if (otherLover.Data.IsDead) return true;

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                otherLover.Exiled();
            }
            else if (AmongUsClient.Instance.AmHost && !otherLover.Is(RoleEnum.Pestilence)) Utils.RpcMurderPlayer(otherLover, otherLover);

            return true;
        }
    }
}