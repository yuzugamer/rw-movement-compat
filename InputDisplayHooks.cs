using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod.RuntimeDetour;
using RainMeadow;
using RWInputDisplay;
using System.Reflection;

namespace MovementMeadowCompat;

public static class InputDisplayHooks
{
    public static void Apply()
    {
        new Hook(typeof(RWInputDisplay.RWInputDisplay.InputGraphic).GetMethod("get_CurrentInput", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), On_InputDisplay_CurrentInput);
    }

    public static Player.InputPackage On_InputDisplay_CurrentInput(Func<RWInputDisplay.RWInputDisplay.InputGraphic, Player.InputPackage> orig, RWInputDisplay.RWInputDisplay.InputGraphic self)
    {
        if (OnlineManager.lobby != null && OnlineManager.lobby.playerAvatars != null)
        {
            OnlineEntity.EntityId avatar = null;
            foreach (var kvp in OnlineManager.lobby.playerAvatars)
            {
                if (kvp.Key == OnlineManager.mePlayer)
                {
                    avatar = kvp.Value;
                    break;
                }
            }
            if (avatar != null && avatar.FindEntity() is OnlineCreature oc && oc.realizedCreature != null && oc.realizedCreature is Player player)
            {
                return player.input[0];
            }
        }
        return orig(self);
    }
}
