using SugarRush.Core;
using SugarRush.GameFlow;
using SugarRush.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Resets the live scene state without reloading it. Used by PauseMenu /
    /// ResultPanel Restart and Retry buttons. Round 2: hard-reset player +
    /// glucose + level. Round 3 would re-seed obstacles / pooled items.
    ///
    /// Falls back to SceneManager.LoadScene when any of the reset targets
    /// can't be located, so a missing LevelManager won't strand the player.
    /// </summary>
    public class RuntimeResetter : MonoBehaviour
    {
        public static void Reset()
        {
            // 1. Resume time first so OnEnable / OnDisable callbacks (which some
            //    systems hook to timeScale==0) still run normally.
            Time.timeScale = 1f;

            // 2. Clear UI overlays so the player doesn't see a frozen panel
            //    while the world snaps back to start.
            GameEvents.RaiseGameResumed();

            // 3. Resolve systems
            var levelManager = FindObjectOfType<LevelManager>();
            var player = GameObject.FindWithTag("Player");
            var glucose = player != null ? player.GetComponent<GlucoseSystem>() : null;
            var skiing = player != null ? player.GetComponent<SkiingController>() : null;

            if (levelManager == null || player == null)
            {
                Debug.LogWarning("[RuntimeResetter] LevelManager or Player not found; falling back to scene reload.");
                FallbackSceneReload();
                return;
            }

            // 4. Order matters: glucose reset before level so HUD/UI observers
            //    see the new value when level reset triggers a distance=0 frame.
            if (glucose != null) glucose.Reset();
            if (skiing != null) skiing.Reset();
            levelManager.Reset();

            Debug.Log("[RuntimeResetter] Run state reset complete.");
        }

        private static void FallbackSceneReload()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
