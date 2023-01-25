/*using HarmonyLib;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.IsDead)
                return;
            
            __instance.KillButton.gameObject.SetActive(false);

            var player = PlayerControl.LocalPlayer;
            var role = Role.GetRole(player);

            if (role == null)
                return;

            if (role.IsBlocked)
            {
                __instance.SetHudActive(false);
                __instance.UseButton.gameObject.SetActive(false);
                __instance.ReportButton.gameObject.SetActive(false);
                __instance.MapButton.gameObject.SetActive(false);

                if (MapBehaviour.Instance)
                {
                    MapBehaviour.Instance.Close();
                    __instance.UseButton.gameObject.SetActive(false);
                    __instance.ReportButton.gameObject.SetActive(false);
                    __instance.MapButton.gameObject.SetActive(false);
                    __instance.KillButton.gameObject.SetActive(false);
                    __instance.AbilityButton.gameObject.SetActive(false);
                    __instance.AdminButton.gameObject.SetActive(false);
                
                    if (Utils.CanVent(player))
                        __instance.ImpostorVentButton.gameObject.SetActive(false);
                
                    if ((player.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage) || (player.Is(Faction.Syndicate) && Role.SyndicateHasChaosDrive))
                        __instance.SabotageButton.gameObject.SetActive(false);
                }

                if (Minigame.Instance)
                    Minigame.Instance.Close();
                
                if (Utils.CanVent(player))
                    __instance.ImpostorVentButton.gameObject.SetActive(false);
                
                if ((player.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage) || (player.Is(Faction.Syndicate) && Role.SyndicateHasChaosDrive))
                    __instance.SabotageButton.gameObject.SetActive(false);
            }
            else
            {
                __instance.SetHudActive(true);
                __instance.UseButton.gameObject.SetActive(true);
                __instance.ReportButton.gameObject.SetActive(true);
                __instance.MapButton.gameObject.SetActive(true);
                __instance.ImpostorVentButton.gameObject.SetActive(VentPatches.CanVent(player));
                
                if ((player.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage) || (player.Is(Faction.Syndicate) && Role.SyndicateHasChaosDrive))
                    __instance.SabotageButton.gameObject.SetActive(true);
            }
        }
    }
}*/