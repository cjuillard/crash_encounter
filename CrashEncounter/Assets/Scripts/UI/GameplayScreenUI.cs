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

        private Camera camera;

        private void Start()
        {
            camera = FindObjectOfType<Camera>();
        }

        public void SetAttackArrowEnabled(Transform startPos)
        {
            attackArrow.gameObject.SetActive(true);

            Vector3 screenPos = camera.WorldToViewportPoint(startPos.position);
            attackArrow.rectTransform.anchorMin = screenPos;
            attackArrow.rectTransform.anchorMax = screenPos;

            Vector2 offsetMax = attackArrow.rectTransform.offsetMax;
            offsetMax.x = 0;
            attackArrow.rectTransform.offsetMax = offsetMax;
        }

        public void SetAttackArrowDisabled()
        {
            attackArrow.gameObject.SetActive(false);
        }

        public void SetArrowTarget(Vector3 mousePosition)
        {
            Vector3 attackArrowStart = attackArrow.transform.position;            
            Vector2 delta = new Vector2(mousePosition.x, mousePosition.y) - new Vector2(attackArrowStart.x, attackArrowStart.y);

            float zRot = Vector2.SignedAngle(Vector2.right, delta);
            attackArrow.transform.rotation = Quaternion.Euler(0, 0, zRot);

            float dist = delta.magnitude;

            Vector2 offsetMax = attackArrow.rectTransform.offsetMax;
            offsetMax.x = dist * canvas.scaleFactor;
            attackArrow.rectTransform.offsetMax = offsetMax;
        }
    }
}
