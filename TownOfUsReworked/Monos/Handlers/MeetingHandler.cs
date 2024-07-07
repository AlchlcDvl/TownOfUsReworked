namespace TownOfUsReworked.Monos;

public class MeetingHandler : MonoBehaviour
{
    private static Vector3? NamePos;

    public static MeetingHandler Instance { get; private set; }

    public MeetingHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void Update()
    {
        if (NoPlayers || IsHnS || !IsInGame || IsCustomHnS || IsTaskRace || !Meeting)
            return;

        // Deactivate skip Button if skipping on emergency meetings is disabled
        if (!CustomPlayer.LocalCustom.Dead)
        {
            Meeting.SkipVoteButton.gameObject.SetActive(!((MeetingPatches.Reported == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) ||
                (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)) && Meeting.state == MeetingHud.VoteStates.NotVoted && !Ability.GetAssassins().Any(x => x.Phone) &&
                !PlayerLayer.GetLayers<Guesser>().Any(x => x.Phone) && !PlayerLayer.GetLayers<Thief>().Any(x => x.Phone));
        }
        else
            Meeting.SkipVoteButton.gameObject.SetActive(false);

        AllVoteAreas.ForEach(SetNames);
        PlayerLayer.LocalLayers.ForEach(x => x.UpdateMeeting(Meeting));
    }

    private static void SetNames(PlayerVoteArea player)
    {
        NamePos ??= player.ColorBlindName.transform.localPosition;
        (player.NameText.text, player.NameText.color) = UpdateGameName(player);
        player.ColorBlindName.color = player.NameText.color;
    }

