using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class Player : NetworkBehaviour
    {
        [SyncVar] [SerializeField] private int playerIndex;
        [SyncVar] [SerializeField] private Color teamColor;
        public Color TeamColor => teamColor;

        [SyncVar] [SerializeField] private bool isAlive;
        public bool IsAlive => isAlive;

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public void Init(int playerIndex)
        {
            this.playerIndex = playerIndex;

            this.teamColor = Game.Instance.Config.TeamColors[playerIndex % Game.Instance.Config.TeamColors.Length];
        }
        
        [Command]
        public void CmdStartAttack(Spawner spawner, Spawner other)
        {
            if (spawner.Owner != this)
                return;

            spawner.StartAttack(other);
        }

        public void SetIsAlive(bool isAlive)
        {
            if (this.isAlive == isAlive)
                return;

            this.isAlive = isAlive;
        }

        //[Command]
        //void CmdFire()
        //{
        //    GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
        //    NetworkServer.Spawn(projectile);
        //    RpcOnFire();
        //}
    }
}
