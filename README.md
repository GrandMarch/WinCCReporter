# WinCCReporter

#### 介绍
wincc自定义报表

#### 软件架构
表格显示部分使用了Reogrid（https://reogrid.net/）

数据存储使用MariadDB（进行了分表）

数据采集基于WinCC

软件一共由两部分组成：数据采集和报表显示。

数据采集部分有readwincctosql完成

报表显示由winccreportcontrol完成

DotNetFormula、MySqlParallelQuery项目是辅助项目。

#### 安装教程

1.  安装.Net Frame Work 4.5以上
2.  在wincc的开发环境中加载.net控件winccreportcontrol.dll
3.  拖拽加载完毕的控件到窗口
4.  使用xml定义报表并放置到winccreportcontrol.dll所在的db文件夹
5.  编写readwincctosql的tags.tl文件.这是一个文本文件，把需要需要存储的文件以行的形式写入。注意修改数据库的配置
5.  运行wincc看效果

#### 报表配置文件说明

1.  WinCCReporter/WinCCReportControl/db/中有几个报表实例。信息不是很敏感，没有删除太多。每一个xml文件都是一个报表。


