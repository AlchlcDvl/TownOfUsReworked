namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Mystic)]
public sealed class Mystic : Investigative
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number RevealCd = 25;

    private bool ConvertedDead => !AllPlayers().Any(x => !x.HasDied() && !x.Is(Faction.Crew) && !x.Is(Handler.CurrentFaction));
    private CustomButton RevealButton;

    protected override UColor MainColor => CustomColorManager.Mystic;
    public override Layer Type => Layer.Mystic;
    public override string StartText => "You Know When Converts Happen";
    public override string Description => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to it\n-" +
        " When all converted and converters die, you will become a <#71368AFF>Seer</color>";

    public override void Init()
    {
        base.Init();
        RevealButton ??= new(this, "REVEAL", new SpriteName("MysticReveal"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Reveal, (PlayerBodyExclusion)Exception,
            new Cooldown(RevealCd));
    }

    private void TurnSeer() => new Seer().RoleUpdate(this);

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (ConvertedDead && !Dead)
            TurnSeer();
    }

    private void Reveal(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash((!target.Is(Handler.CurrentFaction) && !target.Is(Faction.Crew)) || target.IsFramed() ? UColor.red : UColor.green);

        RevealButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);
}