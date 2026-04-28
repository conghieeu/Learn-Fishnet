# Kiến trúc Dự án Mellow Abelson

> Co-op Horror Survival Multiplayer (Unity 6 + FishNet + Easy Save 3)
> AI: Pure C# Behaviour Tree (không NodeCanvas)

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
- **Pure C# AI**: Behaviour Tree tự viết, không visual scripting

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

    AI/                                  # Pure C# AI (không NodeCanvas)
      BehaviourTree/                     # Behaviour Tree engine tự viết
        BtNode.cs                        # Base node: Running/Success/Failure
        SelectorNode.cs                  # OR logic: chạy con đầu tiên success
        SequenceNode.cs                  # AND logic: chạy đến khi failure
        ActionNode.cs                    # Leaf node: thực thi hành động
        ConditionNode.cs                 # Leaf node: kiểm tra điều kiện
        BehaviourTreeRunner.cs           # Runtime runner, tick theo interval
      Sensors/
        SoundDetector.cs                 # Sound detection AI
        SightSensor.cs                   # Sight detection (dùng CheckFieldOfView)
      State/
        MonsterController.cs             # NetworkBehaviour + BT runner
        MonsterState.cs                  # Enum: Idle, Roam, Investigate, Chase, Attack, Flee, Dead
        MonsterStateMachine.cs           # Finite State Machine cho AI
      Utilities/
        CheckFieldOfView.cs              # Utility: FOV check (raycast, angle)
        SetTimeScaleCurve.cs             # Utility: smooth slow-motion

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
  Plugins/

  # Sẽ deprecate sau khi migrate xong
  ParadoxNotion/
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
├── AI/                ← Pure C# (không phụ thuộc NodeCanvas)
│   ├── BehaviourTree/ BtNode, Selector, Sequence, Action, Condition, Runner
│   ├── Sensors/       SoundDetector, SightSensor
│   ├── State/         MonsterController, MonsterStateMachine
│   └── Utilities/     CheckFieldOfView, SetTimeScaleCurve
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
  MellowAbelson.AI.BehaviourTree    → BtNode, SelectorNode, SequenceNode, ActionNode, ConditionNode, BehaviourTreeRunner
  MellowAbelson.AI.State            → MonsterController, MonsterStateMachine
  MellowAbelson.AI.Sensors          → SoundDetector, SightSensor
  MellowAbelson.AI.Utilities        → CheckFieldOfView, SetTimeScaleCurve

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
[CreateAssetMenu(menuName = "MellowAbelson/Game Event")]
public class GameEventSO : ScriptableObject
{
    public UnityEvent OnRaised { get; private set; }
    public void Raise() => OnRaised?.Invoke();
}

public class GameEventListener : MonoBehaviour
{
    public GameEventSO Event;
    public UnityEvent Response;
}
```

**Các event chính:** `OnPlayerDamaged`, `OnPlayerDied`, `OnItemPickedUp`,
`OnRoundStateChanged`, `OnQuotaReached`, `OnMonsterDetectedPlayer`, `OnTerminalCommand`

### 4c. Interaction System (IInteractable)

```csharp
public interface IInteractable
{
    string InteractionPrompt { get; }
    float InteractionRange { get; }
    bool CanInteract(GameObject interactor);
    void OnInteract(GameObject interactor);
    void OnInteractHold(GameObject interactor, float holdProgress);
    void OnInteractCancel(GameObject interactor);
}

public abstract class InteractableBase : NetworkBehaviour, IInteractable { }

// Implement: PickupItem, DoorController, TerminalController, HazardBase
```

**Luồng:**
```
CLIENT: Update() → Raycast → IInteractable → Show prompt
        Player nhấn E → [ServerRpc] ServerInteract(netId)
