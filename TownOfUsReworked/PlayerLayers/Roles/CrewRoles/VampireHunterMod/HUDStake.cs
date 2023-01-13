using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDStake
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter);

            if (!flag8)
                return;
                
            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (role.VampsDead && !isDead)
            {
                role.TurnVigilante();

                unchecked
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TurnVigilante, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                return;
            }

            __instance.KillButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance && !isDead);
            __instance.KillButton.SetCoolDown(role.StakeTimer(), CustomGameOptions.StakeCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}
