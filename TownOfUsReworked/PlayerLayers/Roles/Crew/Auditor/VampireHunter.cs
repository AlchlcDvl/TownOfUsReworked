namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class VampireHunter : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float StakeCd { get; set; } = 25f;

    public static bool VampsDead => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && x.Is(SubFaction.Undead));
    private CustomButton StakeButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.VampireHunter : CustomColorManager.Crew;
    public override string Name => "Vampire Hunter";
    public override LayerEnum Type => LayerEnum.VampireHunter;
    public override Func<string> StartText => () => "Stake The <color=#7B8968FF>Undead</color>";
    public override Func<string> Description => () => "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <color=#7B8968FF>Undead</color> " +
        "tries to interact with you, you will kill them\n- When all <color=#7B8968FF>Undead</color> players die, you will become a <color=#FFFF00FF>Vigilante</color>";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewAudit;
        StakeButton = CreateButton(this, "STAKE", new SpriteName("Stake"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Stake, new Cooldown(CustomGameOptions.StakeCd));
    }

    public void TurnVigilante() => new Vigilante().Start<Role>(Player).RoleUpdate(this);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (VampsDead && !Dead)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
            TurnVigilante();
        }
    }

    private void Stake() => StakeButton.StartCooldown(Interact(Player, StakeButton.TargetPlayer, ShouldKill(StakeButton.TargetPlayer)));

    private static bool ShouldKill(PlayerControl player) => player.Is(SubFaction.Undead) || player.IsFramed();
}