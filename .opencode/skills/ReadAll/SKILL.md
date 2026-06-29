---
name: dump-codebase
description: Generate a flattened repository dump for project. Use this skill before repository-wide analysis, architecture explanations, cross-file debugging, or large refactors that require understanding the entire codebase.
---

# Dump Codebase

This skill creates a single flattened representation of the repository with:

- directory structure
- source code
- line numbers
- file metadata

## When to use

Use this skill when:

- explaining repository architecture
- answering questions involving many files
- searching for bugs spanning multiple files
- performing repository-wide refactoring
- understanding an unfamiliar codebase

## Procedure

1. Determine the primary language(s) in the repository.

Typical choices:

- Python → `.py`
- C# → `.cs`
- TypeScript → `.ts .tsx`
- JavaScript → `.js`

If multiple languages are important, include all relevant extensions.

If the user explicitly requests every file, use `*`.

2. Run:

```bash
python codebaseExtractor.py .py
```

or

```bash
python codebaseExtractor.py .cs
```

or

```bash
python codebaseExtractor.py "*" --output codebase_dump.txt
```

depending on the task.

3. Read the generated output (or capture stdout if no output file was specified).

4. Use the dump as the primary context for reasoning.

5. If files are modified, regenerate the dump before continuing repository-wide analysis.

## Notes

- The script respects `.gitignore`.
- Generated directories such as `bin`, `obj`, `node_modules`, `.venv`, and other ignored paths are automatically excluded.
- Each source line includes its original line number to simplify precise edits and references.