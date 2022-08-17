using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class Pawn : NetworkBehaviour
    {
        [SerializeField] private float speed = 1;

        [SyncVar] [SerializeField]
        private Player owner;

        [SyncVar] [SerializeField]
        private Spawner target;

        public void Init(Player owner, Spawner target)
        {
            this.owner = owner;
            this.target = target;
        }

        private void FixedUpdate()
        {
            Vector3 delta = target.transform.position - transform.position;
            float moveStep = speed * Time.fixedDeltaTime;
            if(delta.sqrMagnitude < moveStep * moveStep)
            {
                // TODO do attack
                NetworkServer.Destroy(gameObject);
                return;
            }

            transform.position += moveStep * delta.normalized;
        }
    }
}
