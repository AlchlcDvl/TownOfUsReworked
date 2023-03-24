using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDProtect
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.GuardianAngel))
                return;

            var role = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (role.ProtectButton == null)
                role.ProtectButton = Utils.InstantiateButton();

            role.ProtectButton.UpdateButton(role, "PROTECT", role.ProtectTimer(), CustomGameOptions.ProtectCd, AssetManager.Protect, AbilityTypes.Effect, "ActionSecondary", null,
                role.ButtonUsable && role.TargetAlive, !role.Protecting, role.Protecting, role.TimeRemaining, CustomGameOptions.ProtectDuration, role.ButtonUsable, role.UsesLeft,
                CustomGameOptions.ProtectBeyondTheGrave);

            if (!role.TargetAlive && !role.Player.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSurv);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TurnSurv();
            }
        }
    }
}