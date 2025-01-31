# MinimalFantasySnake
Fantasy Snake is a Snake game with Fantasy RPG mixed. The player controls a growing line of Heroes to fight against Monster.

### Core Gameplay
- [x] The game will be played on a 16x16 grid board.
- [x] At the start of the game, spawn a player-controlled Hero, a number of collectable Heroes, and a number of Monsters.
- [ ] The player can do the following actions:
  - [ ] To move
    - [x] Press WASD on the keyboard to move in the up, left, right, and down directions.
    - [ ] Pressing the DPad on the gamepad
    - [x] The player cannot move in the opposite direction. If the player character is facing up, they can go left, up, or right, but not down.
    - [x] The Hero beside the front character will move to occupy the same space as the previous Hero.
- [x] Unlike Snake, there is no fixed interval for movement. This game uses a turn-based system, in which a turn is passed only when the player makes a move.
- [x] Every character (both Heroes and Monsters) has three stats: Health, Attack, Defense.
  - [x] There is a UI showing its stats for each character.
- [ ] Collision occurs when player character move in a direction that will occupy the space of other entity (Heroes line, collectable Hero or Monster)
  - [x] A collision with the Hero line will result in the game being over immediately.
  - [x] Collide with collectable Hero will collect that hero. The collected hero will be at the end of the line. The front character will occupy the space of that hero. Also, spawn new heroes according to the chance configuration.
  - [x] Collide with the Monster will result in battle.
    - [x] When a battle occurs, reduce health both Hero and Monster at the same time using this formula: Damage = (Attacker Attack - Defender Defense)
    - [x] If the Health of the Monster is 0 or lower, remove that enemy from the game and spawn new Monsters according to the chance configuration.
    - [x] If the Health of the Hero is 0 or lower, remove that hero from the game and move the rest of the line normally.
- [x] There is no win condition. The player can play this game endlessly.
- [x] At least these values must be able to be configured.
  - [x] Start number of entity spawn
  - [x] Min and max stats
  - [x] The chance of spawning (how many Heroes or Monsters will be spawned when removed)

### Extra Gameplay
This section outlines additional gameplay requirements. These extra features
are optional for evaluation.
- [ ] Adjustable board size
- [ ] Adding obstacles.
  - [ ] An obstacle can be either 1x1, 1x2, 2x1, or 2x2 in size.
  - [ ] Collide with Obstacle will remove the front character. Move the rest of the line normally.
- [ ] Player can rotate hero characters in the line
  - [ ] Pressing Q on the keyboard or the left shoulder button on the gamepad will rotate the second character in line to be the front character and the front character to be the last.
  - [ ] Pressing E on the keyboard or the right shoulder button on the gamepad will switch the last character in line to be the front character and the front character to be the second.
  - [ ] The rest of the line will be rotated accordingly.
  - [ ] Rotating does not make a move. The line of Heroes will be in the same position.
  - [ ] Example: Considering a line of Heroes A, B, and C. Pressing Q will rotate line to B, C, and A while pressing E will rotate line to C, A and B.
- [ ] Both hero character and monster can be in one of 3 classes. These classes beat over each other in the same manner as rock-paper-scissor.
  - [ ] Warrior deals double damage to Rogue
  - [ ] Rogue deals double damage to Wizard
  - [ ] Wizard deals double damage to Warrior.
  - [ ] Damage is doubled after normal damage calculation
- [ ] Make game more configurable
  - [ ] Ability to specify each class stats separately
  - [ ] Ability to make monster stronger over time
- [ ] Adding Items
  - [ ] Recovery item
  - [ ] Increase attack or defense
- [ ] Adding Level system with progression

### FeedBack
#### Combat
During implementation of combat GDD does not specify about this following conditions
- Monster doesn't have enough attack value to kill hero and Hero doesn't have enough attack value too
  - Injured Solution: since combat already occur this make both hero and monster injured
    - Player can walk away by changing direction or re-initiate the fight by going to the same direction
  - Fight till death Solution: the battle happen in a loop treat it like a both monster and hero trade a hit untill one side is dead

#### SpawningMechanic
There's a chance that no monster will be spawned at all leaving the board completly empty, So I've added minimum number of unit type in the field