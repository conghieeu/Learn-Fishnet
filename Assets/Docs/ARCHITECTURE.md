# Kiến trúc Dự án Mellow Abelson

> Co-op Horror Survival Multiplayer (Unity 6 + FishNet + NodeCanvas + Easy Save 3)

---

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Cấu trúc thư mục](#2-cấu-trúc-thư-mục)
3. [Namespace Design](#3-namespace-design)
4. [Kiến trúc Hệ thống](#4-kiến-trúc-hệ-thống)
5. [Data Flow](#5-data-flow)
6. [Design Patterns](#6-design-patterns)
7. [Architectural Rules](#7-architectural-rules)
8. [Migration Plan](#8-migration-plan)
9. [Dependency Graph](#9-dependency-graph)

---

## 1. Tổng quan

### Công nghệ

| Công nghệ | Mục đích |
|-----------|----------|
| **Unity 6 + URP** | Game Engine |
| **FishNet** | Networking framework (RPCs, SyncVars, SyncLists) |
| **NodeCanvas (ParadoxNotion)** | AI Behavior Trees, FSMs, Dialogue Trees |
| **Easy Save 3 (ES3)** | Save/Load serialization |
| **Unity Input System** | Player input |
| **Quantum Console (QFSW)** | In-game debug console |
| **Cinemachine** | Camera system |
| **PicoShot Localization** | Multi-language support |

### Nguyên tắc kiến trúc

- **Server Authority**: Mọi game state chỉ do server quyết định
- **Event-Driven**: Cross-system communication qua ScriptableObject events
- **Interface-based**: IInteractable cho mọi object tương tác
- **Loosely Coupled**: Các module giao tiếp qua ServiceLocator và Event System
- **Server-Authoritative Saving**: Easy Save 3 chỉ chạy trên server

---

## 2. Cấu trúc thư mục

```
Assets/
  _Scripts/                              # ALL game code (MellowAbelson namespace)
    Core/                                # Foundation layer (zero gameplay logic)
      Boot/
        GameBootstrap.cs                 # Entry point, khởi tạo ServiceLocator
        ServerInitializer.cs             # Khởi tạo server
        ClientInitializer.cs             # Khởi tạo client
      Services/
        ServiceLocator.cs                # DI container singleton
        IService.cs                      # Interface cho services
      Network/
        ConnectionManager.cs             # Host/Client/Server connection
        RpcHelper.cs                     # FishNet RPC helpers
      Input/
        InputManager.cs                  # Unity Input System wrapper
        InputSystem_Actions.cs           # Auto-generated từ .inputactions
        InputSystem_Actions.inputactions # Input bindings
      Events/
        GameEventSO.cs                   # ScriptableObject event base
        GameEventWithPayloadSO.cs        # Event với payload (generic)
        GameEventListener.cs             # Listener component
      Save/
        SaveManager.cs                   # ES3 wrapper (server-authoritative)
        ISaveable.cs                     # Interface cho object cần lưu
        SaveKeys.cs                      # Constants cho save keys
      Console/
        ConsoleCommands.cs               # Quantum Console debug commands

    Player/                              # Player systems
      Movement/
        PlayerMovementController.cs      # WASD movement + network sync
        StaminaSystem.cs                 # Thể lực (sprint drain + regen)
      Look/
        PlayerLookController.cs          # FPS mouse-look (FOV, smoothing)
        PlayerCameraController.cs        # Cinemachine camera ownership
      Interaction/
        PlayerInteractor.cs              # Player raycast tìm IInteractable
        IInteractable.cs                 # Interface tương tác
      Inventory/
        PlayerInventory.cs               # SyncList<InventorySlot> + ServerRpc ops
        InventorySlotData.cs             # Struct: itemId, stackCount, durability
        HotbarController.cs              # SyncVar selected slot
      Equipment/
        EquipmentHandler.cs              # SyncList equipment slots
        EquipmentSlot.cs                 # Struct
      Stats/
        PlayerStats.cs                   # SyncVar stats + modifiers
        StatModifier.cs                  # Stat modifier (additive/multiplicative)
        StatType.cs                      # Enum: Health, Stamina, MoveSpeed, ...
      Health/
        PlayerHealth.cs                  # SyncVar health, ServerRpc TakeDamage
        DamageInfo.cs                    # Struct sát thương
      Animation/
        PlayerAnimatorController.cs      # Networked animation
      Spectator/
        SpectatorController.cs           # Chế độ xem khi chết

    Gameplay/                            # World & game systems
      Interaction/
        InteractableBase.cs              # Abstract base cho IInteractable objects
        PickupItem.cs                    # Item trên mặt đất
        DoorController.cs                # Cửa
        TerminalController.cs            # Máy tính điều khiển
        HazardBase.cs                    # Bẫy (turret, mine, ...)
      Items/
        ItemDatabase.cs                  # Lookup item theo ID
        ItemDataSO.cs                    # ScriptableObject định nghĩa item
        ItemInstance.cs                  # Runtime item data (non-networked)
      Round/
        RoundManager.cs                  # State machine: Lobby → Landing → Exploration → ...
        RoundState.cs                    # Enum các state
        GameTimer.cs                     # SyncVar timer
        QuotaSystem.cs                   # SyncVar quota + collected
        ScoreManager.cs                  # SyncVar score
      Store/
        StoreManager.cs                  # Server-authoritative mua bán
        StoreItemDataSO.cs               # ScriptableObject item trong store
      Scrap/
        ScrapSpawner.cs                  # Procedural spawn scrap items
        ScrapValueData.cs                # Weighted random values
      Persistence/
        WorldPersistenceManager.cs       # Quản lý save/load world state qua ES3
        WorldObjectSaveData.cs           # Dữ liệu lưu của object

    AI/                                  # NodeCanvas AI
      BehaviourTrees/
        AggressiveBehaviourTree.asset    # Behavior tree cho monster hung hăng
        IntimidatedBehaviourTree.asset   # Behavior tree cho monster nhát gan
      Blackboard/
        AIBlackboardKeys.cs              # Constants cho blackboard keys
      Sensors/
        SoundDetector.cs                 # Sound detection AI
        SightSensor.cs                   # Sight detection (dùng CheckFieldOfView)
      State/
        MonsterController.cs             # NetworkBehaviour + BehaviourTreeOwner
        MonsterState.cs                  # Enum: Idle, Patrol, Chase, Attack, Flee, Dead
      CustomNodes/
        Conditions/
          CheckFieldOfView.cs            # FOV check condition cho AI
        Actions/
          SetTimeScaleCurve.cs           # Smooth slow-motion action

    UI/                                  # UI layer
      HUD/
        HUDController.cs
        InteractionPromptUI.cs
        CrosshairController.cs
      Inventory/
        InventoryUI.cs                   # SyncList.OnChange → UI update
        InventorySlotUI.cs
        DragDropHandler.cs
        HotbarUI.cs
      Menu/
        MainMenuController.cs
        PauseMenuController.cs
      Lobby/
        LobbyUI.cs
      Terminal/
        TerminalUI.cs
      Notification/
        NotificationSystem.cs

    Networking/                          # FishNet game-specific code
      Lobby/
        LobbyManager.cs                  # SyncList<LobbyPlayerData>
        LobbyPlayerData.cs               # Struct
      Spawner/
        PlayerSpawnManager.cs            # Unified spawner
        SpawnPoint.cs
      Scene/
        SceneLoadManager.cs              # Scene loading cho connection/global
        LoadingScreenController.cs       # Singleton loading screen
      AntiCheat/
        ServerValidator.cs               # Validate ServerRpc inputs

    Data/                                # ScriptableObjects
      Items/
        ItemDataSO.cs
        EquipmentDataSO.cs
        ConsumableDataSO.cs
      AI/
        MonsterConfigSO.cs               # Config cho từng loại monster
      Round/
        RoundConfigSO.cs
        LevelConfigSO.cs
      Store/
        StoreCatalogSO.cs
      Player/
        PlayerConfigSO.cs
      Network/
        LobbyConfigSO.cs

  # Third-party (giữ nguyên)
  FishNet/
  ParadoxNotion/
  Plugins/

  # Sẽ deprecate sau khi migrate xong
  Basic/
  1-Scripts/
```

### Sơ đồ quan hệ các module

```
_Scripts/
│
├── Core/              ← Không phụ thuộc gì ngoài Unity + FishNet
│   ├── Services/      ServiceLocator, IService
│   ├── Events/        GameEventSO (decouple cross-system)
│   ├── Save/          ES3 wrapper
│   └── Input/         Input System
│
├── Data/              ← ScriptableObjects (dữ liệu tĩnh, config)
│
├── Player/            ← Phụ thuộc Core + FishNet
│   ├── Movement/      PlayerMovementController + StaminaSystem
│   ├── Look/          LookController + Camera
│   ├── Interaction/   PlayerInteractor + IInteractable
│   ├── Inventory/     PlayerInventory, HotbarController
│   ├── Equipment/     EquipmentHandler
│   ├── Stats/         PlayerStats
│   ├── Health/        PlayerHealth, DamageInfo
│   ├── Animation/
│   └── Spectator/
│
├── Gameplay/          ← Phụ thuộc Player Interaction + Core.Save
│   ├── Interaction/   InteractableBase, Door, Terminal, Hazard
│   ├── Items/         ItemDatabase, ItemDataSO
│   ├── Round/         RoundManager, Quota, Timer
│   ├── Store/         StoreManager
│   ├── Scrap/         ScrapSpawner
│   └── Persistence/   WorldPersistenceManager (ES3)
│
├── AI/                ← Phụ thuộc Gameplay.Round + NodeCanvas
│   ├── Sensors/       SoundDetector, SightSensor
│   ├── State/         MonsterController
│   └── CustomNodes/   CheckFieldOfView, SetTimeScaleCurve
│
├── Networking/        ← Phụ thuộc Core + FishNet
│   ├── Lobby/         LobbyManager
│   ├── Spawner/       PlayerSpawnManager
│   ├── Scene/         SceneLoadManager
│   └── AntiCheat/     ServerValidator
│
└── UI/                ← Phụ thuộc Player + Gameplay (read-only)
    ├── HUD/
    ├── Inventory/
    ├── Menu/
    ├── Lobby/
    ├── Terminal/
    └── Notification/
```

---

## 3. Namespace Design

```
Root: MellowAbelson

Core:
  MellowAbelson.Core.Services      → ServiceLocator, IService
  MellowAbelson.Core.Events        → GameEventSO, GameEventListener
  MellowAbelson.Core.Save          → SaveManager, ISaveable
  MellowAbelson.Core.Input         → InputManager
  MellowAbelson.Core.Network       → ConnectionManager, RpcHelper
  MellowAbelson.Core.Console       → ConsoleCommands

Player:
  MellowAbelson.Player.Movement    → PlayerMovementController, StaminaSystem
  MellowAbelson.Player.Look        → PlayerLookController, PlayerCameraController
  MellowAbelson.Player.Interaction → PlayerInteractor, IInteractable
  MellowAbelson.Player.Inventory   → PlayerInventory, HotbarController
  MellowAbelson.Player.Equipment   → EquipmentHandler
  MellowAbelson.Player.Stats       → PlayerStats
  MellowAbelson.Player.Health      → PlayerHealth, DamageInfo
  MellowAbelson.Player.Spectator   → SpectatorController

Gameplay:
  MellowAbelson.Gameplay.Interaction  → InteractableBase, DoorController, TerminalController
  MellowAbelson.Gameplay.Items        → ItemDatabase, ItemDataSO
  MellowAbelson.Gameplay.Round        → RoundManager, QuotaSystem, GameTimer
  MellowAbelson.Gameplay.Store        → StoreManager
  MellowAbelson.Gameplay.Scrap        → ScrapSpawner
  MellowAbelson.Gameplay.Persistence  → WorldPersistenceManager

AI:
  MellowAbelson.AI.State            → MonsterController
  MellowAbelson.AI.Sensors          → SoundDetector, SightSensor
  MellowAbelson.AI.Blackboard       → AIBlackboardKeys
  MellowAbelson.AI.CustomNodes      → CheckFieldOfView, SetTimeScaleCurve

Networking:
  MellowAbelson.Networking.Lobby    → LobbyManager
  MellowAbelson.Networking.Spawner  → PlayerSpawnManager
  MellowAbelson.Networking.Scene    → SceneLoadManager
  MellowAbelson.Networking.AntiCheat → ServerValidator

UI:
  MellowAbelson.UI.HUD
  MellowAbelson.UI.Inventory
  MellowAbelson.UI.Menu
  MellowAbelson.UI.Lobby
  MellowAbelson.UI.Terminal
  MellowAbelson.UI.Notification

Data:
  MellowAbelson.Data.Items
  MellowAbelson.Data.AI
  MellowAbelson.Data.Round
  MellowAbelson.Data.Store
  MellowAbelson.Data.Player
  MellowAbelson.Data.Network
```

---

## 4. Kiến trúc Hệ thống

### 4a. Core - Boot & Services

```csharp
// GameBootstrap (scene đầu tiên)
GameBootstrap.Awake()
  → ServiceLocator.Register<SaveManager>(saveManager)
  → ServiceLocator.Register<InputManager>(inputManager)
  // Thêm services khác khi cần

// ServiceLocator pattern
ServiceLocator.Get<SaveManager>().Save("key", data);
ServiceLocator.Get<InputManager>().GetMove();
```

**Luồng khởi tạo:**
```
GameBootstrap (scene Bootstrap)
  ↓ Awake()
RegisterServices()
  ↓
Load MainMenu scene (hoặc Lobby)
  ↓
Player chọn Host/Join
  ↓
Load Gameplay scene → Player spawn
```

### 4b. Event System (ScriptableObject)

```csharp
// Định nghĩa event
[CreateAssetMenu(menuName = "MellowAbelson/Game Event")]
public class GameEventSO : ScriptableObject
{
    public UnityEvent OnRaised { get; private set; }
    public void Raise() => OnRaised?.Invoke();
}

// Listener (gắn trên GameObject)
public class GameEventListener : MonoBehaviour
{
    public GameEventSO Event;
    public UnityEvent Response;
    // Subscribe trong OnEnable, unsubscribe trong OnDisable
}
```

**Các event chính:**
- `OnPlayerDamaged`
- `OnPlayerDied`
- `OnItemPickedUp`
- `OnRoundStateChanged`
- `OnQuotaReached`
- `OnMonsterDetectedPlayer`
- `OnTerminalCommand`

### 4c. Interaction System (IInteractable)

```csharp
public interface IInteractable
{
    string InteractionPrompt { get; }     // "Press E to pick up"
    float InteractionRange { get; }
    bool CanInteract(GameObject interactor);
    void OnInteract(GameObject interactor);
    void OnInteractHold(GameObject interactor, float holdProgress);
    void OnInteractCancel(GameObject interactor);
}

public abstract class InteractableBase : NetworkBehaviour, IInteractable
{
    // Base implementation, các class con override OnInteract
}

// Các implement:
// PickupItem : InteractableBase    → nhặt item
// DoorController : InteractableBase → mở cửa
// TerminalController : InteractableBase → dùng terminal
// HazardBase : InteractableBase → bẫy (turret, mine)
```

**Luồng Interaction:**
```
CLIENT                          SERVER
  |                               |
  |-- Update() → Raycast          |
  |-- Tìm IInteractable            |
  |-- Show prompt UI              |
  |-- Player nhấn E               |
  |-- [ServerRpc] ServerInteract  |
  |                               |-- Validate distance
  |                               |-- Resolve netId → IInteractable
  |                               |-- OnInteract()
  |                               |-- [ObserversRpc] sync
```

### 4d. Inventory & Equipment

```csharp
public class PlayerInventory : NetworkBehaviour, ISaveable
{
    public SyncList<InventorySlotData> Slots;
    
    [ServerRpc] ServerPickupItem(int itemId, int count);
    [ServerRpc] ServerDropItem(int slotIndex);
    [ServerRpc] ServerMoveItem(int fromSlot, int toSlot);
    [ServerRpc] ServerUseItem(int slotIndex);
}

[Serializable]
public struct InventorySlotData
{
    public int ItemId;
    public int StackCount;
    public float CurrentDurability;
}

public class HotbarController : NetworkBehaviour
{
    [SyncVar] int SelectedSlotIndex;
    [ServerRpc] ServerSelectSlot(int index);
    [ServerRpc] ServerScroll(int direction);
}
```

### 4e. Stats & Equipment

```csharp
public class PlayerStats : NetworkBehaviour
{
    [SyncVar] float _baseMoveSpeed = 5f;
    [SyncVar] float _moveSpeedMultiplier = 1f;
    [SyncVar] float _damageMultiplier = 1f;
    [SyncVar] float _defense = 0f;

    // Cộng dồn modifiers từ equipment
    void Recalculate() { /* sum base + equipment modifiers */ }
}

public class EquipmentHandler : NetworkBehaviour
{
    SyncList<EquipmentSlotData> EquippedSlots;
    [ServerRpc] Equip(int inventorySlotIndex);
    [ServerRpc] Unequip(int equipmentSlotIndex);
}
```

### 4f. Health & Damage

```csharp
public class PlayerHealth : NetworkBehaviour, ISaveable
{
    [SyncVar] float _currentHealth;
    [SyncVar] bool _isDead;

    [ServerRpc] void ServerTakeDamage(DamageInfo damage)
    {
        // Validate damage
        // Apply damage
        // [ObserversRpc] OnDamageTaken / OnDeath
    }
}

[Serializable]
public struct DamageInfo
{
    public float Amount;
    public DamageType Type;  // Physical, Fall, Fire, Poison, Explosion, Crush, Drowning
    public int SourceNetId;
    public Vector3 HitPoint;
    public Vector3 HitDirection;
}
```

### 4g. AI - Monster Controller

```csharp
public class MonsterController : NetworkBehaviour
{
    BehaviourTreeOwner _behaviourTree;  // NodeCanvas
    [SyncVar] MonsterState _currentState;

    // Server: Set state, sync qua ObserversRpc
    [Server] void SetState(MonsterState newState);

    // Client: nghe sound, báo server
    [ServerRpc(RequireOwnership = false)]
    void ReportSoundHeard(Vector3 position, float intensity);
}

public enum MonsterState { Idle, Roaming, Investigating, Chasing, Attacking, Fleeing, Dead }
```

**NodeCanvas Custom Nodes:**
- `CheckFieldOfView` - Kiểm tra target trong tầm nhìn (khoảng cách, góc, vật cản)
- `SetTimeScaleCurve` - Smooth slow-motion effect

**AI Sensors:**
- `SoundDetector` - Monster nghe thấy tiếng động (footstep, gunshot, voice, ...)
- `SightSensor` - Monster nhìn thấy player (dùng CheckFieldOfView)

### 4h. Round Manager (State Machine)

```csharp
public class RoundManager : NetworkBehaviour, ISaveable
{
    [SyncVar] RoundState _currentState;
    [SyncVar] float _roundTimeRemaining;
    [SyncVar] int _currentDay;
    [SyncVar] float _currentQuota;
    [SyncVar] float _totalScrapCollected;

    [Server] void StartRound();
    [Server] void EndRound();
    [Server] void AddScrap(float value);
}

public enum RoundState { Lobby, Landing, Exploration, ExtractionPhase, BetweenRounds, GameOver }
```

**State Machine Flow:**
```
Lobby
  |-- all players ready
  v
Landing
  |-- timer expires
  v
Exploration
  |-- all players in ship OR timer expires
  v
ExtractionPhase
  |-- ship leaves
  v
BetweenRounds
  |-- store closed, next day
  v
Landing (hoặc GameOver nếu không đạt quota)
```

---

## 5. Data Flow

### 5a. Network RPC Flow (tổng quát)

```
CLIENT                          SERVER                         OTHER CLIENTS
  |                               |                               |
  |-- [ServerRpc] Request  ---->  |                               |
  |                               |-- Validate input              |
  |                               |-- Process (authoritative)     |
  |                               |-- Update SyncVars             |
  |   (SyncVar auto-sync)  <---- |                               |
  |                               |-- [ObserversRpc] Broadcast -->|-- Update visuals
  |                               |-- SyncList.OnChange --------->|-- UI updates
```

### 5b. Easy Save 3 Integration (Server-Authoritative)

```
SERVER (host):
  OnRoundEnd() OR OnServerShutdown():
    1. WorldPersistenceManager.SaveAll()
       → Thu thập tất cả ISaveable objects
       → SaveManager.Save("WorldState_Day_{day}", worldData)
    2. For mỗi player:
       → SaveManager.Save("PlayerData_{clientId}", playerData)
    3. SaveManager.Save("RoundState", roundData)
       → CurrentDay, Quota, v.v.

  OnServerStart():
    1. SaveManager.Load("RoundState") → resume game
    2. SaveManager.Load("WorldState_Day_{day}") → restore world
    3. Player connect → load("PlayerData_{id}") → set SyncVars

CLIENT (non-host):
  - KHÔNG save/load game state
  - Chỉ save local settings (volume, graphics, keybindings)
  - Mọi state đồng bộ qua FishNet SyncVars/SyncLists/RPCs
```

### 5c. UI Update Flow (SyncList/SyncVar Events)

```
PlayerInventory.Slots (SyncList<InventorySlotData>)
  |
  |-- OnChange event fires on ALL clients when server modifies
  |
  v
InventoryUI.OnInventoryChanged()
  - Đọc SyncList state
  - Rebuild slot visuals
  - DragDropHandler cập nhật reference
  |
  v
HotbarUI.OnHotbarChanged()
  - Subscribe HotbarController.SelectedSlotIndex.OnChange
  - Highlight active slot
```

---

## 6. Design Patterns

| Pattern | Vị trí | Mục đích |
|---------|--------|----------|
| **Service Locator** | `Core.Services` | Đăng ký/truy xuất services, tránh Find/Singleton tràn lan |
| **Event-Driven** | `Core.Events` (GameEventSO) | Decouple cross-system: 1 event → nhiều listener |
| **State Machine** | `RoundManager`, `MonsterController` | Quản lý state rõ ràng, dễ mở rộng, dễ debug |
| **Strategy** | AI (NodeCanvas) | Mỗi monster type = 1 BehaviorTree asset riêng biệt |
| **Interface** | `IInteractable` | Tất cả object tương tác đều implement, Player chỉ cần gọi 1 hàm |
| **Observer** | `SyncList.OnChange` | UI tự động cập nhật khi data thay đổi |
| **Factory** | `ItemFactory` / `ScrapSpawner` | Tạo object từ ID, weighted random |

### Event-Driven Cross-System Example:

```
[PlayerDied Event]
  → RoundManager: cập nhật score, check game-over
  → BodyRetrievalHandler: spawn corpse
  → SpectatorController: activate spectator mode
  → UIManager: show death screen

[RoundStateChanged Event]
  → AIDirector: adjust monster spawn rates
  → StoreManager: open/close store
  → HUDController: update timer display
  → WorldPersistenceManager: trigger save
```

---

## 7. Architectural Rules

### Quy tắc bất di bất dịch

1. **Server Authority**
   - Mọi game state chỉ do server quyết định
   - Client chỉ gửi request qua `[ServerRpc]`
   - Server validate và broadcast kết quả

2. **SyncVar/SyncList cho persistent state**
   - Dùng cho state cần đồng bộ với late-join clients
   - `[ObserversRpc]` chỉ dùng cho transient events (animation, VFX, sound)

3. **ES3 saves chỉ trên server**
   - Client chỉ save local settings (audio, graphics, keybindings)
   - Mọi gameplay state đồng bộ qua FishNet

4. **ScriptableObject là read-only data**
   - Không chứa runtime state
   - Dùng để config/tuning (damage values, spawn rates, v.v.)

5. **IInteractable cho MỌI object tương tác**
   - Không hard-code logic tương tác theo type
   - Player chỉ gọi `OnInteract()`, không cần biết object là gì

6. **GameEventSO cho cross-system communication**
   - Tránh reference trực tiếp giữa các module
   - 1 system raise event, N system listen

7. **Không dùng FindObjectOfType**
   - Dùng ServiceLocator hoặc inspector references
   - Performance + maintainability

### Coding Conventions

```csharp
// Namespace theo module
namespace MellowAbelson.Player.Health { ... }

// Class naming: PascalCase, rõ ràng
public class PlayerHealth : NetworkBehaviour { ... }

// ServerRpc prefix: "Server"
[ServerRpc] void ServerTakeDamage(DamageInfo damage);

// ObserversRpc prefix: "Observers"
[ObserversRpc] void ObserversOnDamageTaken(DamageInfo damage);

// TargetRpc prefix: "Target"
[TargetRpc] void TargetReceiveMessage(string message);

// SyncVar fields: _ prefix
[SyncVar] private float _currentHealth;

// Public properties: không prefix
public float CurrentHealth => _currentHealth;
```

---

## 8. Migration Plan

### Phase 1: Tạo cấu trúc mới (✅ Hoàn thành)
- Tạo `Assets/_Scripts/` với đầy đủ sub-folders
- Tạo core files: ServiceLocator, IService, GameEventSO, SaveManager
- Tạo Player/AI/Gameplay/Networking systems mới

### Phase 2: Migrate từ `1-Scripts/` (✅ Hoàn thành)
| File cũ | File mới | Trạng thái |
|---------|----------|------------|
| `1-Scripts/InputSystem_Actions.cs` | `_Scripts/Core/Input/` | Giữ nguyên bản gốc |
| `1-Scripts/InputSystem_Actions.inputactions` | `_Scripts/Core/Input/` | ✅ Copied |
| `1-Scripts/StandaloneLookController.cs` | `_Scripts/Player/Look/PlayerLookController.cs` | ⏳ Chờ refactor |
| `1-Scripts/CustomNodes/CheckFieldOfView.cs` | `_Scripts/AI/CustomNodes/Conditions/` | ✅ Migrated |
| `1-Scripts/CustomNodes/SetTimeScaleCurve.cs` | `_Scripts/AI/CustomNodes/Actions/` | ✅ Migrated |

### Phase 3: Migrate từ `Basic/Scripts/` (⏳ Chờ)
| File cũ | File mới | Ghi chú |
|---------|----------|---------|
| `Basic/Scripts/ConnectionManager.cs` | `_Scripts/Core/Network/ConnectionManager.cs` | ✅ Created |
| `Basic/Scripts/PlayerMovement.cs` | `_Scripts/Player/Movement/PlayerMovementController.cs` | ⏳ Chờ refactor (thêm stamina, Input System) |
| `Basic/Scripts/PlayerCamera.cs` | `_Scripts/Player/Look/PlayerCameraController.cs` | ⏳ Chờ |
| `Basic/Scripts/PlayerCubeCreator.cs` | **XÓA** (debug code) | ⏳ |
| `Basic/Scripts/FishNetTestCode.cs` | **XÓA** (debug code) | ⏳ |
| `Basic/Scripts/SyncMaterialColor.cs` | `_Scripts/Network/Sync/` | ⏳ |
| `Basic/Scripts/Load scene/LoadGameplayScene.cs` | `_Scripts/Networking/Scene/SceneLoadManager.cs` | ✅ Created |
| `Basic/Scripts/Load scene/LoadingScreen.cs` | `_Scripts/Networking/Scene/LoadingScreenController.cs` | ✅ Created |
| `Basic/Scripts/Load scene/LoadingScreenSceneProcessor.cs` | `_Scripts/Networking/Scene/` | ⏳ |
| 5 spawner variants | `_Scripts/Networking/Spawner/PlayerSpawnManager.cs` | ✅ Created |

### Phase 4: Deprecate (⏳)
- Update scene/prefab references → dùng file mới trong `_Scripts/`
- Đổi tên `1-Scripts/` → `1-Scripts_DEPRECATED/`
- Đổi tên `Basic/` → `Basic_DEPRECATED/`
- Xóa hẳn sau 1 vòng test đầy đủ

---

## 9. Dependency Graph (Build Order)

```
1.  Core.Services        (zero dependencies)
2.  Core.Events          (zero dependencies)
3.  Core.Input           (Unity Input System)
4.  Core.Save            (ES3)
5.  Core.Network         (FishNet)
6.  Data (tất cả SO)     (Core)
7.  Player.Movement      (Core.Input + FishNet)
8.  Player.Look          (Core.Input)
9.  Player.Interaction   (FishNet)
10. Player.Inventory     (FishNet)
11. Player.Health        (FishNet)
12. Gameplay.Items       (Player.Interaction)
13. Gameplay.Interaction (Player.Interaction + Items)
14. Gameplay.Round       (nhiều hệ thống)
15. Gameplay.Persistence (Core.Save)
16. AI.Sensors           (FishNet + NodeCanvas)
17. AI.State             (Gameplay.Round + NodeCanvas)
18. UI (tất cả)          (Player + Gameplay)
19. Networking.Lobby     (FishNet)
20. Networking.Spawner   (FishNet + Data)
21. Networking.AntiCheat (tất cả gameplay)
```

---

## 10. File hiện tại trong `Assets/_Scripts/`

```
_Scripts/
├── Core/
│   ├── Boot/GameBootstrap.cs
│   ├── Services/IService.cs, ServiceLocator.cs
│   ├── Network/ConnectionManager.cs
│   ├── Input/InputManager.cs, InputSystem_Actions.inputactions
│   ├── Events/GameEventSO.cs, GameEventWithPayloadSO.cs, GameEventListener.cs
│   ├── Save/SaveManager.cs, ISaveable.cs, SaveKeys.cs
│   └── Console/ConsoleCommands.cs
│
├── Player/
│   ├── Movement/StaminaSystem.cs
│   ├── Interaction/IInteractable.cs, PlayerInteractor.cs
│   ├── Inventory/PlayerInventory.cs, InventorySlotData.cs, HotbarController.cs
│   ├── Health/PlayerHealth.cs, DamageInfo.cs
│   └── Stats/PlayerStats.cs, StatType.cs, StatModifier.cs
│
├── AI/
│   ├── Blackboard/AIBlackboardKeys.cs
│   ├── Sensors/SoundDetector.cs
│   ├── State/MonsterController.cs
│   └── CustomNodes/Conditions/CheckFieldOfView.cs, Actions/SetTimeScaleCurve.cs
│
├── Gameplay/
│   ├── Interaction/InteractableBase.cs, PickupItem.cs
│   ├── Items/ItemDataSO.cs, ItemDatabase.cs
│   ├── Round/RoundManager.cs
│   └── Persistence/WorldPersistenceManager.cs
│
└── Networking/
    ├── Scene/SceneLoadManager.cs, LoadingScreenController.cs
    └── Spawner/PlayerSpawnManager.cs
```

---

*Document generated: 2026-04-29*
*Architecture design for Mellow Abelson — Co-op Horror Survival Multiplayer*
