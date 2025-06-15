namespace TownOfUsReworked.Monos;

public abstract class NameHandler : MonoBehaviour
{
    protected PlayerControl Player { get; set; }

    public static (string, UColor) UpdateGameName(LayerHandler playerHandler, LayerHandler localHandler, bool amOwner, bool deadSeeEverything, out bool revealed)
    {
        revealed = false;
        var player = playerHandler.Player;
        var meeting = (bool)Meeting();
        var local = localHandler.Player;
        var name = "";
        var color = UColor.white;
        var role = playerHandler.CurrentRole;
        var disp = playerHandler.CurrentDisposition;
        var localRole = localHandler.CurrentRole;
        var localDisp = localHandler.CurrentDisposition;
        var removeFromConsig = false;
        var isFactionedEvil = role.Faction.IsFactionedEvil(true);
        var localIsFactionedEvil = localRole.Faction.IsFactionedEvil(true);
        var same = role.Faction == localRole.Faction;

        if (player.CanDoTasks() && (deadSeeEverything || IsCustomHnS() || IsTaskRace()) && !amOwner)
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
            localDisp.UpdatePlayerName(playerHandler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);

            if (playerHandler.CurrentAbility is Snitch snitch && (localRole.Faction.IsFactionedEvil() || (localRole.Faction == Faction.Outcast && Snitch.SnitchSeesOutcasts)) && (role.TasksDone ||
                role.TasksLeft <= Snitch.SnitchTasksRemaining))
            {
                color = snitch.Color;
                name += (name.Contains('\n') ? " " : "\n") + snitch.Name;
                revealed = true;
            }

            var neo = local.GetNeophyte();

            if (same && (isFactionedEvil || (neo && neo == player.GetNeophyte())))
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

                name += $" {role.FactionColorString}ξ</color>";
            }

            localHandler.CurrentAbility.UpdatePlayerName(playerHandler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);
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
        }

        if (amOwner || deadSeeEverything)
        {
            if (player.IsVesting())
                name += " <#DDDD00FF>υ</color>";

            if (player.IsOnAlert())
                name += " <#998040FF>σ</color>";
        }

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
        }

        if (player.IsShielded() && Medic.ShowShielded == ShieldOptions.Everyone && !deadSeeEverything)
            name += " <#006600FF>✚</color>";

        if (player.IsProtected() && GuardianAngel.ShowProtect == ProtectOptions.Everyone && !deadSeeEverything)
            name += " <#FFFFFFFF>η</color>";

        if ((localIsFactionedEvil || deadSeeEverything) && (player == Syndicate.DriveHolder || (SyndicateSettings.GlobalDrive && Syndicate.SyndicateHasChaosDrive && isFactionedEvil)) && same)
            name += " <#008000FF>Δ</color>";

        if ((deadSeeEverything || local.Is<Pestilence>()) && Pestilence.Infected.TryGetValue(player.PlayerId, out var count))
        {
            while (count-- > 0)
                name += " <#424242FF>米</color>";
        }

        if (PlayerLayer.GetLayers<Revealer>().Any(x => x.TasksDone && !x.Caught && localRole.Faction == x.Faction))
        {
            if (isFactionedEvil || (role.Faction == Faction.Outcast && Revealer.RevealerRevealsOutcasts) || (role.Faction == Faction.Crew && Revealer.RevealerRevealsCrew))
            {
                if (Revealer.RevealerRevealsRoles)
                {
                    color = role.Color;
                    name += $"\n{role}";
                    revealed = true;
                }
                else if (disp is not FactionChanger { RevealerReveals: false })
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
            if (disp.Type != Layer.NoneDisposition && !disp.Hidden)
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