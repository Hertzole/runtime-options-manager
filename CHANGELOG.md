## [0.3.0] - Unreleased
### Added
- Added `ISettingWriter` to overwrite how files are written

### Changed
- **[BREAKING]** Int/Float/AudioSetting now uses `ToggleableInt`/`ToggleableFloat` instead for min/max values

### Fixed
- Fixed settings being marked as dirty on boot/load and thus saving on boot/load
- Fixed resolution dropdown value returning the wrong value
- Fixed input setting not loading keybinds correctly in some cases

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