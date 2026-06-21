using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>
    /// Plays a one-shot fullscreen intro video (VideoClip -> RenderTexture -> RawImage) before
    /// gameplay, freezing the game until it finishes or the player skips. Self-contained: does not
    /// depend on a scene camera. Hands off via OnFinished (e.g. to a StoryCardPlayer.Play()).
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class StoryVideoPlayer : MonoBehaviour
    {
        [SerializeField] private VideoClip _clip;
        [SerializeField] private GameObject _root;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private GameObject _skipHint;
        [SerializeField] private bool _freezeGameplay = true;
        [SerializeField] private string _playOnceKey = "sr_video_intro";

        public UnityEvent OnFinished;

        private readonly IStoryProgressStore _store = new PlayerPrefsStore();
        private VideoPlayer _video;
        private RenderTexture _rt;
        private bool _active;
        private bool _finished;

        private void Awake()
        {
            _video = GetComponent<VideoPlayer>();
        }

        private void Start()
        {
            if (_clip == null || (!string.IsNullOrEmpty(_playOnceKey) && _store.HasSeen(_playOnceKey)))
            {
                if (_root != null) _root.SetActive(false);
                Finish(false);
                return;
            }

            if (_root != null) _root.SetActive(true);
            if (_freezeGameplay) Time.timeScale = 0f;
            _active = true;

            int w = (int)_clip.width;  if (w <= 0) w = 1280;
            int h = (int)_clip.height; if (h <= 0) h = 720;
            _rt = new RenderTexture(w, h, 0);
            if (_rawImage != null) _rawImage.texture = _rt;

            _video.playOnAwake = false;
            _video.isLooping = false;
            _video.source = VideoSource.VideoClip;
            _video.clip = _clip;
            _video.renderMode = VideoRenderMode.RenderTexture;
            _video.targetTexture = _rt;
            _video.audioOutputMode = VideoAudioOutputMode.Direct;
            // Unscaled clock so the video keeps playing while gameplay is frozen (timeScale = 0).
            _video.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
            _video.loopPointReached += OnVideoEnd;
            _video.errorReceived += OnVideoError;
            _video.Play();
        }

        // Update runs regardless of Time.timeScale, so skip input works while frozen.
        private void Update()
        {
            if (!_active) return;
            if (_freezeGameplay) Time.timeScale = 0f;

            if (Input.GetKeyDown(KeyCode.Escape)
                || Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetMouseButtonDown(0))
            {
                Finish(true);
            }
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            Finish(true);
        }

        private void OnVideoError(VideoPlayer vp, string message)
        {
            Debug.LogError("[StoryVideo] " + message, this);
            Finish(true);
        }

        private void Finish(bool markSeen)
        {
            if (_finished) return;
            _finished = true;

            if (_video != null)
            {
                _video.loopPointReached -= OnVideoEnd;
                _video.errorReceived -= OnVideoError;
                _video.Stop();
            }
            if (markSeen && !string.IsNullOrEmpty(_playOnceKey)) _store.MarkSeen(_playOnceKey);

            _active = false;
            if (_freezeGameplay) Time.timeScale = 1f;
            if (_root != null) _root.SetActive(false);
            if (_rt != null) { _rt.Release(); _rt = null; }
            OnFinished?.Invoke();
        }
    }
}
