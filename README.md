# DingoAssetsLoadSystem

`DingoAssetsLoadSystem` is a runtime asset loading framework for Unity focused on three things:

- async loading from file paths and URIs;
- shared typed caching between multiple consumers;
- deterministic unload when the last consumer releases the asset.

This file is the module-level technical documentation for the repository itself.

## Why this exists

The module solves a common Unity problem: many systems need to load the same external asset, react to loading progress/state, and release memory safely without each feature reimplementing its own `UnityWebRequest`, `glTFast`, or cleanup logic.

Instead of scattering asset-specific code around UI, gameplay, and scene controllers, the project uses one generic loading contract:

- `Loader` knows how to build an asset from a path.
- `Cache` owns the shared loaded instance.
- `Handle` exposes a stable API to gameplay code.
- `Wrapper` binds loading state to Unity components and GameObjects.

## Key advantages

- One mental model for text, textures, audio, parsed data, meshes, and instantiated glTF scenes.
- Shared cache per asset type prevents duplicate loads for the same key.
- Lifetime is bound to receivers, so assets are released automatically when no one uses them anymore.
- A consistent state machine (`None`, `Loading`, `NotFound`, `Loaded`, `Failed`) makes UI and gameplay code predictable.
- Strong extension points let you add a new asset type without rewriting the architecture.
- Wrapper components remove boilerplate from MonoBehaviours and keep view logic isolated.
- Local text assets can include file timestamps in the cache key, which gives automatic cache busting when the file changes.
- Parsed text cache keys include parser identity and version, so parser changes can invalidate cached results safely.

## Supported asset pipelines

| Module | Asset type | Source | Notes |
| --- | --- | --- | --- |
| `TextLoad` | `TextFileAsset` | local files and remote URIs | Optional byte retention, decoding, BOM stripping, newline normalization, file stamp based cache keys |
| `ParsedTextLoad` | generic parsed object `T` | text source + parser | Requires `NEWTONSOFT_EXISTS`; can parse on thread pool |
| `Texture2DLoad` | `Texture2D` | local files and remote URIs | Configurable readability, mipmaps, linear color space |
| `AudioClipLoad` | `AudioClip` | local files and remote URIs | Configurable `AudioType`, streaming, compressed-in-memory mode |
| `MeshLoad` | `Mesh` | `.gltf` / `.glb` | Requires `GLTFAST`; can select mesh, combine meshes, recalc normals/bounds |
| `MeshGOLoad` | `MeshGOAsset` | `.gltf` / `.glb` | Requires `GLTFAST` or `COM_UNITY_CLOUD_GLTFast`; imports a reusable prototype hierarchy |

## Dependencies

### Internal repository dependencies

The repository directly depends on the following AppSDK submodules:

| Dependency | Used by | Repository | Branch |
| --- | --- | --- | --- |
| `UnityBindVariables` | Core flow layer: `Bind<T>` and `IReadonlyBind<T>` used in `GlobalAssetCache` and handles | `https://github.com/DingoBite/UnityBindVariables` | default repository branch (`.gitmodules` does not pin a branch) |
| `DingoUnityExtensions` | Wrapper layer: `ValueContainer<T>`, `RevealBehaviour`, `SingleKeyText`, and related Unity view helpers | `https://github.com/DingoBite/DingoUnityExtensions` | `dev` |

### External package dependencies

The following Unity packages or vendorized libraries are used by the feature-specific loaders:

- `Cysharp.Threading.Tasks` (`UniTask`) for async loading, cancellation, and main-thread switching.
- Unity `UnityWebRequest` modules for request-based text, texture, and audio loading.
- `com.unity.cloud.gltfast` for `MeshLoad` and `MeshGOLoad`.
- `com.unity.nuget.newtonsoft-json` for `ParsedTextLoad` and `TextAssetExtensions.Deserialize`.

## Architecture

The whole module is built around a generic core located in the root folder:

- `AssetLoadData.cs`
- `AssetLoadHandle.cs`
- `AssetLoadDataWrapper.cs`
- `UnityReceiverLiveness.cs`

