namespace TownOfUsReworked.Custom;

public static class ButtonUtils
{
    private static bool Use;

    public static void DisableButtons(this PlayerControl player)
    {
        var hud = HUD();
        Use = hud.UseButton.isActiveAndEnabled;
        player.GetButtons().ForEach(x => x.Disable());
        hud.SabotageButton.gameObject.SetActive(false);
        hud.ReportButton.gameObject.SetActive(false);
        hud.ImpostorVentButton.gameObject.SetActive(false);
        hud.UseButton.gameObject.SetActive(false);
        hud.PetButton.gameObject.SetActive(false);
        hud.AbilityButton.gameObject.SetActive(false);
    }

    public static List<CustomButton> GetButtonsFromList(this PlayerControl player) => [ .. AllButtons.Where(x => x.Owner.Player == player) ];

    public static List<CustomButton> GetButtons(this PlayerControl player)
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
        player.GetButtons().ForEach(x => x.Enable());
        hud.KillButton.gameObject.SetActive(false);
        hud.SabotageButton.gameObject.SetActive(player.CanSabotage());
        hud.ReportButton.gameObject.SetActive(!player.Is(LayerEnum.Coward) && !Meeting() && !player.HasDied() && IsInGame());
        hud.ImpostorVentButton.gameObject.SetActive(player.CanVent() && IsInGame());

        if (IsHnS())
            hud.AbilityButton.gameObject.SetActive(!CustomPlayer.Local.IsImpostor() && IsInGame());
        else
            hud.AbilityButton.gameObject.SetActive(!Meeting() && (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && IsInGame());

        if (Use)
            hud.UseButton.gameObject.SetActive(true);
        else
            hud.PetButton.gameObject.SetActive(true);
    }

    public static void DisableAllButtons()
    {
        var hud = HUD();
        Use = hud.UseButton.isActiveAndEnabled;
        AllButtons.ForEach(x => x.Disable());
        hud.KillButton.gameObject.SetActive(false);
        hud.SabotageButton.gameObject.SetActive(false);
        hud.ReportButton.gameObject.SetActive(false);
        hud.ImpostorVentButton.gameObject.SetActive(false);
        hud.UseButton.gameObject.SetActive(false);
        hud.PetButton.gameObject.SetActive(false);
        hud.AbilityButton.gameObject.SetActive(false);
    }

    public static void SetDelay(this ActionButton button, float timer)
    {
        button.isCoolingDown = timer > 0f;
        button.graphic.transform.localPosition = button.position + (Vector3)(URandom.insideUnitCircle * 0.05f);
        button.cooldownTimerText.SetText($"{Mathf.CeilToInt(timer)}");
        button.cooldownTimerText.color = UColor.white;
        button.cooldownTimerText.gameObject.SetActive(button.isCoolingDown);
        button.SetCooldownFill(1f);
    }

    public static void DestroyButtons(this PlayerControl player) => AllButtons.Where(x => x.Owner.Player == player).ForEach(x => x.Destroy());

    public static bool CannotUse(this PlayerControl player) => player.onLadder || player.inVent || player.inMovingPlat;

    public static float GetModifiedCooldown(this PlayerControl player, float cooldown, float difference = 0f, float factor = 1f)
    {
        var result = (cooldown * factor * player.GetMultiplier()) + difference + player.GetDifference();

        if (result <= 0f)
            result = 0f;

        return result;
    }

