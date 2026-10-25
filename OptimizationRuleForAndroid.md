# 

# \# Unity Android Asset Optimization Rules

# 

# \## 1. Core Rules

# 

# \- Always inspect the existing Unity project, Unity version, render pipeline, target Android devices, and current asset import settings before modifying assets.

# \- Optimize assets for Android mobile devices by default, especially for 2D puzzle and hybrid-casual games.

# \- Preserve visual quality and existing gameplay functionality while optimizing.

# \- Never blindly reduce every texture to the same resolution or compression format.

# \- Never replace, delete, rename, or overwrite source assets without a clear reason.

# \- Make the smallest safe change and avoid modifying unrelated assets.

# \- Measure texture memory, build size, rendering performance, and visual quality before and after significant optimization.

# \- Prefer automated, repeatable import settings over manually configuring every texture.

# \- Do not modify `.meta` files, GUIDs, or asset references unnecessarily.

# \- Do not reimport or modify the entire project when only a small group of assets needs optimization.

# 

# \## 2. Source Image File Rules

# 

# \- Use PNG for sprites requiring transparency, including bottles, colored sauces, characters, icons, and UI elements.

# \- Use JPG for opaque backgrounds and photographs when transparency is unnecessary.

# \- Use PSD or another editable source format only when required for the art workflow. Import the optimized exported image into Unity when appropriate.

# \- Avoid importing unnecessarily large source images.

# \- Remove unused transparent margins when doing so will not break sprite alignment, pivots, slicing, animation, or nine-slicing.

# \- Do not upscale small images to larger resolutions without a visual requirement.

# \- Avoid duplicate images when existing assets can be reused.

# \- Keep source images organized and use consistent, descriptive filenames.

# 

# \### Recommended source resolutions for a portrait 2D puzzle game

# 

# These are starting points, not mandatory limits. Adjust them according to the maximum size at which an asset appears on screen.

# 

# | Asset | Suggested source resolution |

# |---|---|

# | Bottle sprite | 256×512 |

# | Detailed bottle sprite | 512×1024 |

# | Colored sauce or liquid | 128×256 to 256×512 |

# | Small icon | 64×64 to 128×128 |

# | Large UI icon | 128×128 to 256×256 |

# | Full-screen background | 1080×1920 |

# | Large detailed background | 1440×2560, only when justified |

# | Small decorative element | 64×64 to 256×256 |

# 

# \- Use the lowest resolution that preserves acceptable quality at the asset's largest intended on-screen size.

# \- Consider the actual sprite size, Canvas scaling, device resolution, camera zoom, and maximum display size before choosing a resolution.

# \- Do not force all assets into square dimensions.

# \- Do not increase source resolution solely because the target device has a high-resolution display.

# 

# \## 3. Power-of-Two Texture Rules

# 

# \- Prefer power-of-two dimensions for repeating textures, mipmapped textures, texture streaming workflows, and assets that benefit from predictable memory usage.

# \- Power-of-two dimensions include 64×64, 128×128, 256×256, 512×512, and 1024×1024.

# \- For sprites and UI, non-power-of-two dimensions are allowed when they are appropriate for the artwork and supported by the target platform.

# \- Do not distort an image to force power-of-two dimensions.

# \- Do not add transparent padding solely to make every sprite power-of-two without measuring the impact.

# \- When packing sprites into a Sprite Atlas, let Unity manage the atlas dimensions and packing configuration.

# \- Check atlas padding and edge bleeding when using compressed textures.

# 

# \## 4. Unity Texture Import Settings

# 

# For each imported texture, inspect and configure the following settings:

# 

# \- Texture Type

# \- Sprite Mode

# \- Max Size

# \- Filter Mode

# \- Compression

# \- Generate Mip Maps

# \- Alpha Source

# \- Read/Write Enabled

# \- Android platform override

# \- Pixels Per Unit, where applicable

# 

# \### Default settings for 2D sprites

# 

# \- Texture Type: Sprite (2D and UI).

# \- Filter Mode: Bilinear for smooth artwork; Point for pixel art.

# \- Generate Mip Maps: Disabled for normal 2D sprites and UI unless a specific rendering requirement justifies them.

# \- Read/Write Enabled: Disabled unless CPU-side code genuinely needs to access the texture pixels.

# \- Preserve Alpha: Enable or configure alpha handling correctly for transparent sprites.

