namespace TownOfUsReworked.PlayerLayers.Roles;

public class Seer : Crew
{
    public bool ChangedDead => !AllRoles.Any(x => x.Player != null && !x.Player.HasDied() && (x.RoleHistory.Count > 0 || x.Is(LayerEnum.Amnesiac) || x.Is(LayerEnum.Thief) ||
        x.Player.Is(LayerEnum.Traitor) || x.Is(LayerEnum.VampireHunter) || x.Is(LayerEnum.Godfather) || x.Is(LayerEnum.Mafioso) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Guesser) ||
        x.Is(LayerEnum.Rebel) || x.Is(LayerEnum.Mystic) || x.Is(LayerEnum.Sidekick) || x.Is(LayerEnum.GuardianAngel) || x.Is(LayerEnum.Executioner) || x.Player.Is(LayerEnum.Fanatic) ||
        x.Is(LayerEnum.BountyHunter) || x.Is(LayerEnum.PromotedGodfather) || x.Is(LayerEnum.PromotedRebel) || x.Is(LayerEnum.Actor)));
    public CustomButton SeerButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Seer : CustomColorManager.Crew;
    public override string Name => "Seer";
    public override LayerEnum Type => LayerEnum.Seer;
    public override Func<string> StartText => () => "You Can See People's Histories";
    public override Func<string> Description => () => "- You can investigate players to see if their roles have changed\n- If all players whose roles changed have died, you will become a " +
        "<color=#FFCC80FF>Sheriff</color>";

    public Seer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewInvest;
        SeerButton = new(this, "Seer", AbilityTypes.Alive, "ActionSecondary", See, CustomGameOptions.SeerCd);
        Data.Role.IntroSound = GetAudio("SeerIntro");
        return this;
    }

    public void TurnSheriff() => new Sheriff().Start<Role>(Player).RoleUpdate(this);

    public void See()
    {
        var cooldown = Interact(Player, SeerButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Flash(SeerButton.TargetPlayer.GetRole().RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);

        SeerButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SeerButton.Update2("SEE");

        if (ChangedDead && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnSheriff, this);
            TurnSheriff();
        }
    }
}