// namespace TownOfUsReworked.Monos;

// public class PhoneHandler : MonoBehaviour
// {
//     public bool WikiActive;
//     public bool RoleCardActive;
//     public PassiveButton ToTheWiki;
//     public PassiveButton NextButton;
//     public PassiveButton BackButton;
//     public PassiveButton LoreButton;
//     public PassiveButton YourStatus;
//     public readonly Dictionary<int, List<(Info, PassiveButton)>> Buttons = [];
//     public readonly Dictionary<int, KeyValuePair<string, Info>> Sorted = [];
//     public int Page;
//     public int ResultPage;
//     public int MaxPage;
//     public bool PagesSet;
//     public Info Selected;
//     public bool LoreActive;
//     public bool SelectionActive;
//     public readonly List<string> Entry = [];

//     public static PhoneHandler Instance { get; private set; }

//     public PhoneHandler(IntPtr ptr) : base(ptr) => Instance = this;

//     public void Awake()
//     {

//     }
// }