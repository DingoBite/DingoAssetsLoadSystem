# DingoAssetsLoadSystem

`DingoAssetsLoadSystem` - это runtime-фреймворк для загрузки ассетов в Unity, сфокусированный на трёх вещах:

- асинхронная загрузка по файловым путям и URI;
- общий типизированный кеш для нескольких потребителей;
- детерминированная выгрузка, когда последний потребитель освобождает ресурс.

Этот файл является технической документацией уровня самого репозитория.

## Зачем нужен модуль

Модуль решает типичную для Unity задачу: нескольким системам нужно загружать один и тот же внешний ассет, реагировать на состояние загрузки и безопасно освобождать память, не дублируя в каждом месте собственную логику на `UnityWebRequest`, `glTFast` и ручной cleanup.

Вместо разрозненного asset-specific кода в UI, gameplay и scene-controller слоях проект использует единый контракт:

- `Loader` знает, как построить ассет из пути.
- `Cache` владеет общей загруженной инстанцией.
- `Handle` даёт стабильный API для gameplay-кода.
- `Wrapper` связывает состояние загрузки с Unity-компонентами и GameObject.

## Ключевые преимущества

- Единая ментальная модель для текста, текстур, аудио, распарсенных данных, мешей и инстанцируемых glTF-сцен.
- Общий кеш по типу ассета не даёт дублировать загрузки для одного и того же ключа.
- Lifetime привязан к receivers, поэтому ресурсы освобождаются автоматически, когда ими больше никто не пользуется.
- Единая state machine (`None`, `Loading`, `NotFound`, `Loaded`, `Failed`) делает UI и gameplay-код предсказуемыми.
- Сильные extension points позволяют добавить новый тип ассета без переписывания архитектуры.
- Wrapper-компоненты убирают boilerplate из `MonoBehaviour` и изолируют view-логику.
- Для локальных текстовых файлов ключ кеша может учитывать file timestamp, что даёт автоматический cache busting при изменении файла.
- Для распарсенного текста ключ кеша включает идентификатор и версию парсера, поэтому изменение parser-контракта безопасно инвалидирует кеш.

## Поддерживаемые пайплайны ассетов

| Модуль | Тип ассета | Источник | Примечания |
| --- | --- | --- | --- |
| `TextLoad` | `TextFileAsset` | локальные файлы и remote URI | Опциональное сохранение байтов, декодирование, удаление BOM, нормализация переводов строк, ключи кеша на основе file stamp |
| `ParsedTextLoad` | generic-объект `T` после парсинга | текстовый источник + парсер | Требует `NEWTONSOFT_EXISTS`; может парсить в thread pool |
| `Texture2DLoad` | `Texture2D` | локальные файлы и remote URI | Настраиваемые readability, mipmaps, linear color space |
| `AudioClipLoad` | `AudioClip` | локальные файлы и remote URI | Настраиваемые `AudioType`, streaming, compressed-in-memory mode |
| `MeshLoad` | `Mesh` | `.gltf` / `.glb` | Требует `GLTFAST`; умеет выбирать mesh, объединять меши, пересчитывать normals/bounds |
| `MeshGOLoad` | `MeshGOAsset` | `.gltf` / `.glb` | Требует `GLTFAST` или `COM_UNITY_CLOUD_GLTFast`; импортирует переиспользуемую prototype hierarchy |

## Зависимости

### Внутренние репозиторные зависимости

Репозиторий напрямую зависит от следующих AppSDK-сабмодулей:

| Зависимость | Для чего используется | Репозиторий | Ветка |
| --- | --- | --- | --- |
| `UnityBindVariables` | Core flow layer: `Bind<T>` и `IReadonlyBind<T>` внутри `GlobalAssetCache` и handle-слоя | `https://github.com/DingoBite/UnityBindVariables` | стандартная ветка репозитория (`.gitmodules` не фиксирует ветку) |
| `DingoUnityExtensions` | Wrapper layer: `ValueContainer<T>`, `RevealBehaviour`, `SingleKeyText` и сопутствующие Unity view helpers | `https://github.com/DingoBite/DingoUnityExtensions` | `dev` |

