using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RhythmGenerator : MonoBehaviour
{
    public Voice[] voices;
    public bool wrapVoices = false;

    //TODO: MOVE SOMEWHERE ELSE
    private PlayAudioFromRhythm audioPlayer;

    void Start()
    {
        //audioPlayer = GetComponent<PlayAudioFromRhythm>();
    }

    /*
    void UpdateSteps()
    {
        int numVoices = voices.Length;
        VoiceAtStep[] newStep = new VoiceAtStep[numVoices];

        //calculate rule outcomes for each voice 
        for (int i = 0; i < numVoices; i++)
        {
            VoiceAtStep voice = Steps[currentStep][i]; //get voice values from previous step

            //get neighbour info for this step
            VoiceAtStep leftVoice, rightVoice;
            if (wrapVoices)
            {
                leftVoice = Steps[currentStep][modWrap(i - 1, numVoices)];
                rightVoice = Steps[currentStep][modWrap(i + 1, numVoices)];
            }
            else
            {
                leftVoice = Steps[currentStep][Mathf.Max(0, i - 1)];
                rightVoice = Steps[currentStep][Mathf.Min(numVoices - 1, i + 1)];
            }

            voices[i].UpdateVoice(leftVoice.sounding, rightVoice.sounding);

            //check whether voice sounds in the next step by generating a random number and checking if it's higher than (final voice value / voice divisor value)
            voice.sounding = (Random.Range(0f, 1f) < voice.value);

            newStep[i] = voice;
        }

        Steps.Add(newStep);
        currentStep++;

        Debug.Log(string.Join(", ", newStep));
    }

    private void PlayAudioAtCurrent()
    {
        bool[] sounding = new bool[numVoices];
        VoiceAtStep[] current = Steps[currentStep];
        for (int i = 0; i < numVoices; i++)
        {
            sounding[i] = current[i].sounding;
        }
        audioPlayer.PlayAudio(sounding);
    }

    //modulo function which wraps negative numbers instead of returning a negative 
    private int modWrap(int a, int b)
    {
        int c = a % b;
        return c < 0 ? c + b : c;
    }
    */
}
