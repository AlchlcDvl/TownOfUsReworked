using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber))
                return true;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (__instance == role.BombButton)
            {
                if (role.BombTimer() != 0f)
                    return false;

                role.Bombs.Add(BombExtensions.CreateBomb(PlayerControl.LocalPlayer.GetTruePosition()));
                role.LastPlaced = DateTime.UtcNow;

                if (CustomGameOptions.BombCooldownsLinked)
                    role.LastDetonated = DateTime.UtcNow;

                return false;
            }
            else if (__instance == role.DetonateButton)
            {
                if (role.DetonateTimer() != 0f)
                    return false;

                if (role.Bombs.Count == 0)
                    return false;

                role.Bombs.DetonateBombs(role.PlayerName);
                role.LastDetonated = DateTime.UtcNow;

                if (CustomGameOptions.BombCooldownsLinked)
                    role.LastPlaced = DateTime.UtcNow;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Detonate);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}
