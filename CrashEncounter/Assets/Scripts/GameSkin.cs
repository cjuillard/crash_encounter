using System;
using UnityEngine;

namespace Runamuck
{
    [CreateAssetMenu(fileName = "GameSkin", menuName = "ScriptableObjects/GameSkin", order = 1)]
    public class GameSkin : ScriptableObject
    {
        public GameObject[] PawnVisuals;
        public GameObject PawnImpactFX;

        internal GameObject GetPawn(int playerIndex)
        {
            return PawnVisuals[playerIndex % PawnVisuals.Length];
        }
    }
}