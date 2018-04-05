namespace FsWpfPlot.Models

    open System.ComponentModel
    open System
    open System.Windows
    open System.Windows.Media.Media3D
    open FsWpfPlot.Types
    open FsWpfPlot.HelperFunctions
    
    type SurfacePlotModel() = 
        
        let propertyChangedHandler = new Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
        interface INotifyPropertyChanged with
            [<CLIEvent>]
            member this.PropertyChanged: IEvent<PropertyChangedEventHandler,PropertyChangedEventArgs> = 
                propertyChangedHandler.Publish
        member this.raisePropertyChanged prop = propertyChangedHandler.Trigger(this, PropertyChangedEventArgs prop)
    
        member val Granularity = 91 with get,set
        member val MinX = 0. with get,set
        member val MinY = 0. with get,set
        member val MaxX = 5. with get,set
        member val MaxY = 5. with get,set
        member val FuncZ = fun x y -> (sin x*y) * 0.5 with get,set

        member this.GetClosestPointByGranularity row col =            
            let x' = this.MinX + (float col) / (float this.Granularity - 1.) * (this.MaxX - this.MinX)
            let y' = this.MinY + (float row) / (float this.Granularity - 1.) * (this.MaxY - this.MaxY)

            Point(x',y')

        member this.DataFromFunction (f: (float->float->float)) =
            Array2D.init this.Granularity this.Granularity (fun row col ->
                    let point = this.GetClosestPointByGranularity row col
                    Point3D(point.X, point.Y, f point.X point.Y))
            
        member val Graphics = new SurfacePlotGraphicElementsModel() with get, set

        //member this.data = this.DataFromFunction this FuncZ
        member val Data = null with get, set

        member this.ColorValues 
            with get() =
                 match this.Graphics.ColorCoding with 
                | ByGradientY ->
                    findGradientY <| this.Data
                | ByLights -> 
                    null