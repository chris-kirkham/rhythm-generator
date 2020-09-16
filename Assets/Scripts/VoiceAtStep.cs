///info about a voice at a single timestep
public struct VoiceAtStep
{
    public int id;
    public int step;
    public float time;
    public bool sounding;
    public float value;
    public float stepInterval;
    public float leftNeighbourInfluence;
    public float rightNeighbourInfluence;
    public float stepIncrement;

    public VoiceAtStep(int id, int step, float time, bool sounding, float value, float stepInterval, float leftNeighbourInfluence, float rightNeighbourInfluence, float stepIncrement)
    {
        this.id = id;
        this.step = step;
        this.time = time;
        this.sounding = sounding;
        this.value = value;
        this.stepInterval = stepInterval;
        this.leftNeighbourInfluence = leftNeighbourInfluence;
        this.rightNeighbourInfluence = rightNeighbourInfluence;
        this.stepIncrement = stepIncrement;
    }

    public override string ToString()
    {
        return "(" + sounding + ", " + value + ")";
    }
}
