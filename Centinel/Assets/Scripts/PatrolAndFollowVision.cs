using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class PatrolAndFollowVision : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    public Transform goal;
    private bool reset;
    private RaycastHit hit;
    private States estados;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        estados = GetComponent<States>();
        reset = false;

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0) {
            return;
        }

        if (reset) {
            destPoint = 0;
            reset = false;
        }

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;        
    }


    void Update()
    {
        if (estados.state == Estado.GARDA) {
        // Choose the next destination point when the agent gets
        // close to the current one.
            if ((!agent.pathPending && agent.remainingDistance < 0.5f) || reset) {
                if (!estados.fromStop) {
                    estados.SetState(Estado.STOP);
                } else {
                    GotoNextPoint();
                }
            }
        } else if (estados.state == Estado.PERSEGUIR) {
            agent.destination = goal.position;
        } else if (estados.state == Estado.STOP) {
            agent.destination = transform.position;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Agent") && (estados.state != Estado.DURMIR )) {
            if (Physics.Raycast(transform.position, goal.position - transform.position, out hit)) {
                if (hit.transform.tag == "Agent") {
                    estados.SetState(Estado.PERSEGUIR);
                } else {
                    if (estados.state == Estado.PERSEGUIR) {
                        estados.SetState(Estado.STOP);
                        estados.fromSleep = true;
                        reset = true;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Agent") && estados.state == Estado.PERSEGUIR) {
            estados.SetState(Estado.STOP);
            estados.fromSleep = true;
            reset = true;
        }
    }
}