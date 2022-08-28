## [0.3.0] - 2022-08-28
### Added
- Added ButtonSetting for creating buttons in your settings
- Added option to decide how language names are displayed
- Added `ISettingWriter` to overwrite how files are written
- Added language sample
- Added full support for refresh rate setting
- Added methods to add and remove categories and settings inside categories

### Changed
- All saveable settings now inherit from Setting and Setting inherits from BaseSetting
- **[BREAKING]** Int/Float/AudioSetting now uses `ToggleableInt`/`ToggleableFloat` instead for min/max values

### Removed
- Removed settings creating UI toolkit elements from scratch

### Fixed
- Fixed settings being marked as dirty on boot/load and thus saving on boot/load
- Fixed resolution dropdown value returning the wrong value
- Fixed fullscreen mode dropdown value returning the wrong value
- Fixed input setting not loading keybinds correctly in some cases
- Fixed the game starting in the wrong resolution
- Fixed the game not booting in some situations
- Fixed language settings sometimes returning null
- Fixed v-sync setting not applying v-sync on load
- Fixed audio setting not loading properly in some cases
- Fixed settings going back to default values in the editor
- Fixed null reference exception when opening settings manager in the inspector
- Fixed tests being included in package

## [0.2.0] - 2022-06-18
### Added
- LanguageSetting now implements `IDropdownValues`
- Added missing assembly definitions to samples

### Changed
- Renamed package to 'Runtime Options Manager'
- Changed all namespaces from `Hertzole.SettingsManager` to `Hertzole.OptionsManager`

### Fixed
- Fixed language setting stopping the game from booting

## [0.1.2] - 2022-06-13

### Fixed
- Fixed OpenUPM samples

## [0.1.1] - 2022-06-13

### Fixed
- Fixed compile issue when not using localization

## [0.1.0] - 2022-06-12

First release!