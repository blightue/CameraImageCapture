# Changelog
All notable changes to this project will be documented in this file.

## [0.6.5]-2023-01-27

### Added

- Added a static class `Photographer` as a singleton image capture

## [0.6.4]-2022-11-16

### Fixed

- Fixed Capture Components wont save editor changes issue

## [0.6.3]-2022-10-17

### Update

- Removed unused log

## [0.6.2]-2022-08-30

### Fixed

- Fixed CICEditorWindow "CIC" namespace not found issue on mac

## [0.6.1]-2022-08-09

### Fixed

- Fixed CIC editor Reveal in Finder button not show on MacOS issue
- Fixed CaptureAndSave method in build-in render pipeline issue

## [0.6.0]-2022-07-27

### Update

- Redesign CIC core Interface -> CIC core now is a interface with extension method
- Alpha fix now check is camera is output alpha channel

### Added

- Implement CICEditorWindow --- Open via Tools->Camera Image Capture->Capturer

## [0.5.1]-2022-07-24

### Fixed

- Fixed image has no alpha channel when using URP with **PostProcessing** enabled issue

## [0.5.0]-2022-07-21

### Added

- Add CIC Config component & CIC Config scriptable object
- Add CIC editor **Open in exporter** button
- Add CIC config use demo

### Changed

- Change CIC, CIC config, Capture config icon
- Move cic editor inspector func to cic editor base

## [0.4.0]-2022-07-17

### Added

- `DataReader` Implement

### Update

- `DataSaver` is now obsolete, use `DataWriter` instead

## [0.3.5]-2022-07-15

### Remove

- Remove property `IsOverrideFile` in CIC component

### Add

- Add property `IsLogCapture` in CIC component

## [0.3.4]-2022-06-28

### Update

- Reconstruct capture information

### Added

- Check logged image-file serials is correct after read capture information file

## [0.3.3]-2022-06-23

### Fixed

- Fixed render texture memory leak issue

## [0.3.2]-2022-06-22

### Fixed

- Fixed component field won't save issue

## [0.3.1]-2022-06-20

### Added

- Demo gif in README
- Component editor add `Is Override Camera Resolution` option

## [0.3.0]-2022-06-17

### Changed

- Package scripts namespace changed (CIC -> SuiSuiShou.CIC)
- Component editor redraw

### Added

- Added a script  icon to CIC Component

## [0.2.0]-2022-05-03

### Add

- Add logging capture history to local support for generating correct image serials

## [0.1.2]-2022-01-15

### Add

- Overwrite file option
- Image write type (main thread / Async)
- Filename serialized option (-0/-1/-2 ...)
- Image format option

## [0.1.1] - 2022-01-12
### Changed

- readme.md setup & how to use
- `CameraImageCaptureEditor.cs` for more readable.

  
