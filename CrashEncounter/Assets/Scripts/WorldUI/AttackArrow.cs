using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class AttackArrow : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float sourceAndTargetOffset = 3;
        [SerializeField] private float controlPointVerticalOffset = 5;
        [SerializeField] private SpriteRenderer arrowHead;

        private Vector3[] linePositions = new Vector3[32];
        
        public Vector2 testVector = new Vector2(1, 0);

        public void Show(Vector3 source, Vector3 target)
        {
            gameObject.SetActive(true);

            Vector3 offsetSource = source + new Vector3(0, sourceAndTargetOffset, 0);
            Vector3 offsetTarget = target + new Vector3(0, sourceAndTargetOffset, 0);
            Vector3 p1 = (offsetSource + offsetTarget) * 0.5f + new Vector3(0, controlPointVerticalOffset, 0);
            for(int i = 0; i < linePositions.Length; i++)
            {
                linePositions[i] = GetBezierPoint(offsetSource, p1, offsetTarget, i / (float)(linePositions.Length - 1));
            }
            lineRenderer.positionCount = linePositions.Length;
            lineRenderer.SetPositions(linePositions);

            // Update arrow head and dir
            Vector2 dir = new Vector2(target.x - source.x, target.z - source.z).normalized;

            arrowHead.transform.position = offsetTarget;
            arrowHead.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y), Vector3.up);
            
        }


        public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }
    }
}
