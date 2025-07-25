using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "PlayableDirector", menuName = "Timeline/PlayableDirector")]
public class PlayableDirectorSO : DescriptionBaseSO
{
    public PlayableDirector Director;
    public PlayableDirector PreDirector;
}
