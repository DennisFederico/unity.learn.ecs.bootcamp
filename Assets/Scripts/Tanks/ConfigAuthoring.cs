using Unity.Entities;
using UnityEngine;

namespace Tanks {
    public class ConfigAuthoring : MonoBehaviour {
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private GameObject cannonBallPrefab;
        [SerializeField] private int tankCount;

        private class ConfigAuthoringBaker : Baker<ConfigAuthoring> {
            public override void Bake(ConfigAuthoring authoring) {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent(entity, new Config {
                    // Bake the prefab into entities. GetEntity will return the root entity of the prefab hierarchy.
                    TankPrefab = GetEntity(authoring.tankPrefab, TransformUsageFlags.Dynamic),
                    CannonBallPrefab = GetEntity(authoring.cannonBallPrefab, TransformUsageFlags.Dynamic),
                    TankCount = authoring.tankCount
                });
            }
        }
    }

    public struct Config : IComponentData {
        public Entity TankPrefab;
        public Entity CannonBallPrefab;
        public int TankCount;
    }
}