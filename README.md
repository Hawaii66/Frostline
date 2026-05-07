# Frostline

Frostline is a survival logistics game set in a frozen world where your train is your home, your factory, and your only reliable source of warmth. The train can only move forward along the rails, and every route choice matters. From a central starting loop, players explore branching rail networks, scavenge in the snow, expand their train, and slowly reclaim the world by bringing heat back to the land.

The project combines ideas from train logistics games, co-op chaos, exploration, survival, and procedural world generation. The long-term goal is to create a replayable world where each run generates a new rail network, new risks, new opportunities, and new stories.

## Core Fantasy

- Live aboard a moving train in a deadly frozen world
- Leave the train in a suit to scavenge resources before freezing
- Upgrade and expand the train into a mobile settlement
- Use heat as the main progression system
- Explore uncertain side tracks and make risky route decisions
- Deploy a small scout train from a crane car to inspect dangerous routes
- Build heaters and infrastructure to melt snow and restore land
- Eventually establish ground outposts and permanent bases
- Replay in a newly generated world each time

## High-Level Game Loop

The player maintains a large forward-moving train by managing heat, fuel, food, repairs, and onboard production. As the train reaches junctions and side routes, the player must decide whether to stay safe, scout ahead, or commit the large train to a risky line.

Outside the train, players enter the frozen world in protective suits and gather materials such as wood, coal, scrap, machine parts, and rare salvage. These resources are used to craft train modules, repair infrastructure, fuel heaters, and expand the train's capabilities.

Over time, the player creates heated zones in the environment. These zones melt snow, reveal soil and hidden structures, and unlock more advanced forms of survival and settlement. The game gradually evolves from survival on a train into rebuilding a network of warm outposts and restored land.

## Key Pillars

### 1. Train as Home
The main train is not just transport. It is:
- shelter
- workshop
- kitchen
- storage
- research space
- production line
- strategic map of your progress

Its wagon layout matters, and every new module changes how the run plays.

### 2. Heat as the Main Resource
Heat is central to nearly every system:
- personal survival outside
- keeping train cars livable
- powering heaters and thaw zones
- unlocking permanent expansion on the ground

The game is about controlling and extending warmth into a dead world.

### 3. Forward-Only Commitment
The large train does not freely reverse. This makes route choice meaningful and creates tension at every junction. Safety comes from loops, reconnecting lines, service hubs, and carefully designed forward paths rather than simply turning around.

### 4. Scouting Before Commitment
A smaller bidirectional scout train can be carried on a crane wagon attached to the main train. At suitable rail layouts, the scout train can be lifted off onto side tracks to inspect routes before the large train commits.

This creates an important information loop:
- stop the large train in a safe place
- deploy the scout train
- inspect route conditions, hazards, and potential rewards
- return and decide whether the large train should proceed

### 5. Reclaiming the World
The player does not only survive the cold. They slowly push it back. Heaters, lights, energy nodes, and support infrastructure allow small safe zones to form outside. These zones can grow into outposts, bases, farms, and eventually restored regions connected by rail.

## Procedural World Generation

The world is intended to be replayable through procedural generation.

### Macro Structure
The world begins from a central start node and expands outward as a graph. Initial branches are spread radially around the center using polar coordinates with some randomness in angle and distance.

Nodes represent important places or macro structures such as:
- junctions
- reward sites
- service hubs
- dead ends
- reconnect points
- landmarks
- region gates
- obstacle sites
- bridge/tunnel transitions

Edges represent rail connections between nodes.

### Node-Driven Generation
Nodes define:
- what kind of place they are
- what kinds of nodes may connect into them
- what kinds of nodes may connect out of them
- how many outgoing branches they can produce
- whether they start or change a region generation rule set

Some nodes act as region changers. For example, a bridge node may mark the transition into a new region with different generation rules, hazards, rewards, and topology.

### Forward-Only Topology
Because the large train should not rely on reversing, the generated rail network must support forward progression. This means the world graph is designed more like a directed gameplay network than a realistic unrestricted rail simulation.

The generator should favor:
- safe loops near the start
- split-and-rejoin structures
- reconnecting branches
- forward paths into new safe hubs
- controlled commitment routes
- very few true hard dead ends for the large train

### Rail Routing on a Voxel Grid
The world terrain is planned as a voxel-based map, similar in spirit to transport-building games. Rails are built from discrete directional segments:
- straight
- diagonal
- 45-degree turns
- 90-degree turns

The abstract node-edge graph is converted into actual rail paths on the voxel grid through a route generation system. Instead of drawing a straight line between two nodes, each rail connection becomes a small journey.

A route is generated as a sequence of discrete directional actions such as:
- `+x`
- `-x`
- `+y`
- `-y`
- `+x,+y`
- `-x,+y`
- `+x,-y`
- `-x,-y`

These actions are constrained to stay within a corridor or bounding box between two nodes, while still allowing the path to bend and wander. Consecutive actions determine which rail segment prefab is needed:
- same direction -> straight
- 45-degree heading change -> 45-degree turn
- 90-degree heading change -> 90-degree turn

This allows procedural routes to feel authored without becoming perfectly straight.

## Main Systems

### Main Train
- forward-moving mobile base
- modular wagons
- heat, fuel, food, repairs
- crafting and logistics
- long-term upgrade path

### Scout Train
- small bidirectional recon vehicle
- limited fuel and cargo
- deployed via crane wagon
- used to inspect risky side tracks
- does not remove uncertainty entirely

### Resource Gathering
- leave the train in a suit
- survive freezing conditions
- gather fuel, scrap, parts, and special materials
- return before running out of warmth

### Heat Network
- place heaters and support structures outside
- consume fuel to maintain warm zones
- melt snow and reveal terrain
- create permanent footholds

### Ground Bases
- unlocked later in progression
- built in thawed regions
- support farming, production, and expansion
- complement rather than replace the train

## Tone and Inspiration

Frostline is inspired by:
- Snowpiercer for mobile society in a frozen world
- Overcooked for co-op pressure and role overlap
- Unrailed for rail-driven teamwork and improvised logistics
- Voxel Tycoon for voxel-based transport structure and visual readability

The goal is not to copy any one game, but to combine:
- survival
- logistics
- route commitment
- environmental restoration
- emergent co-op stories

## Current Design Focus

The current design work is focused on:
- defining the procedural world graph
- making forward-only train progression playable
- separating macro nodes from rail path routing
- generating rail routes on a voxel-friendly directional grid
- ensuring the world is replayable but not unfair
- balancing risk, information, and commitment

## Open Design Questions

Some major open questions include:
- how often the large train should be able to stop safely
- where double-track should exist and where single-track should dominate
- how to balance loops, reconnects, and commitment routes
- how much information the scout train should reveal
- how to prevent softlocks in procedural generation
- how to transition from train survival to true settlement gameplay

## Temporary Project Name

This project is currently using the working title:

Frostline

## Vision

Frostline aims to create a world where every track decision matters, every expedition is tense, and every patch of melted earth feels like a real victory. The train begins as a fragile moving shelter in a dead frozen world, but over time it becomes the backbone of a growing network of warmth, life, and reclaimed land.

- GENERATED BY AI -
