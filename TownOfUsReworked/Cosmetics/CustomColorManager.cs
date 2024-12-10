namespace TownOfUsReworked.Cosmetics;

public static class CustomColorManager
{
    public static readonly Dictionary<int, CustomColor> AllColors = [];

    public static void SetColor(Renderer rend, int id)
    {
        if (AllColors.TryGetValue(id, out var color))
            SetColor(rend, color.GetColor(), color.GetShadowColor());
    }

    public static void SetColor(Renderer rend, UColor color, UColor? shadow = null)
    {
        if (!rend)
            return;

        rend.material.SetColor(PlayerMaterial.BackColor, shadow ?? Shadow(color));
        rend.material.SetColor(PlayerMaterial.BodyColor, color);
        rend.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
    }

    public static bool IsChanging(this int id) => AllColors.TryGetValue(id, out var color) && color.Changing;

    public static bool IsLighter(this int id) => AllColors.TryGetValue(id, out var color) && color.Lighter;

    public static UColor GetColor(this int id, bool shadow) => AllColors.TryGetValue(id, out var color) ? (shadow ? color.GetShadowColor() : color.GetColor()) : UColor.white;

    public static UColor Shadow(this UColor color, float val = 0.2f) => new(Mathf.Clamp01(color.r - val), Mathf.Clamp01(color.g - val), Mathf.Clamp01(color.b - val), color.a);

    public static UColor Light(this UColor color, float val = 0.2f) => new(Mathf.Clamp01(color.r + val), Mathf.Clamp01(color.g + val), Mathf.Clamp01(color.b + val), color.a);

    public static UColor Alternate(this UColor color, float val = 0.2f) => color.IsColorDark() ? color.Light(val) : color.Shadow(val);

    public static bool IsColorDark(this UColor color) => color is { r: <= 0.5f, g: <= 0.5f, b: <= 0.5f };

    public static UColor FromHex(string hexCode) => ColorUtility.TryParseHtmlString(hexCode, out var color) ? color : default;

    public static Color32 Shadow(this Color32 color) => new(ClampByte(color.r - 51, 0, 255), ClampByte(color.g - 51, 0, 255), ClampByte(color.b - 51, 0, 255), color.a);

    public static Color32 Light(this Color32 color) => new(ClampByte(color.r + 51, 0, 255), ClampByte(color.g + 51, 0, 255), ClampByte(color.b + 51, 0, 255), color.a);

    public static Color32 Alternate(this Color32 color, byte val = 51) => ((UColor)color).Alternate((float)val / 255);

    public static bool IsColorDark(this Color32 color) => color is { r: < 128, g: < 128, b: 128 };

    // Layer Colors
    public static readonly UColor Role = FromHex("#FFD700FF");
    public static readonly UColor Modifier = FromHex("#7F7F7FFF");
    public static readonly UColor Ability = FromHex("#FF9900FF");
    public static readonly UColor Layer = FromHex("#2684C1FF");
    public static readonly UColor Disposition = FromHex("#DD585BFF");

    // Faction Colors
    public static readonly UColor Crew = FromHex("#8CFFFFFF");
    public static readonly UColor Neutral = FromHex("#B3B3B3FF");
    public static readonly UColor Intruder = FromHex("#FF1919FF");
    public static readonly UColor Syndicate = FromHex("#008000FF");
    public static readonly UColor Faction = FromHex("#00E66DFF");

    // Game Mode Colors
    public static readonly UColor GameMode = FromHex("#A81538FF");
    public static readonly UColor TaskRace = FromHex("#1E49CFFF");
    public static readonly UColor HideAndSeek = FromHex("#7500AFFF");
    public static readonly UColor Classic = FromHex("#C02A2CFF");
    public static readonly UColor Custom = FromHex("#E6956AFF");
    public static readonly UColor AllAny = FromHex("#CBD542FF");
    public static readonly UColor RoleList = FromHex("#FA1C79FF");
    public static readonly UColor KillingOnly = FromHex("#06E00CFF");

    // Task Race Colors
    public static readonly UColor Runner = FromHex("#ECC23EFF");

    // Hide And Seek Colors
    public static readonly UColor Hunter = FromHex("#FF004EFF");
    public static readonly UColor Hunted = FromHex("#1F51FFFF");

