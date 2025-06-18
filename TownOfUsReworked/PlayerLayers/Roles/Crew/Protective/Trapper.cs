namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Trapper)]
public sealed class Trapper : Protective, ITrapper
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxTraps = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BuildCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number BuildDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TrapCd = 25;

    public HashSet<byte> Trapped { get; } = [];
    public bool Building { get ; private set; }

    private readonly List<Layer> TriggeredRoles = [];
    private CustomButton BuildButton;
    private CustomButton TrapButton;
    private int TrapsMade;
    private bool AttackedSomeone;

    protected override UColor MainColor => CustomColorManager.Trapper;
    public override Layer Type => Layer.Trapper;
    public override string StartText => "<size=90%>Use Your Tinkering Skills To Obstruct The <#FF0000FF>Evildoers</color></size>";
    public override string Description => "- You can build a trap, adding it to your armory\n- You can place these traps on players and either log the roles of interactors on " +
        "them\nor protect from an attack once and attack the attacker in return";

    public override void Init()
    {
        base.Init();
        Trapped.Clear();
        TriggeredRoles.Clear();
        BuildButton ??= new(this, "BUILD TRAP", new SpriteName("Build"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)StartBuilding, new Cooldown(BuildCd), MaxTraps,
            (UsableFunc)Usable, new Duration(BuildDur), (EffectEndVoid)EndBuilding, new CanClickAgain(false));
        TrapButton ??= new(this, "PLACE TRAP", new SpriteName("Trap"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetTrap, new Cooldown(TrapCd), MaxTraps,
            (PlayerBodyExclusion)Exception);
        TrapsMade = TrapButton.UsesCount = 0;
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Trapped.Contains(player.PlayerId))
            name += " <#BE1C8CFF>∮</color>";
    }

    private void StartBuilding()
    {
        BuildButton.Begin();
        Building = true;
    }

    private void EndBuilding()
    {
        TrapButton.Uses++;
        TrapsMade++;
        Building = false;
    }

    private void SetTrap(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            PerformRpcAction(TrapperActionsRpc.Place, target.PlayerId);
            Trapped.Add(target.PlayerId);
        }

        TrapButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Trapped.Contains(player.PlayerId);

    private bool Usable() => TrapsMade <= MaxTraps;

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (trigger.AmOwner)
            PerformRpcAction(TrapperActionsRpc.Trigger, trapped, trigger, isAttack);

        TriggeredRoles.Add(trigger.GetRole().Type);
        AttackedSomeone = isAttack;
        Trapped.Remove(trapped.PlayerId);
    }

    public override void ReadRPC(RpcReader reader)
    {
        var trapAction = reader.Read<TrapperActionsRpc>();

        switch (trapAction)
        {
            case TrapperActionsRpc.Place:
            {
                Trapped.Add(reader.ReadByte());
                break;
            }
            case TrapperActionsRpc.Trigger:
            {
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBool());
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {trapAction}");
                break;
            }
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        var message = "";

        if (AttackedSomeone)
            message = "Your trap attacked someone!";
        else if (TriggeredRoles.Any())
        {
            message = "Your trap detected the following roles: ";
            TriggeredRoles.Shuffle();
            message += Join(", ", TriggeredRoles.Select(x => LayerDictionary[x].Name));
        }

        if (!IsNullEmptyOrWhiteSpace(message))
            Run("<#8D0F8CFF>〖 Trap Triggers 〗</color>", message);

        TriggeredRoles.Clear();
    }
}