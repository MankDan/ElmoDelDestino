# Elmo del destino - A C# Text-Based Adventure

The "Elmo del destino" (Helm of destiny) is a rogue-like text-based adventure built in C# for the console environment. The project is a practical implementation of Object-Oriented design principles, featuring a modular architecture with systems for procedural generation, state persistence, and external configuration.

<img width="910" height="722" alt="image" src="https://github.com/user-attachments/assets/dde4bd0b-38b8-4fd3-89d8-d617a25c40ac" />
<img width="916" height="625" alt="image" src="https://github.com/user-attachments/assets/73346bef-6300-43ed-bbc1-efceeca1a0df" />


## Table of Contents

1.  [Key Features](#key-features)
2.  [Technical Architecture](#technical-architecture)
3.  [Core Systems Breakdown](#core-systems-breakdown)
4.  [Getting Started](#getting-started)
5.  [Project Structure](#project-structure)
6.  [Configuration System](#configuration-system)
7.  [License](#license)

## Key Features

*   **Procedural Map Generation:** The game world is generated at runtime using a seed-based algorithm, ensuring a unique layout for each playthrough while allowing for reproducible maps.
*   **State Persistence:** A robust save/load system allows the entire game state—including player position, inventory, and world state—to be serialized to disk and resumed later.
*   **External JSON Configuration:** Game balance parameters (e.g., item stats, enemy health, player attributes) are loaded from external `.conf` (JSON) files, allowing for easy modification without recompiling the source code.
*   **Custom Console Rendering Engine:** A dedicated static class (`AdvConsole`) provides advanced control over the console, featuring selective screen clearing to reduce flicker, precise cursor positioning, and a conceptual double-buffer for rendering scenes.
*   **Object-Oriented Design:** The codebase is built on SOLID principles, utilizing abstraction, inheritance, and interfaces to create a decoupled and extensible architecture.

## Technical Architecture

The application is built on a component-based architecture where distinct systems manage specific responsibilities.

*   **Core Engine (`Game.cs`):** The central class that initializes all subsystems, manages the main game loop, and processes user input. It acts as the orchestrator for all other components.
*   **Managers:**
    *   `SaveLoadManager`: Handles the serialization and deserialization of the game state. It interacts with any object that implements the `ISalvabile` interface.
    *   `ConfigurationManager`: Manages the loading of external JSON configuration files, with built-in resilience to create default configurations if files are missing or corrupt.
*   **Domain Model:**
    *   `Stanza`, `Giocatore`, `Personaggio`, `Oggetto`: These classes represent the core entities of the game world. They encapsulate both the data (state) and the logic (behavior) of the game's elements.
*   **Rendering & UI:**
    *   `AdvConsole`: A utility class that abstracts the `System.Console` API. It uses P/Invoke (`LibraryImport`) to interface with the Win32 API (`kernel32.dll`) for advanced features like non-blocking input and enabling ANSI escape sequences.
    *   `StampaStanza`: A dedicated renderer that uses `AdvConsole` to draw a visual representation of the current room, its contents, and its exits.

---

## Core Systems Breakdown

#### 1. The Game Loop
The primary game loop in `Game.Inizia()` executes a sequence of operations for each "frame" of the game:
1.  **Clear:** Cleans relevant sections of the console buffer to prepare for the new frame.
2.  **Render:** Draws the current state of the game world and UI to the console.
3.  **Input:** Waits for and captures user input in a non-blocking manner.
4.  **Update:** Processes the user's input and updates the game state accordingly.

#### 2. Persistence System
The save/load functionality is designed to be scalable.
*   The `ISalvabile` interface defines a contract (`Salva` and `Carica` methods) for any object that needs to persist its state.
*   The `SaveLoadManager` iterates over a collection of `ISalvabile` objects, delegating the responsibility of serialization to the objects themselves. This decouples the manager from the specific implementation details of the objects it saves.

#### 3. Configuration Loader
The `GenericConfig<T>` class provides a type-safe, resilient way to load settings.
*   It deserializes JSON files into strongly-typed C# objects.
*   If a file is not found or fails to parse, it catches the exception, logs the issue, and creates a new configuration file with default values defined in the corresponding class. This ensures the application can always start.

#### 4. Console Rendering Engine (`AdvConsole`)
To provide a smooth user experience, `AdvConsole` uses several techniques:
*   **Conceptual Double Buffering:** The visual representation of a room is first constructed in an in-memory `char[,]` array. This buffer is then written to the console in a single, fast operation to minimize screen flicker.
*   **Low-Level API Calls:** P/Invoke is used to set console modes, which is necessary for disabling default behaviors like QuickEdit mode and enabling the processing of ANSI escape sequences for potential future color support.

---

## Getting Started

### Prerequisites
*   .NET 8.0 SDK or later

### Running the Application
1.  Clone the repository to your local machine:
    ```bash
    git clone https://github.com/MankDan/ElmoDelDestino.git
    cd ElmoDelDestino.git
    ```
2.  Navigate to the source directory and use the .NET CLI to run the project:
    ```bash
    dotnet run --project src/Gioco
    ```

On the first launch, the application will automatically create a `.elmodeldestino` directory in your local application data folder (`%localappdata%`) to store save files and configurations.

## Project Structure

The source code is organized into namespaces based on feature and responsibility.
```
src/Gioco/
├── Program.cs # Application entry point
├── Game.cs # Core game engine and game loop
│
├── Config/ # Classes for managing .conf files
├── ConsoleUtils/ # The AdvConsole rendering engine
├── Managers/ # SaveLoadManager, ConfigurationManager
├── Logger/ # The Log4Elmo logging system
├── Interfacce/ # Interfaces (ISalvabile, IStampabile...)
├── Giocatori/ # The player class
├── Oggetti/ # Item classes
├── Personaggi/ # Character/NPC classes
└── Stanze/ # Room/map classes
```

## Configuration System

The game's behavior can be modified by editing the `.conf` files located in `%localappdata%/.elmodeldestino/config/`. These are JSON files that control attributes for:

*   **Game:** `Game.conf` (e.g., number of scrolls, world seed)
*   **Player:** `Giocatore.conf` (e.g., starting health, inventory capacity)
*   **Items & Weapons:** `Spada.conf`, `Pozione.conf`, etc. (e.g., damage, healing amount)
*   **Enemies:** `Guardiano.conf` (e.g., health, damage range)

This approach allows for rapid prototyping and balancing without needing to alter the source code.

## License

This project is distributed under the MIT License. See the `LICENSE` file for more information.
