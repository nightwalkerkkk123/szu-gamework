using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>Abstraction over persistent "已看过" storage so logic stays testable.</summary>
    public interface IStoryProgressStore
    {
        bool HasSeen(string key);
        void MarkSeen(string key);
    }

    /// <summary>Production store backed by UnityEngine.PlayerPrefs.</summary>
    public class PlayerPrefsStore : IStoryProgressStore
    {
        public bool HasSeen(string key)
        {
            return !string.IsNullOrEmpty(key) && PlayerPrefs.GetInt(key, 0) == 1;
        }

        public void MarkSeen(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Pure functions deciding which sequences to play and flattening their pages.</summary>
    public static class StoryProgress
    {
        /// <summary>
        /// Returns the sequences that should play: those with an empty PlayOnceKey (always)
        /// or a key the store hasn't seen. Skips null sequences.
        /// </summary>
        public static IReadOnlyList<StoryCardSequence> ResolvePlayable(
            IReadOnlyList<StoryCardSequence> sequences, IStoryProgressStore store)
        {
            var result = new List<StoryCardSequence>();
            if (sequences == null) return result;
            foreach (var seq in sequences)
            {
                if (seq == null) continue;
                if (string.IsNullOrEmpty(seq.PlayOnceKey) || !store.HasSeen(seq.PlayOnceKey))
                {
                    result.Add(seq);
                }
            }
            return result;
        }

        /// <summary>Concatenates the non-null pages of the given sequences in order.</summary>
        public static Sprite[] FlattenPages(IReadOnlyList<StoryCardSequence> sequences)
        {
            var pages = new List<Sprite>();
            if (sequences == null) return pages.ToArray();
            foreach (var seq in sequences)
            {
                if (seq == null || seq.Pages == null) continue;
                foreach (var page in seq.Pages)
                {
                    if (page != null) pages.Add(page);
                }
            }
            return pages.ToArray();
        }
    }
}
