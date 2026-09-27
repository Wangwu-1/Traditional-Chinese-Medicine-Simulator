# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

> **先读这条**：上层 `E:\Gaming_Project\CLAUDE.md` 描述的是**同级另一个工程** `E:\Gaming_Project\1\2D dungeon-style side-scrolling RPG\`（2D 地牢 RPG），它提到的 `Player.cs`、`Enemy.cs`、`Real.unity` 等文件在本工程**都不存在**。那个文件对同级多个工程生效，不要改它。本工程一律以本文件为准。

## 项目定位

Unity **VR** 工程「Traditional Chinese Medicine Simulator」（中医模拟器）。

当前状态：**VR 模板工程的起点，尚无自研代码**。`Assets/` 下只有 VR Core 模板自带资源、一套室内场景素材包和一个孤立 fbx，没有任何项目自己的脚本或 asmdef。实际玩法内容尚未开始。

路径 `E:\Gaming_Project\Traditional Chinese Medicine Simulator\`，**非 git 仓库**。

## 技术栈

- Unity 2022.3.62f3c1，URP 14.0.12，**Linear 色彩空间**（VR 必需）
- XR Interaction Toolkit 3.1.2 + XR Plug-in Management 4.5.1 + OpenXR 1.14.3
- Input System 1.14.0 —— **仅启用新输入系统**（`activeInputHandler: 1`）
- 无自定义 asmdef、无 Editor 脚本、无测试程序集、无 CI 配置

## 运行方式

工程**没有命令行构建脚本**，一切通过 Unity Editor（用 Unity Hub 打开本目录）。当前 Build Settings 里唯一启用的场景是 `Assets/Scenes/SampleScene.unity`。

两个场景用途不同，选错了会浪费很多时间：

- **`Assets/Scenes/BasicScene.unity`** —— 最小 VR 场景（Directional Light / XR Interaction Manager / Plane / EventSystem + XR Origin 预制体实例）。**做功能验证用这个**，干净、加载快。
- **`Assets/Scenes/SampleScene.unity`** —— VR 模板自带的完整教程演示场景（约 17.8k 行，含 `StepManager` 分步教学流程和一堆交互示例）。参考实现可以看它，但别在上面做实验。

场景里的 XR Origin 是 Starter Assets 样例的 `XR Origin (XR Rig).prefab`（guid `f6336ac4ac8b4d34bc5072418cdc62a0`）预制体实例。改手部/交互器配置**优先改预制体本身**，而不是在每个场景里各改一遍。

## XR 配置

- 激活的 loader：`Assets/XR/Loaders/Open XR Loader.asset`，Standalone 与 Android 两个平台都指向它，`m_InitManagerOnStart: 1`。
- `Oculus Loader.asset` 和 `Open XR Loader No Pre Init.asset` 存在但**未启用** —— 别被文件名误导。
- OpenXR 已启用交互配置（跨平台共 5 个）：Standalone 下为 `OculusTouch`、`MicrosoftMotion`、`MetaQuestTouchPlus`、`KHRSimple`；Android 下为 `OculusTouch`、`MetaQuestTouchPlus`、`MetaQuestFeature`。**`Valve Index`、`HTC Vive`、`HP Reverb G2`、Mock Runtime 均未启用**。渲染模式为 Single Pass Instanced。
  - 读该 `.asset` 时注意：文件内含**未被引用的重复 "Standalone" 块**（那些孤立块里 Valve Index 等显示为启用），**只有 `Values:` 列出的 4 个容器才生效**，不要用朴素的 `grep m_enabled: 1` 下结论。
- XRI 设置集中在 `Assets/XRI/Settings/`（交互层、编辑器设置）。

## VR 模拟器（无头显开发）

已启用 XR Device Simulator。它是**全局的**：Editor 进 Play 模式时自动生成，**不需要往任何场景里放东西**。

- 开关：`Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset`
- UI 预制体与脚本样例：`Assets/Samples/XR Interaction Toolkit/3.1.2/XR Device Simulator/`
- 键位：`WASD` 平移 HMD、`Q/E` 升降、**按住鼠标右键**转视角、鼠标左键 = 右手柄 Trigger、左右手柄映射到键盘；屏幕左下角有官方操作提示图

两个非显而易见的坑：

1. **`m_UseClassic` 不影响运行时。** 加载器 `XRInteractionSimulatorLoader` 只读 `simulatorPrefab` 字段；`m_UseClassic` 仅控制设置面板里自动选哪个预制体。想换模拟器必须显式改 `simulatorPrefab`，否则只会得到一句 `prefab was missing` 警告。
2. **样例 asmdef 原引用 `Unity.XR.Hands`，本工程未安装该包**，已改成名称引用。若日后装了 XR Hands，可把引用加回去以启用 `XR_HANDS_1_1_OR_NEWER` 分支的手部手势支持。

测试**真机头显前**，先把 `m_AutomaticallyInstantiateSimulatorPrefab` 改回 `0` —— 否则模拟器会抢占 HMD 输入，你会以为设备坏了。

## 资源

- `Assets/VRTemplateAssets/` —— VR 模板自带资源。`Scripts/` 下的 `StepManager`、`XRKnob`、`Callout`、`XRPokeFollowAffordanceFill` 等都是**模板脚本，不是本项目自研代码**，改前先想清楚是否该动。
- `Assets/Artassert/Fantastic Interior Pack/` —— 室内场景素材包（作者 Tidal Flask），URP 版本，`3d/` 下分 `ENV` 与 `PROPS`。原始 unitypackage 在**同级的** `Assets/Artassert/FANTASTIC - Interior Pack/`（内含 Standard 与 URP 两个归档，其中 Standard 那份对本 URP 工程无用）。中医馆的室内场景大概率基于它。
- `Assets/Settings/Project Configuration/` —— URP 配置（Performance / Quality 两套 Config）、Android/Standalone Preset、场景模板。
- `Assets/Artassert/01.fbx` —— 单个模型（54 MB），被实例化放置在 `SampleScene.unity` 中（对象名 `01`），`BasicScene.unity` 未使用。
- `Assets/Artassert/PolyOne/Free VR Hands/` —— 一套 Rigged VR 手部模型（`Free Pack - VR Hands ( Rigged ).prefab`）及演示场景，目前仅被其自带演示场景引用。

## 写代码时注意

- **没有自定义 asmdef**，新脚本默认进 `Assembly-CSharp`。若将来要加 asmdef，必须显式引用 `Unity.XR.Interaction.Toolkit`、`Unity.XR.CoreUtils`、`Unity.InputSystem`、`UnityEngine.UI` —— 这些是包程序集，asmdef 不会自动获得。
- **只有新输入系统**：`Input.GetKey` / `Input.GetAxis` 等旧 API **不可用**，一律用 `InputAction` / `InputActionReference`。判定平面上有 `EventSystem` + XR UI Input Module 已配好。
- 手部交互现成组件（`XRDirectInteractor`、`XRGrabInteractable`、`XRPokeInteractable` 等）都在 XRI 包里，做抓取/点按/旋钮先找现成的，不要自己写射线。
