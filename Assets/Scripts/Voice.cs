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

    //start/end options
    [Header("Start params")]
    [Min(0)] public float startTime = 0f;
    [Min(0)] public float endTime = 0f;
    public bool soundOnFirstStep = false;
    public float initialSoundingProbability = 0f;

    //loop parameters/trackers
    public bool[] loopSequencer;
    private int loopCurrentStep = 0;

    //serialised voice parameters, to be changed in the inspector/automated via animation clips
    [Header("Voice parameters")]
    [Tooltip("Time between steps - 1 = one semibreve at the voice manager's tempo")]
    public float stepInterval = 0.25f; 
    
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


        //initialise loop sequencer to length 1 if not initialsed to length >0 in inspector
        if(loopSequencer.Length == 0)
        {
            loopSequencer = new bool[1] { true };
        }
        

        StartCoroutine(StartStepCoroutineDelayed());
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
    }

    //waits until elapsed time has passed firstStepDelay, then starts the voice step coroutine
    private IEnumerator StartStepCoroutineDelayed()
    {
        while (Time.time < NoteLengthToRealTime(startTime))
        {
            yield return null;
        }

        Debug.Log("Start at time: " + Time.time);
        StartCoroutine(StepCoroutine());
    }

    private IEnumerator StepCoroutine()
    {
        //while(Time.time < NoteLengthToRealTime(endTime) || endTime <= 0f) //repeat while time < endTime; if endTime <= 0, repeat infinitely 
        while(true)
        {
            if(manager == null)
            {
                Debug.LogError("Voice " + name + " has no VoiceManager!");
            }
            else
            {
                loopCurrentStep++;
                if (loopCurrentStep > loopSequencer.Length - 1) loopCurrentStep = 0;

                (VoiceAtStep, VoiceAtStep) neighbours = manager.GetNeighbours(id);
                UpdateVoice(neighbours.Item1, neighbours.Item2);

                if (steps[steps.Count - 1].sounding && loopSequencer[loopCurrentStep])

                {
                    if (emphasiseEachNBeats > 0 && (steps.Count - 1) % emphasiseEachNBeats == 0)
                    {
                        audioPlayer.SetVolume(emphasisVolume);
                    }
                    else
                    {
                        audioPlayer.SetVolume(volume);
                    }

                    audioPlayer.PlayAudio();
                }
            }

            yield return new WaitForSeconds(NoteLengthToRealTime(stepInterval)); //convert step interval from 1 = 1 semibreve at manager tempo to actual time
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

    private float NoteLengthToRealTime(float noteLength)
    {
        return (noteLength * 60 * 4) / manager.tempo;
    }
}