# \- Max Size: Use the smallest size that preserves the required visual quality.

# \- Compression: Use a suitable compressed format for the target Android platform.

# \- Pixels Per Unit: Maintain the project's existing scale convention. Do not change it merely to reduce texture memory.

# \- Wrap Mode: Clamp for ordinary bottle sprites, UI, and isolated images; use Repeat only when the texture requires tiling.

# \- Alpha Is Transparency: Enable when appropriate for sprites with transparent edges and verify the visual result.

# 

# Do not enable or disable import settings indiscriminately across all textures. Different asset categories may require different configurations.

# 

# \## 5. Android Platform Override Rules

# 

# \- Configure Android-specific texture settings instead of relying exclusively on Default platform settings.

# \- Inspect the Android override in the texture Import Settings Inspector.

# \- Enable Override for Android when a specific Android texture format, maximum size, or compression configuration is required.

# \- Select a format compatible with the intended Android device range.

# \- Verify the effective format on a real device or in the built player. Do not assume the selected format is always used on every device.

# 

# \### Recommended Android texture compression

# 

# | Format | Use case | Important consideration |

# |---|---|---|

# | ASTC 6×6 | High-quality mobile textures | Good quality-to-memory balance on supported devices |

# | ASTC 8×8 | General-purpose artwork with greater memory savings | Inspect transparent edges and gradients |

# | ASTC 4×4 | Detailed textures that need higher quality | Higher memory usage than larger ASTC blocks |

# | ETC2 RGBA8 | Broad Android compatibility for RGBA textures on supported GLES 3.0 devices | More memory than ASTC 6×6 for the same dimensions |

# | ETC2 RGB4 | Opaque RGB textures | Do not use when alpha transparency is required |

# | Uncompressed RGBA32 | Debugging or specific exceptional cases | Avoid as the default for production mobile sprites |

# 

# \- Prefer ASTC when the minimum supported device range reliably supports the selected ASTC format.

# \- Prefer ETC2 as a practical fallback for an appropriately configured Android device range when ASTC cannot be guaranteed.

# \- Confirm the graphics API and device compatibility requirements before selecting the compression format.

# \- If using multiple texture-compression targets, configure and test Unity's platform and build settings deliberately.

# \- Do not assume ASTC is universally supported by every Android device.

# \- Do not use uncompressed textures for all sprites simply to avoid compression artifacts.

# 

# \## 6. Max Size and Compression Quality

# 

# \- Set Max Size according to the intended display size, not the original source image dimensions.

# \- Use 512 as a starting Max Size for regular sprites and 1024 for detailed sprites when their actual screen size justifies it.

# \- Reduce larger textures incrementally and compare the result at normal gameplay scale.

# \- Use 2048 or higher only when the asset's size, visual detail, or rendering requirements justify it.

# \- Set compression quality according to visual importance and memory constraints.

# \- Inspect transparent outlines, small text, gradients, thin bottle edges, and colored liquid boundaries after compression.

# \- Do not assume that the smallest build file produces the best runtime memory usage.

# \- Avoid applying aggressive compression to important UI text, logos, or assets with visible artifacts.

# 

# \## 7. Sprite Atlases and Draw Calls

# 

# \- Group sprites that are commonly rendered together into suitable Sprite Atlases.

# \- Use separate atlases for logical groups such as gameplay objects, UI, and menus when that improves loading or memory management.

# \- Avoid creating one enormous atlas containing every asset in the game.

# \- Configure atlas texture compression and Max Size for Android.

# \- Enable tight packing only when compatible with the sprite's rendering requirements.

# \- Preserve sufficient atlas padding to avoid color bleeding around transparent edges.

# \- Check whether UI and gameplay objects share atlas textures where appropriate.

# \- Use the Frame Debugger and Profiler to verify whether atlas usage actually reduces draw calls or texture switches.

# \- Do not create an atlas for every individual sprite.

# 

# \## 8. Transparency and Colored Sauce Assets

# 

# \- Keep transparent bottle sprites and colored liquid sprites separate when the liquid color or fill level changes dynamically.

# \- Prefer tinting a reusable white or neutral liquid sprite when this satisfies the art requirements.

# \- Avoid creating separate texture files for every liquid color when a shared sprite and material can achieve the same result.

