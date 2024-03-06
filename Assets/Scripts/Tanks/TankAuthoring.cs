using Unity.Entities;
using UnityEngine;

namespace Tanks {
    public class TankAuthoring : MonoBehaviour {
        
        [SerializeField] private GameObject turret;
        [SerializeField] private GameObject cannon;
        
        private class TankAuthoringBaker : Baker<TankAuthoring> {
            public override void Bake(TankAuthoring authoring) {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new Tank {
                    Turret = GetEntity(authoring.turret, TransformUsageFlags.Dynamic),
                    Cannon = GetEntity(authoring.cannon, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
    
    public struct Tank : IComponentData {
        public Entity Turret;
        public Entity Cannon;
    }
}