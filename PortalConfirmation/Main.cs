using Harmony;
using MelonLoader;
using System;
using System.Linq;
using System.Reflection;
using VRC;

[assembly: MelonModInfo(typeof(PortalConfirmation.Main), "Portal Confirmation", "1.2.0", "404#0004", "https://github.com/l-404-l/Portal-Confirmation/releases")] //Pepega Knah Assembly XD 
[assembly: MelonModGame("VRChat", "VRChat")]

namespace PortalConfirmation
{
    public class Main : MelonMod
    {
        public static HarmonyInstance PCP = HarmonyInstance.Create("PortalConfirmation404"); // This is so no other mod will fuck with it just stfu leave it lol
        public static MethodInfo PopUpM;
        public static bool BypassThatThing = false;
        //public delegate void EnterFix(IntPtr instance);
        //public static EnterFix OldEnter;
        public override unsafe void OnApplicationStart()
        {
            foreach (var t in typeof(PortalInternal).GetMethods().ToList().FindAll(x =>
            {
                if (!x.Name.Contains("Method_Public_Void_"))
                    return false;
                try
                {
                    if (UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(x).Any(z => z.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Global && z.ReadAsObject() != null && z.ReadAsObject().ToString() == " was at capacity, cannot enter."))
                        return true;

                }
                catch
                {
                    return false;
                }
                return false;
            }))
            {
                PCP.Patch(t, new HarmonyMethod(typeof(Main).GetMethod("EnterConfirm")));
            }

            PopUpM = typeof(VRCUiPopupManager).GetMethods().ToList().First(x =>
            {
                if (!x.Name.Contains("Method_Public_Void_String_String_String_Action_Action_1_VRCUiPopup_"))
                    return false;
                try
                {
                    if (UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(x).Any(z => z.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Global && z.ReadAsObject() != null && z.ReadAsObject().ToString() == "UserInterface/MenuContent/Popups/StandardPopupV2"))
                        return true;
                }
                catch { }
                return false;
            });

            //var getoriginal = (IntPtr)typeof(PortalInternal).GetField("NativeMethodInfoPtr_Method_Public_Void_4", AccessTools.all).GetValue(null);
            //var original = *(IntPtr*)getoriginal;
            //Imports.Hook((IntPtr)(&original), Marshal.GetFunctionPointerForDelegate(new Action<IntPtr>(EnterConfirm))); // GAYYYYYYY
            //OldEnter = Marshal.GetDelegateForFunctionPointer<EnterFix>(original);
        }

        public static void ShowPopUPthingy(string a, string b, string c, Action d, Action<VRCUiPopup> e = null) // ADCS!!!
        {
            Il2CppSystem.Action f = d;
            Il2CppSystem.Action<VRCUiPopup> g = e;
            PopUpM.Invoke(VRCUiPopupManager.prop_VRCUiPopupManager_0, new object[] { a, b, c, f, g });
        }

        public static bool EnterConfirm(PortalInternal __instance, MethodBase __originalMethod)
        {
            if (!BypassThatThing)
            {
                if (__instance != null)
                {
                    var dropper = PlayerManager.Method_Public_Static_Player_Int32_0(__instance.field_Internal_Int32_0);
                    ShowPopUPthingy("Portal Confirmation", "Would you like to enter this world? \n" +
                        $"Name: {__instance.field_Private_ApiWorld_0.name}\n" +
                        $"Author: {__instance.field_Private_ApiWorld_0.authorName}\n" +
                        $"Player: {(dropper != null ? dropper.field_Private_APIUser_0.displayName : "No Player")}", "Yes", new Action(() =>
                        {
                            BypassThatThing = true;
                            __originalMethod.Invoke(__instance, null);
                        }));
                }
                return false;
            }
            BypassThatThing = false;
            return true;
        }
    }
}
