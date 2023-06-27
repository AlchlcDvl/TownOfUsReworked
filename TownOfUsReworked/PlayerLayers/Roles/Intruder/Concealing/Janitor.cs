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

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            StartText = () => "Sanitise The Ship, By Any Means Neccessary";
            AbilitiesText = () => $"- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported\n{CommonAbilities}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
            RoleType = RoleEnum.Janitor;
            RoleAlignment = RoleAlignment.IntruderConceal;
            InspectorResults = InspectorResults.DealsWithDead;
            CurrentlyDragging = null;
            Type = LayerEnum.Janitor;
            CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean);
            DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag);
            DropButton = new(this, "Drop", AbilityTypes.Effect, "Tertiary", Drop);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float CleanTimer()
        {
            var timespan = DateTime.UtcNow - LastCleaned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.JanitorCleanCd, ConstantVariables.LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus : 0) *
                1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DragTimer()
        {
            var timespan = DateTime.UtcNow - LastDragged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DragCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Clean()
        {
            if (CleanTimer() != 0f || Utils.IsTooFar(Player, CleanButton.TargetBody))
                return;

            var playerId = CleanButton.TargetBody.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FadeBody);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Coroutines.Start(Utils.FadeBody(CleanButton.TargetBody));
            LastCleaned = DateTime.UtcNow;

            if (CustomGameOptions.JaniCooldownsLinked)
                LastKilled = DateTime.UtcNow;
        }

        public void Drag()
        {
            if (Utils.IsTooFar(Player, DragButton.TargetBody) || CurrentlyDragging)
                return;

            var playerId = DragButton.TargetBody.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Drag);
            writer.Write(PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            CurrentlyDragging = DragButton.TargetBody;
            var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
            drag.Source = Player;
            drag.Dragged = CurrentlyDragging;
            drag.Body = CurrentlyDragging.gameObject.AddComponent<Rigidbody2D>();
            drag.Collider = CurrentlyDragging.gameObject.GetComponent<Collider2D>();
        }

        public void Drop()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Drop);
            writer.Write(CurrentlyDragging.ParentId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            foreach (var component in CurrentlyDragging?.bodyRenderers)
                component.material.SetFloat("_Outline", 0f);

            CurrentlyDragging.gameObject.GetComponent<DragBehaviour>().Destroy();
            CurrentlyDragging = null;
            LastDragged = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            CleanButton.Update("CLEAN", CleanTimer(), CustomGameOptions.JanitorCleanCd, true, CurrentlyDragging == null);
            DragButton.Update("DRAG", DragTimer(), CustomGameOptions.DragCd, true, CurrentlyDragging == null);
            DropButton.Update("DROP", true, CurrentlyDragging != null);
        }
    }
}
