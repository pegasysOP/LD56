using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip planningPhaseClip;
    public AudioClip simulationPhaseClip;
    public AudioClip victoryFanfareClip;
    public AudioClip failureFanfareClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPlanningPhaseClip()
    {
        audioSource.Stop();
        audioSource.clip = planningPhaseClip;
        audioSource.Play();
    }

    public void PlaySimulationPhaseClip()
    {
        audioSource.Stop();
        audioSource.clip = simulationPhaseClip;
        audioSource.Play();
    }

    public void PlayVictoryFanfareClip()
    {
        audioSource.Stop();
        audioSource.clip = victoryFanfareClip;
        audioSource.Play();
    }

    public void PlayFailureFanfareClip()
    {
        audioSource.Stop();
        audioSource.clip = failureFanfareClip;
        audioSource.Play();
    }
}
