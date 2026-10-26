namespace VelocityRush.Core
{
    /// <summary>
    /// Top-level phases of the game loop, in typical progression order.
    /// </summary>
    public enum GameState
    {
        /// <summary>First frame before any scene flow has run.</summary>
        Boot = 0,

        /// <summary>Front-end title screen.</summary>
        MainMenu = 1,

        /// <summary>Car browsing and upgrading.</summary>
        Garage = 2,

        /// <summary>A scene load is in flight.</summary>
        Loading = 3,

        /// <summary>On the grid, 3..1 lights before the race opens.</summary>
        Countdown = 4,

        /// <summary>Race is live.</summary>
        Racing = 5,

        /// <summary>Race is suspended by the player.</summary>
        Paused = 6,

        /// <summary>Race ended, results are being shown.</summary>
        Finished = 7
    }
}
