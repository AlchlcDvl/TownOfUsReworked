namespace TownOfUsReworked.PlayerLayers.Roles;

public class Ambusher : Intruder
{
    public PlayerControl AmbushedPlayer { get; set; }
    public CustomButton AmbushButton { get; set; }
    public bool OnAmbush => AmbushButton.EffectActive;

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Ambusher : Colors.Intruder;
    public override string Name => "Ambusher";
    public override LayerEnum Type => LayerEnum.Ambusher;
    public override Func<string> StartText => () => "Spook The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n{CommonAbilities}";

    public Ambusher(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderKill;
        AmbushedPlayer = null;
        AmbushButton = new(this, "Ambush", AbilityTypes.Target, "Secondary", Ambush, CustomGameOptions.AmbushCd, CustomGameOptions.AmbushDur, UnAmbush, Exception1);
    }

    public void UnAmbush() => AmbushedPlayer = null;

    public void Ambush()
    {
        var interact = Interact(Player, AmbushButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            AmbushedPlayer = AmbushButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else if (interact.Reset)
            AmbushButton.StartCooldown(CooldownType.Reset);
        else if (interact.Protected)
            AmbushButton.StartCooldown(CooldownType.GuardianAngel);
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