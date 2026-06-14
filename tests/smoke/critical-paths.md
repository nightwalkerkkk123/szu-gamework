# Smoke Test: Critical Paths

**Purpose**: Run these 10-15 checks in under 15 minutes before any QA hand-off.  
**Run via**: `/smoke-check` (which reads this file)  
**Update**: Add new entries when new core systems are implemented.  

## Core Stability (always run)

1. Game launches to L1 scene without crash.
2. Player starts on the first platform and does not fall immediately.
3. Main menu / result screen responds to all inputs without freezing.

## Core Mechanic

4. Player can jump and roll while skiing.
5. Player stumbles when hitting a Stumble obstacle.
6. Player crashes when hitting a Crash obstacle or falling into a gap.
7. Glucose passive decay is visible on the HUD.
8. Picking up insulin spray lowers glucose; picking up sugar raises it.
9. Low crisis and high crisis trigger failure after the configured time.
10. Finish line triggers victory.

## Data Integrity

11. SaveService records run results without error.
12. Result panel displays correct win/lose state and score.

## Performance

13. Game maintains 60 fps on target hardware.
14. No memory growth over 5 minutes of play.

## UI / Input

15. Pause / resume works via keyboard and gamepad.
16. "再来一局" button restarts the scene correctly.
