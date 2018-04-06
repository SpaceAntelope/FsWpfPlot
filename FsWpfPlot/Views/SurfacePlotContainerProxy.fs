namespace FsWpfPlot

module Views =

    open System
    open System.Windows
    open System.Windows.Controls
    open System.Windows.Markup    
    open FsWpfPlot.Models    
    open Classes
    open HelperFunctions
    open FsWpfPlot.Types
    open HelixToolkit.Wpf
    open FsWpfPlot.VisualElements
    open Microsoft.Win32

    type SurfacePlotContainerProxy(model: SurfacePlotModel ) = 
        inherit XamlProxyBase("/FsWpfPlot;component/Views/SurfacePlotContainer.xaml", model)
        let miExit : MenuItem = base.Window?miExit
        let miGradientY : MenuItem = base.Window?miGradientY
        let miLighting : MenuItem = base.Window?miLighting
        let miExport : MenuItem = base.Window?miExport
        let viewPort : HelixViewport3D = base.Window?viewPort

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

            miExport.Click.Add(fun _ -> HelperFunctions.SaveViewPortToFile viewPort.Viewport)

        member this.Show() = Application().Run(this.Window)
