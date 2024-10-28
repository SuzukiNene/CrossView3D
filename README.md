[2024/10/27]  
WebGL版のビルドに対応し、GitHub Pagesに公開しました。  
I prepared WebGL build version on GitHub Pages.  
- https://suzukinene.github.io/CrossViewWeb/  
<br>

# CrossView
CrossViewは、ロボカップ サッカーシミュレーション(2D)のための、"3D"ログプレーヤーです。  
Unityで開発しました。  
CrossView is "3D" LogPlayer for RoboCup Soccer Simulation(2D), Made with Unity.  

![CrossView_Playing_001](https://github.com/user-attachments/assets/adbf0a3a-d6d2-4026-924a-5e0d0006f528)  

![CrossView_ThirdPerson_001](https://github.com/user-attachments/assets/7cf9d508-32cb-40f8-8854-1fd6c9bc5c80)  
<br>

## ロボカップ サッカーシミュレーション(2D)とは？ - About RoboCup Soccer Simulation(2D)

ロボカップ サッカーにおいて、ハードウェアを用いずにソフトウェアだけで競技を行うリーグです。  
One of the league in RoboCup Soccer where competitions are held using only software, without the use of hardware.

- Official Site  
https://ssim.robocup.org/  
- Official YouTube Channel  
https://www.youtube.com/@rcsoccersim

リーグ参加者は、各プレーヤーごとに、動作を制御するプログラムを1つ用意し、合計11個のプログラムをサッカーサーバーに接続させます。  
League participants will prepare one program for each player to control their actions, and connect a total of 11 programs to the soccer server.

サッカーサーバーから各プレーヤーに対しては、プレーヤーの視界の範囲内にある物体（ボールや他のプレーヤーなど）の情報が、センサー情報として定期的に与えられます。  
The soccer server periodically provides each player with sensor information about objects (such as the ball or other players) within the player's field of view.

プレーヤーは、それらの情報を元に、自身の行動を決定し、DashやKickといったコマンドをサーバーに送ります。  
また、体や首の向き、視界の角度を調整することも可能です。  
Based on this information, the player decides what to do and sends commands such as Dash or Kick to the server.  
Players can also adjust the direction of their Body and Neck, as well as the Angle of their field of view.  

すると、サーバーは、コマンドの実行内容が反映されたシミュレーション環境の情報を、再び、各プレーヤーに送ります。  
Then, the server sends each player information about the simulation environment, reflecting the execution of the commands.

ただし、サーバーは、現実世界の不確実性を考慮し、ランダムにノイズを混ぜた情報をプレーヤーに与えます。  
However, the server takes real-world uncertainty into account and gives players information that is randomly mixed with noise.

これにより、プレーヤーは、シミュレーション環境が必ずしも期待通りの反応を返さなくても、柔軟に行動できる能力が求められます。  
This requires players to be flexible and adaptable even when the simulated environment does not always respond as expected.

このように、各プレーヤーが"自律的"に意思決定を行う、マルチエージェントと呼ばれるソフトウェアを構築します。  
In this way, we build software called multi-agent, in which each player makes decisions "autonomously."

詳細は、公式マニュアルをご覧ください。  
For more information, see the official manual.  

- Official Manual  
https://rcsoccersim.github.io/  
https://rcsoccersim.readthedocs.io/en/latest/  
<br>

## ログファイル - Log File

CrossViewは、ロボカップの大会時に記録されたログファイルを読み込み、試合の様子を再現するためのソフトウェアです。  
CrossView is software that reads log files recorded during RoboCup competitions and reconstructs the state of the matches.

仕様 - Specification

- 対応するログファイルの拡張子は、"\*.rcg"です。  
（公式アーカイブのURLから開く場合は、"\*.rcg.gz"でも可）  
The file extension that can be loaded is "\*.rcg".  
(If you open it from the official archive URL, you can also use "\*.rcg.gz")  

- 対応するログファイルのバージョンは、Version 4～6（2008年以降）です。  
（ログファイルの先頭行が"ULG4"～"ULG6"のものです。）  
The supported log file versions are Version 4 to 6 (2008 and later).  
(The header lines of the log file are "ULG4" to "ULG6".)  

ログファイルは、以下のURLで公開されています。  
The log file is available at the following URL:  

- 公式アーカイブ - Official Archive  
http://archive.robocup.info/Soccer/Simulation/2D/logs/RoboCup/ 

ファイル選択ダイアログにて、ログファイルのファイルパス、または、公式アーカイブ上のURLを指定することが可能です。  
In the SelectFileDialog, you can specify the file path of the log file or the URL of the log file in the official archive site.  

OpenSiteボタンを押下すると、公式アーカイブのサイトを開くので、所望のログファイルのURLを手動でコピー&ペーストしてご利用ください。  
Clicking the OpenSite button will take you to the official archive site, where you can manually copy and paste the URL of the desired log file.  

![CrossView_SelectFile_001](https://github.com/user-attachments/assets/e3084d8f-6953-45a6-a049-90566a34479b)  
<br>

## なぜ、3Dなのか？ - Why 3D?

Q:サッカーサーバーには、付属のリアルタイムモニター兼ログプレーヤーが存在するのに、なぜ、わざわざ3Dのログプレーヤーを開発する必要があるのか？  
Q:Why do i need to develop a 3D LogPlayer when there is a real-time monitor and log player that comes with the soccer server?  

A:Soccer Simulation(2D)は、サッカーを2D環境の中でシミュレーションする競技ですが、競技の様子を"3D"で表現することで、いくつかメリットがあるのではないかと考えました。  
A:Soccer Simulation (2D) is a league in which soccer is simulated in a 2D environment, but i thought that there might be some benefits to expressing the game in "3D."  

1. キックやタックルなどのアクションが表現しやすく、視覚的にわかりやすい。  
Actions such as kicks and tackles are easy to express and visually understand.  
2. 各プレーヤーの視界を表現できる。（任意のプレーヤーの三人称視点の映像や、視界範囲をスポットライト等で表現するなど）  
It is possible to express each player's field of view (such as showing a third-person view of a player, or showing the field of view with spotlight).  
3. 単純に、見ていて楽しい。  
Quite simply, it's fun to watch.  

開発者のための分析ツールとして、お役に立てば嬉しいです。  
I hope that this will be useful as an analysis tool for developers.  

![CrossView_ViewAngle_001](https://github.com/user-attachments/assets/6b3294cc-2f71-4ea6-a09a-52752680819c)  

![CrossView_NightMode_001](https://github.com/user-attachments/assets/aa09d0f1-c9b0-40e5-bfc2-a57295d6e4c3)  
<br>

## 機能 - Function

画面上部  
Top of screen  

- 左右チームごとに、チーム名と得点を表示します。  
The team names and scores for each left and right team are displayed.  
- 得点時、ファウルやオフサイド発生時に、背景色が変わります。  
The background colour changes when a goal is scored, a foul is committed or an offside occurs.  

画面下部  
Bottom of screen  

- Disk button  
ログファイル(\*.rcg)を選択するダイアログを開きます。  
Opens a dialog for selecting a log file (\*.rcg).  
ファイルパス、またはURLを指定することで、ログファイルを読み込みます。  
Loads a log file by specifying the file path or URL.  
- PrevEvent button  
現在のPlayMode、もしくは１つ前のPlayModeの先頭サイクルに再生位置を移動します。  
Moves the playback position to the first cycle of the current PlayMode or the previous PlayMode.  
- Prev button  
１つ前のシミュレーションサイクルに再生位置を移動します。  
Moves the playback position to the previous simulation cycle.  
- Play/Pause button  
ログを再生/一時停止します。  
Play/pause the log.  
- Next button  
１つ後のシミュレーションサイクルに再生位置を移動します。  
Moves the playback position to the next simulation cycle.  
- NextEvent button  
１つ後のPlayModeの先頭サイクルに再生位置を移動します。  
Moves the playback position to the first cycle of the next PlayMode.  
- Speed Dropdown  
再生スピードを変更します。  
Change the playback speed.  
- Cycle Edit  
現在のシミュレーションサイクル位置を表示します。  
Displays the current simulation cycle position.  
一時停止中の場合は、任意のサイクル位置を入力してEnterキーを押すことで、指定位置にジャンプできます。  
When paused, you can jump to a specific cycle position by entering the desired cycle position and pressing the Enter key.  
- TotalCycle  
現在のログファイルにおける、最大サイクル数を表示します。  
Displays the maximum number of cycles in the current log file.  
最大サイクル数の位置まで再生が達すると、ログの再生が自動的に停止します。  
When the playback reaches the maximum number of cycles, the log playback will automatically stop.  
- PlayMode  
プレイ状態を表す文字列を表示します。（例："play_on", "goal_l", "foul_charge_r"）  
Displays a string indicating the play status. (e.g. "play_on", "goal_l", "foul_charge_r")  
内部エラーなどのシステムメッセージ("INFO")を表示する場合もあります。  
It may also display system messages ("INFO") such as internal errors.  
- ZoomOut button  
表示倍率を下げます。  
Decrease the display magnification.  
- ZoomIn button  
表示倍率を上げます。  
Increase the display magnification.  
- ViewMode button  
以下に示す３つのモードを切り替えます。  
Switch between the three modes shown below.  
	- 全体表示（サッカーフィールドを斜め上から見下ろした映像）  
Full view (a diagonal view of the soccer field)  
	- 全体表示（サッカーフィールドを真上から見下ろした映像）  
Full view (top view of soccer field)  
	- 個別表示（任意のプレーヤー視点から見た映像）  
Personal view(view from any player's point of view)  
		- プレーヤードロップダウンにより、プレーヤーを切り替えることができます。  
The player dropdown allows you to switch between players.  

- NightMode button  
プレーヤーの視界範囲（角度と距離）を、スポットライトで表現します。  
The player's field of view (angle and range) is represented by a spotlight.  
- PlayInfo button  
個別表示の画面上に、Informationとして選択プレーヤーの詳細情報を表示します。  
Detailed information about the selected player will be displayed as Information on the Personal view.  
- Setting button  
プレーヤーの3Dキャラクターを変更可能です。  
You can change the player's 3D character.  
ログファイルを読み込むと、選択したキャラクターでプレーヤーが生成されます。  
（読み込み後にキャラクターを変更しても、すぐには反映されません。）  
Loading the log file will spawn a player with the selected character.  
(If you change the character after loading, the changes will not be reflected immediately.)  

![CrossView_TopCamera_001](https://github.com/user-attachments/assets/0f158858-9878-4e73-a395-9f151f045128)  

![CrossView_ChangeSpeedAndJumpCycle_001](https://github.com/user-attachments/assets/cfa9835a-d292-4e10-853f-1177ce86c4f4)  

![CrossView_Settings_002](https://github.com/user-attachments/assets/0c70c58c-5862-425b-832d-3415841b52e0)  
<br>

## 補足 - Appendix

プレーヤーのアクション  
Player's action  
- アクションの状態を3Dキャラクターのアニメーションで表現します。  
The action state is expressed through 3D character animation.  
- Idle, Dash, Kick, Tackle, Catchの5種類のアニメーションを使います。（Catchはゴールキーパーのみ）  
Uses five types of animations: Idle, Dash, Kick, Tackle, and Catch. (Catch is only available to GoalKeeper.)  
ただし、アニメーションの実行とボールの移動タイミングが合わない場合があります。（制限事項）  
However, the timing of the animation and the ball movement may not match. (Limitations)  

プレーヤーの視界  
Player's field of view  
- 体の向きだけでなく、首振りも3Dキャラクターで表現します。  
Not only can the character's body turn, but it can also turn its neck.  
- また、3Dキャラクターの足元に、視界を表す扇形のオブジェクトを表示します。  
In addition, a fan-shaped object representing the field of view is displayed at the 3D character's feet.  
- 視界の角度に合わせて扇形の角度も変わります。  
The angle of the fan changes according to the angle of the field of view.  
- さらに、NightModeをONにすると、視界の角度と距離をスポットライトで表現できます。  
Additionally, when you turn on NightMode, the viewing angle and range can be represented by a spotlight.  
<br>

## 開発環境/テスト環境 - Development Environment/Test Environment

- CPU  
	- Intel Core i7-1065G7  
- Memory  
	- 16GB  
- GPU  
	- NVIDIA GeForce GTX 1650 with Max-Q Design  
- OS  
	- Windows10 Home 64-bit  
- Unity  
	- Unity 2020.3.48f1  
- Editor  
	- Visual Studio Community 2019  
<br>

## ソースコード - Source Code

- GitHub  
https://github.com/SuzukiNene/CrossView3D  

	```
	External Assetsフォルダ以下のファイルは同梱していません。  
	本Unityプロジェクトをビルドする場合は、手動で追加してください。  
	Files under the External Assets folder are not included.  
	If you want to build this Unity project, please add them manually.  
	```
<br>

## バイナリ - Binary

- GitHub  
https://github.com/SuzukiNene/CrossView3D/releases  
- WebGL version  
https://suzukinene.github.io/CrossViewWeb/  
- Microsoft Store  
(T.B.D)  
<br>

## 外部アセット - External Assets

- Unity Asset Store  
	- HYPERCASUAL - Simple Female & Male Characters  
		https://assetstore.unity.com/packages/3d/characters/hypercasual-simple-female-male-characters-209163  
	- Banana Man - Banana Yellow Games  
		https://assetstore.unity.com/packages/3d/characters/humanoids/banana-man-196830  
	- SpaceRobotKyle - UnityTechnologies  
		https://assetstore.unity.com/packages/3d/characters/robots/robot-kyle-urp-4696  
	- Goal - Easy Primitive People  
		https://assetstore.unity.com/packages/3d/characters/easy-primitive-people-161846  
	- SoccerBall_01 - FreeSportsKIt_SA  
		https://assetstore.unity.com/packages/3d/characters/free-sports-kit-239377  
	- SimpleNaturePack  
		https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153  
	- Bench_4 - Modern_Bench Pack  
		https://assetstore.unity.com/packages/3d/environments/modern-bench-pack-221011  
	- Flat Icons [Free] - Heathen Engineering  
		https://assetstore.unity.com/packages/2d/gui/icons/ux-flat-icons-free-202525  
	- Spinner - SimpleSpinner  
		https://assetstore.unity.com/packages/2d/gui/icons/simple-spinner-progress-indicators-for-ui-237500  

- Mixamo Animations  
	- X Bot@Breathing Idle.fbx  
	- X Bot@Jog Forward.fbx  
	- X Bot@Soccer Pass.fbx  
	- X Bot@Soccer Tackle.fbx  
	- X Bot@Goalkeeper Idle.fbx  
	- X Bot@Goalkeeper Pass.fbx  
	- X Bot@Goalkeeper Catch.fbx  
		- https://www.mixamo.com/#/?page=1&query=soccer  

- Download Package  
	- UnityStandaloneFileBrowser  
		- https://github.com/gkngkc/UnityStandaloneFileBrowser  
<br>

## ライセンス - License  

- Unity Asset Store  
	- https://unity.com/legal/as-terms  

- Adobe Mixamo  
	- https://www.adobe.com/legal/terms.html  
	- https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html  

- UnityStandaloneFileBrowser  
	- https://github.com/gkngkc/UnityStandaloneFileBrowser/blob/master/LICENSE.txt  
<br>


thank you.
