using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject pawnPrefab;
        
        [SyncVar] [SerializeField] private int playerIndex;
        [SyncVar] [SerializeField] private Color teamColor;
        public Color TeamColor => teamColor;

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

        //[Command]
        //void CmdFire()
        //{
        //    GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
        //    NetworkServer.Spawn(projectile);
        //    RpcOnFire();
        //}
    }
}
