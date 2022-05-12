# percentage

> 基于 [kas/percentage](https://github.com/kas/percentage) 项目开发

## 功能

在托盘显示你的电池百分比，Windows 10、Windows 11 可用

![Windows 11 示例图](https://github.com/marxti/percentage/blob/dev/percentage_win11.png)

1. 右键菜单提供字体设置功能，可以自行设置所需字体
2. 可自适应系统黑暗模式
3. 由于托盘图标宽度固定，所以在 100% 电量时显示 00
4. 充电时，显示为绿色
5. 电量低于 20% 时，显示为红色

## 安装

1. 自行编译或下载最新 release 版本
2. 将 percentage.exe 放到系统启动文件夹
> 要打开系统启动文件夹，点击 Windows+R，输入 "shell:startup", 然后点击 Enter

## 开发工具

本项目使用 C# 通过 Visual Studio Community 2022 编译开发
