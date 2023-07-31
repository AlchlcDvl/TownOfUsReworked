namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        public static readonly List<PlayerControl> RecentlyKilled = new();
        public static readonly Dictionary<PlayerControl, PlayerControl> CachedMorphs = new();
        public static readonly List<DeadPlayer> KilledPlayers = new();
        public static List<DeadBody> AllBodies => UObject.FindObjectsOfType<DeadBody>().ToList();
        public static List<Vent> AllVents => UObject.FindObjectsOfType<Vent>().ToList();
        public static List<GameObject> AllObjects => UObject.FindObjectsOfType<GameObject>().ToList();
        public static List<Console> AllConsoles => UObject.FindObjectsOfType<Console>().ToList();
        public static List<SystemConsole> AllSystemConsoles => UObject.FindObjectsOfType<SystemConsole>().ToList();
        public static List<PlayerVoteArea> AllVoteAreas => Meeting.playerStates.ToList();
        private static bool Shapeshifted;
        public static PlayerControl FirstDead;
        public static PlayerControl CachedFirstDead;
        public static bool RoundOne;
        public static HudManager HUD => HudManager.Instance;
        public static MeetingHud Meeting => MeetingHud.Instance;
        public static ShipStatus Ship => ShipStatus.Instance;
        public static MapBehaviour Map => MapBehaviour.Instance;
        private const string Everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛ" +
            "γΣΦΘξ✧¢";
        private static readonly Dictionary<string, string> KeyWords = new()
        {
            { "%modversion%", TownOfUsReworked.VersionFinal },
            { "%discord%", $"[{TownOfUsReworked.DiscordInvite}]Discord[]" },
            { "%github%", $"[{TownOfUsReworked.GitHubLink}]GitHub[]" }
        };
        public static readonly List<Vector3> SkeldSpawns = new()
        {
            new(-2.2f, 2.2f, 0f), //Cafeteria. botton. top left.
            new(0.7f, 2.2f, 0f), //Caffeteria. button. top right.
            new(-2.2f, -0.2f, 0f), //Caffeteria. button. bottom left.
            new(0.7f, -0.2f, 0f), //Caffeteria. button. bottom right.
            new(4.3f, 0f, 0f), //Cafeteria vent
            new(10f, 3f, 0f), //Weapons top
            new(9.5f, -1f, 0f), //Weapons bottom
            new(6.5f, -3.5f, 0f), //O2
            new(11.5f, -3.5f, 0f), //O2-nav hall
            new(17.0f, -3.5f, 0f), //Navigation top
            new(18.2f, -5.7f, 0f), //Navigation bottom
            new(16f, -2f, 0f), //Navigation vent
            new(11.5f, -6.5f, 0f), //Nav-shields top
            new(9.5f, -8.5f, 0f), //Nav-shields bottom
            new(9.2f, -12.2f, 0f), //Shields top
            new(8.0f, -14.3f, 0f), //Shields bottom
            new(2.5f, -16f, 0f), //Comms left
            new(4.2f, -16.4f, 0f), //Comms middle
            new(5.5f, -16f, 0f), //Comms right
            new(-1.5f, -10.0f, 0f), //Storage top
            new(-1.5f, -15.5f, 0f), //Storage bottom
            new(-4.5f, -12.5f, 0f), //Storrage left
            new(0.3f, -12.5f, 0f), //Storrage right
            new(4.5f, -7.5f, 0f), //Admin top
            new(4.5f, -9.5f, 0f), //Admin bottom
            new(-9.0f, -8.0f, 0f), //Elec top left
            new(-6.0f, -8.0f, 0f), //Elec top right
            new(-8.0f, -11.0f, 0f), //Elec bottom
            new(-12.0f, -13.0f, 0f), //Elec-lower hall
            new(-17f, -10f, 0f), //Lower engine top
            new(-17.0f, -13.0f, 0f), //Lower engine bottom
            new(-21.5f, -3.0f, 0f), //Reactor top
            new(-21.5f, -8.0f, 0f), //Reactor bottom
            new(-13.0f, -3.0f, 0f), //Security top
            new(-12.6f, -5.6f, 0f), //Security bottom
            new(-17.0f, 2.5f, 0f), //Upper engibe top
            new(-17.0f, -1.0f, 0f), //Upper engine bottom
            new(-10.5f, 1.0f, 0f), //Upper-mad hall
            new(-10.5f, -2.0f, 0f), //Medbay top
            new(-6.5f, -4.5f, 0f) //Medbay bottom
        };
        public static readonly List<Vector3> MiraSpawns = new()
        {
            new(-4.5f, 3.5f, 0f), //Launchpad top
            new(-4.5f, -1.4f, 0f), //Launchpad bottom
            new(8.5f, -1f, 0f), //Launchpad- med hall
            new(14f, -1.5f, 0f), //Medbay
            new(16.5f, 3f, 0f), //Comms
            new(10f, 5f, 0f), //Lockers
            new(6f, 1.5f, 0f), //Locker room
            new(2.5f, 13.6f, 0f), //Reactor
            new(6f, 12f, 0f), //Reactor middle
            new(9.5f, 13f, 0f), //Lab
            new(15f, 9f, 0f), //Bottom left cross
            new(17.9f, 11.5f, 0f), //Middle cross
            new(14f, 17.3f, 0f), //Office
            new(19.5f, 21f, 0f), //Admin
            new(14f, 24f, 0f), //Greenhouse left
            new(22f, 24f, 0f), //Greenhouse right
            new(21f, 8.5f, 0f), //Bottom right cross
            new(28f, 3f, 0f), //Caf right
            new(22f, 3f, 0f), //Caf left
            new(19f, 4f, 0f), //Storage
            new(22f, -2f, 0f) //Balcony
        };
        public static readonly List<Vector3> PolusSpawns = new()
        {
            new(16.6f, -1f, 0f), //Dropship top
            new(16.6f, -5f, 0f), //Dropship bottom
            new(20f, -9f, 0f), //Above storrage
            new(22f, -7f, 0f), //Right fuel
            new(25.5f, -6.9f, 0f), //Drill
            new(29f, -9.5f, 0f), //Lab lockers
            new(29.5f, -8f, 0f), //Lab weather notes
            new(35f, -7.6f, 0f), //Lab table
            new(40.4f, -8f, 0f), //Lab scan
            new(33f, -10f, 0f), //Lab toilet
            new(39f, -15f, 0f), //Specimen hall top
            new(36.5f, -19.5f, 0f), //Specimen top
            new(36.5f, -21f, 0f), //Specimen bottom
            new(28f, -21f, 0f), //Specimen hall bottom
            new(24f, -20.5f, 0f), //Admin tv
            new(22f, -25f, 0f), //Admin books
            new(16.6f, -17.5f, 0f), //Office coffe
            new(22.5f, -16.5f, 0f), //Office projector
            new(24f, -17f, 0f), //Office figure
            new(27f, -16.5f, 0f), //Office lifelines
            new(32.7f, -15.7f, 0f), //Lavapool
            new(31.5f, -12f, 0f), //Snowmad below lab
            new(10f, -14f, 0f), //Below storrage
            new(21.5f, -12.5f, 0f), //Storrage vent
            new(19f, -11f, 0f), //Storrage toolrack
            new(12f, -7f, 0f), //Left fuel
            new(5f, -7.5f, 0f), //Above elec
            new(10f, -12f, 0f), //Elec fence
            new(9f, -9f, 0f), //Elec lockers
            new(5f, -9f, 0f), //Elec window
            new(4f, -11.2f, 0f), //Elec tapes
            new(5.5f, -16f, 0f), //Elec-O2 hall
            new(1f, -17.5f, 0f), //O2 tree hayball
            new(3f, -21f, 0f), //O2 middle
            new(2f, -19f, 0f), //O2 gas
            new(1f, -24f, 0f), //O2 water
            new(7f, -24f, 0f), //Under O2
            new(9f, -20f, 0f), //Right outside of O2
            new(7f, -15.8f, 0f), //Snowman under elec
            new(11f, -17f, 0f), //Comms table
            new(12.7f, -15.5f, 0f), //Comms antenna pult
            new(13f, -24.5f, 0f), //Weapons window
            new(15f, -17f, 0f), //Between coms-office
            new(17.5f, -25.7f, 0f) //Snowman under office
        };
        public static readonly List<Vector3> dlekSSpawns = new()
        {
            new(2.2f, 2.2f, 0f), //Cafeteria. botton. top left.
            new(-0.7f, 2.2f, 0f), //Caffeteria. button. top right.
            new(2.2f, -0.2f, 0f), //Caffeteria. button. bottom left.
            new(-0.7f, -0.2f, 0f), //Caffeteria. button. bottom right.
            new(-10.0f, 3.0f, 0f), //Weapons top
            new(-9.0f, 1.0f, 0f), //Weapons bottom
            new(-6.5f, -3.5f, 0f), //O2
            new(-11.5f, -3.5f, 0f), //O2-nav hall
            new(-17.0f, -3.5f, 0f), //Navigation top
            new(-18.2f, -5.7f, 0f), //Navigation bottom
            new(-11.5f, -6.5f, 0f), //Nav-shields top
            new(-9.5f, -8.5f, 0f), //Nav-shields bottom
            new(-9.2f, -12.2f, 0f), //Shields top
            new(-8.0f, -14.3f, 0f), //Shields bottom
            new(-2.5f, -16f, 0f), //Comms left
            new(-4.2f, -16.4f, 0f), //Comms middle
            new(-5.5f, -16f, 0f), //Comms right
            new(1.5f, -10.0f, 0f), //Storage top
            new(1.5f, -15.5f, 0f), //Storage bottom
            new(4.5f, -12.5f, 0f), //Storrage left
            new(-0.3f, -12.5f, 0f), //Storrage right
            new(-4.5f, -7.5f, 0f), //Admin top
            new(-4.5f, -9.5f, 0f), //Admin bottom
            new(9.0f, -8.0f, 0f), //Elec top left
            new(6.0f, -8.0f, 0f), //Elec top right
            new(8.0f, -11.0f, 0f), //Elec bottom
            new(12.0f, -13.0f, 0f), //Elec-lower hall
            new(17f, -10f, 0f), //Lower engine top
            new(17.0f, -13.0f, 0f), //Lower engine bottom
            new(21.5f, -3.0f, 0f), //Reactor top
            new(21.5f, -8.0f, 0f), //Reactor bottom
            new(13.0f, -3.0f, 0f), //Security top
            new(12.6f, -5.6f, 0f), //Security bottom
            new(17.0f, 2.5f, 0f), //Upper engibe top
            new(17.0f, -1.0f, 0f), //Upper engine bottom
            new(10.5f, 1.0f, 0f), //Upper-mad hall
            new(10.5f, -2.0f, 0f), //Medbay top
            new(6.5f, -4.5f, 0f) //Medbay bottom
        };

        public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TextMeshPro ColorBlindText(this PlayerControl p) => p.cosmetics.colorBlindText;

        public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static SpriteRenderer MyRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

        public static bool IsImpostor(this GameData.PlayerInfo playerinfo) => playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static bool IsImpostor(this PlayerVoteArea playerinfo) => PlayerByVoteArea(playerinfo)?.Data?.IsImpostor() == true;

        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl) => playerControl.Data.DefaultOutfit;

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType customOutfitType, GameData.PlayerOutfit outfit)
        {
            playerControl.Data.SetOutfit((PlayerOutfitType)customOutfitType, outfit);
            playerControl.SetOutfit(customOutfitType);
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            if (playerControl == null)
                return;

            var outfitType = (PlayerOutfitType)CustomOutfitType;

            if (!playerControl.Data.Outfits.TryGetValue(outfitType, out var newOutfit))
                return;

            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);

            if (!playerControl.Data.IsDead)
                playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);

            if (!playerControl.Data.IsDead)
                playerControl.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);

            playerControl.cosmetics.colorBlindText.color = UColor.white;
        }

        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) => (CustomPlayerOutfitType)playerControl.CurrentOutfitType;

        public static void Morph(PlayerControl player, PlayerControl morphTarget)
        {
            if (DoUndo.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, morphTarget.Data.DefaultOutfit);
        }

        public static void DefaultOutfit(PlayerControl player) => Coroutines.Start(DefaultOutfitCoro(player));

        public static IEnumerator DefaultOutfitCoro(PlayerControl player)
        {
            if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Invis)
            {
                HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
                {
                    var rend = player.MyRend();
                    rend.color = new(255, 255, 255, p);
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, p);
                })));

                yield return new WaitForSeconds(1f);
            }

            player.SetOutfit(CustomPlayerOutfitType.Default);
            CachedMorphs.Remove(player);
            yield return null;
        }

        public static void Camouflage() => CustomPlayer.AllPlayers.ForEach(CamoSingle);

        public static void CamoSingle(PlayerControl player) => Coroutines.Start(CamoSingleCoro(player));

        public static IEnumerator CamoSingleCoro(PlayerControl player)
        {
            if (player.GetCustomOutfitType() is not CustomPlayerOutfitType.Camouflage and not CustomPlayerOutfitType.Invis and not CustomPlayerOutfitType.PlayerNameOnly &&
                !player.Data.IsDead && !CustomPlayer.LocalCustom.IsDead && player != CustomPlayer.Local)
            {
                player.SetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(player));
                PlayerMaterial.SetColors(UColor.grey, player.MyRend());

                HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
                {
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, 1 - p);
                })));
            }

            yield return null;
        }

        public static void Conceal() => CustomPlayer.AllPlayers.ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));

        public static void Invis(PlayerControl player, bool condition = false) => Coroutines.Start(InvisCoro(player, condition));

        public static IEnumerator InvisCoro(PlayerControl player, bool condition)
        {
            var color = Color.clear;
            color.a = condition || CustomPlayer.LocalCustom.IsDead || player == CustomPlayer.Local || CustomPlayer.Local.Is(AbilityEnum.Torch) ? 0.1f : 0f;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && !player.Data.IsDead)
            {
                player.SetOutfit(CustomPlayerOutfitType.Invis, InvisOutfit(player));

                HUD.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
                {
                    var rend = player.MyRend();
                    var a = Mathf.Clamp(1 - p, color.a, 1);
                    var r = rend.color.r * (1 - p);
                    var g = rend.color.g * (1 - p);
                    var b = rend.color.b * (1 - p);
                    rend.color = new(r, g, b, a);
                    var text = player.NameText();
                    text.color = new(text.color.a, text.color.a, text.color.a, 1 - p);
                    var cbtext = player.ColorBlindText();
                    cbtext.color = new(cbtext.color.a, cbtext.color.a, cbtext.color.a, 1 - p);
                })));
            }

            yield return null;
        }

        public static GameData.PlayerOutfit InvisOutfit(PlayerControl player) => new()
        {
            ColorId = player.CurrentOutfit.ColorId,
            HatId = "",
            SkinId = "",
            VisorId = "",
            PlayerName = " ",
            PetId = ""
        };

        public static GameData.PlayerOutfit BlankOutfit(PlayerControl player) => new()
        {
            ColorId = player.GetDefaultOutfit().ColorId,
            HatId = "",
            SkinId = "",
            VisorId = "",
            PlayerName = " ",
            PetId = ""
        };

        public static void Shapeshift()
        {
            if (!Shapeshifted)
            {
                Shapeshifted = true;
                var allPlayers = CustomPlayer.AllPlayers;
                var shuffledPlayers = CustomPlayer.AllPlayers;
                shuffledPlayers.Shuffle();

                for (var i = 0; i < allPlayers.Count; i++)
                {
                    var morphed = allPlayers[i];
                    var morphTarget = shuffledPlayers[i];
                    Morph(morphed, morphTarget);
                    CachedMorphs.Add(morphed, morphTarget);
                }
            }
            else
            {
                CustomPlayer.AllPlayers.ForEach(x =>
                {
                    if (CachedMorphs.ContainsKey(x))
                        Morph(x, CachedMorphs[x]);
                });
            }
        }

        public static void DefaultOutfitAll() => CustomPlayer.AllPlayers.ForEach(DefaultOutfit);

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
        {
            if (!self.Contains(item))
                self.Add(item);
        }

        public static Color GetShadowColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
        {
            if ((DoUndo.IsCamoed && camoCondition) || otherCondition)
                return new Color32(125, 125, 125, 255);
            else
                return ColorUtils.GetColor(player.GetDefaultOutfit().ColorId, true);
        }

        public static Color GetPlayerColor(this PlayerControl player, bool camoCondition = true, bool otherCondition = false)
        {
            if ((DoUndo.IsCamoed && camoCondition) || otherCondition)
                return UColor.grey;
            else
                return ColorUtils.GetColor(player.GetDefaultOutfit().ColorId, false);
        }

        public static PlayerControl PlayerById(byte id) => CustomPlayer.AllPlayers.Find(x => x.PlayerId == id);

        public static PlayerVoteArea VoteAreaById(byte id) => AllVoteAreas.Find(x => x.TargetPlayerId == id);

        public static DeadBody BodyById(byte id) => AllBodies.Find(x => x.ParentId == id);

        public static DeadBody BodyByPlayer(PlayerControl player) => BodyById(player.PlayerId);

        public static PlayerControl PlayerByBody(DeadBody body) => PlayerById(body.ParentId);

        public static PlayerVoteArea VoteAreaByPlayer(PlayerControl player) => VoteAreaById(player.PlayerId);

        public static Vent VentById(int id) => AllVents.Find(x => x.Id == id);

        public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state.TargetPlayerId);

        public static Vector2 GetSize() => Vector2.Scale(AllVents[0].GetComponent<BoxCollider2D>().size, AllVents[0].transform.localScale) * 0.75f;

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            return Vector2.Distance(refplayer.GetTruePosition(), player.GetTruePosition());
        }

        public static double GetDistBetweenPlayers(PlayerControl player, Vent refVent)
        {
            if (player == null || refVent == null)
                return double.MaxValue;

            return Vector2.Distance(refVent.transform.position, player.GetTruePosition());
        }

        public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refBody)
        {
            if (player == null || refBody == null)
                return double.MaxValue;

            return Vector2.Distance(refBody.TruePosition, player.GetTruePosition());
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            MurderPlayer(killer, target, reason, lunge);
            CallRpc(CustomRPC.Action, ActionsRPC.BypassKill, killer, target, reason, lunge);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason, bool lunge)
        {
            if (killer == null || target == null)
                return;

            var data = target.Data;

            if (data == null)
                return;

            lunge &= !killer.Is(AbilityEnum.Ninja) && killer != target;

            if (data.IsDead)
                return;

            if (killer == CustomPlayer.Local)
                Play("Kill");

            if (FirstDead == null)
                FirstDead = target;

            target.gameObject.layer = LayerMask.NameToLayer("Ghost");
            target.Visible = false;

            if (CustomPlayer.Local.Is(RoleEnum.Coroner) && !CustomPlayer.LocalCustom.IsDead)
                Flash(Colors.Coroner);

            if (CustomPlayer.LocalCustom.IsDead)
                Flash(Colors.Stalemate);

            var targetRole = Role.GetRole(target);

            if (target.Is(ModifierEnum.VIP))
            {
                Flash(targetRole.Color);

                if (!Role.LocalRole.AllArrows.ContainsKey(target.PlayerId))
                    Role.LocalRole.AllArrows.Add(target.PlayerId, new(CustomPlayer.Local, Colors.VIP));
                else
                    Role.LocalRole.AllArrows[target.PlayerId].Update(Colors.VIP);
            }

            var killerRole = Role.GetRole(killer);

            if (target.AmOwner)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (Map)
                    Map.Close();

                HUD.KillOverlay.ShowKillAnimation(killer.Data, data);
                HUD.ShadowQuad.gameObject.SetActive(false);
                HUD.Chat.SetVisible(true);
                target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                target.RpcSetScanner(false);
            }

            killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(lunge ? killer : target, target));

            if (killer != target)
            {
                targetRole.KilledBy = " By " + killerRole.PlayerName;
                targetRole.DeathReason = reason;
            }
            else
                targetRole.DeathReason = DeathReasonEnum.Suicide;

            if (target.Is(RoleEnum.Framer))
                ((Framer)targetRole).Framed.Clear();

            RecentlyKilled.Add(target);
            KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
            ReassignPostmortals(target);

            if (target == Role.DriveHolder)
                RoleGen.AssignChaosDrive();

            if (target.Is(ObjectifierEnum.Lovers) && AmongUsClient.Instance.AmHost)
            {
                var lover = target.GetOtherLover();

                if (!lover.Is(RoleEnum.Pestilence) && CustomGameOptions.BothLoversDie && !lover.Data.IsDead)
                    RpcMurderPlayer(lover, lover);
            }

            if (target.Is(ModifierEnum.Diseased))
                killerRole.Diseased = true;
            else if (target.Is(ModifierEnum.Bait))
                BaitReport(killer, target);

            if (killer.Is(AbilityEnum.Politician))
                Ability.GetAbility<Politician>(killer).VoteBank++;

            if (target.Is(RoleEnum.Troll) && AmongUsClient.Instance.AmHost)
            {
                var troll = Role.GetRole<Troll>(target);
                troll.Killed = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.TrollWin, troll);

                if (!CustomGameOptions.AvoidNeutralKingmakers)
                    RpcMurderPlayer(target, killer, DeathReasonEnum.Trolled, false);
            }

            if (Meeting)
                MarkMeetingDead(target, killer);
        }

        public static void MarkMeetingDead(PlayerControl target, PlayerControl killer, bool doesKill = true, bool noReason = false)
        {
            Play("Kill");

            if (target == CustomPlayer.Local)
            {
                HUD.KillOverlay.ShowKillAnimation(killer.Data, target.Data);
                HUD.ShadowQuad.gameObject.SetActive(false);
                target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                target.RpcSetScanner(false);
                Meeting.SetForegroundForDead();

                if (target.Is(AbilityEnum.Swapper))
                {
                    var swapper = Ability.GetAbility<Swapper>(target);
                    swapper.Swap1 = null;
                    swapper.Swap2 = null;
                    swapper.SwapMenu.HideButtons();
                    CallRpc(CustomRPC.Action, ActionsRPC.SetSwaps, swapper, 255, 255);
                }

                if (target.Is(RoleEnum.Dictator))
                {
                    var dict = Role.GetRole<Dictator>(target);
                    dict.DictMenu.HideButtons();
                    dict.ToBeEjected.Clear();
                    dict.ToDie = false;
                    CallRpc(CustomRPC.Action, ActionsRPC.SetExiles, dict, false, dict.ToBeEjected.ToArray());
                }

                if (target.Is(RoleEnum.Retributionist))
                    Role.GetRole<Retributionist>(target).RetMenu.HideButtons();

                if (target.Is(LayerEnum.Assassin, PlayerLayerEnum.Ability))
                {
                    var assassin = Ability.GetAbility<Assassin>(target);
                    assassin.Exit(Meeting);
                    assassin.AssassinMenu.HideButtons();
                }

                if (target.Is(RoleEnum.Guesser))
                {
                    var guesser = Role.GetRole<Guesser>(CustomPlayer.Local);
                    guesser.Exit(Meeting);
                    guesser.GuessMenu.HideButtons();
                }

                if (target.Is(RoleEnum.Thief))
                {
                    var thief = Role.GetRole<Thief>(CustomPlayer.Local);
                    thief.Exit(Meeting);
                    thief.GuessMenu.HideButtons();
                }
            }

            target.Die(DeathReason.Kill, false);
            KilledPlayers.Add(new(killer.PlayerId, target.PlayerId));
            var voteArea = VoteAreaByPlayer(target);

            if (voteArea.DidVote)
                voteArea.UnsetVote();

            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = UColor.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
            {
                if (target == role.BlackmailedPlayer && role.PrevOverlay != null)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            foreach (var role in Role.GetRoles<Silencer>(RoleEnum.Silencer))
            {
                if (target == role.SilencedPlayer && role.PrevOverlay != null)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
            {
                if (target == role.BlackmailedPlayer && role.PrevOverlay != null && role.IsBM)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            foreach (var role in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
            {
                if (target == role.SilencedPlayer && role.PrevOverlay != null && role.IsSil)
                {
                    voteArea.Overlay.sprite = role.PrevOverlay;
                    voteArea.Overlay.color = role.PrevColor;
                }
            }

            if (CustomPlayer.Local.Is(LayerEnum.Assassin, PlayerLayerEnum.Ability) && !CustomPlayer.LocalCustom.IsDead)
            {
                var assassin = Ability.GetAbility<Assassin>(CustomPlayer.Local);
                assassin.Exit(Meeting);
                assassin.AssassinMenu.HideSingle(target.PlayerId);
            }

            if (CustomPlayer.Local.Is(RoleEnum.Guesser) && !CustomPlayer.LocalCustom.IsDead)
            {
                var guesser = Role.GetRole<Guesser>(CustomPlayer.Local);
                guesser.Exit(Meeting);
                guesser.GuessMenu.HideSingle(target.PlayerId);
            }

            if (CustomPlayer.Local.Is(RoleEnum.Thief) && !CustomPlayer.LocalCustom.IsDead)
            {
                var thief = Role.GetRole<Thief>(CustomPlayer.Local);
                thief.Exit(Meeting);
                thief.GuessMenu.HideSingle(target.PlayerId);
            }

            if (CustomPlayer.Local.Is(AbilityEnum.Swapper) && !CustomPlayer.LocalCustom.IsDead)
            {
                var swapper = Ability.GetAbility<Swapper>(CustomPlayer.Local);

                if (swapper.SwapMenu.Actives.Any(x => x.Key == target.PlayerId && x.Value))
                {
                    if (swapper.Swap1 == voteArea)
                        swapper.Swap1 = null;
                    else if (swapper.Swap2 == voteArea)
                        swapper.Swap2 = null;

                    swapper.SwapMenu.Actives[target.PlayerId] = false;
                    CallRpc(CustomRPC.Action, ActionsRPC.SetSwaps, swapper, 255, 255);
                }

                swapper.SwapMenu.HideSingle(target.PlayerId);
            }

            if (CustomPlayer.Local.Is(RoleEnum.Dictator) && !CustomPlayer.LocalCustom.IsDead)
            {
                var dictator = Role.GetRole<Dictator>(CustomPlayer.Local);

                if (dictator.DictMenu.Actives.Any(x => x.Key == target.PlayerId && x.Value))
                {
                    dictator.ToBeEjected.Clear();

                    for (byte i = 0; i < dictator.DictMenu.Actives.Count; i++)
                        dictator.DictMenu.Actives[i] = false;

                    dictator.DictMenu.Actives[target.PlayerId] = false;
                    CallRpc(CustomRPC.Action, ActionsRPC.SetExiles, dictator, false, dictator.ToBeEjected.ToArray());
                }

                dictator.DictMenu.HideSingle(target.PlayerId);
            }

            if (CustomPlayer.Local.Is(RoleEnum.Retributionist) && !CustomPlayer.LocalCustom.IsDead)
            {
                var ret = Role.GetRole<Retributionist>(CustomPlayer.Local);
                ret.RetMenu.HideButtons();
                ret.RetMenu.GenButtons(Meeting);
            }

            foreach (var area in AllVoteAreas)
            {
                if (area.VotedFor != target.PlayerId)
                    continue;

                area.UnsetVote();

                if (target == CustomPlayer.Local)
                    Meeting.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var mayor in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
                {
                    if (mayor.Player == target)
                        mayor.ExtraVotes.Clear();
                    else
                    {
                        var votesRegained = mayor.ExtraVotes.RemoveAll(x => x == target.PlayerId);

                        if (mayor.Local)
                            mayor.VoteBank += votesRegained;

                        CallRpc(CustomRPC.Misc, MiscRPC.AddVoteBank, mayor, votesRegained);
                    }
                }

                AssignPostmortals(target);
                SetPostmortals.AssassinatedPlayers.Add(target);
                Meeting.CheckForEndVoting();
            }

            if (!noReason)
            {
                var role2 = Role.GetRole(target);

                if ((killer != target && doesKill) || !doesKill)
                {
                    role2.DeathReason = DeathReasonEnum.Guessed;
                    role2.KilledBy = " By " + killer.name;
                }
                else
                    role2.DeathReason = DeathReasonEnum.Misfire;
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || killer == target)
                yield break;

            var extraDelay = URandom.RandomRangeInt(0, (int)((100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay)) + 1));

            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + (extraDelay / 100f));

            var body = BodyById(target.PlayerId);

            if (body != null)
            {
                if (AmongUsClient.Instance.AmHost)
                    killer.ReportDeadBody(target.Data);
                else
                    CallRpc(CustomRPC.Action, ActionsRPC.BaitReport, killer, target);
            }
        }

        public static void EndGame()
        {
            Ash.DestroyAll();
            Objects.Range.DestroyAll();
            OtherButtonsPatch.Zooming = true;
            OtherButtonsPatch.Zoom();
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByVote, false);
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false, bool bypass = false)
        {
            var fullReset = false;
            var gaReset = false;
            var survReset = false;
            var abilityUsed = false;
            bypass |= player.Is(AbilityEnum.Ruthless);
            Spread(player, target);

            if (target == CachedFirstDead && (toConvert || toKill))
                fullReset = true;
            else if ((target.IsOnAlert() || ((target.IsAmbushed() || target.IsGFAmbushed()) && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) &&
                CustomGameOptions.AmbushMates))) || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) && player.Is(SubFaction.Undead)) ||
                (target.Is(RoleEnum.SerialKiller) && (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || player.Is(RoleEnum.Glitch)) && !toKill)) && !bypass)
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert))
                    {
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                        CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if ((player.IsShielded() || player.IsRetShielded()) && !target.Is(AbilityEnum.Ruthless))
                {
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Role.BreakShield(player, CustomGameOptions.ShieldBreaks);
                    CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, player);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player, target.IsAmbushed() ? DeathReasonEnum.Ambushed : DeathReasonEnum.Killed);

                if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert))
                {
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                    CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
                }
            }
            else if ((target.IsCrusaded() || target.IsRebCrusaded()) && (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && CustomGameOptions.CrusadeMates)) && !bypass)
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert))
                    {
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                        CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if ((player.IsShielded() || player.IsRetShielded()) && !target.Is(AbilityEnum.Ruthless))
                {
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Role.BreakShield(player, CustomGameOptions.ShieldBreaks);
                    CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, player);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                {
                    var crus = target.GetCrusader();
                    var reb = target.GetRebCrus();

                    if (crus?.HoldsDrive == true || reb?.HoldsDrive == true)
                        Crusader.RadialCrusade(target);
                    else
                        RpcMurderPlayer(target, player, DeathReasonEnum.Crusaded);
                }

                if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert))
                {
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                    CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
                }
            }
            else if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert) && !bypass)
            {
                fullReset = CustomGameOptions.ShieldBreaks;
                Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
            }
            else if (target.IsVesting() && (toKill || toConvert) && !bypass)
                survReset = true;
            else if (target.IsProtected() && (toKill || toConvert) && !bypass)
                gaReset = true;
            else if ((target.IsProtectedMonarch() || player.IsOtherRival(target)) && (toKill || toConvert))
                fullReset = true;
            else
            {
                if (toKill)
                {
                    if (target.Is(ObjectifierEnum.Fanatic) && (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)) && target.IsUnturnedFanatic() && !bypass)
                    {
                        var fact = player.GetFaction();
                        var fanatic = Objectifier.GetObjectifier<Fanatic>(target);
                        fanatic.TurnFanatic(fact);
                        CallRpc(CustomRPC.Change, TurnRPC.TurnFanatic, fanatic, fact);
                    }
                    else
                        RpcMurderPlayer(player, target);
                }
                else if (toConvert && !target.Is(SubFaction.None))
                    RpcMurderPlayer(player, target, DeathReasonEnum.Failed);

                abilityUsed = true;
                fullReset = true;
            }

            if ((target.IsOnAlert() || ((target.IsAmbushed() || target.IsGFAmbushed()) && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) &&
                CustomGameOptions.AmbushMates))) || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) && player.Is(SubFaction.Undead)) ||
                (target.Is(RoleEnum.SerialKiller) && (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || player.Is(RoleEnum.Glitch)) && !toKill) || ((target.IsCrusaded() ||
                target.IsRebCrusaded()) && (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && CustomGameOptions.CrusadeMates)))) && bypass &&
                !target.Is(RoleEnum.Pestilence))
            {
                RpcMurderPlayer(target, player);
            }

            return new() { fullReset, gaReset, survReset, abilityUsed };
        }

        public static List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead = false) => CustomPlayer.AllPlayers.Where(x =>
            Vector2.Distance(truePosition, x.GetTruePosition()) <= radius && (!x.Data.IsDead || (x.Data.IsDead && includeDead))).ToList();

        public static bool IsTooFar(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null)
                return true;

            return GetDistBetweenPlayers(player, target) > CustomGameOptions.InteractionDistance;
        }

        public static bool IsTooFar(PlayerControl player, DeadBody target)
        {
            if (player == null || target == null)
                return true;

            return GetDistBetweenPlayers(player, target) > CustomGameOptions.InteractionDistance;
        }

        public static bool IsTooFar(PlayerControl player, Vent target)
        {
            if (player == null || target == null)
                return true;

            return GetDistBetweenPlayers(player, target) > CustomGameOptions.InteractionDistance / 2;
        }

        public static bool NoButton(PlayerControl target, RoleEnum role) => CustomPlayer.AllPlayers.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(role) || !IsRoaming || Meeting || target != CustomPlayer.Local;

        public static bool NoButton(PlayerControl target, ModifierEnum mod) => CustomPlayer.AllPlayers.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(mod) || !IsRoaming || Meeting || target != CustomPlayer.Local;

        public static bool NoButton(PlayerControl target, Faction faction) => CustomPlayer.AllPlayers.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(faction) || !IsRoaming || Meeting || target != CustomPlayer.Local;

        public static bool NoButton(PlayerControl target, ObjectifierEnum obj) => CustomPlayer.AllPlayers.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(obj) || !IsRoaming || Meeting || target != CustomPlayer.Local;

        public static bool NoButton(PlayerControl target, AbilityEnum ability) => CustomPlayer.AllPlayers.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(ability) || !IsRoaming || Meeting || target != CustomPlayer.Local;

        public static void Spread(PlayerControl interacter, PlayerControl target)
        {
            Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer).ForEach(pb => pb.RpcSpreadInfection(interacter, target));
            Role.GetRoles<Arsonist>(RoleEnum.Arsonist).ForEach(arso => arso.RpcSpreadDouse(target, interacter));
            Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac).ForEach(cryo => cryo.RpcSpreadDouse(target, interacter));
        }

        public static bool Check(int probability)
        {
            if (probability == 0)
                return false;

            if (probability == 100)
                return true;

            var num = URandom.RandomRangeInt(1, 100);
            return num <= probability;
        }

        public static void StopDragging(byte id)
        {
            Role.GetRoles<Janitor>(RoleEnum.Janitor).Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id).ToList().ForEach(x => x.Drop());
            Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id).ToList().ForEach(x => x.Drop());
        }

        public static void LogSomething(object message, ModLogType type = ModLogType.Message)
        {
            if (type == ModLogType.Message)
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);
            else if (type == ModLogType.Debug)
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogDebug(message);
            else if (type == ModLogType.Fatal)
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogFatal(message);
            else if (type == ModLogType.Info)
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogInfo(message);
            else if (type == ModLogType.Warning)
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogWarning(message);
            else if (type == ModLogType.Error)
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogError(message);
        }

        public static string CreateText(string itemName, string folder = "")
        {
            try
            {
                var resourceName = "";

                if (folder != "")
                    resourceName = $"{TownOfUsReworked.Resources}{folder}.{itemName}";
                else
                    resourceName = TownOfUsReworked.Resources + itemName;

                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(resourceName);
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                KeyWords.ToList().ForEach(x => text = text.Replace(x.Key, x.Value));
                return text;
            }
            catch
            {
                LogSomething($"Error Loading {itemName}");
                return "";
            }
        }

        public static bool IsInRange(float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
        {
            if (minInclusive && maxInclusive)
                return num >= min && num <= max;
            else if (minInclusive)
                return num >= min && num < max;
            else if (maxInclusive)
                return num > min && num <= max;
            else
                return num > min && num < max;
        }

        public static string GetRandomisedName()
        {
            var length = URandom.RandomRangeInt(1, 11);
            var position = 0;
            var name = "";

            while (position < length)
            {
                var random = URandom.RandomRangeInt(0, Everything.Length);
                name += Everything[random];
                position++;
            }

            return name;
        }

        public static IEnumerator FadeBody(DeadBody body)
        {
            if (body == null)
                yield break;

            foreach (var renderer in body.bodyRenderers)
            {
                var backColor = renderer.material.GetColor(Shader.PropertyToID("_BackColor"));
                var bodyColor = renderer.material.GetColor(Shader.PropertyToID("_BodyColor"));
                var newColor = new Color(1f, 1f, 1f, 0f);

                for (var i = 0; i < 60; i++)
                {
                    renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                    renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                    yield return null;
                }
            }

            body.gameObject.Destroy();
            Role.Cleaned.Add(PlayerById(body.ParentId));
        }

        public static void RpcSpawnVent(Role role)
        {
            if (role.RoleType is not RoleEnum.Godfather and not RoleEnum.Miner)
                return;

            var position = role.Player.transform.position;
            CallRpc(CustomRPC.Action, ActionsRPC.Mine, role, position);
            AddVent(role, position);
        }

        public static void AddVent(Role role, Vector3 position)
        {
            if (role.RoleType == RoleEnum.Miner)
            {
                var miner = (Miner)role;
                var vent = SpawnVent(miner.Vents, position);
                miner.Vents.Add(vent);
            }
            else if (role.RoleType == RoleEnum.PromotedGodfather)
            {
                var gf = (PromotedGodfather)role;
                var vent = SpawnVent(gf.Vents, position);
                gf.Vents.Add(vent);
            }
        }

        public static Vent SpawnVent(List<Vent> vents, Vector3 position)
        {
            var ventPrefab = UObject.FindObjectOfType<Vent>();
            var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);

            vent.Id = GetAvailableId();
            vent.transform.position = position;

            if (vents.Count > 0)
            {
                var leftVent = vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else
                vent.Left = null;

            vent.Right = null;
            vent.Center = null;

            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();

            if (IsSubmerged)
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent("ElevatorMover"); //Just in case elevator vent is not blocked

                if (vent.gameObject.transform.position.y > -7)
                    vent.gameObject.transform.position = new(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.03f);
                else
                {
                    vent.gameObject.transform.position = new(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.0009f);
                    vent.gameObject.transform.localPosition = new(vent.gameObject.transform.localPosition.x, vent.gameObject.transform.localPosition.y, -0.003f);
                }
            }

            return vent;
        }

        public static int GetAvailableId()
        {
            var id = 0;

            while (true)
            {
                if (ShipStatus.Instance.AllVents.All(v => v.Id != id))
                    return id;

                id++;
            }
        }

        public static void Flash(Color32 color, string message, float duration = 0.5f, float size = 100f) => Flash(color, duration, message, size);

        public static void Flash(Color32 color, float duration = 0.5f, string message = "", float size = 100f) => Coroutines.Start(FlashCoro(color, duration, message, size));

        public static IEnumerator FlashCoro(Color color, float duration, string message, float size)
        {
            if (IntroCutscene.Instance)
                yield break;

            color.a = 0.3f;

            if (HudManager.InstanceExists && HUD.FullScreen)
            {
                var fullscreen = HUD.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            HUD.Notifier.AddItem($"<color=#FFFFFFFF><size={size}%>{message}</size></color>");

            yield return new WaitForSeconds(duration);

            if (HudManager.InstanceExists && HUD.FullScreen)
                SetFullScreenHUD();
        }

        public static void SetFullScreenHUD()
        {
            if (!(HudManager.InstanceExists && HudManager.Instance.FullScreen))
                return;

            var fullscreen = HUD.FullScreen;
            fullscreen.color = new(0.6f, 0.6f, 0.6f, 0f);
            var fs = false;

            if (ShipStatus.Instance)
            {
                switch (TownOfUsReworked.NormalOptions.MapId)
                {
                    case 0:
                    case 1:
                    case 3:
                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        fs = reactor1.IsActive || oxygen1.IsActive;
                        break;

                    case 2:
                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        fs = seismic.IsActive;
                        break;

                    case 4:
                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();
                        fs = reactor.IsActive;
                        break;

                    case 5:
                        fs = CustomPlayer.Local.myTasks.Any(x => x.TaskType == RetrieveOxygenMask);
                        break;

                    case 6:
                        var reactor3 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                        var oxygen3 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        var seismic2 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        fs = reactor3.IsActive || seismic2.IsActive || oxygen3.IsActive;
                        break;
                }
            }

            fullscreen.enabled = true;
            fullscreen.gameObject.active = true;

            if (fs)
                fullscreen.color = new(1f, 0f, 0f, 0.37254903f);
        }

        public static void Warp()
        {
            var coordinates = GenerateWarpCoordinates();
            var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
            writer.Write((byte)CustomRPC.Action);
            writer.Write((byte)ActionsRPC.WarpAll);
            writer.Write((byte)coordinates.Count);

            foreach (var (key, value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
            WarpPlayersToCoordinates(coordinates);
        }

        public static void WarpPlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(CustomPlayer.Local.PlayerId))
            {
                Flash(Colors.Warper);

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (Map)
                    Map.Close();

                if (CustomPlayer.Local.inVent)
                {
                    CustomPlayer.Local.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    CustomPlayer.Local.MyPhysics.ExitAllVents();
                }
            }

            foreach (var (key, value) in coordinates)
            {
                var player = PlayerById(key);
                player.transform.position = value;

                if (TownOfUsReworked.IsTest)
                    LogSomething($"Warping {player.Data.PlayerName} to ({value.x}, {value.y})");
            }

            if (AmongUsClient.Instance.AmHost)
            {
                Role.GetRoles<Janitor>(RoleEnum.Janitor).Where(x => x.CurrentlyDragging != null).ToList().ForEach(x => x.Drop());
                Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Where(x => x.CurrentlyDragging != null).ToList().ForEach(x => x.Drop());
            }
        }

        public static Dictionary<byte, Vector2> GenerateWarpCoordinates()
        {
            var targets = CustomPlayer.AllPlayers.Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();
            var coordinates = new Dictionary<byte, Vector2>(targets.Count);
            var allLocations = new List<Vector3>();

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (!player.onLadder && !player.inMovingPlat)
                    allLocations.Add(player.transform.position);
            }

            AllVents.ForEach(x => allLocations.Add(GetVentPosition(x)));
            var tobeadded = TownOfUsReworked.NormalOptions.MapId switch
            {
                0 => SkeldSpawns,
                1 => MiraSpawns,
                2 => PolusSpawns,
                3 => dlekSSpawns,
                _ => null
            };

            if (tobeadded != null)
                allLocations.AddRange(tobeadded);

            targets.ForEach(x => coordinates.Add(x.PlayerId, allLocations.Random()));
            return coordinates;
        }

        public static Vector3 GetVentPosition(Vent vent)
        {
            var destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }

        public static IEnumerator Fade(bool fadeAway, bool enableAfterFade)
        {
            HUD.FullScreen.enabled = true;

            if (fadeAway)
            {
                for (var i = 1f; i >= 0; i -= Time.deltaTime)
                {
                    HUD.FullScreen.color = new(0, 0, 0, i);
                    yield return null;
                }
            }
            else
            {
                for (var i = 0f; i <= 1; i += Time.deltaTime)
                {
                    HUD.FullScreen.color = new(0, 0, 0, i);
                    yield return null;
                }
            }

            if (enableAfterFade)
            {
                var fs = false;
                var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                if (reactor.IsActive)
                    fs = true;

                HUD.FullScreen.enabled = fs;
            }
        }

        public static void Teleport(PlayerControl player, Vector3 position)
        {
            player.MyPhysics.ResetMoveState();
            player.NetTransform.RpcSnapTo(new(position.x, position.y));

            if (IsSubmerged && CustomPlayer.Local == player)
            {
                ChangeFloor(player.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            if (CustomPlayer.Local == player)
            {
                Flash(Colors.Teleporter);

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (Map)
                    Map.Close();
            }

            player.moveable = true;
            player.Collider.enabled = true;
            player.NetTransform.enabled = true;
        }

        public static Dictionary<byte, int> CalculateAllVotes(this MeetingHud __instance, out bool tie, out KeyValuePair<byte, int> max)
        {
            var dictionary = new Dictionary<byte, int>();

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                if (!playerVoteArea.DidVote || playerVoteArea.AmDead || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
                    continue;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                    dictionary[playerVoteArea.VotedFor] = num + 1;
                else
                    dictionary[playerVoteArea.VotedFor] = 1;
            }

            foreach (var role in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
            {
                foreach (var number in role.ExtraVotes)
                {
                    if (dictionary.TryGetValue(number, out var num))
                        dictionary[number] = num + 1;
                    else
                        dictionary[number] = 1;
                }
            }

            foreach (var role in Role.GetRoles<Mayor>(RoleEnum.Mayor))
            {
                if (role.Revealed)
                {
                    if (dictionary.TryGetValue(role.Voted, out var num))
                        dictionary[role.Voted] = num + CustomGameOptions.MayorVoteCount;
                    else
                        dictionary[role.Voted] = 1 + CustomGameOptions.MayorVoteCount;
                }
            }

            var knighted = new List<byte>();

            foreach (var role in Role.GetRoles<Monarch>(RoleEnum.Monarch))
            {
                foreach (var id in role.Knighted)
                {
                    if (!knighted.Contains(id))
                    {
                        var area = VoteAreaById(id);

                        if (dictionary.TryGetValue(area.VotedFor, out var num))
                            dictionary[area.VotedFor] = num + CustomGameOptions.KnightVoteCount;
                        else
                            dictionary[area.VotedFor] = 1 + CustomGameOptions.KnightVoteCount;

                        knighted.Add(id);
                    }
                }
            }

            foreach (var swapper in Ability.GetAbilities<Swapper>(AbilityEnum.Swapper))
            {
                if (swapper.IsDead || swapper.Disconnected || swapper.Swap1 == null || swapper.Swap2 == null)
                    continue;

                var swapPlayer1 = PlayerByVoteArea(swapper.Swap1);
                var swapPlayer2 = PlayerByVoteArea(swapper.Swap2);

                if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                    continue;

                var swap1 = 0;
                var swap2 = 0;

                if (dictionary.TryGetValue(swapper.Swap1.TargetPlayerId, out var value))
                    swap1 = value;

                if (dictionary.TryGetValue(swapper.Swap2.TargetPlayerId, out var value2))
                    swap2 = value2;

                dictionary[swapper.Swap2.TargetPlayerId] = swap1;
                dictionary[swapper.Swap1.TargetPlayerId] = swap2;
            }

            max = dictionary.MaxPair(out tie);

            if (tie)
            {
                foreach (var player in __instance.playerStates)
                {
                    if (!player.DidVote || player.AmDead || player.VotedFor == PlayerVoteArea.MissedVote || player.VotedFor == PlayerVoteArea.DeadVote)
                        continue;

                    if (PlayerByVoteArea(player).Is(AbilityEnum.Tiebreaker))
                    {
                        if (dictionary.TryGetValue(player.VotedFor, out var num))
                            dictionary[player.VotedFor] = num + 1;
                        else
                            dictionary[player.VotedFor] = 1;
                    }
                }
            }

            dictionary.MaxPair(out tie);
            return dictionary;
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(255, int.MinValue);

            foreach (var keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                    tie = true;
            }

            return result;
        }

        public static void AssignPostmortals(bool revealer, bool ghoul, bool banshee, bool phantom, PlayerControl player = null)
        {
            if (!AmongUsClient.Instance.AmHost)
                return;

            if (player != null)
            {
                if (SetPostmortals.RevealerOn && !SetPostmortals.WillBeRevealers.Contains(player) && player.Is(Faction.Crew) && SetPostmortals.WillBeRevealers.Count(x => x != null) <
                    CustomGameOptions.RevealerCount)
                {
                    SetPostmortals.WillBeRevealers.Add(player);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetRevealer, player);
                }
                else if (SetPostmortals.PhantomOn && !SetPostmortals.WillBePhantoms.Contains(player) && player.Is(Faction.Neutral) && SetPostmortals.WillBePhantoms.Count(x => x != null) <
                    CustomGameOptions.PhantomCount && !NeutralHasUnfinishedBusiness(player))
                {
                    SetPostmortals.WillBePhantoms.Add(player);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetPhantom, player);
                }
                else if (SetPostmortals.BansheeOn && !SetPostmortals.WillBeBanshees.Contains(player) && player.Is(Faction.Syndicate) && SetPostmortals.WillBeBanshees.Count(x => x != null)
                    < CustomGameOptions.BansheeCount)
                {
                    SetPostmortals.WillBeBanshees.Add(player);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetBanshee, player);
                }
                else if (SetPostmortals.GhoulOn && !SetPostmortals.WillBeGhouls.Contains(player) && player.Is(Faction.Intruder) && SetPostmortals.WillBeGhouls.Count(x => x != null) <
                    CustomGameOptions.GhoulCount)
                {
                    SetPostmortals.WillBeGhouls.Add(player);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetGhoul, player);
                }
            }
            else if (revealer)
            {
                var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Crew) && x.Data.IsDead && !x.Data.Disconnected).ToList();

                if (toChooseFrom.Count == 0)
                    CallRpc(CustomRPC.Misc, MiscRPC.SetRevealer, 255);
                else
                {
                    var pc = toChooseFrom.Random();
                    SetPostmortals.WillBeRevealers.Add(pc);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetRevealer, pc);
                }
            }
            else if (phantom)
            {
                var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Neutral) && !NeutralHasUnfinishedBusiness(x) && x.Data.IsDead && !x.Data.Disconnected)
                    .ToList();

                if (toChooseFrom.Count == 0)
                    CallRpc(CustomRPC.Misc, MiscRPC.SetPhantom, 255);
                else
                {
                    var pc = toChooseFrom.Random();
                    SetPostmortals.WillBePhantoms.Add(pc);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetPhantom, pc);
                }
            }
            else if (banshee)
            {
                var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Syndicate) && x.Data.IsDead && !x.Data.Disconnected).ToList();

                if (toChooseFrom.Count == 0)
                    CallRpc(CustomRPC.Misc, MiscRPC.SetBanshee, 255);
                else
                {
                    var pc = toChooseFrom.Random();
                    SetPostmortals.WillBeBanshees.Add(pc);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetBanshee, pc);
                }
            }
            else if (ghoul)
            {
                var toChooseFrom = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Intruder) && x.Data.IsDead && !x.Data.Disconnected).ToList();

                if (toChooseFrom.Count == 0)
                    CallRpc(CustomRPC.Misc, MiscRPC.SetGhoul, 255);
                else
                {
                    var pc = toChooseFrom.Random();
                    SetPostmortals.WillBeGhouls.Add(pc);
                    CallRpc(CustomRPC.Misc, MiscRPC.SetGhoul, pc);
                }
            }
        }

        public static void AssignPostmortals(PlayerControl player) => AssignPostmortals(false, false, false, false, player);

        public static void ReassignPostmortals(PlayerControl player)
        {
            var revealer = SetPostmortals.WillBeRevealers.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBeRevealers.Count < CustomGameOptions.RevealerCount;
            var phantom = SetPostmortals.WillBePhantoms.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBePhantoms.Count < CustomGameOptions.PhantomCount;
            var banshee = SetPostmortals.WillBeBanshees.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBeBanshees.Count < CustomGameOptions.BansheeCount;
            var ghoul = SetPostmortals.WillBeGhouls.RemoveAll(x => x == player) > 0 && SetPostmortals.WillBeGhouls.Count < CustomGameOptions.GhoulCount;

            SetPostmortals.WillBeRevealers.RemoveAll(x => x == null);
            SetPostmortals.WillBePhantoms.RemoveAll(x => x == null);
            SetPostmortals.WillBeBanshees.RemoveAll(x => x == null);
            SetPostmortals.WillBeGhouls.RemoveAll(x => x == null);

            AssignPostmortals(revealer, ghoul, banshee, phantom);
        }

        public static PlayerControl GetClosestPlayer(this PlayerControl refPlayer, List<PlayerControl> allPlayers = null, float maxDistance = 0f, bool ignoreWalls = false)
        {
            if (refPlayer.Data.IsDead && !refPlayer.Is(RoleEnum.Jester) && !refPlayer.Is(RoleEnum.Ghoul))
                return null;

            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            PlayerControl closestPlayer = null;
            allPlayers ??= CustomPlayer.AllPlayers;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var player in allPlayers)
            {
                if (player.Data.IsDead || player == refPlayer || !player.Collider.enabled || player.onLadder || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargetting))
                    continue;

                var distance = Vector2.Distance(truePosition, player.GetTruePosition());
                var vector = player.GetTruePosition() - truePosition;

                if (distance > closestDistance || distance > maxDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                    continue;

                closestPlayer = player;
                closestDistance = distance;
            }

            return closestPlayer;
        }

        public static Vent GetClosestVent(this PlayerControl refPlayer, float maxDistance = 0f, bool ignoreWalls = false)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            Vent closestVent = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance / 2;

            foreach (var vent in AllVents)
            {
                var distance = Vector2.Distance(truePosition, new(vent.transform.position.x, vent.transform.position.y));
                var vector = new Vector2(vent.transform.position.x, vent.transform.position.y) - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                    continue;

                closestVent = vent;
                closestDistance = distance;
            }

            return closestVent;
        }

        public static DeadBody GetClosestBody(this PlayerControl refPlayer, float maxDistance = 0f, bool ignoreWalls = false)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            DeadBody closestBody = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var body in AllBodies)
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);
                var vector = body.TruePosition - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask) && !ignoreWalls)
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            return closestBody;
        }

        public static void RemoveTasks(PlayerControl player)
        {
            foreach (var task in player.myTasks)
            {
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();
                    var updateArrow = normalPlayerTask.taskStep > 0;
                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();

                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        UObject.FindObjectsOfType<TowelTaskConsole>().ToList().ForEach(x => x.Image.color = UColor.white);

                    normalPlayerTask.taskStep = 0;

                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;

                    if ((normalPlayerTask.TaskType is TaskTypes.EmptyGarbage or TaskTypes.EmptyChute) && (TownOfUsReworked.NormalOptions.MapId is 0 or 3 or 4))
                        normalPlayerTask.taskStep = 1;

                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
            }
        }

        public static float CycleFloat(float max, float min, float currentVal, bool increment, float change = 1f)
        {
            var result = currentVal;
            var value = increment ? change : -change;

            if (currentVal + value > max)
                result = min;
            else if (currentVal + value < min)
                result = max;
            else
                result += value;

            return result;
        }

        public static int CycleInt(int max, int min, int currentVal, bool increment, int change = 1) => (int)CycleFloat(max, min, currentVal, increment, change);

        public static byte CycleByte(int max, int min, int currentVal, bool increment, int change = 1) => (byte)CycleFloat(max, min, currentVal, increment, change);

        public static string WrapText(string text, int width = 90, bool overflow = true)
        {
            var result = new StringBuilder();
            var startIndex = 0;
            var column = 0;

            while (startIndex < text.Length)
            {
                var num = text.IndexOfAny(new char[] { ' ', '\t', '\r' }, startIndex);

                if (num != -1)
                {
                    if (num == startIndex)
                        ++startIndex;
                    else if (text[startIndex] == '\n')
                        startIndex++;
                    else
                    {
                        AddWord(text[startIndex..num]);
                        startIndex = num + 1;
                    }
                }
                else
                    break;
            }

            if (startIndex < text.Length)
                AddWord(text[startIndex..]);

            return result.ToString();

            void AddWord(string word)
            {
                var word1 = "";

                if (!overflow && word.Length > width)
                {
                    for (var startIndex = 0; startIndex < word.Length; startIndex += word1.Length)
                    {
                        word1 = word.Substring(startIndex, Math.Min(width, word.Length - startIndex));
                        AddWord(word1);
                    }
                }
                else
                {
                    if (column + word.Length >= width)
                    {
                        if (column > 0)
                        {
                            result.AppendLine();
                            column = 0;
                        }
                    }
                    else if (column > 0)
                    {
                        result.Append(' ');
                        column++;
                    }

                    result.Append(word);
                    column += word.Length;
                }
            }
        }

        public static string WrapTexts(List<string> texts, int width = 90, bool overflow = true)
        {
            var result = WrapText(texts[0], width, overflow);
            texts.Skip(1).ToList().ForEach(x => result += $"\n{WrapText(x, width, overflow)}");
            return result;
        }
    }
}