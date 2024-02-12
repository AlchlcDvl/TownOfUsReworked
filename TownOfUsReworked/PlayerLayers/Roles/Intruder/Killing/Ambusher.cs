namespace TownOfUsReworked.PlayerLayers.Roles;

public class Ambusher : Intruder
{
    public PlayerControl AmbushedPlayer { get; set; }
    public CustomButton AmbushButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Ambusher : CustomColorManager.Intruder;
    public override string Name => "Ambusher";
    public override LayerEnum Type => LayerEnum.Ambusher;
    public override Func<string> StartText => () => "Spook The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n{CommonAbilities}";

    public Ambusher() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderKill;
        AmbushedPlayer = null;
        AmbushButton = new(this, "Ambush", AbilityTypes.Alive, "Secondary", Ambush, CustomGameOptions.AmbushCd, CustomGameOptions.AmbushDur, UnAmbush, Exception1);
        return this;
    }

    public void UnAmbush() => AmbushedPlayer = null;

    public void Ambush()
    {
        var cooldown = Interact(Player, AmbushButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            AmbushedPlayer = AmbushButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else
            AmbushButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !CustomGameOptions.AmbushMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.AmbushMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        AmbushButton.Update2("AMBUSH");
    }

    public override void TryEndEffect() => AmbushButton.Update3(IsDead || (AmbushedPlayer != null && AmbushedPlayer.HasDied()));

    public override void ReadRPC(MessageReader reader) => AmbushedPlayer = reader.ReadPlayer();
}