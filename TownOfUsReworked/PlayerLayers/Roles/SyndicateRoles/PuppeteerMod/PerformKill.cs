using System;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PossessButtonKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Puppeteer))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Puppeteer>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;

            if ((role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000.0f + PlayerControl.GameOptions.KillCooldown > 0)
                return false;

            if (role.Player.killTimer > 0)
                return false;
            
            if (__instance.graphic.sprite == Puppeteer.PossessSprite)
            {
                if (target == null)
                    return false;

                if (role.duration > 0)
                    return false;

                Coroutines.Start(PuppeteerCoroutine.Possessing(target, role));
                return false;
            }

            if(__instance.graphic.sprite == Puppeteer.UnPossessSprite)
            {
                role.duration = CustomGameOptions.PossessDuration;
                
                unchecked
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.UnPossess,
                        SendOption.Reliable, -1);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                }
                
                role.UnPossess();
                return false;
            }

            role.lastPossess = DateTime.UtcNow;
            return true;
        }
    }
}
