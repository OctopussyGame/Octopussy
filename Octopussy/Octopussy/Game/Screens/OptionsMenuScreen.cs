#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Octopussy
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        

        MenuEntry turnLeftMenuEntry;
        MenuEntry stepBackMenuEntry;
        MenuEntry stepForwardMenuEntry;
        MenuEntry turnRightMenuEntry;
        MenuEntry shootMenuEntry;
        MenuEntry changeWeaponMenuEntry;
        MenuEntry specialAbilityMenuEntry;
        MenuEntry pickWeaponMenuEntry;
        MenuEntry pauseMenuEntry;
        MenuEntry helpMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            turnLeftMenuEntry = new MenuEntry("A - otoceni vlevo");
            stepBackMenuEntry = new MenuEntry("S - krok dozadu (zvysit rychlost pohybu vzad)");
            stepForwardMenuEntry = new MenuEntry("W - krok vpred (zvysit rychlost pohybu dopredu)");
            turnRightMenuEntry = new MenuEntry("D - otoceni vpravo");
            shootMenuEntry = new MenuEntry("Space - vystrel ci provedeni akce s momentalne drzenou zbrani");
            changeWeaponMenuEntry = new MenuEntry("Shift - zmeni zbran (cyklicky)");
            specialAbilityMenuEntry = new MenuEntry("F - spusteni bonusove specialni schopnosti");
            pickWeaponMenuEntry = new MenuEntry("E - sebrani nove zbrane, nebo interakce s prostredim");
            pauseMenuEntry = new MenuEntry("ESC - vyvola in-game menu a zapauzuje hru");
            helpMenuEntry = new MenuEntry("F1  - napoveda");

            var back = new MenuEntry("Zpet");
            
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(turnLeftMenuEntry);
            MenuEntries.Add(stepBackMenuEntry);
            MenuEntries.Add(stepForwardMenuEntry);
            MenuEntries.Add(turnRightMenuEntry);
            MenuEntries.Add(shootMenuEntry);
            MenuEntries.Add(changeWeaponMenuEntry);
            MenuEntries.Add(specialAbilityMenuEntry);
            MenuEntries.Add(pickWeaponMenuEntry);
            MenuEntries.Add(pauseMenuEntry);
            MenuEntries.Add(helpMenuEntry);
            MenuEntries.Add(back);
        }


        #endregion

    }
}
