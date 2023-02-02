using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.WraithMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Wraith);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Wraith>(PlayerControl.LocalPlayer);

            if (__instance == role.InvisButton)
            {
                if (__instance.isCoolingDown)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.InvisTimer() != 0)
                    return false;
                
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Invis);
                var position = PlayerControl.LocalPlayer.transform.position;
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.InvisDuration;
                role.Invis();
                //SoundManager.Instance.PlaySound(TownOfUsReworked.InvisSound, false, 0.4f);

                return false;
            }

            return true;
        }
    }
}