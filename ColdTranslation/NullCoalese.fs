module ColdTranslation.NullCoalese

open System

//source : https://gist.github.com/jbtule/8477768
//inspired by http://stackoverflow.com/a/2812306/637783

type NullCoalesce =  
    static member Coalesce(a: 'a option, b: 'a Lazy) = match a with Some a -> a | _ -> b.Value
    static member Coalesce(a: 'a Nullable, b: 'a Lazy) = if a.HasValue then a.Value else b.Value
    static member Coalesce(a: 'a when 'a:null, b: 'a Lazy) = match a with null -> b.Value | _ -> a

let inline nullCoalesceHelper< ^t, ^a, ^b, ^c when (^t or ^a) : (static member Coalesce : ^a * ^b -> ^c)> a b = 
                                            ((^t or ^a) : (static member Coalesce : ^a * ^b -> ^c) (a, b))

let inline (|??) a b = nullCoalesceHelper<NullCoalesce, _, _, _> a b

