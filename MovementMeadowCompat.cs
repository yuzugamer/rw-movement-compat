using System.Security;
using System.Security.Permissions;
using BepInEx;
using System;
using System.Linq;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MovementMeadowCompat;

[BepInPlugin("yuzugamer.movementmeadowcompat", "Meadow Compat for Movement Mods", "0.5.0")]

public class MovementMeadowCompat : BaseUnityPlugin
{
    private static bool init;

    public void OnEnable()
    {
        On.RainWorld.PostModsInit += On_RainWorld_PostModsInit;
    }


    public static void On_RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        if (init) return;
        {
            init = true;
            try
            {
                if (ModManager.ActiveMods.Any(mod => mod.id == "slime-cubed.inputdisplay"))
                {
                    InputDisplayHooks.Apply();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "alphappy.inputlog"))
                {
                    InputLogHooks.Apply();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "movementmarker"))
                {
                    MovementMarkerHooks.Apply();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "alphappy.checkpointer"))
                {
                    CheckpointerHooks.Apply();
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Movement mod meadow compat failed to load!");
                UnityEngine.Debug.LogException(ex);
            }
        }
    }
}