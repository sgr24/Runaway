using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBlockTrigger : MonoBehaviour
{
    public Timer timerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timerScript.StopTimer();
            Debug.Log("Timer stopped on block!");
        }
    }
}
