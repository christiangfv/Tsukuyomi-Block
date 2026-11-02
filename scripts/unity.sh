#!/usr/bin/env bash
set -euo pipefail
repo_dir="$(cd "$(dirname "$0")/.." && pwd)"
project_dir="$repo_dir/Tsukuyomi-Block"
unity_version="$(awk '/^m_EditorVersion:/ {print $2}' "$project_dir/ProjectSettings/ProjectVersion.txt")"
editor_path="${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/$unity_version/Unity.app/Contents/MacOS/Unity}"
if [[ ! -x "$editor_path" ]]; then
  echo "Install Unity $unity_version in Unity Hub, or set UNITY_EDITOR to its executable." >&2
  exit 1
fi
export TMPDIR="${TSUKUYOMI_TMPDIR:-/tmp}"
cd "$project_dir"
exec "$editor_path" -projectPath "$project_dir" "$@"
