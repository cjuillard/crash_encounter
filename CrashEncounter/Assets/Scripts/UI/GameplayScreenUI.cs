using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runamuck
{
    public class GameplayScreenUI : MonoBehaviour
    {
        [SerializeField] private Image attackArrow;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject loseWindow;
        [SerializeField] private GameObject winWindow;

        private Camera mainCam;

        private Player player;
        private Spawner startSpawner;

        private void Start()
        {
            mainCam = FindObjectOfType<Camera>();
        }

        public void OnGameStart()
        {
            foreach(Player player in FindObjectsOfType<Player>())
            {
                if (player.isLocalPlayer)
                    this.player = player;
            }
        }

        public void SetAttackArrowEnabled(Spawner startSpawner)
        {
            this.startSpawner = startSpawner;
            attackArrow.gameObject.SetActive(true);

            Vector3 screenPos = mainCam.WorldToViewportPoint(startSpawner.transform.position);
            attackArrow.rectTransform.anchorMin = screenPos;
            attackArrow.rectTransform.anchorMax = screenPos;

            Vector2 offsetMax = attackArrow.rectTransform.offsetMax;
            offsetMax.x = 0;
            attackArrow.rectTransform.offsetMax = offsetMax;
        }

        public void SetAttackArrowDisabled()
        {
            attackArrow.gameObject.SetActive(false);
            startSpawner = null;
        }

        private void Update()
        {
            
        }

        public void SetArrowTarget(Vector3 mousePosition)
        {
            if (startSpawner == null)
                return;
            
            Vector3 attackArrowStart = attackArrow.transform.position;            
            Vector2 delta = new Vector2(mousePosition.x, mousePosition.y) - new Vector2(attackArrowStart.x, attackArrowStart.y);

            float zRot = Vector2.SignedAngle(Vector2.right, delta);
            attackArrow.transform.rotation = Quaternion.Euler(0, 0, zRot);

            float dist = delta.magnitude;

            Vector2 offsetMax = attackArrow.rectTransform.offsetMax;
            offsetMax.x = dist / canvas.scaleFactor;
            attackArrow.rectTransform.offsetMax = offsetMax;
        }

        private RaycastHit[] results = new RaycastHit[16];

        public bool GetSpawnerAtMousePos(Vector3 mousePosition, out Spawner spawner)
        {
            spawner = null;
            Ray ray = mainCam.ScreenPointToRay(mousePosition);
            
            int hitCount = Physics.RaycastNonAlloc(ray, results);
            if (hitCount == 0)
                return false;

            for(int i = 0; i < hitCount; i++)
            {
                var result = results[i];
                var hitSpawner = result.collider.GetComponentInParent<Spawner>();
                if(hitSpawner != null)
                {
                    spawner = hitSpawner;
                    return true;
                }
            }

            return false;
        }

        public void OnGameOver(Player winner)
        {
            if(player == winner)
            {
                winWindow.SetActive(true);
            } else
            {
                loseWindow.SetActive(true);
            }
        }
    }
}
