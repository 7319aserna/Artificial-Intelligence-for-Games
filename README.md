# [Artificial Intelligence for Games](https://asgith.github.io/aie-artificial-intelligence-for-games-page)

![alt text](https://asgith.github.io/images/aie/artificial-intelligence-for-games-02.png "Artificial Intelligence for Games Screenshot")

[Artificial Intelligence for Games](https://asgith.github.io/aie-artificial-intelligence-for-games-page) is a Unity game project which demonstrates the concept of artificial intelligence. In this project, there are 4 AI agents, one which can pathfind based on where the user clicks in the space, and the rest who can flee/pursue/or wander. There is still much more to learn, and is something that I would love to work on more in the future.

Latest Update:

* May 23rd, 2019
  - Implemented an instance of pathfinding that will a raycast a point (red node) selected based on the mouse cursor.
  - Once a point has been chosen, this would create nodes around the object until a node reaches the selected target.
  - Afterwards, the node closest to the object would go through and figure out which node next to it, is the closest to the target.
  - The next node chosen would repeat the process while recording each node selected before, onto a list to be the path the object follows afterwards (nodes highlighted in blue).

![alt text](https://asgith.github.io/images/aie/artificial-intelligence-for-games-03.png "Artificial Intelligence for Games Screenshot")

## How to Play

Set Pathfinding Target: left click anywhere in the area

![alt text](https://asgith.github.io/images/aie/artificial-intelligence-for-games.png "Artificial Intelligence for Games Screenshot")

## Miscellaneous

Tools used:

* Game Engine - [Unity Game Engine](https://unity.com/)

## To Run it

Download the .zip folder and extract it. Launch the executable (.exe).