SERVER: Validate → OnInteract() → [ObserversRpc] sync
```

### 4d. Inventory, Equipment, Stats

```csharp
public class PlayerInventory : NetworkBehaviour, ISaveable
{
    public SyncList<InventorySlotData> Slots;
    [ServerRpc] ServerPickupItem(int itemId, int count);
    [ServerRpc] ServerDropItem(int slotIndex);
    [ServerRpc] ServerMoveItem(int fromSlot, int toSlot);
}
```

### 4e. Health & Damage

```csharp
public class PlayerHealth : NetworkBehaviour, ISaveable
{
    [SyncVar] float _currentHealth;
    [SyncVar] bool _isDead;
    [ServerRpc] void ServerTakeDamage(DamageInfo damage);
    // Server validate → [ObserversRpc] OnDamageTaken / OnDeath
}
```

### 4f. AI - Pure C# Behaviour Tree

**Behaviour Tree Engine (tự viết, 6 files):**

```
BtNode (abstract)
  ├── SelectorNode    (OR: chạy các con, success khi 1 con success)
  ├── SequenceNode    (AND: chạy các con, failure khi 1 con failure)
  ├── ActionNode      (thực thi hành động, trả về Success/Failure/Running)
  └── ConditionNode   (kiểm tra điều kiện, trả về Success/Failure)
```

```csharp
// Ví dụ: tạo behaviour tree cho monster
var tree = new SelectorNode("Root",
    new SequenceNode("Attack",
        new ConditionNode("CanSeeTarget?", () => CanSeeTarget()),
        new ActionNode("Chase", ChaseTarget),
        new ActionNode("Attack", AttackTarget)
    ),
    new SequenceNode("Investigate",
        new ConditionNode("HeardSound?", () => state == Investigating),
        new ActionNode("MoveToSound", MoveToLastSound)
    ),
    new ActionNode("Patrol", Patrol)
);

runner.BuildTree(tree);
// runner.SetTickRate(0.2f); // tick mỗi 200ms
```

**MonsterController (NetworkBehaviour):**

```csharp
public class MonsterController : NetworkBehaviour
{
    [SyncVar] MonsterState _currentState;
    BehaviourTreeRunner _btRunner;  // Pure C#, không NodeCanvas

    protected virtual void BuildDefaultBehaviourTree()
    {
        // Xây dựng BT khác nhau cho từng loại monster
        // Aggressive → ưu tiên tấn công
        // Intimidated → ưu tiên bỏ chạy
    }
}
```

**MonsterStateMachine (FSM helper):**

```csharp
public class MonsterStateMachine
{
    Dictionary<MonsterState, Action> _stateActions;
    MonsterState _currentState;
    event Action<MonsterState, MonsterState> OnStateChanged;

    void SetState(MonsterState newState);
    void Update(); // gọi action tương ứng với state hiện tại
}
```

**Utilities (pure C#, không NodeCanvas dependency):**

```csharp
// Static utility, không cần MonoBehaviour
public static class CheckFieldOfView
{
    public static bool IsTargetInSight(Transform agent, GameObject target,
        float sightDistance, float viewAngle, LayerMask obstacleMask);

    // Dùng từ ConditionNode:
    new ConditionNode("SeePlayer?", () =>
        CheckFieldOfView.IsTargetInSight(transform, player, 15f, 90f, walls));
}

