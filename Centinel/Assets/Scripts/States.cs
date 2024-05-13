using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    public Estado state;
    private MeshRenderer mesh;
    public Material[] materials;

    public float reactivateStop = 2.5f;
    public float reactivateSleep = 2.5f;

    public float stopTime = 0f;
    public float sleepTime = 0f;
    public bool fromStop = false;
    public bool fromSleep = false;

    // Start is called before the first frame update
    void Start()
    {
        SetState(Estado.GARDA);
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state) {
            case Estado.GARDA:
                mesh.material = materials[0];
                fromStop = false;
                break;
            case Estado.PERSEGUIR:
                mesh.material = materials[1];
                break;
            case Estado.STOP:
                fromStop = false;
                mesh.material = materials[2];
                stopTime += Time.deltaTime;
                if (!fromSleep) {
                    StartCoroutine(sleep());
                    fromSleep = true;
                }
                if (stopTime >= reactivateStop) {
                    SetState(Estado.GARDA);
                    stopTime = 0f;
                    fromStop = true;
                    fromSleep = false;
                }
                break;
            case Estado.DURMIR:
                mesh.material = materials[3];
                sleepTime += Time.deltaTime;
                if (sleepTime >= reactivateSleep) {
                    SetState(Estado.STOP);
                    sleepTime = 0f;
                    fromSleep = true;
                }
                break;
            default:
                break;
        }


    }

    public void SetState(Estado nuevoEstado) {
        state = nuevoEstado;
    }

    private IEnumerator sleep() {
        while (state == Estado.STOP) {
            int rand = Random.Range(0,2);
            if (rand == 0) {
                SetState(Estado.DURMIR);
            }
            yield return new WaitForSeconds(1);
        }
    }
}

public enum Estado {
        GARDA,
        PERSEGUIR,
        STOP,
        DURMIR
    }