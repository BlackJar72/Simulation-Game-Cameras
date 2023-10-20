# Simulation-Game-Cameras
A system of camera controls for simulation games created with Unity3d

This is actually a collection of usuable cameras that will hopefully, eventually include the 
ability to switch between them in-game.  It will also, hopefully, ultimately exist in version 
for both the old and new input systems.

The included controllers are as follows:
* The classic controller, mouse is unlocked and the cursor is visible; mouse-orbit with WASD or use WASD + Left Shift to move.
* Classic with discrete y values, like classic controller, but has configurable (and resettable) levels, for using with buildings, houses, and similar structures with descrete floors
* First person controller, fly around with WASD + mouse, while mving up and down with shift and space (like creative mode in some games)
* First person with discrete y values, like the above but will switch to the highest of the set floors to be under the camera holder
* Free cam, flies around with WASD + mouse and always moves in the direction you are facing (including on the virtical)

All are come with events to detect mouse down, mouse up, and mouse click, reporting the Raycast Hit object for us elsewhere.

The following optional scripts are also included:
* A camera mode switcher, to be place on the camera holder object allong with one or more of the above controllers (all be the first should start disabled),
   allowing swapping between controllers with the press of a button.
* An event converter, which converts my custom C# events into untity events for those who want to connect events in the editor.

  


## MIT License

Copyright (c) 2022 JaredBGreat

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
