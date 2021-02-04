module Parse

open Ast

type Token = Id of string
           | Num of int
           | LPar | RPar | Plus | Mult | Divide | Minus
           | Equal
           | IF | THEN | ELSE
           | LET | IN

exception Scanerror

let isblank c = System.Char.IsWhiteSpace c
let isdigit c = System.Char.IsDigit c
let isletter c = System.Char.IsLetter c
let isLetterDigit c = System.Char.IsLetterOrDigit c

let rec scanid cs acc =
    match cs with
        | c :: cs when isLetterDigit c -> scanid cs (acc + c.ToString())
        | _ -> (match acc with
                    | "if"   -> IF
                    | "then" -> THEN
                    | "else" -> ELSE
                    | "let"  -> LET
                    | "in"   -> IN
                    | _      -> Id acc
                , cs)

let rec scanint cs acc =
    match cs with
        | c :: cs when isdigit c -> scanint cs (acc + c.ToString())
        | _ -> (int acc |> Num, cs)

let scan s =
    let rec sc cs =
        match cs with
            | []        -> []
            | '+' :: cs -> Plus :: sc cs
            | '-' :: cs -> Minus :: sc cs
            | '*' :: cs -> Mult :: sc cs
            | '/' :: cs -> Divide :: sc cs
            | '=' :: cs -> Equal :: sc cs
            | '(' :: cs -> LPar :: sc cs
            | ')' :: cs -> RPar :: sc cs
            |   c :: cs when isdigit c ->
                let (num, cs1) = scanint cs (c.ToString())
                num :: sc cs1
            |   c :: cs when isletter c ->
                let (ident, cs1) = scanid cs (c.ToString())
                ident :: sc cs1
            |   c :: cs when isblank c -> sc cs
            | _ -> raise Scanerror
    s |> List.ofSeq |> sc


(*
P     ::= 'if' E 'then' E 'else' E
        | 'let' Id '=' E 'in' E
        | E
E     ::=  T  Eopt
Eopt  ::=  '+' T Eopt | '-' T Eopt | ε
T     ::=  F Topt
Topt  ::=  '*' F Topt | '/' F Topt | ε
F     ::=  Num | Id | '(' P ')'
*)

exception Parseerror

let rec P ts =
    match ts with
        | IF :: ts ->
            // E 'then' E 'else' E
            let (e, ts1) = E ts
            let ts2 = match ts1 with
                          | THEN :: ts -> ts
                          | _          -> raise Parseerror
            let (pos, ts3) = E ts2
            let ts4 = match ts3 with
                          | ELSE :: ts -> ts
                          | _          -> raise Parseerror
            let (neg, ts5) = E ts4
            (Cond(e, pos, neg), ts5)
        | LET :: ts ->
            // Id '=' E 'in' E
            let (ident, ts1) = match ts with
                                  | Id i :: Equal :: ts -> (i, ts)
                                  | _          -> raise Parseerror
            let (bound, ts2) = E ts1
            let ts3 = match ts2 with
                          | IN :: ts -> ts
                          | _        -> raise Parseerror
            let (body, ts4) = E ts3
            (Let(ident, bound, body), ts4)
        | _ -> E ts

and E ts =
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
            let (e, ts1) = P ts
            match ts1 with
                | RPar :: ts2 -> (e, ts2)
                | _ -> raise Parseerror
        | _ -> raise Parseerror

let parse s =
    match s |> scan |> P with
        | (ast, []) -> ast
        | _ -> raise Parseerror