### 1. `AssetLoadData<TAsset, TInfo>`

This is the state payload flowing through the system. It contains:

- `Path`
- `Info`
- `Asset`
- `State`
- `Error`

`State` is represented by `AssetLoadState`:

- `None`
- `Loading`
- `NotFound`
- `Loaded`
- `Failed`

This lets every asset pipeline report progress in the same shape, which is why wrappers can be implemented consistently.

### 2. `GlobalAssetCache<TKey, TAsset, TInfo>`

`GlobalAssetCache` is the core runtime service. It owns:

- cache entries indexed by a typed key;
- the bindable `Flow` per entry;
- the set of active receivers using that entry;
- cancellation tokens for in-flight loads;
- release logic when an entry is no longer needed.

Responsibilities:

- create or reuse a cache entry for a requested key;
- start a load when the entry is empty or reload is forced;
- publish state updates into `Flow`;
- cancel old loads during reload or invalidation;
- release the asset through `IAssetReleaser` when the last receiver goes away;
- prune destroyed Unity receivers via `UnityReceiverLiveness.IsUnityAlive`.

Important behavior:

- Receiver ownership is tracked per cache instance.
- Calling `Acquire(...)` for a receiver first releases any previous key used by the same receiver in that cache.
- If the last receiver unloads, the asset is released immediately and the cache entry is removed.
- This is a lifetime-oriented cache, not a long-lived memory pool or LRU cache.

### 3. `AssetLoadHandle<TKey, TAsset, TInfo>`

A handle is the API used by gameplay code and wrappers.

It stores:

- `Path`
- `Info`
- a reference to the target `GlobalAssetCache`
- a proxy `Flow`
- a set of active receivers for this handle

Main operations:

- `LoadFor(object receiver)`
- `LoadFor(object receiver, bool forceReload)`
- `UnloadFor(object receiver)`
- `Invalidate()`
- `Set(string newPath, TInfo newInfo = default, bool dropToNone = true)`

The handle subscribes to the cache entry flow and forwards updates into its own proxy bind. This means callers can listen to one stable `Flow` even if the underlying cache entry changes.

### 4. `AssetLoadDataWrapper<THandle, TAsset, TInfo>`

Wrappers are Unity-facing adapters built on top of `ValueContainer<THandle>`.

They provide:

- automatic subscription to handle flow;
- automatic load/unload on `OnEnable` / `OnDisable`;
- manual `LoadOnly()` and `Unload()` entry points;
- optional lifetime automation through `AutoManageLifetime`;
- an abstract `ApplyView(...)` method where each concrete wrapper updates Unity components.

This is the reason the system integrates cleanly with UI and scene objects without duplicating state logic.

### 5. `UnityReceiverLiveness`

Destroyed Unity objects are not normal `null` references, so the cache uses `UnityReceiverLiveness.IsUnityAlive` to safely detect dead Unity receivers and prune them from entries.

This reduces stale receiver leaks when GameObjects are destroyed without explicit unload calls.

## Runtime flow

The standard flow looks like this:

1. Create a typed handle, for example `new Texture2DLoadHandle(path, info)`.
2. Call `LoadFor(receiver)` from gameplay code, or assign the handle into a wrapper.
3. The handle asks the global cache to `Acquire` the asset for that receiver.
4. The cache creates or reuses an entry and publishes `Loading`.
5. The asset-specific loader runs asynchronously.
6. The cache publishes `Loaded`, `NotFound`, or `Failed`.
7. Every subscribed consumer receives the same final state through `Flow`.
8. When `UnloadFor(receiver)` removes the last receiver, the cache releases the asset and removes the entry.

Reload flow:

- `LoadFor(receiver, forceReload: true)` restarts loading.
- `Invalidate()` invalidates the current cache key.
- If receivers still exist, invalidation triggers a new load.
- If no receivers exist, invalidation clears and removes the entry.

## Module details

### `TextLoad`

Files:

