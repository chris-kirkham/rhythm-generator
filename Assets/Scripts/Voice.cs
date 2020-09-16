using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayAudioFromRhythm))]
public class Voice : MonoBehaviour
{
    //these are set by the manager the voice is attached to on startup
    private VoiceManager manager;
    private int id;

    //audio player for this voice
    private PlayAudioFromRhythm audioPlayer;

    //list to hold voice info at each step of the music
    private List<VoiceAtStep> steps;

    //options for first step of music
    [Header("Start params")]
    [Min(0)] public float firstStepOffset = 0f;
    public bool soundOnFirstStep = false;
    public float initialSoundingProbability = 0f;

    //serialised voice parameters, to be changed in the inspector/automated via animation clips
    [Header("Voice parameters")]
    [Tooltip("Time between steps")]
    public float stepInterval = 0.1f; 
    
    [Tooltip("value added to next step's sounding probability when left neighbour sounds")]
    public float leftNeighbourInfluence = 0f;

    [Tooltip("value added to next step's sounding probability when right neighbour sounds")]
    public float rightNeighbourInfluence = 0f;
    
    [Tooltip("value added to sounding probability per step")]
    public float stepIncrement = 0f;

    //playing/expression parameters
    [Header("Expression parameters")]
    [Min(0)] public int emphasiseEachNBeats = 0;
    [Range(0, 1)] public float volume = 0.5f;
    [Range(0, 1)] public float emphasisVolume = 0.75f;

    private void Start()
    {
        audioPlayer = GetComponent<PlayAudioFromRhythm>();
        audioPlayer.Init();

        StartCoroutine(StepCoroutine());
    }

    //Called by the VoiceManager this voice is attached to on startup
    public void Init(VoiceManager manager, int id)
    {
        this.manager = manager;
        this.id = id;

        steps = new List<VoiceAtStep>() 
        { 
            new VoiceAtStep
            (
                id,
                0,
                0f,
                soundOnFirstStep,
                initialSoundingProbability,
                stepInterval,
                leftNeighbourInfluence,
                rightNeighbourInfluence,
                stepIncrement
            ) 
        };

        Debug.Log(name + " steps size:" + steps.Count);
    }

    public IEnumerator StepCoroutine()
    {
        while(true)
        {
            (VoiceAtStep, VoiceAtStep) neighbours = manager.GetNeighbours(id);
            UpdateVoice(neighbours.Item1, neighbours.Item2);
            //Debug.Log(name + ": " + steps[steps.Count - 1]);

            if (steps[steps.Count - 1].sounding)
            {
                if(emphasiseEachNBeats > 0 && (steps.Count - 1) % emphasiseEachNBeats == 0)
                {
                    audioPlayer.SetVolume(emphasisVolume);
                    Debug.Log("beat emphasis");
                }
                else
                {
                    audioPlayer.SetVolume(volume);
                }

                audioPlayer.PlayAudio();
            }

            yield return new WaitForSeconds(stepInterval);
        }
    }

    public void UpdateVoice(VoiceAtStep leftNeighbour, VoiceAtStep rightNeighbour)
    {
        VoiceAtStep currStep = steps[steps.Count - 1];

        //create this voice's next step - inherit non-automatable variables from voice at current step, 
        //get automatable variables from public params
        VoiceAtStep nextStep = new VoiceAtStep
        (
            0,
            currStep.step + 1,
            Time.time,
            false,
            currStep.value,
            stepInterval,
            leftNeighbourInfluence,
            rightNeighbourInfluence,
            stepIncrement
        );

        //if current voice sounded this step, reset it
        if(currStep.sounding) nextStep.value = 0f;

        //check if neighbours sounded this step
        if (leftNeighbour.sounding && leftNeighbour.id != id) nextStep.value += leftNeighbourInfluence;
        if (rightNeighbour.sounding && rightNeighbour.id != id) nextStep.value += rightNeighbourInfluence;

        //add steps-without-sounding increment
        nextStep.value += stepIncrement;

        //calculate whether voice sounds next step
        nextStep.sounding = (Random.Range(0f, 1f) < nextStep.value);

        //store next step in steps list
        steps.Add(nextStep);
    }

    public VoiceAtStep GetMostRecentStep()
    {
        return steps[steps.Count - 1];
    }

    private bool IsMultipleOf(float a, float b)
    {
        float multiplier = 1000f;

        return (int)(a * multiplier) % (int)(b * multiplier) == 0;
    }
}
