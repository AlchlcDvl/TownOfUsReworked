using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class UpdateNames
    {
        public static readonly Dictionary<byte, string> PlayerNames = new();

        public static void Postfix()
        {
            if (ConstantVariables.Inactive || ConstantVariables.IsHnS)
                return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!PlayerNames.ContainsKey(player.PlayerId))
                    PlayerNames.Add(player.PlayerId, player.Data.PlayerName);

                (player.NameText().text, player.NameText().color) = UpdateGameName(player);
            }
        }

        private static (string, Color) UpdateGameName(PlayerControl player)
        {
            if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis && player != PlayerControl.LocalPlayer)
                return ("", Color.clear);

            var name = CustomGameOptions.NoNames ? "" : player.Data.PlayerName;
            var color = Color.white;
            var info = player.AllPlayerInfo();
            var localinfo = PlayerControl.LocalPlayer.AllPlayerInfo();
            var roleRevealed = false;

            if (DoUndo.IsCamoed && player != PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !ConstantVariables.IsLobby)
                name = Utils.GetRandomisedName();
            else
                name = CustomGameOptions.NoNames ? "" : PlayerNames.FirstOrDefault(x => x.Key == player.PlayerId).Value;

            if (info[0] == null || localinfo[0] == null)
                return (name, color);

            if (player.CanDoTasks() && (PlayerControl.LocalPlayer == player || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)))
            {
                var role = info[0] as Role;
                name += $" ({role.TasksCompleted}/{role.TotalTasks})";
                roleRevealed = true;
            }

            if (player.IsKnighted())
                name += "<color=#FF004EFF>κ</color>";

            if (player.Is(RoleEnum.Mayor) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) && PlayerControl.LocalPlayer.PlayerId != player.PlayerId)
            {
                var mayor = info[0] as Mayor;

                if (mayor.Revealed)
                {
                    roleRevealed = true;
                    name += $"\n{mayor.Name}";
                    color = mayor.Color;

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.PlayerId))
                            consigliere.Investigated.Remove(player.PlayerId);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.PlayerId))
                            inspector.Inspected.Remove(player.PlayerId);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.PlayerId))
                            retributionist.Inspected.Remove(player.PlayerId);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedGodfather) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var godfather = localinfo[0] as PromotedGodfather;

                if (godfather.IsConsig)
                {
                    if (godfather.Investigated.Contains(player.PlayerId))
                    {
                        var role = info[0] as Role;
                        roleRevealed = true;

                        if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                        {
                            color = role.Color;
                            name += $"\n{role.Name}";

                            if (godfather.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                                godfather.Investigated.Remove(player.PlayerId);
                        }
                        else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                        {
                            color = role.FactionColor;
                            name += $"\n{role.FactionName}";
                        }
                    }
                }
                else if (godfather.IsBM)
                {
                    if (godfather.BlackmailedPlayer == player)
                    {
                        name += " <color=#02A752FF>Φ</color>";
                        player.MyRend().material.SetColor("_VisorColor", godfather.Color);
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                var medic = localinfo[0] as Medic;

                if (medic.ShieldedPlayer != null && medic.ShieldedPlayer == player && (int)CustomGameOptions.ShowShielded is 1 or 2)
                    name += " <color=#006600FF>✚</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
            {
                var ret = localinfo[0] as Retributionist;

                if (ret.IsMedic)
                {
                    if (ret.ShieldedPlayer != null && ret.ShieldedPlayer == player && (int)CustomGameOptions.ShowShielded is 1 or 2)
                        name += " <color=#006600FF>✚</color>";
                }
                else if (ret.IsInsp)
                {
                    if (ret.Inspected.Contains(player.PlayerId))
                    {
                        name += $"\n{player.GetInspResults()}";
                        color = ret.Color;
                        roleRevealed = true;
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var arsonist = localinfo[0] as Arsonist;

                if (arsonist.DousedPlayers.Contains(player.PlayerId))
                {
                    name += " <color=#EE7600FF>Ξ</color>";
                    player.MyRend().material.SetColor("_VisorColor", arsonist.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var plaguebearer = localinfo[0] as Plaguebearer;

                if (plaguebearer.InfectedPlayers.Contains(player.PlayerId))
                {
                    name += " <color=#CFFE61FF>ρ</color>";
                    player.MyRend().material.SetColor("_VisorColor", plaguebearer.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var cryomaniac = localinfo[0] as Cryomaniac;

                if (cryomaniac.DousedPlayers.Contains(player.PlayerId))
                {
                    name += " <color=#642DEAFF>λ</color>";
                    player.MyRend().material.SetColor("_VisorColor", cryomaniac.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Framer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var framer = localinfo[0] as Framer;

                if (framer.Framed.Contains(player.PlayerId))
                {
                    name += " <color=#00FFFFFF>ς</color>";
                    player.MyRend().material.SetColor("_VisorColor", framer.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var executioner = localinfo[0] as Executioner;

                if (player == executioner.TargetPlayer)
                {
                    name += " <color=#CCCCCCFF>§</color>";

                    if (CustomGameOptions.ExeKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = executioner.Color;

                    player.MyRend().material.SetColor("_VisorColor", executioner.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guesser) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guesser = localinfo[0] as Guesser;

                if (player == guesser.TargetPlayer)
                {
                    color = guesser.Color;
                    name += " <color=#EEE5BEFF>π</color>";
                    player.MyRend().material.SetColor("_VisorColor", guesser.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guardianAngel = localinfo[0] as GuardianAngel;

                if (player == guardianAngel.TargetPlayer)
                {
                    name += " <color=#FFFFFFFF>★</color>";

                    if (player.IsProtected() && (int)CustomGameOptions.ShowProtect is 1 or 2)
                        name += " <color=#FFFFFFFF>η</color>";

                    if (CustomGameOptions.GAKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = guardianAngel.Color;

                    player.MyRend().material.SetColor("_VisorColor", guardianAngel.Color);
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
                        name += $" <color=#F995FCFF>Λ</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = whisperer.SubFactionColor;

                    player.MyRend().material.SetColor("_VisorColor", Colors.Sect);
                }
                else
                {
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        var color2 = (int)(stats.Item2 / 100f * 256);

                        if (color2 > 0 && player.PlayerId == stats.Item1)
                            color = new(255, 255, color2, 255);
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
                        name += $" <color=#7B8968FF>γ</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = dracula.SubFactionColor;

                    player.MyRend().material.SetColor("_VisorColor", Colors.Undead);
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
                        name += $" <color=#575657FF>$</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = jackal.SubFactionColor;

                    player.MyRend().material.SetColor("_VisorColor", Colors.Cabal);
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
                        name += $" <color=#E6108AFF>Σ</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = necromancer.SubFactionColor;

                    player.MyRend().material.SetColor("_VisorColor", Colors.Reanimated);
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
                        name += $" <color=#7B8968FF>γ</color>\n{role.Name}";
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

                    player.MyRend().material.SetColor("_VisorColor", Colors.Undead);
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
                        name += $" <color=#575657FF>$</color>\n{role.Name}";
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

                    player.MyRend().material.SetColor("_VisorColor", Colors.Cabal);
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
                        name += $" <color=#E6108AFF>Σ</color>\n{role.Name}";
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

                    player.MyRend().material.SetColor("_VisorColor", Colors.Reanimated);
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
                        name += $" <color=#F995FCFF>Λ</color>\n{role.Name}";
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

                    player.MyRend().material.SetColor("_VisorColor", Colors.Sect);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer))
            {
                var blackmailer = localinfo[0] as Blackmailer;

                if (blackmailer.BlackmailedPlayer == player)
                {
                    name += " <color=#02A752FF>Φ</color>";
                    color = blackmailer.Color;
                    player.MyRend().material.SetColor("_VisorColor", blackmailer.Color);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
            {
                var inspector = localinfo[0] as Inspector;

                if (inspector.Inspected.Contains(player.PlayerId))
                {
                    name += $"\n{player.GetInspResults()}";
                    color = inspector.Color;
                    roleRevealed = true;
                }
            }

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var lover = localinfo[3] as Objectifier;
                var otherLover = PlayerControl.LocalPlayer.GetOtherLover();

                if (otherLover == player)
                {
                    name += $" {lover.ColoredSymbol}";

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
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var rival = localinfo[3] as Objectifier;
                var otherRival = PlayerControl.LocalPlayer.GetOtherRival();

                if (otherRival == player)
                {
                    name += $" {rival.ColoredSymbol}";

                    if (CustomGameOptions.RivalsRoles)
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
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Mafia) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var mafia = localinfo[3] as Mafia;

                if (player.Is(ObjectifierEnum.Mafia) && player != PlayerControl.LocalPlayer)
                {
                    name += $" {mafia.ColoredSymbol}";

                    if (CustomGameOptions.MafiaRoles)
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
                }
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var role = localinfo[0] as Role;

                if (role.TasksDone)
                {
                    var role2 = info[0] as Role;

                    if (CustomGameOptions.SnitchSeesRoles)
                    {
                        if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                            (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                        {
                            color = role2.Color;
                            name += $"\n{role2.Name}";
                            roleRevealed = true;
                        }
                    }
                    else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) ||
                        (player.Is(Faction.Crew) && CustomGameOptions.SnitchSeesCrew))
                    {
                        if (!(player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) && !(player.Is(ObjectifierEnum.Fanatic) && CustomGameOptions.SnitchSeesFanatic))
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

            if (player.Is(AbilityEnum.Snitch) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) && player != PlayerControl.LocalPlayer)
            {
                var role = info[0] as Role;

                if (role.TasksDone || role.TasksLeft <= CustomGameOptions.SnitchTasksRemaining)
                {
                    var ability = info[2] as Ability;
                    color = ability.Color;
                    name += (name.Contains('\n') ? " " : "\n") + ability.Name;
                    roleRevealed = true;
                }
            }

            if (PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && player != PlayerControl.LocalPlayer && (player.GetFaction() == Faction.Intruder || player.GetFaction() ==
                Faction.Syndicate) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var role = info[0] as Role;

                if (CustomGameOptions.FactionSeeRoles)
                {
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
                else
                    color = role.FactionColor;

                if (player.SyndicateSided() || player.IntruderSided())
                {
                    var objectifier = info[3] as Objectifier;
                    name += $" {objectifier.ColoredSymbol}";
                }
                else
                    name += $" {role.FactionColorString}ξ</color>";
            }

            if (PlayerControl.LocalPlayer.Is(Faction.Syndicate) && player == Role.DriveHolder)
                name += " <color=#008000FF>Δ</color>";

            if (Role.GetRoles<Revealer>(RoleEnum.Revealer).Any(x => x.CompletedTasks) && PlayerControl.LocalPlayer.Is(Faction.Crew))
            {
                var role = info[0] as Role;

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew)
                        && CustomGameOptions.RevealerRevealsCrew))
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                }
                else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew)
                    && CustomGameOptions.RevealerRevealsCrew))
                {
                    if (!(player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor) && !(player.Is(ObjectifierEnum.Fanatic) && CustomGameOptions.RevealerRevealsFanatic))
                    {
                        color = role.FactionColor;
                        name += $"\n{role.FactionName}";
                    }
                    else
                    {
                        color = Colors.Crew;
                        name += "\nCrew";
                    }

                    roleRevealed = true;
                }
            }

            if (player == PlayerControl.LocalPlayer && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                if ((player.IsShielded() || player.IsRetShielded()) && (int)CustomGameOptions.ShowShielded is 0 or 2)
                    name += " <color=#006600FF>✚</color>";

                if (player.IsProtected() && (int)CustomGameOptions.ShowProtect is 0 or 2)
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
                if ((player.IsShielded() || player.IsRetShielded()) && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
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

                if (player == Role.DriveHolder)
                    name += " <color=#008000FF>Δ</color>";

                foreach (var arsonist in Role.GetRoles<Arsonist>(RoleEnum.Arsonist))
                {
                    if (arsonist.DousedPlayers.Contains(player.PlayerId))
                        name += " <color=#EE7600FF>Ξ</color>";
                }

                foreach (var plaguebearer in Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
                {
                    if (plaguebearer.InfectedPlayers.Contains(player.PlayerId))
                        name += " <color=#CFFE61FF>ρ</color>";
                }

                foreach (var cryomaniac in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
                {
                    if (cryomaniac.DousedPlayers.Contains(player.PlayerId))
                        name += " <color=#642DEAFF>λ</color>";
                }

                foreach (var framer in Role.GetRoles<Framer>(RoleEnum.Framer))
                {
                    if (framer.Framed.Contains(player.PlayerId))
                        name += " <color=#00FFFFFF>ς</color>";
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;
                    consigliere.Investigated.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;
                    inspector.Inspected.Clear();
                }
            }

            if ((player.IsShielded() || player.IsRetShielded()) && CustomGameOptions.ShowShielded == ShieldOptions.Everyone && !(PlayerControl.LocalPlayer.Data.IsDead &&
                CustomGameOptions.DeadSeeEverything))
            {
                name += " <color=#006600FF>✚</color>";
            }

            if (player.IsProtected() && CustomGameOptions.ShowProtect == ProtectOptions.Everyone && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
                name += " <color=#FFFFFFFF>η</color>";

            if (player.IsMarked())
                name += " <color=#F1C40FFF>χ</color>";

            if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) || player == PlayerControl.LocalPlayer)
            {
                if (info[3] != null)
                {
                    var objectifier = info[3] as Objectifier;

                    if (objectifier.ObjectifierType != ObjectifierEnum.None && !objectifier.Hidden)
                        name += $" {objectifier.ColoredSymbol}";
                }

                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role.Name}";
                roleRevealed = true;
            }

            if (roleRevealed)
                player.NameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);

            return (name, color);
        }
    }
}