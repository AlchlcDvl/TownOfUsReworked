using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;

namespace TownOfUsReworked.Patches
{
    //Vent and kill shit
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
    class ToggleHighlightPatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool active, [HarmonyArgument(1)] RoleTeamTypes team)
        {
            var player = PlayerControl.LocalPlayer;
            bool isActive = player != null  && !MeetingHud.Instance && (player.Is(RoleAlignment.NeutralKill) | player.Is(RoleEnum.Thief) |
                player.Is(Faction.Intruder) | player.Is(RoleEnum.Sheriff) | player.Is(RoleEnum.Altruist) | player.Is(RoleEnum.Amnesiac) |
                player.Is(RoleEnum.Cannibal) | player.Is(RoleEnum.Detective) | player.Is(RoleEnum.Dracula) | player.Is(RoleEnum.Dampyr) |
                player.Is(RoleEnum.VampireHunter) | player.Is(RoleEnum.Medic) | player.Is(RoleEnum.Shifter) | player.Is(RoleEnum.Tracker) |
                player.Is(RoleEnum.Vigilante) | player.Is(Faction.Syndicate) | player.Is(RoleEnum.Inspector) | player.Is(RoleEnum.Escort) |
                player.Is(RoleEnum.Troll));

            if (isActive)
            {
                var color = Role.GetRole(player).Color; 
                ((Renderer)__instance.cosmetics.currentBodySprite.BodySprite).material.SetColor("_OutlineColor", color);
            }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
    class SetVentOutlinePatch
    {
        public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
        {
            var player = PlayerControl.LocalPlayer;
            bool active = player != null && !MeetingHud.Instance && ((((player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) |
                (player.Is(RoleEnum.SerialKiller) && CustomGameOptions.SKVentOptions != SKVentOptions.Never) | (player.Is(RoleEnum.Arsonist) &&
                CustomGameOptions.ArsoVent)| (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent) | (player.Is(RoleEnum.Pestilence) &&
                CustomGameOptions.PestVent) | (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) | (player.Is(RoleEnum.Murderer) &&
                CustomGameOptions.MurdVent) | player.Is(RoleEnum.Engineer) | (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) |
                player.Is(RoleEnum.Miner) | player.Is(RoleEnum.Consigliere) | (player.Is(RoleEnum.Wraith) && CustomGameOptions.WraithVent) |
                (player.Is(RoleEnum.Morphling) && CustomGameOptions.MorphlingVent) | (player.Is(RoleEnum.Grenadier) && CustomGameOptions.GrenadierVent) |
                player.Is(RoleEnum.Janitor) | (player.Is(RoleEnum.Undertaker) && CustomGameOptions.UndertakerVentOptions != UndertakerOptions.Never) |
                player.Is(RoleEnum.Blackmailer) | player.Is(RoleEnum.Disguiser) | player.Is(RoleEnum.Camouflager) | (player.Is(RoleEnum.Poisoner) &&
                CustomGameOptions.PoisonerVent) | player.Is(RoleEnum.Impostor) | (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) |
                (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) | (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) |
                (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent) | (player.Is(RoleEnum.Dampyr) && CustomGameOptions.DampVent) |
                (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent)) && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default) |
                CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone);

            if (active)
            {
                var color = Role.GetRole(player).Color;
                ((Renderer)__instance.myRend).material.SetColor("_OutlineColor", color);
                ((Renderer)__instance.myRend).material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            }
        }
    }
}
