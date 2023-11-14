# Lab18-RaceCars

This is a WinForms GUI application where cars in seperate lanes will race each other.

Each lane is in its own worker thread and modulised to easily change any aspect of the lane in code. Every lane tracks its own data such as their position in the race, distance traveled, how many times its won, etc.

Each car will get a random speed modifier applied to them at a constant interval resulting in a random race outcome.

The user has the ability to reset and start the race.
