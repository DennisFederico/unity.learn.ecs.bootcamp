using UnityEngine;

public class Spawner : MonoBehaviour {
    // The set of targets and seekers are fixed, so rather than retrieve them every frame, we'll cache their transforms in these field.
    public static Transform[] targetTransforms;
    public static Transform[] seekerTransforms;

    [SerializeField] private GameObject seekerPrefab;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] public int numSeekers;
    [SerializeField] public int numTargets;
    [SerializeField] private Vector2 bounds;

    public void Start() {
        Random.InitState(123);

        seekerTransforms = new Transform[numSeekers];
        for (int i = 0; i < numSeekers; i++) {
            GameObject go = GameObject.Instantiate(seekerPrefab);
            Seeker seeker = go.GetComponent<Seeker>();
            Vector2 dir = Random.insideUnitCircle;
            seeker.direction = new Vector3(dir.x, 0, dir.y);
            seekerTransforms[i] = go.transform;
            go.transform.localPosition = new Vector3(Random.Range(0, bounds.x), 0, Random.Range(0, bounds.y));
        }

        targetTransforms = new Transform[numTargets];
        for (int i = 0; i < numTargets; i++) {
            GameObject go = GameObject.Instantiate(targetPrefab);
            Target target = go.GetComponent<Target>();
            Vector2 dir = Random.insideUnitCircle;
            target.direction = new Vector3(dir.x, 0, dir.y);
            targetTransforms[i] = go.transform;
            go.transform.localPosition = new Vector3(Random.Range(0, bounds.x), 0, Random.Range(0, bounds.y));
        }
    }
}