using SugarRush.GameFlow;
using SugarRush.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// In-game HUD showing distance, speed, time and glucose value.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [SerializeField] private Text _distanceText;
        [SerializeField] private Text _speedText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _glucoseText;

        private void Update()
        {
            if (_levelManager != null)
            {
                float distance = _levelManager.DistanceTraveled;
                float target = _levelManager.Data != null ? _levelManager.Data.TargetDistanceMeters : 0f;
                float progress = target > 0f ? Mathf.Clamp01(distance / target) * 100f : 0f;
                SetText(_distanceText, $"距离: {distance:F0}m / {target:F0}m ({progress:F0}%)");
                SetText(_timeText, $"时间: {_levelManager.ElapsedTime:F1}s");
            }
            else
            {
                SetText(_distanceText, "距离: --");
                SetText(_timeText, "时间: --");
            }

            if (_skiingController != null)
            {
                SetText(_speedText, $"速度: {_skiingController.Velocity.magnitude:F1}m/s");
            }
            else
            {
                SetText(_speedText, "速度: --");
            }

            if (_glucoseSystem != null)
            {
                SetText(_glucoseText, $"血糖: {_glucoseSystem.CurrentValue:F0} ({_glucoseSystem.CurrentZone})");
            }
            else
            {
                SetText(_glucoseText, "血糖: --");
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}