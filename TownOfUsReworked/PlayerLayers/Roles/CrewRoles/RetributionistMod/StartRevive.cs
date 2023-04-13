using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => StartRevive.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class StartRevive
    {
        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;

            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected)
                return;

            if (exiled == PlayerControl.LocalPlayer)
                return;

            var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);
            Revive(ret, Utils.PlayerByVoteArea(VotingComplete.Imitate));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.RetributionistAction);
            writer.Write((byte)RetributionistActionsRPC.RetributionistRevive);
            writer.Write(ret.Player.PlayerId);
            writer.Write(ret.Revived.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
                return;

            if (obj.name.Contains("ExileCutscene"))
                ExileControllerPostfix(ConfirmEjects.lastExiled);
        }

        public static void Revive(Retributionist ret, PlayerControl revived)
        {
            if (revived == null)
                return;

            var role = Role.GetRole(revived);
            ret.RevivedRole = revived.Is(RoleEnum.Revealer) ? ((Revealer)role).FormerRole : role;
            ret.Revived = revived;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class VotingComplete
    {
        #pragma warning disable
        public static PlayerVoteArea Imitate;
        #pragma warning restore

        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
            {
                var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

                foreach (var button in ret.OtherButtons.Where(button => button != null))
                    button.SetActive(false);
            }
        }
    }
}