# \- Preserve the alpha channel and ensure the selected Android compression format supports the required transparency.

# \- Avoid unnecessary full-screen transparent overlays, oversized translucent textures, and overlapping UI layers.

# \- Use Sprite Masks, shaders, clipping, or appropriate geometry only when they are justified by the existing rendering system.

# \- Inspect transparent bottle interiors and liquid edges on Android for halos, dark fringes, color bleeding, and compression artifacts.

# \- Avoid Read/Write Enabled for textures that are only rendered by the GPU.

# \- Do not introduce a custom shader or rendering system solely for optimization without checking the existing implementation.

# 

# \## 9. Backgrounds, UI, and Large Textures

# 

# \- Use appropriately sized backgrounds instead of unnecessarily large 4K images.

# \- Consider the portrait aspect ratios supported by the project and preserve the existing safe-area and responsive-layout behavior.

# \- Use TextMeshPro for dynamic UI text instead of baking frequently changing text into images.

# \- Prefer reusable UI sprites and nine-sliced sprites for scalable panels and buttons where appropriate.

# \- Avoid using large textures for simple flat-color UI elements.

# \- Avoid duplicating the same background, button, icon, or decorative sprite across multiple files without a reason.

# \- Disable mipmaps for ordinary UI textures unless a demonstrated rendering requirement needs them.

# 

# \## 10. Audio, Animation, and Other Assets

# 

# \- Configure audio compression, load type, sample rate, and mono/stereo settings according to the sound's actual requirements.

# \- Use compressed audio for appropriate background music and effects; avoid loading all audio clips into memory unnecessarily.

# \- Keep short sound effects lightweight and test for artifacts after compression.

# \- Optimize animation clips by removing unused curves and redundant keyframes where safe.

# \- Avoid oversized sprite sheets and excessive duplicated animation frames.

# \- Use Sprite Atlases for appropriate sprite collections and Addressables for content that should be loaded on demand.

# \- Optimize meshes and particle counts when they are relevant to the project's rendering workload.

# \- Remove unused assets and packages only after confirming that they are not referenced by scenes, prefabs, scripts, Addressables, Resources, or runtime loading systems.

# \- Do not optimize asset files in isolation when the real problem is excessive loading, duplicated runtime instances, or incorrect asset lifetime management.

# 

# \## 11. Memory and Loading Rules

# 

# \- Estimate texture memory using the dimensions, effective texture format, mipmaps, and platform configuration.

# \- Remember that source PNG/JPG file size does not equal GPU texture memory.

# \- Avoid keeping unused textures and audio assets resident in memory.

# \- Use Addressables or other appropriate loading strategies for large or optional content.

# \- Release Addressables handles and other owned resources correctly.

# \- Avoid unnecessary synchronous asset loading during gameplay.

# \- Check for duplicate texture memory caused by separate assets, materials, atlases, or runtime loading paths.

# \- Use the Unity Profiler, Memory Profiler, and platform build tools to identify actual memory bottlenecks.

# \- Test on real Android devices with different memory capacities and graphics capabilities.

# 

# \## 12. Android Build Optimization

# 

# \- Use the project's supported Android build configuration and validate the minimum Android version.

# \- Use Android App Bundles (AAB) for Google Play distribution when appropriate.

# \- Review texture-compression settings and texture formats for the actual Android build targets.

# \- Strip unused managed code and engine modules when safe and supported by the project configuration.

# \- Review managed stripping, IL2CPP, and native library settings without breaking reflection, SDKs, or runtime-loaded code.

# \- Remove unused packages and plugins only after dependency checks.

# \- Avoid adding large assets to Resources when they do not need to be available through Resources.Load.

# \- Use Addressables for optional levels or content when it provides a real loading or download-size benefit.

# \- Compare build size after changes and inspect Unity's build reports to identify large assets and dependencies.

# \- Never claim an optimization succeeded based solely on a smaller source file or a successful build.

# 

# \## 13. Required Validation Workflow

# 

# For every significant asset optimization:

# 

# 1\. Inspect the source dimensions, format, transparency, current importer settings, and usage.

# 2\. Record the original settings before changing them.

# 3\. Select an appropriate Max Size, compression format, and Android override.

# 4\. Reimport only the required assets.

# 5\. Compare the original and optimized appearance at actual gameplay scale.

