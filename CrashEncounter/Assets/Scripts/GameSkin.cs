using System;
using UnityEngine;

namespace Runamuck
{
    [CreateAssetMenu(fileName = "GameSkin", menuName = "ScriptableObjects/GameSkin", order = 1)]
    public class GameSkin : ScriptableObject
    {
        public GameObject[] PawnVisuals;
        public GameObject PawnImpactFX;

        [Tooltip("This is the amount of team color to tint our impact particles")]
        public float ImpactTeamColorLerp = 0.5f;

        internal GameObject GetPawn(int playerIndex)
        {
            return PawnVisuals[playerIndex % PawnVisuals.Length];
        }
    }
}