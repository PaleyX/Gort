# Gort

A simple programming language to test PaleyExpressions.

Introduction
------------

Commands
--------
|Command|Description|Example|
|-------|-----------|-----------|
| { \<expression>| The equivalent of an if statement | { a > 5 } |
| } | The equivalent of an endif statement | } |
| { | The equivalent of an else statement | { |
| [ \<expression> | The equivalent of a while statement | [ a < 10 |
| [ | start an infinite loop | [ |
| ] | end loop | ] |
| ! | break out of a loop | ! |
| ! \<expression> | break out of a loop if the expression is true | ! a > 5 |
| : \<expression>| display the result of the expression | : a + 5 |
| \<identifier> \<expression>|Assign the result of the expression to the identifier | a 5 + 3 |
|#|Comment|# This is a comment|

Examples
--------
Count the number of set bits in a number:
```csharp
# set n to the value you want counted

c 0
[ n > 0 
   c c + (n & 1)
   n n >> 1
]
: c
```