using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod
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

                    var role = Role.GetRole(player);
                    var objectifier = Objectifier.GetObjectifier(player);

                    if (CustomGameOptions.RevealerRevealsRoles)
                    {
                        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                        {
                            if (!player.Is(ObjectifierEnum.Traitor))
                                state.NameText.color = role.FactionColor;
                            else if (player.Is(ObjectifierEnum.Traitor))
                            {
                                if (CustomGameOptions.RevealerRevealsTraitor)
                                    state.NameText.color = role.FactionColor;
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
                            state.NameText.color = role.Color;
                        else if (player.Is(Faction.Crew) && CustomGameOptions.RevealerRevealsCrew)
                            state.NameText.color = role.Color;
                    }
                    else
                    {
                        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)) 
                        {
                            if ((player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor) || !player.Is(ObjectifierEnum.Traitor))
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                return;

            var role = Role.GetRole<Revealer>(PlayerControl.LocalPlayer);

            if (!role.CompletedTasks || role.Caught)
                return;

            if (MeetingHud.Instance)
            {
                UpdateMeeting(MeetingHud.Instance);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.RevealerFinished);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}