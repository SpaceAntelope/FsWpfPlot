namespace FsWpfPlot.Examples.Datasets

open FSharp.Data
open Newtonsoft.Json

module Wb =

    type ExampleCsv = CsvProvider<Schema="Index(int),Country(string),Surface(float option),Pop2000(float option),Pop2010(float option),Pollution(float option),EducationM(float option),EducationF(float option)", HasHeaders = false>

    type ExampleData = {
        Index: int
        Country : string
        Surface: float option
        Pop2000: float option
        Pop2010: float option   
        Pollution: float option
        EducationM: float option
        EducationF: float option
    }

    let GenerateExampleData() = 
        let NanToOpt num = if System.Double.IsNaN num then None else Some(num)
        let mutable cnt = 0
        let inc() = cnt <- cnt + 1; cnt
        [| for country in WorldBankData.GetDataContext().Countries -> {
                    Index = inc()
                    Country = country.Name
                    Surface = country.Indicators.``Surface area (sq. km)``.[2000]  |> NanToOpt
                    Pop2000 = country.Indicators.``Population, total``.[2000] |> NanToOpt
                    Pop2010 = country.Indicators.``Population, total``.[2010] |> NanToOpt
                    Pollution = country.Indicators.``CO2 emissions (kt)``.[2000] |> NanToOpt
                    EducationM = country.Indicators.``School enrollment, secondary, male (% gross)`` .[2000] |> NanToOpt
                    EducationF = country.Indicators.``School enrollment, secondary, female (% gross)`` .[2000] |> NanToOpt
        }|]

    let storeDataToCache (data : ExampleData[]) =
        data 
        |> JsonConvert.SerializeObject 
        |> fun str -> System.IO.File.WriteAllText("wb.data.json", str)

        data 
        |> Seq.map (fun item -> ExampleCsv.Row(item.Index, item.Country, item.Surface, item.Pop2000, item.Pop2010, item.Pollution, item.EducationM, item.EducationF))
        |> fun rows -> 
            System.IO.File.AppendAllText("wb.data.csv", ExampleCsv(rows).SaveToString())

    let GetExampleDataFromCache() =
        System.IO.Directory.GetFiles(__SOURCE_DIRECTORY__ ,"wb.data.*")
        |> List.ofArray
        |> function 
        | [] -> 
            let data = GenerateExampleData()
            data |> storeDataToCache
            data
        | filename::_ ->            
            match System.IO.Path.GetExtension(filename) with 
            | ".csv"-> 
                    filename
                    |> ExampleCsv.Load
                    |> fun csv -> csv.Rows
                    |> Seq.map (fun row ->
                            {
                                Index = row.Index
                                Country = row.Country
                                Surface = row.Surface
                                Pop2000 = row.Pop2000
                                Pop2010 = row.Pop2010
                                Pollution = row.Pollution
                                EducationM = row.EducationM
                                EducationF = row.EducationF
                            })
                    |> Array.ofSeq
            | ".json" -> 
                    filename
                    |> System.IO.File.ReadAllText
                    |> JsonConvert.DeserializeObject<ExampleData[]>
            | ext -> failwith ("I do not know what to do with " +  ext)

    let GetExampleData() = GetExampleDataFromCache()

    

