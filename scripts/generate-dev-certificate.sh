#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
CERT_DIR="$ROOT_DIR/certificates"
PFX_PATH="$CERT_DIR/misssolitude.pfx"
KEY_PATH="$CERT_DIR/misssolitude.key"
CRT_PATH="$CERT_DIR/misssolitude.crt"

PASSWORD="${ASPNETCORE_Kestrel__Certificates__Default__Password:-${PFX_PASSWORD:-changeit}}"

if ! command -v openssl >/dev/null 2>&1; then
  echo "OpenSSL is required to generate the development certificate." >&2
  exit 1
fi

mkdir -p "$CERT_DIR"

if [ -f "$PFX_PATH" ]; then
  echo "Certificate already exists at $PFX_PATH"
  exit 0
fi

if [ -f "$KEY_PATH" ] || [ -f "$CRT_PATH" ]; then
  if [ -f "$KEY_PATH" ] && [ -f "$CRT_PATH" ]; then
    echo "Found existing key and certificate. Exporting PFX..."
    openssl pkcs12 -export \
      -out "$PFX_PATH" \
      -inkey "$KEY_PATH" \
      -in "$CRT_PATH" \
      -passout "pass:$PASSWORD"
    rm "$KEY_PATH" "$CRT_PATH"
    echo "Created $PFX_PATH with password '$PASSWORD'."
    exit 0
  else
    echo "Partial certificate material found (key or cert missing). Remove leftovers in $CERT_DIR or supply the missing file." >&2
    exit 1
  fi
fi

echo "Generating self-signed development certificate for localhost..."

export MSYS2_ARG_CONV_EXCL='*'

openssl req -x509 -newkey rsa:2048 -sha256 -days 365 -nodes \
  -keyout "$KEY_PATH" \
  -out "$CRT_PATH" \
  -subj "/CN=localhost" \
  -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"

openssl pkcs12 -export \
  -out "$PFX_PATH" \
  -inkey "$KEY_PATH" \
  -in "$CRT_PATH" \
  -passout "pass:$PASSWORD"

rm "$KEY_PATH" "$CRT_PATH"

echo "Created $PFX_PATH with password '$PASSWORD'."