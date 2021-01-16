module Expr

type expr =
    | Var of string
    | Const of int
    | Add of expr * expr
    | Mul of expr * expr
    | Div of expr * expr
    | Cond of expr * expr * expr


let rec eval env expr =
    match expr with
        | Var x            -> Map.find x env
        | Const n          -> n
        | Add(left, right) -> eval env left + eval env right
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
        | Mul(left, right) -> sprintf "(%s * %s)" (toString left) (toString right)
        | Div(left, right) -> sprintf "(%s / %s)" (toString left) (toString right)
        | Cond(test, positive, negative) ->
            sprintf "(if %s then %s else %s)" (toString test)
                                              (toString positive) (toString negative)

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
