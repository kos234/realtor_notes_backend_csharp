// using realtor_notes_backend.users.dtos;
// using realtor_notes_backend.users.model;
// using realtor_notes_backend.users.service;
//
// namespace realtor_notes_backend.users.repository;
//
// [Obsolete]
// public class MemoryGlobalRepository : ICodeRepository, ISessionRepository, ITwoFaRepository, IUserRepository, IUserSubscriptionRepository
// {
//     IDictionary<string, (CodeDTO, int)> codes = new Dictionary<string, (CodeDTO, int)>();
//     IDictionary<long, Session> sessions = new Dictionary<long, Session>();
//     IDictionary<int, (TwoFaDto, DateTimeOffset)> twoFa = new Dictionary<int, (TwoFaDto, DateTimeOffset)>();
//     IDictionary<int, User> users = new Dictionary<int, User>();
//     IDictionary<long, UserSubscription> userSubscriptions = new Dictionary<long, UserSubscription>();
//     
//     
//     public Task SaveCode(int userId, string label, string code, int duration)
//     {
//         codes[userId + label] = (new CodeDTO(code, DateTimeOffset.Now), duration);
//         return Task.CompletedTask;
//     }
//
//     public Task<CodeDTO?> GetCode(int userId, string label)
//     {
//         return Task.FromResult(codes.TryGetValue(userId + label, out var code) && DateTimeOffset.Now.Subtract(code.Item1.CreateAt).TotalSeconds < code.Item2 ? code.Item1 : null);
//     }
//
//     public Task DeleteCode(int userId, string label)
//     {
//         codes.Remove(userId + label);
//         return Task.CompletedTask;
//     }
//
//     public Task Save(Session session)
//     {
//         long id = sessions.Count == 0 ? 0 : sessions.Values.Max(x => x.Id) + 1;
//         session.Id = id;
//         sessions[session.Id] = session;
//         return Task.CompletedTask;
//     }
//
//     public Task<Session?> GetById(long id)
//     {
//         return Task.FromResult(sessions.TryGetValue(id, out var code) ? code : null);
//     }
//
//     public Task<Session?> GetByRefresh(string refresh)
//     {
//         var session = sessions.Values.FirstOrDefault(s => s.Refresh == refresh);
//         return Task.FromResult(sessions == null ? null : session);
//     }
//
//     public Task<Session?> GetByTrustDevice(int userId, string deviceId)
//     {
//         var session = sessions.Values.FirstOrDefault(s => s.UserId == userId && s.DeviceId == deviceId);
//         return Task.FromResult(sessions == null ? null : session);
//     }
//
//     public Task Delete(long id)
//     {
//         sessions.Remove(id);
//         return Task.CompletedTask;
//     }
//
//     public Task SaveTwoFaKey(int userId, TwoFaDto key, int ttl)
//     {
//         twoFa[userId] = (key, DateTimeOffset.Now.AddSeconds(ttl));
//         return Task.CompletedTask;
//     }
//
//     public Task DeleteTwoFaKey(int userId)
//     {
//         twoFa.Remove(userId);
//         return Task.CompletedTask;
//     }
//
//     public Task<TwoFaDto?> GetTwoFaKey(int userId)
//     {
//         return Task.FromResult(twoFa.TryGetValue(userId, out var code) && DateTimeOffset.Now.Subtract(code.Item2).TotalSeconds < 0 ? code.Item1 : null);
//     }
//
//     public Task Save(User user)
//     {
//         int id = users.Count == 0 ? 0 : users.Values.Max(x => x.Id) + 1;
//         user.Id = id;
//         users[user.Id] = user;
//         return Task.CompletedTask;
//     }
//
//     public Task<User?> GetUserById(int userId)
//     {
//         return Task.FromResult(users.TryGetValue(userId, out var code) ? code : null);
//     }
//
//     public Task<User?> GetUserByEmail(string email)
//     {
//         var user = users.Values.FirstOrDefault(s => s.Email == email);
//         return Task.FromResult(user == null ? null : user);    
//     }
//
//     public Task<User?> GetUserByLogin(string login)
//     {
//         var user = users.Values.FirstOrDefault(s => s.Login == login);
//         return Task.FromResult(user == null ? null : user);    
//     }
//
//     public Task<bool> IsUnavailableLogin(string login)
//     {
//         var user = users.Values.FirstOrDefault(s => s.Login == login);
//         return Task.FromResult(user != null);    
//     }
//
//     public Task<bool> IsUnavailableEmail(string email)
//     {
//         var user = users.Values.FirstOrDefault(s => s.Email == email);
//         return Task.FromResult(user != null);   
//     }
//
//     public Task<bool> IsUserHaveTrialSubscription(int userId)
//     {
//         var userSubscription = userSubscriptions.Values.FirstOrDefault(s => s.IsTrial && s.UserId == userId);
//         return Task.FromResult(userSubscription != null);
//     }
//
//     public Task<UserSubscription?> GetActiveSubscription(int userId)
//     {
//         var userSubscription = userSubscriptions.Values.FirstOrDefault(s => s.IsCanceled == false && s.UserId == userId && s.ExpirationDate >= DateTimeOffset.Now);
//         return Task.FromResult(userSubscription);
//     }
//
//     public Task Save(UserSubscription userSubscription)
//     {
//         long id = userSubscriptions.Count == 0 ? 0 : userSubscriptions.Values.Max(x => x.Id) + 1;
//         userSubscription.Id = id;
//         userSubscriptions[userSubscription.Id] = userSubscription;
//         return Task.CompletedTask;
//     }
// }