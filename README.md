# Turn Based RPG

## What is Turn Based RPG?

Turn based RPG is a Windows console app game made from very cleverly constructed Console.WriteLine's. You have 2 premade characters that have a variety of abilities, attacks, and items that you use to defeat the enemy formation. The enemy will heal their allies and attack their enemies (that's you!) according to which character is most threatening to them.

## How to Play

Use the directional arrow keys, enter, escape, and tab keys to interact with the game's interface.

The boxes at the top right of the screen indicate whose turn it currently is.

Your characters exist on the left side of the console window, your enemies on the right.

On your characters' turns, attack the enemy using your abilities.

The game ends (or crashes) once you have defeated all of the enemies, or you have your characters defeated.

## About the Infrastructure

### Startup Pipeline

On start, the main project bootstraps all required services and class instances. Repositories load game data from JSON files at this time. Once the initial game state has been initialized, game state data is sent via events to the UI layer which constructs stringified UI components. Afterwards, the render function is called which retrieves all UI components and prints them to the console screen.

### Rendering

Most of the time, the render function is only called whenever the player interacts with the game and an update is needed to represent the interaction. However, in the case where health bars are being changed over time, the render function is called 30 times a second, which can strain the player's system. To reduce the amount of calculations performed, each component has its own change detection and caching capabilities, and only re-renders itself when a change is detected from game data.

### Ability Targeting

A large part of the complexity in the project stems from the tactical aspect of the game, where positioning plays a key part. Each ability has it's own target pattern with different behaviors such as static positioning or the inability to target pass front column enemies. To do this, custom logic was implemented to check valid target positions for an ability and cut off any positions that went out of bounds.

### AI

The AI was implemented using a decision tree that assigns each target position and all accompanying positions, in the case of abilities with more than 1 target, point values depending on how threatening the characters in those positions are. Point values change depending on how much damage or healing an ability does and how much health a target has. The AI is much more likely to target a character it can kill than one who is unlikely to die, and more likely to target a character who is very threatening versus one who is not. The AI decides how much threat a character has depending on how much damage or healing is done by that character, so a character who is dealing massive amounts of damage will be much more threatening than the character sitting around doing nothing. Additionally, the AI will ignore any target positions that it cannot target with a given ability, in the case of static targeting and the inability to target pass enemies.

## Installation using Visual Studio

Before installing the project, you must have [Visual Studio](https://visualstudio.microsoft.com/downloads/) installed.

1. Clone or download the repository using the button above.

2. Extract the file to a location on your hard drive if necessary.

3. Navigate to the clone or extraction location and open the TurnBasedRPG.sln file in Visual Studio.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[GPL V3](https://choosealicense.com/licenses/gpl-3.0/)
