using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>
    /// Full-screen story-card overlay. On Start it plays unseen sequences page by page,
    /// freezing gameplay until the player advances through them (or skips with ESC).
    /// </summary>
    public class StoryCardPlayer : MonoBehaviour
    {
        [SerializeField] private StoryCardSequence[] _sequences;
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _pageImage;
        [SerializeField] private GameObject _nextHint;
        [SerializeField] private bool _freezeGameplay = true;
        [Tooltip("勾选时 Start 自动播放；取消勾选则由外部调用 Play()（如视频过场结束后）")]
        [SerializeField] private bool _autoPlayOnStart = true;

        public UnityEvent OnComplete;

        private readonly IStoryProgressStore _store = new PlayerPrefsStore();
        private Sprite[] _pages;
        private IReadOnlyList<StoryCardSequence> _playing;
        private int _index;
        private bool _active;

        private void Start()
        {
            if (_autoPlayOnStart) Play();
        }

        /// <summary>
        /// Begin playing the sequences. Call externally when _autoPlayOnStart is false
        /// (e.g. handed off from an intro video's OnFinished event).
        /// </summary>
        public void Play()
        {
            _playing = StoryProgress.ResolvePlayable(_sequences, _store);
            _pages = StoryProgress.FlattenPages(_playing);

            if (_pages.Length == 0)
            {
                if (_root != null) _root.SetActive(false);
                if (_freezeGameplay) Time.timeScale = 1f;
                OnComplete?.Invoke();
                return;
            }

            if (_root != null) _root.SetActive(true);
            if (_freezeGameplay) Time.timeScale = 0f;
            _index = 0;
            _active = true;
            ShowPage(0);
        }

        // Update runs regardless of Time.timeScale, so input works while frozen.
        private void Update()
        {
            if (!_active) return;

            // Re-assert the freeze every frame: other components' Start() (e.g. GameFlowManager
            // EnterIntro) set timeScale = 1 and would otherwise let gameplay run behind the card.
            if (_freezeGameplay) Time.timeScale = 0f;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Complete();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetMouseButtonDown(0))
            {
                Advance();
            }
        }

        private void Advance()
        {
            _index++;
            if (_index >= _pages.Length)
            {
                Complete();
            }
            else
            {
                ShowPage(_index);
            }
        }

        private void ShowPage(int i)
        {
            if (_pageImage != null)
            {
                _pageImage.sprite = _pages[i];
                _pageImage.preserveAspect = true;
            }
            if (_nextHint != null) _nextHint.SetActive(true);
        }

        private void Complete()
        {
            if (_active && _playing != null)
            {
                foreach (var seq in _playing)
                {
                    if (seq != null) _store.MarkSeen(seq.PlayOnceKey);
                }
            }

            _active = false;
            if (_freezeGameplay) Time.timeScale = 1f;
            if (_root != null) _root.SetActive(false);
            OnComplete?.Invoke();
        }
    }
}
