module Ast

type expr =
    | Var of string
    | Const of int
    | Add of expr * expr
    | Sub of expr * expr
    | Mul of expr * expr
    | Div of expr * expr
    | Cond of expr * expr * expr
    | Let of string * expr * expr