    // Subfaction Colors
    public static readonly UColor Undead = FromHex("#7B8968FF");
    public static readonly UColor Cabal = FromHex("#575757FF");
    public static readonly UColor Reanimated = FromHex("#E6108AFF");
    public static readonly UColor Sect = FromHex("#F995FCFF");
    public static readonly UColor Apocalypse = FromHex("#A7C596FF");
    public static readonly UColor SubFaction = FromHex("#204D42FF");
    public static readonly UColor Attack = FromHex("#E37C21FF");
    public static readonly UColor Defense = FromHex("#2B0538FF");

    // Crew Colors
    public static readonly UColor Mayor = FromHex("#704FA8FF");
    public static readonly UColor Vigilante = FromHex("#FFFF00FF");
    public static readonly UColor Engineer = FromHex("#FFA60AFF");
    public static readonly UColor Medic = FromHex("#006600FF");
    public static readonly UColor Sheriff = FromHex("#FFCC80FF");
    public static readonly UColor Altruist = FromHex("#660000FF");
    public static readonly UColor Veteran = FromHex("#998040FF");
    public static readonly UColor Tracker = FromHex("#009900FF");
    public static readonly UColor Transporter = FromHex("#00EEFFFF");
    public static readonly UColor Medium = FromHex("#A680FFFF");
    public static readonly UColor Coroner = FromHex("#4D99E6FF");
    public static readonly UColor Operative = FromHex("#A7D1B3FF");
    public static readonly UColor Detective = FromHex("#4D4DFFFF");
    public static readonly UColor Shifter = FromHex("#DF851FFF");
    public static readonly UColor VampireHunter = FromHex("#C0C0C0FF");
    public static readonly UColor Escort = FromHex("#803333FF");
    public static readonly UColor Bastion = FromHex("#7E3C64FF");
    public static readonly UColor Revealer = FromHex("#D3D3D3FF");
    public static readonly UColor Mystic = FromHex("#708EEFFF");
    public static readonly UColor Retributionist = FromHex("#8D0F8CFF");
    public static readonly UColor Chameleon = FromHex("#5411F8FF");
    public static readonly UColor Seer = FromHex("#71368AFF");
    public static readonly UColor Monarch = FromHex("#FF004EFF");
    public static readonly UColor Dictator = FromHex("#00CB97FF");
    public static readonly UColor Trapper = FromHex("#BE1C8CFF");

    // Neutral Colors
    public static readonly UColor Jester = FromHex("#F7B3DAFF");
    public static readonly UColor Executioner = FromHex("#CCCCCCFF");
    public static readonly UColor Glitch = FromHex("#00FF00FF");
    public static readonly UColor Arsonist = FromHex("#EE7600FF");
    public static readonly UColor Amnesiac = FromHex("#22FFFFFF");
    public static readonly UColor Survivor = FromHex("#DDDD00FF");
    public static readonly UColor GuardianAngel = FromHex("#FFFFFFFF");
    public static readonly UColor Plaguebearer = FromHex("#CFFE61FF");
    public static readonly UColor Pestilence = FromHex("#424242FF");
    public static readonly UColor Werewolf = FromHex("#9F703AFF");
    public static readonly UColor Cannibal = FromHex("#8C4005FF");
    public static readonly UColor Juggernaut = FromHex("#A12B56FF");
    public static readonly UColor Dracula = FromHex("#AC8A00FF");
    public static readonly UColor Murderer = FromHex("#6F7BEAFF");
    public static readonly UColor SerialKiller = FromHex("#336EFFFF");
    public static readonly UColor Cryomaniac = FromHex("#642DEAFF");
    public static readonly UColor Thief = FromHex("#80FF00FF");
    public static readonly UColor Troll = FromHex("#678D36FF");
    public static readonly UColor Jackal = FromHex("#45076AFF");
    public static readonly UColor Phantom = FromHex("#662962FF");
    public static readonly UColor Necromancer = FromHex("#BF5FFFFF");
    public static readonly UColor Whisperer = FromHex("#2D6AA5FF");
    public static readonly UColor Guesser = FromHex("#EEE5BEFF");
    public static readonly UColor Actor = FromHex("#00ACC2FF");
    public static readonly UColor BountyHunter = FromHex("#B51E39FF");
    public static readonly UColor Betrayer = FromHex("#11806AFF");

