using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Tanks {
    public partial struct TankSpawnSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Enabled = false;
            
            var config = SystemAPI.GetSingleton<Config>();

            // Random numbers from a hard-coded seed.
            var random = new Random(123);
            
            for (var i = 0; i < config.TankCount; i++) {
                var tankEntity = state.EntityManager.Instantiate(config.TankPrefab);
                if (i==0) state.EntityManager.AddComponent<PlayerTank>(tankEntity);
                var randomBaseColor = new URPMaterialPropertyBaseColor() {
                    Value = RandomColor(ref random)
                };
                // Every root entity instantiated from a prefab has a LinkedEntityGroup component,
                // which is a list of all the entities that make up the prefab hierarchy (*including the root*).
                
                var linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(tankEntity);
                foreach (var entity in linkedEntities) {
                    // We want to set the URPMaterialPropertyBaseColor component only on the entities that have it, so we first check.
                    if (state.EntityManager.HasComponent<URPMaterialPropertyBaseColor>(entity.Value)) {
                        // Set the color of each entity that makes up the tank.
                        state.EntityManager.SetComponentData(entity.Value, randomBaseColor);
                    }
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
        
        // Return a random color that is visually distinct. (Naive randomness would produce a distribution of colors clustered 
        // around a narrow range of hues. See https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/ )
        private static float4 RandomColor(ref Random random) {
            // 0.618034005f is inverse of the golden ratio
            var hue = (random.NextFloat() + 0.618034005f) % 1;
            return (Vector4) Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }
}