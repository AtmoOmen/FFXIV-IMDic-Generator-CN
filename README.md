# FFXIV-IMDic-Generator-CN

## **起因:**

之前用的自定义词库文件的帖子被锁，其他还存活着的词库文件对应的游戏版本都过老，因此想写一个能自动生成输入法词库的工具，方便自己方便大家（本质是个很简单的小工具，我也没怎么写过图形界面，如果有功能/交互上的问题麻烦回复或者在 [Github](https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN) 上提 Issue 告知我！）



## **介绍:**

适用于 最终幻想14 的中文输入法自定义词库生成器，包含本地生成和在线自动生成两类主要功能，数据源来自 FFCafe 的 [ffxiv-datamining-cn](https://github.com/thewakingsands/ffxiv-datamining-cn)。
因为数据源为全自动生成上传的，因此拉取在线文件自动生成的词库也是可以随着游戏版本更新而自动更新的！

软件现已融合调用 [深蓝词库转换](https://github.com/studyzy/imewlconverter) 软件的部分，可以直接在软件内进行词库转换 (感谢开源!)



## **使用说明:**

### **从本地文件生成**

1. 点击 "选择..." 按钮选择包含指定 .csv 数据文件的文件夹
2. 点击 "从本地文件生成" 按钮即可

### **从在线文件生成**

直接点击 "从在线文件生成" 按钮即可，无须选择文件夹

注: 你可以通过修改程序同一目录下的 Links.txt 文件来自定义你想要生成的词库数据范围

### **生成指定类型/格式词库**

在 "输出格式转换" 栏目点击 "转换后格式" 下拉栏，选择你想要的输入法和词库格式

注: 请务必看好 "转换后格式" 下拉栏中各项目的每一个字以及括号内的注解，避免产生各种不必要的麻烦、问题



## **常见问题:**

1. 从在线文件生成 速度过慢/显示"下载文件时发生错误/读取CSV文件时发生错误"等等
    答: 请自行寻找能稳定访问 Github 的方法 (网上方法/工具很多)，或者在 Links.txt 里批量替换链接至 Github 的国内镜像网址等等 (可能发生安全问题)
2. 词库数据量数量过大
    答: 请尝试调整数据源(本地的直接将几个特殊的csv文件挪出来即可，在线的请自行调整 Links.txt 文件里的链接)，或采取其他方法降低词库内的词条数量。
3. 搜狗输入法无法导入词库
    答: 请将转换后格式调整为 "纯拼音无汉字 (搜狗新版)"



## **温馨提示:**

由于一些输入法(点名微软输入法)的对于自定义词库的处理/显示逻辑，使用该词库可能会(严重)影响你的日常输入 (不过一般来说只要撑过一段时间也会恢复正常的)



## **下载:**

**[Github](https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN/releases)**



## **附件:**

### **部分数据文件 名称-大致内容 对照表**

| 数据文件名                | 大体内容                |
|-----------------------|-----------------------|
| Action                | 技能                    |
| BaseParam             | 各类基础属性              |
| BuddyEquip            | 陆行鸟装甲                 |
| ChocoboRaceAbility    | 陆行鸟竞赛技能             |
| ChocoboRaceItem       | 陆行鸟竞赛物品             |
| ClassJob              | 职业                    |
| Companion             | 宠物                    |
| CompanyAction         | 部队特效                  |
| CompanyCraftDraft     | 部队工坊设计图             |
| ContentGauge          | 部分副本内进度条文本         |
| ContentsTutorial      | 特殊场景名词               |
| CraftAction           | 生产技能                  |
| PlaceName             | 地名                    |
| AnimaWeapon5PatternGroup | 部分特殊武器相关         |
| AOZScore              | 青魔成就                  |
| BeastTribe            | 友好部族名词               |
| Quest                 | 任务名 (数据量最大的项目之一) |
| Item                  | 物品名 (数据量最大的项目之一) |
| DeepDungeonItem       | 深层迷宫物品                |
| DeepDungeonMagicStone | 深宫魔陶器                |
| DynamicEvent          | 博兹雅/高原 CE 名           |
| DynamicEventEnemyType | CE 敌人类型名称           |
| Emote                 | 情感动作                  |
| EurekaAetherItem      | 优雷卡以太壶               |
| EventItem             | 各种任务物品名              |
| GrandCompany          | 部队相关                  |
| HousingPreset         | 房屋部件相关               |
| Pet                   | 召唤兽名称                |
| PetAction             | 召唤兽技能                |
| PetMirage             | 召唤兽名称                |
| World                 | 服务器名称                |
| Title                 | 各种称号                  |
| TripleTriadCard       | 九宫幻卡 (用来补充重要 NPC 名称的) |
| TripleTriadCardType   | 九宫幻卡类型 (同上)         |
| Weather               | 天气                    |
