using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformVest
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Survivor))
                return false;

            var role = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);

            if (__instance == role.VestButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (!role.ButtonUsable)
                    return false;

                if (role.VestTimer() != 0f)
                    return false;

                role.TimeRemaining = CustomGameOptions.VestDuration;
                role.UsesLeft--;
                role.Vest();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Vest);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.VestSound, false, 1f);
                } catch {}

                return false;
            }

            return true;
        }
    }
}