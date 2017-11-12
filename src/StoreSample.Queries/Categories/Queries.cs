using MediatR;

namespace StoreSample.Queries.Categories
{
    public class GetAll : IRequest<Category[]>
    {
    }
}
