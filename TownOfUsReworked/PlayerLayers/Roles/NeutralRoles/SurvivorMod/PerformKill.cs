using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);

            if (!role.ButtonUsable)
                return false;

            var vestButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            if (__instance == vestButton)
            {
                if (__instance.isCoolingDown || !__instance.isActiveAndEnabled || role.VestTimer() != 0)
                    return false;

                role.TimeRemaining = CustomGameOptions.VestDuration;
                role.UsesLeft--;
                role.Vest();

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Vest, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                try
                {
                    SoundManager.Instance.PlaySound(TownOfUsReworked.VestSound, false, 1f);
                } catch {}

                return false;
            }

            return true;
        }
    }
}