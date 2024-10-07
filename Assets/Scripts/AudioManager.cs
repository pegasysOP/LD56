using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

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
        musicSource.clip = planningPhaseClip;
        musicSource.Play();
    }

    public void PlaySimulationPhaseClip()
    {
        musicSource.clip = simulationPhaseClip;
        musicSource.Play();
    }

    public void PlayVictoryFanfareClip()
    {
        musicSource.clip = victoryFanfareClip;
        musicSource.Play();
    }

    public void PlayRoundVictoryFanfareClip()
    {
        musicSource.clip = roundVictoryClip;
        musicSource.Play();
    }

    public void PlayFailureFanfareClip()
    {
        musicSource.clip = failureFanfareClip;
        musicSource.Play();
    }

    public void PlayPickUpClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(pickupClip);
    }

    public void PlayPutDownClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(putdownClip);
    }

    public void PlayInvalidPlacementClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(invalidPlaceClip);
    }

    public void PlayDamageClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(damangeClip);
    }

    public void PlayDeathClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(deathClip);
    }

    public void PlaySpecialAttackClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(specialAttackClip);
    }

    public void PlayMovementClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(movementClip);
    }

    public void PlayBeeSpecialAttackClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(beeSpecialAttackClip);
    }

    public void PlayQueenBeeSpecialAttackClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(queenBeeSpecialAttackClip);
    }

    public void PlayBeetleSpecialAttackClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(beetleSpecialAttackClip);
    }

    public void PlaySpiderSpecialAttackClip()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(spiderSpecialAttackClip);
    }

    public void PlayMothSpecialAttackClip()
    { 
        sfxSource.pitch = Random.Range(0.9f, 1.05f);
        sfxSource.PlayOneShot(mothSpecialAttackClip);
    }
}
