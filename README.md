# 🍑Peach  -- A lightweight and fast socket communication library based on DotNetty

基于DotNetty的Socket通讯类库,可通过扩展来实现任意协议，内置实现一个简单的文本协议.`CommandLineProtocol`

# 使用

## 服务端

### 1. 实现MyService
可分别重写  
1. `OnConnected` 有客户端连接上的事件  
2. `OnDisConnected` 客户端断开连接时的事件
3. `OnReceive` 收到客户端消息的事件
4. `OnException` 发生异常时的事件，如异常断开

```
public class MyService : Peach.AbsSocketService<Peach.Messaging.CommandLineMessage>
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    public override void OnConnected(ISocketContext<CommandLineMessage> context)
    {
        _logger.LogInformation("client connected from {0}", context.RemoteEndPoint);
        base.OnConnected(context);
    }

    public override void OnDisconnected(ISocketContext<CommandLineMessage> context)
    {
        _logger.LogInformation("client disconnected from {0}", context.RemoteEndPoint);
        base.OnDisconnected(context);
    }

    public override void OnException(ISocketContext<CommandLineMessage> context, Exception ex)
    {
        _logger.LogError(ex,"client from {0}, occ error {1}", context.RemoteEndPoint,ex.Message);
        base.OnException(context, ex);
    }

    public override void OnReceive(ISocketContext<CommandLineMessage> context, CommandLineMessage msg)
    {
        string replyMessage = string.Empty;
        string replyCmd = string.Empty;
        switch (msg.Command)
        {
            case "echo":
                replyMessage = msg.Parameters[0];
                replyCmd = "echo";
                break;
            case "init":
                replyMessage = "ok";
                replyCmd = "init_reply";

                break;
            default:
                replyMessage = "error unknow command";
                break;
        }


        Task.Run(async () =>
        {
            await context.SendAsync(new CommandLineMessage(replyCmd, replyMessage));
        });
    }

}
```
### 2. 挂载服务

服务默认挂载在5566端口  

```
static void Main(string[] args)
{
    var builder = new HostBuilder()          
    .ConfigureServices((context,services) =>
    {
        //协议
        services.AddSingleton<IProtocol<CommandLineMessage>, CommandLineProtocol>();
        //挂载服务逻辑
        services.AddSingleton<ISocketService<CommandLineMessage>, MyService>();
        //添加挂载的宿主服务
        services.AddTcpServer<CommandLineMessage>();
    })
    .ConfigureLogging(
        logger =>
        {                   
            logger.AddConsole();
        }
    );
    builder.RunConsoleAsync().Wait();
}
```

## 客户端

### 1. 使用内置的TcpClient

监听接收消息和链接的消息，如下所示：
```
TcpClient<CommandLineMessage> client = new TcpClient<CommandLineMessage>(new CommandLineProtocol());
client.OnReceived += Client_OnReceived;
client.OnConnected += Client_OnConnected;

Task.Run(async () =>
{
    //连接服务器，可以链接多个哦
    var socketContext = await client.ConnectAsync(new IPEndPoint(Hey.IPUtility.GetLocalIntranetIP(), 5566));
    //发送消息
    var initCmd = new Hey.Messaging.CommandLineMessage("init");
    await socketContext.SendAsync(initCmd);
}).Wait();
```

可用的事件:
- `OnReceived` 当收到服务端消息时
- `OnError` 当通讯发生异常时
- `OnConnected` 当连接上服务器时
- `OnDisconnected` 当与服务端断开链接时
- `OnIdleState` 链接闲置时触发，一般在此事件中发送心跳包


## 扩展协议
Peach支持使用自定义协议，扩展协议需要自行实现两个接口，可参考https://github.com/dotbpe/dotbpe 框架中关于Amp协议的实现

### 1. IMessage 接口

实现类具体实现通讯消息的内容载体，只需实现如何获取消息长度的属性

```
public interface IMessage
{
    int Length { get;  }
}
```

### 2. IProtocol 接口

实现类需要描述消息头信息和具体打包解包逻辑，头信息描述参见`ProtocolMeta`字段描述


```
/// <summary>
/// 协议接口
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IProtocol<TMessage>
    where TMessage :  Messaging.IMessage
{
    ProtocolMeta GetProtocolMeta();

    TMessage Parse(Buffer.IBufferReader reader);

    void Pack(Buffer.IBufferWriter writer, TMessage message);

}
```