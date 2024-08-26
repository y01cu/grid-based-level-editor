using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public Transform[] spheres; // Assign your spheres in the inspector
    public Vector3 destination = new Vector3(2, 0, 0);
    public float speed = 1.0f;

    private bool[] sphereIsMoving; // Track which spheres are moving

    void Start()
    {
        sphereIsMoving = new bool[spheres.Length];
        sphereIsMoving[spheres.Length - 1] = true; // Start with the furthest sphere moving
    }

    void Update()
    {
        MoveSpheres();
    }

    void MoveSpheres()
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            if (sphereIsMoving[i])
            {
                // Calculate the target position for the current sphere
                Vector3 targetPosition = (i == 0) ? destination : spheres[i - 1].position;

                // Move the current sphere towards the target position
                spheres[i].position = Vector3.MoveTowards(spheres[i].position, targetPosition, speed * Time.deltaTime);

                // If the current sphere has reached the next one, allow the next sphere to start moving
                if (i > 0 && !sphereIsMoving[i - 1] && Vector3.Distance(spheres[i].position, spheres[i - 1].position) < 1f)
                {
                    sphereIsMoving[i - 1] = true;
                }
            }
        }
    }
}
