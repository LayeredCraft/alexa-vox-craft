# Architecture Decision Records

This directory contains Architecture Decision Records (ADRs) for the AlexaVoxCraft project.

## What is an ADR?

An Architecture Decision Record captures an important architectural decision made along with its context and consequences. ADRs help preserve institutional knowledge and provide context for future developers.

## Index

| ADR | Title | Status | Date |
|-----|-------|--------|------|
| [0000](0000-adr-template.md) | ADR Template | - | - |
| [0001](0001-multi-locale-interaction-model-builder.md) | Multi-Locale Interaction Model Builder | Accepted | 2026-03-25 |

## Creating a New ADR

1. Copy `0000-adr-template.md` to a new file with the next sequential number
2. Fill in all template sections
3. Update this README index
4. Submit for review via PR

## When to Write an ADR

Write an ADR when making decisions about:

- Significant technology choices (frameworks, databases, protocols)
- Architectural patterns (CQRS, event sourcing, microservices)
- Non-obvious decisions that future developers might question
- Decisions with long-term consequences
- Trade-offs between competing approaches

## ADR Lifecycle

- **Proposed**: Under discussion, not yet accepted
- **Accepted**: Decision has been made and implemented
- **Deprecated**: No longer applies but kept for historical context
- **Superseded**: Replaced by a newer ADR (link to replacement)

## Maintenance

- Review ADRs quarterly to mark deprecated or superseded records
- Include ADR review in PR checklist for architectural changes
- Reference relevant ADRs in code comments when helpful