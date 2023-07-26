namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Janitor : Intruder
    {
        public CustomButton CleanButton;
        public CustomButton DragButton;
        public CustomButton DropButton;
        public DateTime LastDragged;
        public DeadBody CurrentlyDragging;
        public DateTime LastCleaned;

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
        public override string Name => "Janitor";
        public override LayerEnum Type => LayerEnum.Janitor;
        public override RoleEnum RoleType => RoleEnum.Janitor;
        public override Func<string> StartText => () => "You Know Their Secrets";
        public override Func<string> AbilitiesText => () => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting " +
            $"reported\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.DealsWithDead;

        public Janitor(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderConceal;
            CurrentlyDragging = null;
            CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean);
            DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag);
            DropButton = new(this, "Drop", AbilityTypes.Effect, "Tertiary", Drop);
        }

        public float CleanTimer()
        {
            var timespan = DateTime.UtcNow - LastCleaned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.JanitorCleanCd, LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus : 0) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float DragTimer()
        {
            var timespan = DateTime.UtcNow - LastDragged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DragCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Clean()
        {
            if (CleanTimer() != 0f || IsTooFar(Player, CleanButton.TargetBody))
                return;

            Spread(Player, PlayerByBody(CleanButton.TargetBody));
            CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, this, CleanButton.TargetBody);
            Coroutines.Start(FadeBody(CleanButton.TargetBody));
            LastCleaned = DateTime.UtcNow;

            if (CustomGameOptions.JaniCooldownsLinked)
                LastKilled = DateTime.UtcNow;
        }

        public void Drag()
        {
            if (IsTooFar(Player, DragButton.TargetBody) || CurrentlyDragging)
                return;

            CurrentlyDragging = DragButton.TargetBody;
            Spread(Player, PlayerByBody(CurrentlyDragging));
            CallRpc(CustomRPC.Action, ActionsRPC.Drag, this, CurrentlyDragging);
            var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
            drag.Source = Player;
            drag.Dragged = CurrentlyDragging;
        }

        public void Drop()
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Drop, CurrentlyDragging);

            foreach (var component in CurrentlyDragging?.bodyRenderers)
                component.material.SetFloat("_Outline", 0f);

            CurrentlyDragging.gameObject.GetComponent<DragBehaviour>().Destroy();
            CurrentlyDragging = null;
            LastDragged = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            CleanButton.Update("CLEAN", CleanTimer(), CustomGameOptions.JanitorCleanCd, LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus : 0, true,
                CurrentlyDragging == null);
            DragButton.Update("DRAG", DragTimer(), CustomGameOptions.DragCd, true, CurrentlyDragging == null);
            DropButton.Update("DROP", true, CurrentlyDragging != null);
        }
    }
}
