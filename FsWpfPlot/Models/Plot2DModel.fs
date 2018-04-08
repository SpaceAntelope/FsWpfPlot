namespace FsWpfPlot.Models

open System.ComponentModel
open System
open System.Windows
open System.Windows.Media.Media3D
open FsWpfPlot.Types
open FsWpfPlot.HelperFunctions
open System.Windows.Data
    

type Plot2DModel() = 
    // automatic get/setters won't cut it for dependency property binding
    let mutable func: ChartFunction option = Some(fun x -> (sin x) * 0.5) 
    let mutable data : Point[] = null
    let mutable plotKind : PlotKind = PlotKind.Surface
    let mutable colorCoding : ColorCoding = ColorCoding.ByGradientY

    let propertyChangedHandler = new Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged: IEvent<PropertyChangedEventHandler,PropertyChangedEventArgs> = 
            propertyChangedHandler.Publish
    member this.raisePropertyChanged prop = propertyChangedHandler.Trigger(this, PropertyChangedEventArgs prop)
    
    member val Resolution = 91 with get,set
    member val MaxX = 5. with get,set
    member val MinX = 0. with get,set
    member val MaxY = 5. with get,set
    member val MinY = 0. with get,set
    //member val MaxZ = 1. with get,set
    //member val MinZ = 0. with get,set
    member this.Func
        with get() = func
            and set(value) = 
                func<- value
                this.raisePropertyChanged "Func"
                this.raisePropertyChanged "Data"
        
    member this.setRangeX min max = this.MinX <- min ; this.MaxX <- max
    member this.getRangeX = this.MaxX - this.MinX
    //member this.setRangeY min max = this.MinY <- min ; this.MaxY <- max
    //member this.getRangeY = this.MaxY - this.MinY

    member val CameraPosition = Point3D(0.,0.,0.) with get, set

    member this.MapToResolution col =  
        this.MinX + (float col) / (float this.Resolution - 1.) * (this.MaxX - this.MinX)

    member this.Data 
        with get() = 
            match this.Func with
            | Some(func) -> 
                Array.init this.Resolution (fun col -> 
                    let x = this.MapToResolution col
                    Point(x, func x))
            | None -> data
        and set(value) =
            data <- value
            this.raisePropertyChanged "Data"
        
    member this.ColorCoding
        with get() = colorCoding
        and set(value) = 
            colorCoding <- value
            this.raisePropertyChanged "PlotKind"

    member this.PlotKind 
        with get() = plotKind
        and  set(value: PlotKind) = 
                plotKind <- value
                this.raisePropertyChanged "PlotKind"

    member val PlotTitle = "2D Plot" with get, set