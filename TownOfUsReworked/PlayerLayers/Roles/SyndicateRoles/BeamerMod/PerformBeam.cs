using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using Hazel;
using Reactor.Utilities;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformBeam
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                return true;

            var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);

            if (__instance == role.SetBeamButton1)
            {
                if (role.BeamTimer() != 0f)
                    return false;

                role.OpenMenu1();
                return false;
            }
            else if (__instance == role.SetBeamButton2)
            {
                if (role.BeamTimer() != 0f)
                    return false;

                role.OpenMenu2();
                return false;
            }
            else if (__instance == role.BeamButton)
            {
                if (role.BeamTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Beam);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(role.BeamPlayers());
                role.LastBeamed = DateTime.UtcNow;
                return false;
            }

            return true;
        }
    }
}