#region File Description

//-----------------------------------------------------------------------------
// ParticleVertex.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

#endregion

namespace Octopussy.Managers.ParticlesManager
{
    /// <summary>
    /// Custom vertex structure for drawing particles.
    /// </summary>
    internal struct ParticleVertex
    {
        // Stores which corner of the particle quad this vertex represents.
        public const int SizeInBytes = 36;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
            new VertexElement(0, VertexElementFormat.Short2,
                              VertexElementUsage.Position, 0),
            new VertexElement(4, VertexElementFormat.Vector3,
                              VertexElementUsage.Position, 1),
            new VertexElement(16, VertexElementFormat.Vector3,
                              VertexElementUsage.Normal, 0),
            new VertexElement(28, VertexElementFormat.Color,
                              VertexElementUsage.Color, 0),
            new VertexElement(32, VertexElementFormat.Single,
                              VertexElementUsage.TextureCoordinate, 0)
            );

        public Short2 Corner;

        // Stores the starting position of the particle.
        public Vector3 Position;

        // Stores the starting velocity of the particle.

        // Four random values, used to make each particle look slightly different.
        public Color Random;

        // The time (in seconds) at which this particle was created.
        public float Time;
        public Vector3 Velocity;


        // Describe the layout of this vertex structure.
    }
}