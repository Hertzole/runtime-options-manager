# Unity Runtime Settings
### Experimental drop-in solution for runtime settings

⚠ This is very experimental as of right now! ⚠

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=unity-runtime-settings&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=unity-runtime-settings)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=unity-runtime-settings&metric=coverage)](https://sonarcloud.io/summary/new_code?id=unity-runtime-settings)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=unity-runtime-settings&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=unity-runtime-settings)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=unity-runtime-settings&metric=bugs)](https://sonarcloud.io/summary/new_code?id=unity-runtime-settings)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=unity-runtime-settings&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=unity-runtime-settings)

## ❓ What is this?

Unity runtime settings is supposed to be a drop-in solution for managing runtime settings for your game. Settings like
resolution, audio volumes, graphics, etc. 

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
   - `se.hertzole.settingsmanager`
2. Click apply and close the project settings.
3. Open up the package manager.  
   Click in the menu bar Window → Package Manager
4. Select `Packages: My Registries` in the menu bar of the package manager window.
5. You should see Unity Runtime Settings under the `Hertzole` section. Click on it and then press Install in the bottom right corner.

### Unity package manager through git
1. Open up the Unity package manager
2. Click on the plus icon in the top left and "Add package from git url"
3. Paste in `https://github.com/Hertzole/unity-runtime-settings.git#package`  
   You can also paste in `https://github.com/Hertzole/unity-runtime-settings.git#dev-package` if you want the latest (but unstable!) changes.