### Внешние package-зависимости

Ниже перечислены Unity packages и vendorized libraries, которые используются feature-specific загрузчиками:

- `Cysharp.Threading.Tasks` (`UniTask`) для async-загрузки, cancellation и переключения на main thread.
- Модули Unity `UnityWebRequest` для request-based загрузки текста, текстур и аудио.
- `com.unity.cloud.gltfast` для `MeshLoad` и `MeshGOLoad`.
- `com.unity.nuget.newtonsoft-json` для `ParsedTextLoad` и `TextAssetExtensions.Deserialize`.

## Архитектура

Вся система построена вокруг generic-ядра, расположенного в корне модуля:

- `AssetLoadData.cs`
- `AssetLoadHandle.cs`
- `AssetLoadDataWrapper.cs`
- `UnityReceiverLiveness.cs`

### 1. `AssetLoadData<TAsset, TInfo>`

Это state payload, который проходит через всю систему. Он содержит:

- `Path`
- `Info`
- `Asset`
- `State`
- `Error`

Состояние задаётся через `AssetLoadState`:

- `None`
- `Loading`
- `NotFound`
- `Loaded`
- `Failed`

За счёт этого каждый asset-pipeline отдаёт одинаковую форму состояния, и wrappers можно реализовывать единообразно.

### 2. `GlobalAssetCache<TKey, TAsset, TInfo>`

`GlobalAssetCache` - это главное runtime-сервисное звено. Он владеет:

- cache entries, индексированными по типизированному ключу;
- bindable `Flow` для каждого entry;
- набором активных receivers, использующих entry;
- cancellation tokens для in-flight загрузок;
- release-логикой, когда entry больше не нужен.

Ответственности:

- создать или переиспользовать cache entry для запрошенного ключа;
- стартовать загрузку, когда entry пустой или запрошен принудительный reload;
- публиковать state updates в `Flow`;
- отменять старые загрузки при reload или invalidate;
- освобождать ассет через `IAssetReleaser`, когда исчезает последний receiver;
- очищать уничтоженные Unity receivers через `UnityReceiverLiveness.IsUnityAlive`.

Важное поведение:

- Ownership по receivers отслеживается внутри конкретного cache instance.
- Вызов `Acquire(...)` для receiver сначала освобождает предыдущий ключ, который этот receiver использовал в этом кеше.
- Если последний receiver выгружается, ассет освобождается сразу, а cache entry удаляется.
- Это кеш, ориентированный на lifetime, а не долгоживущий memory pool и не LRU-кеш.

### 3. `AssetLoadHandle<TKey, TAsset, TInfo>`

Handle - это API, с которым работает gameplay-код и wrappers.

Он хранит:

- `Path`
- `Info`
- ссылку на целевой `GlobalAssetCache`
- proxy `Flow`
- набор активных receivers для данного handle

Основные операции:

- `LoadFor(object receiver)`
- `LoadFor(object receiver, bool forceReload)`
- `UnloadFor(object receiver)`
- `Invalidate()`
- `Set(string newPath, TInfo newInfo = default, bool dropToNone = true)`

Handle подписывается на flow cache entry и проксирует обновления в собственный bind. Поэтому подписчики могут слушать один стабильный `Flow`, даже если underlying cache entry изменился.

### 4. `AssetLoadDataWrapper<THandle, TAsset, TInfo>`

Wrappers - это Unity-facing адаптеры, построенные поверх `ValueContainer<THandle>`.

Они дают:

- автоматическую подписку на flow handle;
- автоматическую загрузку/выгрузку в `OnEnable` / `OnDisable`;
- ручные точки входа `LoadOnly()` и `Unload()`;
- опциональную автоматизацию lifetime через `AutoManageLifetime`;
- абстрактный метод `ApplyView(...)`, где конкретный wrapper обновляет Unity-компоненты.