    private static (string, UColor) UpdateGameName(PlayerVoteArea player)
    {
        var color = UColor.white;
        var name = "";
        var info = player.GetLayers();
        var localinfo = CustomPlayer.Local.GetLayers();
        var revealed = false;

        if (HudHandler.Instance.IsCamoed && player != CustomPlayer.Local && !CustomPlayer.LocalCustom.Dead)
            name = ClientGameOptions.OptimisationMode ? "" : GetRandomisedName();
        else
            name = PlayerHandler.Instance.PlayerNames.FirstOrDefault(x => x.Key == player.TargetPlayerId).Value;

        if (info.Count != 4 || localinfo.Count != 4)
            return (name, color);

        if (player.CanDoTasks() && (CustomPlayer.Local.PlayerId == player.TargetPlayerId || DeadSeeEverything))
        {
            var role = info[0] as Role;
            name = $"{name} ({role.TasksCompleted}/{role.TotalTasks})";
            revealed = true;
        }

        if (player.IsKnighted())
            name += " <color=#FF004EFF>κ</color>";

        if (player.IsSpellbound())
            name += " <color=#0028F5FF>ø</color>";

        if (player.IsMarked())
            name += " <color=#F1C40FFF>χ</color>";

        if (player.Is(LayerEnum.Mayor) && !DeadSeeEverything && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
        {
            var mayor = info[0] as Mayor;

            if (mayor.Revealed)
            {
                revealed = true;
                name += $"\n{mayor.Name}";
                color = mayor.Color;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.TargetPlayerId))
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.TargetPlayerId))
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
            }
        }
        else if (player.Is(LayerEnum.Dictator) && !DeadSeeEverything && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
        {
            var dict = info[0] as Dictator;

            if (dict.Revealed)
            {
                revealed = true;
                name += $"\n{dict.Name}";
                color = dict.Color;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.TargetPlayerId))
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.TargetPlayerId))
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Coroner) && !DeadSeeEverything)
        {
            var coroner = localinfo[0] as Coroner;

            if (coroner.Reported.Contains(player.TargetPlayerId) && !revealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Consigliere) && !DeadSeeEverything)
        {
            var consigliere = localinfo[0] as Consigliere;

            if (consigliere.Investigated.Contains(player.TargetPlayerId) && !revealed)
            {
                var role = info[0] as Role;
                revealed = true;

                if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                {
                    color = role.Color;
                    name += $"\n{role}";

                    if (consigliere.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather) && !DeadSeeEverything)
        {
            var godfather = localinfo[0] as PromotedGodfather;

            if (godfather.IsConsig && godfather.Investigated.Contains(player.TargetPlayerId) && !revealed)
            {
                var role = info[0] as Role;
                revealed = true;

                if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                {
                    color = role.Color;
                    name += $"\n{role}";

                    if (godfather.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Medic))
        {
            var medic = localinfo[0] as Medic;

            if (medic.ShieldedPlayer && medic.ShieldedPlayer.PlayerId == player.TargetPlayerId && (int)CustomGameOptions.ShowShielded is 1 or 2)
                name += " <color=#006600FF>✚</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Trapper))
        {
            var trapper = localinfo[0] as Trapper;

            if (trapper.Trapped.Contains(player.TargetPlayerId))
                name += " <color=#BE1C8CFF>∮</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
        {
            var ret = localinfo[0] as Retributionist;

            if (ret.Reported.Contains(player.TargetPlayerId) && !revealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }

            if (ret.ShieldedPlayer && ret.ShieldedPlayer.PlayerId == player.TargetPlayerId && (int)CustomGameOptions.ShowShielded is 1 or 2)
                name += " <color=#006600FF>✚</color>";

            if (ret.Trapped.Contains(player.TargetPlayerId))
                name += " <color=#BE1C8CFF>∮</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Arsonist) && !DeadSeeEverything)
        {
            var arsonist = localinfo[0] as Arsonist;

            if (arsonist.Doused.Contains(player.TargetPlayerId))
                name += " <color=#EE7600FF>Ξ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Plaguebearer) && !DeadSeeEverything)
        {
            var plaguebearer = localinfo[0] as Plaguebearer;

            if (plaguebearer.Infected.Contains(player.TargetPlayerId) && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
                name += " <color=#CFFE61FF>ρ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Cryomaniac) && !DeadSeeEverything)
        {
            var cryomaniac = localinfo[0] as Cryomaniac;

            if (cryomaniac.Doused.Contains(player.TargetPlayerId))
                name += " <color=#642DEAFF>λ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Framer) && !DeadSeeEverything)
        {
            var framer = localinfo[0] as Framer;

            if (framer.Framed.Contains(player.TargetPlayerId))
                name += " <color=#00FFFFFF>ς</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Executioner) && !DeadSeeEverything)
        {
            var executioner = localinfo[0] as Executioner;

            if (player.TargetPlayerId == executioner.TargetPlayer.PlayerId)
            {
                name += " <color=#CCCCCCFF>§</color>";

                if (CustomGameOptions.ExeKnowsTargetRole && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                }
                else
                    color = executioner.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Guesser) && !DeadSeeEverything)
        {
            var guesser = localinfo[0] as Guesser;

            if (player.TargetPlayerId == guesser.TargetPlayer.PlayerId)
            {
                color = guesser.Color;
                name += " <color=#EEE5BEFF>π</color>";
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.GuardianAngel) && !DeadSeeEverything)
        {
            var guardianAngel = localinfo[0] as GuardianAngel;

            if (player.TargetPlayerId == guardianAngel.TargetPlayer.PlayerId)
            {
                name += " <color=#FFFFFFFF>★</color>";

                if (CustomGameOptions.GAKnowsTargetRole && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                }
                else
                    color = guardianAngel.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Whisperer) && !DeadSeeEverything)
        {
            var whisperer = localinfo[0] as Whisperer;

            if (whisperer.Persuaded.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    revealed = true;
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else if (whisperer.PlayerConversion.TryGetValue(player.TargetPlayerId, out var value))
                name += $" {value}%";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Dracula) && !DeadSeeEverything)
        {
            var dracula = localinfo[0] as Dracula;

            if (dracula.Converted.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    revealed = true;
                }
                else
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Jackal) && !DeadSeeEverything)
        {
            var jackal = localinfo[0] as Jackal;

            if (jackal.Recruited.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    revealed = true;
                }
                else
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Necromancer) && !DeadSeeEverything)
        {
            var necromancer = localinfo[0] as Necromancer;

            if (necromancer.Resurrected.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    revealed = true;
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(Alignment.NeutralKill) && !DeadSeeEverything && CustomGameOptions.NKsKnow)
        {
            if (((player.GetRole().Type == Role.LocalRole.Type && CustomGameOptions.NoSolo == NoSolo.SameNKs) || (player.GetAlignment() == CustomPlayer.Local.GetAlignment() &&
                CustomGameOptions.NoSolo == NoSolo.AllNKs)) && !revealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }
        }

        if (CustomPlayer.Local.IsBitten() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Dracula))
        {
            var dracula = CustomPlayer.Local.GetDracula();

            if (dracula.Converted.Contains(player.TargetPlayerId) && !dracula.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsRecruit() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Jackal))
        {
            var jackal = CustomPlayer.Local.GetJackal();

            if (jackal.Recruited.Contains(player.TargetPlayerId) && !jackal.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsResurrected() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Necromancer))
        {
            var necromancer = CustomPlayer.Local.GetNecromancer();

            if (necromancer.Resurrected.Contains(player.TargetPlayerId) && !necromancer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsPersuaded() && !DeadSeeEverything && !CustomPlayer.Local.Is(LayerEnum.Whisperer))
        {
            var whisperer = CustomPlayer.Local.GetWhisperer();

            if (whisperer.Persuaded.Contains(player.TargetPlayerId) && !whisperer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else if (whisperer.PlayerConversion.TryGetValue(player.TargetPlayerId, out var value))
                name += $" {value}%";
        }

        if (CustomPlayer.Local.Is(LayerEnum.Lovers) && !DeadSeeEverything)
        {
            var lover = localinfo[3] as Objectifier;
            var otherLover = CustomPlayer.Local.GetOtherLover();

            if (otherLover.PlayerId == player.TargetPlayerId)
            {
                name += $" {lover.ColoredSymbol}";

                if (CustomGameOptions.LoversRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Rivals) && !DeadSeeEverything)
        {
            var rival = localinfo[3] as Objectifier;
            var otherRival = CustomPlayer.Local.GetOtherRival();

            if (otherRival.PlayerId == player.TargetPlayerId)
            {
                name += $" {rival.ColoredSymbol}";

                if (CustomGameOptions.RivalsRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Linked) && !DeadSeeEverything)
        {
            var link = localinfo[3] as Objectifier;
            var otherLink = CustomPlayer.Local.GetOtherLink();

            if (otherLink.PlayerId == player.TargetPlayerId)
            {
                name += $" {link.ColoredSymbol}";

                if (CustomGameOptions.LinkedRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Mafia) && !DeadSeeEverything)
        {
            var mafia = localinfo[3] as Mafia;

            if (player.Is(LayerEnum.Mafia) && player.TargetPlayerId != CustomPlayer.Local.PlayerId)
            {
                name += $" {mafia.ColoredSymbol}";

                if (CustomGameOptions.MafiaRoles && !revealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Snitch) && CustomGameOptions.SnitchSeestargetsInMeeting && !DeadSeeEverything && player.TargetPlayerId != CustomPlayer.Local.PlayerId)
        {
            var role = localinfo[0] as Role;

            if (role.TasksDone)
            {
                var role2 = info[0] as Role;

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) || (player.Is(Faction.Crew)
                        && CustomGameOptions.SnitchSeesCrew))
                    {
                        color = role2.Color;
                        name += $"\n{role2.Name}";
                        revealed = true;
                    }
                }
                else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) || (player.Is(Faction.Crew)
                    && CustomGameOptions.SnitchSeesCrew))
                {
                    if (!(player.Is(LayerEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) && !(player.Is(LayerEnum.Fanatic) && CustomGameOptions.SnitchSeesFanatic))
                    {
                        color = role2.FactionColor;
                        name += $"\n{role2.FactionName}";
                    }
                    else
                    {
                        color = CustomColorManager.Crew;
                        name += "\nCrew";
                    }

                    revealed = true;
                }
            }
        }

        if (player.Is(LayerEnum.Snitch) && !DeadSeeEverything && player.TargetPlayerId != CustomPlayer.Local.PlayerId && (CustomPlayer.Local.Is(Faction.Syndicate) ||
            CustomPlayer.Local.Is(Faction.Intruder) || (CustomPlayer.Local.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals)))
        {
            var role = info[0] as Role;

            if (role.TasksDone || role.TasksLeft <= CustomGameOptions.SnitchTasksRemaining)
            {
                var ability = info[2] as Ability;
                color = ability.Color;
                name += (name.Contains('\n') ? " " : "\n") + $"{ability.Name}";
                revealed = true;
            }
        }

        if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player.TargetPlayerId != CustomPlayer.Local.PlayerId && player.GetFaction() is Faction.Intruder or Faction.Syndicate &&
            !DeadSeeEverything)
        {
            var role = info[0] as Role;

            if (CustomGameOptions.FactionSeeRoles && !revealed)
            {
                color = role.Color;
                name += $"\n{role}";
                revealed = true;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.TargetPlayerId))
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.TargetPlayerId))
                        godfather.Investigated.Remove(player.TargetPlayerId);
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

        if ((CustomPlayer.Local.Is(Faction.Syndicate) || DeadSeeEverything) && (player.TargetPlayerId == Role.DriveHolder?.PlayerId || (CustomGameOptions.GlobalDrive &&
            Role.SyndicateHasChaosDrive && player.Is(Faction.Syndicate))))
        {
            name += " <color=#008000FF>Δ</color>";
        }

        if (PlayerLayer.GetLayers<Revealer>().Any(x => x.TasksDone && !x.Caught) && CustomPlayer.Local.Is(Faction.Crew))
        {
            var role = info[0] as Role;

            if (CustomGameOptions.RevealerRevealsRoles)
            {
                if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew) &&
                    CustomGameOptions.RevealerRevealsCrew))
                {
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                }
            }
            else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew) &&
                CustomGameOptions.RevealerRevealsCrew))
            {
                if (!(player.Is(LayerEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor) && !(player.Is(LayerEnum.Fanatic) &&
                    CustomGameOptions.RevealerRevealsFanatic))
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
                else
                {
                    color = CustomColorManager.Crew;
                    name += "\nCrew";
                }

                revealed = true;
            }
        }

        if (player.TargetPlayerId == CustomPlayer.Local.PlayerId && !player.AmDead)
        {
            if (player.IsShielded() && (int)CustomGameOptions.ShowShielded is 0 or 2)
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

        if (DeadSeeEverything)
        {
            if (player.IsShielded() && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
                name += " <color=#006600FF>✚</color>";

            if (player.IsTrapped())
                name += " <color=#BE1C8CFF>∮</color>";

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

            if (player.IsFramed())
                name += " <color=#00FFFFFF>ς</color>";

            if (player.IsInfected())
                name += " <color=#CFFE61FF>ρ</color>";

            if (player.IsArsoDoused())
                name += " <color=#EE7600FF>Ξ</color>";

            if (player.IsCryoDoused())
                name += " <<color=#642DEAFF>λ</color>";
        }

        if (player.IsShielded() && (int)CustomGameOptions.ShowShielded is 3 && !DeadSeeEverything)
            name += " <color=#006600FF>✚</color>";

        if ((DeadSeeEverything || CustomPlayer.Local.Is(LayerEnum.Pestilence)) && Pestilence.Infected.TryGetValue(player.TargetPlayerId, out var count))
        {
            for (var i = 0; i < count; i++)
                name += " <color=#424242FF>米</color>";
        }

        if (DeadSeeEverything || player.TargetPlayerId == CustomPlayer.Local.PlayerId)
        {
            if (info[3])
            {
                var objectifier = info[3] as Objectifier;

                if (objectifier.Type != LayerEnum.NoneObjectifier && !objectifier.Hidden)
                    name += $" {objectifier.ColoredSymbol}";
            }

            if (info[0])
            {
                var role = info[0] as Role;

                if (!name.Contains(role.Name))
                {
                    color = role.Color;
                    name += $"{(name.Contains('\n') ? " " : "\n")}{role}";
                    revealed = true;
                }
            }
        }

        if (revealed)
            player.ColorBlindName.transform.localPosition = new(-0.93f, 0f, -0.1f);
        else
            player.ColorBlindName.transform.localPosition = NamePos.Value;

        return (name, color);
    }
}