    // Intruder Colors
    public static readonly UColor Consigliere = FromHex("#FFFF99FF");
    public static readonly UColor Grenadier = FromHex("#85AA5BFF");
    public static readonly UColor Morphling = FromHex("#BB45B0FF");
    public static readonly UColor Wraith = FromHex("#5C4F75FF");
    public static readonly UColor Camouflager = FromHex("#378AC0FF");
    public static readonly UColor Janitor = FromHex("#2647A2FF");
    public static readonly UColor Miner = FromHex("#AA7632FF");
    public static readonly UColor Blackmailer = FromHex("#02A752FF");
    public static readonly UColor Disguiser = FromHex("#40B4FFFF");
    public static readonly UColor Consort = FromHex("#801780FF");
    public static readonly UColor Teleporter = FromHex("#939593FF");
    public static readonly UColor Godfather = FromHex("#404C08FF");
    public static readonly UColor Mafioso = FromHex("#6400FFFF");
    public static readonly UColor Ambusher = FromHex("#2BD29CFF");
    public static readonly UColor Ghoul = FromHex("#F1C40FFF");
    public static readonly UColor Enforcer = FromHex("#005643FF");

    // Syndicate Colors
    public static readonly UColor Warper = FromHex("#8C7140FF");
    public static readonly UColor Framer = FromHex("#00FFFFFF");
    public static readonly UColor Rebel = FromHex("#FFFCCEFF");
    public static readonly UColor Sidekick = FromHex("#979C9FFF");
    public static readonly UColor Concealer = FromHex("#C02525FF");
    public static readonly UColor Shapeshifter = FromHex("#2DFF00FF");
    public static readonly UColor Bomber = FromHex("#C9CC3FFF");
    public static readonly UColor Poisoner = FromHex("#B5004CFF");
    public static readonly UColor Crusader = FromHex("#DF7AE8FF");
    public static readonly UColor Banshee = FromHex("#E67E22FF");
    public static readonly UColor Collider = FromHex("#B345FFFF");
    public static readonly UColor Stalker = FromHex("#7E4D00FF");
    public static readonly UColor Spellslinger = FromHex("#0028F5FF");
    public static readonly UColor Drunkard = FromHex("#FF7900FF");
    public static readonly UColor Timekeeper = FromHex("#3769FEFF");
    public static readonly UColor Silencer = FromHex("#AAB43EFF");

    // Modifier Colors
    public static readonly UColor Bait = FromHex("#00B3B3FF");
    public static readonly UColor Coward = FromHex("#456BA8FF");
    public static readonly UColor Diseased = FromHex("#374D1EFF");
    public static readonly UColor Drunk = FromHex("#758000FF");
    public static readonly UColor Dwarf = FromHex("#FF8080FF");
    public static readonly UColor Giant = FromHex("#FFB34DFF");
    public static readonly UColor Volatile = FromHex("#FFA60AFF");
    public static readonly UColor VIP = FromHex("#DCEE85FF");
    public static readonly UColor Shy = FromHex("#1002C5FF");
    public static readonly UColor Professional = FromHex("#860B7AFF");
    public static readonly UColor Indomitable = FromHex("#2DE5BEFF");
    public static readonly UColor Astral = FromHex("#612BEFFF");
    public static readonly UColor Yeller = FromHex("#F6AAB7FF");
    public static readonly UColor Colorblind = FromHex("#B34D99FF");

    // Ability Colors
    public static readonly UColor Assassin = FromHex("#073763FF");
    public static readonly UColor Torch = FromHex("#FFFF99FF");
    public static readonly UColor Tunneler = FromHex("#E91E63FF");
    public static readonly UColor ButtonBarry = FromHex("#E600FFFF");
    public static readonly UColor Tiebreaker = FromHex("#99E699FF");
    public static readonly UColor Snitch = FromHex("#D4AF37FF");
    public static readonly UColor Underdog = FromHex("#841A7FFF");
    public static readonly UColor Insider = FromHex("#26FCFBFF");
    public static readonly UColor Radar = FromHex("#FF0080FF");
    public static readonly UColor Multitasker = FromHex("#FF804DFF");
    public static readonly UColor Ruthless = FromHex("#2160DDFF");
    public static readonly UColor Ninja = FromHex("#A84300FF");
    public static readonly UColor Politician = FromHex("#CCA3CCFF");
    public static readonly UColor Swapper = FromHex("#66E666FF");

