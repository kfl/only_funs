module Expr

type expr =
    | Var of string
    | Const of int
    | Add of expr * expr
    | Sub of expr * expr
    | Mul of expr * expr
    | Div of expr * expr
    | Cond of expr * expr * expr


let rec eval env expr =
    match expr with
        | Var x            -> Map.find x env
        | Const n          -> n
        | Add(left, right) -> eval env left + eval env right
        | Sub(left, right) -> eval env left - eval env right
        | Mul(left, right) -> eval env left * eval env right
        | Div(left, right) ->
            let rhs = eval env right in
            if rhs = 0 then failwith "Divide by zero"
            else eval env left / rhs
        | Cond(test, positive, negative) ->
            if eval env test <> 0 then eval env positive
            else eval env negative

let rec toString expr =
    match expr with
        | Var x            -> x
        | Const n          -> string n
        | Add(left, right) -> sprintf "(%s + %s)" (toString left) (toString right)
        | Sub(left, right) -> sprintf "(%s - %s)" (toString left) (toString right)
        | Mul(left, right) -> sprintf "(%s * %s)" (toString left) (toString right)
        | Div(left, right) -> sprintf "(%s / %s)" (toString left) (toString right)
        | Cond(test, positive, negative) ->
            sprintf "(if %s then %s else %s)" (toString test)
                                              (toString positive) (toString negative)


type Token = Id of string
           | Num of int
           | LPar | RPar | Plus | Mult | Divide | Minus

exception Scanerror

let isblank c = System.Char.IsWhiteSpace c
let isdigit c = System.Char.IsDigit c
let digitToNum c = c.ToString() |> int |> Num
let isletter c = System.Char.IsLetter c
let letterToId c = c.ToString() |> Id


let scan s =
    let rec sc cs =
        match cs with
            | []        -> []
            | '+' :: cs -> Plus :: sc cs
            | '-' :: cs -> Minus :: sc cs
            | '*' :: cs -> Mult :: sc cs
            | '/' :: cs -> Divide :: sc cs
            | '(' :: cs -> LPar :: sc cs
            | ')' :: cs -> RPar :: sc cs
            |   c :: cs when isdigit c -> digitToNum c :: sc cs  // FIXME
            |   c :: cs when isletter c -> letterToId c :: sc cs // FIXME
            |   c :: cs when isblank c -> sc cs
            | _ -> raise Scanerror
    s |> List.ofSeq |> sc


(*
E     ::=  T  Eopt
Eopt  ::=  '+' T Eopt | '-' T Eopt | ε
T     ::=  F Topt
Topt  ::=  '*' F Topt | '/' F Topt | ε
F     ::=  Num | Id | '(' E ')'
*)

exception Parseerror

let rec E ts =
    let (t, ts1) = T ts
    let (eopt, ts2) = Eopt t ts1
    (eopt, ts2)

and Eopt t1 ts =
    match ts with
        | Plus :: ts ->
                let (t2, ts1) = T ts
                let (eopt, ts2) = Eopt (Add(t1, t2)) ts1
                (eopt, ts2)
        | Minus :: ts ->
                let (t2, ts1) = T ts
                let (eopt, ts2) = Eopt (Sub(t1, t2)) ts1
                (eopt, ts2)
        | _ -> (t1, ts)

and T ts =
    let (f, ts1) = F ts
    let (topt, ts2) = Topt f ts1
    (topt, ts2)

and Topt f1 ts =
    match ts with
        | Mult :: ts ->
                let (f2, ts1) = F ts
                let (topt, ts2) = Topt (Mul( f1, f2)) ts1
                (topt, ts2)
        | Divide :: ts ->
                let (f2, ts1) = F ts
                let (topt, ts2) = Topt (Div(f1,f2)) ts1
                (topt, ts2)
        | _ -> (f1, ts)
and F ts =
    match ts with
        | Num n :: ts -> (Const n, ts)
        | Id v :: ts -> (Var v, ts)
        | LPar :: ts ->
            let (e, ts1) = E ts
            match ts1 with
                | RPar :: ts2 -> (e, ts2)
                | _ -> raise Parseerror
        | _ -> raise Parseerror

let parse s =
    match s |> scan |> E with
        | (ast, []) -> ast
        | _ -> raise Parseerror


let ex1 : expr = Add(Add(Var "x", Const 42), Var "x") // (X + 42) + X
let ex2 = Add(Const 40, Const 2)                      // 40 + 2     ~~> 42
let ex3 = Mul(Const 2, Add(Const 20, Const 2))        // 2 * (20 + 2)   ~~> 44
let ex4 = Add(Mul(Var "x", Const 20), Var "x")        // (x * 20) + x ~~> 42 [ x -> 2]
let ex5 = Add(Cond(Add(Var "x", Const 1),
                   Add(Var "x", Const 2),
                   Const 3),
              Const 40) // (if (x + 1) then x + 2 else 3) + 40
let ex6 = Add(Cond(Var "x",
                   Div(Const 2, Var "x"),
                   Const 100),
              Const 40) // (if x then 2 / x else 100) + 40
let ex7 = Add(Var "x", Var "y")
