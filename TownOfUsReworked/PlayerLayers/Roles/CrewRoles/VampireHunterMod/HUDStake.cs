using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using System.Linq;
using TownOfUsReworked.Patches;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDStake
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            UpdateStakeButton(__instance);
        }

        private static void UpdateStakeButton(HudManager __instance)
        {
            KillButton = __instance.KillButton;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter);
            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (flag8)
            {

                if (isDead)
                    KillButton.gameObject.SetActive(false);
                else
                {
                    KillButton.gameObject.SetActive(!MeetingHud.Instance);
                    KillButton.SetCoolDown(role.StakeTimer(), CustomGameOptions.VigiKillCd);
                    Utils.SetTarget(ref role.ClosestPlayer, KillButton);
                }
            }

            if (role.VampsDead && !isDead)
            {
                var turn = false;
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();

                if (alives.Count == 2)
                {
                    foreach (var player in alives)
                    {
                        if (player.Is(Faction.Intruders) | player.Is(RoleAlignment.NeutralKill) | player.Is(Faction.Syndicate))
                            turn = true;
                    }
                }
                else
                    turn = true;
                
                if (turn)
                {
                    role.TurnVigilante();

                    unchecked
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TurnVigilante,    
                            SendOption.Reliable, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
        }
    }
}
