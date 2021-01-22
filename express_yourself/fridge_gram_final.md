```
E    ::= E "+" T | E "-" T | T
T    ::= T "*" F | T "/" F | F
F    ::= Num | "(" E ")"
```

```
E     ::=  T  Eopt
Eopt  ::=  "+" T Eopt | "-" T Eopt | ε
T     ::=  F Topt
Topt  ::=  "*" F Topt | "/" F Topt | ε
F     ::=  Num | "(" E ")"
```
