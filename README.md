## 为什么使用该工具类
在我们日常和服务器交互时.我们不可避免的会使用Protobuf,MessagePack等二进制传输格式,这些工具会完整的将类的内容序列化下来.这个工具接收两个参数(上一次发送服务器的类,现在需要发送的类),然后生成一个新的类型,这个类型只赋值了发生变化的变量.

## 差距
```json
{
  "DressUp": {
    "mBody": 1000008,
    "mDress": 0,
    "mShoes": 1030001,
    "mPants": 1010011,
    "mHair": 1020001
  },
  "LastScore": 0,
  "ArriveRestaurant": 1,
  "Level": {
    "1": 1000004
  },
  "InfiniteEnergy": -1,
  "Energy": 5,
  "MaxEnergy": 5,
  "ConsumeEnergy": -1,
  "Name": "Player",
  "Head": "ui://Settings/0",
  "Language": -1,
  "Pet": [],
  "UpdateTime": 1656323106,
  "LocalUpdateTime": 0,
  "ReceiveSystemChatReward": [],
  "WinningStreak": {
    "TimeStamp": 0,
    "Count": 0,
    "Level": 0
  },
  "Guide": [
    0,
    1,
    2,
    8,
    3,
    4,
    17,
    6,
    10,
    14,
    13,
    15,
    1002,
    18,
    900
  ],
  "Achievement": [],
  "GemAds": {
    "TimeStamp": 0,
    "Count": 0
  },
  "EnergyAds": {
    "TimeStamp": 1655117167,
    "Count": 2
  },
  "DoubleCoinAds": {
    "TimeStamp": 0,
    "Count": 0
  },
  "AccelerateShopAds": {
    "TimeStamp": 1654766114,
    "Count": 0
  },
  "CurRest": 1,
  "PerfectNumber": 81,
  "PlayedStory": 1000033,
  "IsSupportUserPrivacy": true,
  "IsReturn": false,
  "id": 0,
  "Version": null,
  "DevicesId": "085c9bd34e08166850ee85f41bb700ab67bdfce7"
}
```
使用Protobuf3.1.17进行序列化以上数据
|            | 反序列时间 | 内存 |
|------------|------------|------|
| Compress   | 0.1188ms   | 13b   |
| Uncompress | 0.0641ms   | 175b  |

## 该工具的性能如何
因为代码是提前生成并编译好的.所以整个调用过程都是友好的

## 有什么限制
该工具仅支持在Unity下使用,如果要在其他引擎下使用需要修改GenerateCode.cs,不过放心.代码量很少.很容易移植到其他引擎下

## 如何使用
```csharp
//继承自IPropertiesCompare接口的类会自动生成对比代码
public partial class Data_Homeland : Panthea.CompareDifferentProperties.IPropertiesCompare<Data_Homeland>
{
    //我们不需要向服务器发送这个变量,所以我们也不需要检查这个变量是否发生变化
    [IgnoreCompare]
    [JsonIgnore]
    public int a;
    public string b;
    public float c;
    public double d;
}
```
当编写完以上代码后点击Tools/生成协议对比代码
然后在你需要发送服务器协议之前调用
```csharp
var newMessage = Compare.GetDiff_Data_Homeland(Message,prevMessage);
```