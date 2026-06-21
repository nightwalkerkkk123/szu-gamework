using UnityEngine;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>
    /// Ordered set of full-screen story illustration pages, optionally shown only once.
    /// </summary>
    [CreateAssetMenu(menuName = "SugarRush/Story/Card Sequence", fileName = "Seq_NewStory")]
    public class StoryCardSequence : ScriptableObject
    {
        [Tooltip("PlayerPrefs 键；留空 = 每次都播")]
        public string PlayOnceKey;

        [Tooltip("有序页面")]
        public Sprite[] Pages;
    }
}
