﻿using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDStake
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter))
                return;

            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (role.StakeButton == null)
                role.StakeButton = Utils.InstantiateButton();

            role.StakeButton.UpdateButton(role, "STAKE", role.StakeTimer(), CustomGameOptions.StakeCooldown, AssetManager.Stake, AbilityTypes.Direct, "ActionSecondary");

            if (VampireHunter.VampsDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TurnVigilante();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnVigilante);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}
