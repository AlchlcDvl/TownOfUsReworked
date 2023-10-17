namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mystic : Crew
{
    public static bool ConvertedDead => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.Is(SubFaction.None));
    public CustomButton RevealButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
    public override string Name => "Mystic";
    public override LayerEnum Type => LayerEnum.Mystic;
    public override Func<string> StartText => () => "You Know When Converts Happen";
    public override Func<string> Description => () => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to it\n-" +
        " When all converted and converters die, you will become a <color=#71368AFF>Seer</color>";

    public Mystic(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewAudit;
        RevealButton = new(this, "MysticReveal", AbilityTypes.Target, "ActionSecondary", Reveal, CustomGameOptions.MysticRevealCd, Exception);
    }

    public void TurnSeer() => new Seer(Player).RoleUpdate(this);

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

    public void Reveal()
    {
        var interact = Interact(Player, RevealButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            Flash((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(Alignment.NeutralNeo)) || RevealButton.TargetPlayer.IsFramed()
                ? UColor.red : UColor.green);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        RevealButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;
}