public class SetTimeScaleCurve : MonoBehaviour
{
    // Smooth slow-motion, gọi từ Update
    // Không cần NodeCanvas ActionTask
}
```

**Mối quan hệ giữa các thành phần AI:**

```
MonsterController (NetworkBehaviour)
  ├── BehaviourTreeRunner (MonoBehaviour)
  │     └── BehaviourTree (BtNode tree)
  │           ├── ConditionNode → CheckFieldOfView (static utility)
  │           └── ActionNode → navigation, animation calls
  ├── MonsterStateMachine (pure C# FSM)
  ├── SoundDetector → [ServerRpc] ReportSoundHeard()
  └── SyncVar<MonsterState> → [ObserversRpc] sync to clients
```

### 4g. Round Manager (State Machine)

```csharp
public class RoundManager : NetworkBehaviour, ISaveable
{
    [SyncVar] RoundState _currentState;
    [SyncVar] float _roundTimeRemaining;
    [SyncVar] int _currentDay;
    [SyncVar] float _currentQuota;
    [SyncVar] float _totalScrapCollected;
}

public enum RoundState { Lobby, Landing, Exploration, ExtractionPhase, BetweenRounds, GameOver }
```

**State Flow:**
```
Lobby → Landing → Exploration → ExtractionPhase → BetweenRounds → Landing (hoặc GameOver)
```

---

## 5. Data Flow

### 5a. Network RPC Flow

```
CLIENT                          SERVER                         OTHER CLIENTS
  |-- [ServerRpc] Request  ---->  |                               |
  |                               |-- Validate + Process          |
  |                               |-- Update SyncVars             |
  |   (SyncVar auto-sync)  <---- |                               |
  |                               |-- [ObserversRpc] Broadcast -->|-- Update visuals
```

### 5b. Easy Save 3 (Server-Authoritative)

```
SERVER ONLY:
  OnRoundEnd():
    SaveManager.Save("WorldState_Day_{day}", worldData)
    SaveManager.Save("PlayerData_{clientId}", playerData)
    SaveManager.Save("RoundState", roundData)

  OnServerStart():
    Load("RoundState") → resume
    Load("WorldState_Day_{day}") → restore world

CLIENT: Chỉ save local settings (volume, graphics, keybindings)
```

### 5c. UI Update Flow

```
PlayerInventory.Slots (SyncList)
  |-- OnChange → InventoryUI.RebuildVisuals()
  |-- DragDropHandler → ServerRpc MoveItem()
```

---

## 6. Design Patterns

| Pattern | Vị trí | Mục đích |
|---------|--------|----------|
| **Service Locator** | `Core.Services` | Đăng ký/truy xuất services |
| **Event-Driven** | `Core.Events` | Decouple cross-system |
| **State Machine** | `RoundManager`, `MonsterStateMachine` | Quản lý state rõ ràng |
| **Composite (Behaviour Tree)** | `AI.BehaviourTree` | Selector/Sequence/Action/Condition |
| **Interface** | `IInteractable` | Tất cả object tương tác đều implement |
| **Observer** | `SyncList.OnChange` | UI tự động cập nhật |
| **Factory** | `ItemFactory` / `ScrapSpawner` | Tạo object từ ID |

### Event-Driven Example:

```
[PlayerDied] → RoundManager (score), SpectatorController (activate), UIManager (death screen)
[RoundStateChanged] → AIDirector (spawn rates), StoreManager, HUDController, WorldPersistenceManager
```

---

## 7. Architectural Rules

### Quy tắc bất di bất dịch

1. **Server Authority** - Client gửi `[ServerRpc]`, server validate + broadcast
2. **SyncVar/SyncList** cho persistent state; `[ObserversRpc]` cho transient events
3. **ES3 saves chỉ trên server** - Client chỉ save local settings
4. **ScriptableObject là read-only data** - Không chứa runtime state
5. **IInteractable cho MỌI object tương tác**
6. **GameEventSO cho cross-system** - Tránh reference trực tiếp
7. **Không dùng FindObjectOfType** - Dùng ServiceLocator hoặc inspector
8. **AI thuần C#** - Không visual scripting (NodeCanvas), toàn bộ AI trong code

### Coding Conventions

```csharp
namespace MellowAbelson.Player.Health { ... }

// ServerRpc → "Server" prefix, ObserversRpc → "Observers" prefix
[ServerRpc] void ServerTakeDamage(DamageInfo damage);
[ObserversRpc] void ObserversOnDamageTaken(DamageInfo damage);

// SyncVar → _ prefix
[SyncVar] private float _currentHealth;
public float CurrentHealth => _currentHealth;
```

---

## 8. Migration Plan

### Phase 1-2: Cấu trúc mới + Core (✅ Hoàn thành)
- `_Scripts/` với đầy đủ sub-folders
- Core: ServiceLocator, GameEventSO, SaveManager, InputManager
- AI: Behaviour Tree engine + Utilities (thuần C#, thay thế NodeCanvas)

### Phase 3: Player + Gameplay + AI (✅ Hoàn thành)
| File | Vị trí | Trạng thái |
|------|--------|------------|
| Player systems | `_Scripts/Player/` | ✅ Created |
| AI Behaviour Tree | `_Scripts/AI/BehaviourTree/` | ✅ 6 files |
| AI Utilities | `_Scripts/AI/Utilities/` | ✅ CheckFieldOfView, SetTimeScaleCurve |
| Gameplay systems | `_Scripts/Gameplay/` | ✅ Created |
| Networking | `_Scripts/Networking/` | ✅ Created |

### Phase 4: Migrate từ thư mục cũ (⏳)
| File cũ | File mới | Ghi chú |
|---------|----------|---------|
| `1-Scripts/StandaloneLookController.cs` | `_Scripts/Player/Look/` | ⏳ |
| `Basic/Scripts/PlayerMovement.cs` | `_Scripts/Player/Movement/` | ⏳ |
| `Basic/Scripts/PlayerCamera.cs` | `_Scripts/Player/Look/` | ⏳ |
| `Basic/Scripts/PlayerCubeCreator.cs` | XÓA | ⏳ |
| `Basic/Scripts/FishNetTestCode.cs` | XÓA | ⏳ |
| `1-Scripts/CustomNodes/*` | Đã thay bằng AI/Utilities/ | ✅ Converted |
| `ParadoxNotion/` | XÓA toàn bộ | ⏳ Chờ verify |

### Phase 5: Deprecate (⏳)
- Update scene/prefab references → `_Scripts/`
- **Xóa `ParadoxNotion/`** (tiết kiệm ~50MB)
- Đổi tên `1-Scripts/` → `1-Scripts_DEPRECATED/`
- Đổi tên `Basic/` → `Basic_DEPRECATED/`

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
16. AI.BehaviourTree     (zero dependencies - thuần C#)
17. AI.Sensors           (FishNet)
18. AI.State             (AI.BehaviourTree + Gameplay.Round)
19. UI (tất cả)          (Player + Gameplay)
20. Networking.Lobby     (FishNet)
21. Networking.Spawner   (FishNet + Data)
22. Networking.AntiCheat (tất cả gameplay)
```

---

## 10. File hiện tại trong `Assets/_Scripts/`

```
_Scripts/  (44 files)
├── Core/                          (10 files)
│   ├── Boot/GameBootstrap.cs
│   ├── Services/IService.cs, ServiceLocator.cs
│   ├── Network/ConnectionManager.cs
│   ├── Input/InputManager.cs, InputSystem_Actions.inputactions
│   ├── Events/GameEventSO.cs, GameEventWithPayloadSO.cs, GameEventListener.cs
│   ├── Save/SaveManager.cs, ISaveable.cs, SaveKeys.cs
│   └── Console/ConsoleCommands.cs
│
├── Player/                        (9 files)
│   ├── Movement/StaminaSystem.cs
│   ├── Interaction/IInteractable.cs, PlayerInteractor.cs
│   ├── Inventory/PlayerInventory.cs, InventorySlotData.cs, HotbarController.cs
│   ├── Health/PlayerHealth.cs, DamageInfo.cs
│   └── Stats/PlayerStats.cs, StatType.cs, StatModifier.cs
│
├── AI/                            (10 files - thuần C#)
│   ├── BehaviourTree/BtNode.cs, SelectorNode.cs, SequenceNode.cs,
│   │                ActionNode.cs, ConditionNode.cs, BehaviourTreeRunner.cs
│   ├── Sensors/SoundDetector.cs
│   ├── State/MonsterController.cs, MonsterStateMachine.cs
│   └── Utilities/CheckFieldOfView.cs, SetTimeScaleCurve.cs
│
├── Gameplay/                      (5 files)
│   ├── Interaction/InteractableBase.cs, PickupItem.cs
│   ├── Items/ItemDataSO.cs, ItemDatabase.cs
│   ├── Round/RoundManager.cs
│   └── Persistence/WorldPersistenceManager.cs
│
└── Networking/                    (3 files)
    ├── Scene/SceneLoadManager.cs, LoadingScreenController.cs
    └── Spawner/PlayerSpawnManager.cs
```

### So sánh trước/sau khi bỏ NodeCanvas

| Khía cạnh | NodeCanvas | Pure C# (hiện tại) |
|-----------|-----------|-------------------|
| AI logic | Visual tree + C# tasks | 100% C# code |
| Diff/version control | ❌ Binary .asset files | ✅ Text .cs files |
| Vibe code friendly | ❌ Không thể AI edit | ✅ AI có thể edit thoải mái |
| Performance | Runtime overhead | Minimum overhead |
| Dependency | +ParadoxNotion (~50MB) | Zero |
| Flexibility | Giới hạn bởi NodeCanvas API | Tự do implement |
| Time to implement | Nhanh (kéo thả) | Chậm hơn (viết code) |
| Debug | Visual debugger | Breakpoints + logs |

---

*Document generated: 2026-04-29*
*Architecture design for Mellow Abelson — Co-op Horror Survival Multiplayer*
