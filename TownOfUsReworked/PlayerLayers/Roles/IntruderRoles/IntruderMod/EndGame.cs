using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason == GameOverReason.ImpostorByKill || reason == GameOverReason.ImpostorBySabotage ||
                reason == GameOverReason.ImpostorByVote)
                return true;

            foreach (var role in Role.AllRoles)
            {
                if (role.Faction == Faction.Intruders)
                {
                    ((Blackmailer)role).Loses();
                    ((Camouflager)role).Loses();
                    ((Consigliere)role).Loses();
                    ((Consort)role).Loses();
                    ((Disguiser)role).Loses();
                    //((Godfather)role).Loses();
                    //((Mafioso)role).Loses();
                    ((Grenadier)role).Loses();
                    ((Impostor)role).Loses();
                    ((Janitor)role).Loses();
                    ((Miner)role).Loses();
                    ((Morphling)role).Loses();
                    ((Poisoner)role).Loses();
                    ((Teleporter)role).Loses();
                    ((TimeMaster)role).Loses();
                    ((Undertaker)role).Loses();
                    ((Wraith)role).Loses();
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.IntruderLose,
                SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            return true;
        }
    }
}