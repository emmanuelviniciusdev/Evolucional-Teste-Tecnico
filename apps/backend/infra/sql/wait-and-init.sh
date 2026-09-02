#!/usr/bin/env bash
set -euo pipefail

SA_PASSWORD="${SA_PASSWORD:-${MSSQL_SA_PASSWORD:-Escola_Dev_P@ssw0rd}}"
SQL_HOST="${SQL_HOST:-sqlserver}"
MARKER="/initialized/done"
SQLCMD18="/opt/mssql-tools18/bin/sqlcmd"
SQLCMD="/opt/mssql-tools/bin/sqlcmd"

run_sqlcmd() {
  if [ -x "$SQLCMD18" ]; then
    "$SQLCMD18" -S "$SQL_HOST" -U sa -P "$SA_PASSWORD" -C "$@"
  else
    "$SQLCMD" -S "$SQL_HOST" -U sa -P "$SA_PASSWORD" "$@"
  fi
}

if [ -f "$MARKER" ]; then
  echo "Database already initialized; skipping."
  exit 0
fi

echo "Waiting for SQL Server at ${SQL_HOST}..."
until run_sqlcmd -Q "SELECT 1" >/dev/null 2>&1; do
  echo "SQL Server is unavailable - sleeping"
  sleep 3
done

echo "Applying /init.sql..."
run_sqlcmd -i /init.sql

mkdir -p /initialized
touch "$MARKER"
echo "Database initialized."
