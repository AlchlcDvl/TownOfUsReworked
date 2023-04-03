using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud))]
    public static class SetRevive
    {
        #pragma warning disable
        public static PlayerVoteArea Imitate;
        #pragma warning restore

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix()
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                {
                    var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

                    foreach (var button in ret.OtherButtons.Where(button => button != null))
                        button.SetActive(false);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RetributionistAction);
                    writer.Write((byte)RetributionistActionsRPC.RetributionistReviveSet);
                    writer.Write(ret.Player.PlayerId);

                    if (Imitate == null)
                        writer.Write(sbyte.MaxValue);
                    else
                    {
                        writer.Write(Imitate.TargetPlayerId);
                        ret.Revived = Utils.PlayerById(Imitate.TargetPlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}