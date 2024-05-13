using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PatrolRotate : MonoBehaviour {
    public Transform point;
    private NavMeshAgent agent;
    public Transform goal;

    public float speed;
    public bool rotate;
    private RaycastHit hit;
    private bool canStop;

    private States estados;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        estados = GetComponent<States>();
        rotate = true;
        canStop = true;   

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;
    }

    void ReturnToPosition()
    {
        // Set the agent to go to the currently selected destination.
        agent.destination = point.position;
    }

    void Update()
    {
        if (estados.state == Estado.GARDA) {
        // Choose the next destination point when the agent gets
        // close to the current one.
            if ((!agent.pathPending && agent.remainingDistance < 0.5f) || !rotate) {
                ReturnToPosition();
            }
        } else if (estados.state == Estado.PERSEGUIR) {
            agent.destination = goal.position;
            rotate = false;
        } else if (estados.state == Estado.STOP || estados.state == Estado.DURMIR) {
            agent.destination = transform.position;
            rotate = false;
        }

        if (estados.state == Estado.GARDA && agent.remainingDistance < 0.5f) {
            rotate = true;
        }

        if (rotate) {
            rotatingPratol();
            if (canStop && estados.state == Estado.GARDA) {
                StartCoroutine(stop());
            }
        }
    }

    void rotatingPratol() {
        transform.Rotate(transform.up * -1, speed * Time.deltaTime);
    }

    IEnumerator stop() {
        canStop = false;
        while (estados.state == Estado.GARDA) {
            int rand = Random.Range(0,10);
            Debug.Log(rand);
            if (rand == 0) {
                estados.SetState(Estado.STOP);
            }
            yield return new WaitForSeconds(1f);
        }
        canStop = true;
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Agent") && (estados.state != Estado.DURMIR )) {
            ShootRay();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Agent") && estados.state == Estado.PERSEGUIR) {
            estados.SetState(Estado.STOP);
            estados.fromSleep = true;
        }
    }

    void ShootRay()
	{
        Vector3 targetDir = goal.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
		
		if (angle < 30f) {
            if (Physics.Raycast(transform.position, goal.position - transform.position, out hit)) {
                if (hit.transform.tag == "Agent") {
                    estados.SetState(Estado.PERSEGUIR);
                } else if (estados.state == Estado.PERSEGUIR){
                    estados.SetState(Estado.STOP);
                    estados.fromSleep = true;
                }
            }
        }
	}

    void OnDrawGizmos(){

            // Draw the field of view
            Gizmos.color = Color.red;
            
            Vector3 directionToTarget = goal.transform.position - transform.position;
            Vector3 viewAngleA = DirFromAngle(-60f / 2, false);
            Vector3 viewAngleB = DirFromAngle(60f / 2, false);

            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * 25f);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * 25f);
    }

    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}