- `TextFileAsset.cs`
- `TextLoadInfo.cs`
- `TextCacheKey.cs`
- `TextKeyFactory.cs`
- `TextLoader.cs`
- `TextLoadHandle.cs`
- `TextLoadWrapper.cs`
- `TextReleaser.cs`
- `TextGlobal.cs`

Behavior:

- Local files are read through `File.ReadAllBytesAsync`.
- Remote sources use `UnityWebRequest.Get`.
- Relative paths are normalized to full local paths.
- Absolute non-file URIs are preserved as-is.
- `404` maps to `NotFound`.
- Network or protocol problems become `Failed` with an exception.

`TextLoadInfo` controls:

- `UseFileStampInKey`
- `KeepBytes`
- `DecodeText`
- `NormalizeNewLines`
- `StripBom`
- `EncodingName`

`TextFileAsset` keeps:

- resolved path;
- decoded text;
- optional raw bytes;
- encoding name;
- optional file length and last write timestamp.

Technical note:

- When `UseFileStampInKey` is enabled for a local file, cache keys include file length and `LastWriteTimeUtc.Ticks`.
- This means a changed file naturally maps to a new cache key without manual versioning.

### `ParsedTextLoad`

Files:

- `ParsedTextLoadInfo.cs`
- `ParsedTextLoader.cs`
- `ParsedTextReleaser.cs`
- `ParsedTextGlobal.cs`
- `NewtonsoftJsonParser.cs`
- `TextAssetExtensions.cs`

Compile-time dependency:

- The whole module is guarded by `NEWTONSOFT_EXISTS`.

Behavior:

- First loads the raw text asset through `TextLoader`.
- Then parses it through an `ITextParser<T>`.
- Parsing can happen on the thread pool when `ParseOnThreadPool` is `true`.

Why the cache key is important:

- The parsed cache key contains the raw `TextCacheKey`.
- It also includes parser `Id` and `Version`.
- This prevents reusing parsed objects that were built with a different parser contract.

This is especially useful when the same JSON file can be interpreted by multiple parsers or parser versions.

### `Texture2DLoad`

Files:

- `Texture2DLoadInfo.cs`
- `Texture2DCacheKey.cs`
- `Texture2DKeyFactory.cs`
- `Texture2DLoader.cs`
- `Texture2DLoadHandle.cs`
- `Texture2DLoadWrapper.cs`
- `Texture2DReleaser.cs`
- `Texture2DGlobal.cs`

Behavior:

- Uses `UnityWebRequestTexture.GetTexture(...)`.
- Supports local files and remote URIs.
- Destroys loaded textures on release.

`Texture2DLoadInfo` controls:

- `Readable`
- `MipmapChain`
- `LinearColorSpace`
- `MipmapCount` on Unity 6+

`Texture2DLoadWrapper` is already wired for common UI use cases:

- shows/hides preload and not-found views;
- assigns texture into `RawImage`;
- updates `AspectRatioFitter`;
- optionally adjusts `LayoutElement` width and height from texture aspect ratio.

### `AudioClipLoad`

Files:

- `AudioClipLoadInfo.cs`
- `AudioClipCacheKey.cs`
- `AudioClipKeyFactory.cs`
- `AudioClipLoader.cs`
- `AudioClipLoadHandle.cs`
- `AudioClipLoadWrapper.cs`
- `AudioClipReleaser.cs`
- `AudioClipGlobal.cs`

Behavior:

- Uses `UnityWebRequestMultimedia.GetAudioClip(...)`.
- Supports local files and remote URIs.
- Returns `NotFound` on `404`.
- Throws an exception for other load failures.

`AudioClipLoadInfo` controls:

- `AudioType`
- `StreamAudio`
- `CompressedInMemory`

`AudioClipReleaser` supports multiple release strategies:

- `Destroy`
- `UnloadAudioDataThenDestroy`
- `UnloadAudioDataOnly`

`AudioClipLoadWrapper` can:

- assign the loaded clip into an `AudioSource`;
- autoplay on success;
- stop and clear the clip on unload.

