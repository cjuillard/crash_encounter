using Mirror;
using System;
using UnityEngine;

namespace Runamuck
{
    public class Spawner : NetworkBehaviour
    {
        [SerializeField] private GameObject pawn;
        [SerializeField] private int spawnRateInTicks = 50;

        [SyncVar] [SerializeField]
        private int activeCount = 5;
        public int ActiveCount => activeCount;
        
        [SyncVar] [SerializeField] private Player owner;
        public Player Owner => owner;
        
        private int ticksTillSpawn;
        
        private GameplayScreenUI screen;

        private void Start()
        {
            this.screen = FindObjectOfType<GameplayScreenUI>();
        }

        void FixedUpdate()
        {
            if (!isServer)
                return;

            if (owner == null)
                return;

            ticksTillSpawn--;
            if(ticksTillSpawn <= 0)
            {
                activeCount++;
                ticksTillSpawn = spawnRateInTicks;
            }
        }

        [Command]
        void CmdAttackSpawner(Spawner other)
        {
            //GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
            //NetworkServer.Spawn(projectile);
            //RpcOnFire();
        }

        //// this is called on the tank that fired for all observers
        //[ClientRpc]
        //void RpcOnFire()
        //{
        //    animator.SetTrigger("Shoot");
        //}

        public void Capture(Player player)
        {
            owner = player;
        }

        public void GiveUpOwnership()
        {
            owner = null;
        }

        public void OnMouseDown()
        {
            screen.SetAttackArrowEnabled(transform);
        }

        public void OnMouseDrag()
        {
            screen.SetArrowTarget(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            screen.SetAttackArrowDisabled();
        }
    }
}
