using static TownOfUsReworked.Languages.Language;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Operative : Crew
    {
        public List<Bug> Bugs = new();
        public DateTime LastBugged;
        public int UsesLeft;
        public List<RoleEnum> BuggedPlayers = new();
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton BugButton;
        public Dictionary<byte, TMP_Text> PlayerNumbers = new();

        public Operative(PlayerControl player) : base(player)
        {
            Name = GetString("Operative");
            StartText = () => GetString("OperativeStartText");
            AbilitiesText = () => GetString("OperativeAbilitiesText1")
                + GetString("OperativeAbilitiesText2");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Operative : Colors.Crew;
            RoleType = RoleEnum.Operative;
            BuggedPlayers = new();
            UsesLeft = CustomGameOptions.MaxBugs;
            RoleAlignment = RoleAlignment.CrewInvest;
            Bugs = new();
            InspectorResults = InspectorResults.DropsItems;
            PlayerNumbers = new();
            Type = LayerEnum.Operative;
            BugButton = new(this, "Bug", AbilityTypes.Effect, "ActionSecondary", PlaceBug, true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void OnLobby()
        {
            base.OnLobby();
            Bug.Clear(Bugs);
            Bugs.Clear();
        }

        public float BugTimer()
        {
            var timespan = DateTime.UtcNow - LastBugged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void GenNumber(PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            var nameText = UObject.Instantiate(voteArea.NameText, voteArea.transform);
            nameText.transform.localPosition = new(-1.211f, -0.18f, -0.1f);
            nameText.text = GameData.Instance.GetPlayerById(targetId).DefaultOutfit.ColorId.ToString();
            PlayerNumbers[targetId] = nameText;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var role2 in GetRoles<Operative>(RoleEnum.Operative))
                role2.PlayerNumbers.Clear();

            foreach (var voteArea in __instance.playerStates)
                GenNumber(voteArea);

            if (BuggedPlayers.Count == 0)
                Utils.HUD.Chat.AddChat(PlayerControl.LocalPlayer, GetString("OperativeBugChat1"));
            else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                Utils.HUD.Chat.AddChat(PlayerControl.LocalPlayer, GetString("OperativeBugChat2"));
            else if (BuggedPlayers.Count == 1)
                Utils.HUD.Chat.AddChat(PlayerControl.LocalPlayer, GetString("OperativeBugChat3").Replace("%BuggedPlayers%", $"{BuggedPlayers}"));
            else
            {
                var message = GetString("OperativeMessage");
                var position = 0;
                BuggedPlayers.Shuffle();

                foreach (var role in BuggedPlayers)
                {
                    if (position < BuggedPlayers.Count - 1)
                        message += $" {role},";
                    else
                        message += GetString("OperativeMessageAndRole").Replace("%role%", $"{role}");

                    position++;
                }

                Utils.HUD.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BugButton.Update("BUG", BugTimer(), CustomGameOptions.BugCooldown, UsesLeft, ButtonUsable);
        }

        public void PlaceBug()
        {
            if (BugTimer() != 0f || !ButtonUsable)
                return;

            UsesLeft--;
            LastBugged = DateTime.UtcNow;
            Bugs.Add(new(Player.GetTruePosition()));
        }
    }
}
