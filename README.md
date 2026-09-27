# Traditional Chinese Medicine Simulator（中医模拟器）

> Unity VR 工程 · 目前处于 **VR 模板起点**，尚无自研代码
> Unity VR project · currently a **template starting point** with no self-authored code yet

---

## 目录 / Table of Contents

1. [项目简介 / Overview](#1-项目简介--overview)
2. [环境与版本 / Environment & Versions](#2-环境与版本--environment--versions)
3. [快速开始 / Getting Started](#3-快速开始--getting-started)
4. [目录地图 / Directory Map](#4-目录地图--directory-map)
5. [场景清单 / Scene Inventory](#5-场景清单--scene-inventory)
6. [XR 配置 / XR Configuration](#6-xr-配置--xr-configuration)
7. [内容资产 / Content Assets](#7-内容资产--content-assets)
8. [第三方资产与授权 / Third-Party Assets & Licensing](#8-第三方资产与授权--third-party-assets--licensing)
9. [代码现状与约定 / Code Status & Conventions](#9-代码现状与约定--code-status--conventions)
10. [现状注意事项 / Current Notes & Gotchas](#10-现状注意事项--current-notes--gotchas)

> 本文档面向**开发者**，描述工程**现状**（有什么、在哪、怎么跑、哪些是模板哪些是自己的）。
> 不含玩法规划。
> 另有一份 [`CLAUDE.md`](CLAUDE.md)，是写给 AI 助手的指令文件。**两者分工不同**：README 面向人，CLAUDE.md 面向工具。
> This document is for **humans onboarding to the project**. It describes the *current state* only — no roadmap. The separate `CLAUDE.md` is an instruction file for AI assistants.

---

## 1. 项目简介 / Overview

**中文**

本工程是一个中医主题的 Unity **VR** 项目，路径为 `E:\Gaming_Project\Traditional Chinese Medicine Simulator\`，**非 git 仓库**。

需要明确的是当前的开发阶段：**技术底座与素材库已经就位，但实际玩法内容尚未开始**。`Assets/` 下没有一个属于本项目自己编写的脚本——全部是 VR 模板自带内容、XRI 官方样例，以及第三方素材包。因此工程现在**跑起来是一个 VR 模板演示，而不是一个中医游戏**。初次接手时请以此为前提阅读后续章节。

**English**

A Traditional Chinese Medicine themed Unity **VR** project at `E:\Gaming_Project\Traditional Chinese Medicine Simulator\`. **Not a git repository.**

The current stage matters: the tech stack and the asset library are in place, but **actual gameplay content has not been started**. There is not a single script in `Assets/` authored by this project — everything is VR template content, official XRI samples, or third-party asset packs. Running the project today gives you **a VR template demo, not a TCM game**. Read the following sections with that in mind.

---

## 2. 环境与版本 / Environment & Versions

**中文**

| 项目 / Item | 版本 / Value |
|---|---|
| Unity | `2022.3.62f3c1` |
| 渲染管线 / Render Pipeline | URP `14.0.12` |
| 色彩空间 / Color Space | **Linear**（VR 必需） |
| XR Interaction Toolkit | `3.1.2` |
| XR Plug-in Management | `4.5.1` |
| OpenXR | `1.14.3` |
| Input System | `1.14.0` —— **仅启用新输入系统** |
| TextMesh Pro | `3.0.9` |
| Timeline | `1.7.7` |
| 包源 / Registry | `https://packages.unity.cn`（国内镜像） |

**仅新输入系统**这一点会直接影响写代码的方式：`activeInputHandler` 为 `1`，因此 `Input.GetKey` / `Input.GetAxis` 等**旧输入 API 全部不可用**，必须使用 `InputAction` / `InputActionReference`。详见第 9 节。

**English**

Unity `2022.3.62f3c1` with URP `14.0.12` in **Linear** color space (required for VR). XR stack is XRI `3.1.2` + XR Plug-in Management `4.5.1` + OpenXR `1.14.3`, plus Input System `1.14.0`.

**Only the new Input System is enabled** (`activeInputHandler: 1`). The legacy `Input.GetKey` / `Input.GetAxis` APIs are therefore **unavailable** — use `InputAction` / `InputActionReference` instead. Package registry is the China mirror `packages.unity.cn`.

---

## 3. 快速开始 / Getting Started

**中文**

### 打开工程

本工程**没有命令行构建脚本，也没有 CI**。一切通过 Unity Editor：用 **Unity Hub** 打开 `E:\Gaming_Project\Traditional Chinese Medicine Simulator\` 目录即可。

Build Settings 中**唯一启用**的场景是 `Assets/Scenes/SampleScene.unity`。

### 两个项目场景用途不同（最容易踩的坑）

| 场景 | 行数 | 用途 |
|---|---|---|
| `Assets/Scenes/BasicScene.unity` | 624 | **功能验证用这个。** 最小 VR 场景，干净、加载快。含 `Directional Light`、`XR Interaction Manager`、`Plane`（带传送区）、`EventSystem`（XR UI Input Module）、以及 `XR Origin (XR Rig)` 预制体实例。无烘焙光照。 |
| `Assets/Scenes/SampleScene.unity` | 17,762 | VR 模板自带的**完整教程演示**场景，含 `StepManager` 分步教学流程、Coaching UI 层级与烘焙光照。**适合当参考实现读，但别在上面做实验。** |

### ⚠️ 两个场景用的是**不同的** XR rig

这一点很容易踩坑：切换场景后手部/交互行为会不一样，因为两处引用的不是同一个预制体。

| 场景 | 使用的 rig 预制体 | 路径 |
|---|---|---|
| `BasicScene.unity` | `XR Origin (XR Rig).prefab` | `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/Prefabs/` |
| `SampleScene.unity` | `Complete XR Origin Set Up Variant.prefab` | `Assets/VRTemplateAssets/Prefabs/Setup/` |

**要改手部/交互器配置，请改对应场景所用的那个预制体本身**，而不是在每个场景里各改一遍（否则两边会逐渐跑偏）。

### 无头显开发：VR 模拟器

工程**已启用 XR Device Simulator**。它是**全局的**——Editor 进 Play 模式时自动生成，**不需要往任何场景里放东西**。

- 开关：`Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset`
- 预制体与样例脚本：`Assets/Samples/XR Interaction Toolkit/3.1.2/XR Device Simulator/`

| 操作 | 按键 |
|---|---|
| 平移 HMD | `W` `A` `S` `D` |
| 升降 HMD | `Q` / `E` |
| 转视角 | **按住鼠标右键**拖动 |
| 右手柄 Trigger | 鼠标左键 |
| 左右手柄 | 映射到键盘 |

屏幕左下角有官方操作提示图。

> ⚠️ **测试真机头显前，必须先把 `m_AutomaticallyInstantiateSimulatorPrefab` 改回 `0`**（该文件当前实测值为 `1`）。否则模拟器会抢占 HMD 输入，你会以为设备坏了。

**English**

### Opening the project

There is **no CLI build script and no CI**. Everything goes through the Unity Editor — open the folder with **Unity Hub**.

The only scene enabled in Build Settings is `Assets/Scenes/SampleScene.unity`.

### The two project scenes serve different purposes

- **`Assets/Scenes/BasicScene.unity`** (624 lines) — **use this for feature verification.** A minimal VR scene: `Directional Light`, `XR Interaction Manager`, a `Plane` with a teleport area, an `EventSystem` with XR UI Input Module, and an `XR Origin (XR Rig)` prefab instance. No baked lighting.
- **`Assets/Scenes/SampleScene.unity`** (17,762 lines) — the VR template's **full tutorial demo**, with a `StepManager` step-by-step flow, a large coaching UI hierarchy, and baked lightmaps. **Read it as a reference implementation; don't experiment on it.**

### ⚠️ The two scenes use **different** XR rigs

This is easy to trip over: hand and interaction behaviour differ between the two scenes because they do not reference the same prefab. `BasicScene.unity` instances `XR Origin (XR Rig).prefab` from `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/Prefabs/`, while `SampleScene.unity` instances `Complete XR Origin Set Up Variant.prefab` from `Assets/VRTemplateAssets/Prefabs/Setup/`.

**Change hand/interactor configuration on the prefab the relevant scene actually uses** — not once per scene, which lets the two drift apart.

### Developing without a headset: the VR simulator

XR Device Simulator is **enabled and global** — it spawns automatically on Play, and you do **not** need to place anything in any scene. Toggle it in `Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset`.

Controls: `WASD` moves the HMD, `Q`/`E` raises/lowers it, **hold right mouse button** to look around, left mouse button is the right controller trigger. Hand controllers map to the keyboard. An on-screen legend appears bottom-left.

> ⚠️ **Before testing on a real headset, set `m_AutomaticallyInstantiateSimulatorPrefab` back to `0`** (its current measured value is `1`). Otherwise the simulator captures HMD input and you'll think your device is broken.

---

## 4. 目录地图 / Directory Map

**中文**

`Assets/` 总计约 **1.4 GB**。体量本身就是有用信息——它直接反映了当前工程的构成（几乎全是素材）。

| 目录 / Path | 体量 / Size | 性质 / What it is |
|---|---|---|
| `Assets/Artassert/` | **1.3 GB** | 第三方素材：室内包两份副本 + VR 手部 + 一个孤立模型 |
| `Assets/VRTemplateAssets/` | 74 MB | VR Core 模板自带资源（`Audio` `Fonts` `Graphics` `Materials` `Models` `Prefabs` `Scripts` `Shaders` `Sprites` `Themes` `Tutorial` `Videos`） |
| `Assets/Samples/` | 12 MB | XRI 包样例：Starter Assets + XR Device Simulator |
| `Assets/TextMesh Pro/` | 3.6 MB | TMP Essentials |
| `Assets/Scenes/` | 2.6 MB | 两个项目场景 + 烘焙光照数据 |
| `Assets/Settings/` | ~1 MB | URP 配置、Preset、场景模板 |
| `Assets/XR/` | 109 KB | XR Loader 与 OpenXR/Oculus 设置 |
| `Assets/XRI/` | 12 KB | 交互层、编辑器设置、设备模拟器设置 |

工程内**无自定义 asmdef、无自研 Editor 脚本、无测试程序集、无 CI 配置**。`Assets/` 下仅有 **3 个 asmdef**，全部位于 `Assets/Samples/` 之内。

**English**

`Assets/` totals roughly **1.4 GB** — the size distribution is itself informative, since it's almost entirely assets.

| Path | Size | What it is |
|---|---|---|
| `Assets/Artassert/` | **1.3 GB** | Third-party assets: two copies of the interior pack, VR hands, one loose model |
| `Assets/VRTemplateAssets/` | 74 MB | VR Core template content |
| `Assets/Samples/` | 12 MB | XRI package samples (Starter Assets + XR Device Simulator) |
| `Assets/TextMesh Pro/` | 3.6 MB | TMP Essentials |
| `Assets/Scenes/` | 2.6 MB | The two project scenes plus baked lighting data |
| `Assets/Settings/` | ~1 MB | URP configs, presets, scene templates |
| `Assets/XR/` | 109 KB | XR loaders and OpenXR/Oculus settings |
| `Assets/XRI/` | 12 KB | Interaction layers, editor settings, simulator settings |

There are **no custom asmdefs, no project-authored Editor scripts, no test assemblies and no CI config**. Only **3 asmdefs** exist under `Assets/`, all inside `Assets/Samples/`.

---

## 5. 场景清单 / Scene Inventory

**中文**

工程共 **11 个** `.unity` 场景。行数可作复杂度参考。

### 项目场景（本工程）

| 场景 | 行数 | 说明 |
|---|---|---|
| `Assets/Scenes/BasicScene.unity` | 624 | 最小 VR 验证场景 |
| `Assets/Scenes/SampleScene.unity` | 17,762 | 模板教程演示场景（当前唯一启用构建项） |

### 室内包自带演示场景（搭景参考）

| 场景 | 行数 |
|---|---|
| `Assets/Artassert/Fantastic Interior Pack/scenes/demoscene_interior_assets_modular.unity` | 7,954 |
| `.../demoscene_interior_assets_props.unity` | 22,983 |
| `.../demoscene_interior_level_1_studyroom.unity` | 29,006 |
| `.../demoscene_interior_level_2_bedroom.unity` | 15,474 |
| `.../demoscene_interior_level_3_tavern.unity` | 56,378 |
| `.../demoscene_interior_level_4_basement.unity` | 54,924 |
| `.../demoscene_interior_level_5_bathroom.unity` | 20,714 |

这 7 个场景是该素材包的展示与搭景范例，**不含任何 VR 交互**，纯粹是布景参考。

### 样例场景

| 场景 | 行数 | 说明 |
|---|---|---|
| `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/DemoScene.unity` | 4,782 | XRI 抓取/poke/攀爬/注视等交互示例 |
| `Assets/Artassert/PolyOne/Free VR Hands/Scene/Demo_Free VR Hands.unity` | 573 | VR 手部模型展示 |

**English**

There are **11** `.unity` scenes. **Two** belong to this project: `BasicScene.unity` (624 lines, minimal VR verification scene) and `SampleScene.unity` (17,762 lines, the template tutorial scene, and the only one enabled in Build Settings — currently the active build scene).

Seven are demo scenes shipped with the interior pack (`demoscene_interior_assets_modular`, `_assets_props`, and `_level_1_studyroom` through `_level_5_bathroom`, ranging from 7,954 to 56,378 lines). They showcase the pack and contain **no VR interaction** — useful purely as set-dressing references.

The remaining two are samples: XRI's `DemoScene.unity` (4,782 lines) and PolyOne's `Demo_Free VR Hands.unity` (573 lines).

---

## 6. XR 配置 / XR Configuration

**中文**

### Loader

**激活的 loader**：`Assets/XR/Loaders/Open XR Loader.asset`（guid `28fe04729daeb2345bebc951fad25769`）。`XRGeneralSettingsPerBuildTarget.asset` 中 **Standalone / iPhone / Android 三个目标平台全部指向它**，均设 `m_InitManagerOnStart: 1`（`m_AutomaticLoading` 与 `m_AutomaticRunning` 均为 `0`）。

> 小提示：iPhone 那一项在文件里被序列化命名为 "Android Settings"，属模板遗留的命名瑕疵，不影响功能。

> ⚠️ `Oculus Loader.asset` 与 `Open XR Loader No Pre Init.asset` **存在但未启用**——别被文件名误导。

### OpenXR 已启用的 feature（跨两个平台共 5 个）

解析 `Open XR Package Settings.asset` 中**被 `Values` 实际引用的** 4 个 settings 容器后，逐项核对 `m_enabled` 的结果：

| 平台 | 已启用 |
|---|---|
| **Standalone**（4 个） | `OculusTouchControllerProfile`、`MicrosoftMotionControllerProfile`、`MetaQuestTouchPlusControllerProfile`、`KHRSimpleControllerProfile` |
| **Android**（3 个） | `OculusTouchControllerProfile`、`MetaQuestTouchPlusControllerProfile`、`MetaQuestFeature` |

以下均**未启用**：`ValveIndexControllerProfile`、`HTCViveControllerProfile`、`HPReverbG2ControllerProfile`、`MockRuntime`、`MetaQuestTouchProControllerProfile`、`HandInteractionProfile`、`PalmPoseInteraction`、`EyeGazeInteraction` 等。

渲染模式：每个容器均为 `m_renderMode: 1` = **Single Pass Instanced**（非 Multiview）。

> ⚠️ **读这个文件时别用朴素的全局 grep。** 该 `.asset` 内含**多个未被引用的重复 "Standalone" 块**（如 Valve Index / HTC Vive 在这些孤立块里显示为启用），它们是废弃的序列化残留。**只有 `Values:` 列出的 4 个容器才是生效配置**——按行号 `grep m_enabled: 1` 会把孤立块一并算进来，得出错误结论。

### 设备模拟器

`XRDeviceSimulatorSettings.asset` 实测值：

```
m_AutomaticallyInstantiateSimulatorPrefab: 1
m_AutomaticallyInstantiateInEditorOnly:    1
m_UseClassic:                              1
m_SimulatorPrefab: guid 18ddb545287c546e19cc77dc9fbb2189
```

该 guid 解析为 **`Samples/.../XR Device Simulator/XRDeviceSimulator/XR Device Simulator.prefab`（classic 版）**。样例包内另有较新的 `XRInteractionSimulator`。

> ⚠️ **`m_UseClassic` 不影响运行时。** 加载器 `XRInteractionSimulatorLoader` 只读 `simulatorPrefab` 字段；`m_UseClassic` 仅决定设置面板里自动选哪个预制体。**想换模拟器必须显式改 `simulatorPrefab`**，否则只会得到一句 `prefab was missing` 警告。

### XR Origin rig 与输入资产

`BasicScene` 所用的 `XR Origin (XR Rig).prefab` 是 XRI 3.1.2 Starter Assets 的完整 rig，**已内置整套移动与交互能力，不需要自己实现**：

| 能力 | 组件 |
|---|---|
| 移动 | `DynamicMoveProvider`（样例脚本）+ `GravityProvider` |
| 转向 | `SnapTurnProvider` + `ContinuousTurnProvider` |
| 传送 | `TeleportationProvider` |
| 攀爬 | `ClimbProvider` + `ClimbTeleportInteractor` |
| 双手抓取移动 | `GrabMoveProvider` + `TwoHandedGrabMoveProvider` |
| 跳跃 | `JumpProvider` |
| 交互器 | `XRRayInteractor`、`XRPokeInteractor`、`NearFarInteractor`、`XRGazeInteractor` |

输入动作资产为 `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/XRI Default Input Actions.inputactions`，由 rig 上的 `InputActionManager` 引用。动作集包含 `XRI Head`、`XRI Left/Right`、`XRI Left/Right Interaction`、`XRI Left/Right Locomotion`、`XRI UI`。**要新增输入，优先在这个资产里加动作，而不是另建一个 InputAction 资产。**

### URP 配置

`Assets/Settings/Project Configuration/` 下有 **两套** URP 配置。质量等级共 6 级（`Very Low` `Low` `Medium` `High` `Very High` `Ultra`），实际生效哪一套由等级决定：

- `Performance URP Config.asset` —— 被 `Very Low`(0) 与 `Low`(1) **显式引用**；`Medium`/`High`/`Very High` 未指定，回落到 `GraphicsSettings` 默认管线（同样是 Performance）。
- `Quality URP Config.asset` —— **仅**被 `Ultra`(5) 引用。

> ⚠️ **平台默认等级与 Editor 当前等级并不一致**——这是调优时容易误判的地方：

| 范围 | 质量等级 | 实际使用 |
|---|---|---|
| Editor 当前 | `Low` (1) | Performance |
| **Standalone 平台默认** | **`Ultra` (5)** | **Quality** |
| Android 平台默认 | `Low` (1) | Performance |
| iPhone 平台默认 | `Medium` (2) | Performance（回落） |

也就是说：**你在 Editor 里看到的是 Performance 画质，但打出的 Standalone 包默认跑 Quality。**

两套的关键差异（均为实测值）：

| 参数 | Performance | Quality |
|---|---|---|
| HDR | `0` 关 | `1` 开 |
| 主光阴影距离 | `2.5` | `10` |
| 附加光渲染 | `0` 关 | 逐像素 |
| 软阴影质量 | — | `2` |

两套共同值：Forward 渲染器、MSAA `4`、RenderScale `1`、主光阴影贴图 4096、级联数 1、`RequireDepthTexture` 关。Performance 额外开启 SRP Batcher 与 Adaptive Performance、关闭 Render Graph；其 renderer data 为 `Android Preset.asset`。

### 附注

样例 asmdef 原引用 `Unity.XR.Hands`，但本工程**未安装该包**，已改为名称引用。日后若安装 XR Hands，可把引用加回以启用 `XR_HANDS_1_1_OR_NEWER` 分支的手部手势支持。

**English**

**Active loader:** `Assets/XR/Loaders/Open XR Loader.asset` (guid `28fe04729daeb2345bebc951fad25769`). In `XRGeneralSettingsPerBuildTarget.asset`, **all three targets — Standalone, iPhone and Android — point at it**, each with `m_InitManagerOnStart: 1` (`m_AutomaticLoading` and `m_AutomaticRunning` are both `0`).

> Minor note: the iPhone entry is serialized under the name "Android Settings" — a cosmetic leftover from the template, with no functional impact.

**Five OpenXR features are enabled** across the two platforms, resolved by parsing only the 4 settings containers actually listed in `Values:` and checking each one's `m_enabled`:

| Platform | Enabled |
|---|---|
| **Standalone** (4) | `OculusTouchControllerProfile`, `MicrosoftMotionControllerProfile`, `MetaQuestTouchPlusControllerProfile`, `KHRSimpleControllerProfile` |
| **Android** (3) | `OculusTouchControllerProfile`, `MetaQuestTouchPlusControllerProfile`, `MetaQuestFeature` |

**Disabled:** `ValveIndexControllerProfile`, `HTCViveControllerProfile`, `HPReverbG2ControllerProfile`, `MockRuntime`, `MetaQuestTouchProControllerProfile`, `HandInteractionProfile`, `PalmPoseInteraction`, `EyeGazeInteraction` and others.

Render mode is `m_renderMode: 1` = **Single Pass Instanced** for every container (not Multiview).

> ⚠️ **Don't read this file with a naive global grep.** It contains several **unreferenced duplicate "Standalone" blocks** (Valve Index and HTC Vive appear enabled inside those orphans) — dead serialization leftovers. **Only the 4 containers listed under `Values:` are live**; grepping `m_enabled: 1` by line number sweeps the orphans in and yields the wrong answer.

**Device simulator** (`XRDeviceSimulatorSettings.asset`, measured): auto-instantiate is `1`, editor-only is `1`, `m_UseClassic` is `1`, and `m_SimulatorPrefab` (guid `18ddb545287c546e19cc77dc9fbb2189`) resolves to the **classic** `XR Device Simulator.prefab` under `Samples/.../XRDeviceSimulator/`. A newer `XRInteractionSimulator` also ships in the samples.

> ⚠️ **`m_UseClassic` has no effect at runtime.** The loader `XRInteractionSimulatorLoader` only reads the `simulatorPrefab` field; `m_UseClassic` merely decides which prefab the settings panel auto-selects. **To switch simulators you must change `simulatorPrefab` explicitly**, or you'll just get a `prefab was missing` warning.

**XR Origin rig and input assets:** the `XR Origin (XR Rig).prefab` used by `BasicScene` is the full XRI 3.1.2 Starter Assets rig, and **it already ships an entire locomotion and interaction suite** — you don't need to implement any of it: `DynamicMoveProvider` + `GravityProvider` (movement), `SnapTurnProvider` + `ContinuousTurnProvider` (turning), `TeleportationProvider` (teleport), `ClimbProvider` + `ClimbTeleportInteractor` (climbing), `GrabMoveProvider` + `TwoHandedGrabMoveProvider` (two-handed grab-move), `JumpProvider` (jumping), and `XRRayInteractor` / `XRPokeInteractor` / `NearFarInteractor` / `XRGazeInteractor` as interactors.

Input actions live in `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/XRI Default Input Actions.inputactions`, referenced by the rig's `InputActionManager`. Action maps include `XRI Head`, `XRI Left/Right`, `XRI Left/Right Interaction`, `XRI Left/Right Locomotion` and `XRI UI`. **Add new actions to this asset rather than creating a second InputAction asset.**

**URP:** two configs live in `Assets/Settings/Project Configuration/`, and which one is live depends on the quality level (six exist: `Very Low` through `Ultra`). `Performance URP Config.asset` is **explicitly referenced by `Very Low` (0) and `Low` (1)**; `Medium`/`High`/`Very High` specify none and fall back to the `GraphicsSettings` default pipeline — also Performance. `Quality URP Config.asset` is referenced **only** by `Ultra` (5).

> ⚠️ **Per-platform defaults do not match the Editor's current level** — an easy way to misjudge tuning work:

| Scope | Quality level | In use |
|---|---|---|
| Editor, current | `Low` (1) | Performance |
| **Standalone default** | **`Ultra` (5)** | **Quality** |
| Android default | `Low` (1) | Performance |
| iPhone default | `Medium` (2) | Performance (fallback) |

In other words: **the Editor shows you the Performance look, but a Standalone build defaults to Quality.**

Key measured differences: HDR `0` → `1`, main-light shadow distance `2.5` → `10`, additional lights off → per-pixel, soft shadow quality `2` in Quality. Both use the Forward renderer, MSAA `4`, render scale `1`, a 4096 main-light shadowmap and 1 cascade; Performance additionally enables SRP Batcher and Adaptive Performance and disables Render Graph, and its renderer data is `Android Preset.asset`.

**Note:** the sample asmdef originally referenced `Unity.XR.Hands`, which **is not installed here**, so it was rewritten to name-based references. If you install XR Hands later, the reference can be restored to enable the `XR_HANDS_1_1_OR_NEWER` hand-gesture path.

---

## 7. 内容资产 / Content Assets

**中文**

> **重要前提**：经 GUID 交叉比对，**室内素材包与 VR 手部模型目前均未被任何项目场景引用**（`BasicScene.unity` / `SampleScene.unity` 对室内包 868 个资产 GUID 的引用数为 **0**）。它们现在是**纯粹待用的素材库**。

### FANTASTIC – Interior Pack（作者 Tidal Flask，URP 版）

已导入版本位于 `Assets/Artassert/Fantastic Interior Pack/`，约 427 MB。

| 子目录 | 内容 |
|---|---|
| `2d/textures` | 401 MB，贴图（BC / N / AO 等，单张 13–21 MB） |
| `3d/ENV/` | 104 个 `MOD_*` 模型：模块化环境构件 |
| `3d/PROPS/` | 244 个 `SM_PROP_*` 模型 |
| `prefabs/` | `ENV/`(96) `PROPS/`(258) `FX/`(8) |
| `materials/` | 36 个材质 |
| `animation/` `settings/` | 动画与包设置 |
| `Documentation - FANTASTIC Interior Pack.pdf` | 3 MB 完整文档（含分步指南与 FAQ） |

**`3d/ENV/` 模块化套件**（搭房间用）：`Base`（踢脚线/天花板）、`Column`（柱）、`Floor`（地板）、`Gateways`（门与窗 `MOD_Door_interior` / `MOD_Window_interior_01,02`）、`Railing`（栏杆）、`Stairs`（楼梯）、`Trim`（墙面装饰线）、`Wall`（墙板，含 `OneSided` 与 `PivotMiddle` 变体）。

**`COMP_*` 预组合件**：`prefabs` 下已拼装好的组合体，可直接拖入场景，例如 `COMP_PROP_bookshelf_interior_01`、`COMP_PROP_fireplace_interior_01`、`COMP_MOD_window_interior_01`。

### 与中医馆搭景直接相关的道具

从 244 个道具中筛出可直接用于中医题材的部分：

| 用途 | 可用模型 |
|---|---|
| **药柜 / 药材收纳** | `cabinet_interior_01..08`、`bookshelf_interior_01,02`、`shelf_interior_01..03`、`wardrobe_interior_01` |
| **药罐 / 药瓶 / 器具** | `jar_interior_01..04`、`pot_interior_01..04`、`bottle_interior_01,02`、`bowl_interior_01,02`、`kettle_interior`、`cookingpot_interior_01,02` |
| **诊室陈设** | `paravan_interior`（**屏风**）、`lantern_interior_01..04`（灯笼）、`painting_interior_01..10`（挂画）、`plant_interior_01..09`（植物）、`rug_interior_01..08`（地毯） |
| **桌椅** | `table_interior_01..08`、`stool_interior`、`chair_interior_01,02`、`bench_interior_01..03`、`desk_interior_01` |
| **氛围特效** | `prefabs/FX/` 下火焰、辉光、蒸汽共 8 个（`P_FX_fire_v1..v3_interior`、`P_FX_steam_interior`、`P_FX_godray_interior` 等） |

### PolyOne – Free VR Hands

`Assets/Artassert/PolyOne/Free VR Hands/`（5.6 MB）：`Free Pack - VR Hands ( Rigged ).prefab`（带骨骼绑定的手部模型），含 `Animation` / `Materials` / `Model` / `Texture`，以及演示场景。**目前仅在该演示场景中被引用。**

### 孤立模型

`Assets/Artassert/01.fbx`（54 MB）：单个模型，被**实例化放置在 `SampleScene.unity` 中**，场景内对象名为 `01`，位置约 `(9.26, 0.15, 19.45)`。它**只被这个模板演示场景引用**，`BasicScene.unity` 未使用。

### 素材规模统计

| 类型 | 数量 |
|---|---|
| 预制体 `.prefab` | 445（室内包 362 / Samples 54 / 模板 27 / PolyOne 2） |
| 模型 `.fbx` | 386 |
| 材质 `.mat` | 102 |
| 贴图 | 209 PNG + 6 TIF + 3 EXR |
| 动画剪辑 `.anim` | 5 |
| Animator Controller | 5 |

**English**

> **Important caveat:** a GUID cross-reference shows that **neither the interior pack nor the VR hands model is currently referenced by any project scene** (zero of the pack's 868 asset GUIDs appear in `BasicScene.unity` or `SampleScene.unity`). They are, today, a **purely unused asset library**.

### FANTASTIC – Interior Pack (by Tidal Flask, URP version)

The imported copy lives at `Assets/Artassert/Fantastic Interior Pack/` (~427 MB): `2d/textures` (401 MB of BC/N/AO maps, 13–21 MB each), `3d/ENV/` (104 `MOD_*` modular environment pieces), `3d/PROPS/` (244 `SM_PROP_*` models), `prefabs/` (`ENV` 96, `PROPS` 258, `FX` 8), `materials/` (36), plus `animation/`, `settings/` and a 3 MB `Documentation - FANTASTIC Interior Pack.pdf`.

The **`3d/ENV/` modular kit** (for building rooms) covers `Base` (skirting/ceiling), `Column`, `Floor`, `Gateways` (doors and windows), `Railing`, `Stairs`, `Trim` and `Wall` (including `OneSided` and `PivotMiddle` variants). Pre-assembled `COMP_*` prefabs (e.g. `COMP_PROP_bookshelf_interior_01`, `COMP_MOD_window_interior_01`) can be dropped straight into a scene.

### Props directly relevant to a TCM clinic

| Purpose | Models |
|---|---|
| **Herb cabinets / storage** | `cabinet_interior_01..08`, `bookshelf_interior_01,02`, `shelf_interior_01..03`, `wardrobe_interior_01` |
| **Herb jars / vessels** | `jar_interior_01..04`, `pot_interior_01..04`, `bottle_interior_01,02`, `bowl_interior_01,02`, `kettle_interior`, `cookingpot_interior_01,02` |
| **Clinic furnishing** | `paravan_interior` (a **folding screen**), `lantern_interior_01..04`, `painting_interior_01..10`, `plant_interior_01..09`, `rug_interior_01..08` |
| **Tables & seating** | `table_interior_01..08`, `stool_interior`, `chair_interior_01,02`, `bench_interior_01..03`, `desk_interior_01` |
| **Atmosphere FX** | 8 prefabs under `prefabs/FX/` — fire, glow and steam |

### PolyOne – Free VR Hands

`Assets/Artassert/PolyOne/Free VR Hands/` (5.6 MB) holds `Free Pack - VR Hands ( Rigged ).prefab` — a rigged hand model — with `Animation`, `Materials`, `Model`, `Texture` folders and a demo scene. **It is currently referenced only by that demo scene.**

### Loose model

`Assets/Artassert/01.fbx` (54 MB) is **instantiated in `SampleScene.unity`** as a GameObject named `01` at roughly `(9.26, 0.15, 19.45)`. It is referenced **only** by that template demo scene; `BasicScene.unity` does not use it.

### Asset counts

445 `.prefab` (interior pack 362 / Samples 54 / template 27 / PolyOne 2), 386 `.fbx`, 102 `.mat`, 209 PNG + 6 TIF + 3 EXR textures, 5 `.anim` clips and 5 Animator Controllers.

---

## 8. 第三方资产与授权 / Third-Party Assets & Licensing

**中文**

工程当前内容**全部为第三方或模板来源**，没有自研资产。发行前请逐项核实授权条款。

| 资产 | 来源 / 作者 | 位置 | 授权状态 |
|---|---|---|---|
| VR Core 模板资源 | Unity 官方 | `Assets/VRTemplateAssets/` | Unity 模板内容 |
| XRI 样例 | Unity 官方（XR Interaction Toolkit 包） | `Assets/Samples/` | 随包授权 |
| **FANTASTIC – Interior Pack** | **Tidal Flask**（info@tidalflask.com, www.tidalflask.com） | `Assets/Artassert/Fantastic Interior Pack/` | ⚠️ **需核对商业授权范围**（原始包内附文档与 FAQ） |
| Free VR Hands | PolyOne | `Assets/Artassert/PolyOne/` | ⚠️ 免费包，需核对再分发条款 |
| `01.fbx` | 来源不明 | `Assets/Artassert/01.fbx` | ⚠️ **来源与授权待确认** |
| TextMesh Pro Essentials | Unity 官方 | `Assets/TextMesh Pro/` | 含 `LiberationSans - OFL.txt`、`EmojiOne Attribution.txt` 等 |

原始素材包归档（含完整文档）位于 `Assets/Artassert/FANTASTIC - Interior Pack/`。

**English**

**All current content is third-party or template-derived — none of it is original to this project.** Verify licensing per item before shipping.

| Asset | Source | Location | Status |
|---|---|---|---|
| VR Core template content | Unity | `Assets/VRTemplateAssets/` | Template content |
| XRI samples | Unity (XR Interaction Toolkit package) | `Assets/Samples/` | Covered by the package |
| **FANTASTIC – Interior Pack** | **Tidal Flask** | `Assets/Artassert/Fantastic Interior Pack/` | ⚠️ **Confirm commercial license scope** (docs/FAQ ship inside the original pack) |
| Free VR Hands | PolyOne | `Assets/Artassert/PolyOne/` | ⚠️ Free pack — check redistribution terms |
| `01.fbx` | Unknown | `Assets/Artassert/01.fbx` | ⚠️ **Origin and license unverified** |
| TextMesh Pro Essentials | Unity | `Assets/TextMesh Pro/` | Includes `LiberationSans - OFL.txt`, `EmojiOne Attribution.txt` |

The original pack archives (with full documentation) are in `Assets/Artassert/FANTASTIC - Interior Pack/`.

---

## 9. 代码现状与约定 / Code Status & Conventions

**中文**

### 工程内零自研脚本

这是本 README 最需要记住的一条：**`Assets/` 下没有任何属于本项目的脚本。**

| 来源 | 数量 | 位置与命名空间 |
|---|---|---|
| VR 模板脚本 | 14 | `Assets/VRTemplateAssets/Scripts/`，命名空间 `Unity.VRTemplate` |
| XRI Starter Assets 样例 | 13 | `Assets/Samples/.../Starter Assets/Scripts/` |
| XRI Device Simulator 样例 | 5 | `Assets/Samples/.../XR Device Simulator/` |
| XRI 样例 Editor 校验 | 1 | `Assets/Samples/.../Starter Assets/Editor/` |

模板脚本包括 `StepManager`、`XRKnob`、`Callout`、`CalloutGazeController`、`XRPokeFollowAffordanceFill`、`LaunchProjectile`、`AnchorVisuals` 等。**它们不是本项目的代码，改动前请先想清楚是否真的该动。**

### 写代码时的约定

- **没有自定义 asmdef**，新脚本默认进 `Assembly-CSharp`。若将来要加 asmdef，**必须显式引用** `Unity.XR.Interaction.Toolkit`、`Unity.XR.CoreUtils`、`Unity.InputSystem`、`UnityEngine.UI`——这些是包程序集，asmdef **不会自动获得**。
- **只有新输入系统**：`Input.GetKey` / `Input.GetAxis` 等旧 API **不可用**，一律用 `InputAction` / `InputActionReference`。判定平面上已有 `EventSystem` + XR UI Input Module 配好。
- **手部交互先用现成组件**：`XRDirectInteractor`、`XRGrabInteractable`、`XRPokeInteractable` 等都在 XRI 包里。做抓取 / 点按 / 旋钮请先找现成的，**不要自己写射线**。

**English**

**There is not a single script in `Assets/` authored by this project.** Fourteen `.cs` files belong to the VR template (`Assets/VRTemplateAssets/Scripts/`, namespace `Unity.VRTemplate`, including `StepManager`, `XRKnob`, `Callout`, `XRPokeFollowAffordanceFill`, `LaunchProjectile`), thirteen to the XRI Starter Assets sample, five to the Device Simulator sample, and one to a sample editor validator. **They are not project code — think twice before modifying them.**

**Conventions when you do write code:**

- **No custom asmdef exists**, so new scripts land in `Assembly-CSharp`. If you add an asmdef later you **must explicitly reference** `Unity.XR.Interaction.Toolkit`, `Unity.XR.CoreUtils`, `Unity.InputSystem` and `UnityEngine.UI` — package assemblies are **not** granted automatically.
- **New Input System only:** the legacy `Input.GetKey` / `Input.GetAxis` APIs are **unavailable**; use `InputAction` / `InputActionReference`. An `EventSystem` with the XR UI Input Module is already set up for world-space UI.
- **Prefer the built-in XRI components** (`XRDirectInteractor`, `XRGrabInteractable`, `XRPokeInteractable`) for grabbing, poking and knobs over writing your own raycasting.

---

## 10. 现状注意事项 / Current Notes & Gotchas

**中文**

以下均为**事实与风险**，非规划。

### 室内素材包占据两份副本，合计 1.25 GB

| 路径 | 体量 | 内容 |
|---|---|---|
| `Assets/Artassert/FANTASTIC - Interior Pack/` | **825 MB** | **原始下载包**：`Standard_FANTASTIC_Interior_Pack.unitypackage`（内置管线）+ `URP_FANTASTIC_Interior_Pack.unitypackage`（URP）+ 快速上手说明 |
| `Assets/Artassert/Fantastic Interior Pack/` | **427 MB** | **已导入的 URP 版**（实际被使用的资产都在这里） |

这是「原始归档 + 导入结果」的关系，**不是误操作**。但需要注意：**内置管线那份 `.unitypackage` 对本 URP 工程没有用处**，连同 URP 归档共 825 MB，是可以回收的空间。

### Bundle Identifier 仍是模板默认值

| 平台 | 值 |
|---|---|
| Android | `com.unity.template.vr` |
| Standalone | `com.Unity-Technologies.VR-Template` |

`companyName` 也仍为 `DefaultCompany`，`productName` 为 `Traditional Chinese Medicine Simulator`。**正式发布前必须修改。**

### 无版本控制

工程**非 git 仓库**，当前没有任何版本控制。1.4 GB 的 `Assets/` 中真正需要版本化的只有 `Assets/Scenes/`、`Assets/Settings/`、`ProjectSettings/`、`Packages/` 等少量内容，其余是体积庞大的第三方素材——**若要引入 git，建议先配置好 `.gitignore` 与 LFS 策略再初始化**。

### 本地生成目录

`Library/`、`Temp/`、`Logs/`、`UserSettings/` 由 Unity 本地生成，不应纳入版本控制。

**English**

Everything below is fact or risk — not a roadmap.

### The interior pack occupies two copies, 1.25 GB total

`Artassert/FANTASTIC - Interior Pack/` (**825 MB**) holds the **original downloads**: `Standard_FANTASTIC_Interior_Pack.unitypackage` (built-in pipeline), `URP_FANTASTIC_Interior_Pack.unitypackage` (URP), and a quick-start readme. `Artassert/Fantastic Interior Pack/` (**427 MB**) is the **imported URP version** — all actively used assets live here.

This is "original archive + imported result", **not a mistake**. Worth knowing though: the **built-in-pipeline `.unitypackage` is useless to this URP project**, so together with the URP archive, **825 MB is reclaimable**.

### Bundle identifiers are still template defaults

Android uses `com.unity.template.vr` and Standalone uses `com.Unity-Technologies.VR-Template`; `companyName` is still `DefaultCompany`. `productName` is `Traditional Chinese Medicine Simulator`. **All of these must change before release.**

### No version control

The project is **not a git repository**. Of the 1.4 GB under `Assets/`, only a small part (`Assets/Scenes/`, `Assets/Settings/`, `ProjectSettings/`, `Packages/`) genuinely needs versioning — the rest is bulky third-party material. **If you introduce git, set up `.gitignore` and an LFS strategy before initializing.**

### Local generated directories

`Library/`, `Temp/`, `Logs/` and `UserSettings/` are generated locally by Unity and should not be tracked.
