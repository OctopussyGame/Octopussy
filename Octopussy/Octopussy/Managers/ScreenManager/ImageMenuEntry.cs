#region File Description

//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Octopussy.Game.Screens;

#endregion

namespace Octopussy.Managers.ScreenManager
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    internal class ImageMenuEntry
    {
        #region Fields

        private readonly Texture2D texture;
        private readonly Texture2D textureSelected;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        private Vector2 position;

        private Rectangle rec;
        private Rectangle recSelected;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float selectionFade;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

// ReSharper disable MemberCanBePrivate.Global
        public Vector2 PositionOriginal { get; set; }
// ReSharper restore MemberCanBePrivate.Global

// ReSharper disable MemberCanBePrivate.Global
        public Vector2 PositionSelected { get; set; }
// ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public ImageMenuEntry(Rectangle rec, Rectangle recSelected, Texture2D texture, Texture2D textureSelected)
        {
            this.rec = rec;
            this.recSelected = recSelected;
            this.texture = texture;
            this.textureSelected = textureSelected;
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
// ReSharper disable UnusedParameter.Global
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
// ReSharper restore UnusedParameter.Global
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float) gameTime.ElapsedGameTime.TotalSeconds*4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
// ReSharper disable UnusedParameter.Global
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
// ReSharper restore UnusedParameter.Global
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            
            var origin = new Vector2(0, 0);

            if (!isSelected)
                position = PositionOriginal;
            else
                position = PositionSelected;

            if (isSelected)
                spriteBatch.Draw(textureSelected, position, recSelected, Color.White, 0, origin, 1, SpriteEffects.None,
                                 0);
            else
                spriteBatch.Draw(texture, position, rec, Color.White, 0, origin, 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
// ReSharper disable UnusedParameter.Global
        public virtual int GetHeight(MenuScreen screen)
// ReSharper restore UnusedParameter.Global
        {
            return rec.Height;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
// ReSharper disable UnusedParameter.Global
        public virtual int GetWidth(MenuScreen screen)
// ReSharper restore UnusedParameter.Global
        {
            return recSelected.Width;
        }

        #endregion
    }
}