using MediatR;

namespace StoreSample.Queries.CategoryAggregates
{
    public class GetAll : IRequest<CategoryAggregate[]>
    {
    }
}