Именно благодаря этому система чисто интегрируется в UI и scene objects, не дублируя state-логику.

### 5. `UnityReceiverLiveness`

Уничтоженные Unity-объекты ведут себя не как обычные `null`-ссылки, поэтому кеш использует `UnityReceiverLiveness.IsUnityAlive` для безопасного определения мёртвых Unity receivers и очистки их из entries.

Это уменьшает риск stale receiver leaks, когда `GameObject` был уничтожен без явного unload-вызова.

## Runtime flow

Стандартный сценарий выглядит так:

1. Создаётся типизированный handle, например `new Texture2DLoadHandle(path, info)`.
2. Вызывается `LoadFor(receiver)` из gameplay-кода или handle присваивается во wrapper.
3. Handle просит глобальный кеш выполнить `Acquire` для этого ассета и receiver.
4. Кеш создаёт или переиспользует entry и публикует `Loading`.
5. Asset-specific loader выполняется асинхронно.
6. Кеш публикует `Loaded`, `NotFound` или `Failed`.
7. Все подписанные потребители получают одно и то же итоговое состояние через `Flow`.
8. Когда `UnloadFor(receiver)` удаляет последнего receiver, кеш освобождает ассет и удаляет entry.

Сценарий reload:

- `LoadFor(receiver, forceReload: true)` перезапускает загрузку.
- `Invalidate()` инвалидирует текущий cache key.
- Если receivers ещё существуют, invalidate запускает новую загрузку.
- Если receivers больше нет, invalidate очищает и удаляет entry.

## Детали по модулям

### `TextLoad`

Файлы:

- `TextFileAsset.cs`
- `TextLoadInfo.cs`
- `TextCacheKey.cs`
- `TextKeyFactory.cs`
- `TextLoader.cs`
- `TextLoadHandle.cs`
- `TextLoadWrapper.cs`
- `TextReleaser.cs`
- `TextGlobal.cs`

Поведение:

- Локальные файлы читаются через `File.ReadAllBytesAsync`.
- Remote-источники загружаются через `UnityWebRequest.Get`.
- Relative paths нормализуются в полные локальные пути.
- Абсолютные non-file URI сохраняются как есть.
- `404` маппится в `NotFound`.
- Network и protocol errors превращаются в `Failed` с исключением.

`TextLoadInfo` управляет:

- `UseFileStampInKey`
- `KeepBytes`
- `DecodeText`
- `NormalizeNewLines`
- `StripBom`
- `EncodingName`

`TextFileAsset` хранит:

- resolved path;
- decoded text;
- optional raw bytes;
- encoding name;
- optional file length и timestamp последней записи.

Техническая деталь:

- Когда `UseFileStampInKey` включён для локального файла, cache key включает размер файла и `LastWriteTimeUtc.Ticks`.
- Это значит, что изменившийся файл естественным образом попадает в новый cache key без ручного versioning.

### `ParsedTextLoad`

Файлы:

- `ParsedTextLoadInfo.cs`
- `ParsedTextLoader.cs`
- `ParsedTextReleaser.cs`
- `ParsedTextGlobal.cs`
- `NewtonsoftJsonParser.cs`
- `TextAssetExtensions.cs`

Compile-time зависимость:

- Весь модуль закрыт через `NEWTONSOFT_EXISTS`.

Поведение:

- Сначала загружается raw text asset через `TextLoader`.
- Затем он парсится через `ITextParser<T>`.
- Парсинг может выполняться в thread pool, если `ParseOnThreadPool` равен `true`.

Почему важен cache key:

- Ключ распарсенного объекта содержит raw `TextCacheKey`.
- Также он включает `Id` и `Version` парсера.
- Это защищает от повторного использования распарсенного объекта, построенного по другому parser-контракту.

