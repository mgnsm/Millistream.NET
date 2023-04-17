# C# Coding Style

Please follow these general guidelines when contributing code to Millistream.NET.

- Use `_camelCase` for internal and private fields and use `readonly` where possible. Prefix internal and private instance fields with `_`, static fields with `s_` and thread static fields with `t_`. When used on static fields, `readonly` should come after `static` (e.g. `static readonly` not `readonly static`).  Public fields should be used sparingly and should use PascalCasing with no prefix when used.
- Avoid `this.` unless absolutely necessary.
- Always specify the visibility, even if it's the default (e.g. `private string _foo` not `string _foo`). Visibility should be the first modifier (e.g. `public abstract` not `abstract public`).
- Specify namespace imports at the top of the file, *outside* of `namespace` declarations.
- Only use `var` when you have to. Use explicit type names whenever possible.
- Use the language keywords instead of the BCL types (e.g. `int, string, float` instead of `Int32, String, Single`, etc) for both type references as well as method calls (e.g. `int.Parse` instead of `Int32.Parse`).
- Use PascalCasing to name all constant local variables and fields. The only exception is for interop code where the constant value should exactly match the name and value of the code you are calling via interop.
- Use ```nameof(...)``` instead of ```"..."``` whenever possible and relevant.
- Specify fields at the top within type declarations.
- Use Unicode escape sequences (\uXXXX) instead of literal characters when including non-ASCII characters in the source code. Literal non-ASCII characters occasionally get garbled by a tool or editor.
- Use these conventions for single `if`-statements:
    - Never use single-line form (for example: `if (source == null) throw new ArgumentNullException("source");`)
    - Omit the braces