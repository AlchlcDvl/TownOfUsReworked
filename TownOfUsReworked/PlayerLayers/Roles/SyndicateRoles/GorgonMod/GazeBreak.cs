using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class GazeBreak
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Gorgon))
                return;
            
            var role = Role.GetRole<Gorgon>(__instance);

            if (LobbyBehaviour.Instance || MeetingHud.Instance)
                return;

            var breakList = new Queue<byte>();

            foreach (var stoned in role.gazeList)
            {
                if (GameData.Instance.GetPlayerById(stoned.Key).IsDead)
                    continue;
                    
                PlayerControl closestPlayer = null;
                System.Collections.Generic.List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray().ToList().FindAll(x =>
                    x.PlayerId != role.Player.PlayerId && x.PlayerId != stoned.Key);

                if (Utils.SetClosestPlayerToPlayer(GameData.Instance.GetPlayerById(stoned.Key)._object, ref closestPlayer, 0.8f, targets))
                    breakList.Enqueue(stoned.Key);
            }

            foreach (var breakQueue in breakList)
            {
                role.gazeList.Remove(breakQueue);
                Utils.RpcMurderPlayer(__instance, GameData.Instance.GetPlayerById(breakQueue)._object);
            }
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class freezeCantMove
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Gorgon))
                return;
                
            var role = Role.GetRole<Gorgon>(__instance);

            var breakList = new Queue<byte>();
            var keys = role.gazeList.Keys.ToArray();

            foreach (var key in keys)
            {
                if (GameData.Instance.GetPlayerById(key).IsDead)
                {
                    breakList.Enqueue(key);
                    continue;
                }

                if (key != PlayerControl.LocalPlayer.PlayerId)
                    continue;
                    
                role.gazeList[key] += Time.fixedDeltaTime;
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt();

                if (role.gazeList[key] >= CustomGameOptions.GazeTime)
                    breakList.Enqueue(key);
            }

            foreach (var breakQueue in breakList)
            {
                Utils.RpcMurderPlayer(__instance, GameData.Instance.GetPlayerById(breakQueue)._object);
                role.gazeList.Remove(breakQueue);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
                return;
                
            foreach (var role in Role.GetRoles(RoleEnum.Gorgon))
            {
                if (((Gorgon)role).gazeList.Count <= 0)
                    continue;

                foreach (var (key, _) in ((Gorgon)role).gazeList)
                    role.Player.MurderPlayer(GameData.Instance.GetPlayerById(key)._object);

                ((Gorgon)role).gazeList.Clear();
            }
        }
    }
}