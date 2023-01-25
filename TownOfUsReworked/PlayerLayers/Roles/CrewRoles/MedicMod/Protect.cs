using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Protect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Medic))
                return false;

            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Medic, role.ClosestPlayer, __instance) || __instance != role.ShieldButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer);
                return false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.Protect);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            role.ShieldedPlayer = role.ClosestPlayer;
            role.UsedAbility = true;
            
            try
            {
                //SoundManager.Instance.PlaySound(TownOfUsReworked.ProtectSound, false, 1f);
            } catch {}

            return false;
        }
    }
}
