# B08 – AI-Assisted 2D Game Engine in C# WinForms + Asteroids Demo

## Overview
This project is a simple 2D Asteroids-style game developed in C# using WinForms.

The implementation follows these constraints:
- No external game engine
- No `System.Windows.Forms.Timer`
- Manual game loop
- Engine separated from game logic

## Project Structure

```text
B08_AsteroidsEngine
├── Engine
│   ├── Entity.cs
│   ├── GameWorld.cs
│   ├── Input.cs
│   └── GameLoop.cs
│
├── Game
│   ├── Ship.cs
│   ├── Bullet.cs
│   ├── Asteroid.cs
│   └── AsteroidsRules.cs
│
├── MainForm.cs
├── Program.cs
└── README.md