namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Dictator : Crew
    {
        public bool RoundOne;
        public bool Revealed;
        public List<byte> ToBeEjected;
        public readonly Dictionary<byte, GameObject> MoarButtons = new();
        public readonly Dictionary<byte, bool> Actives = new();
        public CustomButton RevealButton;
        public bool Ejected;
        public bool ToDie;

        public Dictator(PlayerControl player) : base(player)
        {
            Name = "Dictator";
            StartText = () => "You Have The Final Say";
            AbilitiesText = () => "- You can reveal yourself to the crew to eject up to 3 players in a meeting\n- When revealed, you cannot be protected";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Dictator : Colors.Crew;
            RoleType = RoleEnum.Dictator;
            RoleAlignment = RoleAlignment.CrewSov;
            InspectorResults = InspectorResults.Manipulative;
            Type = LayerEnum.Dictator;
            ToBeEjected = new();
            Ejected = false;
            ToDie = false;
            RevealButton = new(this, "DictReveal", AbilityTypes.Effect, "ActionSecondary", Reveal);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void Reveal()
        {
            if (RoundOne)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.DictatorReveal);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Revealed = true;
            Utils.Flash(Color);
            BreakShield(PlayerId, true);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", !Revealed, !Revealed && !RoundOne);
        }

        public void GenButton(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (IsExempt(voteArea))
            {
                MoarButtons.Add(voteArea.TargetPlayerId, null);
                Actives.Add(voteArea.TargetPlayerId, false);
                return;
            }

            var template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            var targetBox = UObject.Instantiate(template, voteArea.transform);
            targetBox.name = "DictateButton";
            targetBox.transform.localPosition = new(-0.4f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetManager.GetSprite("DictDisabled");
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick = new();
            button.OnClick.AddListener(SetActive(voteArea, __instance));
            button.OnMouseOut = new();
            button.OnMouseOut.AddListener((Action)(() => renderer.color = Actives[voteArea.TargetPlayerId] ? UColor.green : UColor.white));
            button.OnMouseOver = new();
            button.OnMouseOver.AddListener((Action)(() => renderer.color = UColor.red));
            var component2 = targetBox.GetComponent<BoxCollider2D>();
            component2.size = renderer.sprite.bounds.size;
            component2.offset = Vector2.zero;
            targetBox.transform.GetChild(0).gameObject.Destroy();
            MoarButtons.Add(voteArea.TargetPlayerId, targetBox);
            Actives.Add(voteArea.TargetPlayerId, false);
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            HideButtons();

            if (ToBeEjected.Count == 0)
                return;

            ToDie = ToBeEjected.Any(x => Utils.PlayerById(x).Is(Faction.Crew));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.SetExiles);
            writer.Write(PlayerId);
            writer.Write(ToDie);
            writer.WriteBytesAndSize(ToBeEjected.ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        private Action SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            void Listener()
            {
                if (__instance.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote && !CustomGameOptions.DictateAfterVoting) || __instance.state ==
                    MeetingHud.VoteStates.Discussion || !Revealed || Ejected || ToDie)
                {
                    return;
                }

                var id = voteArea.TargetPlayerId;

                if (ToBeEjected.Contains(id))
                {
                    ToBeEjected.Remove(id);
                    Actives[id] = false;
                }
                else
                {
                    ToBeEjected.Add(id);
                    Actives[id] = true;
                }

                if (ToBeEjected.Count > 3)
                {
                    Actives[ToBeEjected[0]] = false;
                    ToBeEjected.Remove(ToBeEjected[0]);
                }

                foreach (var pair in MoarButtons)
                {
                    if (MoarButtons[pair.Key] == null)
                        continue;

                    MoarButtons[pair.Key].GetComponent<SpriteRenderer>().sprite = Actives[pair.Key] ? AssetManager.GetSprite("DictActive") : AssetManager.GetSprite("DictDisabled");
                    MoarButtons[pair.Key].GetComponent<SpriteRenderer>().color = Actives[pair.Key] ? UColor.green : UColor.white;
                }
            }

            return Listener;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var area in __instance.playerStates)
                GenButton(area, __instance);
        }

        private bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead)
                return true;

            var player = Utils.PlayerByVoteArea(voteArea);
            return player.Data.IsDead || player.Data.Disconnected || (player == Player && player == CustomPlayer.Local) || IsDead || !Revealed || Ejected;
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);

            if (Utils.Meeting.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote && !CustomGameOptions.DictateAfterVoting))
                HideButtons();

            if (ToBeEjected.Count == 0)
                return;

            ToDie = ToBeEjected.Any(x => Utils.PlayerById(x).Is(Faction.Crew));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.SetExiles);
            writer.Write(PlayerId);
            writer.Write(ToDie);
            writer.WriteBytesAndSize(ToBeEjected.ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void HideSingle(byte targetId)
        {
            var button = MoarButtons[targetId];

            if (button == null)
                return;

            button.SetActive(false);
            button.GetComponent<PassiveButton>().OnClick = new();
            button.GetComponent<PassiveButton>().OnMouseOver = new();
            button.GetComponent<PassiveButton>().OnMouseOut = new();
            button.Destroy();
            MoarButtons[targetId] = null;
        }

        public void HideButtons()
        {
            for (byte i = 0; i < MoarButtons.Count; i++)
                HideSingle(i);
        }
    }
}