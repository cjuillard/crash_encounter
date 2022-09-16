using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Runamuck
{
    public class Spawner : NetworkBehaviour
    {
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int TransitionID = Animator.StringToHash("Transition");

        [SerializeField] private GameObject pawnPrefab;
        [SerializeField] private int spawnRateInTicks = 50;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private float spawnOffset = 2;
        [SerializeField] private float spawnOffsetRange = .1f;  // Adds a little variation in the range so they aren't in a perfect line
        [SerializeField] private int maxWaveSize = 5;
        [SerializeField] private float offsetSize = 4;
        [SerializeField] private AudioClip transitionClip;

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
            
            // Init color
            Color newColor = owner != null ? owner.TeamColor : Color.gray;
            meshMat.SetColor(BaseColorID, newColor);
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
            animator.SetTrigger(TransitionID);
        }

        void OnOwnerChange(Player oldOwner, Player newOwner)
        {
            UpdateColor();
            GameAudio.Instance.PlayClip(transitionClip, transform.position);
        }

        public void StartAttack(Spawner other)
        {
            StartCoroutine(RunAttack(other));
        }

        private IEnumerator RunAttack(Spawner other)
        {
            int initialActiveCount = activeCount; // Grab the initial count
            Player initialOwner = Owner;

            Vector3 dir = (other.transform.position - transform.position).normalized;
            Vector3 orthoDir = new Vector3(-dir.z, dir.y, dir.x);

            for (int i = 0; i < initialActiveCount; i+= maxWaveSize)
            {
                if (activeCount == 0)   // This can happen if you try to send off multiple attacks from the same Spawner or this Spawner gets attacked
                    yield break;
                if (initialOwner != Owner)
                    yield break;

                // Get the wave size, constrained by both the initial active count and the current active count
                int waveSize = Math.Min(maxWaveSize, initialActiveCount - i);
                waveSize = Math.Min(waveSize, activeCount);

                Vector3 basePos = transform.position + dir * spawnOffset;
                for (int j = 0; j < waveSize; j++)
                {
                    Vector3 newPos = basePos + dir * (j % 2 == 0 ? spawnOffsetRange : 0);
                    Vector3 offset = Vector3.zero;
                    if(waveSize > 1)
                    {
                        offset = -(orthoDir * offsetSize * 0.5f) + orthoDir * offsetSize * (j / (float)(waveSize - 1));
                        newPos += offset;
                    }
                    GameObject go = Instantiate(pawnPrefab, newPos, Quaternion.FromToRotation(Vector3.forward, dir));
                    var pawn = go.GetComponent<Pawn>();
                    pawn.Init(Owner, other, offset);
                    NetworkServer.Spawn(go);
                }

                activeCount -= waveSize;

                yield return new WaitForSeconds(.5f);
            }
            yield return null;
        }

        public void Capture(Player player)
        {
            Owner = player;
            if (ActiveCount < 0)
                activeCount = 0;
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
