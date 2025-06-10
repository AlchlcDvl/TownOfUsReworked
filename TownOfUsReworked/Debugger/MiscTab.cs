// namespace TownOfUsReworked.Debugger;

// public sealed class MiscTab : BaseTab
// {
//     public override string Name => "Misc";
//     private bool LateRpc;

//     public override void OnGUI()
//     {
//         if (!LocalPlayer)
//             return;

//         LateRpc = GUILayout.Toggle(LateRpc, "Late RPC");

//         if (LateRpc)
//         {
//             if (GUILayout.Button("Test RPC (Argless)"))
//                 CallLateRpc(CustomRPC.Test, TestRPC.Argless);

//             if (GUILayout.Button("Test RPC (Arguments)"))
//                 CallLateRpc(CustomRPC.Test, TestRPC.Args, GetRandomisedName());
//         }
//         else
//         {
//             if (GUILayout.Button("Test RPC (Argless)"))
//                 CallRpc(CustomRPC.Test, TestRPC.Argless);

//             if (GUILayout.Button("Test RPC (Arguments)"))
//                 CallRpc(CustomRPC.Test, TestRPC.Args, GetRandomisedName());
//         }
//     }
// }