Это особенно полезно, когда один и тот же JSON-файл может интерпретироваться несколькими парсерами или версиями парсера.

### `Texture2DLoad`

Файлы:

- `Texture2DLoadInfo.cs`
- `Texture2DCacheKey.cs`
- `Texture2DKeyFactory.cs`
- `Texture2DLoader.cs`
- `Texture2DLoadHandle.cs`
- `Texture2DLoadWrapper.cs`
- `Texture2DReleaser.cs`
- `Texture2DGlobal.cs`

Поведение:

- Использует `UnityWebRequestTexture.GetTexture(...)`.
- Поддерживает локальные файлы и remote URI.
- Уничтожает загруженные текстуры при release.

`Texture2DLoadInfo` управляет:

- `Readable`
- `MipmapChain`
- `LinearColorSpace`
- `MipmapCount` в Unity 6+

`Texture2DLoadWrapper` уже подготовлен для типовых UI-сценариев:

- показывает и скрывает preload и not-found views;
- назначает текстуру в `RawImage`;
- обновляет `AspectRatioFitter`;
- при необходимости подстраивает `LayoutElement` по aspect ratio текстуры.

### `AudioClipLoad`

Файлы:

- `AudioClipLoadInfo.cs`
- `AudioClipCacheKey.cs`
- `AudioClipKeyFactory.cs`
- `AudioClipLoader.cs`
- `AudioClipLoadHandle.cs`
- `AudioClipLoadWrapper.cs`
- `AudioClipReleaser.cs`
- `AudioClipGlobal.cs`

Поведение:

- Использует `UnityWebRequestMultimedia.GetAudioClip(...)`.
- Поддерживает локальные файлы и remote URI.
- Возвращает `NotFound` на `404`.
- Бросает исключение для остальных ошибок загрузки.

`AudioClipLoadInfo` управляет:

- `AudioType`
- `StreamAudio`
- `CompressedInMemory`

`AudioClipReleaser` поддерживает несколько стратегий освобождения:

- `Destroy`
- `UnloadAudioDataThenDestroy`
- `UnloadAudioDataOnly`

`AudioClipLoadWrapper` умеет:

- назначать загруженный клип в `AudioSource`;
- автоматически воспроизводить его при успешной загрузке;
- останавливать и очищать клип при unload.

### `MeshLoad`

Файлы:

- `MeshLoadInfo.cs`
- `MeshCacheKey.cs`
- `MeshKeyFactory.cs`
- `MeshLoader.cs`
- `MeshLoadHandle.cs`
- `MeshLoadWrapper.cs`
- `MeshReleaser.cs`
- `MeshGlobal.cs`

Compile-time зависимость:

- Модуль закрыт флагом `GLTFAST`.

Поведение:

- Поддерживает только `.gltf` и `.glb`.
- Загружает mesh-данные через `glTFast`.
- Умеет выбирать меш по `MeshName` или `MeshIndex`.
- Умеет объединять все импортированные меши в один сгенерированный mesh.
- Может пересчитывать normals и bounds.
- Может пометить результат как no longer readable через `UploadMeshData(true)`.

Release:

- Итоговый owned mesh instance уничтожается при unload.

`MeshLoadWrapper` может назначать меш в:

- `MeshFilter`
- `SkinnedMeshRenderer`
- `MeshCollider`

### `MeshGOLoad`

Файлы:

- `MeshGOAsset.cs`
- `MeshGOLoadInfo.cs`
- `MeshGOCacheKey.cs`
- `MeshGOKeyFactory.cs`
- `MeshGOLoader.cs`
- `MeshGOLoadHandle.cs`
- `MeshGOLoadWrapper.cs`
- `MeshGOReleaser.cs`
- `MeshGOGlobal.cs`

Compile-time зависимость:

- Загрузчик ожидает `GLTFAST` или `COM_UNITY_CLOUD_GLTFast`.

Поведение:

