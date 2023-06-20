namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mafioso : Intruder
    {
        public Role FormerRole;
        public Godfather Godfather;
        public bool CanPromote => (Godfather.IsDead || Godfather.Disconnected) && !IsDead;

        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            RoleType = RoleEnum.Mafioso;
            StartText = () => "Succeed The <color=#404C08FF>Godfather</color>";
            AbilitiesText = () => "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities of your former " +
                $"role\n{CommonAbilities}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Mafioso : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderUtil;
            Type = LayerEnum.Mafioso;
            InspectorResults = InspectorResults.IsCold;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void TurnGodfather()
        {
            var newRole = new PromotedGodfather(Player)
            {
                FormerRole = FormerRole,
                RoleBlockImmune = FormerRole.RoleBlockImmune,
                RoleAlignment = FormerRole.RoleAlignment
            };

            newRole.RoleUpdate(this);

            if (Local && !IntroCutscene.Instance)
                Utils.Flash(Colors.Godfather);

            if (CustomPlayer.Local.Is(RoleEnum.Seer) && !IntroCutscene.Instance)
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (CanPromote)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnGodfather);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnGodfather();
            }
        }
    }
}