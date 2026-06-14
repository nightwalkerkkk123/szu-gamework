# Endless Runner Template Assets

These assets are derived from the **Endless Runner template for Unity**
by Roman Zhuravlev, used under the MIT License.

- **Source repository**: `E:\CODE\Endless-Runner-Entitas-ECS`
- **Original license**: see `LICENSE` in this folder.
- **Copied assets**:
  - `Visuals/` — Materials, shaders, textures, post-processing profile.
  - `PhysicMaterials/` — Player and land physics materials.
  - `Prefabs/` — Chunk prefabs, player prefab, effect prefabs.
  - `Data/ChunkDefinitions.json` — Original chunk definition data.

## Adaptation Notes

The original project uses the **Entitas ECS** framework. This project uses
traditional MonoBehaviour architecture. Therefore, the copied prefabs may
initially show missing script references. We re-wire the visuals and
prefabs to our own gameplay components (e.g., `SkiingController`,
`GlucoseSystem`, `SegmentSpawner`) rather than importing Entitas.

Only non-code assets were copied. The original `Plugins/Entitas/`,
`Sources/`, `Generated/`, and `Tools/` folders were intentionally excluded
to avoid architecture conflicts and to keep the project dependency-free.
