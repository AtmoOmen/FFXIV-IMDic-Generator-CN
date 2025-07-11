namespace FFXIVIMDicGenerator.Configuration;

/// <summary>
/// FFXIV数据源配置
/// </summary>
public static class FfxivDataSources
{
    public static readonly Dictionary<string, string> FileTypeNames = new()
    {
        { "Seperator1", "———推荐———" },
        { "ClassJob.csv", "职业名" },
        { "Action.csv", "技能 [大数据]" },
        { "CraftAction.csv", "生产技能" },
        { "GeneralAction.csv", "共通技能" },
        { "BaseParam.csv", "角色状态参数" },
        { "Item.csv", "道具 [大数据]" },
        { "ItemSearchCategory.csv", "道具分类" },
        { "Achievement.csv", "成就名" },
        { "Title.csv", "称号" },
        { "World.csv", "服务器名" },
        { "PlaceName.csv", "地名" },
        { "Quest.csv", "任务名 [大数据]" },
        { "ContentFinderCondition.csv", "副本名" },
        { "Mount.csv", "坐骑名" },
        { "Pet.csv", "召唤兽" },
        { "ENpcResident.csv", "NPC名 [大数据]" },
        { "Emote.csv", "情感动作" },
        { "GrandCompany.csv", "大国防联军" },
        { "Weather.csv", "天气名" },
        { "GuardianDeity.csv", "守护神" },
        { "Status.csv", "状态" },
        { "Orchestrion.csv", "乐谱" },
        { "Stain.csv", "染剂颜色" },
        { "Seperator2", "———结束———" },
        { "AchievementCategory.csv", "成就分类 (细分)" },
        { "AchievementKind.csv", "成就种类 (整体)" },
        { "ActionCategory.csv", "技能类型" },
        { "ActionComboRoute.csv", "连击类技能" },
        { "Adventure.csv", "探索笔记" },
        { "Aetheryte.csv", "以太之光" },
        { "AirshipExplorationParamType.csv", "飞空艇数据" },
        { "AirshipExplorationPoint.csv", "飞空艇探索点" },
        { "SubmarineMap.csv", "潜水艇海图" },
        { "AOZScore.csv", "青魔挑战" },
        { "BannerBg.csv", "肖像背景" },
        { "BannerDecoration.csv", "肖像装饰" },
        { "BannerDesignPreset.csv", "肖像预设" },
        { "BannerFrame.csv", "肖像边框" },
        { "BannerTimeline.csv", "肖像动作" },
        { "BeastTribe.csv", "友好部族" },
        { "BeastReputationRank.csv", "友好部族等级" },
        { "BuddyAction.csv", "陆行鸟指令" },
        { "BuddyEquip.csv", "陆行鸟装备" },
        { "CharaCardBase.csv", "铭牌底色" },
        { "CharaCardDecoration.csv", "铭牌花纹" },
        { "CharaCardDesignPreset.csv", "铭牌预设" },
        { "CharaCardHeader.csv", "铭牌顶部装饰" },
        { "CharaCardPlayStyle.csv", "游戏风格" },
        { "ChocoboRaceAbility.csv", "陆行鸟竞赛技能" },
        { "ChocoboRaceItem.csv", "陆行鸟竞赛道具" },
        { "CircleActivity.csv", "同好会主要活动" },
        { "ClassJobCategory.csv", "职业分类 (不推荐)" },
        { "CollectablesShop.csv", "生产肝武商店" },
        { "CollectablesShopItemGroup.csv", "生产肝武商店分类" },
        { "Companion.csv", "宠物" },
        { "CompanionMove.csv", "宠物行动方式" },
        { "CompanyAction.csv", "部队特效" },
        { "CompanyCraftDraft.csv", "部队合建设计图" },
        { "CompanyCraftDraftCategory.csv", "部队合建设计图分类" },
        { "CompanyCraftManufactoryState.csv", "部队合建品质" },
        { "CompanyCraftType.csv", "部队合建笔记范围/种类" },
        { "CompleteJournal.csv", "已完成日志" },
        { "ContentGauge.csv", "副本机制量谱" },
        { "ContentRoulette.csv", "日随副本类型" },
        { "ContentsNote.csv", "挑战笔记" },
        { "ContentsTutorial.csv", "特殊场景探索" },
        { "ContentType.csv", "副本类型" },
        { "CraftType.csv", "生产配方类型" },
        { "DeepDungeon.csv", "深层迷宫" },
        { "DeepDungeonEquipment.csv", "深层迷宫聚魔装备" },
        { "DeepDungeonFloorEffectUI.csv", "深层迷宫魔法效果" },
        { "DeepDungeonItem.csv", "深层迷宫道具" },
        { "DeepDungeonMagicStone.csv", "深层迷宫魔石" },
        { "DpsChallenge.csv", "木人挑战" },
        { "DynamicEvent.csv", "CE 名" },
        { "DynamicEventEnemyType.csv", "CE 敌人类型" },
        { "EmoteCategory.csv", "情感动作分类" },
        { "EObjName.csv", "副本可交互对象名" },
        { "EurekaAetherItem.csv", "优雷卡以太壶" },
        { "EventAction.csv", "交互动作" },
        { "EventItem.csv", "任务道具" },
        { "ExtraCommand.csv", "特殊命令" },
        { "ExVersion.csv", "资料片版本名" },
        { "FashionCheckThemeCategory.csv", "时尚品鉴装备关键词" },
        { "FashionCheckWeeklyTheme.csv", "时尚品鉴主题" },
        { "Fate.csv", "临危受命" },
        { "FCActivityCategory.csv", "部队动态类型" },
        { "FCAuthority.csv", "部队阶级权限" },
        { "FCAuthorityCategory.csv", "部队阶级权限设置类别" },
        { "FCChestName.csv", "部队箱名" },
        { "FccShop.csv", "部队战绩交易" },
        { "FCHierarchy.csv", "部队阶级" },
        { "FCProfile.csv", "部队主要活动" },
        { "FCReputation.csv", "部队所属军队友好关系等级" },
        { "FCRights.csv", "部队功能" },
        { "FieldMarker.csv", "场景标记" },
        { "GatheringPointName.csv", "采集点名称" },
        { "GatheringType.csv", "采集点类型" },
        { "GcArmyExpedition.csv", "冒险者分队任务" },
        { "GcArmyExpeditionType.csv", "冒险者分队任务类型" },
        { "GcArmyTraining.csv", "冒险者分队训练" },
        { "GCRankGridaniaFemaleText.csv", "双蛇党军衔名 (女)" },
        { "GCRankGridaniaMaleText.csv", "双蛇党军衔名 (男)" },
        { "GCRankLimsaFemaleText.csv", "黑涡团军衔名 (女)" },
        { "GCRankLimsaMaleText.csv", "黑涡团军衔名 (男)" },
        { "GCRankUldahFemaleText.csv", "恒辉队军衔名 (女)" },
        { "GCRankUldahMaleText.csv", "恒辉队军衔名 (男)" },
        { "GCShopItemCategory.csv", "军队商店物品类型" },
        { "GilShop.csv", "金币商店类别" },
        { "GroupPoseStamp.csv", "集体动作装饰/边框" },
        { "GroupPoseStampCategory.csv", "集体动作装饰/边框分类" },
        { "HousingPreset.csv", "房屋预设" },
        { "HowTo.csv", "新手指南" },
        { "HWDAnnounce.csv", "重建公告 (废弃)" },
        { "HWDCrafterSupplyTerm.csv", "重建物品期数 (生产)" },
        { "HWDGathereInspectTerm.csv", "重建物品期数 (采集)" },
        { "IKDRoute.csv", "海钓航线" },
        { "InclusionShopCategory.csv", "工票交易类别" },
        { "ItemSeries.csv", "道具系列" },
        { "ItemSpecialBonus.csv", "道具套装效果" },
        { "ItemUICategory.csv", "道具分类" },
        { "JournalCategory.csv", "任务日志分类 (大)" },
        { "JournalGenre.csv", "任务日志分类 (小)" },
        { "JournalSection.csv", "任务日志大类 (最大)" },
        { "Leve.csv", "理符" },
        { "LeveAssignmentType.csv", "理符递交类型" },
        { "LeveClient.csv", "理符任务发布人" },
        { "LogFilter.csv", "消息筛选" },
        { "MainCommand.csv", "快捷指令" },
        { "MainCommandCategory.csv", "快捷命令类别" },
        { "Marker.csv", "目标标记" },
        { "McGuffinUIData.csv", "重要物品" },
        { "MinionRace.csv", "萌宠之王宠物类型" },
        { "MinionSkillType.csv", "萌宠之王宠物技能类型" },
        { "MJICraftworksObjectTheme.csv", "无人岛工坊物品类别" },
        { "MJIHudMode.csv", "无人岛行动模式" },
        { "MJIItemCategory.csv", "无人岛物品类别" },
        { "MJIName.csv", "无人岛地区名" },
        { "MonsterNote.csv", "讨伐笔记" },
        { "MYCTemporaryItemUICategory.csv", "失传技能库类别" },
        { "MYCWarResultNotebook.csv", "战果记录" },
        { "NotebookDivision.csv", "制作笔记分区" },
        { "NotebookDivisionCategory.csv", "制作笔记分类" },
        { "OnlineStatus.csv", "在线状态" },
        { "OpenContentCandidateName.csv", "异国的诗人" },
        { "OrchestrionCategory.csv", "乐谱分类" },
        { "Ornament.csv", "时尚配饰" },
        { "PetAction.csv", "召唤兽技能" },
        { "PetMirage.csv", "召唤兽投影" },
        { "PublicContent.csv", "永结同心" },
        { "QuestRewardOther.csv", "任务特殊奖励" },
        { "RacingChocoboNameCategory.csv", "竞赛陆行鸟名称" },
        { "RacingChocoboParam.csv", "竞赛陆行鸟属性" },
        { "RetainerTaskRandom.csv", "雇员探索委托" },
        { "SecretRecipeBook.csv", "秘籍" },
        { "SnipeTalkName.csv", "任务的狙击小游戏" },
        { "SpecialShop.csv", "特殊兑换物商店" },
        { "TopicSelect.csv", "特殊道具交易" },
        { "Town.csv", "雇员登记市场" },
        { "Trait.csv", "特性" },
        { "TripleTriadCardType.csv", "九宫幻卡类型" },
        { "TripleTriadCompetition.csv", "九宫幻卡大赛" },
        { "TripleTriadRule.csv", "九宫幻卡规则" },
        { "VVDNotebookContents.csv", "多变迷宫故事" },
        { "VVDNotebookSeries.csv", "多变迷宫系列" },
        { "Warp.csv", "副本外场景传送点" },
        { "AnimaWeapon5Param.csv", "魂武属性" },
        { "AnimaWeapon5PatternGroup.csv", "水晶砂交换品" },
        { "AquariumWater.csv", "水族箱环境" }
    };

    public static readonly Dictionary<string, string> OnlineLinks = new()
    {
        { "ClassJob.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ClassJob.csv" },
        { "Action.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Action.csv" },
        { "CraftAction.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/CraftAction.csv" },
        { "GeneralAction.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/GeneralAction.csv" },
        { "BaseParam.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/BaseParam.csv" },
        { "Item.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Item.csv" },
        { "PlaceName.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/PlaceName.csv" },
        { "Quest.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Quest.csv" },
        { "Mount.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Mount.csv" },
        { "Pet.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Pet.csv" },
        { "ENpcResident.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ENpcResident.csv" },
        { "World.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/World.csv" },
        { "Title.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Title.csv" },
        { "Achievement.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Achievement.csv" },
        { "ContentFinderCondition.csv", "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ContentFinderCondition.csv" }
    };
} 