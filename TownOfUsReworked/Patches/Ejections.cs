using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class ConfirmEjects
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;

            if (exiled == null)
                return;

            var player = exiled.Object;
            var role = Role.GetRole(player);

            var flag = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Agent) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) ||
                player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Impostor) ||
                player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Operative) || player.Is(RoleEnum.Undertaker);
            var factionflag = player.Is(Faction.Intruder);
            var subfactionflag = player.Is(SubFaction.Undead);

            var a_or_an = flag ? "an" : "a";
            var a_or_an2 = factionflag ? "an" : "a";
            var a_or_an3 = subfactionflag ? "an" : "a";

            var totalEvilsCount = PlayerControl.AllPlayerControls.ToArray().Where(x => (!x.Is(Faction.Crew) && !x.Is(RoleAlignment.NeutralBen) &&
                !x.Is(RoleAlignment.NeutralEvil)) || x.IsRecruit()).Count();
            var totalEvilsRemaining = CustomGameOptions.GameMode == GameMode.AllAny ? "an unknown number of" : $"{totalEvilsCount}";
            var evils = totalEvilsCount > 1 ? "evils" : "evil";
            var IsAre = totalEvilsCount > 1 ? "are" : "is";
            var totalEvils = $"There {IsAre} {totalEvilsRemaining} <color=#FF0000FF>{evils}</color> remaining.";

            var ejectString = "";
            PlayerControl target = null;

            role.DeathReason = DeathReasonEnum.Ejected;
            role.KilledBy = " ";
            
            foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role2;

                if (exe.TargetPlayer == null)
                    continue;

                if (player.PlayerId == exe.TargetPlayer.PlayerId)
                    target = exe.TargetPlayer;
            }

            if (role == null || !CustomGameOptions.ConfirmEjects)
            {
                if (CustomGameOptions.CustomEject)
                {
                    if (PlayerControl.GameOptions.MapId == 0 || PlayerControl.GameOptions.MapId == 3)
                        ejectString = $"{player.name} is now one with space.";
                    else if (PlayerControl.GameOptions.MapId == 1)
                        ejectString = $"{player.name} is now experiencing fatal free fall.";
                    else if (PlayerControl.GameOptions.MapId == 2)
                        ejectString = $"{player.name} is now enjoying a hot bath.";
                    else if (PlayerControl.GameOptions.MapId == 4)
                        ejectString = $"{player.name} is now experiencing gravity.";
                    else if (PlayerControl.GameOptions.MapId == 5)
                        ejectString = $"{player.name} is now off to a scuba adventure.";
                }
                else
                    ejectString = $"{player.name} was ejected.";
                    
                __instance.completeString = ejectString;
            }
            else
            {
                if (CustomGameOptions.EjectionRevealsRole)
                {
                    if (player.Is(RoleEnum.Jester) && CustomGameOptions.JestEjectScreen)
                        ejectString = "You feel a sense of dread during this ejection. The <color=#F7B3DAFF>Jester</color> has won!";
                    else if (target != null && CustomGameOptions.ExeEjectScreen)
                        ejectString = "You feel a sense of dread during the ejection. The <color=#CCCCCCFF>Executioner</color> has won!";
                    else
                        ejectString = $"{player.name} was {a_or_an} {role.ColorString + role.Name}</color>.";
                }
                else
                {
                    if (player.Is(Faction.Crew) || player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                        ejectString = $"{player.name} was {a_or_an2} {role.FactionColorString + role.FactionName}</color>.";
                    else
                    {
                        if (!player.Is(SubFaction.None))
                            ejectString = $"{player.name} was {a_or_an3} {role.SubFactionColorString + role.SubFactionName}</color>.";
                        else
                            ejectString = $"{player.name} was {a_or_an2} {role.FactionColorString + role.FactionName}</color>.";
                    }
                }

                __instance.ImpostorText.text = totalEvils;
                __instance.completeString = ejectString;
            }
        }
    }
}