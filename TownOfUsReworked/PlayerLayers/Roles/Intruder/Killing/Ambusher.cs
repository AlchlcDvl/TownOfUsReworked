namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Ambusher : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number AmbushCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number AmbushDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AmbushMates { get; set; } = false;

    public PlayerControl AmbushedPlayer { get; set; }
    public CustomButton AmbushButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Ambusher : CustomColorManager.Intruder;
    public override string Name => "Ambusher";
    public override LayerEnum Type => LayerEnum.Ambusher;
    public override Func<string> StartText => () => "Spook The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderKill;
        AmbushedPlayer = null;
        AmbushButton = CreateButton(this, new SpriteName("Ambush"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Ambush, new Cooldown(AmbushCd), (EndFunc)EndEffect, "AMBUSH",
            new Duration(AmbushDur), (EffectEndVoid)UnAmbush, (PlayerBodyExclusion)Exception1);
    }

    public void UnAmbush() => AmbushedPlayer = null;

    public void Ambush()
    {
        var cooldown = Interact(Player, AmbushButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            AmbushedPlayer = AmbushButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else
            AmbushButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !AmbushMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !AmbushMates);

    public bool EndEffect() => Dead || (AmbushedPlayer && AmbushedPlayer.HasDied());

    public override void ReadRPC(MessageReader reader) => AmbushedPlayer = reader.ReadPlayer();
}