using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CrewMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason == GameOverReason.HumansByVote || reason == GameOverReason.HumansByTask)
                return true;

            foreach (var role in Role.AllRoles)
            {
                if (role.Faction == Faction.Crew)
                {
                    ((Agent)role).Loses();
                    ((Altruist)role).Loses();
                    ((Coroner)role).Loses();
                    ((Crewmate)role).Loses();
                    ((Detective)role).Loses();
                    ((Engineer)role).Loses();
                    ((Escort)role).Loses();
                    ((Inspector)role).Loses();
                    ((Investigator)role).Loses();
                    ((Mayor)role).Loses();
                    ((Medic)role).Loses();
                    ((Medium)role).Loses();
                    ((Operative)role).Loses();
                    ((Sheriff)role).Loses();
                    ((Shifter)role).Loses();
                    ((Swapper)role).Loses();
                    ((TimeLord)role).Loses();
                    ((Transporter)role).Loses();
                    ((Tracker)role).Loses();
                    ((VampireHunter)role).Loses();
                    ((Veteran)role).Loses();
                    ((Vigilante)role).Loses();
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.CrewLose,
                SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            return true;
        }
    }
}