using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class GameState : NetworkBehaviour
    {
        private Dictionary<Player, int> ownerCounts = new Dictionary<Player, int>();
        private Spawner[] spawners;
        private List<Player> players = new List<Player>();
        private GamePhase currPhase = GamePhase.NotStarted;

        public void InitGame()
        {
            this.spawners = FindObjectsOfType<Spawner>();
            foreach (Player player in players)
            {
                float minDist2 = float.MaxValue;
                Spawner closestSpawner = null;
                foreach (Spawner spawner in spawners)
                {
                    if (spawner.Owner != null)
                        continue;

                    float dist2 = (spawner.transform.position - player.transform.position).sqrMagnitude;
                    if (dist2 < minDist2)
                    {
                        minDist2 = dist2;
                        closestSpawner = spawner;
                    }
                }

                if (closestSpawner != null)
                {
                    closestSpawner.Capture(player);
                }
            }

            currPhase = GamePhase.Started;
            RpcOnGameStarted();
        }

        [ClientRpc]
        public void RpcOnGameStarted()
        {
            var screen = FindObjectOfType<GameplayScreenUI>();
            screen.OnGameStart();
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                if (currPhase != GamePhase.Started)
                    return;

                // TODO Don't check this every frame
                // check for win conditions
                ownerCounts.Clear();
                foreach (Spawner spawner in spawners)
                {
                    if (spawner.Owner == null)
                        continue;

                    ownerCounts.TryGetValue(spawner.Owner, out int owned);
                    ownerCounts[spawner.Owner] = owned + spawner.ActiveCount;
                }

                foreach (Pawn pawn in FindObjectsOfType<Pawn>())
                {
                    ownerCounts.TryGetValue(pawn.Owner, out int owned);
                    ownerCounts[pawn.Owner] = owned + 1;
                }

                foreach (Player player in players)
                {
                    if (!ownerCounts.TryGetValue(player, out int owned) || owned == 0)
                    {
                        player.SetIsAlive(false);
                    }
                }

                if (ownerCounts.Count == 1)
                {
                    // Winner can be declared
                    foreach (Player winner in ownerCounts.Keys)
                    {
                        GameOver(winner);
                        break;  // should only be 1 anyway
                    }
                }
            }
        }

        private void GameOver(Player winner)
        {
            currPhase = GamePhase.Ended;
            RpcOnGameOver(winner);
        }

        [ClientRpc]
        private void RpcOnGameOver(Player winner)
        {
            FindObjectOfType<GameplayScreenUI>().OnGameOver(winner);
        }

        public void InitPlayer(CENetworkRoomManager netManager, NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            players.Add(gamePlayer.GetComponent<Player>());
            if (players.Count == netManager.numPlayers)
                InitGame();
        }
    }
}
