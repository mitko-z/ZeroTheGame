using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroTheGame
{
    /// <summary>
    /// Define the states of the game in game mode
    /// </summary>
    public enum RunningGameStates
    {
        StartScreen,
        Playing,
        FinishedLevelScreen,
        GameFinished,
        GameOver
    }

    /// <summary>
    /// Define the states of the game in menu mode
    /// </summary>
    public enum RunningMenuStates
    {
        MainMenu,
        SaveGameMenu,
        LoadGameMenu,
        OptionsMenu
    }

    /// <summary>
    /// define two modes of this game
    /// GameMode - the game plays and shows screens between levels
    /// MenuMode - the game is paused and menus are managed
    /// </summary>
    public enum Mode
    {
        GameMode,
        MenuMode
    }



}
