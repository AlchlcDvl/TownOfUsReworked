namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mystic : Crew
{
    private bool ConvertedDead => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.Is(SubFaction.None) && !x.Is(SubFaction));
    private CustomButton RevealButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Mystic : CustomColorManager.Crew;
    public override string Name => "Mystic";
    public override LayerEnum Type => LayerEnum.Mystic;
    public override Func<string> StartText => () => "You Know When Converts Happen";
    public override Func<string> Description => () => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to it\n-" +
        " When all converted and converters die, you will become a <color=#71368AFF>Seer</color>";

    public Mystic() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewAudit;
        RevealButton = new(this, "MysticReveal", AbilityTypes.Alive, "ActionSecondary", Reveal, CustomGameOptions.MysticRevealCd, Exception);
        return this;
    }

    public void TurnSeer() => new Seer().Start<Role>(Player).RoleUpdate(this);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RevealButton.Update2("REVEAL");

        if (ConvertedDead && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnSeer, this);
            TurnSeer();
        }
    }

    private void Reveal()
    {
        var cooldown = Interact(Player, RevealButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            Flash((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(Alignment.NeutralNeo)) || RevealButton.TargetPlayer.IsFramed()
                ? UColor.red : UColor.green);
        }

        RevealButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;
}