- Поддерживает только `.gltf` и `.glb`.
- Создаёт неактивный prototype root `GameObject`.
- Инстанцирует импортированную glTF-сцену под этим prototype.
- Собирает meshes, materials, textures и другие owned Unity objects, чтобы cleanup был детерминированным.

`MeshGOAsset` содержит:

- `PrototypeRoot`
- `OwnedObjects`
- `Importer`

`MeshGOLoadWrapper` создаёт живой клон из `PrototypeRoot` под `_spawnParent`, благодаря чему кешированный asset переиспользуется между consumers, а сам wrapper владеет только своей scene instance.

Это одна из самых сильных частей решения, потому что она разделяет ownership импортированного ассета и ownership scene instance.

## Паттерны интеграции с Unity

### Использование через wrapper

Это предпочтительный путь для UI и scene objects.

Пример:

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

Что wrapper добавляет поверх handle:

- auto-subscription к `Flow`;
- auto-load при enable, если включён `AutoManageLifetime`;
- auto-unload при disable и destroy;
- view-specific логику в `ApplyView(...)`.

### Использование напрямую из кода

Используйте handle напрямую, если Unity-wrapper не нужен.

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

## Как расширять систему

Чтобы поддержать новый тип ассета, обычно добавляются следующие части:

1. `TInfo` struct со всеми load options, важными для cache identity.
2. Структура cache key `TKey`.
3. `ICacheKeyFactory<TKey, TInfo>`.
4. `IAssetLoader<TAsset, TInfo>`.
5. `IAssetReleaser<TAsset, TInfo>`.
6. Статический `GlobalAssetCache<TKey, TAsset, TInfo>`.
7. Типизированный `AssetLoadHandle<TKey, TAsset, TInfo>`.
8. Опционально типизированный `AssetLoadDataWrapper<...>` для Unity view-слоя.

Минимальный skeleton:

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

Такая модель расширения - одно из главных преимуществ дизайна: почти вся policy-логика инжектируется через интерфейсы, а не хардкодится отдельно под каждый тип ассета.

## Важные детали реализации и ограничения

- Кеш глобален на тип ассета, но не удерживает неиспользуемые ресурсы после unload последнего receiver.
- Один и тот же receiver object может держать только один активный ключ внутри одного `GlobalAssetCache`.
- `MeshLoad` и `MeshGOLoad` намеренно ограничены форматами `.gltf` / `.glb`.
- `ParsedTextLoad` существует только при определённом `NEWTONSOFT_EXISTS`.
- Часть wrappers зависит от UI- и helper-типов из `DingoUnityExtensions`.
- `LoadRoutine` использует generation checks, поэтому устаревшие async-результаты безопасно игнорируются после reload или cancellation.
- Если загрузка завершилась уже после того, как entry стал невалидным, только что загруженный ассет сразу освобождается.
- Для request-based loaders код `404` трактуется как `NotFound`, что удобно для view-логики.
- Relative local paths нормализуются через `Path.GetFullPath(...)` до начала использования.

## Карта папок

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

`Tests/` сейчас содержит простые `MonoBehaviour` smoke-test компоненты для:

- `AudioClipLoad`
- `MeshLoad`
- `MeshGOLoad`

Это практические integration helpers, а не полноценный automated test suite.

## Когда решение особенно уместно

Используйте `DingoAssetsLoadSystem`, когда вам нужны:

- runtime-загрузка внешнего контента, а не только editor-imported assets;
- несколько consumers, разделяющих одну загруженную asset instance;
- детерминированный cleanup без ручного bookkeeping в каждой feature;
- Unity-friendly wrappers для UI и scene presentation;
- переиспользуемый паттерн для добавления новых типов ассетов в будущем.

Иными словами, основная ценность модуля не только в том, что он "умеет загружать файлы". Главное преимущество в том, что он стандартизирует runtime ownership ассетов, распространение состояний и cleanup по всей системе.