    // Disposition Colors
    public static readonly UColor Lovers = FromHex("#FF66CCFF");
    public static readonly UColor Traitor = FromHex("#370D43FF");
    public static readonly UColor Rivals = FromHex("#3D2D2CFF");
    public static readonly UColor Fanatic = FromHex("#678D36FF");
    public static readonly UColor Taskmaster = FromHex("#ABABFFFF");
    public static readonly UColor Overlord = FromHex("#008080FF");
    public static readonly UColor Corrupted = FromHex("#4545FFFF");
    public static readonly UColor Allied = FromHex("#4545A9FF");
    public static readonly UColor Mafia = FromHex("#00EEFFFF");
    public static readonly UColor Defector = FromHex("#E1C849FF");
    public static readonly UColor Linked = FromHex("#FF351FFF");

    // Other
    public static readonly UColor Stalemate = FromHex("#E6E6E6FF");
    public static readonly UColor Alignment = FromHex("#1D7CF2FF");
    public static readonly UColor Status = FromHex("#9B59B6FF");
    public static readonly UColor Clear = FromHex("#00000000");
    public static readonly UColor Objectives = FromHex("#B148E2FF");
    public static readonly UColor Attributes = FromHex("#EC1C45FF");
    public static readonly UColor Abilities =FromHex("#206694FF");
    public static readonly UColor What = FromHex("#6697FFFF");
    public static readonly UColor FirstShield = FromHex("#C2185BFF");

    // Stuff
    public static readonly Color32 NormalVision = new(212, 212, 212, 0);
    public static readonly Color32 DimVision = new(212, 212, 212, 51);
    public static readonly Color32 BlindVision = new(212, 212, 212, 255);

    // Color Storage
    // #dcee85 #6c29ab #800000 #808000 #008000 #800080 #000080 #e74c3c #992d22 #00FFFD #917ac0 #Eac1d2 #286e58 #db4f20 #abd432 #2e3b97 #ffd100 #fffcce #40b4ff #a82626 #8ff731 #942b3b #80B3FF
    // #4e4e4e #fffead #1abc9c #2ecc71 #1f8b4c #3498db #ad1457 #c27c0e #ffd2fb #805bc4 #95a5a6 #979c9f #888888 #ff7272 #f25ff1 #FF00FF #916640 #1AFF74 #2672FF #8637C2 #e7dae2 #9B7038 #EDC240
    // #6a1515 #569d29 #f1612b #7d86e1 #EC62A5 #78c689 #fccc52 #6b2d2a #FCBA03 #F8CD46 #FF4D00 #7EFBC2 #4d4d4d #38b553 #0000FF #0000A7 #f25e75 #5865F2 #0437EF #7FFF00 #FB9327 #FAE57E #06DF0C
    // #1E300B #F3A6D3 #F9BFA7 #E1E4E4 #869919 #78081C #69201B #9000D7 #CF036F #B0BF1A #A64D79 #B3D94D #73AD09 #41d1c3 #B0BF1A #80B2FF #33FF77 #AAFF00 #452112 #663366 #9C4A14 #A9A9A9 #8BFDFD
    // #1A3270 #20A1B7 #606168 #99007F #ECFF45 #603FEF #610F69 #67A836 #B545FF #DB7601 #1D4DFC #6699FF #9D7038 #949797 #F5A6D4 #404040 #9C9A9A #A22929 #B34D99 #FFC34F

    // Symbol Storage
    // ⟡ ☆ ♡ ♧ ♤ ▶ ❥ ι ν ψ ✧ ¢ ⁂ ¤ 彡 个 「 」 요 ロ 卄 王 ī l · ° ◥ ◤ ◢ ◣ 《 》 ︵ ︶ ☆ ☀ ☂ ☹ ☺ ♡ ♩ ♪ ♫ ♬ ✓ ☜ ☞ ☟ ☯ ☃ ✿ ❀ ÷ º ¿ ※ ⁑ ∞ ≠ +
}