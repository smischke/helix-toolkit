/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
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
        using Shaders;

        public class SplineMaterialVariable : LineMaterialVariable
        {
            private readonly SplineMaterialCore material;

            public static readonly ConstantBufferDescription MatrixSplineConstantBufferDesc
                = new ConstantBufferDescription(DefaultBufferNames.MatrixSplineModelCB,
                    MatrixSplineMaterialStruct.SizeInBytes);

            /// <summary>
            /// Initializes a new instance of the <see cref="LineMaterialVariable"/> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="technique">The technique.</param>
            /// <param name="materialCore">The material core.</param>
            /// <param name="linePassName">Name of the line pass.</param>
            /// <param name="shadowPassName">Name of the shadow pass.</param>
            /// <param name="depthPassName">Name of the depth pass</param>
            public SplineMaterialVariable(IEffectsManager manager, IRenderTechnique technique, SplineMaterialCore materialCore,
                string linePassName = DefaultPassNames.Default, string shadowPassName = DefaultPassNames.ShadowPass,
                string depthPassName = DefaultPassNames.DepthPrepass)
                : base(manager, technique, MatrixSplineConstantBufferDesc, materialCore, 
                       linePassName, shadowPassName, depthPassName)
            {
                this.material = materialCore;          
            }

            protected override void OnInitialPropertyBindings()
            {
                base.OnInitialPropertyBindings();
                this.AddPropertyBinding(nameof(SplineMaterialCore.SplineMatrix), () => { this.WriteValue(MatrixSplineMaterialStruct.SplineMatrix, this.material.SplineMatrix); });
            }
        }
    }
}
