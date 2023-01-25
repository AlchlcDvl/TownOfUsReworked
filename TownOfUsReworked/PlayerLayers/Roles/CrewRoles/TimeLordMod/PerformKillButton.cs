using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord))
                return false;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Escort, null, __instance) || __instance != role.RewindButton)
                return false;

            if (role.TimeLordRewindTimer() != 0f && !RecordRewind.rewinding && __instance == role.RewindButton && role.ButtonUsable)
                return false;

            role.UsesLeft--;
            StartStop.StartRewind(role);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.Rewind);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    
            try
            {
                //SoundManager.Instance.PlaySound(TownOfUsReworked.RewindSound, false, 1f);
            } catch {}
            
            return false;
        }
    }
}