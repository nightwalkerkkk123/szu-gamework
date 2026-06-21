using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// 鼠标点击跳转到不同场景的按钮控制器。
    /// 挂载到 Canvas 上，为各 Button 绑定对应的场景名称。
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        [Header("场景按钮")]
        [SerializeField] private Button _btnL1_HospitalChalet;
        [SerializeField] private Button _btnL2_Fusion;
        [SerializeField] private Button _btnL2_FusionOurs;
        [SerializeField] private Button _btnLevelSelect;
        [SerializeField] private Button _btnStartScene;
        [SerializeField] private Button _btnSettings;

        private void Start()
        {
            BindButton(_btnL1_HospitalChalet,     "L1_HospitalChalet");
            BindButton(_btnL2_Fusion,              "L2_Fusion");
            BindButton(_btnL2_FusionOurs,          "L2_Fusion_Ours");
            BindButton(_btnLevelSelect,            "LevelSelect");
            BindButton(_btnStartScene,              "StartScene");
            BindButton(_btnSettings,               "SettingsScene");
        }

        private void OnDestroy()
        {
            UnbindButton(_btnL1_HospitalChalet);
            UnbindButton(_btnL2_Fusion);
            UnbindButton(_btnL2_FusionOurs);
            UnbindButton(_btnLevelSelect);
            UnbindButton(_btnStartScene);
            UnbindButton(_btnSettings);
        }

        private void BindButton(Button button, string sceneName)
        {
            if (button == null) return;
            button.onClick.AddListener(() => LoadScene(sceneName));
        }

        private void UnbindButton(Button button)
        {
            if (button == null) return;
            button.onClick.RemoveAllListeners();
        }

        private void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;
            Debug.Log($"[SceneSwitcher] 切换到场景: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
