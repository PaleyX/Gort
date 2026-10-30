# Test suite review

## Current status

- The suite contains 70 tests, all of which passed during this review.
- The strongest coverage is for basic command execution, `if`/`else`, loop breaks, assignment, output, structural mismatches, identifier validation, source preprocessing, the prime program, bit counting, and reverse bits.
- The suite is primarily integration-oriented: most tests execute source text through `Interpreter`, which is appropriate for validating the language surface.
- `PaleyExpressions` supplies expression parsing and evaluation. Its operators should not be exhaustively retested here; Gort-owned integration and error handling should be tested instead.

## Recommended missing tests

### High priority

#### 1. Runtime guard type errors

Add tests for each command that requires a Boolean expression:

- `if` with a numeric or string guard should throw `RunTimeException` with the correct line number.
- A loop with a numeric or string guard should throw `RunTimeException` with the correct line number.
- A conditional break with a numeric or string guard should throw `RunTimeException` with the correct line number.

The current tests only cover valid Boolean guards. These branches are explicit in `CommandIf`, `CommandLoop`, and `CommandBreak`.

#### 2. Break validation and behavior

Add tests for:

- `!` outside a loop, which should fail at compile time.
- A conditional break whose condition is false, confirming that execution continues through the loop body.
- Multiple breaks in one loop.
- A break inside nested loops, confirming it exits only the nearest loop.
- A break inside an `if` within a nested loop.

The existing tests cover an unconditional break, a true conditional break, and one break inside an `if`, but not the false or nested cases.

#### 3. Malformed assignment and command input

Add compile-error tests for:

- An assignment with no expression, such as `x` or an explicit `$x` with no expression.
- An invalid assignment identifier, such as `9x 1`.
- An unknown command sigil.
- Superfluous text after `]`.
- Superfluous text after an `else` line.
- A source line that starts with a valid command character but has malformed syntax.

These paths are implemented in `Interpreter`, `CommandAssign`, `CommandEndLoop`, and `CommandElse`, but are currently untested.

#### 4. Conditional-control-flow combinations

Add tests for combinations that exercise jump-linking rather than only simple examples:

- Nested `if` blocks.
- Nested `if` blocks with an `else` at each level.
- An `else` after a true branch, confirming the else body is skipped.
- An `else` after a false branch, confirming the else body runs exactly once.
- An `if` immediately following a loop and a loop immediately following an `if`.
- Multiple sequential `if`/`else` blocks.

The existing else test covers the false branch only; it does not independently verify that a true branch skips the else body.

#### 5. Interpreter error and lifecycle behavior

Add tests for:

- `new Interpreter(null, ...)`, which should throw `ArgumentNullException`.
- Empty, whitespace-only, and comment-only programs, which should complete without commands.
- A source containing an unknown command and valid commands afterward, verifying compilation fails and does not execute a partially compiled program.
- Multiple compile errors, verifying the reported error count and output.
- A runtime `AssertException` through the `assert` function, verifying that `Interpreter` wraps it as `RunTimeException` and includes the command line number.
- A runtime exception other than `AssertException`, verifying its propagation behavior.

These are important because `Interpreter.Run` has separate compile and execution phases and special handling for `AssertException`.

#### 6. Prevent or specify empty-loop behavior

The implementation treats `[` with no guard specially: `CommandLoop.Execute` returns `Next`, while `CommandEndLoop.Execute` returns the loop command. This can create an infinite loop for an empty-guard loop such as:

```text
[
]
```

Add a test only after deciding the intended language behavior. The implementation should either reject an empty guard at compile time, define it as a valid finite construct, or document it as unsupported. Do not add a test that can hang the test runner without first adding a bounded execution mechanism.

### Medium priority

#### 7. Gort built-in functions

`Gort.Functions` has no direct tests. Add focused tests for:

- `substr` at the beginning, middle, and end of a string.
- `substr` with zero length and invalid ranges.
- `format` with no arguments, multiple arguments, and escaped/braced format text.
- `assert(true, ...)` and `assert(false, ...)`.
- `car` and `cdr` with empty, one-character, and multi-character strings.
- `len` with empty and non-empty strings.
- `sqrt` with zero, positive, and invalid/negative inputs as appropriate for the language.
- `cond` with the first predicate true, a later predicate true, and no predicate true.

