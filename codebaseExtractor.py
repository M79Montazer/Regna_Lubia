import os
from pathlib import Path
import pathspec

ROOT = Path(".").resolve()
OUTPUT_FILE = "codebase_dump.txt"

# -------------------------
# load .gitignore rules
# -------------------------
gitignore = ROOT / ".gitignore"

spec = None
if gitignore.exists():
    with open(gitignore) as f:
        spec = pathspec.PathSpec.from_lines("gitwildmatch", f)
else:
    spec = pathspec.PathSpec.from_lines("gitwildmatch", [])

def is_ignored(path: Path):
    rel = path.relative_to(ROOT).as_posix()
    return spec.match_file(rel)

# -------------------------
# collect cs files
# -------------------------
cs_files = []

for root, dirs, files in os.walk(ROOT):

    root_path = Path(root)

    # filter ignored dirs
    dirs[:] = [
        d for d in dirs
        if not is_ignored(root_path / d)
    ]

    for f in files:
        p = root_path / f

        if is_ignored(p):
            continue

        if p.suffix == ".cs":
            cs_files.append(p)

cs_files = sorted(cs_files)

# -------------------------
# compute folders that contain cs
# -------------------------
valid_dirs = set()

for f in cs_files:
    p = f.parent
    while True:
        valid_dirs.add(p)
        if p == ROOT:
            break
        p = p.parent

# -------------------------
# build tree
# -------------------------
def build_tree():
    lines = []

    for root, dirs, files in os.walk(ROOT):

        root_path = Path(root)

        if root_path not in valid_dirs:
            continue

        level = len(root_path.relative_to(ROOT).parts)
        indent = "  " * level

        name = root_path.name if root_path != ROOT else ROOT.name
        lines.append(f"{indent}{name}/")

        for f in sorted(files):
            p = root_path / f
            if p in cs_files:
                lines.append(f"{indent}  {f}")

    return "\n".join(lines)

# -------------------------
# write output
# -------------------------
with open(OUTPUT_FILE, "w", encoding="utf-8") as out:

    out.write("===== PROJECT STRUCTURE =====\n\n")
    out.write(build_tree())
    out.write("\n\n")

    for file in cs_files:

        rel = file.relative_to(ROOT)

        out.write("\n" + "="*80 + "\n")
        out.write(f"FILE: {rel}\n")
        out.write("="*80 + "\n\n")

        try:
            with open(file, "r", encoding="utf-8") as f:
                out.write(f.read())
        except UnicodeDecodeError:
            with open(file, "r", encoding="latin-1") as f:
                out.write(f.read())

        out.write("\n")

print(f"✅ Done. {len(cs_files)} files written to {OUTPUT_FILE}")
