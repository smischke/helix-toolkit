/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Runtime.Serialization;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        public class SplineMaterialCore : LineMaterialCore
        {
            private Matrix splineMatrix;

            /// <summary>
            /// Gets or sets the spline matrix.
            /// </summary>
            /// <value>
            /// The spline matrix.
            /// </value>
            public Matrix SplineMatrix
            {
                set { this.Set(ref this.splineMatrix, value); }
                get { return this.splineMatrix; }
            }

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new SplineMaterialVariable(manager, manager.GetTechnique(DefaultRenderTechniqueNames.Splines), this);
            }
        }
    }
}
