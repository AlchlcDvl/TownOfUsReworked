using System.Collections;
using Hazel;
using System;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;


namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PuppeteerMod
{
    public class PuppeteerCoroutine
    {
        public static IEnumerator Possessing(PlayerControl target, Puppeteer puppeteer)
        {
            if ((DateTime.UtcNow - puppeteer.PossStart).TotalMilliseconds < CustomGameOptions.PossessCooldown * 1000.0f)
                yield break;

            puppeteer.PossStart = DateTime.UtcNow;
            var start = DateTime.UtcNow;
            puppeteer.possessStarting = true;

            while (true)
            {
                var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, target);
                var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

                if (!flag3)
                {
                    puppeteer.PossStart = DateTime.UtcNow.AddSeconds(-PlayerControl.GameOptions.KillCooldown);
                    puppeteer.possessStarting = false;
                    yield break;
                }

                if (MeetingHud.Instance != null)
                {
                    puppeteer.possessStarting = false;
                    yield break;
                }

                yield return new WaitForSeconds(0.016f);
                
                if ((DateTime.UtcNow - start).TotalMilliseconds >= CustomGameOptions.PossessCooldown * 1000.0f)
                    break;
            }

            puppeteer.possessStarting = false;
            puppeteer.PossStart = DateTime.UtcNow;
            
            puppeteer.PossessPlayer = target;
            puppeteer.PossessTime = 0;
            puppeteer.PossessButton.SetTarget(null);
            DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                
            puppeteer.PossessButton.graphic.sprite = Puppeteer.UnPossessSprite;
            puppeteer.Player.NetTransform.Halt();

            unchecked
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Possess,
                    SendOption.Reliable, -1);
                writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                writer2.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }
        }
    }
}