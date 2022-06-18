# Runtime Options Manager
### Scriptable object based drop-in solution for runtime settings

⚠ This is very experimental as of right now! ⚠

[![OpenUPM](https://img.shields.io/npm/v/se.hertzole.runtime-options-manager?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/se.hertzole.runtime-options-manager/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=runtime-options-manager&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=runtime-options-manager)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=runtime-options-manager&metric=coverage)](https://sonarcloud.io/summary/new_code?id=runtime-options-manager)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=runtime-options-manager&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=runtime-options-manager)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=runtime-options-manager&metric=bugs)](https://sonarcloud.io/summary/new_code?id=runtime-options-manager)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=runtime-options-manager&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=runtime-options-manager)

## ❓ What is this?

Runtime Options Manager is supposed to be a drop-in solution for managing runtime settings for your game. Settings like
resolution, audio volumes, graphics, etc. It's all based around scriptable objects to easily manage your settings in the
editor.

![Settings Manager in project settings](https://i.imgur.com/rvus6eV.png)

![Example setting](https://i.imgur.com/7znry27.png)

## 🔨 Quick Start

1. Install the package either using a git link or OpenUPM. See installation section for instructions.
2. Open up your project settings inside Unity and create a new settings manager.
3. Create a setting in your project by right clicking - Create - Hertzole - Settings - the setting of your choice
4. Create a category in your settings manager and assign the settings.
5. Build your UI. I'd recommend importing the uGUI sample to see how you can create a simple UI for your settings.

## 📦 Installation

### OpenUPM (Recommended)
1. Add the OpenUPM reigstry.   
   Click in the menu bar Edit → Project Settings... → Package Manager
   Add a new scoped registry with the following parameters:  
   Name: `OpenUPM`  
   URL: `https://package.openupm.com`  
   Scopes:  
   - `com.openupm`  
   - `se.hertzole.runtime-options-manager`
2. Click apply and close the project settings.
3. Open up the package manager.  
   Click in the menu bar Window → Package Manager
4. Click the `+` in the top left corner and select `Add package by name...` or `Add package from git URL...`
5. Paste in `se.hertzole.runtime-options-manager` into the field and click add

### Unity package manager through git
1. Open up the Unity package manager
2. Click on the plus icon in the top left and "Add package from git url"
3. Paste in `https://github.com/Hertzole/runtime-options-manager.git#package`  
   You can also paste in `https://github.com/Hertzole/runtime-options-manager.git#dev-package` if you want the latest (but unstable!) changes.
