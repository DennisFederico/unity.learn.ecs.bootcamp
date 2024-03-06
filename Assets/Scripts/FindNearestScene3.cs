using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FindNearestScene3 : MonoBehaviour {
    // The size of our arrays does not need to vary, so rather than create new arrays every field,
    // we'll create the arrays in Awake() and store them in these fields.
    private NativeArray<float3> _targetPositions;
    private NativeArray<float3> _seekerPositions;
    private NativeArray<float3> _nearestTargetPositions;

    public void Start() {
        Spawner spawner = FindFirstObjectByType<Spawner>();
        // We use the Persistent allocator because these arrays must exist for the run of the program.
        _targetPositions = new NativeArray<float3>(spawner.numTargets, Allocator.Persistent);
        _seekerPositions = new NativeArray<float3>(spawner.numSeekers, Allocator.Persistent);
        _nearestTargetPositions = new NativeArray<float3>(spawner.numSeekers, Allocator.Persistent);
    }

    // We are responsible for disposing of our allocations
    // when we no longer need them.
    public void OnDestroy() {
        _targetPositions.Dispose();
        _seekerPositions.Dispose();
        _nearestTargetPositions.Dispose();
    }

    public void Update() {
        // Copy every target transform to a NativeArray.
        for (int i = 0; i < _targetPositions.Length; i++) {
            // Vector3 is implicitly converted to float3
            _targetPositions[i] = Spawner.targetTransforms[i].localPosition;
        }

        // Copy every seeker transform to a NativeArray.
        for (int i = 0; i < _seekerPositions.Length; i++) {
            // Vector3 is implicitly converted to float3
            _seekerPositions[i] = Spawner.seekerTransforms[i].localPosition;
        }

        // To schedule a job, we first need to create an instance and populate its fields.
        FindNearestParallelJob findJob = new FindNearestParallelJob {
            targetPositions = _targetPositions,
            seekerPositions = _seekerPositions,
            nearestTargetPositions = _nearestTargetPositions,
        };

        // This job processes every seeker, so the seeker array length is used as the index count.
        // A batch size of 100 is semi-arbitrarily chosen here simply because it's not too big but not too small.
        JobHandle findHandle = findJob.Schedule(_seekerPositions.Length, 500);

        // The Complete method will not return until the job represented by the handle finishes execution.
        // Effectively, the main thread waits here until the job is done.
        findHandle.Complete();

        // Draw a debug line from each seeker to its nearest target.
        for (int i = 0; i < _seekerPositions.Length; i++) {
            // float3 is implicitly converted to Vector3
            Debug.DrawLine(_seekerPositions[i], _nearestTargetPositions[i]);
        }
    }
}