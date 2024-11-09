
namespace TownOfUsReworked.Monos;

public class LayerHandler : RoleBehaviour
{
    public override bool IsDead => Player?.Data?.IsDead ?? false;
    public override bool IsAffectedByComms => false;

    [HideFromIl2Cpp]
    public Role CustomRole { get; set; }

    [HideFromIl2Cpp]
    public Ability CustomAbility { get; set; }

    [HideFromIl2Cpp]
    public Modifier CustomModifier { get; set; }

    [HideFromIl2Cpp]
    public Disposition CustomDisposition { get; set; }

    public static RoleBehaviour Crewmate;
    public static RoleBehaviour Impostor;
    public static RoleBehaviour CrewmateGhost;
    public static RoleBehaviour ImpostorGhost;

    [HideFromIl2Cpp]
    public List<PlayerLayer> GetLayers() => [ CustomRole, CustomModifier , CustomAbility, CustomDisposition ];

    [HideFromIl2Cpp]
    public T GetLayer<T>() where T : PlayerLayer => GetLayers().Find(x => x is T) as T;

    public void FixedPlayerUpdate()
    {
        CustomRole.UpdatePlayer();
        CustomAbility.UpdatePlayer();
        CustomModifier.UpdatePlayer();
        CustomDisposition.UpdatePlayer();
    }

    public void HudUpdate(HudManager __instance)
    {
        CustomRole.UpdateHud(__instance);
        CustomAbility.UpdateHud(__instance);
        CustomModifier.UpdateHud(__instance);
        CustomDisposition.UpdateHud(__instance);
        Player.GetButtons().ForEach(x => x.SetActive());
        CanVent = Player.CanVent();
    }

    public override float GetAbilityDistance() => GameSettings.InteractionDistance;

    public override void OnDeath(DeathReason reason)
    {
        CustomRole.OnDeath(reason);
        CustomAbility.OnDeath(reason);
        CustomModifier.OnDeath(reason);
        CustomDisposition.OnDeath(reason);
    }

    public override bool DidWin(GameOverReason gameOverReason) => CustomRole.Winner || CustomDisposition.Winner;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl.AmOwner)
            PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = "Achieve your win condition!";
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder) {}

    public override void Initialize(PlayerControl player)
    {
        Player = player;
        CustomRole = Player.GetRole();
        CustomAbility = Player.GetAbility();
        CustomModifier = Player.GetModifier();
        CustomDisposition = Player.GetDisposition();
        NameColor = CustomRole.Color;
    }

    public override void OnMeetingStart()
    {
        CustomRole.OnMeetingStart(Meeting());
        CustomAbility.OnMeetingStart(Meeting());
        CustomModifier.OnMeetingStart(Meeting());
        CustomDisposition.OnMeetingStart(Meeting());
    }

    public override bool CanUse(IUsable console)
    {
        // This is such a cheesy way to handle this omg
        var isCrew = CustomRole.Faction is Faction.Neutral or Faction.Crew || (CustomRole.Faction == Faction.GameMode && CustomRole.Type != LayerEnum.Hunter);
        var role = Player.HasDied() ? (isCrew ? CrewmateGhost : ImpostorGhost) : (isCrew ? Crewmate : Impostor);
        role.Player = Player;
        var result = role.CanUse(console);
        role.Player = null;
        return result;
    }
}