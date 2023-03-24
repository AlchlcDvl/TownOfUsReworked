using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Hazel;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.BountyHunter))
                return;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (role.GuessButton == null)
                role.GuessButton = Utils.InstantiateButton();

            if (role.HuntButton == null)
                role.HuntButton = Utils.InstantiateButton();

            role.GuessButton.UpdateButton(role, "GUESS", role.CheckTimer(), CustomGameOptions.BountyHunterCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", true,
                role.UsesLeft, role.ButtonUsable && !role.TargetFound, role.ButtonUsable);
            role.HuntButton.UpdateButton(role, "HUNT", role.CheckTimer(), CustomGameOptions.BountyHunterCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary",
                role.TargetFound);

            if (role.Failed && !role.Player.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnTroll);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TurnTroll();
            }
        }
    }
}