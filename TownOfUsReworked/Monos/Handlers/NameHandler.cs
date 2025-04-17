namespace TownOfUsReworked.Monos;

public abstract class NameHandler : MonoBehaviour
{
    public static readonly Dictionary<byte, string> PlayerNames = [];
    public static readonly Dictionary<byte, string> ColorNames = [];

    protected PlayerControl Player { get; set; }
    protected Vector3 Size { get; set; }

    [HideFromIl2Cpp]
    protected CustomPlayer Custom { get; set; }

    protected static (string, UColor) UpdateColorblind(PlayerControl player)
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
            var vector = player.transform.position - local.transform.position;

            if (vector.magnitude > local.lightSource.viewDistance)
                return ("", UColor.clear);

            if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask) && !amOwner && !local.HasDied() &&
                GameModifiers.PlayerNames == Data.PlayerNames.Obstructed)
            {
                return ("", UColor.clear);
            }
        }

        if (!TransitioningSize.ContainsKey(player.PlayerId) && player.IsMimicking(out var mimicked))
            player = mimicked;

        string name;

        if (GameModifiers.PlayerNames == Data.PlayerNames.NotVisible)
            name = "";
        else if (!amOwner && !local.HasDied())
        {
            if (Hud.Instance.IsCamoed)
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

        if (ClientOptions.LighterDarker)
            name += $" ({TranslationManager.Translate($"Shade.{(player.CurrentOutfit.ColorId.IsLighter() ? "L" : "D")}")})";

        return (name, color);
    }

    protected static (string, UColor) UpdateGameName(LayerHandler playerHandler, LayerHandler localHandler, out bool revealed)
    {
        revealed = false;
        var player = playerHandler.Player;

        if (IsLobby())
            return (PlayerNames[player.PlayerId], UColor.white);

        var deadSeeEverything = DeadSeeEverything();
        var local = localHandler.Player;
        var meeting = (bool)Meeting();
        var amOwner = player.AmOwner;

        if (!meeting)
        {
            var diff = player.transform.position - local.transform.position;

            if (diff.magnitude > local.lightSource.viewDistance)
                return ("", UColor.white);

            if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, diff.normalized, diff.magnitude, Constants.ShipAndObjectsMask) && !amOwner && !local.HasDied() &&
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

        string name;
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
            if (Hud.Instance.IsCamoed)
                name = ClientOptions.OptimisationMode ? "" : GetRandomisedName();
            else if (CachedMorphs.TryGetValue(player.PlayerId, out var cache))
                name = PlayerNames[cache];
            else if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.Colorblind)
                return ("", UColor.clear);
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

        if (player.name == CachedFirstDead && ((amOwner && (int)GameModifiers.WhoSeesFirstKillShield == 1) || GameModifiers.WhoSeesFirstKillShield == 0))
            name += " <#C2185BFF>Γ</color>";

        if (!deadSeeEverything && !amOwner)
        {
            role.UpdateSelfName(ref name, ref color, ref revealed, ref removeFromConsig);
            localRole.UpdatePlayerName(playerHandler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);

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

            localDisp.UpdatePlayerName(playerHandler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);

            if (playerHandler.CustomAbility is Snitch snitch && (localRole.Faction is not (Faction.Crew or Faction.Neutral) || (localRole.Faction == Faction.Neutral && Snitch.SnitchSeesNeutrals))
                && (role.TasksDone || role.TasksLeft <= Snitch.SnitchTasksRemaining))
            {
                color = snitch.Color;
                name += (name.Contains('\n') ? " " : "\n") + snitch.Name;
                revealed = true;
            }

            if (localRole.Faction == role.Faction && role.Faction is not (Faction.Crew or Faction.Neutral))
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

                name += player.SyndicateSided() || player.IntruderSided() || player.ApocalypseSided() || player.ComplianceSided()
                    ? $" {disp.ColoredSymbol}"
                    : $" {role.FactionColorString}ξ</color>";
            }

            localHandler.CustomAbility.UpdatePlayerName(playerHandler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);
        }

        if (amOwner && !deadSeeEverything)
        {
            if (player.IsShielded() && Medic.ShowShielded == ShieldOptions.Shielded)
                name += " <#006600FF>✚</color>";

            if (player.IsProtected() && GuardianAngel.ShowProtect == ProtectOptions.Protected)
                name += " <#FFFFFFFF>η</color>";

            if (player.IsBhTarget())
                name += " <#B51E39FF>Θ</color>";

            if (player.IsExeTarget() && Executioner.ExeTargetKnows)
                name += " <#CCCCCCFF>§</color>";

            if (player.IsGaTarget() && GuardianAngel.GaTargetKnows)
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

            if (player.IsBhTarget())
                name += " <#B51E39FF>Θ</color>";

            if (player.IsExeTarget())
                name += " <#CCCCCCFF>§</color>";

            if (player.IsGaTarget())
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

            if (player.IsCampaigned())
                name += " <#1A3270FF>°</color>";

            if (role.IsConverted())
                name += $" <#{role.SubFactionColor.ToHtmlStringRGBA()}>{role.SubFactionSymbol}</color>";
        }

        if (player.IsShielded() && Medic.ShowShielded == ShieldOptions.Everyone && !deadSeeEverything)
            name += " <#006600FF>✚</color>";

        if (player.IsProtected() && GuardianAngel.ShowProtect == ProtectOptions.Everyone && !deadSeeEverything)
            name += " <#FFFFFFFF>η</color>";

        if ((local.Is(Faction.Syndicate) || deadSeeEverything) && (player == Syndicate.DriveHolder || (SyndicateSettings.GlobalDrive && Syndicate.SyndicateHasChaosDrive && role.Faction is
            not (Faction.Crew or Faction.Neutral))))
        {
            name += " <#008000FF>Δ</color>";
        }

        if ((deadSeeEverything || local.Is<Pestilence>()) && Pestilence.Infected.TryGetValue(player.PlayerId, out var count))
        {
            for (var i = 0; i < count; i++)
                name += " <#424242FF>米</color>";
        }

        if (PlayerLayer.GetLayers<Revealer>().Any(x => x.TasksDone && !x.Caught) && local.Is(Faction.Crew))
        {
            if (Revealer.RevealerRevealsRoles)
            {
                if (role.Faction is not (Faction.Crew or Faction.Neutral) || (role.Faction == Faction.Neutral && Revealer.RevealerRevealsNeutrals) || (role.Faction == Faction.Crew &&
                    Revealer.RevealerRevealsCrew))
                {
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                }
            }
            else if (role.Faction is not (Faction.Crew or Faction.Neutral) || (role.Faction == Faction.Neutral && Revealer.RevealerRevealsNeutrals) || (role.Faction == Faction.Crew &&
                Revealer.RevealerRevealsCrew))
            {
                if (disp is not FactionChanger { RevealerReveals: false })
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

        if (removeFromConsig && localRole is Consigliere consigliere)
            consigliere.Investigated.Remove(player.PlayerId);

        return (name, color);
    }
}