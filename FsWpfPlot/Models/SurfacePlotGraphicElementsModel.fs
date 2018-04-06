namespace FsWpfPlot.Models

    open System.ComponentModel
    open System.Windows.Media.Media3D
    open System.Windows.Media
    open HelixToolkit.Wpf
    open FsWpfPlot.Types
    open FsWpfPlot
    open FsWpfPlot

    type SurfacePlotGraphicElementsModel() = 
        let propertyChangedHandler = new Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
        interface INotifyPropertyChanged with
            [<CLIEvent>]
            member this.PropertyChanged: IEvent<PropertyChangedEventHandler,PropertyChangedEventArgs> = 
                propertyChangedHandler.Publish
        member this.raisePropertyChanged prop = propertyChangedHandler.Trigger(this, PropertyChangedEventArgs prop)         

        member val ColorCoding = ByLights with get,set

        member this.Lights 
            with get() =
                let group = Model3DGroup()
                match this.ColorCoding with 
                | ByGradientY ->
                    group.Children.Add(AmbientLight(Colors.White))
                | ByLights -> 
                    group.Children.Add(AmbientLight(Colors.Gray))
                    group.Children.Add(PointLight(Colors.Red, Point3D(0.,-1000.,0.)))
                    group.Children.Add(PointLight(Colors.Blue, Point3D(0.,0.,1000.)))
                    group.Children.Add(PointLight(Colors.Green, Point3D(1000.,1000.,0.)))
                group
        
        member this.SurfaceBrush 
            with get() =
                match this.ColorCoding with 
                | ByGradientY ->
                    BrushHelper.CreateGradientBrush(Colors.Red, Colors.White, Colors.Blue)
                    :> Brush
                | ByLights -> 
                    Brushes.White 
                    :> Brush

        //member this.ColorValues
        //    with get() =
        //        match this.ColorCoding with 
        //            BrushHelper.CreateGradientBrush(Colors.Red, Colors.White, Colors.Blue)
        //            :> Brush
        //        | ByLights -> 
        //            Brushes.White 
        //            :> Brush

        
    