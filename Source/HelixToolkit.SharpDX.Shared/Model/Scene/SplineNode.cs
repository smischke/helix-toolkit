/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

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
    namespace Model.Scene
    {
        using Core;
        using global::SharpDX.Direct3D;

        public class SplineNode : LineNode
        {
            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return host.EffectsManager[DefaultRenderTechniqueNames.Splines];
            }

            protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
            {
                return geometry != null && geometry.IsDynamic
                    ? this.EffectsManager.GeometryBufferManager.Register<DynamicSplineGeometryBufferModel>(modelGuid,
                        geometry)
                    : this.EffectsManager.GeometryBufferManager.Register<DefaultSplineGeometryBufferModel>(modelGuid,
                        geometry);
            }
        }

        public class DefaultSplineGeometryBufferModel : DefaultLineGeometryBufferModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultSplineGeometryBufferModel"/> class.
            /// </summary>
            public DefaultSplineGeometryBufferModel()
                : this(false)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultSplineGeometryBufferModel"/> class.
            /// </summary>
            /// <param name="isDynamic"></param>
            /// <param name="topology">The topology.</param>
            public DefaultSplineGeometryBufferModel(bool isDynamic,
                PrimitiveTopology topology = PrimitiveTopology.LineListWithAdjacency)
                : base(isDynamic, topology)
            {
                // main constructor
            }
        }

        public sealed class DynamicSplineGeometryBufferModel : DefaultSplineGeometryBufferModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicSplineGeometryBufferModel"/> class.
            /// </summary>
            public DynamicSplineGeometryBufferModel()
                : base(true)
            {
            }
        }
    }
}