### `MeshLoad`

Files:

- `MeshLoadInfo.cs`
- `MeshCacheKey.cs`
- `MeshKeyFactory.cs`
- `MeshLoader.cs`
- `MeshLoadHandle.cs`
- `MeshLoadWrapper.cs`
- `MeshReleaser.cs`
- `MeshGlobal.cs`

Compile-time dependency:

- The module is guarded by `GLTFAST`.

Behavior:

- Supports only `.gltf` and `.glb`.
- Loads mesh data through `glTFast`.
- Can select a mesh by `MeshName` or `MeshIndex`.
- Can combine all imported meshes into one generated mesh.
- Can recalculate normals and bounds.
- Can mark the result as no longer readable via `UploadMeshData(true)`.

Release:

- The final owned mesh instance is destroyed on unload.

`MeshLoadWrapper` can assign the mesh into:

- `MeshFilter`
- `SkinnedMeshRenderer`
- `MeshCollider`

### `MeshGOLoad`

Files:

- `MeshGOAsset.cs`
- `MeshGOLoadInfo.cs`
- `MeshGOCacheKey.cs`
- `MeshGOKeyFactory.cs`
- `MeshGOLoader.cs`
- `MeshGOLoadHandle.cs`
- `MeshGOLoadWrapper.cs`
- `MeshGOReleaser.cs`
- `MeshGOGlobal.cs`

Compile-time dependency:

- The loader expects `GLTFAST` or `COM_UNITY_CLOUD_GLTFast`.

Behavior:

- Supports only `.gltf` and `.glb`.
- Creates an inactive prototype root GameObject.
- Instantiates the imported glTF scene under that prototype.
- Collects meshes, materials, textures, and other owned Unity objects so cleanup is deterministic.

`MeshGOAsset` contains:

- `PrototypeRoot`
- `OwnedObjects`
- `Importer`

`MeshGOLoadWrapper` instantiates a live clone from `PrototypeRoot` under `_spawnParent`, which makes the cached asset reusable across consumers while each wrapper owns only its spawned instance.

This is one of the strongest parts of the solution because it separates imported asset ownership from scene instance ownership.

## Unity integration patterns

### Wrapper-driven usage

This is the preferred path for UI and scene objects.

Example:

```csharp
using DingoAssetsLoadSystem.TextLoad;
using UnityEngine;

public class ExampleTextBootstrap : MonoBehaviour
{
    [SerializeField] private TextLoadWrapper _wrapper;
    [SerializeField] private string _path;

    private void Start()
    {
        var handle = new TextLoadHandle(_path, new TextLoadInfo(
            useFileStampInKey: true,
            keepBytes: false,
            decodeText: true,
            normalizeNewLines: true));

        _wrapper.UpdateValueWithoutNotify(handle);
    }
}
```

What the wrapper adds on top of the handle:

- auto-subscription to `Flow`;
- auto-load on enable when `AutoManageLifetime` is enabled;
- auto-unload on disable and destroy;
- view-specific `ApplyView(...)` logic.

### Pure code usage

Use the handle directly if you do not need a Unity wrapper.

```csharp
using DingoAssetsLoadSystem;
using DingoAssetsLoadSystem.Texture2DLoad;
using UnityEngine;

public class DirectTextureConsumer : MonoBehaviour
{
    private Texture2DLoadHandle _handle;

    private void Awake()
    {
        _handle = new Texture2DLoadHandle(
            "https://example.com/image.png",
            new Texture2DLoadInfo(readable: true));

        _handle.Flow.AddListener(OnFlowChanged);
        _handle.LoadFor(this);
    }

    private void OnDestroy()
    {
        _handle?.UnloadFor(this);
        _handle?.Flow.RemoveListener(OnFlowChanged);
    }

    private void OnFlowChanged(AssetLoadData<Texture2D, Texture2DLoadInfo> data)
    {
        if (data.State == AssetLoadState.Loaded)
            Debug.Log($"Texture loaded: {data.Asset.width}x{data.Asset.height}");
    }
}
```

