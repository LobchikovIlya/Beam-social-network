using Microsoft.AspNetCore.SignalR.Client;

namespace Beam.Client.BlazorWasm.Services;

public class SignalService
{
    private readonly HubConnection _hubConnection;

    public SignalService(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;


        _hubConnection.On<Guid, bool>("UsersStatusChanged", (userId, isOnline) =>
        {
            OnlineUsers[userId] = isOnline;
            OnUsersStatusChanged?.Invoke(userId, isOnline);
            UsersUpdated?.Invoke();
        });

        _hubConnection.On("UsersListUpdated",async () =>
        {
            Console.WriteLine("📡 Получено событие UsersListUpdated → обновляем список пользователей");
            if (OnUsersListUpdated != null) await OnUsersListUpdated.Invoke();
            UsersUpdated?.Invoke();
        });
        _hubConnection.On<Guid, string>("NotifyNewMessage", (fromUserId, lastMessageText) =>
        {
            Console.WriteLine($"Новое сообщение от {fromUserId}: {lastMessageText}");

            // Добавляем в список "ожидающих"
            if (!NewMessageUsers.Contains(fromUserId))
            {
               NewMessageUsers.Add(fromUserId);
                OnNewMessage?.Invoke(fromUserId, lastMessageText);
                UsersUpdated?.Invoke();
                
            }
        });
        _hubConnection.On<Guid, bool>("UserTyping", (userId, isTyping) =>
        {
            UsersTyping[userId] = isTyping;
            OnUsersTyping?.Invoke(userId, isTyping);
            OnUsersTypingChanged?.Invoke();
        });
    }
    public Dictionary<Guid, bool> UsersTyping { get; private set; } = new();
    public Dictionary<Guid, bool> OnlineUsers { get; private set; } = new();
    public HashSet<Guid> NewMessageUsers { get; private set; } = new();

    // Событие для компонентов
    public event Action<Guid, bool>? OnUsersStatusChanged;
    public event Func<Task>? OnUsersListUpdated;
    public event Action<Guid, string>? OnNewMessage; 
    public event Action? UsersUpdated;
    public event Action? OnUsersTypingChanged;
    public event Action<Guid, bool>? OnUsersTyping;

    public async Task StartAsync()
    {
        try
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
                await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при подключении SignalR: {ex.Message}");
        }
    }
    public async Task ReconnectAsync()
    {
        try
        {
            if (_hubConnection.State == HubConnectionState.Connected)
                await _hubConnection.StopAsync();

            await StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при переподключении SignalR: {ex.Message}");
        }
    }
    
    public async Task SendTypingStatus(Guid receiverId, bool isTyping)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
            await _hubConnection.SendAsync("NotifyTyping", receiverId, isTyping);
    }

    
    public void ClearNewMessage(Guid userId)
    {
        if (NewMessageUsers.Remove(userId))
            UsersUpdated?.Invoke();
    }
    
}

    
        
    
