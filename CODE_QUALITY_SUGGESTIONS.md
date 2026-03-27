# Modern.Forms — Code Quality & Refactoring Suggestions

> **Date:** 2026-03-27  
> **Scope:** Full codebase review of `Modern.Forms` (221 source files, ~30k LOC)  
> **Purpose:** Identify actionable code quality improvements. Each suggestion has a unique ID for reference.

---

## Table of Contents

1. [Testing](#1-testing)
2. [Error Handling](#2-error-handling)
3. [Async Patterns](#3-async-patterns)
4. [API Design & Public Surface](#4-api-design--public-surface)
5. [SOLID Principles & Architecture](#5-solid-principles--architecture)
6. [Resource Management & Memory](#6-resource-management--memory)
7. [Theme & Styling System](#7-theme--styling-system)
8. [Documentation & Maintainability](#8-documentation--maintainability)
9. [Accessibility](#9-accessibility)
10. [Cross-Platform & Build](#10-cross-platform--build)
11. [Summary Scorecard](#summary-scorecard)

---

## 1. Testing

### S-001: Dramatically Increase Unit Test Coverage

**Severity:** Critical  
**Files:** `tests/Modern.Forms.Tests/` (only 3 test files, ~12 tests total)

The entire test suite consists of `ApplicationTests.cs` (1 test), `ControlTests.cs` (~8 tests), and `ScrollableControlTests.cs` (3 tests). This covers a tiny fraction of the 221-file codebase. There are **no tests** for:

- Any concrete control (Button, TextBox, ListBox, TreeView, ComboBox, etc.)
- The rendering pipeline or any `Renderer` subclass
- The layout engines (FlowLayout, TableLayout, DockAndAnchorLayout)
- Event raising and propagation
- Theme system
- Dialogs (FileDialog, MessageBoxForm, etc.)
- Collections (ControlCollection, MenuItemCollection, ListBoxItemCollection)

**Suggestion:** Prioritize tests for the core `Control` lifecycle, layout engines, and at least the most commonly used controls (Button, TextBox, ListBox, ComboBox).

---

### S-002: Add Integration / Rendering Tests

**Severity:** Medium  
**Files:** N/A (none exist)

There are no integration tests verifying that controls render correctly, layout engines produce the right bounds, or that input events flow through the control hierarchy properly. Consider snapshot-based rendering tests (comparing Skia output bitmaps) and end-to-end layout scenario tests.

---

### S-003: No Test Fixtures, Mocks, or Helpers

**Severity:** Low  
**Files:** `tests/Modern.Forms.Tests/`

Tests create controls directly without shared fixtures, builders, or mock objects. As coverage grows, a `TestHelper` or `ControlFactory` would reduce boilerplate and make tests more readable.

---

## 2. Error Handling

### S-004: Replace Generic `catch (Exception)` Blocks With Specific Types

**Severity:** High  
**Files:**
- `src/Modern.Forms/PictureBox.cs` (~line 94) — catches all exceptions during image loading
- `src/Modern.Forms/ControlCollection.cs` (~line 190) — catches `Exception e` (has critical-exception filter, but still broad)
- `src/Modern.Forms/Layout/TableLayout.SorterObjectArray.cs` (~lines 35, 70) — wraps in `InvalidOperationException`

Catching `Exception` can swallow `OutOfMemoryException`, `StackOverflowException`, or other critical failures. Replace with specific types like `HttpRequestException`, `IOException`, `FormatException`, etc.

---

### S-005: Add Logging or Error Reporting for Swallowed Exceptions

**Severity:** Medium  
**Files:**
- `src/Modern.Forms/PictureBox.cs` (~line 94) — sets `IsErrored = true` but never logs the exception
- `samples/ControlGallery/Panels/TreeViewPanel.cs` (~lines 74-79) — silently swallows file-system exceptions

When exceptions are intentionally caught, at minimum capture diagnostic information (e.g., `Debug.WriteLine` or a logging abstraction). Silent swallowing makes debugging nearly impossible.

---

### S-006: Validate Public API Arguments More Consistently

**Severity:** Medium  
**Files:** Various public methods across the codebase

The codebase uses `ArgumentNullException.ThrowIfNull()` in ~28 places (good), but many public methods—especially property setters and simple forwarding methods—don't validate inputs. Consider a consistent policy: all public methods that accept reference types should validate for null.

---

## 3. Async Patterns

### S-007: Fix `async void` in `PictureBox.LoadInternal`

**Severity:** High  
**File:** `src/Modern.Forms/PictureBox.cs` (~line 76)

`private async void LoadInternal(string? url)` is a fire-and-forget method. Any unhandled exception (not covered by the generic catch) will crash the process. Change the return type to `Task` and handle the `Task` result appropriately at the call site, or ensure all exceptions are reliably caught.

---

### S-008: Add `ConfigureAwait(false)` to All Library Async Code

**Severity:** High  
**Files:**
- `src/Modern.Forms/OpenFileDialog.cs` (~line 25)
- `src/Modern.Forms/SaveFileDialog.cs` (~line 23)
- `src/Modern.Forms/FolderBrowserDialog.cs` (~line 26)
- `src/Modern.Forms/PictureBox.cs` (~line 76)
- `src/Modern.Forms/Clipboard.cs`

As a library, Modern.Forms should use `.ConfigureAwait(false)` on all `await` expressions to avoid capturing the synchronization context and potentially deadlocking callers. Zero usages of `ConfigureAwait` were found in the codebase.

---

### S-009: Document or Deprecate `AsyncHelper.RunSync`

**Severity:** Medium  
**File:** `src/Modern.Forms/AsyncHelper.cs` (~lines 17-32)

`RunSync` uses `TaskFactory.StartNew(...).Unwrap().GetAwaiter().GetResult()` to block on async code. This pattern can cause thread-pool starvation and deadlocks. If it must remain, add XML doc comments warning about its limitations. Consider marking it `[Obsolete]` with a migration path.

---

## 4. API Design & Public Surface

### S-010: Make `FileDialog.FileNames` Immutable

**Severity:** High  
**File:** `src/Modern.Forms/FileDialog.cs` (~line 45)

```csharp
public List<string> FileNames { get; } = new List<string>();
```

Exposes a mutable `List<string>` directly. Callers can add, remove, or clear items unexpectedly. Change to `IReadOnlyList<string>` backed by an internal mutable list.

---

### S-011: Make Static Default Styles Read-Only

**Severity:** High  
**File:** `src/Modern.Forms/Control.cs` (~lines 371, 385)

```csharp
public static ControlStyle DefaultStyle = new ControlStyle(null, ...);
public static ControlStyle DefaultStyleHover = new ControlStyle(DefaultStyle);
```

These mutable static fields allow any code to modify framework-wide defaults. Replace with `static readonly` properties or use an immutable pattern.

---

### S-012: Standardize Event Delegate Types

**Severity:** Low  
**Files:** `src/Modern.Forms/Control.Events.cs`, various control files

Some events use `EventHandler`, some use `EventHandler<TEventArgs>`, and nullability annotations vary. Adopt a single convention (e.g., always use `EventHandler<TEventArgs>` for events with args, `EventHandler` for parameterless events) and ensure consistent nullable annotations.

---

### S-013: Add Missing Convenience Overloads

**Severity:** Low  
**File:** `src/Modern.Forms/Control.cs`

`SetBounds(Rectangle)` exists internally but is not public. There is no public `SetLocation(Point)` or `SetSize(Size)` method to complement the property-based API. Consider adding these for API completeness and discoverability.

---

### S-014: Expose Useful Internal Utilities as Public

**Severity:** Low  
**Files:**
- `src/Modern.Forms/DpiHelper.cs` — DPI scaling utilities useful for custom renderers
- `src/Modern.Forms/Layout/LayoutTransaction.cs` — layout batching pattern
- `src/Modern.Forms/Layout/PropertyStore.cs` — efficient property storage

These are `internal` but would be valuable for developers writing custom controls or renderers. Consider making selected members public or providing public wrappers.

---

## 5. SOLID Principles & Architecture

### S-015: Reduce `Control` Class Size / Responsibilities

**Severity:** Medium  
**Files:** `Control.cs` (1,944 lines), `Control.Layout.cs` (634 lines), `Control.Events.cs` (293 lines), `Control.States.cs`, `Control.ExtendedStates.cs` — combined ~2,900+ lines

While the partial-class split helps organization, the `Control` class still owns state management, layout, event dispatch, input handling, painting, child management, and styling. Consider extracting some of these into composable helper objects (e.g., a `ControlLayoutHelper`, `ControlInputManager`) referenced from `Control`, to improve testability and reduce cognitive load.

---

### S-016: Eliminate Liskov Substitution Violations in `ControlAdapter`

**Severity:** Medium  
**Files:**
- `src/Modern.Forms/Control.cs` (~lines 461, 472, 483, 1141, 1298, 1371, 1385, 1437) — 9 `is ControlAdapter` checks
- `src/Modern.Forms/ControlAdapter.cs` (~lines 69-72) — `Visible` setter silently ignores assignments

`ControlAdapter` is a special `Control` subclass that bridges the native window to the managed control tree, but it violates LSP in multiple places. The base class `Control` has 9 explicit `is ControlAdapter` checks to alter behavior. The `Visible` property setter is a no-op. Consider using an abstract method, behavior flags, or a separate non-`Control` type to model this.

---

### S-017: Make `RenderManager` More Extensible

**Severity:** Medium  
**File:** `src/Modern.Forms/Renderers/RenderManager.cs` (~lines 14-41)

All 27+ renderer registrations are hardcoded in the static constructor. Adding a new control type requires modifying `RenderManager`. Consider auto-discovery via reflection, attribute-based registration, or a builder API (e.g., `RenderManager.Register<MyControl, MyRenderer>()`—which exists but isn't the default path).

---

### S-018: Replace Static `Theme` With an Instance-Based Design

**Severity:** Medium  
**File:** `src/Modern.Forms/Theme.cs`

`Theme` is a pure static class with a `Dictionary<string, object>`. This means:
- Only one theme can exist at a time
- Thread safety is not guaranteed (`suspend_count` is not thread-safe)
- Cannot mock or substitute for testing
- Cannot support multiple windows with different themes

Consider an instance-based `ITheme` interface with a default static accessor.

---

### S-019: Narrow the `ILayoutable` and `IArrangedElement` Interfaces

**Severity:** Low  
**Files:**
- `src/Modern.Forms/ILayoutable.cs`
- `src/Modern.Forms/Layout/IArrangedElement.cs`

`ILayoutable` combines sizing, margins, and bounds-setting. `IArrangedElement` combines bounds, preferred size, display rect, parent/child relationships, and property-store access. These could be factored into smaller, focused interfaces (e.g., `IMeasurable`, `IPositionable`, `IContainable`) to support the Interface Segregation Principle.

---

### S-020: Reduce Hard Dependencies in `ScrollableControl`

**Severity:** Low  
**File:** `src/Modern.Forms/ScrollableControl.cs` (~lines 28-50)

```csharp
hscrollbar = Controls.AddImplicitControl(new HorizontalScrollBar { Visible = false });
vscrollbar = Controls.AddImplicitControl(new VerticalScrollBar { Visible = false });
```

Directly instantiates concrete scrollbar and size-grip types. A factory method or virtual creation method would allow subclasses to provide custom scrollbar implementations.

---

## 6. Resource Management & Memory

### S-021: Dispose Old Back-Buffers on Resize

**Severity:** High  
**File:** `src/Modern.Forms/Control.cs` (~lines 35, 522-526)

```csharp
private SKBitmap? back_buffer;
internal SKBitmap GetBackBuffer() {
    if (back_buffer?.Width != ScaledSize.Width) {
        back_buffer = new SKBitmap(...);
    }
    return back_buffer;
}
```

When the control is resized, a new `SKBitmap` is allocated but the previous one is **never disposed**. `SKBitmap` wraps an unmanaged Skia resource. The old bitmap should be explicitly disposed before creating a new one.

---

### S-022: Add Cache Eviction to Sample `ImageLoader` Classes

**Severity:** Medium  
**Files:**
- `samples/ControlGallery/ImageLoader.cs` (~line 11) — `static readonly Dictionary<string, SKBitmap> _cache`
- `samples/Explorer/ImageLoader.cs` (~line 11) — same pattern
- `samples/Outlaw/ImageLoader.cs` (~line 11) — same pattern

All three samples use an unbounded static image cache. While these are samples, they set a pattern that users may copy. Add a maximum cache size, LRU eviction, or `WeakReference<SKBitmap>` to prevent unbounded memory growth.

---

### S-023: Ensure `ImageCollection` Disposes Bitmaps

**Severity:** Medium  
**File:** `src/Modern.Forms/ImageCollection.cs`

`ImageCollection` stores `SKBitmap` values in a `Dictionary<string, SKBitmap>`. When items are removed or the collection is cleared, the underlying `SKBitmap` objects (which hold unmanaged memory) should be disposed.

---

### S-024: Document Static Event Subscription Lifecycle

**Severity:** Low  
**File:** `src/Modern.Forms/Application.cs` (~line 64)

```csharp
public static event EventHandler? OnExit;
```

Static events can cause memory leaks if subscribers don't unsubscribe. Add XML doc comments warning that handlers must be explicitly removed when no longer needed.

---

## 7. Theme & Styling System

### S-025: Centralize All Color Constants in the Theme System

**Severity:** Medium  
**Files:**
- `src/Modern.Forms/TextBoxDocument.cs` (~line 30) — `new SKColor(153, 201, 239)` for selection
- `src/Modern.Forms/Renderers/PictureBoxRenderer.cs` (~lines 41-42) — `SKColors.Red` for error indicator
- `src/Modern.Forms/Extensions/SkiaTextExtensions.cs` (~line 42) — `SKColors.Blue` default selection color
- `src/Modern.Forms/FormTitleBar.cs` (~line 56) — `SKColors.Transparent`

Several color values are hardcoded outside the `Theme` class. The Theme system exists and defines colors in `Theme.cs` (~lines 239-263), but these outliers bypass it. Move all visual constants into the theme for consistency and to support future dark-mode / custom-theme scenarios.

---

### S-026: Make Theme Thread-Safe

**Severity:** Medium  
**File:** `src/Modern.Forms/Theme.cs` (~line 12)

`private static int suspend_count` and the `Dictionary<string, object>` backing store have no synchronization. If themes are ever changed from a background thread (or during initialization races), this could corrupt state. Add `lock` or use `ConcurrentDictionary`.

---

### S-027: Support Multiple Simultaneous Themes

**Severity:** Low  
**File:** `src/Modern.Forms/Theme.cs`

The theme is global and static. A future-looking improvement would be to allow per-window or per-control theme overrides (e.g., for preview panes, dark/light side-by-side, or design-time tooling).

---

## 8. Documentation & Maintainability

### S-028: Resolve or Track the 34 TODO Comments

**Severity:** Medium  
**Files:** Distributed across 20+ files (see representative list below)

| File | Notable TODOs |
|------|---------------|
| `Form.cs` (lines 326, 345) | "We really need non-client size here" |
| `Control.cs` (line 209) | "We should be scaling the Border as well" |
| `TextBox.cs` (line 419) | "Horizontal scrollbar not supported" |
| `TextBoxDocument.cs` (lines 76, 174) | "wholeWord not implemented", "Need to properly handle code points" |
| `FlowLayout.ContainerProxy.cs` (lines 40, 130) | "RTL" (Right-to-Left support) |
| `TableLayout.cs` (line 1048) | "RTL" |
| `Theme.cs` (line 223) | "BuiltInTheme.Default should detect the OS setting" |
| `ScrollControl.cs` (line 8) | "Need to unify this with ScrollableControl" |
| `ListBox.cs` (line 341) | "Shift" (shift-click selection) |
| `ListBoxItemCollection.cs` (line 9) | "Update selected indexes when adding/removing items" |
| `RibbonRenderer.cs` (line 133) | "This should not be done during the paint process" |

Convert these into GitHub Issues with labels and milestones so they can be tracked and prioritized.

---

### S-029: Add a CONTRIBUTING.md

**Severity:** Low  
**Files:** Root of repository (missing)

There is no CONTRIBUTING guide. For an open-source project, adding one with coding standards, PR guidelines, and the expected test coverage policy would help new contributors.

---

### S-030: Add an .editorconfig Rule for Consistent Async Suffix

**Severity:** Low  
**File:** `.editorconfig`

The `.editorconfig` exists with basic formatting rules but does not enforce naming conventions for async methods (e.g., requiring `Async` suffix). Adding analyzer rules (via `.editorconfig` or an `analyzers.ruleset`) would catch style drift.

---

## 9. Accessibility

### S-031: Implement Accessibility Properties on Controls

**Severity:** High  
**Files:** All control files under `src/Modern.Forms/`

There is **zero** accessibility implementation in the codebase. No `AccessibleRole`, `AccessibleName`, `AccessibleDescription`, or screen-reader integration exists. WinForms provides these via `Control.AccessibilityObject`. For a cross-platform UI framework, accessibility should be a first-class concern. At minimum, add:

- `AccessibleName` and `AccessibleDescription` properties on `Control`
- `AccessibleRole` enum and property
- Keyboard navigation beyond basic Tab order
- Focus indicators visible to all users

---

### S-032: Improve Keyboard Navigation and Mnemonics

**Severity:** Medium  
**Files:** `src/Modern.Forms/Control.cs`, various controls

While basic `TabIndex` and `TabStop` properties exist, there is no support for:
- Mnemonic/access keys (Alt+key shortcuts)
- Arrow-key navigation within control groups (radio buttons, toolbars)
- Keyboard shortcuts for menu items and toolbar buttons

---

## 10. Cross-Platform & Build

### S-033: Update CI to Test on All Platforms

**Severity:** Medium  
**File:** `.github/workflows/dotnet.yml`

The CI workflow builds on macOS, Windows, and Ubuntu but **only runs tests on Windows**. Tests should run on all three platforms to catch platform-specific regressions (especially since this is a cross-platform framework).

---

### S-034: Consider Targeting Multiple .NET Versions

**Severity:** Low  
**File:** `Directory.Build.props`

The project targets only `net8.0`. Consider multi-targeting `net8.0;net9.0` (or whichever LTS versions are current) to ensure compatibility and to leverage new runtime optimizations.

---

### S-035: Add Static Analysis / Roslyn Analyzers

**Severity:** Medium  
**Files:** `src/Modern.Forms/Modern.Forms.csproj`, `.editorconfig`

No Roslyn analyzers (e.g., `Microsoft.CodeAnalysis.NetAnalyzers`, `StyleCop.Analyzers`) are configured. Adding analyzers would automatically catch many of the issues identified in this report (nullable misuse, missing `ConfigureAwait`, naming violations, etc.).

---

## Summary Scorecard

| Category | Grade | Rationale |
|----------|:-----:|-----------|
| **Test Coverage** | **F** | Only 12 tests for 221 source files. No control, renderer, layout, or integration tests. |
| **Error Handling** | **C+** | No empty catch blocks (good), but 5 generic `catch (Exception)` sites and no logging/diagnostics for swallowed errors. |
| **Async Best Practices** | **D+** | One `async void`, zero `ConfigureAwait(false)` usage, and a `RunSync` sync-over-async bridge. |
| **API Design** | **B-** | Generally well-designed WinForms-like API. Docked by mutable static defaults, exposed `List<T>`, and some inconsistencies. |
| **Code Organization** | **B+** | Good use of partial classes, renderers separated from controls, clean folder structure. Control class is large but well-partitioned. |
| **SOLID Principles** | **C** | Moderate SRP issues (Control class); LSP violations (ControlAdapter); DIP violations (static RenderManager, Theme); ISP concerns (broad interfaces). |
| **Documentation (XML Docs)** | **A-** | Comprehensive XML documentation on public APIs with `GenerateDocumentationFile` enabled. |
| **Documentation (Project)** | **C+** | README and getting-started guide exist, but no CONTRIBUTING guide, no architecture doc, and 34 untracked TODOs. |
| **Resource Management** | **C** | Dispose pattern on Control is correct, but back-buffer leak on resize, potential SKBitmap leaks in collections, and unbounded caches in samples. |
| **Accessibility** | **F** | No accessibility properties, no screen-reader support, no mnemonic keys. Critical gap for a UI framework. |
| **Code Style & Consistency** | **A-** | Consistent naming (camelCase private, PascalCase public), Allman braces, nullable reference types enabled, `.editorconfig` present. |
| **Cross-Platform Readiness** | **B** | Good abstraction via Modern.WindowKit. No hardcoded Windows paths. But CI only tests on Windows, and some platform features are incomplete. |
| **Theme / Styling** | **C+** | Theme system exists but is static/global, not thread-safe, and several colors bypass it. No dark mode or per-window theming. |
| **Security** | **B+** | No obvious vulnerabilities. Nullable reference types help prevent null-related issues. Minor concern: no input sanitization on file dialog paths. |
| **Dependencies** | **A** | Minimal, well-chosen dependencies (SkiaSharp, HarfBuzz, RichTextKit, Modern.WindowKit). No bloat. |

### Overall Grade: **C+**

The project has a clean, well-organized codebase with excellent XML documentation and sensible architecture. The critical gaps are **test coverage** and **accessibility**, both of which earn failing grades. Addressing the high-severity items (S-001, S-007, S-008, S-010, S-011, S-021, S-031) would significantly improve the project's robustness and usability.
