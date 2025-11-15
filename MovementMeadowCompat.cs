using System.Security;
using System.Security.Permissions;
using BepInEx;
using System;
using System.Linq;
using BepInEx.Logging;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MovementMeadowCompat;

[BepInPlugin("yuzugamer.movementmeadowcompat", "Meadow Compat for Movement Mods", "0.5.0")]

public class MovementMeadowCompat : BaseUnityPlugin
{
    private static bool init;
    internal static MovementMeadowCompat instance;

    public void OnEnable()
    {
        instance = this;
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
                LogError("Movement mod meadow compat failed to load!");
                LogError(ex);
            }
        }
    }

    public static void LogError(object error)
    {
        instance.Logger.LogError(error);
    }
}