namespace TownOfUsReworked.Custom;

public static class ButtonUtils
{
    private static bool Use;

    public static void DisableButtons(this PlayerControl player)
    {
        player.GetButtons().ForEach(x => x.Disable());
        HUD.SabotageButton.gameObject.SetActive(false);
        HUD.ReportButton.gameObject.SetActive(false);
        HUD.ImpostorVentButton.gameObject.SetActive(false);
        HUD.UseButton.gameObject.SetActive(false);
        HUD.PetButton.gameObject.SetActive(false);
        Use = HUD.UseButton.isActiveAndEnabled;
    }

    public static List<CustomButton> GetButtons(this PlayerControl player) => CustomButton.AllButtons.Where(x => x.Owner.Player == player).ToList();

    public static void EnableButtons(this PlayerControl player)
    {
        player.GetButtons().ForEach(x => x.Enable());
        HUD.KillButton.gameObject.SetActive(false);
        HUD.SabotageButton.gameObject.SetActive(player.CanSabotage());
        HUD.ReportButton.gameObject.SetActive(!player.Is(LayerEnum.Coward));
        HUD.ImpostorVentButton.gameObject.SetActive(player.CanVent());

        if (Use)
            HUD.UseButton.gameObject.SetActive(true);
        else
            HUD.PetButton.gameObject.SetActive(true);
    }

    public static void DisableAllButtons()
    {
        CustomButton.AllButtons.ForEach(x => x.Disable());
        HUD.KillButton.gameObject.SetActive(false);
        HUD.SabotageButton.gameObject.SetActive(false);
        HUD.ReportButton.gameObject.SetActive(false);
        HUD.ImpostorVentButton.gameObject.SetActive(false);
        HUD.UseButton.gameObject.SetActive(false);
        HUD.PetButton.gameObject.SetActive(false);
        Use = HUD.UseButton.isActiveAndEnabled;
    }

    public static void SetDelay(this ActionButton button, float timer)
    {
        button.isCoolingDown = timer > 0f;
        button.graphic.transform.localPosition = button.position + ((Vector3)URandom.insideUnitCircle * 0.05f);
        button.cooldownTimerText.text = Mathf.CeilToInt(timer).ToString();
        button.cooldownTimerText.color = UColor.white;
        button.cooldownTimerText.gameObject.SetActive(button.isCoolingDown);
        button.SetCooldownFill(1f);
    }

    public static void DestroyButtons(this PlayerControl player) => CustomButton.AllButtons.Where(x => x.Owner.Player == player).ForEach(x => x.Destroy());

    public static bool CannotUse(this PlayerControl player) => player.onLadder || player.inVent || player.inMovingPlat;

    public static float GetModifiedCooldown(this PlayerControl player, float cooldown, float difference = 0f, float factor = 1f)
    {
        var result = (cooldown * factor * player.GetMultiplier()) + difference + player.GetDifference();

        if (result <= 0f)
            result = 0.1f;

        return result;
    }

    public static float GetUnderdogChange(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Underdog))
            return 0f;

        if (CustomGameOptions.UnderdogIncreasedKC && !Last(player))
            return CustomGameOptions.UnderdogKillBonus;
        else if (Last(player))
            return -CustomGameOptions.UnderdogKillBonus;
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
            num *= CustomGameOptions.GFPromotionCdDecrease;
        if (player.Is(LayerEnum.PromotedRebel))
            num *= CustomGameOptions.RebPromotionCdDecrease;

        if (player.Diseased())
            num *= CustomGameOptions.DiseasedMultiplier;

        return num;
    }

    public static void Reset(CooldownType cooldown = CooldownType.Reset, PlayerControl player = null)
    {
        player ??= CustomPlayer.Local;
        var role = Role.GetRole(player);
        var start = cooldown == CooldownType.Start;
        var meeting = cooldown == CooldownType.Meeting;
        CustomButton.AllButtons.Where(x => x.Owner.Player == player).ForEach(x => x.StartCooldown(cooldown));

        if (role.Requesting && !start)
            role.BountyTimer++;

        if (!start && Role.SyndicateHasChaosDrive)
            RoleGen.AssignChaosDrive();

        if (role is Escort esc)
            esc.BlockTarget = null;
        else if (role is Operative op)
        {
            op.BuggedPlayers.Clear();

            if (CustomGameOptions.BugsRemoveOnNewRound)
                Bug.Clear(op.Bugs);
        }
        else if (role is Tracker track)
        {
            if (CustomGameOptions.ResetOnNewRound)
            {
                track.TrackButton.Uses = track.TrackButton.MaxUses + (track.TasksDone ? 1 : 0);
                track.OnLobby();
            }
        }
        else if (role is Transporter trans)
        {
            trans.TransportPlayer1 = null;
            trans.TransportPlayer2 = null;
        }
        else if (role is Vigilante vigi)
            vigi.RoundOne = start && CustomGameOptions.RoundOneNoShot;
        else if (role is Mayor mayor)
            mayor.RoundOne = start && CustomGameOptions.RoundOneNoMayorReveal;
        else if (role is Monarch mon)
            mon.RoundOne = start && CustomGameOptions.RoundOneNoKnighting;
        else if (role is Medium)
            role.OnLobby();
        else if (role is Retributionist ret)
        {
            ret.BuggedPlayers.Clear();
            ret.BlockTarget = null;
            ret.TransportPlayer1 = null;
            ret.TransportPlayer2 = null;
            ret.MediateArrows.Values.ToList().DestroyAll();
            ret.MediateArrows.Clear();
            ret.MediatedPlayers.Clear();

            if (CustomGameOptions.BugsRemoveOnNewRound && meeting)
                Bug.Clear(ret.Bugs);

            if (CustomGameOptions.ResetOnNewRound)
            {
                ret.TrackerArrows.Values.ToList().DestroyAll();
                ret.TrackerArrows.Clear();
                ret.TrackButton.Uses = ret.TrackButton.MaxUses + (ret.TasksDone ? 1 : 0);
            }
        }
        else if (role is Blackmailer bm)
            bm.BlackmailedPlayer = null;
        else if (role is Enforcer enf)
            enf.BombedPlayer = null;
        else if (role is Consigliere consig && player.HasDied() && DeadSeeEverything)
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

            if (player.HasDied() && DeadSeeEverything)
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
        else if (role is Bomber bomb && CustomGameOptions.BombsRemoveOnNewRound && meeting)
            Bomb.Clear(bomb.Bombs);
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

            if (CustomGameOptions.BombsRemoveOnNewRound && meeting)
                Bomb.Clear(reb.Bombs);

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
        else if (role is GuardianAngel ga && meeting && ga.TargetPlayer == null)
            ga.Rounds++;
        else if (role is Actor act && meeting && !act.Targeted)
            act.Rounds++;
        else if (role is BountyHunter bh && meeting && bh.TargetPlayer == null)
            bh.Rounds++;
        else if (role is Executioner exe && meeting && exe.TargetPlayer == null)
            exe.Rounds++;
        else if (role is Guesser guess && meeting && guess.TargetPlayer == null)
            guess.Rounds++;
        else if (role is Werewolf ww && meeting)
            ww.Rounds++;
    }
}