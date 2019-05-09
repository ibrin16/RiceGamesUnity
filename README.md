## Blow Stuff Up
Here are the descriptions for some of the scripts in Blow Stuff Up

#### AStar
>This is an implementation of the A* algorithm used in order to move the AI  
player in the practice level. It can adapt to different layouts so when  
boxes are destroyed it will change its direction if necessary and it is also  
programmed so that at the start of the game no matter where new boxes are placed  
or how many there are it will find a way to go. Ideally the AI would also shoot a  
ray cast out in front and if the player entered it throw a bomb, but since there  
was a very limited amount of time for the project it was never added.

#### Convert To World
>This script is what actually moves the AI player. It has a method MakeMap which is  
called by the A* class to actually make the map. It keeps track of all empty space,  
indestructable blocks, destroyable blocks, the player, and itself. It also has  
the AI 'winning' if it touches the player. Winning in quotes because since this is  
a practice game mode it only reloads the scene on contact. This is also where  
ideally the player would move more fluidly, but once again there was not enough  
time to implement that, and simply getting a working A* algorithm going was  
a accomplishment given no one in the team had made one before.

#### Bomb
>This is the script for the bomb. This has all the actions the bomb does except for  
the animation which is in a BombAnimation script. The bomb shoots multiple ray casts  
in all direction when it explodes checking for if it hit a player or a box. If a  
player is hit the player respawns at a random spawn point and the score increases  
and if a box is hit it is destroyed and there is a 40% chance for a power-up to  
spawn. The bomb script also knows whether it is an actual bomb or from the rocket  
which spawns a bomb on impact that explodes immediately with no animation.