# 6\. Verify transparency, filtering, atlas edges, and UI layout.

# 7\. Build and run on a representative Android device.

# 8\. Check runtime texture memory, loading behavior, frame performance, and build size.

# 9\. Confirm there are no missing references, broken sprites, or unexpected changes.

# 10\. Report the changes and any trade-offs.

# 

# \### Optimization report

# 

# For meaningful optimization changes, report:

# 

# \- Assets affected

# \- Original and final dimensions

# \- Original and final import settings

# \- Android compression format

# \- Before-and-after file or build size when measured

# \- Runtime memory impact when measured

# \- Visual quality checks performed

# \- Device or build configuration used for validation

# 

# If a measurement is unavailable, state that it was not measured rather than inventing a result.

# 

# \## 14. Final Priority

# 

# Prioritize optimizations in this order:

# 

# 1\. Remove unnecessary oversized textures.

# 2\. Configure Android-specific texture compression and Max Size.

# 3\. Disable unnecessary mipmaps and CPU-readable texture copies.

# 4\. Reuse sprites and optimize Sprite Atlases.

# 5\. Optimize audio, animations, and loading behavior.

# 6\. Profile actual runtime memory and rendering performance.

# 7\. Validate quality and compatibility on real Android devices.

# 

# Always prefer measured improvements over arbitrary asset-size limits. Preserve gameplay, visual quality, and compatibility with the project's supported devices.

# 

# 

# \## 15. Automated Asset Optimization and Importer Management

# 

# \### Inspect, Apply, and Validate

# 

# \- Do not only recommend asset optimization settings. When the task authorizes changes, inspect the existing assets and apply appropriate settings through Unity's supported texture importer APIs.

# \- Before making changes, inspect the current texture import settings, asset usage, platform overrides, and project conventions.

# \- Use `AssetImporter`, `TextureImporter`, `TextureImporterPlatformSettings`, and other appropriate Unity Editor APIs when automating asset optimization.

# \- Apply Android-specific overrides when required by the target devices and project configuration.

# \- Preserve existing sprite references, GUIDs, sprite slicing, pivots, borders, and other important importer settings.

# \- Validate the resulting importer settings after applying changes. Reimport affected assets only when necessary.

# \- Where practical, build or run the project on Android and verify visual quality, texture compatibility, memory usage, and build size.

# \- If changes cannot be applied or validated because of missing permissions, unavailable tools, or build limitations, clearly report what remains incomplete.

# 

# \### Idempotent Import Automation

# 

# \- Asset optimization automation must be idempotent: running it repeatedly with unchanged requirements must not repeatedly modify assets or trigger unnecessary reimports.

# \- Before changing an importer setting, compare the desired value with the existing value.

# \- Modify only settings that differ from the intended configuration.

# \- Call `SaveAndReimport()` only when an actual importer configuration change is necessary.

# \- Do not force asset reimports on every editor launch, domain reload, script compilation, or editor refresh.

# \- Do not run bulk optimization automatically on every import or project opening unless explicitly required by the task.

# \- Avoid unnecessary calls to `AssetDatabase.Refresh()` and avoid recursive import-processing loops.

# \- For automated bulk changes, process only the intended asset paths and report how many assets were inspected, changed, skipped, or failed.

# \- Keep optimization scripts safe to rerun and prevent repeated processing of unchanged assets.

# \- Do not overwrite user-customized settings without checking whether they conflict with the requested optimization policy.

# \- If an asset already satisfies the optimization requirements, leave it unchanged.

# 

# \### Validation and Reporting

# 

# After applying optimizations:

# 

# 1\. Re-read the affected importer settings and confirm they match the intended configuration.

# 2\. Confirm that Android platform overrides and texture formats are configured correctly.

# 3\. Verify that sprite references, slicing, pivots, alpha transparency, and atlas compatibility remain intact.

# 4\. Check the Unity Console for import errors and relevant warnings.

# 5\. Run an Android build or device test when required and feasible.

# 6\. Compare before-and-after measurements when available. Do not claim memory or build-size improvements without evidence.

# 7\. Summarize the changes, unchanged assets, validation results, and any remaining risks.

# 

# \*\*Mandatory rule:\*\* Inspect first, change only what is necessary, reimport only when needed, and validate after applying changes. Never repeatedly rewrite importer settings just because the Unity Editor refreshes.

