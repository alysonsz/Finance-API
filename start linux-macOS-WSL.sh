#!/bin/bash
echo "Iniciando Finance.API..."
dotnet run --project Finance.API &

echo "Iniciando Finance.Web..."
dotnet run --project Finance.Web &
