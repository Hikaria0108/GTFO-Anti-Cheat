# GTFO Anti-Cheat | GTFO 反作弊

## 警告
当前版本依然是早期版本可能存在任何问题，请谨慎使用

## 说明
本插件实现了对一些常规作弊手段的检测，并提供作弊通报、自动踢出/封禁作弊玩家、离线/在线封禁玩家黑白名单等功能

<font size=10>***如果需要兼容MTFO等插件先关闭配置文件中的环境检测***</font>

<font size=10>***请在使用MTFO时请在保证原始数据改变的情况下添加需要的数据而不是直接在原始数据上修改，以确保能够匹配到其他玩家的正常原始数据，避免误伤***</font>

## 现有主要功能
 - 在线玩家黑白名单
 - 强化剂数据检测
 - 武器模型数据检测
 - 武器伤害数据检测

## 想做的功能
 - 玩家后备数据检测
 - 枪械数据检测
 - 玩家数据检测
 - 以后再说
 
## 聊天窗口命令
 - /gac help 查看所有可用命令
 - /gac broadcast [on|off] 开启|关闭 玩家作弊通告
 - /gac autokick [on|off] 开启|关闭 自动踢出作弊玩家
 - /gac autoban [on|off] 开启|关闭 自动踢出并封禁作弊玩家
 - /gac unban [SteamID64] 手动解除本地玩家封禁
 - /gac detect booster [on|off] 开启|关闭 强化剂数据检测
 - /gac detect weaponmodel [on|off] 开启|关闭 武器模型数据检测
 - /gac detect weapondata [on|off] 开启|关闭 武器数据检测
