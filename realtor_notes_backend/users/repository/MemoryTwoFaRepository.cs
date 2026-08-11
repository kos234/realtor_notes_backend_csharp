using Microsoft.Extensions.Caching.Memory;
using realtor_notes_backend.users.dtos;

namespace realtor_notes_backend.users.repository;

public class MemoryTwoFaRepository : ITwoFaRepository
{
    private readonly IMemoryCache _memoryCache;

    public MemoryTwoFaRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    public Task SaveTwoFaKey(int userId, TwoFaDto key, int ttl)
    {
        _memoryCache.Set(GetTwoFaCodeKey(userId), key, new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttl)
        });
        return Task.CompletedTask;
    }
    
    private string GetTwoFaCodeKey(int userId) => $"twofa_{userId}";
    
    public Task DeleteTwoFaKey(int userId)
    {
        _memoryCache.Remove(GetTwoFaCodeKey(userId));
        return Task.CompletedTask;
    }

    public Task<TwoFaDto?> GetTwoFaKey(int userId)
    {
        _memoryCache.TryGetValue(GetTwoFaCodeKey(userId), out TwoFaDto? dto);
        return Task.FromResult(dto);    
    }
}