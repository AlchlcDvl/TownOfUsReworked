using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs
{
// vent and kill shit //
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
    class ToggleHighlightPatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool active, [HarmonyArgument(1)] RoleTeamTypes team)
        {
            var player = PlayerControl.LocalPlayer;
            bool isActive = PlayerControl.LocalPlayer != null  && !MeetingHud.Instance && (player.Is(RoleEnum.Glitch) ||
                player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Plaguebearer) || player.Is(RoleEnum.Arsonist) ||
                player.Is(RoleEnum.Pestilence) || player.Is(Faction.Crewmates) || player.Is(Faction.Intruders));

            if (isActive)
            {
                var role = Role.GetRole(player); 
                ((Renderer)__instance.cosmetics.currentBodySprite.BodySprite).material.SetColor("_OutlineColor", role.Color);
            }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
    class SetVentOutlinePatch
    {
        public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
        {
            //bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer._cachedData) && !MeetingHud.Instance;
            var player = PlayerControl.LocalPlayer;
            bool active = PlayerControl.LocalPlayer != null  && !MeetingHud.Instance &&
                ((player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) ||
                (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) ||
                (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) ||
                (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) ||
                (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) ||
                player.Is(RoleEnum.Engineer) || (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) ||
                player.Is(RoleEnum.Miner) || (player.Is(RoleEnum.Wraith) && CustomGameOptions.WraithVent) ||
                (player.Is(RoleEnum.Morphling) && CustomGameOptions.MorphlingVent) ||
                (player.Is(RoleEnum.Grenadier) && CustomGameOptions.GrenadierVent) ||
                player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Underdog) ||
                ((player.Is(RoleEnum.Undertaker) && CustomGameOptions.UndertakerVent) ||
                (player.Is(RoleEnum.Undertaker) && CustomGameOptions.UndertakerVentWithBody) ||
                (player.Is(RoleEnum.Undertaker) && CustomGameOptions.UndertakerVent && CustomGameOptions.UndertakerVentWithBody) ||
                player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Disguiser) ||
                player.Is(RoleEnum.Camouflager) || (player.Is(RoleEnum.Poisoner) && CustomGameOptions.PoisonerVent) ||
                player.Is(RoleEnum.Impostor) || (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) ||
                (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent) || player.Is(RoleEnum.Consigliere) ||
                (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent)));
                Color color = Role.GetRole(player).Color;

            if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
            {
                if (!active) return;
                ((Renderer)__instance.myRend).material.SetColor("_OutlineColor", color);
                ((Renderer)__instance.myRend).material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            }
            else if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
            {
                ((Renderer)__instance.myRend).material.SetColor("_OutlineColor", color);
                ((Renderer)__instance.myRend).material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            }
        }
    }
}
