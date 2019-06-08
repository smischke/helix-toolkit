/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    public class SplineMaterial : LineMaterial
    {
        public double Tension
        {
            get { return (double)this.GetValue(TensionProperty); }
            set { this.SetValue(TensionProperty, value); }
        }

        public static readonly DependencyProperty TensionProperty = DependencyProperty.Register(
            "Tension", typeof(double), typeof(SplineMaterial), new PropertyMetadata(
              0.5, (d, e) => ((d as LineMaterial).Core as SplineMaterialCore).Tension = (float)(double)e.NewValue));

        protected override MaterialCore OnCreateCore()
        {
            return new SplineMaterialCore
            {
                Name = this.Name,
                LineColor = this.Color.ToColor4(),
                Smoothness = (float) this.Smoothness,
                Thickness = (float) this.Thickness,
                EnableDistanceFading = this.EnableDistanceFading,
                FadingNearDistance = (float) this.FadingNearDistance,
                FadingFarDistance = (float) this.FadingFarDistance,
                Tension = (float)this.Tension,
            };
        }
    }
}