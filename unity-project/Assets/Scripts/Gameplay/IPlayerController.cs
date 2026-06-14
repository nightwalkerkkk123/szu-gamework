namespace SugarRush.Gameplay
{
    /// <summary>
    /// Common interface for player controllers (2D skiing, 3D sphere, etc.)
    /// so GameFlowManager and HUD can interact with them generically.
    /// </summary>
    public interface IPlayerController
    {
        void SetEnabled(bool enabled);
        float Speed { get; }
    }
}
