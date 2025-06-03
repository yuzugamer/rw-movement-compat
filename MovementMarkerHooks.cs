using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RainMeadow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWMovementMarker;
using System.Reflection;

namespace MovementMeadowCompat;

public static class MovementMarkerHooks
{
    public static void Apply()
    {
        new ILHook(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsClass && t.Namespace == nameof(RWMovementMarker)).First(t => t.Name == "StatButton").GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), IL_StatButton_Update);
    }

    public static Player GetOnlinePlayer(Player orig)
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
                return player;
            }
        }
        return orig;
    }

    public static void IL_StatButton_Update(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);
            c.GotoNext(
                MoveType.After,
                x => x.MatchLdarg(1),
                x => x.MatchCallOrCallvirt<RainWorldGame>("get_Players"),
                x => x.MatchLdcI4(0),
                x => x.MatchCallOrCallvirt(out var _),
                x => x.MatchLdfld<AbstractPhysicalObject>(nameof(AbstractPhysicalObject.realizedObject)),
                x => x.MatchIsinst<Player>()
            );
            c.MoveAfterLabels();
            c.EmitDelegate(GetOnlinePlayer);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
    }
}
