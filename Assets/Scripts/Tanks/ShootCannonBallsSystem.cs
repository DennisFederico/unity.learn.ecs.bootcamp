using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Tanks {

    //To avoid rendering the cannon balls at the origin for a single frame.
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct ShootCannonBallsSystem : ISystem {
        
        private float _timer;
 
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            // Only shoot in frames where timer has expired
            _timer -= SystemAPI.Time.DeltaTime;
            if (_timer > 0) {
                return; 
            } 
            _timer = 0.3f;   // reset timer

            var config = SystemAPI.GetSingleton<Config>();
            var ballTransform = state.EntityManager.GetComponentData<LocalTransform>(config.CannonBallPrefab);
        
            // For each turret of every tank, spawn a cannonball and set its initial velocity
            foreach (var (tank, transform, color) in
                     SystemAPI.Query<RefRO<Tank>, RefRO<LocalToWorld>, RefRO<URPMaterialPropertyBaseColor>>()) {
                
                var cannonBallEntity = state.EntityManager.Instantiate(config.CannonBallPrefab);
            
                // Set color of the cannonball to match the tank that shot it.
                state.EntityManager.SetComponentData(cannonBallEntity, color.ValueRO);
            
                // We need the transform of the cannon in world space, so we get its LocalToWorld instead of LocalTransform.
                var cannonTransform = state.EntityManager.GetComponentData<LocalToWorld>(tank.ValueRO.Cannon);
                ballTransform.Position =  cannonTransform.Position;
            
                // Set position of the new cannonball to match the spawn point
                state.EntityManager.SetComponentData(cannonBallEntity, ballTransform);

                // Set velocity of the cannonball to shoot out of the cannon.
                state.EntityManager.SetComponentData(cannonBallEntity, new CannonBallAuthoring.CannonBall {
                    Velocity = math.normalize(cannonTransform.Up) * 12.0f
                });
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}