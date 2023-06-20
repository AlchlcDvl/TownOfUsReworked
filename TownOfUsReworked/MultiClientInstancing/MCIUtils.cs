namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch]
    public static class MCIUtils
    {
        public readonly static Dictionary<int, ClientData> Clients = new();
        public readonly static Dictionary<byte, int> PlayerIdClientId = new();

        public static int AvailableId()
        {
            for (var i = 1; i < 128; i++)
            {
                if (!Clients.ContainsKey(i) && CustomPlayer.Local.OwnerId != i)
                    return i;
            }

            return -1;
        }

        public static void CleanUpLoad()
        {
            if (GameData.Instance.AllPlayers.Count == 1)
            {
                Clients.Clear();
                PlayerIdClientId.Clear();
            }
        }

        public static PlayerControl CreatePlayerInstance()
        {
            var samplePSD = new PlatformSpecificData()
            {
                Platform = Platforms.StandaloneWin10,
                PlatformName = "Robot"
            };

            var sampleId = AvailableId();
            var sampleC = new ClientData(sampleId, $"Robot-{sampleId}", samplePSD, 5, "", "");

            AmongUsClient.Instance.CreatePlayer(sampleC);
            AmongUsClient.Instance.allClients.Add(sampleC);

            sampleC.Character.SetName($"Robot {sampleC.Character.PlayerId}");
            sampleC.Character.SetSkin(HatManager.Instance.allSkins[URandom.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetNamePlate(HatManager.Instance.allNamePlates[URandom.Range(0, HatManager.Instance.allNamePlates.Count)].ProdId);
            sampleC.Character.SetPet(HatManager.Instance.allPets[URandom.Range(0, HatManager.Instance.allPets.Count)].ProdId);
            sampleC.Character.SetHat("hat_NoHat", 0);
            sampleC.Character.SetColor(URandom.Range(0, Palette.PlayerColors.Length));

            Clients.Add(sampleId, sampleC);
            PlayerIdClientId.Add(sampleC.Character.PlayerId, sampleId);
            return sampleC.Character;
        }

        public static void RemovePlayer(byte id)
        {
            if (id == 0)
                return;

            var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
            Clients.Remove(clientId, out var outputData);
            PlayerIdClientId.Remove(id);
            AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.ExitGame);
            AmongUsClient.Instance.allClients.Remove(outputData);
        }

        public static void RemoveAllPlayers()
        {
            PlayerIdClientId.Keys.ToList().ForEach(RemovePlayer);
            SwitchTo(0);
        }

        public static void SwitchTo(byte playerId)
        {
            if (!TownOfUsReworked.MCIActive)
                return;

            CustomPlayer.Local.DisableButtons();
            CustomPlayer.Local.DisableArrows();

            if (MeetingHud.Instance)
            {
                switch (CustomPlayer.Local.GetRole())
                {
                    case RoleEnum.Retributionist:
                        var ret = (Retributionist)Role.LocalRole;
                        ret.HideButtons();
                        ret.PlayerNumbers.Clear();
                        ret.Actives.Clear();
                        ret.MoarButtons.Clear();
                        break;

                    case RoleEnum.Guesser:
                        var guesser = (Guesser)Role.LocalRole;
                        guesser.HideButtons();
                        guesser.OtherButtons.Clear();
                        guesser.Exit(MeetingHud.Instance);
                        break;

                    case RoleEnum.Thief:
                        var thief = (Thief)Role.LocalRole;
                        thief.HideButtons();
                        thief.OtherButtons.Clear();
                        thief.Exit(MeetingHud.Instance);
                        break;

                    case RoleEnum.Dictator:
                        var dict = (Dictator)Role.LocalRole;
                        dict.HideButtons();
                        dict.Actives.Clear();
                        dict.MoarButtons.Clear();
                        dict.ToBeEjected.Clear();
                        break;
                }

                switch (CustomPlayer.Local.GetAbility())
                {
                    case AbilityEnum.Assassin:
                        var assassin = (Assassin)Ability.LocalAbility;
                        assassin.HideButtons();
                        assassin.OtherButtons.Clear();
                        assassin.Exit(MeetingHud.Instance);
                        break;

                    case AbilityEnum.Swapper:
                        var swapper = (Swapper)Ability.LocalAbility;
                        swapper.HideButtons();
                        swapper.Actives.Clear();
                        swapper.MoarButtons.Clear();
                        break;

                    case AbilityEnum.Politician:
                        ((Politician)Ability.LocalAbility).DestroyAbstain();
                        break;
                }
            }

            CustomPlayer.Local.NetTransform.RpcSnapTo(CustomPlayer.Local.transform.position);
            CustomPlayer.Local.moveable = false;

            var light = CustomPlayer.Local.lightSource;

            //Setup new player
            var newPlayer = Utils.PlayerById(playerId);
            PlayerControl.LocalPlayer = newPlayer;
            CustomPlayer.Local.lightSource = light;
            CustomPlayer.Local.moveable = true;

            AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
            AmongUsClient.Instance.HostId = newPlayer.OwnerId;

            HudManager.Instance.SetHudActive(true);

            HudManager.Instance.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

            light.transform.SetParent(CustomPlayer.Local.transform);
            light.transform.localPosition = CustomPlayer.Local.Collider.offset;

            Camera.main!.GetComponent<FollowerCamera>().SetTarget(newPlayer);
            CustomPlayer.Local.MyPhysics.ResetMoveState(true);
            KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);

            CustomPlayer.Local.EnableButtons();
            CustomPlayer.Local.EnableArrows();

            if (MeetingHud.Instance)
            {
                if (!CustomPlayer.LocalCustom.IsDead)
                    MeetingHud.Instance.SetForegroundForAlive();

                switch (CustomPlayer.Local.GetRole())
                {
                    case RoleEnum.Retributionist:
                        var ret = (Retributionist)Role.LocalRole;
                        ret.HideButtons();
                        ret.PlayerNumbers.Clear();
                        ret.Actives.Clear();
                        ret.MoarButtons.Clear();
                        Utils.AllVoteAreas.ForEach(x => ret.GenButtons(x, MeetingHud.Instance));
                        break;

                    case RoleEnum.Guesser:
                        var guesser = (Guesser)Role.LocalRole;
                        guesser.HideButtons();
                        guesser.OtherButtons.Clear();
                        Utils.AllVoteAreas.ForEach(x => guesser.GenButton(x, MeetingHud.Instance));
                        break;

                    case RoleEnum.Thief:
                        var thief = (Thief)Role.LocalRole;
                        thief.HideButtons();
                        thief.OtherButtons.Clear();
                        Utils.AllVoteAreas.ForEach(x => thief.GenButton(x, MeetingHud.Instance));
                        break;

                    case RoleEnum.Dictator:
                        var dict = (Dictator)Role.LocalRole;
                        dict.HideButtons();
                        dict.Actives.Clear();
                        dict.MoarButtons.Clear();
                        Utils.AllVoteAreas.ForEach(x => dict.GenButton(x, MeetingHud.Instance));
                        break;
                }

                switch (CustomPlayer.Local.GetAbility())
                {
                    case AbilityEnum.Assassin:
                        var assassin = (Assassin)Ability.LocalAbility;
                        assassin.HideButtons();
                        assassin.OtherButtons.Clear();
                        Utils.AllVoteAreas.ForEach(x => assassin.GenButton(x, MeetingHud.Instance));
                        break;

                    case AbilityEnum.Swapper:
                        var swapper = (Swapper)Ability.LocalAbility;
                        swapper.HideButtons();
                        swapper.Actives.Clear();
                        swapper.MoarButtons.Clear();
                        Utils.AllVoteAreas.ForEach(x => swapper.GenButton(x, MeetingHud.Instance));
                        break;

                    case AbilityEnum.Politician:
                        ((Politician)Ability.LocalAbility).GenButton(MeetingHud.Instance);
                        break;
                }
            }
        }

        public static void SetForegroundForAlive(this MeetingHud __instance)
        {
            __instance.amDead = false;
            __instance.SkipVoteButton.gameObject.SetActive(true);
            __instance.SkipVoteButton.AmDead = false;
            __instance.Glass.gameObject.SetActive(false);
        }
    }
}