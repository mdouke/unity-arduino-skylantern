# Unity Arduino Sky Lantern

Arduinoのシリアル入力でスカイランタンの動きを制御するUnityデモです。

## Overview

このリポジトリは、チーム制作で作成したUnityプロジェクトを、就職活動・ポートフォリオ用に軽量化したものです。

3D空間内に配置されたスカイランタンが浮遊し、入力に応じて揺れや回転などの動きを変化させます。元のプロジェクトには背景、音源、外部素材なども含まれていましたが、この公開版では私の担当範囲が伝わる部分に絞っています。

## Screenshots

### Play Mode Scene View

Unity Play Modeで、ランタンの配置や動きの確認をしているSceneビューです。

![Play Mode Scene View](docs/images/play-mode-scene.png)

### Play Mode Game View

公開用に軽量化したプロジェクトのGameビューです。Arduinoを接続していない状態でも、キーボード入力でランタンの動作を確認できます。

![Play Mode Game View](docs/images/play-mode-game.png)

### Full Version Game View

チーム制作時の完全版プロジェクトのGameビューです。背景や演出素材を含む完成イメージとして掲載しています。

![Full Version Game View](docs/images/full-version-game.png)

## My Contributions

- ArduinoからUnityへシリアル通信で入力を受け取る処理を実装しました。
- 受信したシリアルデータをUnity側のスクリプトから参照できるように管理しました。
- ランタンの浮遊、ランダム移動、移動範囲制御、揺れ、回転などの動きを実装しました。
- Arduinoを接続していない環境でも動作確認できるよう、キーボード入力による代替操作を用意しました。

スカイランタンの3Dモデルは、別のチームメンバーが一から制作したものです。

## Controls

UnityのPlay Modeで以下のキーを押すと、Arduino入力を模擬してランタンの動きを確認できます。

- `3`: ランタンの動きに変化を加えます。
- `4`: ランタンを回転させます。

Arduino実機を接続していなくても、ランタンの動作デモは確認できます。

## Arduino Input

Unity側では、以下のようなカンマ区切りのシリアル入力を想定しています。

```text
0,0,1,0,0
```

Arduino側のコードはこのリポジトリには含まれていません。Arduinoが接続されていない場合、設定されたシリアルポートが利用できない旨のWarningがUnity Consoleに表示されます。

## Environment

- Unity `6000.0.25f1`
- シリアル通信: `System.IO.Ports`

## Main Scripts

- `Assets/Scripts/SerialHandler.cs`
  - シリアルポートを開き、別スレッドでArduinoからのデータを受信します。
- `Assets/Scripts/SerialManager.cs`
  - Arduinoから受信した最新メッセージを保持します。
- `Assets/Scripts/CombinedCubeMovement3D.cs`
  - ランタンを生成・アニメーションさせ、シリアル入力またはキーボード入力に応じて動きを変化させます。

## Notes

このリポジトリはポートフォリオ閲覧用です。元のチーム制作プロジェクトから、私の担当範囲と関係の薄い素材や外部アセットは除外しています。
