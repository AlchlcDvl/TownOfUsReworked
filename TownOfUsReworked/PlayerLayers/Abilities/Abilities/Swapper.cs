namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Swapper : Ability
    {
        public readonly Dictionary<byte, GameObject> MoarButtons = new();
        public readonly Dictionary<byte, bool> Actives = new();
        public PlayerVoteArea Swap1;
        public PlayerVoteArea Swap2;

        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            TaskText = () => "- You can swap the votes against 2 players in meetings";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Swapper : Colors.Ability;
            AbilityType = AbilityEnum.Swapper;
            MoarButtons = new();
            Actives = new();
            Swap1 = null;
            Swap2 = null;
            Type = LayerEnum.Swapper;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
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
            targetBox.name = "SwapButton";
            targetBox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetManager.GetSprite("SwapDisabled");
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick = new();
            button.OnClick.AddListener(SetActive(voteArea, __instance));
            button.OnMouseOut = new();
            button.OnMouseOut.AddListener((Action)(() => renderer.color = Actives[voteArea.TargetPlayerId] ? UColor.green : UColor.white));
            button.OnMouseOver = new();
            button.OnMouseOver.AddListener((Action)(() => renderer.color = UColor.red));
            var collider = targetBox.GetComponent<BoxCollider2D>();
            collider.size = renderer.sprite.bounds.size;
            collider.offset = Vector2.zero;
            targetBox.transform.GetChild(0).gameObject.Destroy();
            MoarButtons.Add(voteArea.TargetPlayerId, targetBox);
            Actives.Add(voteArea.TargetPlayerId, false);
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            HideButtons();

            if (Swap1 == null || Swap2 == null)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.SetSwaps);
            writer.Write(PlayerId);
            writer.Write(Swap1.TargetPlayerId);
            writer.Write(Swap2.TargetPlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        private Action SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            void Listener()
            {
                if (__instance.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote && !CustomGameOptions.SwapAfterVoting) || __instance.state ==
                    MeetingHud.VoteStates.Discussion)
                {
                    return;
                }

                if (Swap1 == null)
                    Swap1 = voteArea;
                else if (Swap2 == null)
                    Swap2 = voteArea;
                else if (Swap1 == voteArea)
                {
                    Swap1 = null;
                    Actives[Swap1.TargetPlayerId] = false;
                    MoarButtons[Swap1.TargetPlayerId].GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("SwapDisabled");
                    MoarButtons[Swap1.TargetPlayerId].GetComponent<SpriteRenderer>().color = UColor.white;
                }
                else if (Swap2 == voteArea)
                {
                    Swap2 = null;
                    Actives[Swap2.TargetPlayerId] = false;
                    MoarButtons[Swap2.TargetPlayerId].GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("SwapDisabled");
                    MoarButtons[Swap2.TargetPlayerId].GetComponent<SpriteRenderer>().color = UColor.white;
                }
                else
                {
                    Actives[Swap1.TargetPlayerId] = false;
                    MoarButtons[Swap1.TargetPlayerId].GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("SwapDisabled");
                    MoarButtons[Swap1.TargetPlayerId].GetComponent<SpriteRenderer>().color = UColor.white;
                    Swap1 = Swap2;
                    Swap2 = voteArea;
                }

                Actives[voteArea.TargetPlayerId] = !Actives[voteArea.TargetPlayerId];

                foreach (var pair in MoarButtons)
                {
                    if (MoarButtons[pair.Key] == null)
                        continue;

                    MoarButtons[pair.Key].GetComponent<SpriteRenderer>().sprite = Actives[pair.Key] ? AssetManager.GetSprite("SwapActive") : AssetManager.GetSprite("SwapDisabled");
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
            return player.Data.IsDead || player.Data.Disconnected || (player == Player && player == CustomPlayer.Local && !CustomGameOptions.SwapSelf) || IsDead;
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);

            if (MeetingHud.Instance.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote && !CustomGameOptions.SwapAfterVoting))
                HideButtons();

            if (Swap1 == null || Swap2 == null)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.SetSwaps);
            writer.Write(PlayerId);
            writer.Write(Swap1.TargetPlayerId);
            writer.Write(Swap2.TargetPlayerId);
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