These can be tested through `Interpreter` using `typeof(Gort.Functions)`, preserving the same integration path as production programs.

#### 8. Embedded program loading and sample programs

`PrimeTests` is the only test that loads an embedded program. Add tests for:

- Every supported sample that is intended to be executable, especially `BitCount`, `ReverseBits`, `FizzBuzz`, `IsPalindrome`, `ReverseString`, `Longest`, `First`, `Million`, and `Billion` where runtime cost is acceptable.
- A missing or whitespace-padded resource name passed to `GetEmbeddedProgram`.
- A valid resource returning non-empty source text.
- The error path for a missing resource, including the returned empty string and error output if that behavior is part of the contract.

For long-running samples, separate smoke tests from performance tests so normal test runs remain fast.

#### 9. Assignment and display edge cases

Add tests for:

- Reassigning an existing variable.
- Assigning `nil` and displaying it as `nil`.
- Displaying strings, booleans, and expressions.
- Displaying multiple values and verifying newline behavior.
- A missing variable or invalid expression in an assignment/display command.
- Default output behavior when no output callback is supplied, if that public behavior is supported.

`CommandAssign` and `CommandDisplay` currently have only basic successful cases.

#### 10. Source preprocessing edge cases

Extend `ExtensionsTests` and interpreter integration tests for:

- Numeric underscores next to decimal points, signs, and hexadecimal digits.
- Underscores inside both single-quoted and double-quoted strings.
- Escaped quotes inside strings.
- `#` in strings containing escaped quotes.
- Comments on lines containing commands.
- CRLF and LF source text, since `Interpreter` splits using `Environment.NewLine`.
- Leading/trailing whitespace and blank lines around control structures.

The current preprocessing tests are useful but do not cover escaped delimiters or platform newline differences.

#### 11. Command discovery and sigil handling

`MiscTests` verifies `Tools.GetSigil`, but not the actual command lookup path. Add integration tests for:

- Every supported sigil being recognized by `Interpreter`.
- An unknown sigil producing the expected compile error.
- An unadorned valid identifier being converted to assignment.
- A line beginning with a non-command identifier but lacking the required separator not being accidentally treated as assignment.

### Lower priority

#### 12. Numeric and language-boundary cases

Add cases for:

- Negative numbers.
- Fractional values in arithmetic and comparisons.
- Division by zero behavior.
- Modulus with zero and negative operands.
- Large exact 32-bit values in bit operations.
- `nil` comparisons and Boolean equality.

Keep expression-operator tests focused on Gort integration because the operators are implemented by `PaleyExpressions`.

#### 13. Test organization and isolation

Consider separating the suite into:

- `InterpreterCommandTests` for language command integration.
- `InterpreterErrorTests` for compile/runtime failures.
- `FunctionTests` for `Gort.Functions`.
- `ProgramTests` for embedded sample programs.
- `ExtensionsTests` for source preprocessing.

Use a fresh variable dictionary per test rather than a mutable class field where practical. xUnit normally creates a new test-class instance per test, but local setup makes isolation explicit.

## Existing test-quality observations

- `ReverseBitsTests` now correctly asserts `variables["r"]`; `n` is consumed by the right shifts and ends at zero.
- The reverse-bits reference implementation is independent enough to be useful and covers representative 32-bit patterns.
- `PrimeProgramWorks` is valuable end-to-end coverage, but its large output comparison can make failures difficult to diagnose. Add smaller focused prime cases or compare parsed results when diagnosing individual defects.
- Exact diagnostic-string assertions protect user-visible messages, but they are brittle if wording changes. Consider asserting exception type, key message content, and line number separately unless the complete text is part of the contract.
- Several tests assert `sb.Length == 0`; `Assert.Empty(sb.ToString())` or equivalent intent-focused assertions may be clearer.
- There are no coverage thresholds or coverage reports visible in the test project despite the coverlet collector reference. Adding a CI coverage report and threshold would help prevent regressions in the uncovered branches above.

## Suggested implementation order

1. Add runtime type-error and break-validation tests.
2. Add malformed-input and interpreter error tests.
3. Add nested control-flow tests.
4. Add direct/integration tests for `Gort.Functions`.
5. Add embedded-program loading and sample smoke tests.
6. Expand preprocessing and numeric boundary tests.
7. Decide and test the contract for empty loops.
