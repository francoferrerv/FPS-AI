public interface ITalkingCharacter
{
    public string name { get; }

    public void TalkTo(ITalkingCharacter talkingCharacter);

    public void StopTalkingTo(ITalkingCharacter talkingCharacter);
}
