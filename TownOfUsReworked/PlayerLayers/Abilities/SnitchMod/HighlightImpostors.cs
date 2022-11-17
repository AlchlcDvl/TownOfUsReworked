using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId)
                        continue;

                    var role = Role.GetRole(player);
                    var ability = Ability.GetAbility(player);

                    if (CustomGameOptions.SnitchSeesRoles)
                    {
                        if (player.Is(Faction.Intruders) && CustomGameOptions.CustomImpColors)
                        {
                            if (!player.Is(ObjectifierEnum.Traitor))
                                state.NameText.color = role.Color;
                            else if (player.Is(ObjectifierEnum.Traitor))
                            {
                                if (CustomGameOptions.SnitchSeesTraitor)
                                    state.NameText.color = role.Color;
                                else
                                {
                                    foreach (var role2 in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                                    {
                                        var traitor = (Traitor)role2;
                                        state.NameText.color = traitor.former.Color;
                                    }
                                }
                            }
                        }
                        else if (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals)
                        {
                            state.NameText.color = role.Color;
                        }
                        else if (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew)
                        {
                            state.NameText.color = role.Color;
                        }
                    }
                    else
                    {
                        if (player.Is(Faction.Intruders)) 
                        {
                            if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                                state.NameText.color = role.FactionColor;
                            else if (!player.Is(ObjectifierEnum.Traitor))
                                state.NameText.color = role.FactionColor;
                        }
                        else if (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals)
                            state.NameText.color = Colors.Neutral;
                        else if (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew)
                            state.NameText.color = Colors.Crew;
                    }
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                return;

            var role = Ability.GetAbility<Snitch>(PlayerControl.LocalPlayer);

            if (!role.TasksDone)
                return;

            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting)
                UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (CustomGameOptions.SnitchSeesRoles)
                {

                }
                else
                {
                    var playerRole = Ability.GetAbility(player);

                    if (player.Data.IsImpostor() && !player.Is(ObjectifierEnum.Traitor))
                        player.nameText().color = Colors.Intruder;
                    else if (player.Data.IsImpostor() && player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                        player.nameText().color = Colors.Intruder;
                }
            }
        }
    }
}