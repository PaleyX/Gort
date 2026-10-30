# GortLang Code Review

## Validation

- Solution build: passed
- Unit tests: **53 passed, 0 failed**
- Existing tests cover assignments, conditionals, loops, breaks, comments, numeric separators, structural errors, and the prime-number sample.

## Findings

### High — Platform-dependent source parsing

**File:** `GortLang/Interpreter.cs:55`

```csharp
var lines = _source.Split(System.Environment.NewLine);
```

The interpreter only recognizes the host platform’s newline sequence. On Windows, source containing Unix-style `\n` line endings is treated as one logical line. Conversely, source with Windows `\r\n` may not be handled correctly if executed on another platform.

This affects embedded programs, API callers, copied source, and cross-platform execution.

**Recommendation:** Split using a platform-independent approach, such as:

```csharp
_source.Split(["\r\n", "\n", "\r"], StringSplitOptions.None)
```

Add tests for LF, CRLF, and CR input.

---

### Medium — Comment removal is inconsistent with string handling

**Files:**

- `GortLang/Extensions.cs:14`
- `GortLang/Extensions.cs:43`

`StripNumericUnderscores` treats both single- and double-quoted regions as strings:

```csharp
if (m.Value.StartsWith("\"") || m.Value.StartsWith("'"))
```

However, `RemoveComments` only protects double-quoted strings:

```csharp
[GeneratedRegex("""^((?:"(?:\\\\.|[^"\\\\\r\n])*"|[^"#\r\n])*)#.*$""")]
```

A single-quoted expression containing `#` will be truncated as a comment. For example, a supported or intended expression such as:

```plaintext
: 'a#b'
```

may be transformed before expression evaluation.

**Recommendation:** Use one shared quote-aware lexer/preprocessor for comments and numeric separators, or make both transformations support the same string syntax and escape rules.

---

### Medium — Numeric underscore preprocessing mishandles escaped quotes

**File:** `GortLang/Extensions.cs:40`

```csharp
[GeneratedRegex(@"([\""']).*?\1|(?<=\d)_(?=\d)")]
```

The quoted-string branch does not understand escaped quote characters. A string containing an escaped quote can terminate the regex match prematurely, causing underscores inside the remaining string content to be removed.

This conflicts with the comment regex, which explicitly supports escaped characters using `\\.`.

**Recommendation:** Replace the regex-based preprocessing with a small character scanner that tracks:

- Current quote type
- Escape state
- Whether the current character is inside a string

This will also make comment handling more reliable.

---

### Medium — Mismatched closing commands produce poor diagnostics

**Files:**

- `GortLang/Commands/CommandEndif.cs:12`
- `GortLang/Commands/CommandEndLoop.cs:19`

Both commands pop the stack before verifying that a matching construct exists:

```csharp
var command = Environment.Stack.Pop();
```

For a standalone `}` or `]`, this produces a generic stack exception such as `Stack empty.` instead of a language-level error like `If..Endif mismatch` or `Unexpected end loop`.

The existing tests cover mismatches where another construct is present, but not an empty stack.

**Recommendation:** Check `Environment.Stack.Count` before popping and produce a structured compile-time error that includes the source line.

---

### Medium — Runtime errors lack consistent source context

**File:** `GortLang/Interpreter.cs:31-44`

`RunProgram` only converts `AssertException` into a `RunTimeException` with line information. Exceptions from expression evaluation or user-provided functions are allowed to escape unchanged.

Consequences include:

- Inconsistent exception types
- Missing Gort source line numbers
- Third-party expression-engine details exposed directly to callers
- More difficult debugging for invalid operations, missing variables, bad function arguments, and conversion errors

**Recommendation:** Catch expression/runtime exceptions at command execution boundaries and wrap them in `RunTimeException` with the command line number and original exception as the inner exception. Preserve `AssertException` as a special case if desired.

---

## Test coverage gaps

Add regression tests for:

1. LF-only and CRLF source input
2. Empty-stack `}` and `]`
3. Single-quoted strings containing `#`
4. Escaped quotes containing numeric underscores
5. Runtime expression failures with expected line numbers
6. Invalid or empty display/assignment expressions
7. Running the same `Interpreter` instance more than once
8. Multiple compile errors in one source file

## Positive observations

- The command linking model is compact and works for the tested nested control-flow cases.
- Loop break registration correctly handles breaks nested inside conditionals and nested loops.
- The public `Interpreter` API is small and easy to consume.
- The project has useful end-to-end coverage through `PrimeTests`.
- Structural compilation errors are detected before execution in the tested scenarios.
- The current implementation builds cleanly on .NET 10.
