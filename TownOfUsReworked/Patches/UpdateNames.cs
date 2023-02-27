using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateNames
    {
        [HarmonyPriority(Priority.Last)]
        private static void Postfix()
        {
            if (GameStates.IsLobby || PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (MeetingHud.Instance)
                UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
                (player.nameText().text, player.nameText().color) = UpdateGameName(player);
        }

        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in __instance.playerStates)
                (player.NameText.text, player.NameText.color) = UpdateGameName(player);
        }

        private static (string, Color) UpdateGameName(PlayerControl player)
        {
            if (player.nameText().text != player.name)
                player.nameText().text = player.name;
            
            var playerName = player.name;
            var blank = "";
            
            if (CustomGameOptions.NoNames || CamouflageUnCamouflage.IsCamoed)
                player.nameText().text = blank;
            
            var name = player.nameText().text;
            var color = Color.white;
            var info = player.AllPlayerInfo();
            var localinfo = PlayerControl.LocalPlayer.AllPlayerInfo();
            var roleRevealed = false;

            if (player.CanDoTasks() && (PlayerControl.LocalPlayer == player || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)))
            {
                if (info[0] != null)
                {
                    var role = info[0] as Role;
                    name += $"{(CustomGameOptions.NoNames || CamouflageUnCamouflage.IsCamoed ? playerName : blank)} ({role.TasksCompleted}/{role.TotalTasks})";
                    roleRevealed = true;
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var sheriff = localinfo[0] as Sheriff;

                if (sheriff.Interrogated.Contains(player.PlayerId))
                {
                    if (Utils.SeemsEvil(player))
                        color = Colors.Intruder;
                    else
                        color = Colors.Glitch;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var consigliere = localinfo[0] as Consigliere;

                if (consigliere.Investigated.Contains(player.PlayerId))
                {
                    var role = info[0] as Role;
                    roleRevealed = true;

                    if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";

                        if (consigliere.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                    {
                        color = role.FactionColor;
                        name += $"\n{role.FactionName}";
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && (CustomGameOptions.ShowShielded == ShieldOptions.Medic || CustomGameOptions.ShowShielded == ShieldOptions.SelfAndMedic))
            {
                var medic = info[0] as Medic;

                if (medic.ShieldedPlayer == player)
                    name += " <color=#006600FF>✚</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var arsonist = localinfo[0] as Arsonist;

                if (arsonist.DousedPlayers.Contains(player.PlayerId))
                {
                    name += " <color=#EE7600FF>Ξ</color>";
                    player.myRend().material.SetColor("_VisorColor", arsonist.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var plaguebearer = localinfo[0] as Plaguebearer;

                if (plaguebearer.InfectedPlayers.Contains(player.PlayerId))
                {
                    name += " <color=#CFFE61FF>ρ</color>";
                    player.myRend().material.SetColor("_VisorColor", plaguebearer.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var cryomaniac = localinfo[0] as Cryomaniac;

                if (cryomaniac.DousedPlayers.Contains(player.PlayerId))
                {
                    name += " <color=#642DEAFF>λ</color>";
                    player.myRend().material.SetColor("_VisorColor", cryomaniac.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Framer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var framer = localinfo[0] as Framer;

                if (framer.Framed.Contains(player.PlayerId))
                {
                    name += " <color=#00FFFFFF>ς</color>";
                    player.myRend().material.SetColor("_VisorColor", framer.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var executioner = localinfo[0] as Executioner;

                if (player == executioner.TargetPlayer)
                {
                    if (CustomGameOptions.ExeKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = executioner.Color;
                    
                    name += " <color=#CCCCCCFF>§</color>";
                    player.myRend().material.SetColor("_VisorColor", executioner.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guesser) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guesser = localinfo[0] as Guesser;

                if (player == guesser.TargetPlayer)
                {
                    color = guesser.Color;
                    name += " <color=#EEE5BEFF>π</color>";
                    player.myRend().material.SetColor("_VisorColor", guesser.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guardianAngel = localinfo[0] as GuardianAngel;

                if (player == guardianAngel.TargetPlayer)
                {
                    if (CustomGameOptions.GAKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = guardianAngel.Color;
                    
                    name += " <color=#FFFFFFFF>★</color>";
                    player.myRend().material.SetColor("_VisorColor", guardianAngel.Color);

                    if (player.IsProtected() && (CustomGameOptions.ShowProtect == ProtectOptions.GA || CustomGameOptions.ShowProtect == ProtectOptions.SelfAndGA))
                        name += " <color=#FFFFFFFF>η</color>";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var whisperer = localinfo[0] as Whisperer;

                if (whisperer.Persuaded.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#F995FCFF>Λ</color>";
                        roleRevealed = true;
                    }
                    else
                        color = whisperer.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", whisperer.Color);
                }
                else
                {
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        if (stats.Item1 != player.PlayerId)
                            continue;

                        var perc = stats.Item2;
                        var diff = (6, 106, 3);
                        var diffPerc = (byte)((100 - perc) / 100);

                        color = new Color32((byte)(255 - (diff.Item1 * diffPerc)), (byte)(255 - (diff.Item2 * diffPerc)), (byte)(255 - (diff.Item3 * diffPerc)), 255);
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var dracula = localinfo[0] as Dracula;

                if (dracula.Converted.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#7B8968FF>γ</color>";
                        roleRevealed = true;
                    }
                    else
                        color = dracula.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", dracula.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jackal) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var jackal = localinfo[0] as Jackal;

                if (jackal.Recruited.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#575657FF>$</color>";
                        roleRevealed = true;
                    }
                    else
                        color = jackal.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", jackal.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var necromancer = localinfo[0] as Necromancer;

                if (necromancer.Resurrected.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#E6108AFF>Σ</color>";
                        roleRevealed = true;
                    }
                    else
                        color = necromancer.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", necromancer.Color);
                }
            }

            if (PlayerControl.LocalPlayer.IsBitten() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var dracula = PlayerControl.LocalPlayer.GetDracula();

                if (dracula.Converted.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#7B8968FF>γ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }
                    else
                        color = dracula.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", dracula.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.IsRecruit() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var jackal = PlayerControl.LocalPlayer.GetJackal();

                if (jackal.Recruited.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#575657FF>$</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }
                    else
                        color = jackal.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", jackal.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.IsResurrected() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var necromancer = PlayerControl.LocalPlayer.GetNecromancer();

                if (necromancer.Resurrected.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#E6108AFF>Σ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }
                    else
                        color = necromancer.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", necromancer.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.IsPersuaded() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var whisperer = PlayerControl.LocalPlayer.GetWhisperer();

                if (whisperer.Persuaded.Contains(player.PlayerId) && player != PlayerControl.LocalPlayer)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#F995FCFF>Λ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }
                    else
                        color = whisperer.SubFactionColor;

                    player.myRend().material.SetColor("_VisorColor", whisperer.Color);
                }
                else
                {
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        if (stats.Item1 != player.PlayerId)
                            continue;

                        var perc = stats.Item2;
                        var diff = (6, 106, 3);
                        var diffPerc = (byte)((100 - perc) / 100);

                        color = new Color32((byte)(255 - (diff.Item1 * diffPerc)), (byte)(255 - (diff.Item2 * diffPerc)), (byte)(255 - (diff.Item3 * diffPerc)), 255);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var lover = localinfo[0] as Objectifier;
                var otherLover = PlayerControl.LocalPlayer.GetOtherLover();

                if (otherLover == player)
                {
                    if (CustomGameOptions.LoversRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }

                    name += $" {lover.GetColoredSymbol()}";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var rival = localinfo[0] as Objectifier;
                var otherRival = PlayerControl.LocalPlayer.GetOtherRival();

                if (otherRival == player)
                {
                    if (CustomGameOptions.LoversRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }

                    name += $" {rival.GetColoredSymbol()}";
                }
            }

            if (player == PlayerControl.LocalPlayer && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                if (player.IsShielded() && (CustomGameOptions.ShowShielded == ShieldOptions.Self || CustomGameOptions.ShowShielded == ShieldOptions.SelfAndMedic))
                    name += " <color=#006600FF>✚</color>";

                if (player.IsProtected() && (CustomGameOptions.ShowProtect == ProtectOptions.Self || CustomGameOptions.ShowProtect == ProtectOptions.SelfAndGA))
                    name += " <color=#FFFFFFFF>η</color>";

                if (player.IsBHTarget())
                    name += " <color=#B51E39FF>Θ</color>";

                if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
                    name += " <color=#CCCCCCFF>§</color>";

                if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
                    name += " <color=#FFFFFFFF>★</color>";

                if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
                    name += " <color=#EEE5BEFF>π</color>";

                if (player.IsBitten())
                    name += " <color=#7B8968FF>γ</color>";

                if (player.IsRecruit())
                    name += " <color=#575657FF>$</color>";

                if (player.IsResurrected())
                    name += " <color=#E6108AFF>Σ</color>";

                if (player.IsPersuaded())
                    name += " <color=#F995FCFF>Λ</color>";
            }

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)
            {
                if (player.IsShielded() && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
                    name += " <color=#006600FF>✚</color>";

                if (player.IsProtected() && CustomGameOptions.ShowProtect != ProtectOptions.Everyone)
                    name += " <color=#FFFFFFFF>η</color>";

                if (player.IsBHTarget())
                    name += " <color=#B51E39FF>Θ</color>";

                if (player.IsExeTarget())
                    name += " <color=#CCCCCCFF>§</color>";

                if (player.IsGATarget())
                    name += " <color=#FFFFFFFF>★</color>";

                if (player.IsGuessTarget())
                    name += " <color=#EEE5BEFF>π</color>";

                if (player.IsBitten())
                    name += " <color=#7B8968FF>γ</color>";

                if (player.IsRecruit())
                    name += " <color=#575657FF>$</color>";

                if (player.IsResurrected())
                    name += " <color=#E6108AFF>Σ</color>";

                if (player.IsPersuaded())
                    name += " <color=#F995FCFF>Λ</color>";

                var arsonists = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Arsonist).ToList();

                foreach (Arsonist arsonist in arsonists)
                {
                    if (arsonist.DousedPlayers.Contains(player.PlayerId))
                        name += " <color=#EE7600FF>Ξ</color>";
                }

                var plaguebearers = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Plaguebearer).ToList();

                foreach (Plaguebearer plaguebearer in plaguebearers)
                {
                    if (plaguebearer.InfectedPlayers.Contains(player.PlayerId))
                        name += " <color=#CFFE61FF>ρ</color>";
                }

                var cryomaniacs = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Cryomaniac).ToList();

                foreach (Cryomaniac cryomaniac in cryomaniacs)
                {
                    if (cryomaniac.DousedPlayers.Contains(player.PlayerId))
                        name += " <color=#642DEAFF>λ</color>";
                }

                var framers = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Framer).ToList();

                foreach (Framer framer in framers)
                {
                    if (framer.Framed.Contains(player.PlayerId))
                        name += " <color=#00FFFFFF>ς</color>";
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;
                    consigliere.Investigated.Clear();
                }
            }

            if (player.IsShielded() && CustomGameOptions.ShowShielded == ShieldOptions.Everyone && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
                name += " <color=#006600FF>✚</color>";

            if (player.IsProtected() && CustomGameOptions.ShowProtect == ProtectOptions.Everyone && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
                name += " <color=#FFFFFFFF>η</color>";

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var role = localinfo[0] as Role;

                if (role.TasksDone)
                {
                    if (CustomGameOptions.SnitchSeesRoles)
                    {
                        var role2 = info[0] as Role;

                        if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                            (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                        {
                            color = role2.Color;
                            name += $"\n{role2.Name}";
                            roleRevealed = true;
                        }
                    }
                    else
                    {
                        var role2 = info[0] as Role;

                        if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                            (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                        {
                            if (!(player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor))
                            {
                                color = role2.FactionColor;
                                name += $"\n{role2.FactionName}";
                            }
                            else
                            {
                                color = Colors.Crew;
                                name += "\nCrew";
                            }

                            roleRevealed = true;
                        }
                    }
                }
            }

            if (player.Is(AbilityEnum.Snitch) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) && player != PlayerControl.LocalPlayer)
            {
                var role = info[0] as Role;

                if (role.TasksDone || role.TasksLeft <= CustomGameOptions.SnitchTasksRemaining)
                {
                    var ability = info[2] as Ability;
                    color = ability.Color;
                    name += $"\n{ability.Name}";
                    roleRevealed = true;
                }
            }

            if (PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && player != PlayerControl.LocalPlayer && (player.GetFaction() == Faction.Intruder || player.GetFaction() ==
                Faction.Syndicate) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                if (info[0] != null)
                {
                    var role = info[0] as Role;

                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += $" {role.FactionColorString}ξ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.PlayerId))
                                consigliere.Investigated.Remove(player.PlayerId);
                        }
                    }
                    else
                        color = role.FactionColor;
                }
            }

            if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) || player == PlayerControl.LocalPlayer)
            {
                if (info[0] != null)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role.Name}";
                    roleRevealed = true;
                }

                if (info[3] != null)
                {
                    var objectifier = info[3] as Objectifier;
                    name += $" {objectifier.GetColoredSymbol()}";
                }
            }

            if (roleRevealed)
                player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);

            return (name, color);
        }

        private static (string, Color) UpdateGameName(PlayerVoteArea player)
        {
            var blank = "";
            var info = player.AllPlayerInfo();
            var playerName = (info[0] as Role)?.PlayerName;

            if (player.NameText.text != playerName)
                player.NameText.text = playerName;
            
            if (CamouflageUnCamouflage.IsCamoed && CustomGameOptions.MeetingColourblind)
                player.NameText.text = blank;
            
            var name = player.NameText.text;
            var color = Color.white;
            var localinfo = PlayerControl.LocalPlayer.AllPlayerInfo();
            var roleRevealed = false;

            if (CustomGameOptions.Whispers && !(CamouflageUnCamouflage.IsCamoed && CustomGameOptions.MeetingColourblind))
                name = $"[{player.TargetPlayerId}] " + name;

            if (player.CanDoTasks() && (PlayerControl.LocalPlayer.PlayerId == player.TargetPlayerId || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)))
            {
                if (info[0] != null)
                {
                    var role = info[0] as Role;
                    name += $"{(CamouflageUnCamouflage.IsCamoed && CustomGameOptions.MeetingColourblind ? playerName : blank)} ({role.TasksCompleted}/{role.TotalTasks})";
                    roleRevealed = true;
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var sheriff = localinfo[0] as Sheriff;

                if (sheriff.Interrogated.Contains(player.TargetPlayerId))
                {
                    if (Utils.SeemsEvil(player))
                        color = Colors.Intruder;
                    else
                        color = Colors.Glitch;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var coroner = localinfo[0] as Coroner;

                if (coroner.Reported.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role.Name}";
                    roleRevealed = true;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var consigliere = localinfo[0] as Consigliere;

                if (consigliere.Investigated.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    roleRevealed = true;

                    if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
                    }
                    else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                    {
                        color = role.FactionColor;
                        name += $"\n{role.FactionName}";
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && (CustomGameOptions.ShowShielded == ShieldOptions.Medic || CustomGameOptions.ShowShielded == ShieldOptions.SelfAndMedic))
            {
                var medic = info[0] as Medic;

                if (medic.ShieldedPlayer == Utils.PlayerByVoteArea(player))
                    name += " <color=#006600FF>✚</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var arsonist = localinfo[0] as Arsonist;

                if (arsonist.DousedPlayers.Contains(player.TargetPlayerId))
                    name += " <color=#EE7600FF>Ξ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var plaguebearer = localinfo[0] as Plaguebearer;

                if (plaguebearer.InfectedPlayers.Contains(player.TargetPlayerId))
                    name += " <color=#CFFE61FF>ρ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var cryomaniac = localinfo[0] as Cryomaniac;

                if (cryomaniac.DousedPlayers.Contains(player.TargetPlayerId))
                    name += " <color=#642DEAFF>λ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Framer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var framer = localinfo[0] as Framer;

                if (framer.Framed.Contains(player.TargetPlayerId))
                    name += " <color=#00FFFFFF>ς</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var executioner = localinfo[0] as Executioner;

                if (player == executioner.TargetPlayer)
                {
                    if (CustomGameOptions.ExeKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = executioner.Color;
                    
                    name += " <color=#CCCCCCFF>§</color>";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guesser) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guesser = localinfo[0] as Guesser;

                if (player == guesser.TargetPlayer)
                {
                    color = guesser.Color;
                    name += " <color=#EEE5BEFF>π</color>";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guardianAngel = localinfo[0] as GuardianAngel;

                if (player == guardianAngel.TargetPlayer)
                {
                    if (CustomGameOptions.GAKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = guardianAngel.Color;
                    
                    name += " <color=#FFFFFFFF>★</color>";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var whisperer = localinfo[0] as Whisperer;

                if (whisperer.Persuaded.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#F995FCFF>Λ</color>";
                        roleRevealed = true;
                    }
                    else
                        color = whisperer.SubFactionColor;
                }
                else
                {
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        if (stats.Item1 != player.TargetPlayerId)
                            continue;

                        var perc = stats.Item2;
                        var diff = (6, 106, 3);
                        var diffPerc = (byte)((100 - perc) / 100f);

                        color = new Color32((byte)(255 - (diff.Item1 * diffPerc)), (byte)(255 - (diff.Item2 * diffPerc)), (byte)(255 - (diff.Item3 * diffPerc)), 255);
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var dracula = localinfo[0] as Dracula;

                if (dracula.Converted.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#7B8968FF>γ</color>";
                        roleRevealed = true;
                    }
                    else
                        color = dracula.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jackal) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var jackal = localinfo[0] as Jackal;

                if (jackal.Recruited.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#575657FF>$</color>";
                        roleRevealed = true;
                    }
                    else
                        color = jackal.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var necromancer = localinfo[0] as Necromancer;

                if (necromancer.Resurrected.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#E6108AFF>Σ</color>";
                        roleRevealed = true;
                    }
                    else
                        color = necromancer.SubFactionColor;
                }
            }

            if (PlayerControl.LocalPlayer.IsBitten() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var dracula = PlayerControl.LocalPlayer.GetDracula();

                if (dracula.Converted.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#7B8968FF>γ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                    else
                        color = dracula.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.IsRecruit() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var jackal = PlayerControl.LocalPlayer.GetJackal();

                if (jackal.Recruited.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#575657FF>$</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                    else
                        color = jackal.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.IsResurrected() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var necromancer = PlayerControl.LocalPlayer.GetNecromancer();

                if (necromancer.Resurrected.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#E6108AFF>Σ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                    else
                        color = necromancer.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.IsPersuaded() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var whisperer = PlayerControl.LocalPlayer.GetWhisperer();

                if (whisperer.Persuaded.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += " <color=#F995FCFF>Λ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                    else
                        color = whisperer.SubFactionColor;
                }
                else
                {
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        if (stats.Item1 != player.TargetPlayerId)
                            continue;

                        var perc = stats.Item2;
                        var diff = (6, 106, 3);
                        var diffPerc = (byte)((100 - perc) / 100);

                        color = new Color32((byte)(255 - (diff.Item1 * diffPerc)), (byte)(255 - (diff.Item2 * diffPerc)), (byte)(255 - (diff.Item3 * diffPerc)), 255);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var lover = localinfo[0] as Objectifier;
                var otherLover = PlayerControl.LocalPlayer.GetOtherLover();

                if (otherLover == player)
                {
                    if (CustomGameOptions.LoversRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }

                    name += $" {lover.GetColoredSymbol()}";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var rival = localinfo[0] as Objectifier;
                var otherRival = PlayerControl.LocalPlayer.GetOtherRival();

                if (otherRival == player)
                {
                    if (CustomGameOptions.LoversRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }

                    name += $" {rival.GetColoredSymbol()}";
                }
            }

            if (player.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && !player.AmDead)
            {
                if (player.IsShielded() && (CustomGameOptions.ShowShielded == ShieldOptions.Self || CustomGameOptions.ShowShielded == ShieldOptions.SelfAndMedic))
                    name += " <color=#006600FF>✚</color>";

                if (player.IsBHTarget())
                    name += " <color=#B51E39FF>Θ</color>";

                if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
                    name += " <color=#CCCCCCFF>§</color>";

                if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
                    name += " <color=#FFFFFFFF>★</color>";

                if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
                    name += " <color=#EEE5BEFF>π</color>";

                if (player.IsBitten())
                    name += " <color=#7B8968FF>γ</color>";

                if (player.IsRecruit())
                    name += " <color=#575657FF>$</color>";

                if (player.IsResurrected())
                    name += " <color=#E6108AFF>Σ</color>";

                if (player.IsPersuaded())
                    name += " <color=#F995FCFF>Λ</color>";
            }

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)
            {
                if (player.IsShielded() && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
                    name += " <color=#006600FF>✚</color>";

                if (player.IsBHTarget())
                    name += " <color=#B51E39FF>Θ</color>";

                if (player.IsExeTarget())
                    name += " <color=#CCCCCCFF>§</color>";

                if (player.IsGATarget())
                    name += " <color=#FFFFFFFF>★</color>";

                if (player.IsGuessTarget())
                    name += " <color=#EEE5BEFF>π</color>";

                if (player.IsBitten())
                    name += " <color=#7B8968FF>γ</color>";

                if (player.IsRecruit())
                    name += " <color=#575657FF>$</color>";

                if (player.IsResurrected())
                    name += " <color=#E6108AFF>Σ</color>";

                if (player.IsPersuaded())
                    name += " <color=#F995FCFF>Λ</color>";

                var arsonists = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Arsonist).ToList();

                foreach (Arsonist arsonist in arsonists)
                {
                    if (arsonist.DousedPlayers.Contains(player.TargetPlayerId))
                        name += " <color=#EE7600FF>Ξ</color>";
                }

                var plaguebearers = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Plaguebearer).ToList();

                foreach (Plaguebearer plaguebearer in plaguebearers)
                {
                    if (plaguebearer.InfectedPlayers.Contains(player.TargetPlayerId))
                        name += " <color=#CFFE61FF>ρ</color>";
                }

                var cryomaniacs = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Cryomaniac).ToList();

                foreach (Cryomaniac cryomaniac in cryomaniacs)
                {
                    if (cryomaniac.DousedPlayers.Contains(player.TargetPlayerId))
                        name += " <color=#642DEAFF>λ</color>";
                }

                var framers = Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.Framer).ToList();

                foreach (Framer framer in framers)
                {
                    if (framer.Framed.Contains(player.TargetPlayerId))
                        name += " <color=#00FFFFFF>ς</color>";
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;
                    consigliere.Investigated.Clear();
                }
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && CustomGameOptions.SnitchSeestargetsInMeeting && !(PlayerControl.LocalPlayer.Data.IsDead &&
                CustomGameOptions.DeadSeeEverything) && player != PlayerControl.LocalPlayer)
            {
                var role = localinfo[0] as Role;

                if (role.TasksDone)
                {
                    if (CustomGameOptions.SnitchSeesRoles)
                    {
                        var role2 = info[0] as Role;

                        if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                            (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                        {
                            color = role2.Color;
                            name += $"\n{role2.Name}";
                            roleRevealed = true;
                        }
                    }
                    else
                    {
                        var role2 = info[0] as Role;

                        if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                            (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                        {
                            if (!(player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor))
                            {
                                color = role2.FactionColor;
                                name += $"\n{role2.FactionName}";
                            }
                            else
                            {
                                color = Colors.Crew;
                                name += "\nCrew";
                            }

                            roleRevealed = true;
                        }
                    }
                }
            }

            if (player.Is(AbilityEnum.Snitch))
            {
                var role = info[0] as Role;

                if (role.TasksDone)
                {
                    var ability = info[2] as Ability;
                    color = ability.Color;
                    name += $"\n{ability.Name}";
                    roleRevealed = true;
                }
            }

            if (PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && player.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId && (player.GetFaction() == Faction.Intruder ||
                player.GetFaction() == Faction.Syndicate) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                if (info[0] != null)
                {
                    var role = info[0] as Role;

                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
                        name += $" {role.FactionColorString}ξ</color>";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                    else
                        color = role.FactionColor;
                    
                    if (player.Is(ObjectifierEnum.Traitor))
                    {
                        var objectifier = info[3] as Objectifier;
                        name += $" {objectifier.GetColoredSymbol()}";
                    }
                }
            }

            if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) || player.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                if (info[0] != null)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role.Name}";
                    roleRevealed = true;
                }

                if (info[3] != null)
                {
                    var objectifier = info[3] as Objectifier;
                    name += $" {objectifier.GetColoredSymbol()}";
                }
            }

            if (roleRevealed)
                player.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

            return (name, color);
        }
    }
}