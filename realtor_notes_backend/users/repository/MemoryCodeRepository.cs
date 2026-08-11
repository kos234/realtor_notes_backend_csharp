using Microsoft.Extensions.Caching.Memory;
using realtor_notes_backend.users.dtos;

namespace realtor_notes_backend.users.repository;

public class MemoryCodeRepository : ICodeRepository
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCodeRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task SaveCode(int userId, string label, string code, int duration)
    {
        CodeDTO dto = new CodeDTO(code, DateTimeOffset.Now);
        
        _memoryCache.Set(GetEmailCodeKey(userId, label), dto, new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(duration)
        });
        return Task.CompletedTask;
    }

    private string GetEmailCodeKey(int userId, string label) => $"email_{userId}_{label}";

    public Task<CodeDTO?> GetCode(int userId, string label)
    {
        _memoryCache.TryGetValue(GetEmailCodeKey(userId, label), out CodeDTO? dto);
        return Task.FromResult(dto);
    }

    public Task DeleteCode(int userId, string label)
    {
        _memoryCache.Remove(GetEmailCodeKey(userId, label));
        return Task.CompletedTask;
    }
}