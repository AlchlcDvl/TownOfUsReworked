using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.RevealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        public static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId)
                        continue;

                    var ability = Ability.GetAbility(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var role = Role.GetRole(player);

                    if (CustomGameOptions.RevealerRevealsRoles)
                    {
                        if (player.Is(Faction.Intruders))
                        {
                            if (!player.Is(ObjectifierEnum.Traitor))
                                state.NameText.color = role.Color;
                            else if (player.Is(ObjectifierEnum.Traitor))
                            {
                                if (CustomGameOptions.RevealerRevealsTraitor)
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
                        else if (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals)
                        {
                            state.NameText.color = role.Color;
                        }
                        else if (player.Is(Faction.Crew) && CustomGameOptions.RevealerRevealsCrew)
                        {
                            state.NameText.color = role.Color;
                        }
                    }
                    else
                    {
                        if (player.Is(Faction.Intruders)) 
                        {
                            if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor)
                                state.NameText.color = role.FactionColor;
                            else if (!player.Is(ObjectifierEnum.Traitor))
                                state.NameText.color = role.FactionColor;
                        }
                        else if (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals)
                            state.NameText.color = role.FactionColor;
                        else if (player.Is(Faction.Crew) && CustomGameOptions.RevealerRevealsCrew)
                            state.NameText.color = role.FactionColor;
                    }
                }
            }
        }
        
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Revealer))
                return;

            var ability = Ability.GetAbility<Revealer>(PlayerControl.LocalPlayer);

            if (!ability.CompletedTasks | ability.Caught)
                return;

            if (MeetingHud.Instance)
            {
                UpdateMeeting(MeetingHud.Instance);

                unchecked
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RevealerFinished,
                        SendOption.Reliable, -1);
                    writer.Write(ability.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}