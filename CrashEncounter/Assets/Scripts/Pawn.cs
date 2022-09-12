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
        [SerializeField] private AudioClip spawnClip;
        [SerializeField] private AudioClip impactClip;

        [SyncVar] [SerializeField]
        private Player owner;
        public Player Owner => owner;

        [SyncVar] [SerializeField]
        private Spawner target;

        [SyncVar] [SerializeField] private Vector3 targetOffset;
        public Vector3 TargetOffset => targetOffset;

        private Material meshMat;

        private void Awake()
        {
            this.meshMat = meshRenderer.material;
        }

        public void Init(Player owner, Spawner target, Vector3 targetOffset)
        {
            this.owner = owner;
            this.target = target;
            this.targetOffset = targetOffset;

            GameAudio.Instance.PlayClip(spawnClip, transform.position);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            UpdateColor();

            GameObject visualPrefab = Game.Skin.GetPawn(owner.PlayerIndex);
            Instantiate(visualPrefab, transform);
        }

        private void UpdateColor()
        {
            Color newColor = owner != null ? owner.TeamColor : Color.gray;
            meshMat.SetColor(BaseColorID, newColor);
        }

        [ClientRpc]
        public void RpcSpawnImpact(Vector3 impactPos)
        {
            GameAudio.Instance.PlayClip(impactClip, transform.position);
            var impactFX = Instantiate(Game.Skin.PawnImpactFX, impactPos, Quaternion.identity);
            foreach(ParticleSystem system in impactFX.GetComponentsInChildren<ParticleSystem>())
            {
                var mainProps = system.main;
                mainProps.startColor = Color.Lerp(mainProps.startColor.color, owner.TeamColor, Game.Skin.ImpactTeamColorLerp);
            }
        }

        private void FixedUpdate()
        {
            if (!isServer || isDying)
                return;

            Vector3 delta = (target.transform.position + targetOffset) - transform.position;
            float moveStep = speed * Time.fixedDeltaTime;
            if(delta.sqrMagnitude < moveStep * moveStep)
            {
                transform.position = target.transform.position;
                return;
            }

            Vector3 dir = delta.normalized;
            transform.SetPositionAndRotation(transform.position + moveStep * dir, Quaternion.FromToRotation(Vector3.forward, dir));
            transform.position += moveStep * delta.normalized;
        }

        private bool isDying = false;
        public IEnumerator DelayedDestroy()
        {
            if (isDying)
                yield break;

            isDying = true;
            yield return null;
            NetworkServer.Destroy(gameObject);
        }

        public void ServerDestroy()
        {
            RpcSpawnImpact(transform.position);
            StartCoroutine(DelayedDestroy());
        }

        void OnTriggerEnter(Collider other)
        {
            if (!isServer)
                return;

            if (other.gameObject.CompareTag("Pawn"))
            {
                Pawn otherPawn = other.GetComponent<Pawn>();
                if (otherPawn.Owner != owner)
                {
                    ServerDestroy();
                    otherPawn.ServerDestroy();
                }
            }
            else if (other.gameObject.CompareTag("Spawner"))
            {
                Spawner otherSpawner = other.GetComponent<Spawner>();
                if (otherSpawner == target)
                {
                    AttackTarget();
                }
            }
        }

        private void AttackTarget()
        {
            if (target.Owner == owner)
            {
                target.IncrementActiveCount();
            }
            else
            {
                target.DecrementActiveCount();
                if (target.ActiveCount == 0)
                    target.Capture(owner);
            }
            ServerDestroy();
        }
    }
}
