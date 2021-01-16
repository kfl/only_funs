module ObjExpr

[<AbstractClass>]
type Expr() =
    abstract member Eval : int -> int

type VarX() =
    inherit Expr()
    override __.Eval(valx) = valx
    override __.ToString() = "X"

type Const(n : int) =
    inherit Expr()
    override __.Eval(_) = n
    override __.ToString() = string n

type Add(left : Expr, right: Expr) =
    inherit Expr()
    override __.Eval(valx) =
        left.Eval(valx) + right.Eval(valx)
    override __.ToString() =
        sprintf "(%s + %s)" (left.ToString()) (right.ToString())

let e : Expr = Add(Add(VarX(), (Const(42))), VarX()) :> Expr

type Mul(left : Expr, right : Expr) =
    inherit Expr()
    override __.Eval(valx) =
        left.Eval(valx) * right.Eval(valx)
    override __.ToString() =
        sprintf "(%s * %s)" (left.ToString()) (right.ToString())
