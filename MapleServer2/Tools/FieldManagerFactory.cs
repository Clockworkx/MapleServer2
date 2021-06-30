﻿using System.Collections.Generic;
using System.Linq;
using MapleServer2.Servers.Game;
using MapleServer2.Types;

namespace MapleServer2.Tools
{
    public class FieldManagerFactory
    {
        private readonly Dictionary<int, List<FieldManager>> Managers;

        public FieldManagerFactory()
        {
            Managers = new Dictionary<int, List<FieldManager>>();
        }

        public FieldManager GetManager(int key, int instanceId)
        {
            lock (Managers)
            {
                if (!Managers.TryGetValue(key, out List<FieldManager> list))
                {
                    list = new List<FieldManager>() { new FieldManager(key, instanceId) };
                    Managers[key] = list;
                }

                FieldManager manager = list.FirstOrDefault(x => x.InstanceId == instanceId);
                if (manager == default)
                {
                    manager = new FieldManager(key, instanceId);
                    Managers[key].Add(manager);
                }

                manager.Increment();
                return manager;
            }
        }

        public bool Release(int key, int instanceId, Player player)
        {
            lock (Managers)
            {
                if (!Managers.TryGetValue(key, out List<FieldManager> managerList))
                {
                    return false;
                }

                FieldManager fieldManager = managerList.FirstOrDefault(x => x.InstanceId == instanceId);
                if (fieldManager == default || fieldManager.Decrement() > 0)
                {
                    return false;
                }

                //is only called if the leaving player is the last player on the map
                // group or dungeon if
                
                DungeonSession dungeonSession = GameServer.DungeonManager.GetDungeonSession(player.DungeonSessionId);
                if (GameServer.DungeonManager.DungeonUsesMap(dungeonSession, fieldManager, player))
                {
                    return false;
                }

                return managerList.Remove(fieldManager);
            }
        }
    }
}
