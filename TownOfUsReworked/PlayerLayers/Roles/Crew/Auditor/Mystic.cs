namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mystic : Crew
{
    public DateTime LastRevealed { get; set; }
    public static bool ConvertedDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(SubFaction.None));
    public CustomButton RevealButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastRevealed, CustomGameOptions.RevealCooldown);

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
    public override string Name => "Mystic";
    public override LayerEnum Type => LayerEnum.Mystic;
    public override Func<string> StartText => () => "You Know When Converts Happen";
    public override Func<string> Description => () => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to"
        + " it\n- When all converted and converters die, you will become a <color=#71368AFF>Seer</color>";
    public override InspectorResults InspectorResults => InspectorResults.TracksOthers;

    public Mystic(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewAudit;
        RevealButton = new(this, "MysticReveal", AbilityTypes.Direct, "ActionSecondary", Reveal, Exception);
    }

    public void TurnSeer()
    {
        var newRole = new Seer(Player);
        newRole.RoleUpdate(this);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RevealButton.Update("REVEAL", Timer, CustomGameOptions.RevealCooldown);

        if (ConvertedDead && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnSeer, this);
            TurnSeer();
        }
    }

    public void Reveal()
    {
        if (Timer != 0f || IsTooFar(Player, RevealButton.TargetPlayer))
            return;

        var interact = Interact(Player, RevealButton.TargetPlayer);

        if (interact[3])
        {
            Flash((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo)) ||
                RevealButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);
        }

        if (interact[0])
            LastRevealed = DateTime.UtcNow;
        else if (interact[1])
            LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;
}