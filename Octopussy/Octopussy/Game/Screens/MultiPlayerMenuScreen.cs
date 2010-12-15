#region File Description

//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements



#endregion

using Octopussy.Managers.ScreenManager;

namespace Octopussy.Game.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    internal class MultiPlayerMenuScreen : MenuScreen
    {
        #region Fields

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiPlayerMenuScreen()
            : base("Multi Player")
        {
            // Create our menu entries.
            /*var ipAddressMenuEntry = new MenuEntry("IP Adresa: 192.168.1.101");
            var waitForPlayersMenuEntry = new MenuEntry("Zalozit hru a pockat na hrace");

            var back = new MenuEntry("Zpet");

            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(ipAddressMenuEntry);
            MenuEntries.Add(waitForPlayersMenuEntry);
            MenuEntries.Add(back);*/
        }

        #endregion
    }
}