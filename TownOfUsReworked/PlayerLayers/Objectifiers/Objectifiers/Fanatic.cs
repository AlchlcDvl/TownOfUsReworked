namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public bool Turned;
        public bool Betrayed;
        public Faction Side = Faction.Crew;
        public bool Betray => ((Side == Faction.Intruder && ConstantVariables.LastImp) || (Side == Faction.Syndicate && ConstantVariables.LastSyn)) && !IsDead && Turned && !Betrayed;

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            Symbol = "♠";
            TaskText = "- Get attacked by either an <color=#FF0000FF>Intruder</color> or a <color=#008000FF>Syndicate</color> to join their side";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            Hidden = !CustomGameOptions.FanaticKnows && !Turned;
            Type = LayerEnum.Fanatic;
        }

        public void TurnFanatic(Faction faction)
        {
            var fanaticRole = Role.GetRole(Player);
            fanaticRole.Faction = faction;
            Turned = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Utils.Flash(Colors.Mystic);

            if (faction == Faction.Syndicate)
            {
                Color = Colors.Syndicate;
                fanaticRole.IsSynFanatic = true;
                fanaticRole.FactionColor = Colors.Syndicate;
                fanaticRole.Objectives = Role.SyndicateWinCon;
            }
            else if (faction == Faction.Intruder)
            {
                Color = Colors.Intruder;
                fanaticRole.IsIntFanatic = true;
                fanaticRole.FactionColor = Colors.Intruder;
                fanaticRole.Objectives = Role.IntrudersWinCon;
            }

            Side = faction;
            Hidden = false;
            fanaticRole.RoleAlignment = fanaticRole.RoleAlignment.GetNewAlignment(fanaticRole.Faction);
            Player.RegenTask();

            foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
            {
                if (CustomGameOptions.SnitchSeesFanatic)
                {
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && Player == PlayerControl.LocalPlayer)
                    {
                        var gameObj = new GameObject("SnitchArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        Role.LocalRole.AllArrows.Add(snitch.PlayerId, arrow);
                    }
                    else if (snitch.TasksDone && snitch.Player == PlayerControl.LocalPlayer)
                    {
                        var gameObj = new GameObject("SnitchEvilArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        Role.LocalRole.AllArrows.Add(PlayerControl.LocalPlayer.PlayerId, arrow);
                    }
                }
            }

            foreach (var revealer in Role.GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (revealer.Revealed && CustomGameOptions.RevealerRevealsFanatic && Player == PlayerControl.LocalPlayer)
                {
                    var gameObj = new GameObject("RevealerArrow") { layer = 5 };
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.GetSprite("Arrow");
                    arrow.image = renderer;
                    Role.LocalRole.AllArrows.Add(revealer.PlayerId, arrow);
                }
            }
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player) { Objectives = role.Objectives };
            betrayer.RoleUpdate(role);
            Betrayed = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Betray && Turned)
            {
                TurnBetrayer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnFanaticBetrayer);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}