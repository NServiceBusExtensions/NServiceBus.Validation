class MyHandler :
    IHandleMessages<MyMessage>
{
    public Task Handle(MyMessage message, HandlerContext context)
    {
        Console.WriteLine("Hello from MyHandler. MyMessage");
        return Task.FromResult(0);
    }
}