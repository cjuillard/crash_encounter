using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Runamuck
{
    public class Spawner : NetworkBehaviour
    {
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

        [SerializeField] private GameObject pawnPrefab;
        [SerializeField] private int spawnRateInTicks = 50;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float spawnOffset = 2;
        [SerializeField] int maxWaveSize = 5;
        [SerializeField] float offsetSize = 4;

        [SyncVar] [SerializeField]
        private int activeCount = 5;
        public int ActiveCount => activeCount;
        
        [SyncVar(hook = nameof(OnOwnerChange))] [SerializeField] private Player owner;
        public Player Owner
        {
            get => owner;
            set
            {
                owner = value;
                
            }
        }
        
        
        private GameplayScreenUI screen;
        private Material meshMat;
        private int ticksTillSpawn;

        private void Awake()
        {
            this.screen = FindObjectOfType<GameplayScreenUI>();
            this.meshMat = meshRenderer.material;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            UpdateColor();
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

        public void IncrementActiveCount()
        {
            activeCount++;
        }

        public void DecrementActiveCount()
        {
            activeCount--;
        }

        private void UpdateColor()
        {
            Color newColor = owner != null ? owner.TeamColor : Color.gray;
            meshMat.SetColor(BaseColorID, newColor);
        }

        void OnOwnerChange(Player oldOwner, Player newOwner)
        {
            UpdateColor();
        }

        public void StartAttack(Spawner other)
        {
            StartCoroutine(RunAttack(other));
            activeCount = 0;
        }

        private IEnumerator RunAttack(Spawner other)
        {
            int maxAttackers = activeCount; // Grab the initial count
            activeCount = 0;

            Vector3 dir = (other.transform.position - transform.position).normalized;
            Vector3 orthoDir = new Vector3(-dir.z, dir.y, dir.x);

            for (int i = 0; i < maxAttackers; i+= maxWaveSize)
            {
                int waveSize = Math.Min(maxWaveSize, maxAttackers - i);
                Vector3 basePos = transform.position + dir * spawnOffset;

                for (int j = 0; j < waveSize; j++)
                {
                    Vector3 newPos = basePos;
                    if(waveSize > 1)
                    {
                        newPos += -(orthoDir * offsetSize * 0.5f) + orthoDir * offsetSize * (j / (float)(waveSize - 1));
                    }
                    GameObject go = Instantiate(pawnPrefab, newPos, Quaternion.identity);
                    var pawn = go.GetComponent<Pawn>();
                    pawn.Init(Owner, other);
                    NetworkServer.Spawn(go);
                }

                yield return new WaitForSeconds(.5f);
            }
            yield return null;
        }

        //// this is called on the tank that fired for all observers
        //[ClientRpc]
        //void RpcOnFire()
        //{
        //    animator.SetTrigger("Shoot");
        //}

        public void Capture(Player player)
        {
            Owner = player;
        }

        public void GiveUpOwnership()
        {
            Owner = null;
        }

        public void OnMouseDown()
        {
            if(Owner != null && Owner.isLocalPlayer)
            {
                screen.SetAttackArrowEnabled(this);
            }
        }

        public void OnMouseDrag()
        {
            screen.SetArrowTarget(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            screen.SetAttackArrowDisabled();

            if (screen.GetSpawnerAtMousePos(Input.mousePosition, out Spawner other))
            {
                if(Owner != null)
                {
                    Owner.CmdStartAttack(this, other);
                }
            }
        }
    }
}