    public static float GetUnderdogChange(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Underdog))
            return 0f;

        if (Underdog.UnderdogIncreasedCd && !Last(player))
            return Underdog.UnderdogCdBonus;
        else if (Last(player))
            return -Underdog.UnderdogCdBonus;
        else
            return 0f;
    }

    public static float GetDifference(this PlayerControl player)
    {
        var result = 0f;
        result += player.GetUnderdogChange();
        return result;
    }

    public static float GetMultiplier(this PlayerControl player)
    {
        var num = 1f;

        if (player.Is(LayerEnum.PromotedGodfather))
            num *= Godfather.GFPromotionCdDecrease;
        if (player.Is(LayerEnum.PromotedRebel))
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
        var start = cooldown == CooldownType.Start;
        var meeting = cooldown == CooldownType.Meeting;
        AllButtons.Where(x => x.Owner.Player == player).ForEach(x => x.StartCooldown(cooldown));

        if (role.Requesting && !start)
            role.BountyTimer++;

        if (!start && Role.SyndicateHasChaosDrive)
            RoleGen.AssignChaosDrive();

        if (role is Escort esc)
            esc.BlockTarget = null;
        else if (role is Operative op)
        {
            op.BuggedPlayers.Clear();

            if (Operative.BugsRemoveOnNewRound && meeting)
            {
                op.Bugs.ForEach(x => x.gameObject.Destroy());
                op.Bugs.Clear();
            }
        }
        else if (role is Tracker track)
        {
            if (Tracker.ResetOnNewRound)
            {
                track.TrackButton.Uses = track.TrackButton.MaxUses;
                track.Deinit();
            }
        }
        else if (role is Transporter trans)
        {
            trans.TransportPlayer1 = null;
            trans.TransportPlayer2 = null;
        }
        else if (role is Mayor mayor)
            mayor.RoundOne = start && Mayor.RoundOneNoMayorReveal;
        else if (role is Monarch mon)
            mon.RoundOne = start && Monarch.RoundOneNoKnighting;
        else if (role is Medium)
            role.Deinit();
        else if (role is Retributionist ret)
        {
            ret.BuggedPlayers.Clear();
            ret.BlockTarget = null;
            ret.TransportPlayer1 = null;
            ret.TransportPlayer2 = null;
            ret.MediateArrows.Values.ToList().DestroyAll();
            ret.MediateArrows.Clear();
            ret.MediatedPlayers.Clear();

            if (Operative.BugsRemoveOnNewRound && meeting)
            {
                ret.Bugs.ForEach(x => x?.gameObject?.Destroy());
                ret.Bugs.Clear();
            }

            if (Tracker.ResetOnNewRound)
            {
                ret.TrackerArrows.Values.ToList().DestroyAll();
                ret.TrackerArrows.Clear();
                ret.TrackButton.Uses = ret.TrackButton.MaxUses;
            }
        }
        else if (role is Blackmailer bm)
            bm.BlackmailedPlayer = null;
        else if (role is Enforcer enf)
            enf.BombedPlayer = null;
        else if (role is Consigliere consig && player.HasDied() && DeadSeeEverything())
            consig.Investigated.Clear();
        else if (role is Consort cons)
            cons.BlockTarget = null;
        else if (role is Disguiser disg)
        {
            disg.MeasuredPlayer = null;
            disg.DisguisedPlayer = null;
            disg.CopiedPlayer = null;
        }
        else if (role is PromotedGodfather gf)
        {
            gf.BlackmailedPlayer = null;
            gf.BlockTarget = null;
            gf.MeasuredPlayer = null;
            gf.DisguisedPlayer = null;
            gf.CopiedPlayer = null;
            gf.SampledPlayer = null;
            gf.MorphedPlayer = null;
            gf.AmbushedPlayer = null;
            gf.BombedPlayer = null;
            gf.CurrentlyDragging = null;
            gf.TeleportPoint = Vector3.zero;

            if (player.HasDied() && DeadSeeEverything())
                gf.Investigated.Clear();
        }
        else if (role is Janitor jani)
            jani.CurrentlyDragging = null;
        else if (role is Morphling morph)
        {
            morph.SampledPlayer = null;
            morph.MorphedPlayer = null;
        }
        else if (role is Teleporter tele)
            tele.TeleportPoint = Vector3.zero;
        else if (role is Ambusher amb)
            amb.AmbushedPlayer = null;
        else if (role is Concealer conc)
            conc.ConcealedPlayer = null;
        else if (role is Silencer sil)
            sil.SilencedPlayer = null;
        else if (role is Bomber bomb && Bomber.BombsRemoveOnNewRound && meeting)
        {
            bomb.Bombs.ForEach(x => x?.gameObject?.Destroy());
            bomb.Bombs.Clear();
        }
        else if (role is Framer framer && player.HasDied())
            framer.Framed.Clear();
        else if (role is Crusader crus)
            crus.CrusadedPlayer = null;
        else if (role is Poisoner pois)
            pois.PoisonedPlayer = null;
        else if (role is PromotedRebel reb)
        {
            reb.ShapeshiftPlayer1 = null;
            reb.ShapeshiftPlayer2 = null;
            reb.PoisonedPlayer = null;
            reb.ConcealedPlayer = null;
            reb.WarpPlayer1 = null;
            reb.WarpPlayer2 = null;
            reb.Positive = null;
            reb.Negative = null;
            reb.SilencedPlayer = null;
            reb.CrusadedPlayer = null;

            if (Bomber.BombsRemoveOnNewRound && meeting)
            {
                reb.Bombs.ForEach(x => x?.gameObject?.Destroy());
                reb.Bombs.Clear();
            }

            if (player.HasDied())
                reb.Framed.Clear();
        }
        else if (role is Shapeshifter ss)
        {
            ss.ShapeshiftPlayer1 = null;
            ss.ShapeshiftPlayer2 = null;
        }
        else if (role is Warper warp)
        {
            warp.WarpPlayer1 = null;
            warp.WarpPlayer2 = null;
        }
        else if (role is PlayerLayers.Roles.Collider col)
        {
            col.Positive = null;
            col.Negative = null;
        }
        else if (role is Glitch glitch)
        {
            glitch.MimicTarget = null;
            glitch.HackTarget = null;
        }
        else if (role is GuardianAngel ga && meeting && !ga.TargetPlayer)
            ga.Rounds++;
        else if (role is Actor act && meeting && !act.Targeted)
            act.Rounds++;
        else if (role is BountyHunter bh && meeting && !bh.TargetPlayer)
            bh.Rounds++;
        else if (role is Executioner exe && meeting && !exe.TargetPlayer)
            exe.Rounds++;
        else if (role is Guesser guess && meeting && !guess.TargetPlayer)
            guess.Rounds++;
    }
}