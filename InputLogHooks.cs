using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using alphappy.InputLog;
using MonoMod.RuntimeDetour;
using RainMeadow;

namespace MovementMeadowCompat;

public static class InputLogHooks
{
    public static bool actuallyFound;
    public static FieldInfo trackedPlayer;
    public static void Apply()
    {
        trackedPlayer = typeof(Mod).GetField("trackedPlayer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        new Hook(typeof(Mod).GetMethod("Player_Update", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), On_InputLog_Player_Update);
        On.RainWorldGame.ShutDownProcess += On_RainWorldGame_ShutDownProcess;
    }

    public static void On_InputLog_Player_Update(Action<Mod, On.Player.orig_Update, Player, bool> orig, Mod _, On.Player.orig_Update origorig, Player self, bool eu)
    {
        if (!actuallyFound && OnlineManager.lobby != null && OnlineManager.lobby.playerAvatars != null)
        {
            actuallyFound = false;
            Player player = null;
            OnlineEntity.EntityId avatar = null;
            foreach (var kvp in OnlineManager.lobby.playerAvatars)
            {
                if (kvp.Key == OnlineManager.mePlayer)
                {
                    avatar = kvp.Value;
                    break;
                }
            }
            if (avatar != null && avatar.FindEntity() is OnlineCreature oc && oc.realizedCreature != null && oc.realizedCreature is Player)
            {
                player = oc.realizedCreature as Player;
                actuallyFound = true;
            }
            trackedPlayer.SetValue(null, player);
        }
        orig(_, origorig, self, eu);
    }

    public static void On_RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
    {
        orig(self);
        actuallyFound = false;
    }
}
