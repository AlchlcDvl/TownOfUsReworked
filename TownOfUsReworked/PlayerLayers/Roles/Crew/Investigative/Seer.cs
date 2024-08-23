namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Seer : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float SeerCd { get; set; } = 25f;

    public static bool ChangedDead => !AllRoles.Any(x => !x.Player.HasDied() && (x.RoleHistory.Any() || x.Type is LayerEnum.Amnesiac or LayerEnum.Thief or LayerEnum.Actor or
        LayerEnum.VampireHunter or LayerEnum.Godfather or LayerEnum.Mafioso or LayerEnum.Shifter or LayerEnum.Guesser or LayerEnum.Rebel or LayerEnum.Mystic or LayerEnum.Sidekick or
        LayerEnum.GuardianAngel or LayerEnum.Executioner or LayerEnum.BountyHunter or LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel || x.LinkedObjectifier is LayerEnum.Traitor or
        LayerEnum.Fanatic));
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
        SeerButton = CreateButton(this, "ENVISION", new SpriteName("Seer"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)See, new Cooldown(SeerCd));
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