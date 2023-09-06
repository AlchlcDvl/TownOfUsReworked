﻿namespace TownOfUsReworked.PlayerLayers.Roles;

public class SerialKiller : Neutral
{
    public CustomButton BloodlustButton { get; set; }
    public CustomButton StabButton { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastLusted { get; set; }
    public DateTime LastKilled { get; set; }
    public float TimeRemaining { get; set; }
    public bool Lusted => TimeRemaining > 0f;

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.SerialKiller : Colors.Neutral;
    public override string Name => "Serial Killer";
    public override LayerEnum Type => LayerEnum.SerialKiller;
    public override Func<string> StartText => () => "You Like To Play With Knives";
    public override Func<string> Description => () => "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <color=#803333FF>Escort" +
        "</color>, <color=#801780FF>Consort</color> or <color=#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are immune"
        + " to roleblocks";
    public override InspectorResults InspectorResults => InspectorResults.IsAggressive;
    public float LustTimer => ButtonUtils.Timer(Player, LastLusted, CustomGameOptions.BloodlustCd);
    public float StabTimer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.StabCd);

    public SerialKiller(PlayerControl player) : base(player)
    {
        Objectives = () => "- Stab anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        RoleBlockImmune = true;
        StabButton = new(this, "Stab", AbilityTypes.Direct, "ActionSecondary", Stab, Exception);
        BloodlustButton = new(this, "Bloodlust", AbilityTypes.Effect, "Secondary", Lust);
    }

    public void Bloodlust()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void Unbloodlust()
    {
        Enabled = false;
        LastLusted = DateTime.UtcNow;
    }

    public void Lust()
    {
        if (LustTimer != 0f || Lusted)
            return;

        TimeRemaining = CustomGameOptions.BloodlustDur;
    }

    public void Stab()
    {
        if (!Lusted || StabTimer != 0f || IsTooFar(Player, StabButton.TargetPlayer))
            return;

        var interact = Interact(Player, StabButton.TargetPlayer, true);

        if (interact.AbilityUsed || interact.Reset)
            LastKilled = DateTime.UtcNow;
        else if (interact.Protected)
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact.Vested)
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
        || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StabButton.Update("STAB", StabTimer, CustomGameOptions.StabCd, Lusted, Lusted);
        BloodlustButton.Update("BLOODLUST", LustTimer, CustomGameOptions.BloodlustCd, Lusted, TimeRemaining, CustomGameOptions.BloodlustDur);
    }
}