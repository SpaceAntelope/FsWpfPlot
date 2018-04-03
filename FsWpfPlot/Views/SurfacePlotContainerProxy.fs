namespace FsWpfPlot

module Views =

    open System
    open System.Windows
    open System.Windows.Controls
    open System.Windows.Markup    
    open FsWpfPlot.Models    
    open Classes

    type SurfacePlotContainerProxy(model: SurfacePlotModel ) = 
        inherit XamlProxyBase("/FsWpfPlot;component/Views/SurfacePlotContainer.xaml")
        let miExit : MenuItem = base.Window?miExit
        do 
            let win = base.Window
            miExit.Click.Add(fun _ -> win.Close())
            base.Window.DataContext <- model

        member this.Show() = Application().Run(this.Window)
