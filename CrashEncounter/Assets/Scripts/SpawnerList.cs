using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class SpawnerList : MonoBehaviour
    {
        [SerializeField] private List<Spawner> spawners;
        public List<Spawner> Spawners => spawners;
    }
}
