# MinimalFantasySnake
Fantasy Snake is a Snake game with Fantasy RPG mixed. The player controls a growing line of Heroes to fight against Monster.

### Core Gameplay
- The game will be played on a 1616 grid board.
- At the start of the game, spawn a player-controlled Hero, a number of collectable Heroes, and a number of Monsters.
- The player can do the following actions:
  - To move
    - Press WASD on the keyboard to move in the up, left, right, and down directions.
    - Pressing the DPad on the gamepad
    - The player cannot move in the opposite direction. If the player character is facing up, they can go left, up, or right, but not down.
    - The Hero beside the front character will move to occupy the same space as the previous Hero.
- Unlike Snake, there is no fixed interval for movement. This game uses a turn-based system, in which a turn is passed only when the player makes a move.
- Every character (both Heroes and Monsters) has three stats: Health, Attack, Defense.
  - There is a UI showing its stats for each character.
- Collision occurs when player character move in a direction that will occupy the space of other entity Heroes line, collectable Hero or Monster)
  - A collision with the Hero line will result in the game being over immediately.
  - Collide with collectable Hero will collect that hero. The collected hero will be at the end of the line. The front character will occupy the space of that hero. Also, spawn new heroes according to the chance configuration.
  - Collide with the Monster will result in battle.
    - When a battle occurs, reduce health both Hero and Monster at the same time using this formula: Damage = (Attacker Attack - Defender Defense)
    - If the Health of the Monster is 0 or lower, remove that enemy from the game and spawn new Monsters according to the chance configuration.
    - If the Health of the Hero is 0 or lower, remove that hero from the game and move the rest of the line normally.
- There is no win condition. The player can play this game endlessly.
- At least these values must be able to be configured.
  - Start number of entity spawn
  - Min and max stats
  - The chance of spawning (how many Heroes or Monsters will be spawned when removed)

### Extra Gameplay
This section outlines additional gameplay requirements. These extra features
are optional for evaluation.
- Adjustable board size
- Adding obstacles.
  - An obstacle can be either 11, 12, 21, or 22 in size.
  - Collide with Obstacle will remove the front character. Move the rest of the line normally.
- Player can rotate hero characters in the line
  - Pressing Q on the keyboard or the left shoulder button on the gamepad will rotate the second character in line to be the front character and the front character to be the last.
  - Pressing E on the keyboard or the right shoulder button on the gamepad will switch the last character in line to be the front character and the front character to be the second.
  - The rest of the line will be rotated accordingly.
  - Rotating does not make a move. The line of Heroes will be in the same position.
  - Example: Considering a line of Heroes A, B, and C. Pressing Q will rotate line to B, C, and A while pressing E will rotate line to C, A and B.
- Both hero character and monster can be in one of 3 classes. These classes beat over each other in the same manner as rock-paper-scissor.
  - Warrior deals double damage to Rogue
  - Rogue deals double damage to Wizard
  - Wizard deals double damage to Warrior.
  - Damage is doubled after normal damage calculation
- Make game more configurable
  - Ability to specify each class stats separately
  - Ability to make monster stronger over time
- Adding Items
  - Recovery item
  - Increase attack or defense
- Adding Level system with progression
