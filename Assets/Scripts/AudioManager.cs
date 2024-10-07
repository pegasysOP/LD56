using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Phase Music")]
    public AudioClip planningPhaseClip;
    public AudioClip simulationPhaseClip;
    public AudioClip victoryFanfareClip;
    public AudioClip roundVictoryClip;
    public AudioClip failureFanfareClip;

    [Header("Interaction Sounds")]
    public AudioClip pickupClip;
    public AudioClip putdownClip;
    public AudioClip invalidPlaceClip;
    public AudioClip damangeClip;
    public AudioClip deathClip;
    public AudioClip movementClip;

    [Header("Special Attacks")]
    public AudioClip specialAttackClip;
    public AudioClip beeSpecialAttackClip;
    public AudioClip queenBeeSpecialAttackClip;
    public AudioClip beetleSpecialAttackClip;
    public AudioClip spiderSpecialAttackClip;
    public AudioClip mothSpecialAttackClip;
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
        audioSource.clip = planningPhaseClip;
        audioSource.Play();
    }

    public void PlaySimulationPhaseClip()
    {
        audioSource.clip = simulationPhaseClip;
        audioSource.Play();
    }

    public void PlayVictoryFanfareClip()
    {
        audioSource.clip = victoryFanfareClip;
        audioSource.Play();
    }

    public void PlayRoundVictoryFanfareClip()
    {
        audioSource.clip = roundVictoryClip;
        audioSource.Play();
    }

    public void PlayFailureFanfareClip()
    {
        audioSource.clip = failureFanfareClip;
        audioSource.Play();
    }

    public void PlayPickUpClip()
    {
        audioSource.PlayOneShot(pickupClip);
    }

    public void PlayPutDownClip()
    {
        audioSource.PlayOneShot(putdownClip);
    }

    public void PlayInvalidPlacementClip()
    {
        audioSource.PlayOneShot(invalidPlaceClip);
    }

    public void PlayDamageClip()
    {
        audioSource.PlayOneShot(damangeClip);
    }

    public void PlayDeathClip()
    {
        audioSource.PlayOneShot(deathClip);
    }

    public void PlaySpecialAttackClip()
    {
        audioSource.PlayOneShot(specialAttackClip);
    }

    public void PlayMovementClip()
    {
        audioSource.PlayOneShot(movementClip);
    }

    public void PlayBeeSpecialAttackClip()
    {
        audioSource.PlayOneShot(beeSpecialAttackClip);
    }

    public void PlayQueenBeeSpecialAttackClip()
    {
        audioSource.PlayOneShot(queenBeeSpecialAttackClip);
    }

    public void PlayBeetleSpecialAttackClip()
    {
        audioSource.PlayOneShot(beetleSpecialAttackClip);
    }

    public void PlaySpiderSpecialAttackClip()
    {
        audioSource.PlayOneShot(spiderSpecialAttackClip);
    }

    public void PlayMothSpecialAttackClip()
    {
        audioSource.PlayOneShot(mothSpecialAttackClip);
    }
}
