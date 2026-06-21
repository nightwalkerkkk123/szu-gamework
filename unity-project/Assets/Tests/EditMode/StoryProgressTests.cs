using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SugarRush.Foundation.StoryCard;

namespace SugarRush.Tests
{
    public class StoryProgressTests
    {
        private class FakeStore : IStoryProgressStore
        {
            public readonly HashSet<string> Seen = new HashSet<string>();
            public bool HasSeen(string key) => !string.IsNullOrEmpty(key) && Seen.Contains(key);
            public void MarkSeen(string key) { if (!string.IsNullOrEmpty(key)) Seen.Add(key); }
        }

        private static StoryCardSequence MakeSequence(string key, int pageCount)
        {
            var seq = ScriptableObject.CreateInstance<StoryCardSequence>();
            seq.PlayOnceKey = key;
            seq.Pages = new Sprite[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                var tex = new Texture2D(1, 1);
                seq.Pages[i] = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            }
            return seq;
        }

        [Test]
        public void ResolvePlayable_UnseenKeyedSequence_IsIncluded()
        {
            var seq = MakeSequence("k1", 1);
            var result = StoryProgress.ResolvePlayable(new[] { seq }, new FakeStore());
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(seq, result[0]);
        }

        [Test]
        public void ResolvePlayable_SeenKeyedSequence_IsSkipped()
        {
            var seq = MakeSequence("k1", 1);
            var store = new FakeStore();
            store.MarkSeen("k1");
            var result = StoryProgress.ResolvePlayable(new[] { seq }, store);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ResolvePlayable_EmptyKey_AlwaysIncluded()
        {
            var seq = MakeSequence("", 1);
            var result = StoryProgress.ResolvePlayable(new[] { seq }, new FakeStore());
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ResolvePlayable_NullSequence_IsSkipped()
        {
            var seq = MakeSequence("k1", 1);
            var result = StoryProgress.ResolvePlayable(
                new StoryCardSequence[] { null, seq }, new FakeStore());
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void FlattenPages_ConcatenatesInOrder()
        {
            var a = MakeSequence("a", 2);
            var b = MakeSequence("b", 1);
            var pages = StoryProgress.FlattenPages(new[] { a, b });
            Assert.AreEqual(3, pages.Length);
            Assert.AreSame(a.Pages[0], pages[0]);
            Assert.AreSame(a.Pages[1], pages[1]);
            Assert.AreSame(b.Pages[0], pages[2]);
        }

        [Test]
        public void FlattenPages_SkipsNullPages()
        {
            var seq = MakeSequence("a", 2);
            seq.Pages[0] = null;
            var pages = StoryProgress.FlattenPages(new[] { seq });
            Assert.AreEqual(1, pages.Length);
        }
    }
}
