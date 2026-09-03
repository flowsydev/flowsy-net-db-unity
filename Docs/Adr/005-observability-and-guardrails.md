# ADR 005: Observability And Guardrails

**Status:** Accepted.

## Context

Applications need operational diagnostics and opt-in safeguards without exposing sensitive data or turning the library into a SQL parser.

## Decision

The library publishes an `ActivitySource` and `Meter` with stable tags that omit SQL and parameters. Connections can define a slow-operation threshold and require transactions for detected writes. The detector is replaceable.

## Consequences

Write detection is deliberately conservative and supports explicit administrative exceptions. Applications can connect the instrumentation to OpenTelemetry without adding a direct dependency to the library.
