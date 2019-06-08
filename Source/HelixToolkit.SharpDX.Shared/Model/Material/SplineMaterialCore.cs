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
            private float tension = 0.5f;

            /// <summary>
            /// Gets or sets the tension of the spline.
            /// </summary>
            /// <value>
            /// The tension of the spline.
            /// </value>
            public float Tension
            {
                set { Set(ref this.tension, value); }
                get { return this.tension; }
            }

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new SplineMaterialVariable(manager, manager.GetTechnique(DefaultRenderTechniqueNames.Splines), this);
            }
        }
    }
}
