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

    type SurfacePlotContainerProxy(model: SurfacePlotModel ) = 
        inherit XamlProxyBase("/FsWpfPlot;component/Views/SurfacePlotContainer.xaml", model)
        let miExit : MenuItem = base.Window?miExit
        let miGradientY : MenuItem = base.Window?miGradientY
        let miLighting : MenuItem = base.Window?miLighting

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

        member this.Show() = Application().Run(this.Window)
