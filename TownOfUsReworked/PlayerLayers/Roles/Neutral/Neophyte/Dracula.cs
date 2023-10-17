namespace TownOfUsReworked.PlayerLayers.Roles;

public class Dracula : Neutral
{
    public CustomButton BiteButton { get; set; }
    public List<byte> Converted { get; set; }
    public static int AliveCount => CustomPlayer.AllPlayers.Count(x => !x.HasDied());

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
    public override string Name => "Dracula";
    public override LayerEnum Type => LayerEnum.Dracula;
    public override Func<string> StartText => () => "Lead The <color=#7B8968FF>Undead</color> To Victory";
    public override Func<string> Description => () => "- You can convert the <color=#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the " +
        $"number of alive <color=#7B8968FF>Undead</color> exceeds {CustomGameOptions.AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire " +
        "Hunter</color> will force them to kill you";

    public Dracula(PlayerControl player) : base(player)
    {
        Objectives = () => "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
        SubFaction = SubFaction.Undead;
        Alignment = Alignment.NeutralNeo;
        SubFactionColor = Colors.Undead;
        Converted = new() { Player.PlayerId };
        BiteButton = new(this, "Bite", AbilityTypes.Target, "ActionSecondary", Convert, CustomGameOptions.BiteCd, Exception);
        SubFactionSymbol = "γ";
    }

    public void Convert()
    {
        var interact = Interact(Player, BiteButton.TargetPlayer, false, true);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            RoleGen.RpcConvert(BiteButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= CustomGameOptions.AliveVampCount);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        BiteButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Converted.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BiteButton.Update2("BITE");
    }
}