using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Ecs {
    public partial struct SpawnerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Spawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Enabled = false;
            
            var prefab = SystemAPI.GetSingleton<Spawner>().cubePrefab;
            var instances = state.EntityManager.Instantiate(prefab, 10, Allocator.Temp);

            var random = new Random(123);
            foreach (var instance in instances) {
                var localTransform = SystemAPI.GetComponentRW<LocalTransform>(instance);
                localTransform.ValueRW.Position = random.NextFloat3(new float3(10, 10, 10));
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}