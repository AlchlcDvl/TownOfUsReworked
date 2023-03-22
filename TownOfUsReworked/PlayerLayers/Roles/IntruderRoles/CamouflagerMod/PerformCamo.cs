using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformCamo
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Camouflager))
                return true;

            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);

            if (__instance == role.CamouflageButton)
            {
                if (!Utils.ButtonUsable(role.CamouflageButton))
                    return false;

                if (role.CamouflageTimer() != 0f)
                    return false;

                if (CamouflageUnCamouflage.IsCamoed)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Camouflage);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                role.Camouflage();
                Utils.Camouflage();
                return false;
            }

            return true;
        }
    }
}