What are we trying to make
==========================

https://github.com/kfl/only_funs

```
40 + 2     ~~> 42
2*20 + 2   ~~> 42
x * 20 + x ~~> 42  [ x -> 2 ]

(if (x + 1) then x + 2 else 3) + 40 ~~> 43 [ x -> -1 ]
                                    ~~> 44 [ x -> 2 ]

x + y ~~> 42 [ x -> 2; y -> 40 ]

(if (let x = 34 in x + x) then 69 else 420 + 1) ~~> 69
```

Parse
=====

```
parse "x + 0"               ~~> Add(Var "x", Const 0)
parse "x      +          0" ~~> Add(Var "x", Const 0)
parse "x + 0 * 5"           ~~> Add(Var "x", Mul(Const 0, Const 5))
parse "x + (0 * 5)"         ~~> Add(Var "x", Mul(Const 0, Const 5))
parse "x + ((0 * (5)))"     ~~> Add(Var "x", Mul(Const 0, Const 5))
parse "(x + 0) * 5"         ~~> Mul(Add(Var "x", Const 0), Const 5)

scan "(x + 0 )  * 5"  ~~> [ LPar; Id "x"; Plus; Num 0; RPar; Mult; Num 5 ]
scan "(x + 0)*5"      ~~> [ LPar; Id "x"; Plus; Num 0; RPar; Mult; Num 5 ]

```

Grammar
-------

```
E  ::= Num
     | Id
     | E '+' E
     | E '-' E
     | E '*' E
     | E '/' E
     | '(' E ')'
     | 'if' E 'then' E 'else' E
     | 'let' Id '=' E 'in' E
```

```
P    ::= 'if' E 'then' E 'else' E
       | 'let' Id '=' E 'in' E
       | E
E    ::= E '+' T | E '-' T | T
T    ::= T '*' F | T '/' F | F
F    ::= Num | Id | '(' P ')'
```

```
P     ::= 'if' E 'then' E 'else' E
        | 'let' Id '=' E 'in' E
        | E
E     ::=  T  Eopt
Eopt  ::=  '+' T Eopt | '-' T Eopt | ε
T     ::=  F Topt
Topt  ::=  '*' F Topt | '/' F Topt | ε
F     ::=  Num | Id | '(' P ')'
```


Future
======

* Finish mini-lang
* Make a parser
* Make compiler
* Intro Rust, C#
* Another domain than prog lang

* Invite some guests
* Hearthstone
