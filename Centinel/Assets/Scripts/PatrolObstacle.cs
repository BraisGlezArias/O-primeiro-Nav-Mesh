using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class PatrolObstacle : MonoBehaviour
{
    private float elapsedTime;
    public float amplitude;
    public float period;

    void Start()
    {
        elapsedTime = period / 4;
    }


    void Update() {
        elapsedTime += Time.deltaTime;
        Vector3 newPosition = transform.position;
        newPosition.z = CalculatePosition();
        transform.position = newPosition;  
    }

    private float CalculatePosition() {
        float pinPon = Mathf.PingPong(elapsedTime * 2 / period, 1);

        float smoothStep = Mathf.SmoothStep(0, 1, pinPon);

        return (smoothStep-0.5f)*amplitude;
    }
}
