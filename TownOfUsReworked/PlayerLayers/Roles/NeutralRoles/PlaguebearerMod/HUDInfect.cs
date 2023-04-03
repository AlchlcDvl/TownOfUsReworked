using System.Linq;
using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDInfect
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Plaguebearer))
                return;

            var role = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);

            if (role.InfectButton == null)
                role.InfectButton = CustomButtons.InstantiateButton();

            var notInfected = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.InfectedPlayers.Contains(player.PlayerId)).ToList();
            role.InfectButton.UpdateButton(role, "INFECT", role.InfectTimer(), CustomGameOptions.InfectCd, AssetManager.Infect, AbilityTypes.Direct, "ActionSecondary", notInfected);

            if (role.CanTransform && PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count > 1 && !role.Player.Data.IsDead)
            {
                role.TurnPestilence();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnPestilence);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}