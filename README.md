## Walk Through
##### 場景 1 (night scene)
 - [x] 場景物件
 - [ ] 場景氛圍
 - [x] 小地圖
 - [ ] 動態物件(怪物生成)
 - [x] 切關機制
 - [x] 各關卡的機制(封閉在小地圖空間)
 - [x] 各關卡的機制(小怪分空間)
 - [x] 引導方式(文字 or 光束)
 - [ ] 第一關
 - [ ] 第二關
 - [ ] 第三關
##### 場景 2 
 - [ ] 各關卡的機制(封閉在小地圖空間)、(小怪分空間)
 - [ ] 場景物件
 - [ ] 動態物件(怪物生成)
 - [ ] 初始物件(camera、directionlight、...)
 - [ ] 小地圖
## Combat System
#### Common
 - [x] 避免重疊
 - [x] 避免穿過靜態物件
 - [x] 生命計算
 - [x] 受傷特效
#### Main Character
###### 移動
 - [x] 攝影機 -- 基本跟隨
 - [x] 攝影機 -- 注視
 - [ ] 攝影機 -- 遮擋消除
 - [x] 基本移動 -- 動作混接
 - [x] 基本移動 -- 位移
 - [x] 戰鬥移動 -- 普攻移動
 - [x] 戰鬥移動 -- 翻滾
 - [x] 戰鬥移動 -- 衝刺
###### 戰鬥方式
 - [x] 受傷 -- 扣血
 - [x] 受傷 -- 輕傷動作
 - [x] 受傷 -- 重傷倒地動作
 - [x] 受傷 -- 重傷爬起動作
 - [x] 受傷 -- 重傷無敵時間
 - [x] 輕重派生技 -- 傷害計算
 - [x] 輕重派生技 -- 擊退或擊飛
 - [x] 希卡之石 -- 傷害計算
 - [x] 希卡之石 -- 擊退或擊飛
 - [x] 希卡之石 -- 附加屬性
 - [ ] 希卡之石 -- 弱點顯現
 - [ ] 希卡之石 -- 補血蘋果
 - [ ] 特殊戰鬥 -- 完美閃避
 - [ ] 特殊戰鬥 -- 終結技
 - [ ] 特殊戰鬥 -- 無雙

#### 小怪
###### Usao
 - [x] 待命 -- 動作
 - [x] 待命 -- 觸發戰鬥
 - [x] 戰鬥 -- 動作
 - [x] 戰鬥 -- 凝視
 - [x] 戰鬥 -- 發呆
 - [x] 戰鬥 -- 隨機咆哮
 - [x] 戰鬥 -- 觸發受傷
 - [x] 戰鬥 -- 觸發追擊
 - [x] 戰鬥 -- 觸發攻擊
 - [x] 追擊 -- 動作
 - [x] 追擊 -- 凝視
 - [x] 追擊 -- 靠近
 - [x] 追擊 -- 繞開障礙物
 - [x] 追擊 -- 避免跑出場地
 - [x] 追擊 -- 觸發受傷
 - [x] 追擊 -- 觸發戰鬥
 - [x] 攻擊 -- 動作
 - [x] 攻擊 -- 觸發受傷
 - [x] 受傷 -- 動作
 - [x] 受傷 -- 位移
 - [x] 受傷 -- 恢復
 - [x] 受傷 -- 觸發死亡
 - [x] 死亡 -- 動作
 - [x] 死亡 -- 特效
 - [x] 死亡 -- 消失
 - [ ] Usao2
 - [ ] Usao with sword
#### 大怪
 - [ ] 龍龍
 - [ ] 石頭人
 - [ ] P姊
 - [x] 血量顯示

## Scene
 - [ ] 起始畫面
 - [ ] 遊戲畫面
 - [ ] 結算畫面
 - [ ] 失敗畫面
 - [ ] 過場
## 代辦事項：
統整Ai 與碰撞機制與自由落體機制<br>
處理usao 、怪物資源池的問題<br>

## 本周任務
**主要角色戰鬥**
* 戰鬥系統 ( 扣血etc.)
* 主要角色戰鬥動作
* 必要的UI

## 相關規範
1. 引入新的素材之前，請務必在其他專案確認該素材引入不會造成額外的錯誤或衝突。

## 里程碑

###### 2022/12/25 (錦上添花內容完成)
###### 2022/12/18 (主要遊戲內容完成)
###### 2022/12/11 (戰鬥機制完成)
###### 2022/12/4
瑾瑜--<br>
采葳--<br>
恩榮--Usao Ai、 Ai 公用版本、雙向戰鬥系統、龍龍狀態機<br>
###### 2022/11/27

瑾瑜--調整攻擊動作、攻擊判定、主角戰鬥動作串接、小怪受擊僵直<br>
采葳--主角的劍閃爍的問題、柔焦要改成HDR、調整攻擊動作、希卡之石動作、丟出希卡道具(拋物線)、Ui<br>
恩榮--dither物件仍有陰影&整個物件一起消除、動物&靜物的碰撞判定、戰鬥系統架構、小怪戰鬥狀態串接、調整主角移動動作<br>

###### 2022/11/20 (地圖走透透完成)

瑾瑜--補完主角動作狀態(衝刺、迴避)、貼地行走<br>
采葳--研究魔王的材質球、關卡設計、小地圖、小怪、菁英怪的戰鬥動作串接<br>
恩榮--半透明處理(https://ronja-tutorials.com/post/042-dithering/)<br>

###### 2022/11/12

1. 清點素材、決定場景
2. 地圖走透透

瑾瑜--主角動作串接、主角攻擊動作串接<br>
采葳--關卡設計、場景建設、post renderer<br>
恩榮--攝影機、主角移動<br>

md 檔 的 live preview:
https://markdownlivepreview.com/