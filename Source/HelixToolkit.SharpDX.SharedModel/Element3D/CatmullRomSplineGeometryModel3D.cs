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
    public class CatmullRomSplineGeometryModel3D : LineGeometryModel3D
    {
        private SplineMaterialCore material => (SplineMaterialCore)((SplineNode)this.SceneNode).Material;

        public static readonly Matrix CatmullRomUniformSplineMatrix = GetCatmullRomSplineMatrix(0.0f);

        public static readonly Matrix CatmullRomCentripetalSplineMatrix = GetCatmullRomSplineMatrix(0.5f);

        public static readonly Matrix CatmullRomChordalSplineMatrix = GetCatmullRomSplineMatrix(1.0f);

        /// <summary>
        /// The spline tension property
        /// </summary>
        public static readonly DependencyProperty TensionProperty =
            DependencyProperty.Register("Tension", typeof(double), typeof(CatmullRomSplineGeometryModel3D),
                new PropertyMetadata(
                    1.0,
                    (d, e) => ((CatmullRomSplineGeometryModel3D) d).material.SplineMatrix = GetCatmullRomSplineMatrix((float)(double)e.NewValue)));

        /// <summary>
        /// Gets or sets the spline tension.
        /// </summary>
        /// <value>
        /// The spline tension.
        /// </value>
        public double Tension
        {
            get { return (double)this.GetValue(TensionProperty); }
            set { this.SetValue(TensionProperty, value); }
        }

        public static Matrix GetCatmullRomSplineMatrix(float tension)
        {
            return new Matrix(
                -tension, 2f - tension, tension - 2f, tension,
                2f * tension, tension - 3f, 3f - 2f * tension, -tension,
                -tension, 0f, tension, 0f,
                0f, 1f, 0f, 0f
            );
        }

        /// <inheritdoc />
        protected override SceneNode OnCreateSceneNode()
        {
            return new SplineNode { Material = new SplineMaterialCore() };
        }

        /// <inheritdoc />
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            this.material.SplineMatrix = GetCatmullRomSplineMatrix((float)this.Tension);
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}
