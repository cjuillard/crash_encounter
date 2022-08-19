using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class CENetworkManager : NetworkManager
    {
        [SerializeField] private List<Spawner> spawnLocations;

        private int serverPlayerIndex;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // add player at correct spawn position
            if (numPlayers >= spawnLocations.Count)
                return;

            Spawner spawner = spawnLocations.Find(s => s.Owner == null);
            if(spawner == null)
            {
                Debug.LogWarning("All spawn locations are taken, player cannot join");
                return;
            }
            Transform startLoc = spawner.transform;
            GameObject playerGO = Instantiate(playerPrefab, startLoc.position, startLoc.rotation);
            var player = playerGO.GetComponent<Player>();
            player.Init(serverPlayerIndex++);
            NetworkServer.AddPlayerForConnection(conn, playerGO);

            spawner.Capture(player);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            var playerDisconnecting = conn.identity.gameObject;
            foreach(Spawner spawner in spawnLocations)
            {
                if(spawner.Owner == playerDisconnecting)
                    spawner.GiveUpOwnership();
            }

            base.OnServerDisconnect(conn);
        }
    }
}
