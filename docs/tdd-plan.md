# TDD Plan

## Goal

Use tests to protect the current behavior before refactoring, then add each requested feature behind failing tests.

## Phase 1: characterize current behavior

Add tests for the exact sample from the PDF:

- same route;
- same aircraft;
- same passenger commands represented as domain objects;
- same summary report text.

This gives a regression harness before changing `ScheduledFlight`.

## Phase 2: isolate behavior with focused tests

Add smaller tests for:

- general passengers paying full fare and receiving one bag;
- loyalty members paying cash gaining points and receiving two bags;
- loyalty members redeeming points reducing revenue and increasing redeemed points;
- airline employees flying free and receiving one bag;
- default rule rejection when a flight is unprofitable;
- default rule rejection when a flight exceeds capacity;
- default rule rejection when a flight is below the minimum occupancy.

## Phase 3: refactor

Refactor only while tests are green:

- separate summary calculation from report formatting;
- separate proceed-rule decisions from report creation;
- avoid adding new passenger behavior directly into a large report-building method.

## Phase 4: add features test-first

Add failing tests before implementation for:

- discounted passenger revenue, baggage, and summary count;
- default versus relaxed rule behavior;
- alternative aircraft included when the scheduled aircraft cannot proceed;
- no alternative aircraft section when none are suitable.

## Phase 5: console smoke tests

After domain tests pass, update the console demo and manually verify:

- `add discounted Name Age`;
- rule switching;
- existing sample commands.

