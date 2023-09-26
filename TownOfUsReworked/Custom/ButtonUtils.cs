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
        Use = HUD.UseButton.isActiveAndEnabled;

        if (Use)
            HUD.UseButton.gameObject.SetActive(false);
        else
            HUD.PetButton.gameObject.SetActive(false);
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
        Use = HUD.UseButton.isActiveAndEnabled;

        if (Use)
            HUD.UseButton.gameObject.SetActive(false);
        else
            HUD.PetButton.gameObject.SetActive(false);
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

    public static void ResetCustomTimers(CooldownType cooldown = CooldownType.Reset)
    {
        var local = CustomPlayer.Local;
        CustomButton.AllButtons.Where(x => x.Owner.Player == local).ForEach(x => x.StartCooldown(cooldown));
        var role = Role.LocalRole;
        var start = cooldown == CooldownType.Start;
        var meeting = cooldown == CooldownType.Meeting;

        if (role.Requesting && !start)
            role.BountyTimer++;

        if (!start && Role.SyndicateHasChaosDrive)
            RoleGen.AssignChaosDrive();

        if (local.Is(LayerEnum.Escort))
            ((Escort)role).BlockTarget = null;
        else if (local.Is(LayerEnum.Inspector))
        {
            var role2 = (Inspector)role;

            if (local.HasDied() && DeadSeeEverything)
                role2.Inspected.Clear();
        }
        else if (local.Is(LayerEnum.Operative))
        {
            var role2 = (Operative)role;
            role2.BuggedPlayers.Clear();

            if (CustomGameOptions.BugsRemoveOnNewRound)
                Bug.Clear(role2.Bugs);
        }
        else if (local.Is(LayerEnum.Tracker))
        {
            var role2 = (Tracker)role;

            if (CustomGameOptions.ResetOnNewRound)
            {
                role2.TrackButton.Uses = role2.TrackButton.MaxUses + (role2.TasksDone ? 1 : 0);
                role2.OnLobby();
            }
        }
        else if (local.Is(LayerEnum.Transporter))
        {
            var role2 = (Transporter)role;
            role2.TransportPlayer1 = null;
            role2.TransportPlayer2 = null;
        }
        else if (local.Is(LayerEnum.Vigilante))
            ((Vigilante)role).RoundOne = start && CustomGameOptions.RoundOneNoShot;
        else if (local.Is(LayerEnum.Mayor))
            ((Mayor)role).RoundOne = start && CustomGameOptions.RoundOneNoMayorReveal;
        else if (local.Is(LayerEnum.Monarch))
            ((Monarch)role).RoundOne = start && CustomGameOptions.RoundOneNoKnighting;
        else if (local.Is(LayerEnum.Medium))
            ((Medium)role).OnLobby();
        else if (local.Is(LayerEnum.Retributionist))
        {
            var role2 = (Retributionist)role;
            role2.BuggedPlayers.Clear();
            role2.BlockTarget = null;
            role2.TransportPlayer1 = null;
            role2.TransportPlayer2 = null;

            if (local.HasDied() && DeadSeeEverything)
                role2.Inspected.Clear();

            if (CustomGameOptions.BugsRemoveOnNewRound && meeting)
                Bug.Clear(role2.Bugs);

            if (CustomGameOptions.ResetOnNewRound)
            {
                role2.TrackerArrows.Values.ToList().DestroyAll();
                role2.TrackerArrows.Clear();
                role2.TrackButton.Uses = role2.TrackButton.MaxUses + (role2.TasksDone ? 1 : 0);
            }

            role2.MediateArrows.Values.ToList().DestroyAll();
            role2.MediateArrows.Clear();
            role2.MediatedPlayers.Clear();
        }
        else if (local.Is(LayerEnum.Blackmailer))
            ((Blackmailer)role).BlackmailedPlayer = null;
        else if (local.Is(LayerEnum.Enforcer))
            ((Enforcer)role).BombedPlayer = null;
        else if (local.Is(LayerEnum.Consigliere))
        {
            var role2 = (Consigliere)role;

            if (local.HasDied() && DeadSeeEverything)
                role2.Investigated.Clear();
        }
        else if (local.Is(LayerEnum.Consort))
            ((Consort)role).BlockTarget = null;
        else if (local.Is(LayerEnum.Disguiser))
        {
            var role2 = (Disguiser)role;
            role2.MeasuredPlayer = null;
            role2.DisguisedPlayer = null;
            role2.CopiedPlayer = null;
        }
        else if (local.Is(LayerEnum.PromotedGodfather))
        {
            var role2 = (PromotedGodfather)role;
            role2.BlackmailedPlayer = null;
            role2.BlockTarget = null;
            role2.MeasuredPlayer = null;
            role2.DisguisedPlayer = null;
            role2.CopiedPlayer = null;
            role2.SampledPlayer = null;
            role2.MorphedPlayer = null;
            role2.AmbushedPlayer = null;
            role2.BombedPlayer = null;
            role2.TeleportPoint = Vector3.zero;

            if (local.HasDied() && DeadSeeEverything)
                role2.Investigated.Clear();
        }
        else if (local.Is(LayerEnum.Janitor))
            ((Janitor)role).CurrentlyDragging = null;
        else if (local.Is(LayerEnum.Morphling))
        {
            var role2 = (Morphling)role;
            role2.SampledPlayer = null;
            role2.MorphedPlayer = null;
        }
        else if (local.Is(LayerEnum.Teleporter))
            ((Teleporter)role).TeleportPoint = Vector3.zero;
        else if (local.Is(LayerEnum.Ambusher))
            ((Ambusher)role).AmbushedPlayer = null;
        else if (local.Is(LayerEnum.Concealer))
            ((Concealer)role).ConcealedPlayer = null;
        else if (local.Is(LayerEnum.Silencer))
            ((Silencer)role).SilencedPlayer = null;
        else if (local.Is(LayerEnum.Bomber))
        {
            var role2 = (Bomber)role;

            if (CustomGameOptions.BombsRemoveOnNewRound && meeting)
                Bomb.Clear(role2.Bombs);
        }
        else if (local.Is(LayerEnum.Framer))
        {
            var role2 = (Framer)role;

            if (local.HasDied())
                role2.Framed.Clear();
        }
        else if (local.Is(LayerEnum.Crusader))
            ((Crusader)role).CrusadedPlayer = null;
        else if (local.Is(LayerEnum.Poisoner))
            ((Poisoner)role).PoisonedPlayer = null;
        else if (local.Is(LayerEnum.PromotedRebel))
        {
            var role2 = (PromotedRebel)role;
            role2.ShapeshiftPlayer1 = null;
            role2.ShapeshiftPlayer2 = null;
            role2.PoisonedPlayer = null;
            role2.ConcealedPlayer = null;
            role2.WarpPlayer1 = null;
            role2.WarpPlayer2 = null;
            role2.Positive = null;
            role2.Negative = null;
            role2.SilencedPlayer = null;
            role2.CrusadedPlayer = null;

            if (CustomGameOptions.BombsRemoveOnNewRound && meeting)
                Bomb.Clear(role2.Bombs);
        }
        else if (local.Is(LayerEnum.Shapeshifter))
        {
            var role2 = (Shapeshifter)role;
            role2.ShapeshiftPlayer1 = null;
            role2.ShapeshiftPlayer2 = null;
        }
        else if (local.Is(LayerEnum.Warper))
        {
            var role2 = (Warper)role;
            role2.WarpPlayer1 = null;
            role2.WarpPlayer2 = null;
        }
        else if (local.Is(LayerEnum.Collider))
        {
            var role2 = (PlayerLayers.Roles.Collider)role;
            role2.Positive = null;
            role2.Negative = null;
        }
        else if (local.Is(LayerEnum.Glitch))
        {
            var role2 = (Glitch)role;
            role2.MimicTarget = null;
            role2.HackTarget = null;
        }
        else if (local.Is(LayerEnum.GuardianAngel))
        {
            var role2 = (GuardianAngel)role;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;
        }
        else if (local.Is(LayerEnum.Actor))
        {
            var role2 = (Actor)role;

            if (meeting && role2.TargetRole == null)
                role2.Rounds++;
        }
        else if (local.Is(LayerEnum.BountyHunter))
        {
            var role2 = (BountyHunter)role;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;
        }
        else if (local.Is(LayerEnum.Executioner))
        {
            var role2 = (Executioner)role;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;
        }
        else if (local.Is(LayerEnum.Guesser))
        {
            var role2 = (Guesser)role;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;
        }
    }
}