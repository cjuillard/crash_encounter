using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class Pawn : NetworkBehaviour
    {
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

        [SerializeField] private float speed = 1;
        [SerializeField] private MeshRenderer meshRenderer;

        [SyncVar] [SerializeField]
        private Player owner;
        public Player Owner => owner;

        [SyncVar] [SerializeField]
        private Spawner target;
        
        private Material meshMat;

        private void Awake()
        {
            this.meshMat = meshRenderer.material;
        }

        public void Init(Player owner, Spawner target)
        {
            this.owner = owner;
            this.target = target;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            UpdateColor();
        }

        private void UpdateColor()
        {
            Color newColor = owner != null ? owner.TeamColor : Color.gray;
            meshMat.SetColor(BaseColorID, newColor);
        }

        private void FixedUpdate()
        {
            if (!isServer)
                return;

            Vector3 delta = target.transform.position - transform.position;
            float moveStep = speed * Time.fixedDeltaTime;
            if(delta.sqrMagnitude < moveStep * moveStep)
            {
                if(target.Owner == owner)
                {
                    target.IncrementActiveCount();
                } else
                {
                    target.DecrementActiveCount();
                    if (target.ActiveCount == 0)
                        target.Capture(owner);
                }
                NetworkServer.Destroy(gameObject);
                return;
            }

            transform.position += moveStep * delta.normalized;
        }
    }
}
