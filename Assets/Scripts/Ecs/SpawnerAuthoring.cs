using Unity.Entities;
using UnityEngine;

namespace Ecs {
    public class SpawnerAuthoring : MonoBehaviour {
        
        [SerializeField] private GameObject cubePrefab;
        private class SpawnerAuthoringBaker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner {
                    cubePrefab = GetEntity(authoring.cubePrefab, TransformUsageFlags.None)
                });
            }
        }
    }
    
    public struct Spawner : IComponentData {
        public Entity cubePrefab;
    }
}