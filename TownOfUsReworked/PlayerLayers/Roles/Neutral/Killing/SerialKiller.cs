namespace TownOfUsReworked.PlayerLayers.Roles;

public class SerialKiller : Neutral
{
    public CustomButton BloodlustButton { get; set; }
    public CustomButton StabButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.SerialKiller : Colors.Neutral;
    public override string Name => "Serial Killer";
    public override LayerEnum Type => LayerEnum.SerialKiller;
    public override Func<string> StartText => () => "You Like To Play With Knives";
    public override Func<string> Description => () => "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <color=#803333FF>Escort</color>," +
        " <color=#801780FF>Consort</color> or <color=#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are immune to roleblocks";

    public SerialKiller(PlayerControl player) : base(player)
    {
        Objectives = () => "- Stab anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        RoleBlockImmune = true;
        StabButton = new(this, "Stab", AbilityTypes.Target, "ActionSecondary", Stab, CustomGameOptions.StabCd, Exception);
        BloodlustButton = new(this, "Bloodlust", AbilityTypes.Targetless, "Secondary", Lust, CustomGameOptions.BloodlustCd, CustomGameOptions.BloodlustDur);
    }

    public void Lust() => BloodlustButton.Begin();

    public void Stab()
    {
        var interact = Interact(Player, StabButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        StabButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StabButton.Update2("STAB", BloodlustButton.EffectActive);
        BloodlustButton.Update2("BLOODLUST");
    }

    public override void TryEndEffect() => BloodlustButton.Update3(IsDead);
}