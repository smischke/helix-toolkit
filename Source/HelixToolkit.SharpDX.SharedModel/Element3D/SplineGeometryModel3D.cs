using global::SharpDX;
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
    public class SplineGeometryModel3D : LineGeometryModel3D
    {
        /// <summary>
        /// The spline tension property
        /// </summary>
        public static readonly DependencyProperty TensionProperty =
            DependencyProperty.Register("Tension", typeof(double), typeof(SplineGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                ((SplineGeometryModel3D)d).material.Tension = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double Tension
        {
            get { return (double)this.GetValue(TensionProperty); }
            set { this.SetValue(TensionProperty, value); }
        }

        protected new SplineMaterialCore material => base.material as SplineMaterialCore;

        protected override SceneNode OnCreateSceneNode()
        {
            return new SplineNode { Material = new SplineMaterialCore() };
        }
    }
}
