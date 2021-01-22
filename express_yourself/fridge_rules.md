Rule of form 0
--------------

The parsing function for a rule of form `A ::= f1` is

```
let rec A ts = parse code for f1
```

Rule of form 1
--------------

The parsing function for a rule of form `A ::= f1 | ... | fN` is

```
let rec A ts =
  match ts with
   | t11 :: tr  -> parse code for alternative f1
                  ...
   | t1a :: tr  -> parse code for alternative f1
                  ...
   | tN1 :: tr  -> parse code for alternative fN
                  ...
   | tNb :: tr  -> parse code for alternative fn
   | _          -> raise Parseerror
```

Rule of form 2
--------------

The parsing function for a rule of form `A = f1 | ... | fN | ε` is

```
let rec A ts =
  match ts with
   | t11 :: tr  -> parse code for alternative f1
                  ...
   | t1a :: tr  -> parse code for alternative f1
                  ...
   | tN1 :: tr  -> parse code for alternative fN
                  ...
   | tNb :: tr  -> parse code for alternative fn
   | _          -> ts
```

Sequence
--------

The parse code for an alternative `f` which is a sequence
`e1 e2 ... eM` is

```
let ts1 = parse code for e1 given ts
let ts2 = parse code for e2 given ts1
  ...
let tsM = parse code for eM given ts(M-1)
```

Nonterminal
-----------

The parse code for a nonterminal `A` is a call `A(ts)` to its parsing
paring function.


Token
-----

The parse code for a token `"c"` depends on its position `eJ` in the sequence
`e1 ... eM`.

If the tOKEN is *not* the first symbol `e1`, then we must
check that `c` is the current symbol in the token list `ts`,
and, if so, read the next token

If the terminal *is* the first symbol `e1`, then this check
has already been made by the `match` code for alternatives, so we
just need to advance to the next token
