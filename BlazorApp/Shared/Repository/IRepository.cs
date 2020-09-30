using System.Threading.Tasks;

namespace BlazorApp.Shared.Repository
{
    public interface IRepository<T> where T : class
    {
        Task Insert(T entity);
    }
}
