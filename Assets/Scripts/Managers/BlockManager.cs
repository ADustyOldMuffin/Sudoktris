using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Managers
{
    public class BlockManager : SingletonBehavior<BlockManager>
    {
        public List<GameObject> formations;

        private List<Transform> _spawnLocations;

        public GameObject GetRandomBlock()
        {
            return formations[Random.Range(0, formations.Count)];
        }

        public void SetSpawnLocations(List<Transform> positions)
        {
            _spawnLocations = positions;
        }

        public List<BlockFormation> SpawnBlocks()
        {
            Debug.Log("Spawn blocks called");
            
            var results = new List<BlockFormation>();
            foreach (Transform pos in _spawnLocations)
            {
                
                GameObject toSpawn = GetRandomBlock();
                var block = Instantiate(toSpawn).GetComponent<BlockFormation>();
                block.transform.position = block.SpawnLocation = pos.position;
                results.Add(block);
            }

            return results;
        }
    }
}