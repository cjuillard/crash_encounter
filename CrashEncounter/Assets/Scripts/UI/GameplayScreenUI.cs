using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runamuck
{
    public class GameplayScreenUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject loseWindow;
        [SerializeField] private GameObject winWindow;
        [SerializeField] private AttackArrow attackArrow;
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

            Vector3 mousePosition = Input.mousePosition;    // TODO update for mobile
            var targetWorldPos = mainCam.ScreenToWorldPoint(mousePosition);
            targetWorldPos.y = startSpawner.transform.position.y;
            attackArrow.Show(startSpawner.transform.position, targetWorldPos);
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

            // Our world lives in the x/z plane
            var targetWorldPos = mainCam.ScreenToWorldPoint(mousePosition);
            targetWorldPos.y = startSpawner.transform.position.y;
            attackArrow.Show(startSpawner.transform.position, targetWorldPos);
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
