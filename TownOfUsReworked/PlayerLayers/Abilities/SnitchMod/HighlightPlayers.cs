using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightPlayers
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
                        if (role.Faction == Faction.Intruder || role.Faction == Faction.Syndicate)
                        {
                            if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                                state.NameText.color = role.FactionColor;
                            else
                                state.NameText.color = role.Color;
                        }
                        else if (role.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                            state.NameText.color = role.Color;
                        else if (role.Faction == Faction.Crew && CustomGameOptions.SnitchSeesCrew)
                            state.NameText.color = role.Color;
                    }
                    else
                    {
                        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                        {
                            if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                                state.NameText.color = role.FactionColor;
                            else
                                state.NameText.color = role.FactionColor;
                        }
                        else if (role.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                            state.NameText.color = role.FactionColor;
                        else if (role.Faction == Faction.Crew && CustomGameOptions.SnitchSeesCrew)
                            state.NameText.color = role.FactionColor;
                    }
                }
            }
        }

        public static void Postfix()
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                return;

            var ability = Ability.GetAbility<Snitch>(PlayerControl.LocalPlayer);

            if (!ability.TasksDone)
                return;

            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting)
                UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var role = Role.GetRole(player);

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (role.Faction == Faction.Intruder || role.Faction == Faction.Syndicate)
                    {
                        if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                            player.nameText().color = role.FactionColor;
                        else
                            player.nameText().color = role.Color;
                    }
                    else if (role.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                        player.nameText().color = role.Color;
                    else if (role.Faction == Faction.Crew && CustomGameOptions.SnitchSeesCrew)
                        player.nameText().color = role.Color;
                }
                else
                {
                    if (player.Is(Faction.Intruder) || role.Faction == Faction.Syndicate)
                    {
                        if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                            player.nameText().color = role.FactionColor;
                        else
                            player.nameText().color = role.FactionColor;
                    }
                    else if (role.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                        player.nameText().color = role.FactionColor;
                    else if (role.Faction == Faction.Crew && CustomGameOptions.SnitchSeesCrew)
                        player.nameText().color = role.FactionColor;
                }
            }
        }
    }
}