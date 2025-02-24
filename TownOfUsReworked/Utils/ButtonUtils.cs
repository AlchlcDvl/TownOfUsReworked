namespace TownOfUsReworked.Utils;

public static class ButtonUtils
{
    public static void DisableButtons(this PlayerControl player)
    {
        var hud = HUD();
        player.GetButtons().ForEach(x => x.Disable());
        hud.SabotageButton.ToggleVisible(false);
        hud.ReportButton.ToggleVisible(false);
        hud.ImpostorVentButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(false);
        hud.PetButton.ToggleVisible(false);
        hud.AbilityButton.ToggleVisible(false);
    }

    public static IEnumerable<CustomButton> GetButtonsFromList(this PlayerControl player) => CustomButton.AllButtons.Where(x => x.Owner.Player == player);

    public static IEnumerable<CustomButton> GetButtons(this PlayerControl player)
    {
        if (player.Data.Role is LayerHandler handler)
            return handler.Buttons;

        return player.GetButtonsFromList();
    }

    public static void ResetButtons(this PlayerControl player)
    {
        if (player.Data.Role is LayerHandler handler)
            handler.ResetButtons();
    }

    public static void EnableButtons(this PlayerControl player)
    {
        var hud = HUD();
        player.GetButtons().ForEach(x => x.SetActive());
        player.GetRole()?.UpdateButtons();
        hud.KillButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(true);
        hud.PetButton.ToggleVisible(true);
        hud.SabotageButton.ToggleVisible(player.CanSabotage() && IsInGame());
        hud.ReportButton.ToggleVisible(!player.Is<Coward>() && !Meeting() && !player.HasDied() && IsInGame());
        hud.ImpostorVentButton.ToggleVisible(player.CanVent() && IsInGame());

        if (IsHnS())
            hud.AbilityButton.ToggleVisible(!CustomPlayer.Local.IsImpostor() && IsInGame());
        else
            hud.AbilityButton.ToggleVisible(!Meeting() && (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && IsInGame() && CustomPlayer.Local.HasDied());
    }

    public static void DisableAllButtons()
    {
        var hud = HUD();
        CustomButton.AllButtons.ForEach(x => x.Disable());
        hud.KillButton.ToggleVisible(false);
        hud.SabotageButton.ToggleVisible(false);
        hud.ReportButton.ToggleVisible(false);
        hud.ImpostorVentButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(false);
        hud.PetButton.ToggleVisible(false);
        hud.AbilityButton.ToggleVisible(false);
    }

    public static void SetDelay(this ActionButton button, float timer)
    {
        var ceil = Mathf.CeilToInt(timer);
        button.isCoolingDown = timer > 0f;
        button.graphic.transform.localPosition = button.position + (Vector3)(URandom.insideUnitCircle * 0.05f);
        button.cooldownTimerText.text = $"{ceil}";
        button.cooldownTimerText.color = UColor.white;
        button.cooldownTimerText.gameObject.SetActive(button.isCoolingDown);
        button.SetCooldownFill(ceil % 2 == 0 ? 1f : 0f);
    }

    public static void DestroyButtons(this PlayerControl player) => player.GetButtons().ForEach(x => x.Destroy());

    public static bool CannotUse(this PlayerControl player) => player.onLadder || player.inVent || player.inMovingPlat || player.isKilling;

    public static float GetModifiedCooldown(this PlayerControl player, float cooldown, float difference = 0f, float factor = 1f)
    {
        var result = (cooldown * factor * player.GetMultiplier()) + difference + player.GetDifference();

        if (result <= 0f)
            result = 0f;

        return result;
    }

    private static float GetUnderdogChange(this PlayerControl player)
    {
        if (!player.Is<Underdog>())
            return 0f;

        if (Underdog.UnderdogIncreasedCd && !Last(player))
            return Underdog.UnderdogCdBonus;
        else if (Last(player))
            return -Underdog.UnderdogCdBonus;
        else
            return 0f;
    }

    private static float GetDifference(this PlayerControl player)
    {
        var result = 0f;
        result += player.GetUnderdogChange();
        return result;
    }

    private static float GetMultiplier(this PlayerControl player)
    {
        var num = 1f;

        if (player.Is<PromotedGodfather>())
            num *= Godfather.GfPromotionCdDecrease;
        else if (player.Is<PromotedRebel>())
            num *= Rebel.RebPromotionCdDecrease;

        if (player.Diseased())
            num *= Diseased.DiseasedMultiplier;

        return num;
    }

    public static void Reset(CooldownType cooldown = CooldownType.Reset, PlayerControl player = null)
    {
        if (IsHnS())
            return;

        player ??= CustomPlayer.Local;
        var role = player.GetRole();

        if (!role)
            return;

        var start = cooldown == CooldownType.Start;
        var meeting = cooldown == CooldownType.Meeting;
        var dead = DeadSeeEverything();
        player.GetButtons().ForEach(x => x.StartCooldown(cooldown));

        if (role.Requesting && !start)
            role.BountyTimer++;

        switch (role)
        {
            case Escort esc:
            {
                esc.BlockTarget = null;
                break;
            }
            case Operative op:
            {
                op.BuggedPlayers.Clear();

                if (Operative.BugsRemoveOnNewRound && meeting)
                {
                    op.Bugs.ForEach(x => x.gameObject.Destroy());
                    op.Bugs.Clear();
                }

                break;
            }
            case Tracker track:
            {
                if (Tracker.ResetOnNewRound)
                {
                    track.TrackButton.Uses = track.TrackButton.MaxUses;
                    track.ClearArrows();
                }

                break;
            }
            case Mayor mayor:
            {
                mayor.RoundOne = start && Mayor.RoundOneNoMayorReveal;
                break;
            }
            case Monarch mon:
            {
                mon.RoundOne = start && Monarch.RoundOneNoKnighting;
                break;
            }
            case Dictator dict:
            {
                dict.RoundOne = start && Dictator.RoundOneNoDictReveal;
                break;
            }
            case Medium:
            {
                role.ClearArrows();
                break;
            }
            case Retributionist ret:
            {
                ret.BuggedPlayers.Clear();
                ret.BlockTarget = null;
                ret.MediateArrows.Values.DestroyAll();
                ret.MediateArrows.Clear();
                ret.MediatedPlayers.Clear();

                if (Operative.BugsRemoveOnNewRound && meeting)
                {
                    ret.Bugs.ForEach(x => x?.gameObject?.Destroy());
                    ret.Bugs.Clear();
                }

                if (Tracker.ResetOnNewRound)
                {
                    ret.TrackerArrows.Values.DestroyAll();
                    ret.TrackerArrows.Clear();
                    ret.TrackButton.Uses = ret.TrackButton.MaxUses;
                }

                break;
            }
            case Blackmailer bm:
            {
                bm.BlackmailedPlayer = null;
                break;
            }
            case Enforcer enf:
            {
                enf.BombedPlayer = null;
                break;
            }
            case Consigliere consig when player.HasDied() && dead:
            {
                consig.Investigated.Clear();
                break;
            }
            case Consort cons:
            {
                cons.BlockTarget = null;
                break;
            }
            case Disguiser disg:
            {
                disg.MeasuredPlayer = disg.DisguisedPlayer = disg.CopiedPlayer = null;
                break;
            }
            case PromotedGodfather gf:
            {
                gf.BlackmailedPlayer = gf.BlockTarget = gf.MeasuredPlayer = gf.DisguisedPlayer = gf.CopiedPlayer = gf.SampledPlayer = gf.MorphedPlayer = gf.AmbushedPlayer = gf.BombedPlayer = null;
                gf.CurrentlyDragging = null;
                gf.TeleportPoint = Vector2.zero;

                if (player.HasDied() && dead)
                    gf.Investigated.Clear();

                break;
            }
            case Janitor jani:
            {
                jani.CurrentlyDragging = null;
                break;
            }
            case Morphling morph:
            {
                morph.SampledPlayer = morph.MorphedPlayer = null;
                break;
            }
            case Teleporter tele:
            {
                tele.TeleportPoint = Vector2.zero;
                break;
            }
            case Ambusher amb:
            {
                amb.AmbushedPlayer = null;
                break;
            }
            case Concealer conc:
            {
                conc.ConcealedPlayer = null;
                break;
            }
            case Silencer sil:
            {
                sil.SilencedPlayer = null;
                break;
            }
            case Bomber bomb when Bomber.BombsRemoveOnNewRound && meeting:
            {
                bomb.Bombs.ForEach(x => x?.gameObject?.Destroy());
                bomb.Bombs.Clear();
                break;
            }
            case Framer framer when player.HasDied():
            {
                framer.Framed.Clear();
                break;
            }
            case Crusader crus:
            {
                crus.CrusadedPlayer = null;
                break;
            }
            case Poisoner pois:
            {
                pois.PoisonedPlayer = null;
                break;
            }
            case PromotedRebel reb:
            {
                reb.ShapeshiftPlayer1 = reb.ShapeshiftPlayer2 = reb.PoisonedPlayer = reb.ConcealedPlayer = reb.Positive = reb.Negative = reb.SilencedPlayer = reb.CrusadedPlayer = null;

                if (Bomber.BombsRemoveOnNewRound && meeting)
                {
                    reb.Bombs.ForEach(x => x?.gameObject?.Destroy());
                    reb.Bombs.Clear();
                }

                if (player.HasDied())
                    reb.Framed.Clear();

                break;
            }
            case Shapeshifter ss:
            {
                ss.ShapeshiftPlayer1 = ss.ShapeshiftPlayer2 = null;
                break;
            }
            case PlayerLayers.Roles.Collider col:
            {
                col.Positive = col.Negative = null;
                break;
            }
            case Glitch glitch:
            {
                glitch.MimicTarget = glitch.HackTarget = null;
                break;
            }
            case GuardianAngel ga when meeting && !ga.TargetPlayer:
            {
                ga.Rounds++;
                break;
            }
            case Actor act when meeting && !act.Targeted:
            {
                act.Rounds++;
                break;
            }
            case BountyHunter bh when meeting && !bh.TargetPlayer:
            {
                bh.Rounds++;
                break;
            }
            case Executioner exe when meeting && !exe.TargetPlayer:
            {
                exe.Rounds++;
                break;
            }
            case Guesser guess when meeting && !guess.TargetPlayer:
            {
                guess.Rounds++;
                break;
            }
        }
    }
}