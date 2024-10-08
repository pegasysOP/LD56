using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource movementSource;

    [Header("UI")]
    public AudioClip upgradeButtonPressClip;
    public AudioClip regularButtonPressClip;

    [Header("Phase Music")]
    public AudioClip planningPhaseClip;
    public AudioClip simulationPhaseClip;
    public AudioClip victoryFanfareClip;
    public AudioClip roundVictoryClip;
    public AudioClip failureFanfareClip;
    public AudioClip menuClip;

    [Header("Interaction Sounds")]
    public AudioClip pickupClip;
    public AudioClip putdownClip;
    public AudioClip invalidPlaceClip;
    public AudioClip damangeClip;
    public AudioClip deathClip;
    public AudioClip movementClip;
    public AudioClip projectileClip;

    [Header("Special Attacks")]
    public AudioClip specialAttackClip;
    public AudioClip beeSpecialAttackClip;
    public AudioClip queenBeeSpecialAttackClip;
    public AudioClip beetleSpecialAttackClip;
    public AudioClip spiderSpecialAttackClip;
    public AudioClip mothSpecialAttackClip;

    private bool playingMovementClip = false;

    public static AudioManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Ensure this object persists between scenes
        }
        else if (Instance != this)
        {
            // Destroy duplicate instances to prevent multiple AudioManagers
            Destroy(gameObject);
            return;
        }
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

    public void PlayMenuClip()
    {
        musicSource.clip = menuClip;
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
        // Check if a movement clip is already playing
        if (playingMovementClip)
        {
            return;
        }

        // Set the flag to true, indicating that a movement clip is playing
        playingMovementClip = true;

        // Set a random pitch for the sound effect
        movementSource.pitch = Random.Range(0.9f, 1.05f);

        // Play the movement clip
        movementSource.PlayOneShot(movementClip);

        // Start a coroutine to reset the playingMovementClip flag after the clip finishes
        StartCoroutine(ResetMovementClip(movementClip.length));
    }

    private IEnumerator ResetMovementClip(float clipDuration)
    {
        // Wait for the duration of the clip
        yield return new WaitForSeconds(0.1f);

        // Reset the flag, allowing new movement clips to be played
        playingMovementClip = false;
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

    public void PlayUpgradeButtonClip()
    {
        sfxSource.PlayOneShot(upgradeButtonPressClip);
    }

    public void PlayRegularButtonClip()
    {
        sfxSource.PlayOneShot(regularButtonPressClip);
    }

    public void PlayProjectileClip()
    {
        sfxSource.PlayOneShot(projectileClip);
    }
}
