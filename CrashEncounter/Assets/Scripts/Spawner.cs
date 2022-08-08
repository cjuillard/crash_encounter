using Mirror;
using System;
using UnityEngine;

namespace Runamuck
{
    public class Spawner : NetworkBehaviour
    {
        [SerializeField] private GameObject pawn;
        [SerializeField] private int spawnRateInTicks = 50;
        
        private int activeCount = 5;
        public int ActiveCount => activeCount;
        private Player owner;
        public Player Owner => owner;
        
        private int ticksTillSpawn;
        
        void FixedUpdate()
        {
            if (owner == null)
                return;

            ticksTillSpawn--;
            if(ticksTillSpawn <= 0)
            {
                activeCount++;
                ticksTillSpawn = spawnRateInTicks;
            }
        }

        public void Capture(Player player)
        {
            owner = player;
        }

        public void GiveUpOwnership()
        {
            owner = null;
        }
    }
}
