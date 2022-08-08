using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject pawnPrefab;
        
        void Update()
        {
            // only let the local player control the racket.
            // don't control other player's rackets
            //if (isLocalPlayer)
            //    rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;

        }
    }
}
