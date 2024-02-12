namespace TownOfUsReworked.PlayerLayers.Roles;

public class Dracula : Neutral
{
    public CustomButton BiteButton { get; set; }
    public List<byte> Converted { get; set; }
    public static int AliveCount => CustomPlayer.AllPlayers.Count(x => !x.HasDied());

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Dracula : CustomColorManager.Neutral;
    public override string Name => "Dracula";
    public override LayerEnum Type => LayerEnum.Dracula;
    public override Func<string> StartText => () => "Lead The <color=#7B8968FF>Undead</color> To Victory";
    public override Func<string> Description => () => "- You can convert the <color=#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the " +
        $"number of alive <color=#7B8968FF>Undead</color> exceeds {CustomGameOptions.AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire " +
        "Hunter</color> will force them to kill you";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public Dracula() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
        SubFaction = SubFaction.Undead;
        Alignment = Alignment.NeutralNeo;
        SubFactionColor = CustomColorManager.Undead;
        Converted = new() { Player.PlayerId };
        BiteButton = new(this, "Bite", AbilityTypes.Alive, "ActionSecondary", Convert, CustomGameOptions.BiteCd, Exception);
        return this;
    }

    public void Convert()
    {
        var cooldown = Interact(Player, BiteButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RoleGen.RpcConvert(BiteButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= CustomGameOptions.AliveVampCount);

        BiteButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Converted.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BiteButton.Update2("BITE");
    }
}