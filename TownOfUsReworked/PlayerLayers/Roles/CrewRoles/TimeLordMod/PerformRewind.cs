using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformRewind
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord))
                return false;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);
            
            if (__instance == role.RewindButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (!role.ButtonUsable)
                    return false;

                if (role.TimeLordRewindTimer() != 0f && !RecordRewind.rewinding)
                    return false;

                role.UsesLeft--;
                StartStop.StartRewind(role);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Rewind);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
        
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.RewindSound, false, 1f);
                } catch {}

                return false;
            }
            
            return false;
        }
    }
}