module FsWpfPlot.Models

open System.ComponentModel

type ColorCoding = ByLights | ByGradientY

type SurfacePlotModel() = 
    let propertyChangedHandler = new Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged: IEvent<PropertyChangedEventHandler,PropertyChangedEventArgs> = 
            propertyChangedHandler.Publish
    member this.raisePropertyChanged prop = propertyChangedHandler.Trigger(this, PropertyChangedEventArgs prop)
    
    member this.x = ()

    member val MinX = 0. with get,set
    member val MinY = 0. with get,set
    member val MaxX = 5. with get,set
    member val MaxY = 5. with get,set



    