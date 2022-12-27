using HarmonyLib;
using System.Linq;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;


namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Puppeteer))
                return;
            
            Puppeteer role = Role.GetRole<Puppeteer>(__instance);

            if (role.PossessPlayer != null && (LobbyBehaviour.Instance || MeetingHud.Instance))
            {
                role.PossessPlayer = null;
                role.PossessTime = 0;

                unchecked
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.UnPossess,
                        SendOption.Reliable, -1);
                    writer2.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                }
                
                role.UnPossess();
                return;
            }

            if (role.PossessPlayer != null)
            {
                if (PlayerControl.LocalPlayer == __instance)
                {
                    role.PossessTime += Time.fixedDeltaTime;

                    if (role.PossessTime > CustomGameOptions.PossessDuration)
                    {
                        unchecked
                        {
                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.UnPossess,
                                SendOption.Reliable, -1);
                            writer2.Write(role.Player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        }

                        role.UnPossess();
                    }
                }
            }

            if (role.duration > 0)
            {
                if (role.PossessPlayer == null)
                {
                    __instance.moveable = false;
                    role.duration -= Time.fixedDeltaTime;
                }

                if (role.duration <= 0)
                    __instance.moveable = true;
            }
            
            if (role.PossessPlayer == PlayerControl.LocalPlayer)
            {
                PlayerControl closestPlayer = null;
                var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x.PlayerId != role.PossessPlayer.
                    PlayerId).ToList();

                if (Utils.SetClosestPlayer(ref closestPlayer, GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance] * 0.75f, targets))
                {
                    unchecked
                    {
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.PossessKill,
                            SendOption.Reliable, -1);
                        writer2.Write(role.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    }
                    
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, closestPlayer);
                    role.KillUnPossess();
                }
            }
        }
    }
}