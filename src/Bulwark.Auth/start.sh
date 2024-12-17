#!/bin/bash
cd "$(dirname "$0")" # Change to the directory of the script (or binary)
exec dotnet Bulwark.Auth.dll  "$@"
