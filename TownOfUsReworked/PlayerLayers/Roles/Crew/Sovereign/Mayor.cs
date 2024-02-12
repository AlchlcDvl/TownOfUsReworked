namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mayor : Crew
{
    public bool RoundOne { get; set; }
    public CustomButton RevealButton { get; set; }
    public bool Revealed { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Mayor : CustomColorManager.Crew;
    public override string Name => "Mayor";
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText => () => "Reveal Yourself To Commit Voter Fraud";
    public override Func<string> Description => () => $"- You can reveal yourself to the crew\n- When revealed, your votes count {CustomGameOptions.MayorVoteCount + 1} times but you cannot "
        + "be protected";

    public Mayor() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewSov;
        RevealButton = new(this, "MayorReveal", AbilityTypes.Targetless, "ActionSecondary", Reveal);
        Data.Role.IntroSound = GetAudio("MayorIntro");
        return this;
    }

    public void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RevealButton.Update2("REVEAL", !Revealed && !RoundOne);
    }
}