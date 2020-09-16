using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public Voice[] voices; //allows attaching of voices via inspector
    public bool wrapNeighbours = false;

    private void Awake()
    {
        Debug.Log(voices.Length);

        //initialise each attached voice
        for(int i = 0; i < voices.Length; i++)
        {
            voices[i].Init(this, i);
        }
    }

    //returns a voice's neighbours at their most recent timesteps
    public (VoiceAtStep, VoiceAtStep) GetNeighbours(int voiceID)
    {
        int numVoices = voices.Length;
        Voice leftVoice, rightVoice;
        if (wrapNeighbours)
        {
            leftVoice = voices[ModWrap(voiceID - 1, numVoices)];
            rightVoice = voices[ModWrap(voiceID + 1, numVoices)];
        }
        else
        {
            leftVoice = voices[Mathf.Max(0, voiceID - 1)];
            rightVoice = voices[Mathf.Min(numVoices - 1, voiceID + 1)];
        }

        return (leftVoice.GetMostRecentStep(), rightVoice.GetMostRecentStep());
    }

    //modulo function which wraps negative numbers instead of returning a negative 
    private int ModWrap(int a, int b)
    {
        int c = a % b;
        return c < 0 ? c + b : c;
    }
}
