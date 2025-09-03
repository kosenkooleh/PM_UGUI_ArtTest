# PM_UGUI_ArtTest (Unity 2022.3.62f1 LTS)

Two demo scenes in one project:

- **01_Popup_Demo** — UI popup showcase
- **02_Octopus_Demo** — reusable octopus prefab (no bones)
- Optional launcher: **00_Launcher**

## Requirements
- Unity **2022.3.62f1 LTS**
- Packages: TextMeshPro, 2D Animation, 2D IK (installed via `Packages/manifest.json`)

## Scenes & Build Order
`00_Launcher` → `01_Popup_Demo` → `02_Octopus_Demo`  
(Stored in `ProjectSettings/EditorBuildSettings.asset`.)

## How to Run
- Open `Assets/Scenes/00_Launcher.unity` and press **Play**.
- Buttons:
  - **Open Popup** — loads `01_Popup_Demo`
  - **Open Octopus** — loads `02_Octopus_Demo`
  - **Exit** — quits (handled per platform)
- **ESC**: in demos → back to launcher; in launcher → quit

---

## Popup Demo (01_Popup_Demo)

### What to look for
- **Animator** on `Popup` with states: `Hidden` (default), `Open`, `Close`.
- Triggers: `Open`, `Close`.
- CanvasGroup gating: buttons become interactable only after `Open` finishes.
- Close sources: **Close button**, **BackDim**, **Collect** (all call `Animator.SetTrigger("Close")`).
- Collect button shine: local Animator; started via animation event at **t = 5.0s**.

### Change the number of demo buttons (IMPORTANT)
You can dynamically change how many demo buttons are shown.

- **Where:** `Canvas/Popup/PopUpRoot/Content/ButtonsContainer`
- **Component:** `ButtonsDemo` (script on the container)
- **How:** In **Play Mode**, select `ButtonsContainer` and adjust the `Count` **slider** in the **Inspector**.  
  The list regenerates immediately.
- **Note:** This is a runtime-only tweak for demonstration; it is not persisted to the scene on exit.

### Audio
- UI click SFX is played via a single `AudioSource` on the popup root (no per-button sources).
- Only one AudioClip is referenced and triggered from button events.

### Performance
- All UI sprites packed into a single **SpriteAtlas**; Images share **UI/Default** material.
- TextMeshPro uses a shared font asset and (at most) one accent material preset.
- In **Screen Space – Overlay**, base popup renders at ~**3–4 draw calls** (Images + TMP).
- Additional FX (particles/lights) are optional and will add calls, as expected.

---

## Octopus Demo (02_Octopus_Demo)
- Reusable prefab: `Assets/Prefabs/Character/Octopus.prefab`
- Clean root transform (Position 0,0,0; Rotation 0,0,0; Scale 1)
- Optional `SortingGroup` on the root to keep ren
