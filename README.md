# UniversalRP 自定义渲染管线

# [![Build](https://github.com/penrose/penrose/actions/workflows/build.yml/badge.svg)](https://github.com/devagame/UniversalRP/actions/workflows/blank.yml)  [![Author](https://img.shields.io/badge/Unity-2021.3.1f1+-blue.svg "")](https://github.com/devagame/ "") [![Author](https://img.shields.io/badge/UniversalRP-12.1.6+-blue.svg "")](https://github.com/devagame/ "") [![license](https://img.shields.io/github/license/devagame/universalrp)](LICENSE)

## 此项目方案作者: [Killop(夜莺)]( https://github.com/killop)

本项目是 UniversalRP 的扩展库 修复线性空间中的 UI 透明度差异问题 能够让 UI 设计师在 Unity 中保持原有的 sRBG 工作流.

### Warning:
* 场景相机 和 界面相机, 最好使用独立的 RendererData
* UI Renderer Data => Opaque Layer Mask  只勾选 UI Mask
* UI Renderer Data => Transparent Layer Mask  只勾选 UI Mask
* Forward Renderer Data => Opaque Layer Mask 勾选 Everything (暂定)
* Forward Renderer Data => Transparent Layer Mask 勾选 Everything (暂定)

![](README/01.png)