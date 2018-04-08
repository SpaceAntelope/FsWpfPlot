namespace FsWpfPlot.Views 

open System.Windows
open System.Windows.Controls
open FsWpfPlot.Models    
open FsWpfPlot.Classes
open FsWpfPlot.HelperFunctions
open FsWpfPlot.Types
open HelixToolkit.Wpf

type SurfacePlotContainerProxy(model: Plot3DModel ) = 
    inherit XamlProxyBase("/FsWpfPlot;component/Views/SurfacePlotContainer.xaml", model)
    let miExit : MenuItem = base.Window?miExit
    let miGradientY : MenuItem = base.Window?miGradientY
    let miLighting : MenuItem = base.Window?miLighting
    let miExport : MenuItem = base.Window?miExport
    let viewPort : HelixViewport3D = base.Window?viewPort
    let rbLinear : RadioButton = base.Window?rbLinear
    let rbSurface : RadioButton = base.Window?rbSurface
    let rbScatter : RadioButton = base.Window?rbScatter
    let rbWireframe : RadioButton = base.Window?rbWireframe

    let onPlotKindChecked (args : RoutedEventArgs) = 
        let rb = args.OriginalSource :?> RadioButton
        model.PlotKind <-
            if rb = rbLinear then PlotKind.Linear
            else if rb = rbScatter then PlotKind.Points
            else if rb = rbWireframe then PlotKind.Wireframe
            else PlotKind.Surface
            
        
    do 
        let win = base.Window
        miExit.Click.Add(fun _ -> win.Close())

        miGradientY.IsChecked <- true;

        miGradientY.Checked.Add(fun _ -> 
            model.ColorCoding <- ColorCoding.ByGradientY
            model.raisePropertyChanged("ColorCoding")
            miLighting.IsChecked <- false )

        miLighting.Checked.Add(fun _ -> 
            model.ColorCoding <- ColorCoding.ByLights
            model.raisePropertyChanged("ColorCoding")
            miGradientY.IsChecked <- false )

        miExport.Click.Add(fun _ -> SaveViewPortToFile viewPort.Viewport)

        rbSurface.Checked.Add onPlotKindChecked
        rbScatter.Checked.Add onPlotKindChecked
        rbLinear.Checked.Add onPlotKindChecked
        rbWireframe.Checked.Add onPlotKindChecked


    member this.Show() = Application().Run(this.Window)
