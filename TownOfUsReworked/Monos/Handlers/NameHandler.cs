namespace TownOfUsReworked.Monos;

public class NameHandler : MonoBehaviour
{
    public static readonly Dictionary<byte, string> PlayerNames = [];
    public static readonly Dictionary<byte, string> ColorNames = [];

    public PlayerControl Player { get; set; }

    [HideFromIl2Cpp]
    public CustomPlayer Custom { get; set; }

    public static (string, UColor) UpdateColorblind(PlayerControl player)
    {
        if (!DataManager.Settings.Accessibility.ColorBlindMode)
            return ("", UColor.clear);

        var color = player.GetCurrentOutfit().ColorId.GetColor(false);

        if (IsLobby())
            return (ColorNames[player.PlayerId], color);

        var local = CustomPlayer.Local;
        var amOwner = player.AmOwner;

        if (!Meeting())
        {
            var distance = Vector2.Distance(local.transform.position, player.transform.position);

            if (distance > ShipStatus.Instance?.CalculateLightRadius(local.Data))
                return ("", UColor.clear);

            var vector = player.transform.position - local.transform.position;

            if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !amOwner && !local.HasDied() &&
                GameModifiers.PlayerNames == Data.PlayerNames.Obstructed)
            {
                return ("", UColor.clear);
            }
        }

        if (!TransitioningSize.ContainsKey(player.PlayerId))
            player.IsMimicking(out player);

        var name = "";

