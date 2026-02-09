# CLAUDE.md

本文档为 Claude Code (claude.ai/code) 在处理本仓库代码时提供指导。

## 语言规则
- 所有解释、推理和评论必须用简体中文书写。
- 除非是代码、标识符或不可避免的技术术语，否则不得使用英语。
- 错误解释和总结必须用中文。

## 快速构建与调试

```powershell
# 构建并运行（最常用）
dotnet run --project STranslate/STranslate.csproj

# 或构建后运行
dotnet build STranslate.sln --configuration Debug
./.artifacts/Debug/STranslate.exe
```

更多构建选项参见 [项目概述](docs/overview.md)。

## 多语言支持

本项目支持以下五种语言，语言文件位于 [`STranslate/Languages/`](STranslate/Languages/) 目录：

| 语言 | 文件 | 说明 |
|------|------|------|
| 简体中文 | `zh-cn.xaml` | 默认语言 |
| 繁体中文 | `zh-tw.xaml` | 台湾/香港地区 |
| English | `en.xaml` | 英语 |
| 日本語 | `ja.xaml` | 日语 |
| 한국어 | `ko.xaml` | 韩语 |

### 添加或修改国际化字符串

1. 在所有语言文件中添加相同的键值（参考现有字符串格式）
2. 使用 `_i18n.GetTranslation("KeyName")` 在代码中获取翻译
3. 使用 `Internationalization.GetString("KeyName")` 在 XAML 中绑定

### 开发注意事项

- 新增用户可见的提示信息时，必须添加对应的国际化字符串
- 建议按模块分类组织语言文件（参考现有文件的注释分组）
- 所有语言文件必须保持键的一致性（相同的键存在于所有语言文件中）

## 文档导航

本文档已按功能模块拆分为以下子文档：

### 快速开始
- [**项目概述**](docs/overview.md) - STranslate 项目简介、主要功能、构建命令

### 架构设计
- [**架构设计**](docs/architecture.md) - 核心架构说明
  - 启动流程 - 应用程序启动过程
  - 插件系统 - 插件加载与管理
  - 服务管理 - Service 与 Plugin 的关系
  - 关键接口 - IPlugin、IPluginContext 等接口定义
  - 数据流 - 翻译功能的数据流示例

### 功能特性
- [**功能特性**](docs/features.md) - 热键系统、剪贴板监听、历史记录

### 存储与配置
- [**存储与配置**](docs/storage.md) - 设置架构、存储位置

### 插件开发
- [**插件开发指南**](docs/plugin.md) - 插件开发、包格式、社区插件开发

### 开发参考
- [**参考信息**](docs/reference.md) - 关键文件索引、修改核心服务/UI、技术栈与依赖项

## 快速参考

| 任务 | 相关文档 |
|------|---------|
| 了解项目结构 | [项目概述](docs/overview.md) |
| 构建项目 | CLAUDE.md 快速构建 或 [项目概述](docs/overview.md) |
| 开发插件 | [插件开发指南](docs/plugin.md) |
| 修改热键功能 | [功能特性](docs/features.md) |
| 修改剪贴板监听 | [功能特性](docs/features.md) |
| 修改历史记录 | [功能特性](docs/features.md) |
| 修改核心服务/UI | [参考信息](docs/reference.md) |
| 查找关键文件 | [参考信息](docs/reference.md) |
