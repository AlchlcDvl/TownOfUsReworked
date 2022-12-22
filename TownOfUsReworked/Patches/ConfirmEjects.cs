using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

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
            var flag = player.Is(RoleEnum.Altruist) | player.Is(RoleEnum.Agent) | player.Is(RoleEnum.Arsonist) | player.Is(RoleEnum.Amnesiac) |
                player.Is(RoleEnum.Engineer) | player.Is(RoleEnum.Escort) | player.Is(RoleEnum.Executioner) | player.Is(RoleEnum.Impostor) |
                player.Is(RoleEnum.Inspector) | player.Is(RoleEnum.Investigator) | player.Is(RoleEnum.Operative) | player.Is(RoleEnum.Undertaker);
            var factionflag = player.Is(Faction.Intruders) | player.Is(SubFaction.Undead);
            var a_or_an = flag ? "an" : "a";
            var a_or_an2 = factionflag ? "an" : "a";
            PlayerControl target = null;
            
            foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role2;

                if (player.PlayerId == exe.TargetPlayer.PlayerId)
                    target = exe.TargetPlayer;
            }

            if (role == null || !CustomGameOptions.ConfirmEjects)
            {
                if (CustomGameOptions.CustomEject)
                {
                    if ((byte)CustomGameOptions.Map == 0)
                        __instance.completeString = $"{player.name} is now one with space.";
                    else if ((byte)CustomGameOptions.Map == 1)
                        __instance.completeString = $"{player.name} is now experiencing fatal free fall.";
                    else if ((byte)CustomGameOptions.Map == 2)
                        __instance.completeString = $"{player.name} is now enjoying a hot bath.";
                    else if ((byte)CustomGameOptions.Map == 4)
                        __instance.completeString = $"{player.name} is now experiencing gravity.";
                    else if ((byte)CustomGameOptions.Map == 5)
                        __instance.completeString = $"{player.name} is now off to a scuba adventure.";
                }
                else
                    __instance.completeString = $"{player.name} was ejected.";
            }
            else
            {
                if (CustomGameOptions.EjectionRevealsRole)
                {
                    if (player.Is(RoleEnum.Jester) && CustomGameOptions.JestEjectScreen)
                        __instance.completeString = "You feel a sense of dread during this ejection. The <color=#F7B3DAFF>Jester</color> has won!";
                    else if (target != null && CustomGameOptions.ExeEjectScreen)
                        __instance.completeString = "You feel a sense of dread during the ejection. The <color=#CCCCCCFF>Executioner</color> has won!";
                    else
                        __instance.completeString = $"{player.name} was {a_or_an} {role.ColorString + role.Name}</color>.";
                }
                else
                {
                    if (!player.Is(SubFaction.Undead))
                        __instance.completeString = $"{player.name} was {a_or_an2} {role.FactionColorString + role.FactionName}</color>.";
                    else
                        __instance.completeString = $"{player.name} was {a_or_an2} {role.SubFactionColorString + role.SubFactionName}</color>.";
                }
            }

        }
    }
}