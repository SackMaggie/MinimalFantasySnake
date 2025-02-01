# MinimalFantasySnake
Fantasy Snake is a Snake game with Fantasy RPG mixed. The player controls a growing line of Heroes to fight against Monster.

### FeedBack
#### Combat
During implementation of combat GDD does not specify about this following conditions
- Monster doesn't have enough attack value to kill hero and Hero doesn't have enough attack value too
  - Injured Solution: since combat already occur this make both hero and monster injured
    - Player can walk away by changing direction or re-initiate the fight by going to the same direction
  - Fight till death Solution: the battle happen in a loop treat it like a both monster and hero trade a hit untill one side is dead

#### SpawningMechanic
There's a chance that no monster will be spawned at all leaving the board completly empty, So I've added minimum number of unit type in the field

#### Double Input
There's a problem when two keys is pressed at the same time such as W,D key we will get both x and y as 0.71 (same value) which mean it will go up and right ? currently I'm taking vertical as priority so snake will tend to go up or down more often.

#### Code Style

> [!CAUTION]
> I am terribly sorry if my code style does not meet your expectations, but things can be taught.

Why did I choose to implement each type of unit via interface class ?
> After I've read assignment in the sections `Allowed third party plugins` and two of allowed plugins is about dependency injection. And in fact I never use those tool before and to do that I rely solely on interface class instead So it make the project seem lengthy but it's still a loose coupling and improve reusability just like DI

Is this project over engineered ?
> Yes, This project show that perfectly if the code is re-wrote as a tight coupling style, It will eliminate the need of interface class some inheritance in the project, It will make the code a lot easier to wrote but it will cut reusability, extend functionality, etc.

Why I make CustomMonoBehaviour class that seem to do nothing ?
> When child class inherit parent class that contain unity functions only the child class get invoked and the parent function will not get invoked.
To safely fix/avoid the issue create an empty class make a fuction as override-able so the code-editor know and warn the guy that working on the code without accidentaly broke function underneath, If it still broke it's intentional.

Where did the setting as requested from GDD go ?
> It's located inside [settings](/Assets/Snake/Settings/) folder the content contains some scriptable object file

Why all the asset in project hook via guid based ? instead of path loading
> I really don't like accessing stuff by path I would avoid it at all the cost. The reason is "It's a time bomb" it work for now but when somebody or myself in the future move files and folder around all that path aren't gonna get update. So I just hook it via guid instead (the so called drag-n-drop) which managed by unity.

Why there's no asynchronous in the project ? Can I wrote it ?
> Yes, I wrote a bunch of asynchronous programming for example the addressable for asset load and handling [FishNetAddressable Project](https://github.com/SackMaggie/FishNetAddressable).
<br>Since you can wrote it why not use it for asset load? I've no clue if the project will be ported to instant play (no async support) So I would just avoid it untill there's a confirmation

Why not seperate each feature into different branch ? Did you just push strait to main ?
> No, It's totally depend on what will need to be done if it just readme update or some config change I will not create a branch as it's very much 1 commit. However when there's a functionalityy changes and it will take a while to be done I will consider create a branch.


### Core Gameplay
- [x] The game will be played on a 16x16 grid board.
- [x] At the start of the game, spawn a player-controlled Hero, a number of collectable Heroes, and a number of Monsters.
- [x] The player can do the following actions:
  - [x] To move
    - [x] Press WASD on the keyboard to move in the up, left, right, and down directions.
    - [x] Pressing the DPad on the gamepad
    - [x] The player cannot move in the opposite direction. If the player character is facing up, they can go left, up, or right, but not down.
    - [x] The Hero beside the front character will move to occupy the same space as the previous Hero.
- [x] Unlike Snake, there is no fixed interval for movement. This game uses a turn-based system, in which a turn is passed only when the player makes a move.
- [x] Every character (both Heroes and Monsters) has three stats: Health, Attack, Defense.
  - [x] There is a UI showing its stats for each character.
- [x] Collision occurs when player character move in a direction that will occupy the space of other entity (Heroes line, collectable Hero or Monster)
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
- [x] [Adjustable board size](https://github.com/SackMaggie/MinimalFantasySnake/pull/3)
- [ ] Adding obstacles.
  - [ ] An obstacle can be either 1x1, 1x2, 2x1, or 2x2 in size.
  - [ ] Collide with Obstacle will remove the front character. Move the rest of the line normally.
- [x] [Player can rotate hero characters in the line](https://github.com/SackMaggie/MinimalFantasySnake/pull/5)
  - [x] Pressing Q on the keyboard or the left shoulder button on the gamepad will rotate the second character in line to be the front character and the front character to be the last.
  - [x] Pressing E on the keyboard or the right shoulder button on the gamepad will switch the last character in line to be the front character and the front character to be the second.
  - [x] The rest of the line will be rotated accordingly.
  - [x] Rotating does not make a move. The line of Heroes will be in the same position.
  - [x] Example: Considering a line of Heroes A, B, and C. Pressing Q will rotate line to B, C, and A while pressing E will rotate line to C, A and B.
- [x] [Both hero character and monster can be in one of 3 classes. These classes beat over each other in the same manner as rock-paper-scissor.](https://github.com/SackMaggie/MinimalFantasySnake/pull/4)
  - [x] Warrior deals double damage to Rogue
  - [x] Rogue deals double damage to Wizard
  - [x] Wizard deals double damage to Warrior.
  - [x] Damage is doubled after normal damage calculation
- [ ] Make game more configurable
  - [x] Ability to specify each class stats separately [here](/Assets/Snake/Settings/GameSetting.asset)
  - [ ] Ability to make monster stronger over time
- [x] [Adding Items](https://github.com/SackMaggie/MinimalFantasySnake/pull/2)
  - [x] Recovery item
  - [x] Increase attack or defense
- [ ] Adding Level system with progression
