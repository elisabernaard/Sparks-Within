# 🕹️ Sparks Within – Unity VR Experience

**Sparks Within** is a VR-based immersive experience where the player temporarily becomes various non-human entities. Designed for players with limited mobility, the game invites the player to drift between beings through gaze and sound.

---

## 🧠 Collaboration Guidelines

- All developers must work on **separate branches** (`feature/<name>-<feature>`)
- Pull requests (PRs) are **required** to merge into `dev`
- The `main` branch is protected for release builds only
- PRs must be reviewed and approved before merging

---

## 🧱 Project Structure

| Folder                             | Description                             |
| ---------------------------------- | --------------------------------------- |
| `Assets/Scenes/scene_<name>.unity` | Personal working scenes per team member |
| `Assets/Scripts/`                  | Core scripts                            |
| `Assets/Audio/`                    | Sound assets                            |
| `Assets/Prefabs/`                  | Reusable game objects                   |

---

## 🎮 Scene Workflow

- Each team member works in their **own scene file**
  - Example: `scene_onboarding.unity`, `scene_first.unity`
- Final integration happens via a shared `FinalScene.unity`
- Avoid merging `.unity` files directly — instead export content as **prefabs** or **use additive loading**

---

## 🛠 Unity Settings

- **Version Control**: `Visible Meta Files`
- **Asset Serialization**: `Force Text`
- All `.meta` files must be tracked by Git

---

## 🧬 Branching Strategy

| Branch                     | Purpose                                     |
| -------------------------- | ------------------------------------------- |
| `main`                     | Final, stable build for release (protected) |
| `dev`                      | Integration branch for ongoing work         |
| `feature/<name>-<feature>` | Individual development branches             |

---

## 🧩 Final Scene Integration

- All scenes are combined through `FinalScene.unity`
- Use `SceneManager.LoadSceneAsync(..., LoadSceneMode.Additive)` if needed
- Prefab-based scene merging is preferred for stability
