using SharpDX;
#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF
    using Model.Scene;
#endif
    public class BezierSplineGeometryModel3D : LineGeometryModel3D
    {
        private SplineMaterialCore material => (SplineMaterialCore)((SplineNode)this.SceneNode).Material;

        public static readonly Matrix BezierSplineSplineMatrix = new Matrix(
            -1, 3, -3, 1,
            3, -6,  3, 0,
            -3, 3,  0, 0,
            1,  0,  0, 0
        );

        /// <inheritdoc />
        protected override SceneNode OnCreateSceneNode()
        {
            return new SplineNode { Material = new SplineMaterialCore() };
        }

        /// <inheritdoc />
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            this.material.SplineMatrix = BezierSplineSplineMatrix;
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}
