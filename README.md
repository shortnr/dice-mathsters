# 🎲 Dice Mathsters

A multiplayer math game platform for K–12 classrooms, where students 
roll polyhedral dice and build mathematical expressions to hit a target 
number. Designed with Chromebook-friendly deployment in mind.

> **Status:** Core domain library complete. Game loop and multiplayer 
> networking in active development.

---

## ✨ Features

- **Recursive descent expression parser** — hand-written tokenizer and 
  AST builder supporting addition, subtraction, multiplication, division,
  exponentiation, negation, and implicit multiplication
- **Configurable validation engine** — flexible rules for dice usage 
  (must use all dice, max uses per die, subset allowed)
- **Difficulty profiles** — swappable profiles controlling target number 
  generation via geometric mean scaling
- **EaseOutSine scoring** — near-misses earn meaningful points; scores 
  range from 1–100
- **Server-authoritative design** — dice rolls and target numbers 
  generated server-side; clients reconstruct state from received values
- **Deterministic testing** — `IRandomProvider` abstraction allows fully 
  deterministic dice roll tests without mocking frameworks

---

## 🏗️ Architecture

### Current Repository Structure
```
src/
├── DiceMathsters.Domain/        # Core game logic, no dependencies
└── DiceMathsters.Application/   # Handlers (Facade pattern)
tests/
└── DiceMathsters.Tests/         # xUnit test suite
tools/
└── DiceMathsters.TestBench/     # Console playground for functional tests
```

### Intended Layered Architecture

The project is being built toward a clean layered architecture:

| Layer | Description | Status |
|---|---|---|
| **Domain** | Core game logic, no external dependencies | ✅ Complete |
| **Application** | Handlers and use cases | ✅ Complete |
| **Infrastructure/Services** | Networking, persistence | 🔲 Planned |
| **UI** | Student and teacher interfaces | 🔲 Planned |

### Design Patterns in Use

| Pattern | Where |
|---|---|
| **Facade** | Application handlers simplify access to domain subsystems |
| **Strategy** | Swappable `DifficultyProfile` instances |
| **Pipeline / Chain of Responsibility** | Tokenizer → Parser → Evaluator |
| **Composite** | Expression tree (`BinaryMathExpression`, `UnaryMathExpression`, `NegatedMathExpression`) |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

Tests also run automatically after each build if using 
xUnit's Visual Studio runner.

---

## ✅ CI

GitHub Actions runs a full build and test suite on every push to `master`.

---

## 🗺️ Roadmap

- [ ] Game loop (round management, turn timing, player scoring)
- [ ] Multiplayer networking — targeting web-based deployment for 
      Chromebook compatibility (likely WebSockets or Blazor)
- [ ] Teacher dashboard and session configuration
- [ ] Student-facing UI

---

## 📄 License

This project is licensed under the MIT License. See [Creative Commons Attribution-NonCommercial 4.0 International License](https://creativecommons.org/licenses/by-nc/4.0/).
© 2026 Nickolas Short — free to view and adapt for non-commercial purposes.