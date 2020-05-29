using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VRC;

[assembly: MelonModInfo(typeof(PortalConfirmation.Main), "Portal Confirmation", "1.0.0", "404#0004", null)]
[assembly: MelonModGame("VRChat", "VRChat")]

namespace PortalConfirmation
{
    public class Main : MelonMod
    {
        public static MethodInfo HookM = typeof(Imports).GetMethod("Hook", BindingFlags.Public | BindingFlags.Static);
        public delegate void EnterFix(IntPtr instance);
        public static EnterFix OldEnter;
        public static void HookMethod(IntPtr OldMethod, IntPtr NewMethod)
        {
            HookM.Invoke(null, new object[] { OldMethod, NewMethod });
        }
        public override unsafe void OnApplicationStart()
        {
            var getoriginal = (IntPtr)typeof(PortalInternal).GetField("NativeMethodInfoPtr_Method_Public_Void_0", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            var original = *(IntPtr*)getoriginal;
            HookMethod((IntPtr)(&original), Marshal.GetFunctionPointerForDelegate(new Action<IntPtr>(EnterConfirm)));
            OldEnter = Marshal.GetDelegateForFunctionPointer<EnterFix>(original);
        }

        public static void EnterConfirm(IntPtr instance)
        {
            if (IntPtr.Zero != instance)
            {
                var portal = new PortalInternal(instance);
                var dropper = PlayerManager.Method_Public_Static_Player_Int32_0(portal.field_Internal_Int32_0);
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.
                    Method_Public_Void_String_String_String_Action_Action_1_VRCUiPopup_0("Portal Confirmation", "Would you like to enter this world? \n" +
                    $"Name: {portal.field_Private_ApiWorld_0.name}\n" +
                    $"Author: {portal.field_Private_ApiWorld_0.authorName}\n" +
                    $"Player: {(dropper != null ? dropper.field_Private_APIUser_0.displayName : "Null")}", "Yes", new Action(() => {
                        OldEnter(instance); VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.Method_Public_Void_2(); 
                    }));
            }
        }
    }
}
