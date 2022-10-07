using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SnitchMod
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
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = Role.GetRole(player);

                    if (CustomGameOptions.SnitchSeesRoles)
                    {
                        if (player.Is(Faction.Intruders) && CustomGameOptions.CustomImpColors)
                        {
                            if (player.Is(RoleEnum.Impostor)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Grenadier)) state.NameText.color = Patches.Colors.Grenadier;
                            if (player.Is(RoleEnum.Wraith)) state.NameText.color = Patches.Colors.Wraith;
                            if (player.Is(RoleEnum.Miner)) state.NameText.color = Patches.Colors.Miner;
                            if (player.Is(RoleEnum.Underdog)) state.NameText.color = Patches.Colors.Underdog;
                            if (player.Is(RoleEnum.Undertaker)) state.NameText.color = Patches.Colors.Undertaker;
                            if (player.Is(RoleEnum.Poisoner)) state.NameText.color = Patches.Colors.Poisoner;
                            if (player.Is(RoleEnum.Camouflager)) state.NameText.color = Patches.Colors.Camouflager;
                            if (player.Is(RoleEnum.Morphling)) state.NameText.color = Patches.Colors.Morphling;
                            if (player.Is(RoleEnum.Blackmailer)) state.NameText.color = Patches.Colors.Blackmailer;
                            if (player.Is(RoleEnum.Janitor)) state.NameText.color = Patches.Colors.Janitor;
                            if (player.Is(RoleEnum.Consigliere)) state.NameText.color = Patches.Colors.Consigliere;
                            if (player.Is(RoleEnum.Disguiser)) state.NameText.color = Patches.Colors.Disguiser;
                            if (player.Is(RoleEnum.TimeMaster)) state.NameText.color = Patches.Colors.TimeMaster;
                            if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) state.NameText.color = Patches.Colors.Traitor;
                        }
                        else
                        {
                            if (player.Is(RoleEnum.Impostor)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Grenadier)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Wraith)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Miner)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Underdog)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Undertaker)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Poisoner)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Camouflager)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Morphling)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Blackmailer)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Janitor)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Consigliere)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Disguiser)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.TimeMaster)) state.NameText.color = Patches.Colors.Impostor;
                            if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) state.NameText.color = Patches.Colors.Impostor;
                        }

                        if (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals)
                        {
                            if (player.Is(RoleEnum.Glitch)) state.NameText.color = Patches.Colors.Glitch;
                            if (player.Is(RoleEnum.Jester)) state.NameText.color = Patches.Colors.Jester;
                            if (player.Is(RoleEnum.Executioner)) state.NameText.color = Patches.Colors.Executioner;
                            if (player.Is(RoleEnum.GuardianAngel)) state.NameText.color = Patches.Colors.GuardianAngel;
                            if (player.Is(RoleEnum.Juggernaut)) state.NameText.color = Patches.Colors.Juggernaut;
                            if (player.Is(RoleEnum.Plaguebearer)) state.NameText.color = Patches.Colors.Plaguebearer;
                            if (player.Is(RoleEnum.Pestilence)) state.NameText.color = Patches.Colors.Pestilence;
                            if (player.Is(RoleEnum.Werewolf)) state.NameText.color = Patches.Colors.Werewolf;
                            if (player.Is(RoleEnum.Arsonist)) state.NameText.color = Patches.Colors.Arsonist;
                            if (player.Is(RoleEnum.Amnesiac)) state.NameText.color = Patches.Colors.Amnesiac;
                            if (player.Is(RoleEnum.Survivor)) state.NameText.color = Patches.Colors.Survivor;
                            if (player.Is(RoleEnum.Phantom)) state.NameText.color = Patches.Colors.Phantom;
                        }

                        if (player.Is(Faction.Crewmates) && CustomGameOptions.SnitchSeesCrew)
                        {
                            if (player.Is(RoleEnum.Crewmate)) state.NameText.color = Patches.Colors.Crew;
                            if (player.Is(RoleEnum.Sheriff)) state.NameText.color = Patches.Colors.Sheriff;
                            if (player.Is(RoleEnum.Engineer)) state.NameText.color = Patches.Colors.Engineer;
                            if (player.Is(RoleEnum.Swapper)) state.NameText.color = Patches.Colors.Swapper;
                            if (player.Is(RoleEnum.Investigator)) state.NameText.color = Patches.Colors.Investigator;
                            if (player.Is(RoleEnum.TimeLord)) state.NameText.color = Patches.Colors.TimeLord;
                            if (player.Is(RoleEnum.Medic)) state.NameText.color = Patches.Colors.Medic;
                            if (player.Is(RoleEnum.Agent)) state.NameText.color = Patches.Colors.Agent;
                            if (player.Is(RoleEnum.Snitch)) state.NameText.color = Patches.Colors.Snitch;
                            if (player.Is(RoleEnum.Altruist)) state.NameText.color = Patches.Colors.Altruist;
                            if (player.Is(RoleEnum.Vigilante)) state.NameText.color = Patches.Colors.Vigilante;
                            if (player.Is(RoleEnum.Veteran)) state.NameText.color = Patches.Colors.Veteran;
                            if (player.Is(RoleEnum.Haunter)) state.NameText.color = Patches.Colors.Haunter;
                            if (player.Is(RoleEnum.Tracker)) state.NameText.color = Patches.Colors.Tracker;
                            if (player.Is(RoleEnum.Transporter)) state.NameText.color = Patches.Colors.Transporter;
                            if (player.Is(RoleEnum.Medium)) state.NameText.color = Patches.Colors.Medium;
                            if (player.Is(RoleEnum.Mystic)) state.NameText.color = Patches.Colors.Mystic;
                            if (player.Is(RoleEnum.Operative)) state.NameText.color = Patches.Colors.Operative;
                            if (player.Is(RoleEnum.Detective)) state.NameText.color = Patches.Colors.Detective;
                            if (player.Is(RoleEnum.Shifter)) state.NameText.color = Patches.Colors.Shifter;
                            if (player.Is(RoleEnum.Mayor)) state.NameText.color = Patches.Colors.Mayor;
                        }
                    }
                    else
                    {
                        if (player.Is(Faction.Intruders)) 
                        {
                            if (!player.Is(RoleEnum.Traitor)) state.NameText.color = Patches.Colors.Impostor;

                            if (CustomGameOptions.SnitchSeesTraitor && player.Is(RoleEnum.Traitor)) state.NameText.color = Patches.Colors.Impostor;
                        }

                        if (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) state.NameText.color = Patches.Colors.Neutral;

                        if (player.Is(Faction.Crewmates) && CustomGameOptions.SnitchSeesCrew) state.NameText.color = Patches.Colors.Crew;
                    }
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting) UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (CustomGameOptions.SnitchSeesRoles) {}
                else
                {
                    if (player.Data.IsImpostor() && !player.Is(RoleEnum.Traitor)) player.nameText().color = Palette.ImpostorRed;
                    else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) player.nameText().color = Palette.ImpostorRed;
                    var playerRole = Role.GetRole(player);
                    if (playerRole.Faction == Faction.Neutral && CustomGameOptions.SnitchSeesNeutrals)
                    player.nameText().color = playerRole.Color;
                }
            }
        }
    }
}