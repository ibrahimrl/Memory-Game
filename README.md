# Memory Card Matching Game

<div style="display: flex;">
  <img src="ConsoleApplication1/img/gameplay.png" alt="sst1" style="width: 90%;  padding-left: 5%;">
</div>

## Introduction 
This memory matching game is a fun and engaging project developed in C# using GtkSharp. It challenges players to match pairs of cards under two different modes: a timed mode where players must complete levels within a specific time frame, and a limited attempts mode where players have a fixed number of incorrect guesses allowed. With multiple levels of increasing difficulty, this game is designed to test and improve memory skills in a playful setting.

## Project Structure

The project is structured into several key components, utilizing various classes and libraries:

1. Game Class: Handles the logic for card initialization, shuffling, and state management of cards (matched or unmatched).

2. MemoryGame Class:  A subclass of Gtk.Window, it is the main entry point for the GTK application. It manages the UI, game settings, and transitions between different game levels.

3. UI Components:
    - Windows and Dialogs: Manages various GTK windows and dialog components that display the game's levels, settings, and notifications..
    - Buttons and Labels: Used for interactive elements like start, next level, and settings, as well as for displaying timers and attempt counts.

4. Libraries:
    - GtkSharp: Used for all UI components, handling the layout and event management within the application.
    - System.Collections.Generic: For managing collections like arrays and dictionaries which are essential for storing the card states and values.
    - System: Core C# library used for basic operations and utilities like Random for shuffling cards.

5. Assets:
    - img Folder: Contains the image files used within the game and the README file.

## Function Descriptions

### Game Class Functions

- **`Constructor (Game)`**: Initializes a new game instance by setting the number of cards and calling the method to initialize and shuffle card values.
- **`InitializeCards`**: Fills the `CardValues` array with pairs of matching numbers, ensuring that each pair appears twice and that all pairs are unique. This setup is crucial for the matching game logic.
- **`ShuffleCards`**: Randomizes the order of elements within the `CardValues` array to ensure that the game board has a random layout each time a game starts.

### MemoryGame Class Functions

- **`Constructor (MemoryGame)`**: Initializes the main game window, sets up the UI elements, and attaches events.
- **`ToggleButton`**: Switches game modes between timer and limited attempts based on user selection.
- **`OnStartClicked`**: Triggers the start of the game, setting up the environment and initializing the first level based on the selected mode.
- **`ResetGameEnvironment`**: Clears any existing game state and prepares the environment for a new game or level.
- **`InitializeGameLevel`**: Sets up the card grid and initializes gameplay for the specified level, arranging the cards and setting mode-specific parameters.
- **`StartTimer`**: Initiates a countdown timer for the current level, updating the UI accordingly.
- **`UpdateTimer`**: Decrements the timer each second, and handles the time expiration by calling `GameOver`.
- **`GameOver`**: Displays a game over dialog and resets the game environment upon failure or timer run out.
- **`ResetTimer`**: Resets the game timer, typically called when moving to a new level.
- **`CalculateCardsPerRow`**: Determines the number of cards per row based on the current game level.
- **`CalculateNumberOfRows`**: Calculates the number of rows for cards based on the current level.
- **`CalculateCardSize`**: Computes the size of each card to fit them appropriately within the UI grid based on available space.
- **`CreateCardBackPixbuf`**: Generates a graphical representation of the card back using a Pixbuf, resized according to card dimensions.
- **`InitializeCards`**: Populates the game grid with clickable card buttons, assigning values and images.
- **`OnCardClicked`**: Handles the card flip action, checking for matches or enforcing game rules (like decrementing attempts).
- **`AllCardsMatched`**: Checks if all card pairs have been successfully matched, used to determine level completion.
- **`CheckGameProgress`**: Assesses whether the current level is complete and either progresses to the next level or ends the game if all levels are completed.
- **`ShowGameWonMessage`**: Displays a congratulatory message upon winning the game.
- **`AddNextLevelButton`**: Adds a button to the UI allowing the player to proceed to the next level upon successful completion of the current one.
- **`FlipCardBack`**: Reverses a card to show its back after an unsuccessful match attempt or upon resetting the game.
- **`CreateTextPixbuf`**: Creates a Pixbuf containing text for displaying numbers on the card faces during gameplay.

### Program Entry Point

- **`Main`**: Entry point for the application which initializes and runs the memory game.


## Setup

To run this project, ensure you have set up the required environment:

Clone the repository:
  ```bash
  git clone https://gitlab.mff.cuni.cz/teaching/nprg031/2324-summer/student-rahimlii.git
  cd student-rahimlii/ConsoleApplication1
  ```

Add GtkSharp package:
 ```bash
 dotnet add package GtkSharp
 ```

Build the project:
 ```bash
dotnet build
 ```

## Usage

To start the game, navigate to the project directory and run:

 ```bash
dotnet run
 ```

<div style="display: flex;">
  <img src="ConsoleApplication1/img/Start_Screen.png" alt="sst1" style="width: 90%;  padding-left: 5%;">
</div>

Upon launching, you will see the main menu where you can choose between "Timer" and "Limited Attempts" modes. Select a mode to begin playing through the levels. The game progresses with increasing difficulty, and your goal is to match all cards correctly to advance to the next level.

Screenshots:

<div style="display: flex;">
  <img src="ConsoleApplication1/img/level1.png" alt="sst1" style="width: 90%;  padding-left: 5%;">
   <img src="ConsoleApplication1/img/Level1_play.png" alt="sst1" style="width: 90%;  padding-left: 5%;">
</div>

## License

This project is licensed under the [MIT License](LICENSE).