        if (GameModifiers.PlayerNames == Data.PlayerNames.NotVisible)
            name = "";
        else if (!amOwner && !local.HasDied())
        {
            if (HudHandler.Instance.IsCamoed)
                name = ClientOptions.OptimisationMode ? "" : GetRandomisedName();
            else if (CachedMorphs.TryGetValue(player.PlayerId, out var cache))
                name = ColorNames[cache];
            else if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.Colorblind)
                return ("", UColor.clear);
            else
                name = ColorNames[player.PlayerId];
        }
        else
            name = ColorNames[player.PlayerId];

        var ld = player.CurrentOutfit.ColorId.IsLighter() ? "L" : "D";

        if (ClientOptions.LighterDarker)
            name += $" ({ld})";

        return (name, color);
    }

    public static (string, UColor) UpdateGameName(LayerHandler playerHandler, LayerHandler localHandler, out bool revealed)
    {
        revealed = false;
        var player = playerHandler.Player;

        if (IsLobby())
            return (PlayerNames[player.PlayerId], UColor.white);

        var deadSeeEverything = DeadSeeEverything();
        var local = localHandler.Player;
        var meeting = Meeting();
        var amOwner = player.AmOwner;

        if (!meeting)
        {
            var distance = Vector2.Distance(local.transform.position, player.transform.position);

            if (distance > ShipStatus.Instance?.CalculateLightRadius(local.Data))
                return ("", UColor.white);

            var vector = player.transform.position - local.transform.position;

            if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, vector.normalized, distance, Constants.ShipAndObjectsMask) && !amOwner && !local.HasDied() &&
                GameModifiers.PlayerNames == Data.PlayerNames.Obstructed)
            {
                return ("", UColor.clear);
            }
        }

        if (!amOwner && !deadSeeEverything && !TransitioningSize.ContainsKey(player.PlayerId) && player.IsMimicking(out var mimicked) && mimicked.Data.Role is LayerHandler handler)
        {
            player = mimicked;
            playerHandler = handler;
        }

        var name = "";
        var color = UColor.white;
        var role = playerHandler.CustomRole;
        var disp = playerHandler.CustomDisposition;
        var localRole = localHandler.CustomRole;
        var localDisp = localHandler.CustomDisposition;
        var removeFromConsig = false;

        if (GameModifiers.PlayerNames == Data.PlayerNames.NotVisible)
            name = "";
        else if (!playerHandler.Local && !local.HasDied())
        {
            if (HudHandler.Instance.IsCamoed)
                name = ClientOptions.OptimisationMode ? "" : GetRandomisedName();
            else if (CachedMorphs.TryGetValue(player.PlayerId, out var cache))
                name = PlayerNames[cache];
            else if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.Colorblind)
                name = "";
            else
                name = PlayerNames[player.PlayerId];
        }
        else
            name = PlayerNames[player.PlayerId];

        if (player.CanDoTasks() && (deadSeeEverything || amOwner || IsCustomHnS() || IsTaskRace()))
            name += role.TasksDone ? "✔" : $" ({role.TasksCompleted}/{role.TotalTasks})";

        if (player.IsKnighted())
            name += " <#FF004EFF>κ</color>";

        if (player.IsSpellbound() && meeting)
            name += " <#0028F5FF>ø</color>";

        if (player.IsMarked())
            name += " <#F1C40FFF>χ</color>";

        if (player.Data.PlayerName == CachedFirstDead && ((amOwner && (int)GameModifiers.WhoSeesFirstKillShield == 1) || GameModifiers.WhoSeesFirstKillShield == 0))
            name += " <#C2185BFF>Γ</color>";

        if (!deadSeeEverything && !amOwner)
        {
            switch (role)
            {
                case Mayor mayor:
                {
                    if (mayor.Revealed)
                    {
                        revealed = true;
                        name += $"\n{mayor.Name}";
                        color = mayor.Color;
                        removeFromConsig = true;
                    }

                    break;
                }
                case Dictator dict:
                {
                    if (dict.Revealed)
                    {
                        revealed = true;
                        name += $"\n{dict.Name}";
                        color = dict.Color;
                        removeFromConsig = true;
                    }

                    break;
                }
                case NKilling:
                {
                    if (localRole is NKilling && !deadSeeEverything && NeutralKillingSettings.KnowEachOther && ((role.Type == localRole.Type && NeutralSettings.NoSolo == NoSolo.SameNKs) ||
                        NeutralSettings.NoSolo == NoSolo.AllNKs) && !revealed)
                    {
                        color = role.Color;
                        name += $"\n{role}";
                        revealed = true;
                    }

                    break;
                }
            }

            switch (localRole)
            {
                case Consigliere consigliere:
                {
                    if (consigliere.Investigated.Contains(player.PlayerId) && !revealed)
                    {
                        revealed = true;
                        var revealRole = Consigliere.ConsigInfo == ConsigInfo.Role;
                        removeFromConsig = role.SubFaction == consigliere.SubFaction && role.SubFaction != SubFaction.None && revealRole;
                        color = revealRole ? role.Color : role.FactionColor;
                        name += revealRole ? $"\n{role}" : $"\n{role.FactionName}";
                    }

                    break;
                }
                case PromotedGodfather godfather:
                {
                    if (godfather.IsConsig && godfather.Investigated.Contains(player.PlayerId) && !revealed)
                    {
                        revealed = true;
                        var revealRole = Consigliere.ConsigInfo == ConsigInfo.Role;
                        removeFromConsig = role.SubFaction == godfather.SubFaction && role.SubFaction != SubFaction.None && revealRole;
                        color = revealRole ? role.Color : role.FactionColor;
                        name += revealRole ? $"\n{role}" : $"\n{role.FactionName}";
                    }
                    else if (godfather.IsBM && godfather.BlackmailedPlayer == player)
                    {
                        name += " <#02A752FF>Φ</color>";
                        color = godfather.Color;
                    }
                    else if (godfather.IsAmb && godfather.AmbushedPlayer == player)
                    {
                        name += " <#2BD29CFF>人</color>";
                        color = godfather.Color;
                    }
                    if (godfather.FlashedPlayers.Contains(player.PlayerId) && Grenadier.GrenadierIndicators)
                    {
                        name += " <#85AA5BFF>ㅇ</color>";
                        color = godfather.Color;
                    }

                    break;
                }
                case PromotedRebel rebel:
                {
                    if (rebel.IsSil && rebel.SilencedPlayer == player)
                    {
                        name += " <#AAB43EFF>乂</color>";
                        color = rebel.Color;
                    }
                    else if (rebel.IsCrus && rebel.CrusadedPlayer == player)
                    {
                        name += " <#DF7AE8FF>τ</color>";
                        color = rebel.Color;
                    }

                    break;
                }
                case Medic medic:
                {
                    if (medic.ShieldedPlayer && medic.ShieldedPlayer == player && (int)Medic.ShowShielded is 1 or 2)
                        name += " <#006600FF>✚</color>";

                    break;
                }
                case Trapper trapper:
                {
                    if (trapper.Trapped.Contains(player.PlayerId))
                        name += " <#BE1C8CFF>∮</color>";

                    break;
                }
                case Retributionist ret:
                {
                    if (ret.ShieldedPlayer && ret.ShieldedPlayer == player && (int)Medic.ShowShielded is 1 or 2)
                    {
                        name += " <#006600FF>✚</color>";
                        color = ret.Color;
                    }

                    if (ret.Trapped.Contains(player.PlayerId))
                    {
                        name += " <#BE1C8CFF>∮</color>";
                        color = ret.Color;
                    }

                    if (ret.Reported.Contains(player.PlayerId) && !revealed && meeting)
                    {
                        color = role.Color;
                        name += $"\n{role}";
                        revealed = true;
                    }

                    break;
                }
                case Arsonist arsonist:
                {
                    if (arsonist.Doused.Contains(player.PlayerId))
                        name += " <#EE7600FF>Ξ</color>";

                    break;
                }
                case Plaguebearer plaguebearer:
                {
                    if (plaguebearer.Infected.Contains(player.PlayerId) && !amOwner)
                        name += " <#CFFE61FF>ρ</color>";

                    break;
                }
                case Cryomaniac cryomaniac:
                {
                    if (cryomaniac.Doused.Contains(player.PlayerId))
                        name += " <#642DEAFF>λ</color>";

                    break;
                }
                case Framer framer:
                {
                    if (framer.Framed.Contains(player.PlayerId))
                        name += " <#00FFFFFF>ς</color>";

                    break;
                }
                case Spellslinger spellslinger:
                {
                    if (spellslinger.Spelled.Contains(player.PlayerId))
                        name += " <#0028F5FF>ø</color>";

                    break;
                }
                case Executioner executioner:
                {
                    if (player == executioner.TargetPlayer)
                    {
                        name += " <#CCCCCCFF>§</color>";

                        if (Executioner.ExeKnowsTargetRole && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                        }
                        else
                            color = executioner.Color;
                    }

                    break;
                }
                case Guesser guesser:
                {
                    if (player == guesser.TargetPlayer)
                    {
                        color = guesser.Color;
                        name += " <#EEE5BEFF>π</color>";
                    }

                    break;
                }
                case GuardianAngel guardianAngel:
                {
                    if (player == guardianAngel.TargetPlayer)
                    {
                        name += " <#FFFFFFFF>★</color>";

                        if (player.IsProtected() && (int)GuardianAngel.ShowProtect is 1 or 2)
                            name += " <#FFFFFFFF>η</color>";

                        if (GuardianAngel.GAKnowsTargetRole && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                        }
                        else
                            color = guardianAngel.Color;
                    }

                    break;
                }
                case Neophyte neophyte:
                {
                    if (neophyte.Members.Contains(player.PlayerId) && !amOwner)
                    {
                        name += $" <#{neophyte.SubFactionColor.ToHtmlStringRGBA()}>{neophyte.SubFactionSymbol}</color>";

                        if (GameModifiers.FactionSeeRoles && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                        }
                        else
                            color = neophyte.SubFactionColor;
                    }
                    else if (neophyte is Whisperer whisperer && whisperer.PlayerConversion.TryGetValue(player.PlayerId, out var value))
                        name += $" {value}%";

                    break;
                }
                case Grenadier grenadier:
                {
                    if (grenadier.FlashedPlayers.Contains(player.PlayerId) && Grenadier.GrenadierIndicators)
                    {
                        name += " <#85AA5BFF>ㅇ</color>";
                        color = grenadier.Color;
                    }

                    break;
                }
                case Blackmailer blackmailer:
                {
                    if (blackmailer.BlackmailedPlayer == player)
                    {
                        name += " <#02A752FF>Φ</color>";
                        color = blackmailer.Color;
                    }

                    break;
                }
                case Silencer silencer:
                {
                    if (silencer.SilencedPlayer == player)
                    {
                        name += " <#AAB43EFF>乂</color>";
                        color = silencer.Color;
                    }

                    break;
                }
                case Ambusher ambusher:
                {
                    if (ambusher.AmbushedPlayer == player)
                    {
                        name += " <#2BD29CFF>人</color>";
                        color = ambusher.Color;
                    }

                    break;
                }
                case Crusader crusader:
                {
                    if (crusader.CrusadedPlayer == player)
                    {
                        name += " <#DF7AE8FF>τ</color>";
                        color = crusader.Color;
                    }

                    break;
                }
                case Hunter or Hunted or Runner:
                {
                    name += $"\n{role}";
                    color = role.Color;
                    revealed = true;
                    break;
                }
                case Coroner coroner:
                {
                    if (coroner.Reported.Contains(player.PlayerId) && !revealed && meeting)
                    {
                        color = role.Color;
                        name += $"\n{role}";
                        revealed = true;
                    }

                    break;
                }
            }

            if (localRole.IsConverted())
            {
                var neophyte = local.GetNeophyte();

                if (neophyte.Members.Contains(player.PlayerId))
                {
                    if (GameModifiers.FactionSeeRoles && !revealed)
                    {
                        color = role.Color;
                        name += $" <#{neophyte.SubFactionColor.ToHtmlStringRGBA()}>{neophyte.SubFactionSymbol}</color>\n{role}";
                        revealed = true;
                        removeFromConsig = true;
                    }
                    else
                        color = neophyte.SubFactionColor;
                }
                else if (neophyte is Whisperer whisperer && whisperer.PlayerConversion.TryGetValue(player.PlayerId, out var value))
                    name += $" {value}%";
            }

            switch (localDisp)
            {
                case Lovers lover:
                {
                    if (lover.OtherLover == player)
                    {
                        name += $" {lover.ColoredSymbol}";

                        if (Lovers.LoversRoles && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                            removeFromConsig = true;
                        }
                    }

                    break;
                }
                case Rivals rival:
                {
                    if (rival.OtherRival == player)
                    {
                        name += $" {rival.ColoredSymbol}";

                        if (Rivals.RivalsRoles && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                            removeFromConsig = true;
                        }
                    }

                    break;
                }
                case Linked linked:
                {
                    if (linked.OtherLink == player)
                    {
                        name += $" {linked.ColoredSymbol}";

                        if (Linked.LinkedRoles && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                            removeFromConsig = true;
                        }
                    }

                    break;
                }
                case Mafia:
                {
                    if (disp is Mafia)
                    {
                        name += $" {localDisp.ColoredSymbol}";

                        if (Linked.LinkedRoles && !revealed)
                        {
                            color = role.Color;
                            name += $"\n{role}";
                            revealed = true;
                            removeFromConsig = true;
                        }
                    }

                    break;
                }
            }

            if ((localRole.Faction is Faction.Syndicate or Faction.Intruder || (localRole.Faction == Faction.Neutral && Snitch.SnitchSeesNeutrals)) && playerHandler.CustomAbility is Snitch
                snitch && (role.TasksDone || role.TasksLeft <= Snitch.SnitchTasksRemaining))
            {
                color = snitch.Color;
                name += (name.Contains('\n') ? " " : "\n") + snitch.Name;
                revealed = true;
            }

            if (localRole.Faction == role.Faction && role.Faction is Faction.Intruder or Faction.Syndicate)
            {
                if (GameModifiers.FactionSeeRoles && !revealed)
                {
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                    removeFromConsig = true;
                }
                else
                    color = role.FactionColor;

                if (player.SyndicateSided() || player.IntruderSided())
                    name += $" {disp.ColoredSymbol}";
                else
                    name += $" {role.FactionColorString}ξ</color>";
            }

            if (localHandler.CustomAbility is Snitch && !deadSeeEverything && localRole.TasksDone)
            {
                if (Snitch.SnitchSeesRoles)
                {
                    if (role.Faction is Faction.Syndicate or Faction.Intruder || (role.Faction == Faction.Neutral && Snitch.SnitchSeesNeutrals) || (role.Faction == Faction.Crew &&
                        Snitch.SnitchSeesCrew))
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
                        revealed = true;
                    }
                }
                if (role.Faction is Faction.Syndicate or Faction.Intruder || (role.Faction == Faction.Neutral && Snitch.SnitchSeesNeutrals) || (role.Faction == Faction.Crew &&
                    Snitch.SnitchSeesCrew))
                {
                    if (!(disp is Traitor && Snitch.SnitchSeesTraitor) && !(disp is Fanatic && Snitch.SnitchSeesFanatic))
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
        }

        if (amOwner && !deadSeeEverything)
        {
            if (player.IsShielded() && (int)Medic.ShowShielded is 0 or 2)
                name += " <#006600FF>✚</color>";

            if (player.IsProtected() && (int)GuardianAngel.ShowProtect is 0 or 2)
                name += " <#FFFFFFFF>η</color>";

            if (player.IsBHTarget())
                name += " <#B51E39FF>Θ</color>";

            if (player.IsExeTarget() && Executioner.ExeTargetKnows)
                name += " <#CCCCCCFF>§</color>";

            if (player.IsGATarget() && GuardianAngel.GATargetKnows)
                name += " <#FFFFFFFF>★</color>";

            if (player.IsGuessTarget() && Guesser.GuessTargetKnows)
                name += " <#EEE5BEFF>π</color>";

            if (role.IsConverted())
                name += $" <#{role.SubFactionColor.ToHtmlStringRGBA()}>{role.SubFactionSymbol}</color>";
        }

        if ((amOwner || deadSeeEverything) && player.IsVesting())
            name += " <#DDDD00FF>υ</color>";

        if ((amOwner || deadSeeEverything) && player.IsOnAlert())
            name += " <#998040FF>σ</color>";

        if (deadSeeEverything)
        {
            if (player.IsShielded() && Medic.ShowShielded != ShieldOptions.Everyone)
                name += " <#006600FF>✚</color>";

            if (player.IsProtected() && GuardianAngel.ShowProtect != ProtectOptions.Everyone)
                name += " <#FFFFFFFF>η</color>";

            if (player.IsTrapped())
                name += " <#BE1C8CFF>∮</color>";

            if (player.IsAmbushed())
                name += " <#2BD29CFF>人</color>";

            if (player.IsCrusaded())
                name += " <#DF7AE8FF>τ</color>";

            if (player.IsBHTarget())
                name += " <#B51E39FF>Θ</color>";

            if (player.IsExeTarget())
                name += " <#CCCCCCFF>§</color>";

            if (player.IsGATarget())
                name += " <#FFFFFFFF>★</color>";

            if (player.IsGuessTarget())
                name += " <#EEE5BEFF>π</color>";

            if (player.IsFramed())
                name += " <#00FFFFFF>ς</color>";

            if (player.IsInfected())
                name += " <#CFFE61FF>ρ</color>";

            if (player.IsArsoDoused())
                name += " <#EE7600FF>Ξ</color>";

            if (player.IsCryoDoused())
                name += " <#642DEAFF>λ</color>";

            if (player.IsSpellbound())
                name += " <#0028F5FF>ø</color>";

            if (role.IsConverted())
                name += $" <#{role.SubFactionColor.ToHtmlStringRGBA()}>{role.SubFactionSymbol}</color>";
        }

        if (player.IsShielded() && (int)Medic.ShowShielded is 3 && !deadSeeEverything)
            name += " <#006600FF>✚</color>";

        if (player.IsProtected() && (int)GuardianAngel.ShowProtect is 3 && !deadSeeEverything)
            name += " <#FFFFFFFF>η</color>";

        if ((local.Is(Faction.Syndicate) || deadSeeEverything) && (player == Role.DriveHolder || (SyndicateSettings.GlobalDrive && Role.SyndicateHasChaosDrive && player.Is(Faction.Syndicate))))
            name += " <#008000FF>Δ</color>";

        if ((deadSeeEverything || local.Is(LayerEnum.Pestilence)) && Pestilence.Infected.TryGetValue(player.PlayerId, out var count))
        {
            for (var i = 0; i < count; i++)
                name += " <#424242FF>米</color>";
        }

        if (PlayerLayer.GetLayers<Revealer>().Any(x => x.TasksDone && !x.Caught) && local.Is(Faction.Crew))
        {
            if (Revealer.RevealerRevealsRoles)
            {
                if (role.Faction is Faction.Syndicate or Faction.Intruder || (role.Faction == Faction.Neutral && Revealer.RevealerRevealsNeutrals) || (role.Faction == Faction.Crew &&
                    Revealer.RevealerRevealsCrew))
                {
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                }
            }
            else if (role.Faction is Faction.Syndicate or Faction.Intruder || (role.Faction == Faction.Neutral && Revealer.RevealerRevealsNeutrals) || (role.Faction == Faction.Crew &&
                Revealer.RevealerRevealsCrew))
            {
                if (!(disp is Traitor && Revealer.RevealerRevealsTraitor) && !(disp is Fanatic && Revealer.RevealerRevealsFanatic))
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

        if (deadSeeEverything || amOwner)
        {
            if (disp.Type != LayerEnum.NoneDisposition && !disp.Hidden)
                name += $" {disp.ColoredSymbol}";

            if (!revealed)
            {
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }
        }

        if (removeFromConsig)
        {
            if (localRole is Consigliere consigliere)
                consigliere.Investigated.Remove(player.PlayerId);
            else if (localRole is PromotedGodfather godfather)
                godfather.Investigated.Remove(player.PlayerId);
        }

        return (name, color);
    }
}