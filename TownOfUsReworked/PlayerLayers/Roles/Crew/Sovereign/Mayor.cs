namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mayor : Crew
{
    public bool RoundOne { get; set; }
    public CustomButton RevealButton { get; set; }
    public bool Revealed { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
    public override string Name => "Mayor";
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText => () => "Reveal Yourself To Commit Voter Fraud";
    public override Func<string> Description => () => $"- You can reveal yourself to the crew\n- When revealed, your votes count {CustomGameOptions.MayorVoteCount + 1} times but you cannot "
        + "be protected";

    public Mayor(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSov;
        RevealButton = new(this, "MayorReveal", AbilityTypes.Targetless, "ActionSecondary", Reveal);
        player.Data.Role.IntroSound = GetAudio("MayorIntro");
    }

    public void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this);
        Revealed = true;
        Flash(Color);
        BreakShield(Player, true);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RevealButton.Update2("REVEAL", !Revealed && !RoundOne);
    }

    public override void ReadRPC(MessageReader reader)
    {
        Revealed = true;
        Flash(Colors.Mayor);
        BreakShield(Player, true);
    }
}