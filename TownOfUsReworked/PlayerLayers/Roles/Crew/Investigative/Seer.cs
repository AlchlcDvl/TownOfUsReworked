namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Seer : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float SeerCd { get; set; } = 25f;

    public bool ChangedDead => !AllRoles.Any(x => x.Player && !x.Player.HasDied() && (x.RoleHistory.Any() || x.Is(LayerEnum.Amnesiac) || x.Is(LayerEnum.Thief) || x.Is(LayerEnum.Actor) ||
        x.Player.Is(LayerEnum.Traitor) || x.Is(LayerEnum.VampireHunter) || x.Is(LayerEnum.Godfather) || x.Is(LayerEnum.Mafioso) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Guesser) ||
        x.Is(LayerEnum.Rebel) || x.Is(LayerEnum.Mystic) || x.Is(LayerEnum.Sidekick) || x.Is(LayerEnum.GuardianAngel) || x.Is(LayerEnum.Executioner) || x.Player.Is(LayerEnum.Fanatic) ||
        x.Is(LayerEnum.BountyHunter) || x.Is(LayerEnum.PromotedGodfather) || x.Is(LayerEnum.PromotedRebel)));
    public CustomButton SeerButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Seer : CustomColorManager.Crew;
    public override string Name => "Seer";
    public override LayerEnum Type => LayerEnum.Seer;
    public override Func<string> StartText => () => "You Can See People's Histories";
    public override Func<string> Description => () => "- You can investigate players to see if their roles have changed\n- If all players whose roles changed have died, you will become a " +
        "<color=#FFCC80FF>Sheriff</color>";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewInvest;
        SeerButton = CreateButton(this, "ENVISION", new SpriteName("Seer"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)See, new Cooldown(CustomGameOptions.SeerCd));
        Data.Role.IntroSound = GetAudio("SeerIntro");
    }

    public void TurnSheriff() => new Sheriff().Start<Role>(Player).RoleUpdate(this);

    public void See()
    {
        var cooldown = Interact(Player, SeerButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Flash(SeerButton.TargetPlayer.GetRole().RoleHistory.Any() || SeerButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);

        SeerButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (ChangedDead && !Dead)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
            TurnSheriff();
        }
    }
}