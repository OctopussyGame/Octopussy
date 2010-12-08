#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
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
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            var campaignMenuEntry = new MenuEntry("Kampan");
            var singlePlayerMenuEntry = new MenuEntry("Hra jednoho hrace");
            var multiPlayerMenuEntry = new MenuEntry("Hra vice hracu");
            var optionsMenuEntry = new MenuEntry("Nastaveni");
            var exitMenuEntry = new MenuEntry("Konec");

            // Hook up menu event handlers.
            campaignMenuEntry.Selected += (sender, e) => LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                                                            new GameplayScreen(GameMode.SinglePlayer));
            singlePlayerMenuEntry.Selected += (sender, e) => LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                                                            new GameplayScreen(GameMode.SinglePlayer));
            multiPlayerMenuEntry.Selected += (sender, e) => ScreenManager.AddScreen(
                                                                            new MultiPlayerMenuScreen(), e.PlayerIndex);
            optionsMenuEntry.Selected += (sender, e) => ScreenManager.AddScreen(
                                                                            new OptionsMenuScreen(), e.PlayerIndex);
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(campaignMenuEntry);
            MenuEntries.Add(singlePlayerMenuEntry);
            MenuEntries.Add(multiPlayerMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input
        
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Jsi si jisty, ze chces ukoncit hru?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