## How to extend the system

To support a new asset type, you usually add the following pieces:

1. A `TInfo` struct with all load options relevant to cache identity.
2. A cache key struct `TKey`.
3. An `ICacheKeyFactory<TKey, TInfo>`.
4. An `IAssetLoader<TAsset, TInfo>`.
5. An `IAssetReleaser<TAsset, TInfo>`.
6. A static `GlobalAssetCache<TKey, TAsset, TInfo>`.
7. A typed `AssetLoadHandle<TKey, TAsset, TInfo>`.
8. Optionally, a typed `AssetLoadDataWrapper<...>` for Unity views.

Minimal skeleton:

```csharp
public readonly struct MyLoadInfo { }

public readonly struct MyCacheKey
{
    public readonly string Path;
    public MyCacheKey(string path) => Path = path;
}

public sealed class MyKeyFactory : ICacheKeyFactory<MyCacheKey, MyLoadInfo>
{
    public MyCacheKey CreateKey(string path, MyLoadInfo info) => new(path);
}

public sealed class MyLoader : IAssetLoader<MyAsset, MyLoadInfo>
{
    public UniTask<MyAsset> LoadAsync(string path, MyLoadInfo info, CancellationToken ct)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class MyReleaser : IAssetReleaser<MyAsset, MyLoadInfo>
{
    public void Release(MyAsset asset, string path, MyLoadInfo info) { }
}

public static class MyGlobal
{
    public static readonly GlobalAssetCache<MyCacheKey, MyAsset, MyLoadInfo> Cache =
        new(new MyKeyFactory(), new MyLoader(), new MyReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
}

public sealed class MyHandle : AssetLoadHandle<MyCacheKey, MyAsset, MyLoadInfo>
{
    public MyHandle(string path, MyLoadInfo info = default, GlobalAssetCache<MyCacheKey, MyAsset, MyLoadInfo> cache = null)
        : base(path, info, cache ?? MyGlobal.Cache) { }
}
```

This extension model is a major advantage of the design: almost all policy is injected through interfaces rather than hardcoded per asset type.

## Important implementation details and constraints

- The cache is global per asset type, but it does not retain unused assets after the last receiver unloads.
- The same receiver object can hold only one active key inside the same `GlobalAssetCache`.
- `MeshLoad` and `MeshGOLoad` are intentionally limited to `.gltf` / `.glb`.
- `ParsedTextLoad` exists only when `NEWTONSOFT_EXISTS` is defined.
- Some wrappers depend on UI and helper types from `DingoUnityExtensions`.
- `LoadRoutine` uses generation checks so outdated async results are ignored safely after reload or cancellation.
- If a load finishes after its entry is no longer valid, the just-loaded asset is released immediately.
- `404` is treated as `NotFound` for request-based loaders, which is useful for view logic.
- Relative local paths are normalized with `Path.GetFullPath(...)` before use.

## Folder map

```text
DingoAssetsLoadSystem/
  AssetLoadData.cs
  AssetLoadHandle.cs
  AssetLoadDataWrapper.cs
  UnityReceiverLiveness.cs
  AudioClipLoad/
  MeshGOLoad/
  MeshLoad/
  ParsedTextLoad/
  TextLoad/
  Texture2DLoad/
  Tests/
```

`Tests/` currently contains simple MonoBehaviour smoke-test components for:

- `AudioClipLoad`
- `MeshLoad`
- `MeshGOLoad`

These are practical integration helpers rather than a full automated test suite.

## When this solution is a good fit

Use `DingoAssetsLoadSystem` when you need:

- runtime loading of external content instead of only editor-imported assets;
- multiple consumers sharing one loaded asset instance;
- deterministic cleanup without manual bookkeeping in every feature;
- Unity-friendly wrappers for UI and scene presentation;
- a reusable pattern for adding new asset types later.

In short, the main benefit of this module is not only "loading files". The real value is that it standardizes runtime asset ownership, state propagation, and cleanup across the whole project.
