using System;
using HarmonyLib;
using Hazel;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PlaguebearerMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff);

            if (!flag)
                return true;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
            role.randomSheriffAccuracy = UnityEngine.Random.RandomRangeInt(0, 100);

            if (role.UsedThisRound)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove | role.ClosestPlayer == null)
                return false;

            var flag2 = role.InterrogateTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            var playerId = role.ClosestPlayer.PlayerId;
            role.UsedThisRound = true;

            if (role.ClosestPlayer.IsInfected() | role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() | role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (!role.Player.IsProtected())
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    return false;
                }

                role.LastInterrogated = DateTime.UtcNow;
                return false;
            }
            
            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Interrogate, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            role.Interrogated.Add(role.ClosestPlayer.PlayerId);
            role.LastInterrogated = DateTime.UtcNow;

            try
            {
                AudioClip SeerSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Seer.raw");
                SoundManager.Instance.PlaySound(SeerSFX, false, 0.4f);
            }
            catch {}
            
            return false;
        }
    }
}
