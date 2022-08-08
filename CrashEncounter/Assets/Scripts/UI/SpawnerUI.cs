using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runamuck
{
    public class SpawnerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Spawner spawner;

        void Update()
        {
            text.text = spawner.ActiveCount.ToString();
        }
    }
}
