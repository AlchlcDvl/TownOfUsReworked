using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GrenadierMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);

            if (__instance == role.FlashButton)
            {
                if (__instance.isCoolingDown)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);

                if (sabActive)
                    return false;

                if (role.FlashTimer() != 0)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.FlashGrenade);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.GrenadeDuration;
                
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.FlashSound, false, 1f);
                } catch {}

                role.Flash();
                return false;
            }

            return true;
        }
    }
}