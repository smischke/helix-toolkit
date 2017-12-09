﻿#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.DXGI;
    using System;
    using Utilities;
    public class BillboardBufferModel<VertexStruct> : GeometryBufferModel, IBillboardBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(IBillboardText geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        private ShaderResourceView textureView;
        private ShaderResourceView alphaTextureView;
        public ShaderResourceView TextureView { get { return textureView; } }
        public ShaderResourceView AlphaTextureView { get { return alphaTextureView; } }

        public BillboardType Type { private set; get; }

        public BillboardBufferModel(int structSize)
            : base(PrimitiveTopology.TriangleStrip, new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer), null)
        {
        }

        protected override void OnCreateIndexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry)
        {

        }

        protected override void OnCreateVertexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry)
        {
            RemoveAndDispose(ref textureView);
            RemoveAndDispose(ref alphaTextureView);
            var billboardGeometry = geometry as IBillboardText;
            
            if (billboardGeometry != null && billboardGeometry.BillboardVertices != null)
            {
                Type = billboardGeometry.Type;              
                var data = OnBuildVertexArray(billboardGeometry);
                buffer.CreateBufferFromDataArray(context.Device, data, billboardGeometry.BillboardVertices.Count);
              
                if (billboardGeometry.Texture != null)
                {
#if !NETFX_CORE
                    textureView = Collect(global::SharpDX.Toolkit.Graphics.Texture.Load(context.Device, billboardGeometry.Texture));
#else

#endif
                }
                if (billboardGeometry.AlphaTexture != null)
                {
                    alphaTextureView = Collect(global::SharpDX.Toolkit.Graphics.Texture.Load(context.Device, billboardGeometry.AlphaTexture));
                }
            }
            else
            {
                buffer.Dispose();
            }
        }

        protected override bool OnAttachBuffer(DeviceContext context, InputLayout vertexLayout, int vertexBufferSlot)
        {
            switch (Type)
            {
                case BillboardType.MultipleText:
                    context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    break;
                default:
                    context.InputAssembler.PrimitiveTopology = Topology;
                    break;
            }
            context.InputAssembler.InputLayout = vertexLayout;
            context.InputAssembler.SetIndexBuffer(null, Format.Unknown, 0);
            if (VertexBuffer != null)
            {
                context.InputAssembler.SetVertexBuffers(vertexBufferSlot, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset));
            }
            return true;
        }
    }
}
